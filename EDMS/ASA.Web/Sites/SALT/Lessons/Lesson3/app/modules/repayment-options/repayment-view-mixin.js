define([
  "lesson3/app"
],

  function(app){
  //This object
  RepaymentViewMixin = {
    /* All events need to be defined in the inherited classes inlcluding these defaults */
    /*
    events: {
      'click .update-graph': 'handleUpdateGraph',
      'click .compare': 'triggerCompareOverlay'
    },
    */

    initialize: function(options) {
      this.parentView = options.viewsHolder.highChartsViews;
      this.viewsHolder = options.viewsHolder;
      this.bind('calculateAndDisplay', this.calculateAndDisplay);
    },

    handleUpdateGraph: function(){
      this.calculateAndDisplay();

      app.wt.trigger('lesson:' + this.model.get('id') + ':updateGraph', {
        user: app.user.get('UserId'),
        time: new Date(),
        model: this.model
      });

      return false;
    },

    //Calculations will be run using properties as already set, won't be pulling them in
    calculateAndDisplay: function(){
      this.model.resetToDefaults();

      this.model.performCalculations();

      this.updateHeaderCopy();

      this.parentView.trigger('updateGraph', this.model);
    },

    buildHeaderCopyHtml: function() {
      //MUST BE OVERRIDDEN
      return "repayment-option's view must override buildHeaderCopyHtml";
    },

    updateHeaderCopy: function() {
      var headerCopy = this.buildHeaderCopyHtml();
      this.parentView.trigger('updateHeaderCopy', headerCopy);
    },// END calculateHeaderCopy();

    triggerCompareOverlay: function(event){
      event.preventDefault();
      this.viewsHolder.compareView.trigger('handleCompareOverlay');

      app.wt.trigger('lesson:compare:' + this.model.get('id'), {
        user: app.user.get('UserId'),
        time: new Date()
      });

      return false;
    },

    handleQualifyOverlay: function(event){
      event.preventDefault();
      //open compare overlay
      $.fancybox.open({
        href : '#qualify-overlay'
      }, {
        autoSize:    false,
        modal:       false,
        autoWidth:   true,
        autoHeight:  true,
        closeClick:  false,
        openEffect:  'none',
        closeEffect: 'none',
        padding:     0,

        afterLoad: function() {
          $('a.ok').on('click', function(event) {
            $.fancybox.close(true);

            return false;
          });
        }
      });

      return false;
    }
  };

  return RepaymentViewMixin;
});
