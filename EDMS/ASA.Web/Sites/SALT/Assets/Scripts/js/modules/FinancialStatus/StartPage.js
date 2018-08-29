define([
    'salt',
    'backbone',
    'underscore',
    'ASA/ASAUtilities',
    'salt/models/SiteMember',
    'foundation5'
], function (SALT, Backbone, _, utility, SiteMember) {
    var StartPage = Backbone.View.extend({
        events: {
            'click .js-addMore': 'showAddLoanForm',
            'click .js-close': 'close',
            'click .js-start-tour': 'startTour',
            'click .js-loan-upload': 'uploadLoan'
        },
        initialize: function () {
            this.$el.foundation();
            if (this.collection.length) {
                var thirtyDaysAgo = new Date(),
                    importedDate;
                thirtyDaysAgo.setDate(thirtyDaysAgo.getDate() - 30);
                //Look for loans imported more than 30 days ago
                var oldImported = this.collection.find(function (loan) {
                    var lastModified = new Date(utility.convertFromJsonDate(loan.get('LastModified'))),
                        recordSource = loan.get('RecordSourceId');
                    //Check for imported and more than 30 days ago
                    if ((recordSource === utility.SOURCE_IMPORTED_REP_NAV || recordSource === utility.SOURCE_IMPORTED_KWYO) && (thirtyDaysAgo > lastModified)) {
                        importedDate = lastModified;
                        return true;
                    }
                    return;
                });
                if (oldImported) {
                    //Use the imported loan date to calculate how many days ago it was, add this number to the overlay
                    var daysAgo = ((new Date() - importedDate) / (1000 * 60 * 60 * 24)).toFixed(0);
                    $('#js-days-ago').text(daysAgo);
                    $('.js-default-text').empty();
                    $('#thirty-day-old').show();
                    SiteMember.done(function (member) {
                        $('.js-user-firstname').text(member.FirstName);
                    });
                } else {
                    $('#thirty-day-old').empty();
                }
            } else {
                $('#thirty-day-old').empty();
            }
            this.$el.foundation('reveal', 'open');
        },
        close: function () {
            SALT.publish('KWYO:seeloans');
            this.$el.foundation('reveal', 'close');
        },
        showAddLoanForm: function () {
            SALT.publish('KWYO:addloans:start');
            SALT.trigger('show:loanForm');
        },
        startTour: function () {
            SALT.publish('KWYO:tour:start');
            $(document).foundation({
                joyride: {
                    modal: true,
                    expose: true,
                    scroll_speed: 300,
                    template : {
                        link    : '<a href=\"#close\" class=\"joyride-close-tip cancel-circle\"></a>',
                        timer   : '<div class=\"joyride-timer-indicator-wrap\"><span class=\"joyride-timer-indicator\"></span></div>',
                        tip     : '<div class=\"joyride-tip-guide salt-joyride \"><span class=\"joyride-nub\"></span></div>',
                        wrapper : '<div class=\"joyride-content-wrapper\"></div>',
                        button  : '<a href=\"#\" class=\"button base-btn main-btn joyride-next-tip\"></a>',
                        modal : '<div class="joyride-modal-bg"></div>'
                    }
                }
            });
            $(document).foundation('joyride', 'start');
            $('[data-index="1"]').click(_.bind(this.tourClose, this));
            $('.joyride-modal-bg').click(_.bind(this.stopJoyrideEvent, this));
        },
        tourClose: function () {
            this.$el.foundation('reveal', 'close');
            $('html, body').animate({ scrollTop: 0 }, 300);
        },
        stopJoyrideEvent: function () {
            return false;
        },
        uploadLoan: function () {
            SALT.publish('KWYO:importLoans');
        }
    });

    return StartPage;
});
