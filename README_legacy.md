Inputlog
========

Installatie
-----------

### Git
Inputlog gebruikt [Git](http://www.git-scm.com/download) als versie controlesysteem. Volg de instructies op [Atlassian Documentation](https://confluence.atlassian.com/bitbucket/set-up-a-repository-877174034.html) om het repository te installeren.

### Visual Studio
Voor de ontwikkeling van Inputlog is [Visual Studio 2015 - Community Edition](https://www.visualstudio.com/en-us/news/releasenotes/vs2015-update3-vs) vereist. Zorg bij de installatie van Visual Studio dat volgende componenten mee geïnstalleerd worden:

* Visual C#
* Visual Web Developer
* .NET Framework 4.5
* Office Developer Tools
* VS Installer Project Extension (Using the Visual Studio Extension Manager)

### Projecten signen
Volgende projecten dienen eerst gesigned te worden voordat je ze kan builden:

* EmbeddedWord
* GUI
* InputLog.Core

Open hiervoor het `Signing` tabblad in de `Properties` van elk project. Klik op de `Change Password...` knop en voer 3 maal hetzelfde wachtwoord in. Dit wachtwoord vind je terug in het document met wachtwoorden en URLs.9

Project overzicht
-----------------

### InputLog.Core
Dit project bevat alle basisfunctionaliteit die gedeeld wordt door de andere projecten. Hiertoe behoren onder andere de Analyses, IO functionaliteit, de eigenlijke Logging, en een hoop diverse Utilities die kort beschreven zijn in de InputLog Reference Guide. Ieder ander project steunt op de InputLog Core.

### EmbeddedWord
Lowlevel project dat de functionaliteit achter de `Play` tab van InputLog voorziet. Het project zelf is solide, het linken van het output OCX bestand loopt regelmatig mis om onduidelijke redenen. Best om hier geen veranderingen te maken.

### GUI
Output project dat de normale InputLog GUI genereert. Met deze GUI lopen ook regelmatig dingen mis in verband met uitlijning van elementen, zeker in geval van resolutiewijziging of uitrekken van de GUI. Aanraken op eigen risico. Verder bevat dit project ook klasses die voor de functionaliteit achter de tabbladen zorgen, zoals de Recorder, de verschillende Analyzers en Preprocessors, enz. Maakt voor de merging in de PreProcess tab gebruik van een ingenieus Wizard-systeem dat beschreven wordt in de Reference Guide, courtesy of Tom Pauwaert.

### LiteInterface
WPF project voor de InputLog lite versie. De Lite versie voorziet een zo eenvoudig mogelijke interface die enkel logging ondersteunt (en backups maakt van het oorspronkelijke document). De resultante IDFX bestanden worden geupload naar de Inputlog server. Het proces om een Inputlog Lite onderzoeksproject op te starten is gedefinieerd in de reference guide.

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

Meer informatie
---------------

* Starter guide: `Doc\StarterGuide\starterguide.pdf`
* Reference guide: `Doc\ReferenceGuide\referenceguide.pdf`