::	Build_SOA	SOA Build Script
::		Allows use of Environment Variables or a predefined batch file to define the build parameters
::	BuildModes:		Release and/or Debug; List of build modes to perform
::	BuildGroups:		Framework Shared BE DAO Services; list of possible build components (in proper order)
::	Services_to_Build:	ContactValidation Person PersonManagement Loan LoanManagement ReferenceData (almost any order)
::	VSBldCmd:		Build or Rebuild, Build allows incremental builds based on prior results
::					The default is Rebuild

::	If nothing set, the defaults are Debug and Release builds for
::		All Services
::	If low level components are desired, they must be specified

::	Change dir to build folder
pushd %~dp0

::	Always use stored script to override envvars
::	11/17/2011	ALWAYS try to do VS2010 builds
@If Not Defined SOA_Build_ProjectParams set SOA_Build_ProjectParams=SOA_BuildParamsVS2010.bat


::	9/22/2011
::	Allow multiple param files to control multiple build steps
@For %%P in (%SOA_Build_ProjectParams%) DO Call :DO_JustOne_Build %%P
@popd
@GOTO :EOF



::	----------------------------------------------------------------------
:DO_JustOne_Build
::	Must have parameters, else FAIL
@If NOT Exist %1 GOTO :EOF

::	Create local env for this build
@setlocal

::	Create the environment
@Call %1


@IF NOT Defined Call_Installer Set Call_Installer=@REM


::	Now set common defaults if no values set


::	Define BuildModes as an Envar to just build debug or release mode
@If Not defined BuildModes @Set BuildModes=Release Debug

::	Define VSBldCmd as an Envar to control the IDE: Build or Rebuild
@If Not defined VSBldCmd @Set VSBldCmd=Rebuild


::	Buildgroups lists the groups that NEED to be built for this project
::	This is the FULL list in the mandatory order, you should NOT need all of them
::	Set BuildGroups=Framework Shared BE DAO Services SvcAdapter MCD
::	E.G. for a typical service build: set BuildGroups=BE DAO Services

::	Services_to_Build lists the Services that NEED to be built for this project


::	This is the Visual Studio version, it can be set on a project specific basis
@If NOT Defined SOAVS Set SOAVS=2010


::	----------	Setup Files for VS Version	----------------------------------------
::	Oct 2011
::call "!vs%SOAVS%call!"--"This is the Back up"
If %SOAVS%==2010 Call "%vs2010call%" else call "!vs%SOAVS%call!"

Set BUILDNAME=%~n0_%SOAVS%
Set InstallNAME=Install
Set BUILDLOG1=%BUILDNAME%_LOG.txt
:: DEL %BUILDLOG1% /S

@set outline=!BuildGroups:Services=%Services_to_Build%!
@call :go_echo %date%
@call :go_echo Building: %OutLine% in [%BuildModes%] Mode(s)

::	Insure all writes can overwrite existing components
Attrib -R *.* /s


For %%a in (%BuildModes%) DO Call :OneType %%a


::	Clean the environment for next build
@endlocal

@GOTO :EOF



::	----------	Debug or Release Mode	----------------------------------------
:OneType

Set BldType=%1

Set CONFIG=%1
Set BUILDLOG= %BUILDNAME%_%1_log.txt
Set InstallLOG=%InstallNAME%_%1_log.txt
DEL %BUILDLOG% /S

::	04/12/2010	Need to set a valid devpath for Host projects to build
::		use a local setting, otherwise can't do rebuilds
	
@setlocal
::	Desktops write this into the registry to save it
@ call SOA_SetDevpath.bat %bldtype% %CD%

@call :go_echo Start %bldType% mode

For %%b in (%BuildGroups%) DO Call :Build_%%b

@call :go_echo Ended %bldType% mode

::	Build Machines discard the value of devpath
@endlocal
@goto :eof


::	--------------------------------------------------------------------------------
::	----------	Service Framework	----------------------------------------
::	--------------------------------------------------------------------------------

:Build_Framework
@call :go_echo Building FrameWork solution
devenv ".\InternalComponents\Framework\ASA.Framework\ASA.Framework.sln" /%VSBldCmd% %CONFIG%>> %BUILDLOG%

::	Call EDMS_Install.bat ".\InternalComponents\Framework\ASA.Framework" %CONFIG% >> %InstallLOG%

@goto :eof
::	--------------------------------------------------------------------------------


::	--------------------------------------------------------------------------------
::	----------	Shared Business Entities AND DataAccess		----------------
::	----------	Framework Error Handling and WCFExtensions	----------------
::	--------------------------------------------------------------------------------

:Build_Shared
@call :go_echo Building SHARED Business Entities...
devenv ".\Database\BusinessEntities\ASA.BusinessEntities_Shared.sln" /%VSBldCmd% %CONFIG%>> %BUILDLOG%

@call :go_echo Building SHARED data access objects ...
devenv ".\Database\DataAccess\ASA.DataAccess_Shared.sln" /%VSBldCmd% %CONFIG%>> %BUILDLOG%

@call :go_echo Building Exception Handling solution
devenv ".\InternalComponents\Framework\ASA.ErrorHandling\ASA.ErrorHandling.sln" /%VSBldCmd% %CONFIG%>> %BUILDLOG%

@call :go_echo Building Security solution
devenv ".\InternalComponents\Framework\ASA.Security\ASA.Security.sln" /%VSBldCmd% %CONFIG%>> %BUILDLOG%

@call :go_echo Building WCF Extensions solution
devenv ".\InternalComponents\Framework\ASA.WCFExtensions\ASA.WCFExtensions.sln" /%VSBldCmd% %CONFIG%>> %BUILDLOG%

::@call :go_echo Building FrameWork Deployment
::devenv ".\InternalComponents\Framework\ASA.Framework\ASA.Framework.Setup\ASA.Framework.Setup.sln" /%VSBldCmd% %CONFIG%>> %BUILDLOG%

@goto :eof
::	--------------------------------------------------------------------------------



::	--------------------------------------------------------------------------------
::	----------	DB Specific Business Entities	----------------------------------------
::	--------------------------------------------------------------------------------

:Build_BE
@call :go_echo Building Business Entities...

devenv ".\Database\BusinessEntities\ASA.BusinessEntities.sln" /%VSBldCmd% %CONFIG%>> %BUILDLOG%

%Call_Installer% ".\Database\BusinessEntities" %CONFIG% >> %InstallLOG%

@goto :eof
::	--------------------------------------------------------------------------------


::	--------------------------------------------------------------------------------
::	----------	DB Specific Data Access Objects	----------------------------------------
::	--------------------------------------------------------------------------------

:Build_DAO

@call :go_echo Building data access objects ...
devenv ".\Database\DataAccess\ASA.DataAccess.sln" /%VSBldCmd% %CONFIG%>> %BUILDLOG%

%Call_Installer% ".\Database\DataAccess" %CONFIG% >> %InstallLOG%

@goto :eof
::	--------------------------------------------------------------------------------


::	--------------------------------------------------------------------------------
::	----------	:Build_Services		----------------------------------------
::	----------	Driver for Services	----------------------------------------
::	--------------------------------------------------------------------------------

:Build_Services
@call :go_echo Begin Services 
For %%S in ( %Services_to_Build% ) Do Call :One_Service %%S

@call :go_echo End Services 
@goto :eof


::	--------------------------------------------------------------------------------
::	----------	One_Service		----------------------------------------
::	----------	Process and Build one service at a time	------------------------
::	--------------------------------------------------------------------------------

:One_Service
@call :go_echo Building %1 ...

@Set ThisProjDir="%cd%\SOA\Services\%1"

devenv ".\SOA\Services\%1\ASA.Services.%1.sln" /%VSBldCmd% %CONFIG%>> %BUILDLOG%

::Set TRG=.\Tests\Bin\TestReportGenerator.exe

::IF /I "%BldType%" EQU "Debug" IF Exist %TRG% %TRG% 1 .\SOA\Services\%1

%Call_Installer% %ThisProjDir% %CONFIG% >> %InstallLOG% 

@goto :eof

::	--------------------------------------------------------------------------------
::	----------	Service Adapter		----------------------------------------
::	--------------------------------------------------------------------------------

:Build_SvcAdapter
@call :go_echo Building Service Adapter solution
devenv ".\SOA\ESB\ServiceAdapter\ServiceAdapter.sln" /%VSBldCmd% %CONFIG%>> %BUILDLOG%

@goto :eof


::	--------------------------------------------------------------------------------
::	----------	MCD			----------------------------------------
::	--------------------------------------------------------------------------------

:Build_MCD
@call :go_echo Building FrameWork solution
devenv ".\Services\MCDClean\MCDClean.sln" /%VSBldCmd% %CONFIG%>> %BUILDLOG%

@goto :eof


::	--------------------------------------------------------------------------------
::	----------	:Build_Web		----------------------------------------
::	----------	Driver for Web          ----------------------------------------
::	--------------------------------------------------------------------------------

:Build_Web
@call :go_echo Begin Web


devenv ".\SOA\Web\SAL\ASA.Web.Services.sln" /%VSBldCmd% %CONFIG%>> %BUILDLOG%
devenv ".\EDMS\ASA.Web\Sites\SALT\ASA.Web.Sites.SALT.sln" /%VSBldCmd% %CONFIG%>> %BUILDLOG%
devenv ".\EDMS\Tools\SchoolListTransformer\SchoolListTransformer.sln" /%VSBldCmd% %CONFIG%>> %BUILDLOG%
devenv ".\SOA\Web\Lessons\ASALessons.sln" /%VSBldCmd% %CONFIG%>> %BUILDLOG%

:: devenv ".\SOA\Web\WTF\ASA.Web.WTF.sln" /%VSBldCmd% %CONFIG%>> %BUILDLOG%
:: devenv ".\SOA\Web\Onboarding\ASA.Web.Onboarding.sln " /%VSBldCmd% %CONFIG%>> %BUILDLOG%
:: devenv ".\SOA\Web\WipWebSolution\WipWebSolution.sln" /%VSBldCmd% %CONFIG%>> %BUILDLOG%
::	Call EDMS_Install.bat ".\SOA\Web" %CONFIG% >> %InstallLOG% 


@call :go_echo End Web
@goto :eof

::	--------------------------------------------------------------------------------
::	----------	:Build_MRM		----------------------------------------
::	----------	Driver for MRM          ----------------------------------------
::	--------------------------------------------------------------------------------
:Build_MRM
@call :go_echo Begin MRM

devenv ".\SOA\MRM\Adapters\ASA.MRM.Adapters.sln" /%VSBldCmd% %CONFIG%>> %BUILDLOG%
:: devenv ".\SOA\MRM\Person\ASA.MRM.Services.Person.sln" /%VSBldCmd% %CONFIG%>> %BUILDLOG%
:: devenv ".\SOA\MRM\Token\ASA.MRM.Services.Token.sln" /%VSBldCmd% %CONFIG%>> %BUILDLOG%


@call :go_echo End MRM
@goto :eof
::	--------------------------------------------------------------------------------
::	----------	SilverpopAdapter        ----------------------------------------

:Build_SilverpopAdapter
@call :go_echo Building SilverpopAdapter
devenv ".\SOA\ESB\SilverpopAdapter\SilverpopAdapter.sln" /%VSBldCmd% %CONFIG%>> %BUILDLOG%

@goto :eof


::	--------------------------------------------------------------------------------
:GO_Echo
@echo	 %Time% %*
@echo	 %Time% %* >> %BUILDLOG1%
@goto :eof
