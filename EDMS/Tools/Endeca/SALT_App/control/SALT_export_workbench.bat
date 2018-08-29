@echo off
rem Exports Experience Manager configuration from the Endeca Workbench Configuration Repository
rem Author: Andrew Kusz
rem 
call %~dp0..\config\script\set_environment.bat
if not defined ENDECA_ROOT (
  echo ERROR: ENDECA_ROOT is not set.
  exit /b 1
)

call %~dp0runcommand.bat ExportWorkbenchConfig