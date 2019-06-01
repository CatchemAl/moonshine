import matplotlib.pyplot as plt
import numpy as np

y = 5
x = 4
print(x+y)


# Create a list of evenly-spaced numbers over the range
x = np.linspace(0, 20, 100)
x
plt.plot(x, np.sin(x))       # Plot the sine of each x point
plt.show()
