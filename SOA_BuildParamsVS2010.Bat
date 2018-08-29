::	SOA_BuildParams.Bat
::		Sets the Environment Variables to control the SOA build

::	*****	*****	*****	*****	*****	*****	*****
::	DO NOT PROMOTE THIS FILE INTO ANY _SCRUM Stream
::	*****	*****	*****	*****	*****	*****	*****

::	build parameters
::	BuildModes:		Release and/or Debug; List of build modes to perform Defaults to BOTH
::	BuildGroups:		MRM SilverpopAdapter; list of possible build components
::	Services_to_Build:	ContactValidation Person PersonManagement Loan LoanManagement ReferenceData
::	VsBldCmd:		Build or Rebuild, Build allows incremental builds based on prior results
::					The default is Rebuild

::	MRM represents All MRM Components: MRMPerson, MRMToken and MRMAdapters

::	If nothing set, the defaults are Debug and Release builds for
::		All MRM Components and SilverpopAdapter

	Set Services_to_Build= ReferenceData ContactValidation Person Loan Organization PersonManagement LoanManagement
::	Set Services_to_Build= ReferenceData ContactValidation Person Loan Organization PersonManagement LoanManagement

::	VS2010 BUILDS
::	Set BuildGroups=SilverpopAdapter
::	Set BuildGroups=Framework Shared BE DAO Services MRM SvcAdapter SilverpopAdapter
::	Set BuildGroups=Framework Shared BE DAO Services MRM SvcAdapter
::	Set BuildGroups=Framework Shared BE DAO Services MRM 
::	Set BuildGroups=Framework Shared BE DAO Services
::	Set BuildGroups=Framework Shared BE DAO 

::	Set BuildGroups=Framework Shared MRM Web
	Set BuildGroups=Framework Web

::	Set BuildGroups=BE DAO Services
::	Set BuildGroups=BE DAO
::	Set BuildGroups=Services

::	Full builds for Release Engineering, comment out for other streams
::	NOTE:	MRM solutions can only be done in VS2010 or a dual build process
:: 	Set BuildGroups=Framework Shared BE DAO Services SvcAdapter MCD MRM

::	Both modes is the default in the Build script, change only if needed
	Set BuildModes=Debug
::	Set BuildModes=Release

::	Allow incremental builds by uncommenting the following line
::	Set VsBldCmd=Build

::	11/02/2011	Set in Params file for split builds
@Set SOAVS=2010
