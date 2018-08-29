@echo off
rem
rem Copyright (c) 2011 Endeca Technologies Inc. All rights reserved.
rem COMPANY CONFIDENTIAL
rem

call %~dp0..\config\script\set_environment.bat

if "%1" == "--force" (
  echo Removing existing application provisioning...
  call %~dp0runcommand.bat --remove-app
) 

call %~dp0runcommand.bat --skip-definition AssertNotDefined
if not %ERRORLEVEL%==0 (
  exit /b 1
)

echo Setting EAC provisioning and performing initial setup...
call %~dp0runcommand.bat InitialSetup
if not %ERRORLEVEL%==0 (
  exit /b 1
)
echo Finished updating EAC.

rem Import sample Application content
rem echo Importing sample content...
rem call %~dp0runcommand.bat IFCR importNode %~dp0..\config\ifcr
rem if not %ERRORLEVEL%==0 (
rem   exit /b 1
rem )
rem echo Finished importing sample content

echo Importing editors configuration...
call %~dp0set_editors_config.bat
if not %ERRORLEVEL%==0 (
  exit /b 1
)
echo Finished importing editors configuration

echo Importing media...
call %~dp0set_media.bat
if not %ERRORLEVEL%==0 (
  exit /b 1
)
echo Finished importing media

echo Importing templates...
call %~dp0set_templates.bat
if not %ERRORLEVEL%==0 (
  exit /b 1
)
echo Finished importing templates

