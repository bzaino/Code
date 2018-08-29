define([
    'underscore',
    'jquery',
    'backbone',
    'asa/ASAUtilities',
    'salt'
], function (_, $, Backbone, Utility, SALT) {

    var Grid = Backbone.View.extend({
        template: 'KWYO/LoanGrid',
        events: {
            'click .js-loanDetailLink': 'openDetail',
            'click .js-addLoans-btn': 'showAddLoanForm'
        },
        initialize: function () {
            this.$FederalAggregate = $('#FederalAggregate');
            this.$PrivateAggregate = $('#PrivateAggregate');
            /* Listen for the first sync event to render the grid the first time with any current loans*/
            this.collection.once('sync', this.render, this);
            SALT.on('loan:updated', this.updateSingle, this);
        },
        showAddLoanForm: function () {
            SALT.publish('KWYO:addloans:start');
            SALT.trigger('show:loanForm');
        },
        openDetail: function (e) {
            var id = $(e.target).attr('value');
            SALT.trigger('loan:selected', id);
            return id;
        },
        render: function () {
            this.collection.off('remove', this.removeSingle);
            this.collection.off('add', this.renderSingle);
            $('.js-loan-row').remove();
            /* Render each loan in the collection */
            this.collection.map(function (loan) {
                this.renderSingle(loan);
            }, this);

            this.collection.on('add', this.renderSingle, this);
            this.collection.on('remove', this.removeSingle, this);
            this.collection.once('reset', this.render, this);
            $(document).foundation('reflow', 'reveal');
        },
        renderSingle: function (model, collection, options) {
            var _this = this;
            // always show two decimal places for balance and monthly payment
            var renderedHTML;
            Utility.renderDustTemplate(this.template, model.toJSON(), function (err, out) {
                renderedHTML = out;
                if (model.get('LoanType') === 'federal') {
                    _this.$FederalAggregate.before(renderedHTML);
                } else {
                    _this.$PrivateAggregate.before(renderedHTML);
                }
            });
        },
        removeSingle: function (model, collection, options) {
            $('#' + model.id).remove();
        },
        updateSingle: function (model) {
            var context = this.serializeModel(model);
            var renderedHTML;
            Utility.renderDustTemplate(this.template, context, function (err, out) {
                renderedHTML = out;
                $('#' + model.id).html($($.parseHTML(renderedHTML)).html());
            });
        },
        serializeModel: function (model) {
            var context = model.toJSON();
            if (typeof model.get('PrincipalBalanceOutstandingAmount') === 'number' && typeof model.get('MonthlyPaymentAmount') === 'number') {
                context.PrincipalBalanceOutstandingAmount = context.PrincipalBalanceOutstandingAmount.toFixed(2);
                context.MonthlyPaymentAmount = context.MonthlyPaymentAmount.toFixed(2);
            }
            return context;
        }
    });

    return Grid;
});
