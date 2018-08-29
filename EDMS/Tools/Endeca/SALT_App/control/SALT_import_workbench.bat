@echo off
rem Imports Pages, Content, User Segments, Redirects, and Thesaurus entries into the ECR for use in Experience Manager
rem Author: Andrew Kusz
rem 

call %~dp0..\config\script\set_environment.bat
if not defined ENDECA_ROOT (
  echo ERROR: ENDECA_ROOT is not set.
  exit /b 1
)

echo Importing userSegments...
call %~dp0runcommand.bat --skip-definition IFCR importNode %~dp0..\config\WorkbenchExport\userSegments userSegments

echo Importing content...
call %~dp0runcommand.bat --skip-definition IFCR importNode %~dp0..\config\WorkbenchExport\content content

echo Importing pages...
call %~dp0runcommand.bat --skip-definition IFCR importNode %~dp0..\config\WorkbenchExport\pages pages

rem echo Importing thesaurus...
rem call %~dp0runcommand.bat --skip-definition IFCR importNode %~dp0..\config\WorkbenchExport\thesaurus thesaurus

rem echo Importing redirects...
rem call %~dp0runcommand.bat --skip-definition IFCR importNode %~dp0..\config\WorkbenchExport\redirects redirects
