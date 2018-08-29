require([
    'jquery',
    'configuration',
    'underscore',
    'asa/ASAUtilities',
    'salt/models/SiteMember',
    'modules/ReportingStatus',
    'salt',
    'jquery.autocomplete'
], function ($, configuration, _, Utility, SiteMember, reportingStatus, SALT) {

    SiteMember.done(function (siteMember) {
        //Only initiate the logic when the search is enabled
        if (!siteMember.SearchDisabled) {
            var url = Utility.setUrlToHideRecords(configuration.apiEndpointBases.GenericEndeca + 'predictiveSearch', siteMember, false),
                showReportIdQuestion = reportingStatus.checkReportIdStatus(siteMember);

            SALT.on('search:opened:firsttime', function () {
                $.getJSON(url, function (response) {
                    var suggestions = [];

                    _.each(response.secondaryContent[0].records, function (record) {
                        if (Utility.checkNested(record, 'attributes.term[0]')) {
                            var term = record.attributes.term[0].trim();
                            if (Utility.checkNested(record, 'attributes.acronym[0]') && record.attributes.acronym[0].trim()) {
                                suggestions.push({value: term + ' ' + record.attributes.acronym[0].trim(), data: term, type: ''});
                            } else {
                                suggestions.push({value: term, type: ''});
                            }
                        }
                    });

                    _.each(response.mainContent[0].records, function (record) {
                        if (Utility.checkNested(record, 'attributes.headline[0]')) {

                            var contentType,
                                detailRecordState = '';
                            if (Utility.checkNested(record, 'attributes.ContentTypes[0]')) {
                                contentType = record.attributes.ContentTypes[0].trim().toLowerCase() + 's';
                                if (contentType === 'tools' || contentType === 'apps') {
                                    if (record.attributes['iframe-url'][0].indexOf('calcxml') > -1) {
                                        record.detailsAction.recordState = '/calculators/' + record.attributes.sys_title[0].trim();
                                    }
                                    contentType = 'tools';
                                }
                                if (contentType === 'courses') {
                                    record.detailsAction.recordState = configuration.mm101.url + record.attributes['external-url'][0];
                                }
                            }

                            if (Utility.checkNested(record, 'detailsAction.recordState')) {
                                detailRecordState = record.detailsAction.recordState;
                            }

                            //Need to wrap headline in a div in case there are values without html elements in the text
                            var headline = '<div>' + record.attributes.headline[0].trim() + '</div>';
                            var headlineNoHtml = $(headline).text();

                            suggestions.push({value: headlineNoHtml, type: contentType, recState: detailRecordState});

                            if (Utility.checkNested(record, 'attributes.FlatTags[0]')) {
                                var tagArray = record.attributes.FlatTags[0].trim();
                                var trimmedTag, tagCase;
                                if (tagArray) {
                                    tagArray.split('|').forEach(function (tag) {
                                        suggestions.push({value: Utility.toTitleCase(tag.trim()), type: ''});
                                    });
                                }
                            }
                        }
                    });

                    var uniqueList = _.uniq(suggestions, JSON.stringify);
                    var sortedList = _.sortBy(uniqueList, 'type');

                    $(function () {
                        $('#searchCriteria, #searchCriteria-mobile').autocomplete({
                            minChars: 3,
                            lookupLimit: 10,
                            lookup: sortedList,
                            triggerSelectOnValidInput: false,
                            maxHeight: 530,
                            containerClass: 'autocomplete-suggestions',
                            onSelect: function (suggestion) {
                                // if suggestion has a record state, go directly to that piece of content
                                if (suggestion.recState) {
                                    if (suggestion.recState.indexOf('/calculators/') > -1) {
                                        $(location).attr('href', suggestion.recState);
                                    } else if (suggestion.type === 'courses') {
                                        if (showReportIdQuestion) {
                                            $(location).attr('href', '#IDFrm');
                                            $(location).attr('target', '');
                                        }
                                    } else if (suggestion.recState.indexOf('/Lesson/my-money101-landing') > -1) {
                                        $(location).attr('href', '/MM101/');
                                    } else if (suggestion.recState.indexOf('/Tool/saveup-tool') > -1) {
                                        $(location).attr('href', '/landing/Saveup');
                                    } else {
                                        $(location).attr('href', '/content/media' + suggestion.recState);
                                    }
                                } else {
                                    // data attribute is only available for terms with acronyms. use it as the search value to return the appropriate definition in the search page.
                                    this.value = suggestion.data || this.value;
                                    $(this).closest('form').submit();
                                }
                            }
                        });
                    });
                });
            });
        }
    });
});
