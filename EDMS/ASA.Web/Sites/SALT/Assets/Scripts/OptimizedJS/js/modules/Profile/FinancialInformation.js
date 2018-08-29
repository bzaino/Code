define([
    'salt',
    'backbone',
    'asa/ASAUtilities'
], function (SALT, Backbone, Utility) {

    var FinancialInformation = Backbone.View.extend({
        events: {
            'valid #financial-information-form': 'saveChanges',
            'change #financial-information-form': 'enableSubmit',
            'input #financial-information-form': 'enableSubmit'
        },
        initialize: function (options) {
            this.questions = options.Questions;
            this.render();
        },
        render: function () {
            var _this = this;
            var context = this.model.toJSON();
            context.QuestionsAnswers = this.questions;
            Utility.renderDustTemplate('Profile/FinancialInformation', context, function () {
                _this.$el.foundation().updatePolyfill();
            }, this.el);
        },
        saveChanges: function (e) {
            this.$el.find('.js-profile-saved-overlay').show();
            this.$el.find('.js-profile-saved-img').show();
            var formData = $(e.currentTarget).serializeObject();
            SALT.trigger('Profile:Updated', formData, this.$el);
        },
        enableSubmit: function (e) {
            SALT.trigger('Form:Changed', e);
        }
    });

    return FinancialInformation;
});
