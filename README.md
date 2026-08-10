Inputlog
========

Installation
-----------

### Visual Studio
Development for Inputlog has been done using [Visual Studio](https://learn.microsoft.com/en-gb/visualstudio/releases/), but any IDE with support for C# should work. Make sure to have the following tools available on your system:

* [Git](http://www.git-scm.com/download)
* [.NET Framework 4.8](https://dotnet.microsoft.com/en-us/download/dotnet-framework)

The project can then be imported using `Git -> Clone Repository`

Project overview
-----------------

### InputLog.Core
This project contains all code that is required in multiple other projects, such as the actual analyses and logging. Every other project depends on this core project.

### EmbeddedWord
This project provides all functionality for the `Play` tab. This project contains bugs and/or is incomplete for the time being (keep as-is).

### GUI
This project contains all code related to the interface of the Inputlog app. This is also the project that should be used as startup project when using Visual Studio.

### WebApp
MVC ASP project dat de administratie van de Inputlog Server regelt. Accounts aanmaken, mails sturen, overzicht voor de gebruiker en tevens een administratie gedeelte voor de beheerders. Projecten worden gestart via een HTTP formulier waar gebruikers de URL niet van weten, dit formulier wordt achter de schermen door de Client ingevuld. Projecten worden dan in de database ingeschreven en de schijf geplaatst waar de Server deze verder afhandelt. Precieze gang van zaken is beschreven in de reference guide.

### Server
Project dat Inputlog taken afhandelt op de server, in verschillende threads. Resultaten worden op de server bewaard en opgehaald door de GUI of manueel door de gebruiker langs de WebApp.

### ServerConfig
Simpele administratietool voor InputLog zaken op de server. Dient met een adminaccount geconfigureerd te worden en kan vervolgens periodiek opruimen en mails versturen (scheduling door Windows).

### TestCore
[NUnit](http://www.nunit.org/) tests voor het InputLog.Core project.

### TestGUI
NUnit tests voor het GUI project.

### InputlogInstaller
Project voor de Inputlog installer.

More information
----------------

Additional documentation can be found in the following locations:

* Starter guide for users: [starterguide.pdf](Doc\StarterGuide\starterguide.pdf)
* Reference guide for developers: [referenceguide.pdf](Doc/ReferenceGuide/referenceguide.pdf)