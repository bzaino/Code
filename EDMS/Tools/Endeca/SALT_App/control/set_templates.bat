@echo off
call %~dp0..\config\script\set_environment.bat
echo Removing existing cartridge templates for %ENDECA_PROJECT_NAME%
call emgr_update --host localhost:8006 --app_name "%ENDECA_PROJECT_NAME%" --action remove_templates
if not %ERRORLEVEL%==0 (
  exit /b 1
)
echo Setting new cartridge templates for %ENDECA_PROJECT_NAME%
call emgr_update --host localhost:8006 --app_name "%ENDECA_PROJECT_NAME%" --action set_templates --dir %~dp0..\config\cartridge_templates
if not %ERRORLEVEL%==0 (
  exit /b 1
)
echo Finished setting templates
