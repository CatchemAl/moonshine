# tests can be run using py.test
from moonshine.curves import discount_factor
from moonshine.instruments import price_cashflow


def test_discount_factor():
    '''A simple test.'''
    df = discount_factor(0.00, 10)
    expected = 1

    assert df == expected


def test_price_cashflow():
    '''A simple test.'''
    df = price_cashflow(100, 0.00, 10)
    expected = 100

    assert df == expected
