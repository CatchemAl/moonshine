from setuptools import setup

# Package meta-data.
NAME = 'moonshine'
DESCRIPTION = 'My short description for my project.'
URL = r'https://github.com/CatchemAl/moonshine'
EMAIL = 'catchemal@foobar.com'
AUTHOR = 'Catchem AL'
REQUIRES_PYTHON = '>=3.7.0'
VERSION = '0.1'

REQUIRED = [
    # 'requests', 'maya', 'records',
]

setup(
    name=NAME,
    version=VERSION,
    description=DESCRIPTION,
    author=AUTHOR,
    author_email=EMAIL,
    packages=[NAME],  # same as name
    install_requires=REQUIRED,  # external packages as dependencies
)
