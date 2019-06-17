from setuptools import setup, find_packages

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
    packages=find_packages('src'),
    package_dir={'': 'src'},
    install_requires=REQUIRED,  # external packages as dependencies
    entry_points = {
		'console_scripts': [ 
			'getpr = moonshine.__main__:main',
		],
	},
)
