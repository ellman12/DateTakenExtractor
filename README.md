# DateTakenExtractor

DateTakenExtractor (DTE) is a small, fast, simple library for extracting Date Taken metadata from photos and videos, with the library consisting of only a single ```static``` C# class.

This library came into existence because two separate projects of mine used the same classes/packages for finding this data, and trying to keep those two files the same was annoying and difficult. I also wanted to redo the class used in those two projects to be smaller, simpler, and better.

This library uses [MetadataExtractor](https://github.com/drewnoakes/metadata-extractor-dotnet) for reading metadata from files, and is essentially a greatly simplified wrapper around it meant for reading exclusively Date Taken metadata.

## Installing DateTakenExtractor
The easiest way to use this library is via its [NuGet package](https://www.nuget.org/packages/DateTakenExtractor/).

Either add this to your project file
```xml
<ItemGroup>
    <PackageReference Include="DateTakenExtractor" Version="1.0.0"/>
</ItemGroup>
```

Or type this in Visual Studio's Package Manager Console:
```
PM> Install-Package DateTakenExtractor
```

Or search for `DateTakenExtractor` in the NuGet Package Manager in Visual Studio or JetBrains Rider.

## Using DateTakenExtractor
DateTakenExtractor is very simple to use. The class contains four public methods for your use.

```c#
//First checks the metadata, then the filename, for the Date Taken (DT) data.
DateTime? autoResult = DateTakenExtractor.GetDateTimeAuto("C:/yourfilehere.jpg", out DateTakenSrc dateTakenSrc);

//Checks only the metadata of the file for DT data.
DateTime? metadataResult = DateTakenExtractor.GetDateTakenFromMetadata("C:/yourfilehere.jpg");

//Checks only the filename of the file for DT data. Notice the timestamp pattern ↓ in the filename
DateTime? filenameResult = DateTakenExtractor.GetDateTakenFromFilename("C:/IMG_20210320_175909.jpg");

//Attempt to get DT data from both the metadata AND the filename.
DateTakenExtractor.GetDateTakenFromBoth("C:/IMG_20210320_175909.jpg", out DateTime? metadataDT, out DateTime? filenameDT);
```

Date Taken metadata can come from two locations: the file's actual internal metadata, or its filename. If a DTE method can't find metadata in the metadata or the filename, the return value/out parameter is set to `null`.

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