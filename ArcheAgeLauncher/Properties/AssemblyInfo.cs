using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;

// Manage the General information about the Assembly is carried out using 
// set of attributes. Change the values of these attributes to change the information,
// associated with an Assembly.
[assembly: AssemblyTitle("ArcheAge Launcher")]
[assembly: AssemblyDescription("Launcher For ArcheAge Client")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("ArcheAge Launcher")]
[assembly: AssemblyCopyright("Copyright © netcastiel 2013")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to FALSE makes the types in the Assembly invisible 
// for COM components.  If you want to access the type in this Assembly through 
// COM, set the ComVisible attribute to TRUE for that type.
[assembly: ComVisible(false)]

//Button to start building a localized application, set the 
//<UICulture>CultureYouAreCodingWith< / UICulture> in the file .csproj
////inside <Property Group>.  For example, if you use US English
//in your source files, set <UICulture > to en-US.  Then cancel the conversion to comment
// the NeutralResourceLanguage attribute below.  Update "en-US" to
//the line at the bottom to ensure that the UICulture setting in the project file is consistent.

//[assembly: NeutralResourcesLanguage("en-US", UltimateResourceFallbackLocation.Satellite)]


[assembly: ThemeInfo(
    ResourceDictionaryLocation.None, //where the resource dictionaries in specific subject areas
                                     //(used if the resource is not found on the page 
                                     // or in application resource dictionaries)
    ResourceDictionaryLocation.SourceAssembly //where the universal resource dictionary is located
                                              //(used if the resource is not found on the page, 
                                              // in the app or in any resource dictionaries for a specific topic)
)]

// Assembly version information consists of the following four values:
//
// Major version number
// Minor version number 
// Build number
// Editorial
//
// You can set all values or accept the build number and revision number by default, 
// using " * " as shown below:
// [assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyVersion("2.0.*")]
[assembly: AssemblyFileVersion("2.0.0")]