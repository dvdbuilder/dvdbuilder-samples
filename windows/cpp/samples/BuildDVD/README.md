## BuildDVD

The BuildDVD sample shows how you can use the DVDBuilder API to create a DVD file structure from a DVDBuilder project file. 

### Command Line

	 .\BuildDVD.exe <project_file> <output_dir>
 
###	Examples

The following example creates a video DVD of a spinning cube:
	
	.\BuildDVD.exe ..\sample-projects\Cube\project.xml CubeDVD

***

	Windows PowerShell
	Copyright (C) 2014 Microsoft Corporation. All rights reserved.
	
	PS DVDBuilder.CPP\lib> .\BuildDVD.exe ..\sample-projects\Cube\project.xml CubeDVD
	
	Status: WritingVOB
	Progress: 100
	Status: WritingIFO
	Progress: 100
	Success
	PS DVDBuilder.CPP\lib>	
