/*!
 * SALT: Utility
 */
define(function (require, exports, module) {
    var $ = require('jquery'),
        SALT = require('salt');

    SALT.Utility = {
        Status: {
            showStatus: function (msg) {
                $.colorbox({
                    opacity: 0.5,
                    rel: "nofollow",
                    open: true,
                    overlayClose: false,
                    html: '<table><tr><td  valign="middle"><h1>' + msg + ' </h1></td><td style="padding-left: 34px;background: url(/assets/images/global/ajax-loader-2.gif) left center no-repeat; width: 30px; height: 30px" ></td></tr></table>',
                    close: ''
                });
            }
        },
        Pager: function (options) {
            //Private variables
            //---------------
            //options
            //---------------
            maxRecords = options.MaxRecords, //Page size
            pagerDivId = options.PagerDivId, //DIV that pager will be injected into
            onNext = options.OnNext || null, //Event handler for Next
            onPrevious = options.OnPrevious || null, //Event handler for Previous
            //---------------

            currentOffset = 0,
            totalRecords = 0,
            nextPageId = '',
            previousPageId = '',
            messageId = '',
            instanceId = '_' + new Date().getTime(),

            /*
            Resets pager
            */
            clear = function () {
                totalRecords = 0;
                currentOffset = 0;
                $(messageId).html('');
            },
            /*
            Initializes pager control
            */
            init = function () {

                //create element ids
                previousPageId = 'pagerPrevious' + instanceId;
                nextPageId = 'pagerNext' + instanceId;
                messageId = 'pagerMessage' + instanceId;

                //inject control into DIV
                var pager = '<table border=0 cellpadding=0 cellspacing=0 width="100%">' +
                    '<tr> ' +
                    '<td  align="left" width="75px"><a id="' + previousPageId + '" href="#" onclick="return false; ">< Previous</a></td>' +
                    '<td align="center"><div id="' + messageId + '"></div></td>' +
                    '<td align="right"  width="75px"><a id="' + nextPageId + '" href="#" onclick="return false; "> Next ></a></td>' +
                    '</tr></table>';

                $(pagerDivId).html(pager);


                //setup element events
                previousPageId = '#pagerPrevious' + instanceId;
                nextPageId = '#pagerNext' + instanceId;
                messageId = '#pagerMessage' + instanceId;

                $(nextPageId).click(next);
                $(previousPageId).click(previous);
                $(previousPageId).hide();

            },
            /*
            Go to next page
            */
            next = function () {

                if (currentOffset < totalRecords - maxRecords) {
                    currentOffset += maxRecords;
                }

                onNext();
            },
            /*
            Go to previous page
            */
            previous = function () {
                if (currentOffset !== 0) {
                    currentOffset -= maxRecords;
                }
                onPrevious();
            },
            /*
            Update pager UI
            */
            update = function (offset, total) {

                currentOffset = offset;
                totalRecords = total;
                updateMessageUI();
                updateNextUI();
                updatePreviousUI();

            },
            /*
            Update pager message
            */
            updateMessageUI = function () {
                var to = (currentOffset + maxRecords);
                if (to > totalRecords) {
                    to = totalRecords;
                }

                //setup pagemin
                var curOffset = currentOffset + 1;

                //setup message
                var msg = "Records " + curOffset + " - " + to + " of " + totalRecords + "";



                //hide pager if not needed
                if (totalRecords <= maxRecords) {
                    msg = totalRecords + " record" + (totalRecords !== 1 ? "s" : "") + " found";
                    $(nextPageId).hide();
                    $(previousPageId).hide();
                }

                if (totalRecords === 0) {
                    msg = "No records found.";
                }

                $(messageId).html(msg);

            },
            updateNextUI = function () {

                if (currentOffset + maxRecords > totalRecords) {
                    $(nextPageId).hide();
                } else {
                    $(nextPageId).show();
                }

            },
            updatePreviousUI = function () {

                if (currentOffset === 0) {
                    $(previousPageId).hide();
                } else {
                    $(previousPageId).show();
                }

            };

            init();

            //Public interface
            return {
                Clear: clear,
                CurrentOffset: function () {
                    return currentOffset;
                },
                MaxRecords: maxRecords,
                Next: next,
                Previous: previous,
                Update: update,
                TotalRecords: function () {
                    return totalRecords;
                }
            };
        },
        // check for older or unpredictable browsers for graceful degradation on HTML5 usage
        currentBrowser: {
            isSafari: (function () {
                var userAgent = navigator.userAgent.toString().toLowerCase();
                var safariCheck = (userAgent.indexOf('safari') !== -1) && (userAgent.indexOf('chrome') === -1);
                if (safariCheck) {
                    return true;
                } else {
                    return false;
                }
            })(),
            isIE6_9: (function () {
                if ($.browser.msie && ($.browser.version === "6.0" || $.browser.version === "7.0" || $.browser.version === "8.0" || $.browser.version === "9.0")) {
                    return true;
                } else {
                    return false;
                }
            })()
        }
    };

    return SALT.Utility;
});
