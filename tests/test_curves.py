from moonshine.curves import get_discount_factor
from moonshine.instruments import price_cashflow

# To run tests against the code base, either run the test runner directly
# or, when in the root folder, run: python -m pytest
#
# To run the tests against the installed package, install the package with
#
#       pip install .                       for one off installations
#
#       pip install --editable .            for symbolic links development mode
#
# and then run py.test
#
# To later remove the editable install do one of two things:
#
#       pip freeze > tmp.txt
#       - edit the file to remove everything but the develop build
#       pip uninstall -r tmp.txt
#
#
#       remove from site-packages\easy_install.pth
#       delete moonshine.egg-link file


def test_discount_factor() -> None:
    """A simple test."""
    discount_factor = get_discount_factor(0.00, 10)
    expected = 1




    assert discount_factor == expected


def test_price_cashflow() -> None:
    """A simple test."""
    price = price_cashflow(100, 0.00, 10)
    expected = 100

    assert price == expected
