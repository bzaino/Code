REM Set HOMEPATH values
SET HOMEDRIVE=%CD:~0,2%
SET HOMEPATH=%CD:~3%
SET CWRSYNCHOME=%CD%
SET CWOLDPATH=%PATH%
SET PATH=%CWRSYNCHOME%\BIN;%PATH%
SET HOME=%HOMEDRIVE%\%HOMEPATH%

D:\cwRsync\bin\rsync.exe -avrz --exclude "index" --exclude "WEB-INF" "/cygdrive/D/Percussion/Deployment/Server/AssetsImagesapps/ROOT/" sv_replicate@ATSTWEB027::saltcm1

SET PATH=%CWOLDPATH%