#Tauco
  
Provides an easy way to obtain DICOM data from the servers. For DICOM operations utilizes the [Fellow Oak DICOM for .NET library](https://github.com/fo-dicom/fo-dicom).

----------

##Features
* Targets .NET version 4.6 and higher
* Convenient API for basic DICOM operations (fetching patiens and related images)
* Fully asynchronous using async/await pattern
* Built-in caching
* Integrated logging
* Console application for usage outside the .NET environment
* integrated Dicom server for downloading the images 
 

## Limitations
* Only predefined attributes are loaded for DICOM objects
* Patient IDs are parsed to match czech/slovak birth numbers. If parsing fails, no ID is loaded.
* Only read-only operations are permitted

## Examples
Library uses Castle Windsor for building dependency tree.
```csharp
using Castle.Windsor;
using Tauco.Dicom;

var patientsProvider = container.Resolver<IPatientInfoProvider>();
var studyProvider = container.Resolver<IStudyInfoProvider>();
var seriesProvider = container.Resolver<ISeriesInfoProvider>();

```
### Obtaining all patients
####From server
```csharp
var patients = await patientProvider.GetPatients().ToListAsync();
```
####From cache
```csharp
var patients = await patientProvider.GetPatients().LoadFromCache().ToListAsync();
```
###Obtaining single patient
```csharp
var patient = await patientProvider.GetPatientByBirthNumberAsync("9107256444");
```
> **Note:**

> - Both version of birth number with and without the slash can be used.
###Downloading patient images
Following code will download all patient's images to default folder (*Dicom/&lt;Patient name>/&lt;Study UID>*)
```csharp
await patientProvider.DownloadImagesAsync(patient);
```

The path can be customized by providing optional parameter:
```csharp
await patientProvider.DownloadImagesAsync(patient, (studyUid, instanceUid) =>
            {
                var path = Path.GetFullPath(@".\DICOM");
                path = Path.Combine(path, studyUid);

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                return File.Create(Path.Combine(path, instanceUid) + ".dcm");
            });
```
Files can be downloaded for individual studies too:
```csharp
await studyProvider.DownloadImagesAsync(study);
```
##Caching
All server responses are cache by default. Cache files are stored in *Cache/&lt;Object type>/...*. Because Dicom data usually cannot be modified, using of cache whenever possible is recommended.

## Configuration
###Server
Both remote and local server can be configured via the app.config file. Following app keys are expected to be present in configuration section:
```xml
<appSettings>
	<add key="CallingApplicationEntity" value="" />
	<add key="DestinationApplicationEntity" value="" />
	<add key="CalledApplicationEntity" value="" />
	<add key="RemoteAddress" value="" />
	<add key="RemotePort" value="" />
	<add key="LocalAddress" value="" />
	<add key="LocalPort" value="" />
</appSettings>
```
> **Note:**

> - Destination AE will be used by the remote server when downloading the images. This key has to be recognized by Dicom server together with the local address and port; otherwise downloading of files will not work.

###Logging
Logging file location and verbosity can be changed in **Nlog.config** file shipped with source/binaries.
