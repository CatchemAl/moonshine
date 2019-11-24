    public enum BufferedPipelineType
    {
        None = 0,
        Load = 1,
        Buffer1 = 2,
        Run = 3,
        Buffer2 = 4,
        Save = 5
    }

        public class PipelineLog
    {
        public PipelineLog(
            DateTime timestamp,
            string key,
            PipelineType pipelineType,
            bool isEntry)
        {
            Timestamp = timestamp;
            Key = key;
            PipelineType = pipelineType;
            IsEntry = isEntry;
        }

        public DateTime Timestamp { get; }

        public string Key { get; }

        public PipelineType PipelineType { get; }

        public bool IsEntry { get; }

        public override string ToString()
        {
            string flag = IsEntry ? "Entry" : "Exit";
            return $"{Timestamp:hh:mm:ss}: Key={Key}, Type={PipelineType} ({flag})";
        }
    }

    
    public class PipelineLogger
    {
        private readonly ConcurrentBag<PipelineLog> _pipelineLog = new ConcurrentBag<PipelineLog>();
        private readonly PipelineReporter _reporter;

        public PipelineLogger(PipelineReporter reporter)
        {
            _reporter = reporter;
        }

        public IReadOnlyList<PipelineLog> Logs => _pipelineLog.ToList().AsReadOnly();

        public void RecordEntry(string key, PipelineType pipelineType)
        {
            Record(key, pipelineType, true);
        }

        public void RecordExit(string key, PipelineType pipelineType)
        {
            Record(key, pipelineType, false);
        }

        public PipelineReport BuildReport()
        {
            return _reporter.BuildReport(Logs);
        }

        private void Record(string key, PipelineType pipelineType, bool isEntry)
        {
            var timestamp = GetTimestamp();
            var log = new PipelineLog(timestamp, key, pipelineType, isEntry);
            _pipelineLog.Add(log);
        }

        public static DateTime GetTimestamp()
        {
            return DateTime.UtcNow;
        }
    }


    public class PipelineReport
    {
        public PipelineReport(IEnumerable<PipelineRowLog> rowLogs)
        {
            Logs = rowLogs.OrderBy(row => row.StartDate)
                .ThenBy(row => row.Key)
                .ToList()
                .AsReadOnly();

            Types = rowLogs.SelectMany(r => r.TimeSpanByPipelineType.Keys)
                .Distinct()
                .OrderBy(pt => pt)
                .ToList()
                .AsReadOnly();
        }

        public ReadOnlyCollection<PipelineRowLog> Logs { get; }

        public ReadOnlyCollection<BufferedPipelineType> Types { get; }

        public PipelineReport Unbuffer()
        {
            var unbufferedLogs = Logs.Select(log => log.Unbuffer());
            return new PipelineReport(unbufferedLogs);
        }

        public override string ToString()
        {
            string[] columnHeaders = GetHeaders();
            Func<PipelineRowLog, object>[] selectors = GetSelectors();
            return Logs.ToStringTable(columnHeaders, selectors);
        }

        public string ToCsvString()
        {
            var sb = new StringBuilder();

            string[] columnHeaders = GetHeaders();
            string headerRow = string.Join(",", columnHeaders);
            sb.AppendLine(headerRow);

            Func<PipelineRowLog, object>[] selectors = GetSelectors();
            foreach (PipelineRowLog log in Logs)
            {
                var row = selectors.Select(s => s(log));
                string rowText = string.Join(",", row);
                sb.AppendLine(rowText);
            }

            return sb.ToString();
        }

        private string[] GetHeaders()
        {
            var columnHeaders = new List<string> { "Key", "Start" };
            var typeHeaders = Types.Select(pt => $"{pt}");
            return columnHeaders.Concat(typeHeaders).ToArray();
        }

        private Func<PipelineRowLog, object>[] GetSelectors()
        {
            var selectors = new List<Func<PipelineRowLog, object>>
            {
                log => log.Key,
                log =>  $"{log.StartDate:HH':'mm':'ss}"
            };

            var typeSelectors = Types.Select(pt =>
            {
                object Selector(PipelineRowLog row)
                {
                    TimeSpan timespan = row.TimeSpanByPipelineType[pt];
                    return FormatTimeSpan(timespan, pt);
                }

                return (Func<PipelineRowLog, object>)Selector;
            });

            return selectors.Concat(typeSelectors).ToArray();
        }

        private static string FormatTimeSpan(TimeSpan timeSpan, BufferedPipelineType pipelineType)
        {
            if (timeSpan == TimeSpan.MinValue)
            {
                return "-";
            }
            else if (timeSpan == TimeSpan.MaxValue)
            {
                return "DNF";
            }
            else if (timeSpan.TotalMilliseconds < 100 && pipelineType.IsBuffer())
            {
                return string.Empty;
            }
            else if (timeSpan.TotalSeconds > 60)
            {
                return timeSpan.ToString("m':'ss'.'f");
            }
            else
            {
                return timeSpan.ToString("s'.'f");
            }
        }
    }


    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class PipelineReporter
    {
        public PipelineReport BuildReport(IEnumerable<PipelineLog> logs)
        {
            var rowLogs = new List<PipelineRowLog>();
            foreach (var grp in logs.OrderBy(log => log.Timestamp).GroupBy(log => log.Key))
            {
                string key = grp.Key;
                var groupLogs = grp.ToList();

                PipelineRowLog rowLog = BuildKeyReport(key, groupLogs);
                rowLogs.Add(rowLog);
            }

            return new PipelineReport(rowLogs);
        }

        private PipelineRowLog BuildKeyReport(string key, ICollection<PipelineLog> logs)
        {
            var groupedLogs = logs.OrderBy(grp => grp.PipelineType)
                .ThenBy(grp => grp.Timestamp)
                .GroupBy(log => log.PipelineType)
                .ToArray();

            DateTime startTime = logs.Min(log => log.Timestamp);

            var timeByPipelineType = Enum.GetValues(typeof(BufferedPipelineType))
                .Cast<BufferedPipelineType>()
                .Except(new[] { BufferedPipelineType.None })
                .ToDictionary(e => e, _ => TimeSpan.MinValue);

            for (int i = 0; i < groupedLogs.Length; i++)
            {
                var typeGrouping = groupedLogs[i];
                BufferedPipelineType type = typeGrouping.Key.AsBuffered();
                PipelineLog[] startStopTimes = typeGrouping.ToArray();

                TimeSpan timespan;
                switch (startStopTimes.Length)
                {
                    case 1:
                        // The process did not finish
                        timespan = TimeSpan.MaxValue;
                        break;
                    case 2:
                        // The process completed
                        timespan = startStopTimes.Last().Timestamp - startStopTimes.First().Timestamp;
                        break;
                    default:
                        string message = $"Cannot report the {type} time for {key} as duplicate entries were detected.";
                        throw new InvalidOperationException(message);
                }

                timeByPipelineType[type] = timespan;

                if (startStopTimes.Length == 2 && i + 1 < groupedLogs.Length)
                {
                    var stopTime = startStopTimes.Last().Timestamp;
                    var nextLogs = groupedLogs[i + 1];
                    var nextStartTime = nextLogs.Min(log => log.Timestamp);
                    var bufferedTimeSpan = nextStartTime - stopTime;
                    var bufferedType = type.Increment();
                    timeByPipelineType[bufferedType] = bufferedTimeSpan;
                }
            }

            return new PipelineRowLog(startTime, key, timeByPipelineType);
        }
    }

    
    public class PipelineRowLog
    {
        public PipelineRowLog(DateTime startDate, string key, IReadOnlyDictionary<BufferedPipelineType, TimeSpan> timeSpanByPipelineType)
        {
            StartDate = startDate;
            Key = key;
            TimeSpanByPipelineType = timeSpanByPipelineType;
        }

        public DateTime StartDate { get; }

        public string Key { get; }

        public IReadOnlyDictionary<BufferedPipelineType, TimeSpan> TimeSpanByPipelineType { get; }

        public PipelineRowLog Unbuffer()
        {
            var unbufferedMap = TimeSpanByPipelineType
                .Where(kvp => kvp.Key.TryUnbuffer(out _))
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            return new PipelineRowLog(StartDate, Key, unbufferedMap);
        }
    }


    public enum PipelineType
    {
        None = 0,
        Load = 1,
        Run = 2,
        Save = 3
    }


    public static class PipelineTypeExtensions
    {
        public static BufferedPipelineType AsBuffered(this PipelineType type)
        {
            switch (type)
            {
                case PipelineType.Load:
                    return BufferedPipelineType.Load;
                case PipelineType.Run:
                    return BufferedPipelineType.Run;
                case PipelineType.Save:
                    return BufferedPipelineType.Save;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type));
            }
        }

        public static bool TryUnbuffer(this BufferedPipelineType bufferedType, out PipelineType type)
        {
            type = PipelineType.None;

            switch (bufferedType)
            {
                case BufferedPipelineType.Load:
                    type = PipelineType.Load;
                    return true;
                case BufferedPipelineType.Buffer1:
                    type = PipelineType.None;
                    return false;
                case BufferedPipelineType.Run:
                    type = PipelineType.Run;
                    return true;
                case BufferedPipelineType.Buffer2:
                    type = PipelineType.None;
                    return false;
                case BufferedPipelineType.Save:
                    type = PipelineType.Save;
                    return true;
                default:
                    throw new ArgumentOutOfRangeException(nameof(bufferedType));
            }
        }

        public static BufferedPipelineType Increment(this BufferedPipelineType type)
        {
            int value = (int)type + 1;
            return (BufferedPipelineType)value;
        }

        public static bool IsBuffer(this BufferedPipelineType type)
        {
            return type == BufferedPipelineType.Buffer1 || type == BufferedPipelineType.Buffer2;
        }

        public static PipelineType Increment(this PipelineType type)
        {
            int value = (int)type + 1;
            return (PipelineType)value;
        }
    }