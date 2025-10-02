using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Resources;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("")]
[assembly: AssemblyDescription("Core library for InputLog")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("University of Antwerp")]
[assembly: AssemblyProduct("InputLog")]
[assembly: AssemblyCopyright("Copyright © University Of Antwerp 2025")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
[assembly: CLSCompliant(false)]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a LinearAnalysisType in this assembly from 
// COM, set the ComVisible attribute to true on that LinearAnalysisType.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("080133bc-f5c0-4838-8e21-f0ada1152c39")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers 
// by using the '*' as shown below:
// [assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyVersion("1.0.0.0")]
[assembly: AssemblyFileVersion("9.6.0.0")]
// Log4Net configuration
[assembly: log4net.Config.XmlConfigurator(ConfigFile = "Log4Net.config", Watch = true)]
[assembly: NeutralResourcesLanguageAttribute("en-US")]
