define([
    'underscore',
    'backbone',
    'asa/ASAUtilities'
], function (_, Backbone, Utility) {
    var Totals = Backbone.View.extend({
        initialize: function () {
            /* Listen to sync events, and re render the collection */
            this.collection.on('sync', this.handleSync, this);
        },
        handleSync: function () {
            /*  Render the view now that the collection data has been fetched
                Add a listener for all further add events to re-render the totals */
            this.render();
            this.collection.on('add remove', this.render, this);
        },
        calculate: function (key) {
            var totals = { total: 0, federalBalance: 0, privateBalance: 0 };
            _.each(this.collection.models, function (currentModel) {
                var currentValue = parseFloat(currentModel.get(key), 10);
                totals.total += currentValue;
                if (currentModel.get('LoanType') === 'federal') {
                    totals.federalBalance += currentValue;
                } else {
                    totals.privateBalance += currentValue;
                }
            });
            totals = {
                total: totals.total.toFixed(2),
                federalBalance: totals.federalBalance.toFixed(2),
                privateBalance: totals.privateBalance.toFixed(2)
            };
            return totals;
        },
        findNewestLoanDate: function () {
            var latestDate,
                dateObj,
                monthNames = ['January', 'February', 'March', 'April', 'May', 'June', 'July', 'August', 'September', 'October', 'November', 'December'];
            _.each(this.collection.models, function (model) {
                dateObj = Utility.convertSALtoJSdate(model.get('LastModified'));
                //If we have no dates yet, use the first we find, otherwise compare the current date we are iterating against the stored latest date
                if (!latestDate) {
                    latestDate = dateObj;
                } else if (dateObj > latestDate) {
                    latestDate = dateObj;
                }
            });
            //Now that we know the latest date format as expected. e.g. Jan 5, 2015
            return monthNames[latestDate.getMonth()] + ' ' + latestDate.getDate() + ', ' + latestDate.getFullYear();
        },
        render: function () {
            var ctx = this.collection.length && {
                Total: this.calculate('PrincipalBalanceOutstandingAmount'),
                Monthly: this.calculate('MonthlyPaymentAmount'),
                LastDate: this.findNewestLoanDate()
            };

            Utility.renderDustTemplate('KWYO/' + this.el.id, ctx, null, this.$el);
        }
    });

    return Totals;
});
