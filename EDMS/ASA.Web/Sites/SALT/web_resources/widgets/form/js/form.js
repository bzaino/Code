(function($){
    $(document).ready(function(){ 
            $( ".form-datepicker" ).datepicker({
                showOn: "button",
                buttonImage: "/web_resources/widgets/form/images/calendar.gif",
                dateFormat: "d M, yy",
                buttonImageOnly: true,
                changeMonth: true,
                changeYear: true,
                onClose: function(dateText, inst) {
                        $(this).trigger('focusout');     
                                                }


            });
            
            /**
             * Returns  a string containing the schema, domain and if present the port
             * @param {String} url The url to extract the location from
             * @return {String} The location part of the url
             */
            function getLocation(url){
                var reURI = /^((http.?:)\/\/([^:\/\s]+)(:\d+)*)/; // returns groups for protocol (2), domain (3) and port (4)
                var m = url.toLowerCase().match(reURI);
                var proto = m[2], domain = m[3], port = m[4] || "";
                if ((proto == "http:" && port == ":80") || (proto == "https:" && port == ":443")) {
                    port = "";
                }
                return proto + "//" + domain + port;
            }
          
            $('.perc-form').find('form').each(function(){
                var formAction = $(this).attr("action");
                if(formAction && formAction.indexOf("/perc-form-processor") == 0){
                    var tenantId = $.isFunction($.getCm1License)?$.getCm1License():"";
                    var version = $.isFunction($.getCm1Version)?$.getCm1Version():"";
                    
                    formAction = $.getDeliveryServiceBase() + formAction + ((formAction.indexOf('?')!=-1)?"&":"?") + "perc-tid=" + tenantId + "&perc-version=" + version;      

                    $(this).attr("action", formAction);
                    $(this).append(
                        $('<input/>').
                        attr("type", "hidden").
                        attr("name", "perc_hostUrl").
                        attr("value", getLocation(location.href))
                    )
                }
                
                var myRules = {};
                $(this).find('input[type=text], textarea').each(function(){
                    if($(this).attr('fieldmaxlength') > 0)
                    {
                        if(typeof(myRules[$(this).attr('name')]) === "undefined")
                            myRules[$(this).attr('name')] = {};
                        
                        myRules[$(this).attr('name')]['maxlength'] = $(this).attr('fieldmaxlength');
                    }
                });
                
                $(this).validate({
                    errorClass:"form-error-msg"  ,
                    errorPlacement: function(error, element) {
                                       if(element.attr('type') == 'checkbox'){
                                           error.appendTo( element.parent().parent());    
                                        }
                                        else{									
                                           error.appendTo( element.parent());
                                        }   
                                    },
                    rules:myRules				
                                    
                });
            });		
    });
})(jQuery);