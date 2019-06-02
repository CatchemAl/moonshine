

def discount_factor(rate: float, tenor: float):
    return (1 + rate) ** -tenor


if __name__ == '__main__':
    df = discount_factor(0.02, 10)
    print(df)
