#import moonshine as ms
#from moonshine.curves import discount_factor
from .curves import discount_factor
from .instruments import price_cashflow

def main():

    d = discount_factor(0.02, 20)
    price = price_cashflow(10, 0.02, 10)

    print(price)
    print(d)


if __name__ == '__main__':
    main()