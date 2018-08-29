define([
  "lesson2/app",

// Libs
  "backbone"
],

function (app, Backbone) {
    var Views = {};

    Views.Header = Backbone.View.extend({
        template: "nav/header",
        className: 'wrapper',

        serialize: function () {
            return {
                currentStep: this.model.get("currentStep"),
                stepsLength: this.model.get("stepsLength"),
                stepName: this.model.get("stepName")
            };
        },

        events: {
            'click #header-dropdown ul li a': 'handleNavChange',
            'click .save-and-exit': 'handleSaveAndExit',
            'click .loginButton': 'handleLogin'
        },

        handleLogin: function (event) {
            var redirectAfterLogin = $(event.target).data('redirect');

            if (redirectAfterLogin) {
                redirectAfterLogin = encodeURIComponent(redirectAfterLogin.replace(/\//g, "!"));
                SALT.LoginManager.returnURLOverride = '/lessons/lessonsLogin.html?ReturnUrl=' + redirectAfterLogin;
            }

            SALT.global.overlays.open({
                name: 'LogOn',
                anchor: this
            });
        },

        handleSaveAndExit: function (e) {
            // app.user.get('IndividualId')
            // See if user is logged in vs. anonymous
            if (Backbone.Asa.readCookie('IndividualId') === null) {
                e.preventDefault();
                app.router.showLoginPrompt($(e.target).attr("href"));
            } else if (Backbone.Asa.User.individualId === null || typeof Backbone.Asa.User.individualId === "undefined") {
                e.preventDefault();
                app.router.showLoginPrompt($(e.target).attr("href"));
            }
            
            app.wt.trigger("lesson:overall:saveAndExit");

            if (this.model.get('currentStep') === 5) {
                SALT.trigger('content:todo:completed');
            }
            if (this.model.get('currentStep') === 1) {
                $('.save-and-exit').trigger('step1SaveData');
            }
        },

        handleNavChange: function (event) {
            app.router.globalNavigation($(event.target).parent().attr('data-iteration'));

            return false;
        },

        beforeRender: function () {

        },

        initialize: function (options) {
            this.options = options;

            // Clobbering styledCheckBoxes function because it interferes with Lessons and isn't needed
            SALT.global.utils.styledCheckBoxes = $.noop;
        },

        afterRender: function () {
            this.setUpNavigation();
            this.updateProgress();
            this.loadModal();
            SALT.global.utils.alerts.overlays();
        },

        loadModal: function () {
            if (app.router.activeStep === 1 && navOptions[0].introModalChecked === false) {
                if (app.user.get('Lesson2Step') > 0) {
                    $.fancybox.open({
                        href: '#preload-data'
                    }, {
                        autoSize: false,
                        modal: true,
                        autoWidth: true,
                        autoHeight: true,
                        closeClick: false,
                        openEffect: 'none',
                        closeEffect: 'none',
                        padding: 0,

                        afterLoad: function () {
                            $('a.yes').on('click', function (event) {
                                $.fancybox.close(true);

                                $("#current-balance").trigger("keyup");

                                app.wt.trigger('lesson:overall:loadData', {
                                    user: app.user.get('UserId'),
                                    time: new Date(),
                                    step: {
                                        number: app.router.activeStep,
                                        preloaded: true
                                    }
                                }, false);

                                // TODO: add analytics logger callback/promise pattern to guarantee
                                //       we don't continue until all logging is complete
                                setTimeout(function () {
                                    app.wt._preloadCheck.resolve();
                                }, 500);

                                return false;
                            });

                            $('a.no').on('click', function (event) {
                                $.fancybox.close(true);

                                app.wt.trigger('lesson:overall:clearExistingData', {
                                    user: app.user.get('UserId'),
                                    time: new Date()
                                }, false);

                                // TODO: add analytics logger callback/promise pattern to guarantee
                                //       we don't continue until all logging is complete
                                setTimeout(function () {
                                    app.wt._preloadCheck.reject();

                                    localStorage.clear();
                                    $("#current-balance").val("");
                                    $("#interest-rate").val("");
                                    app.router.models.step1.set('Balance', 0);
                                    app.router.models.step1.set('InterestRate', 0);
                                    // trigger events, to set values through out and redraw graph and footer
                                    $('#current-balance').trigger('keyup');
                                    $('#interest-rate').trigger('keyup');

                                    app.user.set('Lesson2Step', 0);

                                    $.ajax({
                                        url: app.serverUrl + '/User/2',
                                        type: 'delete',
                                        async: false
                                    }).fail(function (jqXHR, textStatus) {
                                        alert("Request failed: " + textStatus);
                                    });

                                }, 500);

                                return false;
                            });
                        },

                        afterClose: function () {
                            navOptions[0].introModalChecked = true;
                        }
                    });
                } else {
                    app.wt.trigger('lesson:overall:freshStart', {
                        user: app.user.get('UserId'),
                        time: new Date(),
                        step: {
                            number: app.router.activeStep,
                            preloaded: false
                        }
                    }, false);

                    // TODO: add analytics logger callback/promise pattern to guarantee
                    //       we don't continue until all logging is complete
                    setTimeout(function () {
                        app.wt._preloadCheck.resolve();
                    }, 500);
                }
            }
        },
        updateProgress: function () {
            /**
            animate progress bar
            */
            var startingProgress = (app.router.currentStep / navOptions.length) * 100 + '%';
            var progress = (app.router.activeStep / navOptions.length) * 100 + '%';

            $('#header-progress .progress').css('width', startingProgress).animate({
                width: progress
            }, 650);
        },

        setUpNavigation: function () {
            //set length of steps
            $('#header-center .step2').text(navOptions.length);
            //re-draw all nav options
            $('#header-dropdown ul').empty();

            $.each(navOptions, function (key, value) {
                var visited = navOptions[key].visited;
                var completed = navOptions[key].completed;
                var newPage;

                if (completed) {
                    newPage = '<li class="ready completed"><a data-iteration="' + key + '" href="' + navOptions[key].page + '" href=""><span class="number">' + parseInt(key + 1, 10) + '</span><span>' + navOptions[key].name + '</span></a></li>';
                } else if (visited) {
                    newPage = '<li class="ready"><a data-iteration="' + key + '" href="' + navOptions[key].page + '" href=""><span class="number">' + parseInt(key + 1, 10) + '</span><span>' + navOptions[key].name + '</span></a></li>';
                } else {
                    newPage = '<li class=""><a data-iteration="' + key + '" href="' + navOptions[key].page + '" href=""><span class="number">' + parseInt(key + 1, 10) + '</span><span>' + navOptions[key].name + '</span></a></li>';
                }

                $('#header-dropdown ul').append(newPage);
            });

            // remove existing indicator
            // place it on the newly selected element
            $('#header-dropdown .indicator').remove();
            $('#header-dropdown a[data-iteration=' + parseInt(app.router.activeStep - 1, 10) + ']').parent().append('<span class="indicator">You’re here</span>');
        }
    });

    return Views;
});
