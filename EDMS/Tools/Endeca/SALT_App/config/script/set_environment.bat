@echo off

rem
rem Endeca Environment Variables
rem

rem Application/deployment variables

rem ENDECA_PROJECT_DIR specifies the path of the deployed application
rem e.g. set ENDECA_PROJECT_DIR=c:\apps\myapp
rem project dir is two up from this directory \config\script
set ENDECA_PROJECT_DIR=%~dp0..\..

rem ENDECA_PROJECT_NAME specifies the project name that will be used, for
rem example, as the JCD job prefix for jobs defined in the project's 
rem Job Control Daemon (JCD).
rem e.g. set ENDECA_PROJECT_NAME=myapp
set ENDECA_PROJECT_NAME=SALT


rem NOTE: Endeca software variables are set during installation of the software.
rem This template assumes that the variable ENDECA_ROOT is set and paths
rem under ENDECA_ROOT have been added to the PERLLIB, PERL5LIB and PATH
rem environment variables.
