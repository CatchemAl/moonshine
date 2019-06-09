# To run tests against the code base, either run the test runner directly
# or, when in the root folder, run: python -m pytest
#
# To run the tests against the installed package, install the package with
#
#   pip install .                       for one off installations
#
#   pip install --editable .            for symbolic links development mode
#
# and then run py.test

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
