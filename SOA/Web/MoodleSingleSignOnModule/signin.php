<?php

	require_once('../../config.php');
/**
 * @author Joseph Magly - ASA
 * @package moodle multiauth
 *
 * Authentication Plugin: ASA Auth
 *
 * Takes a get request from any client and processes the provided token (if exsists) If the token is valid
 * the user is created/updated in moodle and a form post is made to Moodle's Login script with the useer name
 * and the token as the password to validate the user and create a logged in context.
 *
 * 2011-05  File created.
 */

	$token = required_param("t", PARAM_CLEAN);
	$res =  get_record('asa_sso', 'token', $token);
	$username = $res->username;


	header('Cache-Control: no-store');


?>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
<META HTTP-EQUIV="CACHE-CONTROL" CONTENT="NO-CACHE">
<title>Untitled Document</title>
<script type="text/javascript" src="https://ajax.googleapis.com/ajax/libs/jquery/1.5.2/jquery.min.js"></script>
<script type="text/javascript">
	$(document).ready(function () {
		$('#login').submit();
	});
</script>
</head>
<body>
	<p align="center">Please wait while we take you to MyMoney 101....</p>
    <form action="../../login/index.php" method="post" id="login">
        <input id="username" name="username" type="hidden" value="<?php echo $username ?>" />
        <input id="password" name="password" type="hidden" value="<?php echo $token ?>"/>
    </form>
</body>
</html>