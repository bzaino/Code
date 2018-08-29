--/////////////////////////////////////////////////////////////////////////
--//  WorkFile Name:      xxSCRIPT_NAMExx --NM you will need multiple scripts.  Change the RTA number from xxxxx. Just add _1,_2, etc. to end of script name.
--//  WorkFile Path:      /NBSDatabase/Cleanup_Scripts_Scripts/MRM/
--//  Create By:          Amanda Nantais
--//  Create Date:        09/11/13
--//    
--//  This cleanup script will insert records into asa_client_web_activity 
--//		to reflect individuals targeted in the Monthly Newsletter mass communication mailing.
--//    
--//  ASA Proprietary Information
--/////////////////////////////////////////////////////////////////////////
--- Revision:   $Revision: 3 $

SET NOCOUNT ON
PRINT 'Start running xxSCRIPT_NAMExx on SQL Server ' + @@SERVERNAME + ' ' + + DB_NAME() + ' database '
PRINT CONVERT(VARCHAR, GETDATE(), 120)
GO
--------------------------------------------------------------
---- Set the script name and organization here
DECLARE @SQL_Script_Name		VARCHAR(120)
DECLARE @Process_id				VARCHAR(16)
DECLARE @Database_Name			VARCHAR(25)
DECLARE @End_dt					DATETIME

SET @SQL_Script_Name			= 'xxSCRIPT_NAMExx'
SET @Process_id					= 'RTA_xxRTA_NUMxx'
SET @Database_Name				= 'MRM'
--------------------------------------------------------------
-- Validate script properties
IF LEN(@SQL_Script_Name) > 60
BEGIN
	PRINT ''
	PRINT 'The length of SQL Script name ' + @SQL_Script_Name + ' is too long, it shall be no longer than 60 characters'
	PRINT ''
  	RETURN 
END
--------------------------------------------------------------
-- Validate if the database is correct
IF DB_NAME() NOT LIKE @Database_Name + '%'
BEGIN
	PRINT ''
	PRINT 'ERROR:  This script should be run in ' + @Database_Name + ' database'
	PRINT ''
	RETURN
END
--------------------------------------------------------------
--Check if the script has already run
IF EXISTS (SELECT 'x' FROM CMN_DBUpdateHistory (NOLOCK) WHERE Object_nm = @SQL_Script_Name AND event_Type_cd = 'CLEANUP' AND Remarks like 'COMPLETED%')
BEGIN
    PRINT ''
    PRINT 'The script has been run in this database already.'
    PRINT ''
    RETURN
END
--------------------------------------------------------------
--Initialize CMN_DBUpdateHistory
DECLARE @DBUpdateHistory_cd varchar(16)

EXEC cmn_GetUpdateHistoryKey @DBUpdateHistory_cd output
EXEC cmn_DBHistoryInsert @DBUpdateHistory_cd, NULL, NULL, NULL, NULL, NULL,@SQL_Script_Name, 'SCRIPT','CLEANUP', @@SERVERNAME, 'START', 0
--------------------------------------------------------------
--Declare Variables for Inserts/Updates
DECLARE	@a13_a12_code			varchar(60),
		@a13_send_email_flag	tinyint,
		@a13_add_user			varchar(64),
		@a13_add_date			datetime,
		@a13_delete_flag		tinyint,
		@a13_a16_code			varchar(64),
		@a13_communication_code varchar(126)
SELECT	@a13_a12_code			=	'Mass Communication',
		@a13_send_email_flag	=	0,
		@a13_add_user			=	@SQL_Script_Name,
		@a13_add_date			=	GETDATE(),
		@a13_delete_flag		=	0,
		@a13_a16_code			=	'Activation',
		@a13_communication_code =	'ACT_20130911'
-----------------------------------------------------
--Create Temp Table for List of ind_cst_keys provided by the business
SELECT	ind_cst_key
INTO	#TMP_Individuals
FROM	co_individual (NOLOCK)
WHERE	ind_cst_key IN (xxID_LISTxx)
-----------------------------------------------------
BEGIN TRANSACTION 
--Insert into client_asa_web_activity
	INSERT INTO [dbo].[client_asa_web_activity]
			   ([a13_ind_cst_key]
			   ,[a13_a12_code]
			   ,[a13_send_email_flag]
			   ,[a13_add_user]
			   ,[a13_add_date]
			   ,[a13_delete_flag]
			   ,[a13_a16_code]
			   ,[a13_communication_code])
	           
	SELECT		ind_cst_key
			   ,@a13_a12_code as 'a13_a12_code'
			   ,@a13_send_email_flag as 'a13_send_email_flag'
			   ,@a13_add_user as 'a13_add_user'
			   ,@a13_add_date as 'a13_add_date'
			   ,@a13_delete_flag as 'a13_delete_flag'
			   ,@a13_a16_code as 'a13_a16_code'
			   ,@a13_communication_code as 'a13_communication_code'
	FROM		#TMP_Individuals (NOLOCK)

	PRINT STR(@@ROWCOUNT) + ' records inserted into the client_asa_web_activity table'
	PRINT ''

-- Update co_individual_ext
	UPDATE [dbo].[co_individual_ext]
	SET		 ind_activation_email_sent_flag_ext = CASE WHEN @a13_a16_code = 'Activation' THEN 1 ELSE ind_activation_email_sent_flag_ext END
			,ind_last_mass_communication_type_ext = @a13_a16_code 
			,ind_last_mass_communication_code_ext = @a13_communication_code
	WHERE	ind_cst_key_ext in (SELECT ind_cst_key FROM	#TMP_Individuals (NOLOCK))

	PRINT STR(@@ROWCOUNT) + ' records updated in the co_individual_ext table'
	PRINT ''

-- Update co_individual
	UPDATE [dbo].[co_individual]
	SET		 ind_change_date = @a13_add_date
			,ind_change_user = @a13_add_user
	WHERE	ind_cst_key in (SELECT ind_cst_key FROM	#TMP_Individuals (NOLOCK))

	PRINT STR(@@ROWCOUNT) + ' records updated in the co_individual table'
	PRINT ''
COMMIT TRANSACTION 

DROP TABLE #TMP_Individuals
--------------------- END ------------------------------------
SELECT @End_dt = GETDATE()
DECLARE @Remark varchar(500)
SELECT @Remark = 'COMPLETED'
EXEC dbo.cmn_DBHistoryUpdate @DBUpdateHistory_cd, @End_dt, 0, @Remark

PRINT ''
PRINT 'Finish running '+ @SQL_Script_Name
PRINT CONVERT(VARCHAR, GETDATE(), 120)
