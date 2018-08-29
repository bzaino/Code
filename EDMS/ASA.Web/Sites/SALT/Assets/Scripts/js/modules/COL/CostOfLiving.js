define([
    'jquery',
    'backbone',
    'underscore',
    'asa/ASAUtilities',
    'modules/COL/DefaultContent',
    'salt',
    'jquery.zglossary'
], function ($, Backbone, _, utility, defaultBreakdown, SALT) {

    var lastValidationResult,
        $slider = $('#js-salary-slider'),
        $output = $('#js-salary-output'),
        max = parseInt($slider.attr('max'), 10);

    var CostOfLiving =  Backbone.View.extend({
        el: '#js-col-container',
        events: {
            'click #js-restart': 'resetForm',
            'change .js-metro-area': 'handleFormChange',
            'change #current-state, #dest-state': 'stateDropdownChanged',
            'change #dest-city': 'destCityChanged',
            'change #current-city': 'currentCityChanged',
            'click #js-whatitmeans-btn': 'whatItMeans',
            'mouseup #js-salary-slider': 'sliderValueChanged',
            'keyup #js-salary-slider': 'sliderValueChanged',
            'touchend #js-salary-slider': 'sliderValueChanged',
            'input #js-salary-slider': 'updateSliderBubble',
            'change #js-salary-slider': 'updateSliderBubble',
            'click .js-question-mark': 'triggerTooltip',
            'click .js-toggle-trigger': 'mobileSectionToggle'
        },
        initialize: function () {
            this.renderDefaultBreakdown();
            $.getJSON('/api/SurveyService/COL')
                .done(function (json) {
                    var $stateDropdowns = $('.js-state');
                    $stateDropdowns.removeAttr('disabled');
                    utility.renderDustTemplate('COL/StateDropdownOptions', json, null, $stateDropdowns);
                });

            // IE bug, autocomplete="off" doesn't work as expected for range sliders
            $slider.val(0).trigger('change');

            this.initializeTooltips();
        },
        validateForm: function () {
            var sliderValue = parseInt($slider.val(), 10),
                // find all dropdowns that don't have a value
                invalidDropdowns = _.find($('.js-col-element'), function (element) {
                    return !element.value;
                });
            if (sliderValue <= 0 || invalidDropdowns) {
                return false;
            }
            return true;
        },
        sliderValueChanged : function (e) {
            // Firefox bug work around, FF doesn't fire change events when modified by the keyboard
            // https://bugzilla.mozilla.org/show_bug.cgi?id=858917
            if (e.type === 'keyup' && navigator.userAgent.search('Firefox')) {
                this.updateSliderBubble(e);
            }
            // if the event was keyup, only run the following code if the keypress was a left or right arrow press
            // to avoid infalted WT numbers, and unnecessary renders
            if (e.type !== 'keyup' || (e.type === 'keyup' && (e.which === 37 || e.which === 39))) {
                //Fire webtrends event if the new value is valid
                if ($slider.val() !== '0') {
                    SALT.publish('COL:UI:Event', {activityName: 'Compare Costs Before You Move 3 Whats Your Current Salary Slider'});
                }
                this.handleFormChange();
            }
        },
        handleFormChange: function () {
            var _this = this;
            if (this.validateForm()) {
                var sliderValue = $slider.val(),
                    currentCity = $('#current-city').val(),
                    destination = $('#dest-city').val();
                lastValidationResult = true;
                $.getJSON('/api/SurveyService/COL/' + currentCity + '/' + destination + '/' + sliderValue)
                    .done(function (json) {
                        json.percentClass = 'percent';
                        json.ComparableSalary = utility.currencyComma(Math.ceil(json.ComparableSalary));
                        _this.whatItMeansHandler(json.PercentageIncomeToMaintain);
                        if (json.PercentageIncomeToMaintain < 0) {
                            json.PercentageIncomeToMaintain = Math.abs(json.PercentageIncomeToMaintain);
                            json.IncomeMoreOrLess = 'less';
                            json.higherOrLower = 'lower';
                        } else {
                            json.IncomeMoreOrLess = 'more';
                            json.higherOrLower = 'higher';
                        }
                        json.city = $('#dest-city option:selected').text();
                        utility.renderDustTemplate('COL/Breakdown', json, function () {
                            $('#js-whatitmeans-btn, #js-see-breakdown').removeAttr('disabled');
                            SALT.publish('COL:UI:Event', {activityName: 'Compare Costs Before You Move 0 Overview Display'});
                        }, $('#js-breakdown'));
                    });
            } else {
                // invalid form
                // render breakdown template with the default values if last validation was true to avoid re-rendering with the same values.
                if (lastValidationResult) {
                    lastValidationResult = false;
                    _this.renderDefaultBreakdown();
                }
            }
        },
        renderDefaultBreakdown: function () {
            utility.renderDustTemplate('COL/Breakdown', defaultBreakdown, function () {
                $('#js-whatitmeans-btn, #js-see-breakdown').attr('disabled', 'disabled');
            }, $('#js-breakdown'));
        },
        populateMetroAreaDropdown: function (stateCode, $dropdownContainer) {
            var _this = this;
            //Disable the dropdown until we have a response from the server
            $dropdownContainer.attr('disabled', 'disabled');
            $.getJSON('/api/SurveyService/COL/' + stateCode)
                .done(function (json) {
                    utility.renderDustTemplate('COL/MetroAreaDropdownOptions', json, function () {
                        $dropdownContainer.removeAttr('disabled');
                        _this.handleFormChange.apply(_this);
                    }, $dropdownContainer);
                });
        },
        resetForm: function () {
            //Reset dropdown values to default and slider to 0
            $('.js-col-element').val('').trigger('change');
            $slider.val(0).trigger('change');
            //Remove active class, so that breakdown will slide off screen in mobile
            $('#js-breakdown').removeClass('active');
        },
        stateDropdownChanged: function (e) {
            var currentValue = $(e.currentTarget).val(),
                $metroDropdown = $(e.currentTarget).closest('.row').find('.js-metro-area');

            $metroDropdown.prop('selectedIndex', 0);
            if (currentValue) {
                this.populateMetroAreaDropdown(currentValue, $metroDropdown);
            } else {
                utility.renderDustTemplate('COL/MetroAreaDropdownOptions', {}, null, $metroDropdown);
            }
        },
        whatItMeansHandler: function (incomePercentage) {
            // show the proper overlay copy based on the 3 states (more, less, about the same)
            $('.js-what-it-means-blocks').hide();
            if (incomePercentage > 1) {
                $('#js-more-block').show();
            } else if (incomePercentage < -1) {
                $('#js-less-block').show();
            } else {
                $('#js-same-block').show();
            }
        },
        destCityChanged: function (e) {
            $('.js-whatitmeans-dest-city').text($(e.currentTarget.options[e.currentTarget.selectedIndex]).text());
            if (e.currentTarget.selectedIndex !== 0) {
                SALT.publish('COL:UI:Event', {activityName: 'Compare Costs Before You Move 2 Where Are You Relocating Dropdown'});
            }
        },
        currentCityChanged: function (e) {
            if (e.currentTarget.selectedIndex !== 0) {
                SALT.publish('COL:UI:Event', {activityName: 'Compare Costs Before You Move 1 Where Do You Live Dropdown'});
            }
        },
        whatItMeans: function () {
            SALT.publish('COL:UI:Event', {activityName: 'Compare Costs Before You Move 4 What It Means Button'});
        },
        triggerTooltip: function (e) {
            $(e.currentTarget).closest('.js-question-block').find('.glossaryTerm').trigger('show-tip', e);
            e.stopPropagation();
        },
        initializeTooltips: function () {
            var tips = ['Select the state and then the city closest to where you currently live.', 'Select the state and then the city closest to where you\'re thinking about relocating.', 'Slide the circle to the annual salary you earn at your current job.'],
                terms = [];
            _.each($('.js-col-step-labels'), function (element, index) {
                terms.push({
                    term: element.textContent,
                    definition: tips[index]
                });
            });

            $('.use-as-glossary').glossary(terms, {showForImage: true, hideTerm: true});
        },
        mobileSectionToggle: function () {
            $('.js-mobile-section').animate({width: 'toggle'}, 420);
        },
        updateSliderBubble: function (e) {
            var sliderVal = parseInt($slider.val(), 10),
                changePercentage = sliderVal / max,
                width = $slider.width(),
                // The offset for the nub needs to be positive when near the beginning of the slider, and negative towards the end of the slider, in order to keep the nub in the center of the bubble
                offset = sliderVal === 0 ? 5 : -48 * changePercentage;
            //Set the position of the nub, and set the text of the tip, using the currencyComma utility function, to properly add commas. eg. $60,000
            $output.css({
                left: ((changePercentage * width) + offset) + 'px'
            }).text(utility.currencyComma(parseInt(e.currentTarget.value, 10)));
        }
    });

    return CostOfLiving;
});
