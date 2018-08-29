define([
    'jquery',
    'underscore',
    'configuration',
    'asa/ASAWebService'
], function ($, _, configuration) {
    /* Global scope */
    var _this = this,
        updateTargetStr = 'state',
        updateMap,
    asaSeniorBorrower = {
        init: function () {
          /* Define option defaults */
            var defaults = {
                mapCanvasId: 'js-map-canvas',
                htmlPlaceHolder: 'js-place-holder',
                showDefaultMap: true,
                selectlocation: '0',
                serviceUrl: configuration.maps.endPoint || null,
                listHtmlMarkupTemplate: '<p>{click_anchor}{link_open}<span class="hidden">{content}</span></p>'
            };
            /* Create options by extending defaults with the passed in arugments */
            if (arguments[0] && typeof arguments[0] === 'object') {
                _this.options = extendDefaults(defaults, arguments[0]);//setup State dropdown
            }
            var sel = $('select#selState');
            _this.options.mapCanvasId = 'js-map-canvas-swap';
            _this.options.showDefaultMap = false;
            sel.val(_this.options.selectlocation).change();
            sel.change(asaSeniorBorrower.selectionChanged({selectlocation: sel.select('option:selected').val()}));
        },
        selectionChanged: function () {
            var currentOpts = _this.options;
            /*only update when not on select a state*/
            if (arguments[0] && typeof arguments[0] === 'object') {
                _this.options = extendDefaults(currentOpts, arguments[0]);
            }
            updateMap = _.once(function () {
                if (_this.options.mapCanvasId === 'js-map-canvas-swap' && _this.options.showDefaultMap === false) {
                    $('.hidden-map-span').removeClass('hidden');
                    $('#js-map-canvas').addClass('hidden');
                }
            });
            try {
                doServiceRequest(_this.options.serviceUrl, renderPlaceHolder);
            } catch (e) {
                statusLog('selectionChanged exception: ', e.message);
            }
        }
    };
    /* Utility method to extend defaults with user options */
    function extendDefaults(source, properties) {
        var property;
        for (property in properties) {
            if (properties.hasOwnProperty(property)) {
                source[property] = properties[property];
            }
        }
        return source;
    }
    /* Private Methods */
    /*basic ajax call handler*/
    function doServiceRequest(serviceUrl, callback, bAsync) {
        if (serviceUrl) {
            var noCacheIE = '&noCacheIE=' + (new Date()).getTime();
            $.ajax({
                url: serviceUrl,
                cache: noCacheIE,
                type: 'GET',
                async: true || bAsync,
                success: callback,
                error: function (xhr, ajaxOptions, e) {
                    statusLog('Exception on ajax call: ', e.message);
                }
            });
        }
    }
    /*Adds html output to placeHolder param htmlPlaceHolder. Requires json as data type*/
    function renderPlaceHolder(data) {
        var htmlOut = '',
        main = data,
        schoolArr = [],
        stateObj = [],
        replaceStateRx = new RegExp(updateTargetStr, 'i');
        _this.options.locCouter = 0;
        _this.options.addArr = [];
        if (typeof data === 'string') {
            main = $.parseJSON(data);
        }
        /*filter for matching state*/
        if (_this.options.selectlocation && _this.options.selectlocation !== '0') {
            stateObj = main.mainContent[0][_this.options.selectlocation.toLowerCase()][0].state[0];
            schoolArr = main.mainContent[0][_this.options.selectlocation.toLowerCase()][0].schools;
            $('.target-text h4').text($('.target-text h4').text().replace(replaceStateRx, _this.options.selectlocation));
            updateTargetStr =  _this.options.selectlocation;
        }
        $.when($.each(schoolArr, function (key, value) {
                if (!value.name) {
                    return;
                }
                var fullAddress = null,
                    _url = typeof value.url === 'string' ? value.url : null;
                /* DefaultMap doesn't use list, build html content if false*/
                if (!_this.options.showDefaultMap) {
                    /*build html content*/
                    htmlOut += buildContent(value);
                }
                /*build address string to qry map location point*/
                if (value.address) {
                    fullAddress = (typeof value.name === 'string' ? value.name + ', ' + value.address : value.address);
                } else {
                    fullAddress = (typeof value.name === 'string' ? value.name + ', ' + value.state : value.state);
                }
            })).done(function () {
                if (htmlOut.toLowerCase().indexOf('google-arrow-button') === -1) {
                    /*remove extra padding when no description */
                    htmlOut = htmlOut.replace(/class="add-padding"/g, '');
                }
                /*update state msg*/
                $('.target-text span').html('');
                if (stateObj.state === _this.options.selectlocation.toLowerCase() && stateObj.description) {
                    $('.target-text span').html('<p>' + stateObj.description + '</p>');
                }
                /*output list when available*/
                document.getElementById(_this.options.htmlPlaceHolder).innerHTML = htmlOut;
            });
    }
    /*build list for html content*/
    function buildContent(value) {
        /* Get clean template and link to blank defult */
        var htmlTmplt = _this.options.listHtmlMarkupTemplate,
            sOnclick = '',
            Add508 = ' alt= "' + value.name + '" title = "' +  value.name + '" ' + ' id = "' + stripString(value.name) + '" ',
            slink = '<a class="add-padding"' + Add508 + '>' + value.name + '</a>';
        /* update when found and keep for later */
        if (value.url) {
            slink = '<a class="add-padding"' + Add508 + 'href="' + value.url + '" target="_blank" rel="external">' + value.name;
            slink += '&nbsp; <i' + Add508 + ' class="fa fa-lg fa-external-link"></i></a>';
        }
        /*add show/hide toggle*/
        if (value.description && value.description.length > 0) {
            sOnclick = 'onclick="$(\'span\', this.parentElement).toggleClass(\'google-map-span\'); $(this).toggleClass(\'google-arrow-down\')"';
            sOnclick = '<a class="google-arrow-button" ' +  sOnclick + '></a>';
            slink = slink.replace('class="add-padding"', '');
            $('.google-place-holder span').addClass('add-padding');
            htmlTmplt = htmlTmplt.replace(/{click_anchor}/g, sOnclick).replace(/{content}/g, value.description);
        } else {
            /*clean unused tags*/
            htmlTmplt = htmlTmplt.replace(/{click_anchor}/g, '').replace(/{content}/g, '');
        }
        htmlTmplt = htmlTmplt.replace(/{link_open}/g, slink);
        return htmlTmplt;
    }
    /* helper removes spaces and nummber from string with is used to match address to marker*/
    function stripString(str) {
        if (str) {
            str = str.replace(/\s+/g, '').replace(/\d+/g, '');
            return str.replace(/\,/g, '').toLowerCase();
        }
        return;
    }
    function statusLog(context, message) {
        if (console && context && message) {
            console.log(context, message);
        }
    }
    window.asaSeniorBorrower = asaSeniorBorrower;
});