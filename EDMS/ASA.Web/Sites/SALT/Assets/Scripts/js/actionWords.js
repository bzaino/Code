//This is the format for all entries into this list - "Content Type Name": "Action Word"
//The character limit for "Action Word" is 10
//How to edit: 
//Below is a list of all the contents types and their matching "action word"
//Only edit the "Content Type Name" and the "Action Word".
//If you change the "Content Type Name" remember to update the matching icons in "Assets/images"
//DO NOT Delete this file
//DO NOT Change the structure or format of the text below. That includes indentations and these characters ("", : , =, {, } , ; )
var actionWords = {
	"Article": "read",
	"eBook": "download",	
	"Form": "download",
	"Video": "watch",	
	"Infographic": "view",
	"Comic": "view",
	"Tool": "try",
	"Lesson": "do",
	"Course": "study"
};
//-------------------------------------------------------------------------------------------------------------
//TOUCH NOTHING BELOW THIS LINE IF YOU ARE NOT A DEVELOPER
//-------------------------------------------------------------------------------------------------------------
//The following is a shim to allow this file to be require'd both client and server side
if (typeof exports !== 'undefined') {
	if (typeof module !== 'undefined' && module.exports) {
		//node.js require style using exports
		module.exports = actionWords;
	}
} else {
	//require.js/AMD require style for use in browser
	define(actionWords);
}