from .curves import discount_factor

def price_cashflow(nominal: float, rate: float, tenor: float):
    return nominal * discount_factor(rate, tenor)


if __name__ == "__main__":
    price = price_cashflow(100, 0.02, 10)
    print(price)