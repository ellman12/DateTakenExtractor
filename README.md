# DateTakenExtractor

[![DateTakenExtractor NuGet version](https://img.shields.io/nuget/v/DateTakenExtractor)](https://www.nuget.org/packages/DateTakenExtractor/)
[![DateTakenExtractor Nuget download count](https://img.shields.io/nuget/dt/DateTakenExtractor)](https://www.nuget.org/packages/DateTakenExtractor/)

DateTakenExtractor (DTE) is a small, fast, simple library for reading and writing Date Taken metadata for photos and videos, with the library consisting of only a single ```static``` C# class.

DTE is essentially a simplified wrapper around [MetadataExtractor](https://github.com/drewnoakes/metadata-extractor-dotnet), designed to make it easier for me to read and write this data in my [two](https://github.com/ellman12/Photos-Storage-Server/) [projects](https://github.com/ellman12/Graphical-Photo-Organizer) that use it. DTE also provides an abstraction around [ExifTool](https://exiftool.org/), an excellent command line tool which I use to read certain types of metadata, as well as write/update Date Taken metadata.

## Installing DateTakenExtractor
The easiest way to use this library is via its [NuGet package](https://www.nuget.org/packages/DateTakenExtractor/).

Either add this to your project file
```xml
<ItemGroup>
    <PackageReference Include="DateTakenExtractor" Version="1.2.0"/>
</ItemGroup>
```

Or type this in Visual Studio's Package Manager Console:
```
PM> Install-Package DateTakenExtractor
```

Or search for `DateTakenExtractor` in the NuGet Package Manager in Visual Studio or JetBrains Rider.

**DTE also REQUIRES [ExifTool](https://exiftool.org/).**
<br>To install ExifTool, download the .exe, rename it from `exiftool(-k).exe` to `exiftool.exe`, and add it to your `PATH` or move the .exe to a directory already in the `PATH`, like `C:/Windows`.

## Using DateTakenExtractor
DateTakenExtractor is very simple to use. The class contains several public methods for your use.

```c#
using D = DateTakenExtractor.DateTakenExtractor;

//These methods can take either file paths as strings, or FileStreams.
//First checks the metadata, then the filename, for the Date Taken (DT) data. dateTakenSrc would either be 'Metadata', 'Filename', or 'None'.
DateTime? autoResult = D.GetDateTakenAuto("C:/yourfilehere.jpg", out DateTakenSrc dateTakenSrc);

//Checks only the metadata of the file for DT data. null if none found.
DateTime? metadataResult = D.GetDateTakenFromMetadata("C:/yourfilehere.jpg");

//Checks only the filename of the file for DT data. Notice the timestamp pattern ↓ in the filename. null if none found.
DateTime? filenameResult = D.GetDateTakenFromFilename("C:/IMG_20210320_175909.jpg");

//Attempt to get DT data from both the metadata AND the filename.
D.GetDateTakenFromBoth("C:/IMG_20210320_175909.jpg", out DateTime? metadataDT, out DateTime? filenameDT);

//New in V1.1: updating Date Taken metadata! This works for .jpg, .png, .mp4, and .mov files.
//.gif and .mkv files are iffy since they're not really meant to contain this kind of data.
D.UpdateDateTaken("C:/IMG_20210320_175909.jpg", new DateTime(2020, 6, 9, 12, 30, 0));
```

Date Taken metadata can come from two locations: the file's actual internal metadata, or its filename. If a DTE method can't find the Date Taken in the metadata or the filename, the return value/out parameter is set to `null`.

### Supported file types:
* JPG
* PNG
* GIF
* MP4
* MOV
* MKV

## Contributing to DateTakenExtractor
To contribute to DateTakenExtractor, follow these steps:

1. Fork this repository.
2. Create a branch: `git checkout -b <branch_name>`.
3. Make your changes and commit them: `git commit -m '<commit_message>'`
4. Push to the original branch: `git push origin DateTakenExtractor/<location>`
5. Create the pull request.

Alternatively see the GitHub documentation on [creating a pull request](https://help.github.com/en/github/collaborating-with-issues-and-pull-requests/creating-a-pull-request).

Feel free to either email me or contribute if you spot a bug 🐛 or have a feature idea 💡.

## Contact
If you want to contact me you can reach me at ellduc4@gmail.com