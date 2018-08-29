define([
    'jquery',
    'backbone',
    'modules/sharedModels',
    'salt',
    'modules/FinancialStatus/LoanTypeMap',
    'asa/ASAUtilities',
    'foundation5',
    'jquery.serializeObject'
], function ($, Backbone, Models, SALT, typeMap, Utility) {
    var loanAddedFirstTime = true,
        AddLoanView = Backbone.View.extend({
        initialize: function () {
            this.$el.foundation();
            /* Listen for application event for show:addLoan and trigger this views show method */
            SALT.on('show:loanForm', this.showLoanForm, this);
            SALT.on('SiteIntercept:run', this.loadSiteIntercept);
        },
        events: {
            'valid #loan-forms': 'addManualLoan',
            'change #form-name': 'captureFormName'
        },
        addManualLoan: function (e) {
            var serializedForm = this.serializeData(e);

            //Set Record Source to Member-KWYO (3)
            serializedForm.RecordSourceId = Utility.SOURCE_SELF_REPORTED_KWYO;
            /*
                Pass wait: true so that the model doesnt get added to the collection until
                the server responds with success.  Pass parse false, as the server will return a single
                model for an insert, rather than a list of models like a GET would
            */
            this.collection.create(serializedForm, { wait: true, parse: false, success: this.trackCompleted});

            this.closeForm();
        },
        trackCompleted: function (model, serverResp) {
            var code = typeMap[model.get('LoanTypeId')].trackingString;
            SALT.publish('KWYO:addloans:complete', { debtType: code });
            SALT.trigger('user:loans:changed');
            if (model.get('LoanType').toLowerCase() === 'federal') {
                SALT.trigger('SiteIntercept:run');
            }
            if (loanAddedFirstTime) {
                SALT.trigger('content:todo:inProgress', {contentId: '101-13584'});
                loanAddedFirstTime = false;
            }
        },
        serializeData: function (evt) {
            var formName = evt.target.id;
            var formData = $('#' + formName).serializeObject();
            formData.LoanTypeId = $('#js-dynamicLoanTypeId').val();
            formData.LoanType = typeMap[formData.LoanTypeId].loanType;
            formData.TypeName = typeMap[formData.LoanTypeId].typeName;
            return formData;
        },
        showLoanForm: function () {
            //show the proper form
            this.$el.foundation('reveal', 'open');
        },
        closeForm: function () {
            /*  Reset form fields,
                Set dropdown back to default value
                Close overlay
            */
            this.$el.foundation('reveal', 'close');

            /* Move the forms found into a variable so we dont run find twice */
            this.$el.find('form').trigger('reset');
            this.$el.find('form').hide();

            /* TODO: Improve this pattern, dont use these selectors directly, cache them */
            $('#addLoanForm').show();
        },
        captureFormName: function (e) {
            var formName = e.currentTarget.value;
            var loanType = e.currentTarget[e.currentTarget.selectedIndex].getAttribute('data-loantype');
            this.showSpecificForm(formName, loanType);
        },
        showSpecificForm: function (formName, loanTypeId) {
            /* Hide any current showing forms */
            $('#loan-forms').children().hide();
            /*  show the form selected from the dropdown */
            $('#' + formName).show();

            $(document).updatePolyfill();
            // add attribute required to shimmed date input fields (for non-Chrome browsers)
            $('.ws-date').attr('required', '');

            $('#js-dynamicLoanTypeId').val(loanTypeId);
        },
        loadSiteIntercept: function () {
            Utility.waitForAsyncScript('QSI', function () {
                QSI.RunIntercepts();
            });
        }
    });

    return AddLoanView;
});
