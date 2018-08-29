/**
 * jQuery Commify Plugin @VERSION
 *
 * https://github.com/christopherstyles/jquery-commify
 *
 * Copyright (c) 20011 - 2012 Christopher Styles
 *
 * Dual licensed under the MIT and GPL licenses:
 *   http://www.opensource.org/licenses/mit-license.php
 *   http://www.gnu.org/licenses/gpl.html
 */

;(function($){
    $.extend({
        commify: function( options ) {
            
            var base, decimal, negative, n, parts, pieces, result;
            
            // Take the first argument as the number to be commified
            n = arguments[0];
            
            // Clean out anything we don't want
            n = String(n).replace(/[^\d\.]/g, "");
            
            // Update the options if specified
            options = (arguments.length > 1) ? $.extend({}, $.commify_defaults, arguments[1]) : $.commify_defaults;
            
            // Round the number if a round value was specified
            if (options.round && typeof options.round === "number")
                n = parseInt(Math.round(n/options.round)*options.round, 10);
            
            // Coerce the number to a String
            n = String(n);
            
            // Split into an array
            parts = n.split(".");
            
            // Get the base number
            base = parts[0];
            
            // Are we a negative?
            negative = (Boolean(~(n.indexOf("-")))); // see: http://jsperf.com/tilde-instead-of-strict-inequality
            if (negative) 
                base = base.replace('-', '');
            
            // Start an empty result
            result = "";
            
            // Break the base number into pieces
            pieces = base.split('').reverse();
            
            // Loop over it, inserting commas for every 3
            for (var i = 0; i < pieces.length; i++) {
                if (i !== 0 && i % 3 === 0)
			        result = ',' + result;
			    result = pieces[i] + result;
            }
            
            // Get the decimal if there is one
            decimal = (parts.length > 1) ? ("." + parts[1]) : "";
            result += decimal;
            
            // Add the negative back if it was originally negative
            if (negative)
                result = "-" + result;
            
            // Add a prefix if one was specified
            if (options.prefix)
                result = options.prefix + result;
            
            return result;
        },
        
        commify_defaults: {
            prefix: null,
            round: null // Specify the nth to which you would like to round (10, 100, 1000 etc)
        },
        
        _commify_log: function(msg, raw) {
            if (window.console) {
                if (raw === true)
                    console.log(msg);
                else
                    console.log("$.commify: " + msg);
                return true;
            }
        },
        
        decommify: function(value) {
            return Number(value.replace(/,/g, ""));
        }
    });
})(jQuery);