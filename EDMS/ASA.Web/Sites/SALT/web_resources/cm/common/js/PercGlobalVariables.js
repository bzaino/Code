(function($)
{
    $(document).ready(function(){
        //Add global variable data file dynamically
        var jsGVFileLocation = $('script[src*="PercGlobalVariables.js"]').attr('src'); 
        jsGVFileLocation = jsGVFileLocation.replace('PercGlobalVariables.js', 'PercGlobalVariablesData.js?_'); 
        $.getScript(jsGVFileLocation + new Date().getTime(), function(){
            $(".perc-global-variable-marker").each(function(){
                var curVarElem = $(this);
                var gvName = curVarElem.attr("title");
                if(gvName)
                {
                    var gvValue = PercGlobalVariablesData[gvName];
                    curVarElem.text(gvValue);
                }
                curVarElem.removeAttr("title");
            });
        });
    });
})(jQuery);
