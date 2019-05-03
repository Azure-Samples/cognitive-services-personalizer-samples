# Personalizer Public Preview - NUget

In this folder you will find a nupkg file that is the Nuget package you can use while the nuget feed one gets deployed.

Prerequisites:
1. Have Visual studio or some tool capable of consuming nuget package feeds
1. Clone this repository by doing git clone


### Configuring the nuget source
To use this nuget file in Visual Studio:

1. Open Visual Studio
2. Go to Tools menu...Nuget Package Manager...Package Manager Settings
3. Click on the Package Sources tree
4. In that list, click the "+" button to add a source
5. Give it a name such as "Personalizer Temp Preview"
6. Point to the preview-nuget subfolder on your disk where this repository got cloned
7. Click OK