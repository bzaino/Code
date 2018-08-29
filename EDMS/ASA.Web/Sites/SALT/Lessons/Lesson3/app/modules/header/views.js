define([
  "lesson3/app",

  // Libs
  "backbone",
  'salt'
],

function(app, Backbone, SALT) {
  var Views = {};

  Views.Header = Backbone.View.extend({
    template: "nav/header",
    className: 'wrapper',

    serialize: function() {
      return {
        currentStep: app.router.activeStep,
        stepsLength: navOptions.length,
        stepName: navOptions[app.router.activeStep - 1].name
      };
    },

    events: {
      'click #header-dropdown ul li a': 'handleNavChange',
      'click .save-and-exit': 'handleSaveAndExit',
      'click .loginButton': 'handleLogin'
    },

    handleLogin: function(event) {
      var redirectAfterLogin = $(event.target).data('redirect');

      if (redirectAfterLogin) {
        redirectAfterLogin = encodeURIComponent(redirectAfterLogin.replace(/\//g, '!'));
        SALT.LoginManager.returnURLOverride = '/lessons/lessonsLogin.html?ReturnUrl=' + redirectAfterLogin;
      }

      SALT.global.overlays.open({
        name: 'LogOn',
        anchor: this
      });
    },

    handleSaveAndExit: function(e) {
        //loop through repayment models and save them
        if (typeof this.options.repaymentModels !== 'undefined'){
            $.each(this.options.repaymentModels, function(key, value){
            app.router.repaymentModels[value.div].save();
        });

        app.wt.trigger('lesson:overall:saveAndExit', {
                user: app.user.get('UserId'),
                time: new Date()
            });
        }

        // See if user is logged in vs. anonymous
        if (Backbone.Asa.readCookie('IndividualId') === null) {
            e.preventDefault();
            app.router.showLoginPrompt($(e.target).attr("href"));
        } else if (Backbone.Asa.User.individualId === null || typeof Backbone.Asa.User.individualId === 'undefined' ) {
            e.preventDefault();
            app.router.showLoginPrompt( $(e.target).attr('href') );
        }

        app.wt.trigger('lesson:overall:saveAndExit');
        if (this.model.get('currentStep') === 6) {
            SALT.trigger('content:todo:completed');
        }
        if (this.model.get('currentStep') === 1) {
            $('.save-and-exit').trigger('step1SaveData');
        }
    },

    handleNavChange: function(event){
      app.router.globalNavigation($(event.target).parent().attr('data-iteration'));

      return false;
    },

    beforeRender: function(){


    },

    initialize: function(options) {
      this.options = options;

      // Clobbering styledCheckBoxes function because it interferes with Lessons and isn't needed
      SALT.global.utils.styledCheckBoxes = $.noop;
    },

    afterRender: function(){
      this.setUpNavigation();
      this.updateProgress();
      this.loadModal();
      SALT.global.utils.alerts.overlays();
    },

    loadModal: function(){
      if (app.router.activeStep === "1") {
        // Flush the old data so it doesn't fill up, it used to only be in the "woah there" no button,
        // but would get stuck if the user logged out
        localStorage.clear();
      }
      if(app.router.activeStep == "1" && navOptions[0].introModalChecked === false && app.user.get('Lesson3Step') > 0){
        $.fancybox.open({
            href : '#preload-data'
          },{
              autoSize    : false,
              modal: true,
              autoWidth: true,
              autoHeight: true,
              closeClick    : false,
              openEffect    : 'none',
              closeEffect    : 'none',
              padding: 0,
              afterLoad: function(){
                  $('a.yes').on('click', function(event){
                    $.fancybox.close(true);
                    app.wt.trigger('lesson:overall:loadData', {
                      user:app.user.get('UserId'),
                      time: new Date(),
                      step: {
                        number: app.router.activeStep,
                        preloaded: true
                      }
                    }, true);
                    return false;
                  });
                  $('a.no').on('click', function(event){
                    $.fancybox.close(true);
                    app.wt.trigger('lesson:overall:clearExistingData', {
                      user:app.user.get('UserId'),
                      time: new Date(),
                      step: {
                        number: app.router.activeStep,
                        preloaded: false
                      }
                    }, true);
                    localStorage.clear();
                    //reset common values
                    $("#total-balance").val("");
                    SALT.trigger('totalBalance:changed', 0);
                    SALT.trigger('totalBalanceFooter:update');
                    SALT.trigger('income:changed', 0);
                    SALT.trigger('dependents:changed', 1);
                    SALT.trigger('interestRate:changed', 0.0386);
                    SALT.trigger('state:reset', 'other');
                    app.user.set('Lesson3Step', 0);

                    $.ajax({
                      url:app.serverUrl+'/User/3',
                      type: 'delete',
                      async:false
                    }).fail(function(jqXHR, textStatus) {
                      alert( "Request failed: " + textStatus );
                    });
                    return false;
                  });

                },
              afterClose: function(){
                  navOptions[0].introModalChecked = true;
                }
            });
      }
      else if (app.router.activeStep == "1" && !app.router.collections.loanType.models[0].attributes.sum && navOptions[0].introModalChecked === false) {
        $.fancybox.open({
            href : '#bookmark'
          },{
            autoSize    : false,
            modal: true,
            autoWidth: true,
            autoHeight: true,
            closeClick    : false,
            openEffect    : 'none',
            closeEffect    : 'none',
            padding: 0,
            afterLoad: function(){
                  $('a.ok').on('click', function(event){
                    $.fancybox.close(true);

                    app.wt.trigger('lesson:overall:freshStart', {
                      user: app.user.get('UserId'),
                      time: new Date(),
                      step: {
                        number:    app.router.activeStep,
                        preloaded: false
                      }
                    }, true);

                    // TODO: add analytics logger callback/promise pattern to guarantee
                    //       we don't continue until all logging is complete
                    setTimeout(function() {
                      app.wt._preloadCheck.resolve();
                    }, 500);

                    return false;
                  });
                },
            afterClose: function(){
                  navOptions[0].introModalChecked = true;
                }
          });
      }
      else {
      //not need but left for debugging purposes.
      //     app.wt.trigger('lesson:overall:freshStart', {
      //       user: app.user.get('UserId'),
      //       time: new Date()
      //     }, true);

        // TODO: add analytics logger callback/promise pattern to guarantee
        //       we don't continue until all logging is complete
        setTimeout(function() {
              app.wt._preloadCheck.resolve();
            }, 500);
      }
    },

    updateProgress: function(){
      /**
        animate progress bar
      */
      var startingProgress = (app.router.currentStep / navOptions.length) * 100 + '%';
      var progress = (app.router.activeStep  / navOptions.length) * 100 + '%';

      $('#header-progress .progress').css('width', startingProgress).animate({
        width: progress
      }, 650);
    },

    setUpNavigation: function(){
      //re-draw all nav options
      $('#header-dropdown ul').empty();
      $.each(navOptions, function(key, value){
        var visited = navOptions[key].visited;
        var completed = navOptions[key].completed;
        var newPage;

        if (completed) {
          newPage = '<li class="ready completed"><a data-iteration="' + key + '" href="' + navOptions[key].page + '" href=""><span class="number">' + parseInt(key + 1, 10) + '</span><span>' + navOptions[key].name + '</span></a></li>';
        } else if(visited) {
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
