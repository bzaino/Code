
/*
 * Setting jQuery to No Conflict and bind to $j instead
 */
var $j = jQuery.noConflict();

/*
 * Define namespaces
 */
 $j.perc = {};
 
/**
 * Redirect the page to the specified url.
 * @param url (string) the url string
 * @param args (Object) the query string as an object of name/value pair
 * object properties.
 */
(function($) {
  $.perc_redirect = function(url, args)
  {    
     // Find debug on query string
     var isDebug = window.location.href.indexOf("debug=true") != -1;
     var hasQuery = url.indexOf("?") != -1;
     if(isDebug)
     {
        if(args == null || args == 'undefined')
           args = new Object();
        args.debug = "true";
     }
     var qStr =  (args == null || args == 'undefined') ? ""
        : $.param(args);
     if(qStr.length > 0)
        qStr = (hasQuery ? "&" : "?") + qStr;
     var newUrl = url + qStr;
     window.location.href = newUrl;
  }  
   
})(jQuery);

/**
 * Auto fills target text field while typing in the source field
 * if target field was empty when the field took focus.
 * @param src (string or jQuery) a selector for the source text field or its
 * jQuery representation.
 * @param tgt (string or jQuery) a selector for the target text field or its
 * jQuery representation.
 * @param filter (Object) optional filter to run text through.
 */      
(function($) {
    $.perc_textAutoFill = function(src, tgt, filter, ignoreEmpty, maxChars)  // IgnoreEmpty is false, by default
  {               
      var tgtEmpty = false;
      var srcFocused = false;

      ignoreEmpty = typeof(ignoreEmpty) != 'undefined' ? ignoreEmpty : false;

      // maxChars is used when the tgt field has a maxlength restriction
      maxChars = typeof(maxChars) != 'undefined' ? maxChars : -1;
       
      $(src).bind('focus.textAutoFill', function(evt){
          var val = $(tgt).val();
          tgtEmpty = (val == 'undefined' || val == null || val == '' || ignoreEmpty == true);
          srcFocused = true;      
      });

      $(src).bind('keyup.textAutoFill', {'filter': filter}, function(evt){
         var isDisabled = $(tgt).attr('disabled');
         if(!srcFocused)
         {
             var val = $(tgt).val();
             tgtEmpty = (val == 'undefined' || val == null || val == '');
             srcFocused = true;
         }
         var filter = function(txt){return txt};
         if(typeof evt.data.filter == 'function')
            filter = evt.data.filter;

         if(tgtEmpty && !isDisabled)
         {
            if(maxChars > 0 && $(src).val().length > maxChars)
            {
                var value = $(src).val().substring(0, maxChars);
                $(tgt).val(filter(value));
            }
            else
            {
                $(tgt).val(filter($(src).val()));
            }
         }
      });
  }   
})(jQuery);

/**
 * Filter a text input field with a passed in filter.
 * @param tgt {String or jQuery} the input field to be filtered. Cannot be <code>null</code>.
 * @param filter {Function} the filter function, the fuction receives the currently
 * typed in char and should return the char back if ok or return empty string if
 * it should be filtered out.
 * @param callback if it is a function, it gets called after the value has been filtered 
 * with the jquery element it is working on.
 */     
(function($) {
  $.perc_filterField = function(tgt, filter, callback)
  {
      $(tgt).bind('keypress.filterField', {'filter': filter}, function(evt){
         var filter = function(txt){return txt};
         
         var rawCode = evt.charCode ? evt.charCode : evt.which;
         if(rawCode == 0 || rawCode == 8 || rawCode == 13)
            return;
         var theChar = String.fromCharCode(rawCode);
         if(typeof evt.data.filter == 'function')
            filter = evt.data.filter;
         var filtered = filter(theChar);
         if(filtered.length == 0)
         {
            evt.preventDefault();
         }
         else if(theChar != filtered)
         {
           // the filter changed the char to something else, update the field
           // and retain correct cursor caret positioning
           
           var field = evt.target;
           var start = $(field).caret().start;
           var end = $(field).caret().end;
           var t1 = field.value.slice(0, start);
           var t2 = field.value.slice(end);
           var newVal = t1 + filtered + t2;
           field.value = newVal;
           var cursorPos = t1.length + 1;
           $(field).caret({start: cursorPos, end: cursorPos});
           evt.preventDefault();
         }
      });
      
      $(tgt).bind('blur.filterField', {'filter': filter}, function(evt){
         // Run through the filter on blur
         tgt.val(evt.data.filter(tgt.val()));
         if($.isFunction(callback))
             callback($(tgt));
      });
      
  }   
})(jQuery); 

/**
 * Filter a text input field with a passed in filter. This function filters the whole word instead of
 * single characters
 * @param tgt {String or jQuery} the input field to be filtered. Cannot be <code>null</code>.
 * @param filter {Function} the filter function, the fuction receives the currently
 * @param splitChar (String) optional parameter to split first the text and filter every splitted value
 * typed in char and should return the char back if ok or return empty string if
 * it should be filtered out.
 */
(function($) {
  $.perc_filterFieldText = function(tgt, filter, splitChar)
  {
      $(tgt).bind('keypress.filterField', {'filter': filter}, function(evt){
         var filter = function(txt){return txt};
         
         var rawCode = evt.charCode ? evt.charCode : evt.which;
         if(rawCode == 0 || rawCode == 8)
            return;
         var theChar = String.fromCharCode(rawCode);
         if(typeof evt.data.filter == 'function')
            filter = evt.data.filter;
         var position = $(evt.target).caret().start;
         var theText = tgt.val();
         
         var fieldMaxLength = tgt.attr("maxlength");
         if (typeof(fieldMaxLength) != 'undefined' && theText.length == fieldMaxLength)
            return;         
         
         theText = theText.substring(0, position) + theChar + theText.substring(position, theText.length);
         
         var filtered = "";
         if (typeof(splitChar) != 'undefined' && splitChar.length == 1)
         {
            var arrayTexts = theText.split(splitChar);
            for (var i = 0; i < arrayTexts.length; i++)
            {
                arrayTexts[i] = filter(arrayTexts[i]);
            }
            filtered = arrayTexts.join(splitChar);
         }
         else
         {
            filtered = filter(theText);
         }
         
         if(filtered.length == 0)
         {
            evt.preventDefault();
         }
         else if(theText != filtered)
         {
           // the filter changed the char to something else, update the field
           // and retain correct cursor caret positioning
           
           var field = evt.target;
           
           var start = $(field).caret().start;
           var newVal = filtered;
           field.value = $.trim(newVal);
           var cursorPos = start + 1;
           $(field).caret({start: cursorPos, end: cursorPos});
           evt.preventDefault();
         }
      });
      
      $(tgt).bind('blur.filterField', {'filter': filter}, function(evt){
         theText = tgt.val();
         var filtered = "";
         if (typeof(splitChar) != 'undefined' && splitChar.length == 1)
         {
            var arrayTexts = theText.split(splitChar);
            for (var i = 0; i < arrayTexts.length; i++)
            {
                arrayTexts[i] = filter(arrayTexts[i]);
            }
            filtered = arrayTexts.join(splitChar);
         }
         else
         {
            filtered = evt.data.filter(theText);
         }
         
         // Run through the filter on blur
         tgt.val($.trim(filtered));
      });  
  }
})(jQuery);

/**
 * Define text filters for "page/asset/folder names" and "site name"
 */ 
(function($) {
  $.perc_textFilters = {
  
     // This filter is used for page, asset and folder name entries.
     // It does not allow 2 sets of characters:
     //   (1) invalid characters for file name in Windows (\/:*?"<>|'),
     //   (2) unsafe URL characters:
     //        '#' (used by HTML anchors),
     //        ';' (used to append jsessionid to URL)
     //        '%' (used to URL encoding/escape)
     URL: function(txt){return txt.replace( /[ ]/g, '-' ).replace( /[\\\/\:\*\?\"<\>\|\#\;\%\']/g, '' );},
     
     // This filter used for site name (or hostname).
     // The allowed characters are: alpha-numeric, '-' and '.'
     HOSTNAME: function(txt){return txt.replace( /[^a-zA-Z0-9\-\.]/g, '' );},
     
     // Filter to not allow spaces
     NOSPACES: function(txt){return txt.replace(/ /g, '');},
     
     //Filter to not allow backslashes
     NOBACKSLASH: function(txt){return txt.replace(/[\\]/g,'');},
     
     // This filter allows only characters which are valid in name and/or id attributes.
     // Note: This filter does NOT force starting alpha, which IS a requirement of the W3C spec.
     // Allowed Characters: alpha-numeric, '-', '_', ':', and '.'
     IDNAMECDATA: function(txt){return txt.replace( /[^a-zA-Z0-9\-\_]/g, '');},
     
     // Same filter as IDNAMECDATA but forcing the alpha starting.
     // Note: This filter does not replace '.' or ':'
     IDNAMECDATAALPHA: function(txt){
        var text = txt.replace(/[\ ]/g, '-').replace( /[^a-zA-Z0-9\-\_]/g, '');
        if (text.length > 0 && text.charAt(0).match(/^[^a-zA-Z]{1}/) != null)
            text = text.substring(1, text.length);
        return text;
     },
     
     // This filter is used for path for asset, folder and files.
     // It does not allow 2 sets of characters:
     //   (1) invalid characters for file name in Windows (:*?"<>|), we need allow / as valid character for paths
     //   (2) unsafe URL characters: (to be consistent with the asset, folder and file names limitation)
     //        '#' (used by HTML anchors),
     //        ';' (used to append jsessionid to URL)
     //        '%' (used to URL encoding/escape)
     PATH: function(txt){return txt.replace( /[\\\:\*\?\"<\>\|\#\;\%]/g, '' );},

     // This filter used for template region name.
     // The allowed characters are: alpha-numeric, '-', '.', ':' and '_'.
     ID: function(txt){return txt.replace( /[^a-zA-Z0-9\-\.\:\_]/g, '' );},
     
     // This filter used for widget name.
     // The allowed characters are: alpha-numeric, '-', '.', ':', '_' and ' ' (space).
     ID_WITH_SPACE: function(txt){return txt.replace( /[^a-zA-Z0-9\-\.\:\_ ]/g, '' );},
     
     // This filter is used for inputs that allow only numbers (digits)
     // The are no special characters allowed, only 0-9.
     ONLY_DIGITS: function(txt){
        return txt.replace( /[^0-9]/g, '' );
    },     
     ALPHA_NUMERIC: function(txt){return txt.replace( /[^a-zA-Z0-9]/g, '' );},
     DIGITS_DOT: function(txt){return txt.replace( /[^0-9\.]/g, '' );},     
     
     // This filter is used for widget description.
     // Invalid characters: "&", "<" and ">".
     DESCRIPTION: function(txt){return txt.replace( /[<>&]/g, '' );},
     WINDOWS_FILE_NAME: function(txt){return txt.replace( /\.*$/g, '' );}
  };
})(jQuery);

/**
 * Defines text auto-fill filters
 */
(function($) {
  $.perc_autoFillTextFilters = {
  
     // This auto-fill filter used for page & asset name entries.
     // It only allowes: alpha-numeric and '-'.
     // It auto converts ' ' or '_' to '-'
     URL: function(txt){return txt.replace( /[ \_]/g, '-' )
        .replace( /[^a-zA-Z0-9\-\_]/g, '' ).replace(/[-]+/g, '-').toLowerCase();},
        
     // This filter allows only characters which are valid in name and/or id attributes.
     // Note: This filter does NOT force starting alpha, which IS a requirement of the W3C spec.
     // Allowed Characters: alpha-numeric, '-', and '_'
     // It autoconverts ' ' to '-'
     IDNAMECDATA: function(txt){return txt.replace(/[:\ \.]/g, '-').replace( /[^a-zA-Z0-9\-\_]/g, '');},
     
     // Same filter as IDNAMECDATA but forcing the alpha starting.
     // Note: This filter does not replace '.' or ':'
     IDNAMECDATAALPHA: function(txt){
        var text = txt.replace(/[\ ]/g, '-').replace( /[^a-zA-Z0-9\-\_]/g, '');
        if (text.length > 0 && text.charAt(0).match(/^[^a-zA-Z]{1}/) != null)
            text = text.substring(1, text.length);
        return text;
     }
  }; 
})(jQuery);

/*
 * UI Blocking function wrappers
 */ 
(function($){
   /**
    * Call this method to cause a ui block to occur. It is very important
    * to make sure to call $.unblockUI() after both completed work or
    * by code handling errors. These functions depend on the jquery.blockui
    * plugin v2 in order to work.
    * @param mode {object} (Optional) one of the $.PercBlockUIMode types.
    * If <code>null</code> or undefined then OVERLAY mode will be used
    * as the default.
    * @param message {string} (Optional) message that will override default.
    */
   $.PercBlockUI = function(mode, message){
      var opts = {};
      if(typeof(mode) != 'object' || mode == null)
         mode = $.PercBlockUIMode.OVERLAY;
      $.extend(opts, mode);
      if(typeof(message) != 'undefined' && message != null && message != '')
         $.extend(opts, {message: message});
      $.blockUI(mode);
   };
   
   
   /**
    * UI Blocking modes
    */
   $.PercBlockUIMode = {
      /*
       * Shows an error message and busy animated image on a
       * visible fullscreen overlay with
       * a busy cursor.
       */
      OVERLAY: {
         message: '<img src="/cm/images/images/Busy.gif"/><br/>Processing. Please wait...',
         css: {
            border: 'none',
            backgroundColor: '#fff',
            opacity: '0.9', 
            color: '#000',
            fontFamily: 'Verdana',
            fontSize: '11px',
            '-webkit-border-radius': '10px',
            '-moz-border-radius': '10px',
            padding: '5px',
            width: '25%'
         },
         overlayCSS: {opacity: '0.6'},
         fadeIn: 40,
         fadeOut: 40,
         baseZ: 9999
      },
      /*
       * Uses an invisible overlay to block, no message and cursor displays
       * the busy icon.
       */
      CURSORONLY: {
         message: null,
         overlayCSS: {opacity: 0.0},
         fadeIn: 0,
         fadeOut:0,
         baseZ: 9999
      }
   };
})(jQuery);

/**
 * AJAX Post queueing
 */ 
(function($){
   /* Private */
   var ajaxInProgress = false;
   var queue = [];
   
   /*
    *  Set up the listeners for AJAX start and stop
    */
   $(document).ready(function(){
      $('body').ajaxSend(
         function(){
            ajaxInProgress = true;
         }
      );
      $('body').ajaxComplete(
         function(){
            ajaxInProgress = false;
            processQueue();
      });
   });
   
   /**
    * Process all functions on the queue.
    */
   function processQueue(){
      if(queue.length > 0 && !ajaxInProgress)
      {
         var func = queue.shift();
         func();
         processQueue();
      }
   }
   /* Public */
   
   /**
    * Queue a function to be sure it only runs after all current
    * AJAX calls complete.
    * @param func {function} the function to be queued and executed after
    * all AJAX calls complete. If not a function then nothing will be queued.
    */
   $.PercQueuePostAJAX = function(func){
      if(typeof(func) == 'function')
      {
         queue.push(func);
         processQueue();
      }
   };

})(jQuery);

(function($){
   /**
    *  Truncate the specified max.
    *  @param txt {String} the text to be truncated.
    *  @param max {int} the max length.
    *  @return the truncated text if truncation was needed or the untouched text if not.
    */
    $.PercTruncateText = function(txt, max) {
        if(txt.length <= max)
            return txt;
        var len = max - 3;
        return   txt.substr(0, len) + "...";
    }
})(jQuery);

(function($){
   /**
    *  Truncate text to the specified max pixel width
    *  @param element {jQuery HTML Element} the HTML element that contains the text
    *  @param maxWidth {int} the max width in pixels
    */
    $.PercTextOverflow = function(element, maxWidth) {
        if($.browser.msie || $.browser.safari) {
            // use CSS attributes on IE and Safari on DIV with label
            // because they support these CSS attributes
            element
                .css("white-space","nowrap")
                .css("text-overflow","ellipsis")
                .css("width",maxWidth)
                .css("overflow","hidden");
        } else {
            // otherwise use textOverflow plugin
            element.textOverflow("...",false);
        }
    }
})(jQuery);

(function($){

   /**
     * Bind a event, with or without data
     * Benefit over $.bind, is that $.binder(event, callback, false|{}|''|false) works.
     * @version 1.0.0
     * @date June 30, 2010
     * @package jquery-sparkle {@link http://www.balupton/projects/jquery-sparkle}
     * @author Benjamin "balupton" Lupton {@link http://www.balupton.com}
     * @copyright (c) 2009-2010 Benjamin Arthur Lupton {@link http://www.balupton.com}
     * @license GNU Affero General Public License version 3 {@link http://www.gnu.org/licenses/agpl-3.0.html}
     */
    $.fn.binder = $.fn.binder || function(event, data, callback){
        // Help us bind events properly
        var $this = $(this);
        // Handle
        if ( (callback||false) ) {
            $this.bind(event, data, callback);
        } else {
            callback = data;
            $this.bind(event, callback);
        }
        // Chain
        return $this;
    };

    /**
     * Event for performing a singleclick
     * @version 1.1.0
     * @date July 16, 2010
     * @since 1.0.0, June 30, 2010
     * @package jquery-sparkle {@link http://www.balupton/projects/jquery-sparkle}
     * @author Benjamin "balupton" Lupton {@link http://www.balupton.com}
     * @copyright (c) 2009-2010 Benjamin Arthur Lupton {@link http://www.balupton.com}
     * @license GNU Affero General Public License version 3 {@link http://www.gnu.org/licenses/agpl-3.0.html}
     */
    $.fn.singleclick = $.fn.singleclick || function(data,callback){
        return $(this).binder('singleclick',data,callback);
    };
    $.event.special.singleclick = $.event.special.singleclick || {
        setup: function( data, namespaces ) {
            $(this).bind('click', $.event.special.singleclick.handler);
        },
        teardown: function( namespaces ) {
            $(this).unbind('click', $.event.special.singleclick.handler);
        },
        handler: function( event ) {
            // Setup
            var clear = function(event){
                // Fetch
                var Me = this;
                var $el = $(Me);
                // Fetch Timeout
                var timeout = $el.data('singleclick-timeout')||false;
                // Clear Timeout
                if ( timeout ) {
                    clearTimeout(timeout);
                }
                timeout = false;
                // Store Timeout
                $el.data('singleclick-timeout',timeout);
            };
            var check = function(event){
                // Fetch
                var Me = this;
                clear.call(Me);
                var $el = $(Me);
                // Update the amount of times we have been clicked
                $el.data('singleclick-clicks', ($el.data('singleclick-clicks')||0)+1);
                // Handle Timeout for when All Clicks are Completed
                var timeout = setTimeout(function(){
                    // Fetch Clicks Count
                    var clicks = $el.data('singleclick-clicks');
                    // Clear Timeout
                    clear.apply(Me,[event]);
                    // Reset Click Count
                    $el.data('singleclick-clicks',0);
                    // Check Click Status
                    if ( clicks === 1 ) {
                        // There was only a single click performed
                        // Fire Event
                        event.type = 'singleclick';
                        $.event.handle.apply(Me, [event])
                    }
                },500);
                // Store Timeout
                $el.data('singleclick-timeout',timeout);
            };
            // Fire
            check.apply(this,[event]);
        }
    };
    

})(jQuery);

(function($){
 
 $(document).ready(function(){
    /* Add custom jeditable type using filter */
    $.editable.addInputType('filteredText', {
        element : function(settings, original) {
           var input = $('<input type="text">');
           if (settings.width  != 'none') { input.width(settings.width);  }
           if (settings.height != 'none') { input.height(settings.height); }
           input.attr('autocomplete','off');
           $.perc_filterField(input, $.perc_textFilters.URL);
           input.attr('id',settings.fieldid);
           $(this).append(input);
           return(input);
         }
     });
 
 });
 })(jQuery);

