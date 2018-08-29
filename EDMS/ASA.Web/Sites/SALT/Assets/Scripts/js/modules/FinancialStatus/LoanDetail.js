define([
    'underscore',
    'backbone',
    'modules/sharedModels',
    'jquery',
    'asa/ASAUtilities',
    'salt',
    'foundation5',
    'jquery.serializeObject'
], function (_, Backbone, Models, $, Utility, SALT) {
    var LoanDetail = Backbone.View.extend({
        events: {
            'click .js-delete-loan': 'deleteLoan',
            'close': 'cleanUp'
        },
        initialize: function () {
            $('.js-loan-row').removeClass('highlight');
            $('#' + this.model.id).addClass('highlight');
            this.$el.foundation('reveal', 'open');
            this.$content = $('#loanDetailContent') || [];
            this.render();
            this.model.once('change', this.modelChanged);
            var $prevBtn = $('.js-prev'),
                $nextBtn = $('.js-next'),
                $pipe = $('.js-pipe');
            if (this.model.collection.hasPrev() && this.model.collection.hasNext()) {
                $pipe.show();
            } else {
                $pipe.hide();
            }
            if (!this.model.collection.hasPrev()) {
                $prevBtn.hide();
            } else {
                $prevBtn.show();
            }
            if (!this.model.collection.hasNext()) {
                $nextBtn.hide();
            } else {
                $nextBtn.show();
            }
        },
        chooseTemplate: function () {
            if (this.model.get('LoanTypeId').toUpperCase() === 'CC') {
                return 'loan_CC';
            }

            // Loan is Federal, determine whether to show manual or imported template
            if (this.model.get('RecordSourceId') === Utility.SOURCE_IMPORTED_REP_NAV || this.model.get('RecordSourceId') === Utility.SOURCE_IMPORTED_KWYO) {
                return 'loan_FederalImported';
            }
            if (this.model.get('LoanTypeId').toUpperCase() === 'FD') {
                return 'loan_FederalManual';
            }

            return 'loan_allTypes';
        },
        render: function () {
            var context = this.model.toJSON(),
                _this = this;
            context.OriginalLoanDate = Utility.convertDate(context.OriginalLoanDate);
            context.ContentEditorText = $('#js-text-' + this.idChooser()).html();
            Utility.renderDustTemplate('KWYO/' + this.chooseTemplate(), context, function () {
                _this.$content.foundation();
                $(document).updatePolyfill();
                // add attribute required to shimmed date input fields (for non-Chrome browsers)
                $('.ws-date').attr('required', '');
                // add an event to the events object (after appending the form html, otherwise it won't bind correctly).
                _this.delegateEvents(_.extend(_this.events, {
                    'valid #loanDetailContent': 'updateLoan',
                    'click .js-launchLoanImport': 'launchImport',
                    'click .js-next': 'paginateNext',
                    'click .js-prev': 'paginatePrev',
                    'click .js-save-loan-button': 'handleValidation'
                }));
            }, this.$content);
        },
        idChooser: function () {
            //For all imported loans we should return 'IMP', otherwise return the two digit LoanTypeId
            var id = this.model.get('LoanTypeId');
            if (this.model.get('RecordSourceId') === Utility.SOURCE_IMPORTED_REP_NAV || this.model.get('RecordSourceId') === Utility.SOURCE_IMPORTED_KWYO) {
                id = 'IMP';
            }
            return id;
        },
        launchImport: function () {
            this.cleanUp();
            //Look for a better way to do this (rather than hardcoding the js-uploadInfo selector)
            $(document).foundation('reflow');
            $('#js-uploadInfo').foundation('reveal', 'open');
        },
        close: function () {
            $('#' + this.model.id).removeClass('highlight');
            this.$el.foundation('reveal', 'close');
            this.cleanUp();
        },
        cleanUp: function () {
            /* Remove the view, unbind all handlers */
            this.undelegateEvents();
        },
        deleteLoan: function () {
            this.close();
            this.model.destroy({wait: true, success: function (model, response) {
                SALT.trigger('user:loans:changed');
            }});
        },
        handleValidation: function (e) {
            e.preventDefault();
            var elementsToValidate  = $(e.currentTarget).closest('form').find('input, textarea, select').get();
            // Manually trigger abide validation, and pass ajax as true to trigger valid/invalid events
            Foundation.libs.abide.validate(elementsToValidate, { type: 'mockEvent'}, true);
        },
        updateLoan: function (e) {
            var formData = this.serializeData(e);
            this.model.save(formData, {wait: true});
            this.close();
        },
        serializeData: function (evt) {
            var formName = evt.target.id;
            var formData = $('#' + formName).serializeObject();
            return formData;
        },
        modelChanged: function (model) {
            SALT.trigger('loan:updated', model);
        },
        paginateNext: function () {
            $('.js-prev').show();
            $('#' + this.model.id).removeClass('highlight');
            this.undelegateEvents();
            SALT.trigger('loan:selected', this.model.collection.next().id);
        },
        paginatePrev: function () {
            $('.js-next').show();
            $('#' + this.model.id).removeClass('highlight');
            this.undelegateEvents();
            SALT.trigger('loan:selected', this.model.collection.prev().id);
        }
    });

    return LoanDetail;
});
