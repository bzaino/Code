require([
    'jquery',
    'salt',
    'underscore',
    'asa/ASAUtilities',
    'configuration',
    'salt/models/SiteMember',
    'foundation5',
    'jquery.cookie'
], function ($, SALT, _, Utility, configuration, SiteMember) {

    function callServiceWithBackup(searchQuery, headlineQuery, toIgnore) {
        makeServiceCall(searchQuery, function (jsonData) {
            // if we have the 4 records we need already, then the data is ready to be used to render related content
            if (Utility.checkNested(jsonData, 'mainContent[0].contents[0].records')) {
                if (jsonData.mainContent[0].contents[0].records.length > 3) {
                    jsonData.mainContent[0].contents[0].records = jsonData.mainContent[0].contents[0].records.slice(0, 4);
                    useRelatedData(jsonData);
                } else {
                    // if we still don't have 4 records, then do a headline search and add the results to the records we already have from the embedded link search
                    searchQuery = {ntk: 'ContentSearch', ntt: headlineQuery};
                    toIgnore = toIgnore ? toIgnore.join(',') : '';
                    useContentSearch(searchQuery, jsonData, toIgnore);
                }
            }
        });
    }

    function getRelatedData() {
        //Order of precendence is:
        // 1) Look for inline links, if 1-3 links are found, fill out with headline search
        // 2) If 0 inline links were found, fill out with glossary search
        // 3) If 0 inline links, and 0 glossary, fill out with headline search

        var $embeddedLinks = $('.js-glossary-scope').find('a[href*="/content/media"], a[href*="/calculators"], a[href*="/articles"]'),
            searchTerm = {ntk: 'sys_title'},
            $glossaryTerms = $('.glossaryTerm'),
            headlineQuery,
            recordsToIgnore = [];

        if ($('#english-content').length) {
            headlineQuery = $('#english-content .title-container h1').text().trim();
        } else {
            headlineQuery = $('.title-container h1').text().trim();
        }

        if ($embeddedLinks.length) {
            //Remove any duplicate lnks before checking if we have 4.
            $embeddedLinks = _.uniq($embeddedLinks, function (link) {
                return link.pathname;
            });
            //Endeca doesnt reliably return the records in the same order that they are searched, so we need to only search for the 4 we want to show
            if ($embeddedLinks.length > 4) {
                $embeddedLinks = $embeddedLinks.slice(0, 4);
            }
            // if we have embedded links to pieces of content, grab the link title of each record and search for it as related content
            searchTerm.ntt = _.reduce($embeddedLinks, function (memo, el) {
                // get the link title from the full URL, and wrap double quotes around it.
                var contentTitle;
                if (el.pathname.indexOf('/_/R-') > -1) {
                    var endecaKeyLocation = el.pathname.indexOf('/_/R-'),
                        primaryKey = el.pathname.substring(endecaKeyLocation + 5);
                    recordsToIgnore.push('NOT(P_Primary_Key:' + primaryKey + ')');
                    contentTitle = el.pathname.substring(0, endecaKeyLocation);
                    contentTitle = contentTitle.substring(contentTitle.lastIndexOf('/') + 1);
                } else if (el.pathname.indexOf('/calculators/') > -1) {
                    contentTitle = el.pathname.substr(el.pathname.indexOf('/calculators/') + 13);
                } else {
                    contentTitle = el.pathname.substr(el.pathname.indexOf('/articles/') + 10);
                }
                return memo + ' ' + '"' + contentTitle + '"';
            }, '');
            callServiceWithBackup(searchTerm, headlineQuery, recordsToIgnore);
        } else if ($glossaryTerms.length) {
            // if we have glossary terms and no embedded links, use glossary terms for search
            searchTerm.ntk = 'GlossaryContentSearch';
            searchTerm.ntt = _.reduce($glossaryTerms, function (memo, el) {
                return memo + ' ' + '"' + el.text + '"';
            }, '');
            callServiceWithBackup(searchTerm, headlineQuery);
        } else {
            // if there are no embedded links or glossary terms at all in the page, then do a headline search to get all records
            searchTerm = {ntk: 'ContentSearch', ntt: headlineQuery};
            useContentSearch(searchTerm);
        }
    }

    function useContentSearch(searchTerm, jsonData, recsToIgnore) {
        //Use article headline for search
        makeServiceCall(searchTerm, function (json) {
            if (jsonData) {
                jsonData.mainContent[0].contents[0].records = jsonData.mainContent[0].contents[0].records.concat(json.mainContent[0].contents[0].records).slice(0, 4);
                useRelatedData(jsonData);
            } else {
                // if we didn't pass jsonData object (this is the first and only service call), don't try to concat the two arays like do above
                json.mainContent[0].contents[0].records = json.mainContent[0].contents[0].records.slice(0, 4);
                useRelatedData(json);
            }
        }, recsToIgnore);
    }

    // a function that makes an async service call to get related content records, and runs a callback function
    function makeServiceCall(searchTerm, callback, recsToIgnore) {
        var pagePathComponents = location.pathname.split('/'),
            pageKey = '',
            apiUrl;

        recsToIgnore = recsToIgnore ? ',' + recsToIgnore : '';
        apiUrl = configuration.apiEndpointBases.SearchPage + '?No=0&Ns=&N=0&Ntk=' + searchTerm.ntk + '&Ntx=mode+matchany&Dx=mode+matchany&Ntt=' + searchTerm.ntt;
        apiUrl = Utility.handleUserConfiguration(apiUrl, $.cookie('UserSegment'));

        //Make sure the current record is not returned in the results
        if (pagePathComponents.length >= 1) {
            pageKey = _.last(pagePathComponents);
            pageKey = pageKey.replace('R-', '');
        }

        apiUrl = Utility.updateQueryString(apiUrl, 'HideRecord', 'NOT(P_Primary_Key:' + pageKey + '),NOT(ContentTypes:Definition)' + recsToIgnore);

        $.getJSON(apiUrl)
            .done(function (json) {
            if (Utility.checkNested(json, 'mainContent[0].contents[0].records') && (json.mainContent[0].contents[0].records.length || json.mainContent[0].contents[0].totalNumRecs === 0)) {
                callback(json);
            }
        });

    }

    function useRelatedData(data) {
        SiteMember.done(function (siteMember) {
            //Add SiteMember to the context so that member-aware UI features like Todos can make use of the member data in dust templates.
            data.SiteMember = siteMember;
            data.useTodoTileDesign = $.cookie('useTodoTileDesign');
            Utility.renderDustTemplate('tiles', data, function (err, out) {
                $('#RelatedContentSection').html(out);
                $(document).foundation();
            });
        });
    }

    SALT.on('glossary:fired', getRelatedData);
});
