var intro = intro || {};
var errorMessage = 'Oops, be sure to fill in all the fields';
var goalValue = 0;
var goalTime = 1;

//pass back to global
save = intro;

intro.global = {
  step: 1,
  utils: {
    init: function () {
      $(".left-button").addClass("invisible");

      $("#main").delay(375).fadeIn('slow', function () {
        curriculum.global.viewport.animateViewport.normal();
        intro.global.utils.updateTotal();
        curriculum.global.viewport.getNewViewport();
      });

      // activate global plugins
      intro.global.utils.plugins.init();
      if (curriculum.global.utils.showLoadDataPrompt()) {

        // Load Colorbox
        $.colorbox({
          href: "includes/confirm-overlay.html",
          initialWidth: 370,
          initialHeight: 193,
          overlayClose: false,
          onComplete: function () {
            $('a.yes').click(function (e) {
              // step data was preloaded; set tracking flag to true.
              curriculum.global.tracking.preloaded = true;

              curriculum.global.tracking.trigger("lesson:overall:loadData");

              e.preventDefault();
              intro.global.utils.preloadData.init();
              curriculum.global.utils.paginate.forwardNavLocked = false;
              curriculum.global.utils.paginate.updateNavigation();
              curriculum.global.utils.animateGraph.refresh();

              $.colorbox.close();
              $('#goal-value').trigger('focus');
              $(".left-button").addClass("invisible");
            });

            $('a.no').click(function (e) {
              $('#goal-value').val(0).trigger("blur");
              $('#goal-time').val(0).trigger("blur");

              // step data was not preloaded; set tracking flag to false.
              curriculum.global.tracking.preloaded = false;

              curriculum.global.tracking.trigger("lesson:overall:clearExistingData");
              e.preventDefault();

              intro.global.utils.flushUserData();
              curriculum.global.utils.paginate.forwardNavLocked = false;
              curriculum.global.utils.paginate.updateNavigation();

              $.colorbox.close();
              $(".left-button").addClass("invisible");
            });
          },
          onClosed: function() {
            $("body").removeClass("lessons-skin");
          }
        });
      } else {
        $.colorbox.close();

        // step data was not preloaded; set tracking flag to false.
        curriculum.global.tracking.preloaded = false;

        curriculum.global.tracking.trigger("lesson:overall:freshStart");

        intro.global.utils.preloadData.init();
        curriculum.global.utils.paginate.forwardNavLocked = false;
        $('#goal-value').trigger('focus');

        setTimeout(function() {
          $("body").removeClass("lessons-skin");
        }, 1000);

      } 

      // Append ID for this page
      $('#content-container .content').attr('id', 'curriculum-intro');

      // Append stylesheet with IE support
      if (document.createStyleSheet) {
        document.createStyleSheet('css/curriculum/intro.css');
      } else {
        $('head').append('<link rel="stylesheet" href="css/curriculum/intro.css" type="text/css" />');
      }

      //update page context
      curriculum.global.utils.paginate.updateContext("1", "Introduction", "monthly savings goal", "What’s coming in?", "Let’s Go");

      ////////goal name
      $('#goal-name.error').bind('keyup blur focus', function (event) {
        if (curriculum.global.utils.isEmpty($('#goal-name').val())) {
          $('#goal-name').addClass('error');
        } else {
          $('#goal-name').removeClass('error');
          $('#footer .error-msg').fadeOut();
        }
      });
      $('#goal-name').bind('blur keyup', function (event) {
        cleanGoalName = curriculum.global.utils.cleanInput.freeForm($('#goal-name').val());
        $('#goal-name').val(cleanGoalName);
      });

      $('#goal-value').bind('keyup focus blur', function (e) {
        //goal value
        if ($(this).hasClass('error')) {
          cleanGoal = curriculum.global.utils.cleanInput.init($('#goal-value').val());
          if (curriculum.global.utils.isEmpty(cleanGoal) || cleanGoal == 0) {
            $('#goal-value').addClass('error');
          } else {
            $('#goal-value').removeClass('error');
            $('#footer .error-msg').fadeOut();
          }
        }
        intro.global.utils.updateTotal();
        //highlight, only on keyup & blur
        if (e.type == 'keyup' || e.type == 'blur') {
          $('#total .value').stop(true, true).effect("highlight", { color: '#faff00' }, 1500);
        }
      });

      //process #goal-time
      $('#goal-time').bind('keyup focus blur', function (e) {
        $('#goal-time').removeAttr('placeholder');
        if ($(this).hasClass('error')) {
          if (!curriculum.global.utils.isEmpty($('#goal-time').val())) {
            $(this).removeClass('error');
            $('#footer .error-msg').fadeOut();
          }
        }
        $(this).toNumber();
        intro.global.utils.updateTotal();
      });

      $('#curriculum-intro .dropdown').dropkick({ theme: 'large' });
      $('#curriculum-intro .dropdown-smaller').dropkick();
    },
    updateTotal: function () {
      goalValue = curriculum.global.utils.cleanInput.init($('#goal-value').val());
      goalTime = $('#goal-time').val();

      if (goalTime < 1) {
        goalTime = 1;
      }

      goalCalculated = goalValue / goalTime;

      $('#goal-value').toNumber().formatCurrency({ roundToDecimalPlace: 0 });
      $('#total .value').text(goalCalculated).toNumber().formatCurrency({ roundToDecimalPlace: 0 });
    },
    plugins: {
      // init all plugins
      init: function () {

        //run the jquery UI auto fill here
      }
    },
    saveData: function () {
      userData.goal = $.extend(userData.goal, { name: $('#goal-name').val(), value: goalValue, newValue: null, time: goalTime, newTime: null });
      //backUp the userData object using jsStorage
      curriculum.global.utils.server.saveToServer(userData);
    }, // END saveData
    preloadData: {
      init: function () {
        //curriculum.global.utils.server.retrieveFromServer(null,userData);

        if (userData.goal) {
          $('#goal-name').val(userData.goal.name);
          $('#goal-value').val(userData.goal.value);
          $('#goal-time').val(userData.goal.time);
        }
      }
    }, // END preloadData
    flushUserData: function () {
      // Set empty userData
     userData = {
        userGuid: userData.userGuid,
        individualId: userData.individualId,
        goal:
          {
            id: userData.goal.id,
            userId: userData.goal.userId
          },
        User: {
          UserId: userData.User.UserId,
          IndividualId: userData.User.IndividualId,
          Lesson1Step: 0
        }
      };
     curriculum.global.utils.server.startOver();
    },
    errors: function () {
      $('#footer .error-msg').text(errorMessage).fadeOut();

      var $goalName = $('#goal-name'),
          goalValue = $.trim($goalName.val()),
          goalPlaceholder = $.trim($goalName.attr("placeholder"));

      //goal name
      if ( goalValue === goalPlaceholder || curriculum.global.utils.isEmpty(goalValue) ) {
        $('#goal-name').addClass('error');
      } else {
        $('#goal-name').removeClass('error');
      }

      //goal value
      cleanGoal = curriculum.global.utils.cleanInput.init($('#goal-value').val());
      if (curriculum.global.utils.isEmpty(cleanGoal) || cleanGoal == 0) {
        $('#goal-value').addClass('error');
      } else {
        $('#goal-value').removeClass('error');
      }

      //goal time
      if (curriculum.global.utils.isEmpty($('#goal-time').val())) {
        $('#goal-time').addClass('error');
      } else {
        $('#goal-time').removeClass('error');
      }

      //once all validation is done, check to see if pagePass is true
      if ($('.error').length == 0) {
        pagePass = true;
        $('#footer .error-msg').fadeOut();
        curriculum.global.utils.paginate.next();
      } else {
        $('#footer .error-msg').fadeIn();

      }
    }, // END errors
    //stub out the error and success so we can get model to validate
    //at save and exit time, but with no UI visual display
    saveAndExitStep1Validation: function () {
        var $goalName = $('#goal-name'),
            goalValue = $.trim($goalName.val()),
            goalPlaceholder = $.trim($goalName.attr('placeholder')),
            validValues = true;

        //goal name
        if (goalValue === goalPlaceholder || curriculum.global.utils.isEmpty(goalValue)) {
            validValues = false;
        }

        //goal value
        cleanGoal = curriculum.global.utils.cleanInput.init($('#goal-value').val());
        if (curriculum.global.utils.isEmpty(cleanGoal) || cleanGoal === 0) {
            validValues = false;
        }

        //goal time
        if (curriculum.global.utils.isEmpty($('#goal-time').val())) {
            validValues = false;
        }
        return validValues;

    } // END saveAndExitStep1Validation

  } // END: utils

}; // END intro global var


$(function() {
  intro.global.utils.init();  
});
