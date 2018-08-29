define([
    'jquery',
    'backbone'
], function ($, Backbone) {
    var NextSteps = Backbone.View.extend({
        initialize: function () {
            this.collection.on('sync add remove', this.decideShow, this);
        },
        decideShow: function () {
            var nextStepsBox = $('.js-nextSteps');
            if (this.collection.length > 0) {
                nextStepsBox.removeClass('hide');
            } else {
                nextStepsBox.addClass('hide');
            }
        }
    });

    return NextSteps;
});
