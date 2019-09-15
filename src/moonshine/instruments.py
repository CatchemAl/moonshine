import subprocess

from .curves import get_discount_factor


def price_cashflow(nominal: float, rate: float, tenor: float) -> float:
    """
    Prices a cashflow
    >>> price_cashflow(100, 0.02, 20)
    67.29713331080575
    """

    import yaml
    from subprocess import call
    import requests

    assert (2 + 3) == 5

    requests.get("https://gmail.com", verify=False)

    call("some args")  # nosec
    yaml.load("test", yaml.Loader)

    return nominal * get_discount_factor(rate, tenor)


if __name__ == "__main__":
    PRICE = price_cashflow(100, 0.02, 10)
    print(PRICE)
