define([
    'backbone',
    'jquery',
    'salt',
    'foundation5'
], function (Backbone, $, SALT) {
    var ButtonMenu = Backbone.View.extend({
        events: {
            'click .js-addMore': 'showAddLoanForm',
            'click .js-flipGrid' : 'flipGridEvent',
            'click .js-glossary': 'glossaryClicked',
            'click .js-import': 'importClicked'
        },
        showAddLoanForm: function () {
            SALT.publish('KWYO:addloans:start');
            SALT.trigger('show:loanForm');
        },
        flipGridEvent: function () {
            SALT.publish('KWYO:switchView');
            var classList = $('.js-elementSwitch'),
            classList0 = classList[0],
            classList1 = classList[1];
            $(classList1).slideUp(700, function () {
                $(classList1).insertBefore(classList0).slideDown(700);
            });
        },
        glossaryClicked: function () {
            SALT.publish('KWYO:glossary');
        },
        importClicked: function () {
            SALT.publish('KWYO:import:nav');
        }
    });

    return ButtonMenu;
});
