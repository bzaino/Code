define([
  "lesson3/app",

  // Libs
  "backbone"
],

function(app, Backbone) {
  var Views = {};

  Views.Favorites = Backbone.View.extend({
    template: "graph-modifiers/favorites",
    className: 'wrapper',

    repaymentOptions: {},

    initialize: function(options) {
      this.repaymentOptions = options.repaymentOptions || {};

    },

    afterRender: function() {
      var selectRepaymentOptions = this.model.get('selectRepaymentOptions');

      //populate favorite dropdown
      for(i=0; i < selectRepaymentOptions.length; i++) {
        var name = selectRepaymentOptions[i].name;
        var div = selectRepaymentOptions[i].div;
        $('#select-repayment-options').find('select').append('<option value="' + div + '">' + name + '</option>');
      }

      var elView = this;
      $('#select-repayment-options').find('select').dropkick({theme: 'large', change: function(value, label) {
        //call another function
        app.wt.trigger('lesson:repaymentOptionSelect:' + value, {
          user: app.user.get('UserId'),
          time: new Date()
        });

        elView.selectRepaymentChange(value);
      }});

      //if there is only one item hide the arrow and remove dropwdown functionality
      if(selectRepaymentOptions.length <= 1) {
        $('.dk_theme_large').addClass('single');
      }

      //set favorite icon onload
      var onLoadValue = $('#select-repayment-options').find('.dk_option_current a').attr('data-dk-dropdown-value');
      this.getFavorite(onLoadValue);

      //show appropriate repayment option info
      $('#repayment-options .repayment-option').first().addClass('selected').fadeIn();
      var repaymentView = this.repaymentOptions[onLoadValue];
      if(typeof repaymentView !== 'undefined'){
        repaymentView.calculateAndDisplay();
      }
    },

    selectRepaymentChange: function(value){
      $('#repayment-options .selected').removeClass('selected').hide();
      $('#'+value).addClass('selected').show();

      this.getFavorite(value);
      this.repaymentOptions[value].trigger('calculateAndDisplay');
    },

    getFavorite: function(value){
      var on = false;
      this.collection.favorites.each(function(element,index, list){
        if (value === element.attributes.valueID) {
          on = true;
        }
      });

      if(on === true) {
        $('#favorites').addClass('on');
      } else {
        $('#favorites').removeClass('on');
      }
    },

    events : {
      'click #favorites': 'handleFavoriteChange'
    },

    handleFavoriteChange: function(event){
      var value = $('#select-repayment-options').find('.dk_option_current a').attr('data-dk-dropdown-value');

      if(!$('#favorites').hasClass('on')){
        //adding a favorited item
        $(event.target).addClass('on');

        this.collection.favorites.add(this.collection.favorites.newAttributes(value),{init: true});

        this.collection.favorites.each(function(element,index, list){
          element.save(null, {
            success: function (model, resp) {
              model.id = model.get('FavoriteId');
            }
          });
        });

        //analytics
        app.wt.trigger('lesson:favorite:'+value, {
          user: Backbone.Asa.User.userGuid,
          time: new Date()
        });
      } else {
        //remove a favorited item
        $(event.target).removeClass('on');

        var iterationID = null;

        this.collection.favorites.each(function(element,index, list){
          if (value === element.attributes.valueID) {
            iterationID = element.attributes.iteration;
          }
        });

        var singleFavoriteType = this.collection.favorites.where({iteration: iterationID});

        //destory local storage
        singleFavoriteType[0].destroy();
        this.collection.favorites.remove(singleFavoriteType[0]);
      }
    }
  });

  return Views;
});
