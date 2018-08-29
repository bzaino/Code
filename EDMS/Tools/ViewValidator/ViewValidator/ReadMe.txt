A)
View Validator got to where it exists as of 12/22/2011 by having what amounts the "four lives". It started with the first life (Checking for Orphaned Tags) and 
each successive feature was added to that. This explains why the program has certain features cleaved onto existing code. This has created a problem in that 
many functions are called hundred or even tems of thousands of times. The three lives were:
1) Checking Razor and XML files for Orphaned XML Tags and Orphaned Razor Tags
2) Checking the Razor files for hard coded XML content
3) Extracting the URL's from Razor and XML files
4) Attempting to resolve the URL's extracted during phase III

B) The first column in the CSV output files has the heading "#". This column contains the number of times View Validator generated this exact error.

C) To create a file containing the folder tree of PublishedContent or the View folder use this DOS command:

    // Use this DOS command to create the folder tree: >dir /s /b /ad "c:\temp\folder" > list.txt
    // Use this DOS command to create the folder tree: >dir /s /b /ad "<source folder>" > <target file>

Then copy the results into PublishedContent_ActiveFolders.txt or RazorViews.txt in the FolderTrees folder

D) To find exactly where an exception is being thrown, see the instructions in the catch block in ViewValidator.cs
