$(document).ready(function(){ 
    $('.perc-login-form').each(function(){
        var myRules = {
            'perc-login-email-field': {
                required: true,
                minlength: 5,
                maxlength: 320,
                email: true
            },
            'perc-login-password-field': {
                required: true,
                minlength: 6,
                maxlength: 25
            }
        };

        var myMessages = {
            'perc-login-email-field': {
                email: "Invalid Email address format.",
                minlength: "Invalid Email address format.",
                maxlength: "Invalid Email address format."
            },
            'perc-login-password-field': {
                required: "Please provide a password.",
                minlength: "Your password must be at least 6 characters long.",
                maxlength: "Your password must be less than 26 characters long."
            }
        };
        
        $(this).validate({
            errorClass: "form-error-msg",
            errorPlacement: function(error, element) {
               if(element.attr('type') == 'checkbox'){
                   error.appendTo( element.parent().parent());
                }
                else {
                   error.appendTo( element.parent());
                }
            },
            rules: myRules,
            messages: myMessages
        });
    });
});