<?php
require_once('../../config.php');
require_once($CFG->dirroot .'/lib/pear/HTML/AJAX/JSON.php');
/**
 * @author Joseph Magly - ASA
 * @package moodle multiauth
 *
 * Authentication Plugin: ASA Auth
 *
 * Takes a JSON asserion via a POST request from ASA.org servers. The data is validated and information
 * is stored in staging table 'asa_sso' to await redirection of the end user to moodle.
 *
 * 2011-05  File created.
 */



 //TODO MAINTANCE FUNCTION TO CLEAN OUT OLD/EXPIRED ASSERIION TOKENS


$success = true;

delete_records_select('asa_sso', 'Issued < DATE_SUB(NOW(), INTERVAL 720 MINUTE)');

$assertDataJSON = $GLOBALS['HTTP_RAW_POST_DATA'];

$assertData = json_decode($assertDataJSON);

	$AssertionTicket->token = clean_param($assertData->{'Token'}, PARAM_CLEAN);
	$AssertionTicket->firstname = clean_param($assertData->{'FirstName'}, PARAM_CLEAN);
	$AssertionTicket->lastname = clean_param($assertData->{'LastName'}, PARAM_CLEAN);
	$AssertionTicket->schoolname = clean_param($assertData->{'SchoolName'}, PARAM_CLEAN);
	$AssertionTicket->schooluri = clean_param($assertData->{'SchoolURI'}, PARAM_CLEAN);
	$AssertionTicket->schoollogo = clean_param($assertData->{'SchoolName'}, PARAM_CLEAN);
	$AssertionTicket->username = clean_param($assertData->{'Username'}, PARAM_CLEAN);
	$AssertionTicket->courses = clean_param(implode(',', $assertData->{'Courses'}), PARAM_CLEAN);
	$AssertionTicket->issued = clean_param(date("Y-m-d H:m:s"), PARAM_CLEAN);
	$AssertionTicket->branch = clean_param($assertData->{'SchoolBranch'}, PARAM_CLEAN);
	$AssertionTicket->oecode = clean_param($assertData->{'SchoolOECode'}, PARAM_CLEAN);
	
	
	

	if ($ticketId = insert_record('asa_sso', $AssertionTicket))
	{
		$success = true;	
		
	}
	else
	{
		$success = false;	
	}


if ($success)
{
 header('X-Asa-Success:1');
}
else
{
 header('X-Asa-Success:0');
}


?>
