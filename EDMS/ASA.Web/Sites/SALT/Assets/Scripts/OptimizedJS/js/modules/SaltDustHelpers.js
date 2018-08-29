/* jshint maxstatements: 55 */
define([
    'dust',
    'asa/ASAUtilities',
    'underscore',
    'configuration',
    'actionWords',
    'dust-helpers'
], function (dust, Utility, _, Configuration, actionWords) {
    dust.filters.refinementState = function (value) {
        return value.substring(value.indexOf('/_/'), value.indexOf('?'));
    };

    dust.filters.shorten = function (value) {
        var lastSpace = value.lastIndexOf(' '),
            firstDash = value.indexOf('-');
            //If there's a dash, and its before the last space, cutoff at dash, otherwise use last space.
            //Sometimes the dash is after the last space eg. "Texarkana TX-AR"
        if (firstDash > -1 && firstDash < lastSpace) {
            return value.substring(0, firstDash);
        }
        return value.substring(0, lastSpace);
    };

    dust.filters.actionType = function (contentType) {
        var actionText = actionWords[contentType[0]];
        if (!actionText) {
            return 'try';
        } else { 
            return actionText;
        }
    };

    dust.filters.contentMainNav = function (value) {
        var contentMap = {
            article: 'N-15',
            video: 'N-16',
            lesson: 'N-18',
            form: 'N-17',
            comic: 'N-4d',
            infographic: 'N-19',
            tool: 'N-1A'
        };
        return contentMap[value.toLowerCase()];
    };

    dust.filters.trim = function (value) {
        if (Array.isArray(value)) {
            return value[0].trim();
        }
        return value.trim();
    };

    dust.filters.contentRefineNav = function (value) {
        var toReturn = '';
        var pathArray = value.split('/');
        if (pathArray) {
            toReturn = pathArray[pathArray.length - 1].split('?')[0];
        }
        return toReturn;
    };

    //potentially leverage helper with params instead to combine these two filters
    dust.filters.returnFirstFlatTag = function (value) {
        var arrayvalue = 0;
        var delim = '|';
        var toReturn = '';
        var parsedArray = value.split(delim);
        if (parsedArray && parsedArray.length > arrayvalue) {
            toReturn = parsedArray[arrayvalue];
            toReturn = toReturn.trim();
        }
        return toReturn;
    };

    dust.filters.returnSecondFlatTag = function (value) {
        var arrayvalue = 1;
        var delim = '|';
        var toReturn = '';
        var parsedArray = value.split(delim);
        if (parsedArray && parsedArray.length > arrayvalue) {
            toReturn = parsedArray[arrayvalue];
            toReturn = toReturn.trim();
        }
        return toReturn;
    };

    //This function is duplicated in the server helpers file at /Content/salt-dust-helpers.js
    //DO NOT MODIFY WITHOUT MODIFYING THAT FUNCTION AS WELL.  BETTER YET, COMBINE THESE FILES PLEASE
    dust.filters.currencyComma = Utility.currencyComma;

    dust.helpers.pathname = function (chunk, ctx, bodies, params) {
        return chunk.write(($.browser.msie) ? Utility.getPathnameHash(location, true) : Utility.getPathnameHash(location, false));
    };

    dust.filters.dc = function (value) {
        return $($.parseHTML(value)).text();
    };

    dust.filters.lc = function (value) {
        return value.toLowerCase();
    };

    dust.filters.us = function (value) {
        return value.replace(/ /g, '_');
    };

    dust.filters.ceil = function (value) {
        if (isNaN(value)) {
            return value;
        }
        return Math.ceil(value);
    };

    dust.filters.abs = function (value) {
        if (isNaN(value)) {
            return value;
        }
        return Math.abs(value);
    };

    //Write out a count of the results, omitting definitions
    dust.filters.countWithoutDefinitions  = function (secondaryContent) {
        var refinements = Utility.checkNested(secondaryContent, 'navigation[0].refinements'),
            //if it's not search result page, we don't need short page spacer in tiles.dust
            count = location.pathname.indexOf('content/search') > -1 ? 0 : '';
        if (refinements && refinements.length) {
            count = _.reduce(refinements, function (runningCount, obj) {
                if (obj.label !== 'Definition') {
                    return runningCount + obj.count;
                } else {
                    return runningCount;
                }
            }, 0);
        }
        return count;
    };

    dust.helpers.glossaryTile = function (chunk, ctx, bodies, params) {
        var searchTerm = this.tap(params.search, chunk, ctx),
            adjustedSearches = params.adjustedSearches,
            records = this.tap(params.records, chunk, ctx),
            termToDisplay = {},
            out = '',
            searchTermArray = '';

        if (searchTerm) {
            searchTerm = searchTerm.toLowerCase();
        } else {
            return chunk.write(out);
        }

        var definitions = _.filter(records, function (rec) {
            return rec.attributes.ContentTypes[0].toLowerCase() === 'definition';
        });

        if (adjustedSearches) {
            searchTerm = getAdjustedTerm(adjustedSearches);
            //SWD-7641 - adjusted terms may come back with double quotes
            searchTerm = searchTerm.replace(/"/g, '');
        }

        //SWD-7640: Strip out all non alpha numeric characters from search term for comparison
        var searchTermNoSpaces = searchTerm.replace(/[\W_]+/g, '');

        //Try to find an exact match for the search term or find the searchTerm within the acronym first
        termToDisplay = _.find(definitions, function (definition) {
            var termNoSpaces = definition.attributes.term[0].trim().toLowerCase().replace(/[\W_]+/g, ''),
                acronymNoSpaces = definition.attributes.acronym[0].trim().toLowerCase().replace(/[\W_]+/g, '');

            return termNoSpaces === searchTermNoSpaces ||
            acronymNoSpaces.indexOf(searchTermNoSpaces) > -1;
        });

        if (!termToDisplay) {
            termToDisplay = definitions[0];
            //if we're defaulting the definition record, check to see if the search term is a part of the defaulted record's
            //"term" attribute (search each token of the search term). This is for SWD-6668 and SWD-7641
            searchTermArray = searchTerm.split(' ');
            _.each(searchTermArray, function (searchTermItem) {
                if (termToDisplay.attributes.term[0].trim().toLowerCase().indexOf(searchTermItem) > -1) {
                    termToDisplay.display = true;
                }
            });
        } else {
            termToDisplay.display = true;
        }

        //SWD 6668 - If search term and acronym are not a part of the definition's term, don't display definition treatment
        if (termToDisplay.display) {
            out = '<div class="definition-body"><h2><a href="/home/glossary.html#' + termToDisplay.attributes.letter[0].substr(1, 1) + '" title="' + termToDisplay.attributes.resource_link_title[0] + '">' + termToDisplay.attributes.term[0].trim() +
                ':</a> (Definition)</h2>' + termToDisplay.attributes.body[0] + "</div>";
        }

        return chunk.write(out);
    };

    //This function is duplicated WITH SOME CHANGES in the server helpers file at /Content/salt-dust-helpers.js
    //DO NOT MODIFY WITHOUT MODIFYING THAT FUNCTION AS WELL.  BETTER YET, COMBINE THESE FILES PLEASE
    /*
    accepts the following parameters:
    - startYear
    - range
    - gradDate (grad date as saved in DB. To be used for graduation year only)
    - savedValue (to be usedfor non-gradDate year dropdowns)
    */
    dust.helpers.yearDropdown = function (chunk, ctx, bodies, params) {
        var manip = this.tap(params.range, chunk, ctx),
            absolute = manip / Math.abs(manip),
            year =  new Date().getFullYear(),
            startYear = parseInt(this.tap(params.startYear, chunk, ctx), 10),
            gradDate = this.tap(params.gradDate, chunk, ctx),
            savedValue = this.tap(params.savedValue, chunk, ctx),
            stringToWrite = '';

        if (startYear) {
            year += startYear;
        }
        if (gradDate) {
            savedValue = gradDate;
        }
        savedValue = savedValue ? parseInt(savedValue, 10) : null;

        for (var i = 0; i < Math.abs(manip) + 1; i++) {
            if ((year + absolute * i) === savedValue) {
                stringToWrite += '<option value="' + (year + absolute * i) + '" selected >' + (year + absolute * i) + '</option>';
            } else {
                stringToWrite += '<option value="' + (year + absolute * i) + '">' + (year + absolute * i) + '</option>';
            }
        }
        return chunk.write(stringToWrite);
    };

    dust.helpers.dateParser = function (chunk, ctx, bodies, params) {
        /* helper to parse date from the  server format to a user friendly format*/
        var myDate = this.tap(params.utcDate, chunk, ctx);
        var parsedDate = Utility.convertSALtoJSdate(myDate);
        return chunk.write(parsedDate.toDateString());
    };

    dust.helpers.dateLinkChooser = function  (chunk, ctx, bodies, params) {
        var enrollStatus = this.tap(params.enrollStatus, chunk, ctx);
        var link = (enrollStatus === 'Enrolled') ? '#WhenWillYouGrad' : '#LastAttended';

        return chunk.write(link);
    };
    dust.helpers.urlParser = function (chunk, ctx, bodies, params) {
        /* helper to parse url from the data and normalize it - written for scholarship search*/
        var url = this.tap(params.url, chunk, ctx);
        if (url.substring(0, 4) === 'http') {
            return chunk.write(url);
        }
        else {
            return chunk.write('http://' + url);
        }
    };

    dust.helpers.contactParser = function (chunk, ctx, bodies, params) {
        //format contact data coming back from Unigo
        var populatedFields = {}, contact = '', address = '', cityStateZip = '';

        for (var key in params) {
        //null and undefined are already null
        //leave out other 'null' data
            if (params[key] && params[key] !== '-' && params[key] !== '- ') {
                populatedFields[key] = params[key];
            }
        }
        if (populatedFields.providerName) {
            contact = populatedFields.providerName + "<br>";
        }
        //if  contactPerson exists, don't use the providerName
        if (populatedFields.contactPerson) {
            contact = "Attn: " + populatedFields.contactPerson + "<br>";
        }
        if (populatedFields.contactTitle) {
            contact += populatedFields.contactTitle  + "<br>";
        }
        if (contact) {
            contact += '<br>';
        }
        if (populatedFields.address1) {
            address = populatedFields.address1;
        }
        //add address2 to address1 without a comma, per the comp
        if (populatedFields.address1 && populatedFields.address2) {
            address += ' ' + populatedFields.address2;
        }
        if (address) {
            address += '<br>';
        }
        if (populatedFields.city) {
            cityStateZip += populatedFields.city;
        }
        if (populatedFields.province) {
            cityStateZip += ', ' + populatedFields.province;
        }
        if (populatedFields.state) {
            cityStateZip += ', ' + populatedFields.state;
        }
        if (populatedFields.zipCode) {
            cityStateZip += ' ' + populatedFields.zipCode;
        }
        if (cityStateZip) {
            cityStateZip += '<br>';
        }
        //if address is null and there is no email or phone we don't need a second <br>, otherwise we do.
        if (address && populatedFields.email1 || populatedFields.phone1) {
            cityStateZip += '<br>';
        }
        return chunk.write(contact + address + cityStateZip);
    };

    dust.helpers.substr = function (chunk, ctx, bodies, params) {
        // Get the values of all the parameters. The tap function takes care of resolving any variable references
        // used in parameters (e.g. param="{name}"
        var str = this.tap(params.str, chunk, ctx),
            begin = dust.helpers.tap(params.begin, chunk, ctx),
            end = dust.helpers.tap(params.end, chunk, ctx),
            len = dust.helpers.tap(params.len, chunk, ctx);
        begin = begin || 0; // Default begin to zero if omitted
        // Use JavaScript substr if len is supplied.
        // Helpers need to return some value using chunk. Here we write the substring into chunk.
        // If you have nothing to output, just return chunk.write("");
        if ((typeof(len) !== 'undefined')) {
            return chunk.write(str.substr(begin, len));
        }
        if ((typeof(end) !== 'undefined')) {
            return chunk.write(str.slice(begin, end));
        }
        if ((typeof(end) === 'undefined') && (typeof(len) === 'undefined')) {
            return chunk.write(str.substr(begin));
        }
        return chunk.write(str);
    };

    dust.helpers.dateUS = function (chunk, ctx, bodies, params) {
        //Converts 2004-08-10 into 08/10/2004
        var dateStr = this.tap(params.dateString, chunk, ctx),
            year =  dateStr.substr(0, 4),
            month = dateStr.substr(5, 2),
            day = dateStr.substr(8, 2);

        return chunk.write(year + '/' + month + '/' + day);
    };

    //This function is duplicated in the server helpers file at /Content/salt-dust-helpers.js
    //DO NOT MODIFY WITHOUT MODIFYING THAT FUNCTION AS WELL.  BETTER YET, COMBINE THESE FILES PLEASE
    dust.helpers.flatTagsSeperator = function (chunk, ctx, bodies, params) {
        //seperates the Flat Tags and convert them into searchable Anchor Tags
        var tags = this.tap(params.str, chunk, ctx),
            tagStr = '';
        if (tags !== ' ' && (typeof(tags) === 'string')) {
            var tagArray = tags.split('|'),
                tagLinks = tagArray[0],
                startTag = '<a class="js-SPA" href="/content/search?ntk=Tags&searchCriteria=';
            tagStr = startTag + encodeURIComponent(tagArray[0].trim()) + '" >' + tagArray[0] + '</a>';
            for (var i = 1; i < tagArray.length; i++) {
                tagLinks += ',' +  tagArray[i];
                if ((tagLinks.trim()).length <= 50 && (ctx.templateName === 'oldTiles' || ctx.templateName === 'partial_todo')) {
                    tagStr += ', ' + startTag + encodeURIComponent(tagArray[i].trim()) + '" >' + tagArray[i] + '</a>';
                } else if (ctx.templateName !== 'oldTiles' && ctx.templateName !== 'partial_todo') {
                    tagStr += ', ' + startTag + encodeURIComponent(tagArray[i].trim()) + '" >' + tagArray[i] + '</a>';
                }
            }
        }
        return chunk.write(tagStr);
    };

    dust.helpers.featuredTaskImage = function (chunk, ctx, bodies, params) {
        var tags = this.tap(params.tagList, chunk, ctx),
            primaryKey = this.tap(params.primaryKey, chunk, ctx),
            lookupkey = '',
            imageName = '',
            imageAlt = '';

        if (primaryKey !== ' ' && (typeof(primaryKey) === 'string')) {
            var toolName = '',
                altName = '';
            switch (primaryKey) {
            case '101-8645':
                toolName = 'repaymentnavigator',
                altName = 'Repayment Navigator';
                break;
            case '101-12042':
                toolName = 'salaryestimator',
                altName = 'Salary Estimator';
                break;
            case '101-7416':
                toolName = 'scholarshipsearch',
                altName = 'Scholarship Search';
                break;
            case '101-4367':
                toolName = 'savingscalculator',
                altName = 'Savings Calculator';
                break;
            case '101-13584':
                toolName = 'kwyo',
                altName = "Debt Organizer";
                break;
            default:
            }
            if (toolName) {
                imageName = 'icon_' + toolName;
                imageAlt = 'Icon for ' + altName + 'tool';
            }
        }

        // if we don't have a specific image for the tool, then
        // pick one by the tags
        if (!imageName && tags !== ' ' && (typeof(tags) === 'string')) {
            var tagArray = tags.split('|'),
                firstTag = tagArray[0].replace(/\s/g, ''),
                altText = '';

            switch (firstTag) {
            case 'credit':
            case 'creditscores':
                firstTag = 'creditdebt',
                altText = 'Credit Debt';
                break;
            case 'checkingaccounts':
                firstTag = 'banking',
                altText = 'Credit Debt';
                break;
            case 'resumes':
            case 'careerskills':
            case 'careerchoice':
                firstTag = 'jobsearch',
                altText = 'Job Search';
                break;
            case 'scholarships':
                firstTag = 'scholarshipsearch',
                altText = 'Scholarship Search';
                break;
            case 'collegesavings':
                firstTag = 'savings',
                altText = 'College Savings';
                break;
            case 'postponements':
            case 'cancellations':
                firstTag = 'studentloanrepayment',
                altText = 'Student Loan Repayment';
                break;


            default:
            }
            imageName = 'icon_' + firstTag,
            imageAlt = 'Icon for ' + altText + 'tool';
        }
        var htmlOut = '<i role="image" class="small-12 salticon featuredIcon large ' + imageName + '" ' + 'aria-label="' + imageAlt + '"></i>';
        return chunk.write(htmlOut);
    };
    //find the tags for the content on the page
    //map the gloat tags to the goals
    // add the goals to the gloat image so that the image can be displayed.
    dust.helpers.gloatImage = function (chunk, ctx, bodies, params) {
        var tags = this.tap(params.tagList, chunk, ctx),
            psTags = Configuration.goalTags.PayForSchool,
            rsdTags = Configuration.goalTags.RepayStudentDebt,
            fjTags = Configuration.goalTags.FindAJob,
            mmTags = Configuration.goalTags.MasterMoney;
           
        // Default if there are no tags
        var gloatHtml = '<img src="/assets/images/sidebar/join-salt1.jpg" title="Join Salt." alt="Join Salt"/>';        
        
        // tags exist    
        if (tags !== ' ' && (typeof(tags) === 'string')) {
            var tagArray = tags.split('|').map(function (nospace) {
                    return nospace.trim();
                }),
                primaryTag = tagArray[0],
                rsdTagsArray = rsdTags.split(','),
                psTagsArray = psTags.split(','),
                fjTagsArray = fjTags.split(','),
                mmTagsArray = mmTags.split(','),
                gloatGoal = '';

            if (rsdTagsArray.indexOf(primaryTag) !== -1) {
                gloatGoal = 'repaystudentdebt-join';
            } else if (psTagsArray.indexOf(primaryTag) !== -1) {
                gloatGoal = 'payforschool-join';
            } else if (fjTagsArray.indexOf(primaryTag) !== -1) {
                gloatGoal = 'findajob-join';
            } else if (mmTagsArray.indexOf(primaryTag) !== -1) {
                gloatGoal = 'managemoney-join';
            } else {
                gloatGoal = 'join-salt1';
            }

            gloatHtml = '<img src="/assets/images/sidebar/' + gloatGoal + '.jpg" title="Join Salt." alt="Join Salt"/>';
        }
        return chunk.write(gloatHtml);
    };

    dust.helpers.personaButtons = function (chunk, ctx, bodies, params) {
        var links = this.tap(params.links, chunk, ctx),
            copy = this.tap(params.copy, chunk, ctx),
            out = '';

        //Strip <p> that CM1 puts in
        copy = copy.substr(copy.indexOf('<p>') + 3, copy.indexOf('</p>') - 4).split('|');

        links.trim().split('|').forEach(function (el, ind, arr) {
            if (el === 'Home') {
                el = '';
            }
            out += '<div class="unauth-home-btn-wrapper small-6 columns">' +
              '<a href="index.html?Type=' + el.replace(/\s/g, '') + '" class="home-link button base-btn main-btn unauth-home-btn js-persona-btn">' +
            copy[ind] + '</a></div>';
        });
        return chunk.write(out);
    };

    dust.helpers.personaNav = function (chunk, ctx, bodies, params) {
        var links = this.tap(params.links, chunk, ctx),
            copy = this.tap(params.copy, chunk, ctx),
            out = '';

        links = links.trim();

        if (copy.indexOf('<p>') > -1) {
            copy = copy.substr(copy.indexOf('<p>') + 3, copy.indexOf('</p>') - 4);
        }
        copy = copy.trim().split('|');
        links.split('|').forEach(function (el, ind, arr) {
            if (el === 'Home') {
                el = '';
            }
            out += '<li><a href="/index.html?Type=' + el + '" class="home-link" title="' + copy[ind] + '">' + copy[ind] + '<i class="fa fa-caret-up"></i></a></li>';
        });
        return chunk.write(out);
    };

    /*this helper is for outputing keys*/
    dust.helpers.iterate = function (chunk, context, bodies, params) {
        params = params || {};
        var obj = params.on || context.current();
        for (var k in obj) {
            chunk = chunk.render(bodies.block, context.push({key: k, value: obj[k]}));
        }
        return chunk;
    };

    /*this helper is for outputing keys*/
    dust.helpers.checkProfileSelect = function (chunk, ctx, bodies, params) {
        params = params || {};
        var responses = params.responsesObj,
            ansID  = this.tap(params.ansVal, chunk, ctx),
            lastAnswer;
        if (responses.length) {
            var lastResponse = responses[responses.length - 1];
            lastAnswer = lastResponse.QuestionExternalId + '-' + lastResponse.AnsExternalId;
            if (ansID && lastAnswer && ansID === lastAnswer) {
                return chunk.write('selected');
            }
        }
        return chunk.write('');
    };

    /*this helper is for outputing keys*/
    dust.helpers.isProfileInputChecked = function (chunk, ctx, bodies, params) {
        params = params || {};
        var responses = params.responsesObj,
            ansID  = this.tap(params.ansVal, chunk, ctx);
        for (var i = 0; i < responses.length; i++) {
            if (responses[i].QuestionExternalId + '-' + responses[i].AnsExternalId === ansID) {
                return chunk.write('checked');
            }
        }
        return chunk.write('');
    };

    /*this helper is for outputing keys*/
    // AnswerName field is set to "Other" or "None" for answers where other/none behavior is needed
    dust.helpers.profileSpecialAnswer = function (chunk, ctx, bodies, params) {
        var ansType  = this.tap(params.ansType, chunk, ctx);
        if (ansType === 'None' || ansType === 'Other') {
            return chunk.write('js-' + ansType.toLowerCase());
        }
        return chunk.write('');
    };

     /*return correct div (for text styling) on /home */
    dust.helpers.dynamicSchoolOutput = function (chunk, ctx, bodies, params) {
        var CurrentSchoolBrand = this.tap(params.CurrentSchoolBrand, chunk, ctx),
            out = '',
            noLogoCase = this.tap(params.noLogoCase, chunk, ctx),
            schoolCase = this.tap(params.schoolCase, chunk, ctx),
            querystring = Utility.getLocationSearch();
        //checks to see if there is an url parameter, no school specified or school has no logo
        //if none then output the standard ASA text style
        if (Utility.getLocationSearch() && Utility.getLocationSearch().indexOf('Type=') !== -1 && Utility.getParameterByNameFromString('Type', querystring) !== 'All' || !CurrentSchoolBrand || CurrentSchoolBrand === 'nologo') {
            out = noLogoCase;
        }
        else {
            //otherwise output the school overlay style
            out = schoolCase;
        }
        return chunk.write(out);
    };


        /* returns '&s=1' to url if there is already '?' in the url */
    /* otherwise returns '?s=1' */
    dust.helpers.fixSpanishQuerystring  = function (chunk, ctx, bodies, params) {
        var url = this.tap(params.url, chunk, ctx), existingQuerystring = url.indexOf('?') > -1;
        if (existingQuerystring) {
            url += '&s=1';
        } else {
            url += '?s=1';
        }
        return chunk.write(url);
    };

    //We want to output the unknown option if the user has a GradYear of 1900
    dust.helpers.checkGradYear = function (chunk, ctx, bodies, params) {
        var gradDate = this.tap(params.gradDate, chunk, ctx),
            gradYear = gradDate ? gradDate : null;

        gradYear = gradYear ? parseInt(gradYear, 10) : null;

        if (gradYear === 1900) {
            return chunk.write('selected');
        }
        return chunk;
    };

    // check if the tag has been selected for tag tray
    dust.helpers.tagSelectedCheck = function (chunk, ctx, bodies, params) {
        var currentTagFromURL = Utility.getParameterByNameFromString('Tag', Utility.getLocationSearch()),
            tag = this.tap(params.tag, chunk, ctx);
        if (currentTagFromURL && currentTagFromURL === tag) {
            return chunk.write('selected');
        }
        return chunk;
    };

    //Live Chat dust helpers.
    dust.helpers.liveChatAccount = function (chunk, ctx, bodies, params) {
        return chunk.write(Configuration.liveChatProperty.AccountID);
    };
    dust.helpers.liveChatWindow = function (chunk, ctx, bodies, params) {
        var adKey = this.tap(params.adKey, chunk, ctx);
        if (adKey) {
            return chunk.write(Configuration.liveChatProperty.WindowAuthID);
        } else {
            return chunk.write(Configuration.liveChatProperty.WindowUnAuthID);
        }
    };
    dust.helpers.liveChatDepartment = function (chunk, ctx, bodies, params) {
        return chunk.write(Configuration.liveChatProperty.DepartmentID);
    };
     /*return correct image on /home */
    dust.helpers.dynamicBanner = function (chunk, ctx, bodies, params) {
        var CurrentSchool = this.tap(params.CurrentSchool, chunk, ctx),
            CurrentSchoolBrand = this.tap(params.CurrentSchoolBrand, chunk, ctx),
            mediumAvailable = this.tap(params.mediumAvailable, chunk, ctx),
            CMImage = this.tap(params.CMImage, chunk, ctx).replace('src=', 'nosrc=').trim(),
            $cmImage = $(CMImage),
            src = $cmImage.is('img') ? $cmImage.attr('nosrc') : $cmImage.find('img').attr('nosrc'),
            out = '',
            querystring = Utility.getLocationSearch();
        //checks to see if the url parameter has type parameter, or no school specified or school has no logo.
        //if a condition is met, use the reponsive image output
        if ((querystring.indexOf('Type=') !== -1 && Utility.getParameterByNameFromString('Type', querystring) !== 'All') || !CurrentSchoolBrand || CurrentSchoolBrand === 'nologo') {
            out = outputResponsiveImage(src, mediumAvailable);
        }
        //otherwise output the school-provided image
        else {
            $.ajax({ url: '/assets/images/backgrounds/' + CurrentSchoolBrand + '-web.jpg', async: false})
                .done(function () {
                    out = '<img class="responsive-image" src="/assets/images/backgrounds/' + CurrentSchoolBrand + '-web.jpg" alt="' + CurrentSchool + '" title="' + CurrentSchool + '" />';
                }).fail(function () {
                    out = outputResponsiveImage(src, mediumAvailable);
                });
        }
        return chunk.write(out);
    };

    dust.helpers.responsiveImage = function (chunk, ctx, bodies, params) {
        // Replacing "src" attributes with "nosrc" to avoid making the http request until the desired HTML is built
        var imageTag = this.tap(params.imageSource, chunk, ctx).replace('src=', 'nosrc=').trim(),
            altVal = this.tap(params.altVal, chunk, ctx),
            mediumAvailable = this.tap(params.mediumAvailable, chunk, ctx),
            sourceName = $(imageTag).is('img') ? $(imageTag).attr('nosrc') : $(imageTag).find('img').attr('nosrc');
        return chunk.write(outputResponsiveImage(sourceName, mediumAvailable, altVal));
    };
    function outputResponsiveImage(sourceName, mediumAvailable, altVal) {
        //var stringToReturn = sourceName;
        if (sourceName) {
            var lastIndexOfSlash = sourceName.lastIndexOf('/'),
                lastIndexOfDot = sourceName.lastIndexOf('.'),
                filePath = sourceName.substring(0, lastIndexOfDot),
                ext = sourceName.substring(lastIndexOfDot),
                // if medium is not available, use the large image
                mediumDefault = mediumAvailable ? '-medium' : '';
            if (altVal) {
                altVal = altVal.replace('<h1>', '').replace('</h1>', '').trim();
                sourceName = '<picture><source media="(min-width: 1026px)" srcset="' + filePath + ext +
                '"><source media="(min-width: 480px)" srcset="' + filePath + mediumDefault + ext +
                '"><img class="responsive-image" src="' + filePath + '-small' + ext +
                '" alt="' + altVal + '" title="' + altVal + '"></picture>';
            } else {
                sourceName = '<picture><source media="(min-width: 1026px)" srcset="' + filePath + ext +
                '"><source media="(min-width: 480px)" srcset="' + filePath + mediumDefault + ext +
                '"><img class="responsive-image" src="' + filePath + '-small' + ext +
                '"></picture>';
            }
        }
        return sourceName;
    }
    dust.helpers.sizeImage = function (chunk, ctx, bodies, params) {
        // Replacing "src" attributes with "nosrc" to avoid making the http request until the desired HTML is built
        var imageTag = this.tap(params.imageSource, chunk, ctx).replace('src=', 'nosrc=').trim(),
            sizeWanted = this.tap(params.size, chunk, ctx),
            sourceName = '',
            $imgTag = $(imageTag), 
            size = sizeWanted === 'small' ? '-small' : '';
        sourceName = $imgTag.is('img') ? $imgTag.attr('nosrc') : $imgTag.find('img').attr('nosrc');

        if (!sourceName) {
            sourceName = '';
        }
        altVal = $imgTag.is('img') ? $imgTag.attr('alt') : $imgTag.find('img').attr('alt');
        var lastIndexOfSlash = sourceName.lastIndexOf('/'),
            lastIndexOfDot = sourceName.lastIndexOf('.'),
            filePath = sourceName.substring(0, lastIndexOfDot),
            ext = sourceName.substring(lastIndexOfDot);
        if (altVal) {
            altVal = altVal.replace('<h1>', '').replace('</h1>', '').trim();
            sourceName = '<img src="' + filePath + size + ext + '" alt="' + altVal + '">';
        } else {
            sourceName = '<img src="' + filePath + size + ext + '">';
        }

        return chunk.write(sourceName);
    };
    dust.helpers.profileOtherInputVisibilty = function (chunk, ctx, bodies, params) {
        var responses = params.responsesObject,
            otherAnswer = _.filter(responses, function (response) {
                return response.AnsName === 'Other';
            });
        if (otherAnswer.length) {
            return chunk.write('');
        }
        return chunk.write('hide');
    };

    //This helper is used both client side and in the nodejs layer, do not modify one without checking the other
    dust.helpers.postedDateFormatter = function (chunk, ctx, bodies, params) {
        var dateObj = new Date(this.tap(params.date, chunk, ctx)),
            lng = this.tap(params.lng, chunk, ctx),
            monthNames = ['January', 'February', 'March', 'April', 'May', 'June', 'July', 'August', 'September', 'October', 'November', 'December'],
            nombres = ['Enero', 'Febrero', 'Marzo', 'Abril', 'Mayo', 'Junio', 'Julio', 'Agosto', 'Septiembre', 'Octubre', 'Noviembre', 'Diciembre'],
            month = monthNames[dateObj.getMonth()],
            //CM1 passes version number as a string thats the first element of an array, e.g. ["1"]
            version = parseInt(this.tap(params.version, chunk, ctx)[0], 10),
            //'Published' shoud be english or spanish depending on paramter
            publishOrUpdated = (lng === 'spanish') ? 'Publicado: ' : 'Published: ';

        //Determine if the piece of content is newly published or has been updated
        //Look at version number, if not 1, its updated, adjust text accordingly
        if (version !== 1) {
            if (lng === 'spanish') {
                publishOrUpdated = 'Actualizado: ';
            } else {
                publishOrUpdated = 'Updated: ';
            }
        }
            //Output date formatted like "June 5, 2014"
            //or Febrero 29, 1996
        if  (lng === 'spanish') {
            return chunk.write(publishOrUpdated + nombres[monthNames.indexOf(month)] + ' ' + dateObj.getDate() + ', ' + dateObj.getFullYear());
        }
        return chunk.write(publishOrUpdated + month + ' ' + dateObj.getDate() + ', ' + dateObj.getFullYear());
    };

    //This helper is used both client side and in the nodejs layer, do not modify one without checking the other
    dust.helpers.getCurrentYear = function (chunk, ctx, bodies, params) {
        return chunk.write(new Date().getFullYear());
    };

    dust.helpers.contentLength = function (chunk, ctx, bodies, params) {
        var phrase = this.tap(params.phrase, chunk, ctx);
        if (phrase.length > 31) {
            return chunk.write('large');
        }
        return chunk.write('');
    };

    dust.helpers.searchResultsHeader = function (chunk, ctx, bodies, params) {
        var originalSearchTerm = this.tap(params.origTerm, chunk, ctx),
            numRecords = this.tap(params.numRecords, chunk, ctx),
            resultsHeading = '';

        if (params.termObject) {
            var termObject = this.tap(params.termObject, chunk, ctx),
                spellCorrected = false,
                autoPhrased = false,
                dymTerm = '';

            _.each(_.keys(termObject), function (term) {
                spellCorrected = termObject[term][0].spellCorrected;
                autoPhrased = termObject[term][0].autoPhrased;
                dymTerm = termObject[term][0].adjustedTerms.replace(/"/g, '');
            });

            if (spellCorrected || (autoPhrased && dymTerm.toLowerCase() !== originalSearchTerm.toLowerCase())) {
                resultsHeading = 'Your search:<br> ' + originalSearchTerm + '</br>';
                resultsHeading += 'Showing results for:<br> <strong>' + dymTerm + '</strong> (' + numRecords + ')';
                return chunk.write(resultsHeading);
            }
        }

        //if the originalSearchTerm was empty, the origTerm param resolves to "false" and we don't want
        //to display that to the user.
        if (!originalSearchTerm) {
            originalSearchTerm = '';
        }

        resultsHeading = 'Your search:<br/> <strong>' + originalSearchTerm + '</strong> (' + numRecords + ')';

        return chunk.write(resultsHeading);
    };

    dust.helpers.SALTTour = function (chunk, ctx, bodies, params) {
        var saltTourPresent = 0,
            mainContentArray = this.tap(params.mainContent, chunk, ctx);

        for (var i = 0, tot = mainContentArray.length; i < tot; i++) {
            if (mainContentArray[i].name === 'PromoTile') {
                for (var x = 0, objectNum = mainContentArray[i].records.length; x < objectNum; x++) {
                    if (mainContentArray[i].records[x].attributes.page_title[0] === 'Salt tour') {
                        saltTourPresent++;
                    }
                }
            }
        }

        //check our counter to see if salttour was one of the records
        if (saltTourPresent === 0) {
            chunk.render(bodies.block, ctx);
        }

        return chunk;
    };

    function getAdjustedTerm(misspelledTerm) {
        var dymTerm = '';

        _.each(_.keys(misspelledTerm), function (term) {
            dymTerm = misspelledTerm[term][0].adjustedTerms;
        });

        return dymTerm;
    }

    dust.helpers.returnAnswersForQuestion = function (chunk, ctx, bodies, params) {
        var questionId = this.tap(params.questionId, chunk, ctx),
            answersArray = this.tap(params.answersArray, chunk, ctx),
            toReturn = '';
        if (answersArray && questionId) {
            _.each(answersArray[questionId - 1], function (element, index, list) {
                toReturn += '<option value="' + element.id + '">' + element.description + '</option>';
            });
        }
        return chunk.write(toReturn);
    };

    dust.helpers.returnOrgLogos = function (chunk, ctx, bodies, params) {

        var member = ctx.get("SiteMember");

        var logoOrgs = [];

        if (member !== undefined) {
            var orgs = ctx.get("SiteMember").Organizations;

            logoOrgs = Utility.returnBrandedOrganizations(orgs);
        } else {
            config = ctx.get("Configuration");
            if (config.CurrentSchoolBrand !== "nologo") {
                var org;
                org.OrgnizationName = config.CurrentSchool;
                org.Brand = config.CurrentSchoolBrand;
                logoOrgs = [org];
            }

        }

        var length = logoOrgs.length;

        _.each(logoOrgs, function (element, index, list) {
            element.orgIndex = index;
            element.orgLength = length;
            chunk.render(bodies.block, ctx.push(element));
        });
        return chunk;
    };
});
