using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ASA.Web.Common.Validation
{
    public static class RegexStrings
    {
        //these are the regex expressions that are being used with the js front end code when registering
        public const string REGISTRATION_FORM_NAME = @"^(?!\s)([a-zA-Z'\-\s]{1,30})$";
        public const string REGISTRATION_FORM_EMAIL = @"^(?!^.{65})((''[\w-\s]+'')|([\w-]+(?:\.[\w-]+)*)|(''[\w-\s]+'')([\w-]+(?:\.[\w-]+)*))(@((?:[\w-]+\.)*\w[\w-]{0,66})\.([a-zA-Z]{2,6}(?:\.[a-zA-Z]{2})?)$)|(@\[?((25[0-5]\.|2[0-4][0-9]\.|1[0-9]{2}\.|[0-9]{1,2}\.))((25[0-5]|2[0-4][0-9]|1[0-9]{2}|[0-9]{1,2})\.){2}(25[0-5]|2[0-4][0-9]|1[0-9]{2}|[0-9]{1,2})\]?$)";
        public const string REGISTRATION_FORM_PASSWORD = @"^\S{8,32}$";
        
        public const string GUID = @"^(\{){0,1}[0-9a-fA-F]{8}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{12}(\}){0,1}$";
        public const string NAME = @"^[-a-zA-Z\s\']+$";
        public const string CURRENCY = @"^(\d{1,9})(\.\d{1,2})?$";
        public const string PHONEus = @"(^(\s)*(1-?)?(\([2-9]\d{2}\)|[2-9]\d{2})(\s)*-?[2-9]\d{2}-?\d{4}$)";
        public const string USPostalCode = @"^\d{5}$";

    }
}
