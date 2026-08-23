Inputlog
========

Disclaimer
----------

This project has recently been published open source, and some parts might not yet fully support open source collaboration. Bear with us while we're updating and improving the software and documentation. (If you'd like to contribute, feel free to reach out to [us](https://www.inputlog.net/contact/))

Installation
------------

### Visual Studio
Development for Inputlog has been done using [Visual Studio](https://learn.microsoft.com/en-gb/visualstudio/releases/), but any IDE with support for C# should work. Make sure to have the following tools available on your system:

* [Git](http://www.git-scm.com/download)
* [.NET Framework 4.8](https://dotnet.microsoft.com/en-us/download/dotnet-framework)

The project can then be imported using `Git -> Clone Repository`

Project overview
----------------

### InputLog.Core
This project contains all code that is required in multiple other projects, such as the actual analyses and logging. Every other project depends on this core project.

### EmbeddedWord
This project provides all functionality for the `Play` tab. The code contains bugs and/or is incomplete for the time being (keep as-is).

### GUI
This project contains all code related to the interface of the Inputlog app. This is also the project that should be used as startup project when using Visual Studio.

### WebApp
*Under construction*

### Server
*Under construction*

### ServerConfig
*Under construction*

### InputlogInstaller
This project is no longer available as making the installer is now handled by external dedicated software (Advanced Installer).

More information
----------------

Additional documentation can be found in the following locations:

* Starter guide for users: [starterguide.pdf](Doc\StarterGuide\starterguide.pdf)
* Reference guide for developers: [referenceguide.pdf](Doc/ReferenceGuide/referenceguide.pdf)