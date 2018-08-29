@echo off
call %~dp0..\config\script\set_environment.bat
echo Getting cartridge templates for %ENDECA_PROJECT_NAME%
emgr_update --host localhost:8006 --action get_templates --prefix "%ENDECA_PROJECT_NAME%" --app_name "%ENDECA_PROJECT_NAME%" --dir %~dp0..\config\cartridge_templates
