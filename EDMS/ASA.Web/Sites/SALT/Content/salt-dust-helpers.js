var dust = require('dustjs-linkedin'),
    actionWords = require('../Assets/Scripts/js/actionWords'),
    fs = require('fs');
require('dustjs-helpers');

//This function is duplicated in the client helpers file at /Assets/Scripts/js/modules/SaltDustHelpers.js
//DO NOT MODIFY WITHOUT MODIFYING THAT FUNCTION AS WELL.  BETTER YET, COMBINE THESE FILES PLEASE
dust.filters.currencyComma = function (value) {
    return value.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ',');
};

dust.filters.actionType = function (value) {
    var actionText = actionWords[value[0]];
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
    if (parsedArray  && parsedArray.length > arrayvalue) {
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

dust.filters.lc = function (value) {
    return value.toLowerCase();
};

dust.filters.us = function (value) {
    return value.replace(/ /g, '_');
};

dust.filters.trim = function (value) {
    if (Array.isArray(value)) {
        return value[0].trim();
    }
    return value.trim();
};

dust.helpers.dateFormatter = function (chunk, context, bodies, params) {
    var unformatted = dust.helpers.tap(params.str, chunk, context),
        year = unformatted.substr(0, 4),
        month = unformatted.substr(5, 2),
        day = unformatted.substr(8, 2);

    return chunk.write(month + '/' + day + '/' + year);
};

dust.helpers.substr = function (chunk, ctx, bodies, params) {
    // Get the values of all the parameters. The tap function takes care of resolving any variable references
    // used in parameters (e.g. param="{name}"
    var str = dust.helpers.tap(params.str, chunk, ctx),
        begin = dust.helpers.tap(params.begin, chunk, ctx),
        end = dust.helpers.tap(params.end, chunk, ctx),
        len = dust.helpers.tap(params.len, chunk, ctx);
    begin = begin || 0; // Default begin to zero if omitted
    // Use JavaScript substr if len is supplied.
    // Helpers need to return some value using chunk. Here we write the substring into chunk.
    // If you have nothing to output, just return chunk.write("");
    if (len) {
        return chunk.write(str.substr(begin, len));
    }
    if (len) {
        return chunk.write(str.slice(begin, end));
    }
    if ((typeof(end) === 'undefined') && (typeof(len) === 'undefined')) {
        return chunk.write(str.substr(begin));
    }
    return chunk.write(str);
};

var currentLetter = '';

dust.helpers.customSort = function (chunk, ctx, bodies, params) {
   //The param passed in is actually an array with 1 element, the letter as a string
    var contentStr = '';
    var tempLetter = params.val[0];
    if (tempLetter !== currentLetter) {
        currentLetter = tempLetter;
        contentStr += '<a name="' + currentLetter.trim() + '" data-magellan-destination="' + currentLetter.trim() + '"> </a> ';
        contentStr += '<p class="spaced"> </p>';
        contentStr += '<div class="glossary-letter">' + currentLetter + '</div>';
        return chunk.write(contentStr);
    } else {
        return chunk.write('');
    }
};

//This function is duplicated WITH SOME CHANGES in the client helpers file at /Assets/Scripts/js/modules/SaltDustHelpers.js
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
    // the check below is only needed for client side rendering, running the check below may cause errors
    /*
    if (gradDate) {
        savedValue = Utility.convertFromJsonDate(gradDate).split('/')[2];
    }
    */
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
//find the tags for the content on the page
    //map the gloat tags to the goals
    // add the goals to the gloat image so that the image can be displayed.
dust.helpers.gloatImage = function (chunk, ctx, bodies, params) {
    var tags = this.tap(params.tagList, chunk, ctx),
        rsdTags = (process.env.RepayStudentDebt),
        fjTags = (process.env.FindAJob),
        psTags = (process.env.PayForSchool),
        mmTags = (process.env.MasterMoney);
        
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
//This function is duplicated in the client helpers file at /Assets/Scripts/js/modules/SaltDustHelpers.js
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
            if ((tagLinks.trim()).length <= 50 && ctx.templateName === 'oldTiles') {
                tagStr += ', ' + startTag + encodeURIComponent(tagArray[i].trim()) + '" >' + tagArray[i] + '</a>';
            } else if (ctx.templateName !== 'oldTiles') {
                tagStr += ', ' + startTag + encodeURIComponent(tagArray[i].trim()) + '" >' + tagArray[i] + '</a>';
            }
        }
    }
    return chunk.write(tagStr);
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

//Add banner helper for registration
dust.helpers.dynamicBanner = function (chunk, ctx, bodies, params) {
    var CurrentSchool = this.tap(params.CurrentSchool, chunk, ctx),
        CurrentSchoolBrand = this.tap(params.CurrentSchoolBrand, chunk, ctx),
        CMImage = this.tap(params.CMImage, chunk, ctx),
        out = '';
    //checks to see if there is the url parameter has type parameter, or no school specified or school has no logo.
    //if one of the condition met, use default image.
    if (!CurrentSchoolBrand || CurrentSchoolBrand === 'nologo') {
        out = CMImage;
    }
    //otherwise output the school-provided image
    else {
        try {
            //Try to read the filesystem for the image desired
            //When file not found show the default image
            out = '<img class="responsive-image"src="/assets/images/backgrounds/' + CurrentSchoolBrand + '-web.jpg" alt="' + CurrentSchool + '" title="' + CurrentSchool + '" />';
            fs.readFileSync('../assets/images/backgrounds/' + CurrentSchoolBrand + '-web.jpg');
        } catch (e) {
            if (e.code === 'ENOENT') {
                out = CMImage;
            } else {
                console.log('Error looking for school image:\n');
                console.log(e);
                throw e;
            }
        }
    }
    return chunk.write(out);
};

//Output the proper banner on the registration PAGE
//The /home page does not use this helper on the server side, so behavior specific to /home ...
//that is in SaltDustHelpers.js will be removed here.
dust.helpers.dynamicSchoolOutput = function (chunk, ctx, bodies, params) {
    var CurrentSchoolBrand = this.tap(params.CurrentSchoolBrand, chunk, ctx),
        out = '',
        noLogoCase = this.tap(params.noLogoCase, chunk, ctx),
        schoolCase = this.tap(params.schoolCase, chunk, ctx);

    //checks to see if there is an url parameter, no school specified or school has no logo
    //if none then output the standard ASA text style
    if (!CurrentSchoolBrand || CurrentSchoolBrand === 'nologo') {
        out = noLogoCase;
    }
    else {
        //otherwise output the school overlay style
        out = schoolCase;
    }
    return chunk.write(out);
};

//Live Chat dust helpers.
dust.helpers.liveChatAccount = function (chunk, ctx, bodies, params) {
    return chunk.write(process.env.LiveChatAccountID);
};
dust.helpers.liveChatWindow = function (chunk, ctx, bodies, params) {
    var adKey = this.tap(params.adKey, chunk, ctx);
    if (adKey) {
        return chunk.write(process.env.LiveChatWindowAuthID);
    } else {
        return chunk.write(process.env.LiveChatWindowUnAuthID);
    }
};
dust.helpers.liveChatDepartment = function (chunk, ctx, bodies, params) {
    return chunk.write(process.env.LiveChatDepartmentID);
};

//This helper is used both client side and in the nodejs layer, do not modify one without checking the other
dust.helpers.postedDateFormatter = function (chunk, ctx, bodies, params) {
    var dateObj = new Date(this.tap(params.date, chunk, ctx)),
        lng = this.tap(params.lng, chunk, ctx),
        monthNames = ['January', 'February', 'March', 'April', 'May', 'June', 'July', 'August', 'September', 'October', 'November', 'December'],
        nombres = ['enero', 'febrero', 'marzo', 'abril', 'mayo', 'junio', 'julio', 'agosto', 'septiembre', 'octubre', 'noviembre', 'diciembre'],
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
        //or 29 de febrero de 1996
    if  (lng === 'spanish') {
        return chunk.write(publishOrUpdated + dateObj.getDate() + ' ' + nombres[monthNames.indexOf(month)] + ' ' + dateObj.getFullYear());
    }
    return chunk.write(publishOrUpdated + month + ' ' + dateObj.getDate() + ', ' + dateObj.getFullYear());
};

//This helper is used both client side and in the nodejs layer, do not modify one without checking the other
dust.helpers.getCurrentYear = function (chunk, ctx, bodies, params) {
    return chunk.write(new Date().getFullYear());
};

function outputResponsiveImage(sourceName, mediumAvailable, altVal) {
    if (sourceName) {
        var lastIndexOfDot = sourceName.lastIndexOf('.'),
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

dust.helpers.responsiveImage = function (chunk, ctx, bodies, params) {
    // Replacing "src" attributes with "nosrc" to avoid making the http request until the desired HTML is built
    var imageTag = this.tap(params.imageSource, chunk, ctx).trim(),
        altVal = this.tap(params.altVal, chunk, ctx),
        mediumAvailable = this.tap(params.mediumAvailable, chunk, ctx),
        srcStartIndex = imageTag.indexOf('src="'),
        unclosedSrc = imageTag.substring(srcStartIndex + 5),
        srcCloseIndex = unclosedSrc.indexOf('"'),
        sourceName = unclosedSrc.substring(0, srcCloseIndex);

    return chunk.write(outputResponsiveImage(sourceName, mediumAvailable, altVal));
};
dust.helpers.sizeImage = function (chunk, ctx, bodies, params) {
    // Replacing "src" attributes with "nosrc" to avoid making the http request until the desired HTML is built
    var imageTag = this.tap(params.imageSource, chunk, ctx).trim(),
        sizeWanted = this.tap(params.size, chunk, ctx),
        srcStartIndex = imageTag.indexOf('src="'),
        unclosedSrc = imageTag.substring(srcStartIndex + 5),
        srcCloseIndex = unclosedSrc.indexOf('"'),
        sourceName = unclosedSrc.substring(0, srcCloseIndex),
        size = sizeWanted === 'small' ? '-small' : '';
    var lastIndexOfSlash = sourceName.lastIndexOf('/'),
        lastIndexOfDot = sourceName.lastIndexOf('.'),
        filePath = sourceName.substring(0, lastIndexOfDot),
        altStartIndex = imageTag.indexOf('alt="'),
        unclosedAlt = imageTag.substring(altStartIndex + 5),
        altCloseIndex = unclosedAlt.indexOf('"'),
        altVal = unclosedAlt.substring(0, altCloseIndex),        
        ext = sourceName.substring(lastIndexOfDot);

    if (altVal && altStartIndex > -1) {
        altVal = altVal.replace('<h1>', '').replace('</h1>', '').trim();
        sourceName = '<img src="' + filePath + size + ext + '" alt="' + altVal + '" title="' + altVal + '">';
    } else {
        sourceName = '<img src="' + filePath + size + ext + '">';
    }

    return chunk.write(sourceName);
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

dust.helpers.returnOrgLogos = function (chunk, ctx, bodies, params) {


    var member = ctx.get("SiteMember");

    var logoOrgs = [];

    if (member !== undefined) {
        var orgs = ctx.get("SiteMember").Organizations;

        if (orgs && (typeof orgs === 'object')) {

            logoOrgs = orgs.filter(function (el) {
                return (el.Brand !== 'nologo');
            });

        }
    } else {
        config = ctx.get("configuration");
        if (config.CurrentSchoolBrand !== "nologo") {
            var org = {
                OrgnizationName: config.CurrentSchool,
                Brand: config.CurrentSchoolBrand
            };
            logoOrgs = [org];
        }

    }

    var length = logoOrgs.length;

    logoOrgs.forEach(function (element, index) {
        element.orgIndex = index;
        element.orgLength = length;
        chunk.render(bodies.block, ctx.push(element));
    });
    return chunk;
};
