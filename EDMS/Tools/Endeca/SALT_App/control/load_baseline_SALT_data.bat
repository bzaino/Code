@echo off
rem
rem Copyright (c) 2006 Endeca Technologies Inc. All rights reserved.
rem COMPANY CONFIDENTIAL
rem

call %~dp0..\config\script\set_environment.bat
xcopy %ENDECA_PROJECT_DIR%\SALT_data\baseline\Content\* %ENDECA_PROJECT_DIR%\data\incoming\Content\* /S /Y
xcopy \\amsa.com\Process\ABS\MFTA\@env@\Endeca\ContentStatistics\* %ENDECA_PROJECT_DIR%\data\incoming\ThirdPartyData\* /S /Y
call %~dp0set_baseline_data_ready_flag.bat

