/**
 * Plugin: jquery.zGlossary
 *
 * Version: 1.0.2
 * (c) Copyright 2011-2013, Zazar Ltd
 *
 * Description: jQuery plugin to find and display term definitions in HTML text
 *
 * History:
 * 1.0.2 - Added show once option
 * 1.0.1 - Correct mistype to _addTerm function (Thanks Aldopaolo)
 * 1.0.0 - Initial release
 *
 **/
(function($){

    $.fn.glossary = function(JsonData, options) {

        // Set plugin defaults
        var defaults = {
            ignorecase: true,
            tiptag: 'h6',
            excludetags: ['A','H1','H2','H3', 'H4', 'H5', 'H6', 'STRONG'],
            linktarget: '_blank',
            showonce: true,
            showForImage: false,
            hideTerm: false
        };
        var options = $.extend(defaults, options);
        var id = 1;

        // Functions
        return this.each(function(index, $element) {

            // Ensure any exclude tags are uppercase for comparisons
            $.each(options.excludetags, function(i,$element) { options.excludetags[i] = $element.toUpperCase(); });

            // Function to find and add term
            var _addTerm = function($element, term, def, optionalHeader) {
                var skip = 0;
                var pos;

                // Check the element is a text node
                if ($element.nodeType == 3) {
                    // Case insensistive matching option
                    if (options.ignorecase) {
                        pos = new RegExp('\\b(' + term + ')\\b', 'i').exec($element.data);
                    } else {
                        pos = new RegExp('\\b(' + term + ')\\b').exec($element.data);
                    }
                    //If we have found a term, set the index to where it was found, otherwise -1.
                    pos = pos ? pos.index : -1;

                    // Check if the term is found
                    if (pos > -1) {

                        // Check for excluded tags
                        if (jQuery.inArray($($element).parent().get(0).tagName.toUpperCase(), options.excludetags) < 0) {
                            // Create link element
                            var spannode,
                                tipCopy = options.hideTerm? def : '<'+ options.tiptag +'>'+ term + '</'+ options.tiptag +'>' + def;
                            if (options.showForImage) {
                                spannode = document.createElement('span');
                                spannode.className = 'glossaryTerm';
                                // Popup definition
                                spannode.id = "glossaryID" + id;
                                $(spannode).on('show-tip', function (e, originalEvent) {
                                    $.glossaryTip(tipCopy, {mouse_event: originalEvent});
                                    return false;
                                });
                            } else {
                                spannode = document.createElement('a');
                                spannode.className = 'glossaryTerm';
                                // Popup definition
                                spannode.id = "glossaryID" + id;
                                spannode.href = '#';
                                spannode.title = 'Click for \''+ term +'\' definition';
                                $(spannode).click(function(e) {
                                    $.glossaryTip(tipCopy, {mouse_event: e})
                                    return false;
                                });
                            }

                            var middlebit = $element.splitText(pos);
                            var endbit = middlebit.splitText(term.length);
                            var middleclone = middlebit.cloneNode(true);

                            spannode.appendChild(middleclone);
                            middlebit.parentNode.replaceChild(spannode, middlebit);

                            skip = 1;
                            id += 1;
                        }
                    }
                }
                else if ($element.nodeType == 1 && $element.childNodes && !/(script|style)/i.test($element.tagName)) {

                    // Search child nodes
                    for (var i = 0; i < $element.childNodes.length; i++) {

                        var ret = _addTerm($element.childNodes[i], term, def);

                        // If term found and show once option go to next term
                        if (options.showonce && ret === 1) {
                            skip = 1;
                            break;
                        } else {
                            i += ret;
                        }
                    }
                }

                return skip;
            };

            // Get glossary list items
            for (var i = 0; i < JsonData.length; i++) {
                // Find term in text
                var item = JsonData[i];
                _addTerm($element, item.term.trim(), item.definition);
            }

        });

    };

    // Glossary tip popup
    var glossaryTip = function() {}

    $.extend(glossaryTip.prototype, {

        setup: function(){

            if ($('#glossaryTip').length) {
                $('#glossaryTip').remove();
            }
            glossaryTip.holder = $('<div id="glossaryTip"><div id="glossaryClose">x</div></div>');
            glossaryTip.content = $('<div id="glossaryContent"></div>');
            glossaryTip.nub = $('<span class="nub"></span>');

            $('body').append(glossaryTip.holder.append(glossaryTip.content).append(glossaryTip.nub));
        },

        show: function(content, event) {

            function closeIfClickedAway(e) {
                if (!$('#glossaryTip').find(e.target).length) {
                    closeTip();
                }
            }

            function closeTip() {
                glossaryTip.holder.stop(true).fadeOut(200);
                $(document).off('click', closeIfClickedAway);
                return false;
            }

            glossaryTip.content.html(content);

            // Display tip at mouse cursor
            var x,
                eventPosX = event.mouse_event.pageX ? parseInt(event.mouse_event.pageX, 10) : $(event.mouse_event.target).position().left + 160,
                y = event.mouse_event.pageY ? parseInt(event.mouse_event.pageY, 10) - $('#glossaryTip').height() - 24 : $(event.mouse_event.target).position().top - $('#glossaryTip').height() + 65,
                $glossaryScope = $('.js-glossary-scope'),
                glossaryWidth = $glossaryScope.width();
            //Edge cases where zglossary is weak
            //Also IE and Chrome measure .content widths differently
            //x is set to the left edge of the screen if div.content is responsively resized to below 320px
            //or the mouse event is below 100 (iphones etc)
            if (glossaryWidth < 320 || eventPosX < 100) {
                x = 10;
            }
            // if parseInt(event.mouse_event.pageX, 10) < 600, then the browser is not full screen - content is full width
            else if (glossaryWidth > 585 && $glossaryScope.width() < 620 && eventPosX < 600) {
                x = 30;
            }
            // responsively narrowed window screen where .content is not full screen width
            else if (glossaryWidth < 584) {
                x = 20;
            }
            //otherwise set set the overlay left to 100 pxs less than the screen event.
            else {
                x = eventPosX - 100;
            }
            glossaryTip.holder.css({top: y, left: x});

            // Display the tip
            if (glossaryTip.holder.is(':hidden')) {
                glossaryTip.holder.show();
                //nubLoc is the location of the arrow below the tip (span.nub)
                var nubLoc = eventPosX - $('#glossaryTip').position().left;
                if (nubLoc > $('#glossaryTip').width() - 20) {
                    nubLoc -= 30;
                }
                if (nubLoc > $('#glossaryTip').width() * .85) {
                     $('#glossaryTip .nub').addClass('far-right');
                      nubLoc -= 10;
                }
                $('#glossaryTip .nub').css({left: nubLoc});
            }

            // Add click handler to close
            $('#glossaryClose').bind('click', closeTip);

            // clicking away from the tooltip should close it.
            $(document).on('click', closeIfClickedAway);

        }

    });
    //resizing the window causes the glossary popup to act strangely
    //- easiest to close it and let them re-click on a term
    $( window ).resize(function() {
        $('#glossaryTip').remove();
    });

    $.glossaryTip = function(content, event) {
        var tip = $.glossaryTip.instance;

        if (!tip) { tip = $.glossaryTip.instance = new glossaryTip(); }

        tip.setup();
        tip.show(content, event);

        return tip;
    }
})(jQuery);
