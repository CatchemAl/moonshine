import moonshine as ms
from moonshine.curves import discount_factor


def main():

    d = discount_factor(0.02, 20)
    price = ms.price_cashflow(10, 0.02, 10)

    print(price)
    print(d)


if __name__ == '__main__':
    main()




