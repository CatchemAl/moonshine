# Welcome to Moonshine!

Moonshine is an example repository that shows you how to set up a Python project. This notebook documents the basics of setting up Python, managing virtual environments, and structuring a project.

# Table of Contents
1. [Installation](#Installation)
2. [Setting up a virtual environment](#Setting-up-a-virtual-environment)
3. [Setting up VS Code](#Setting-up-VS-Code)


## Installation

1. Download [Anaconda](https://www.anaconda.com/distribution/).

2. If prompted, install VS Code. Otherwise, [install](https://code.visualstudio.com/download) it separately.

3. From the anaconda prompt, ensure everything is up to date: 
    `conda update conda python`. 

4. Next install pip `conda install pip` to use as a package manager

5. To work with JupyterLab, you will need to install Node JS so do that using `conda install -c conda-forge nodejs`.

6. Now you can install your packages through pip. Note that for large amounts of packages, it is normally easiest to install directly from a **requirements.txt** file such as the one below `pip install -r requirements.txt`:

```
aiohttp
autopep8
forex-python
ipywidgets
jupyterlab
matplotlib
matplotlib-venn
notebook
npm
numba
numpy
pandas
plotly
psycopg2
pylint
pytest
requests
scipy
seaborn
sqlalchemy
statsmodels
```

7. Finally, add the relevant JupyterLab extensions using  `jupyter labextension install @jupyter-widgets/jupyterlab-manager`



> Note that pipenv is generally recommended for package management these days. However, it does not work very nicely with Conda. Also, Conda is generally better than pip for package management but conda does not currently enable intellisense to work out of the box with VS Code.

## Setting up a virtual environment

It is a good idea to set up a virtual environment for each project. To do this in conda, you can use
`conda create -n tempenv pip python=3.7`.

Next, activate the newly created environment with
`conda activate tempenv` and install NodeJS with `conda install -c conda-forge nodejs`.

Finally, install the pip requirements and the JupyterLab widgets as before.

## Setting up VS Code

To launch VS Code from the Anaconda terminal, type `code`. This will launch Code with your conda environment already activated which is very helpful. However, it is not necessary to do this so long as you have properly set up your environment using pip.

In VS Code, you will want to install a number of extensions. The following are generally helpful:
 - Python
 - Visual Studio Keymap
 - PostgreSQL
 - Edit csv

Create a basic project with some files and tests so you can configure your VS Code appropriately. You will want to configure:
 - The virtual environment settings
 - Your launch/debug settings
 - Your unit testing framekwork (PyTest)

First select the interpreter using `Ctrl+Shift+P` and 'Select Interpreter'. Select the interpreter appropriate to your Conda virtual environment. This should automatically create a settings.json file in a .vscode folder.

By creating a test module, e.g. test_foo.py with a method test_bar, VS Code should automatically detect that this is a test file and add additional configurations for unit tests:

```json
{
    "python.testing.pyTestArgs": [
        "tests"
    ],
    "python.testing.unittestEnabled": false,
    "python.testing.nosetestsEnabled": false,
    "python.testing.pyTestEnabled": true,
    "python.pythonPath": "C:\\Users\\alexj\\AppData\\Local\\Continuum\\anaconda3\\envs\\py37\\python.exe"
}
```

Finally, you will want to create a launch.json file to save your launch settings. To do this, `Ctrl+Shift+D` to open the Debug panel, click the dropdown box and Add Configuration... It is a good idea to have a number of configurations such as current file and main entry point:

```json
{
    // Use IntelliSense to learn about possible attributes.
    // Hover to view descriptions of existing attributes.
    // For more information, visit: https://go.microsoft.com/fwlink/?linkid=830387
    "version": "0.2.0",
    "configurations": [
        {
            "name": "Python: Current File",
            "type": "python",
            "request": "launch",
            "program": "${file}",
            "console": "integratedTerminal"
        },
        {
            "name": "Python: Current File (Console)",
            "type": "python",
            "request": "launch",
            "program": "${file}",
            "console": "externalTerminal"
        },
        {
            "name": "Python: Main",
            "type": "python",
            "request": "launch",
            "program": "${workspaceFolder}/src/main.py",
            "console": "integratedTerminal"
        }
    ]
}
```