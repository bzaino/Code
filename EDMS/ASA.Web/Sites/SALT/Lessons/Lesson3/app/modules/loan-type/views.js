define([
  'lesson3/app',

  // Libs
  'backbone',
  'lesson3/plugins/asa-plugins',
  'salt'
],

function(app, Backbone, asaPlugins, SALT) {
  
  var Views = {};
  var viewsHolder = {};

  Views.LoanType = Backbone.View.extend({
    template: 'content/loan-type',
    className: 'wrapper',
        
    initialize: function() {
      this.model.on("remove", function() {
        this.remove();
      }, this);
    },

    serialize: function() {
      return {
        name: this.model.get('loan'),
        value: this.model.get('value'),
        order: this.model.get('iteration')
      };
    }

  });
  
  Views.LoanTypeList = Backbone.View.extend({
    className: 'loan',
    beforeRender: function() {
      this.collection.each(function(LoanType) {
        this.insertView(new Views.LoanType({

          model: LoanType,
          afterRender: function() {

            var ele = $('.loan-type[data-iteration=' + this.model.get('iteration') + ']');

            if(this.model.collection.length == 2) {
              $('.add-another').hide();
            } else {
              $('.add-another').show();
            }

            ele.find('a.btn-remove').css('display','inline-block');

            //remove the remove button from the 1st element every time the collection is rendered
            $('.loan-type[data-iteration=1]').find('a.btn-remove').css('display','none');

            //repopulate input fields
            ele.find('.loan-type').val(this.model.get('loan'));
            ele.find('.loan-type-value').val(this.model.get('value')).formatCurrency({ roundToDecimalPlace: 0 });

          }
        }));
      }, this);

    },

    initialize: function(options) {
      var _this = this;

      this.parentView = options.viewsHolder.content;
      
      this.collection.on('add', function(LoanType,opts) {
        this.render();
      }, this);

      this.collection.on('remove', function() {
        this.render();
      }, this);

      //bind dropdown change
      this.bind('loanTypeChange', this.loanTypeChange);

      //bind dropdown change
      this.bind('stateChange', this.stateChange);

      SALT.on('dependents:changed', function (dependents) {
        _this.setDependentsDisplayField(dependents);
      });
      SALT.on('income:changed', function (income) {
        _this.setIncomeDisplayField(income);
      });
      SALT.on('interestRate:changed', function (interestRate) {
        _this.setInterestRateDisplayField(interestRate);
      });
      SALT.on('state:reset', function (state) {
        _this.resetStateDropdown(state);
      });
    },

    afterRender: function() {
      //placeholder text IE
      $('input[type=text]').placeHolder();
      //dropdowns
      this.prepareSelectElements();
      //update footer
      this.getData();
    },

    getData: function() {
      var theModel = this.collection.where({iteration: 1});      
      var sumLoanValues = theModel[0].get('sum');

      //pre-set existing values
      this.setIncomeDisplayField(theModel[0].get('income'));
      this.setDependentsDisplayField(theModel[0].get('dependents'));

      // tell other models to update
      SALT.trigger('dependents:changed', theModel[0].get('dependents')); //same value as on income-based model
      SALT.trigger('income:changed', theModel[0].get('income')); //used on income-based and Income-sensitive models
      SALT.trigger('state:changed', theModel[0].get('state')); //used on income-based and Income-sensitive models
      SALT.trigger('interestRate:changed', theModel[0].get('interestRate')); //used on all base model derived models
      
      //update footer value
      SALT.trigger('totalBalanceFooter:update');
    },
    setIncomeDisplayField: function(value) {
      $('#income').val(value).formatCurrency({ roundToDecimalPlace: 0 });
    },
    setDependentsDisplayField: function(value) {
      $('#dependents').val(value);
    },
    setInterestRateDisplayField: function(value) {
      $('#interestRate').val( (parseFloat(value * 100)).toFixed(2) + '%' );
    },
    /**
     * Generates the options for loan type selects, and initializes custom dropdowns
     */
    prepareSelectElements: function() {
      var $dropDowns = this.$el.find('.dropdown-large'),  
          numDropdownsSelected = $dropDowns.length,
          theCollection = this.collection,
          theModel = theCollection.where({ iteration: 1 }),
          dropDownOptions = ['2-4 year degrees', 'professional or grad school'];

      // If there is only one dropdown selected
      if( numDropdownsSelected == 1 ) {
        
        var options = '';

        // If 2-4 year degree is the first and only option selected
        if(theModel[0].get('loan') == dropDownOptions[0]) {            
          options += '<option selected="selected">' + dropDownOptions[0] + '</option>';
          options += '<option>' + dropDownOptions[1] + '</option>';
        
        // If grad school is the first and only option selected
        } else if (theModel[0].get('loan') == dropDownOptions[1]){
          options += '<option selected="selected">' + dropDownOptions[1] + '</option>';
          options += '<option>' + dropDownOptions[0] + '</option>';
        
        // If it's the first time, and nothing is set, just render
        } else {
          for( i=0; i<2; i++ ) {
            options += '<option>' + dropDownOptions[i] + '</option>';
          }
          
          var singleLoan = this.collection.where({iteration: 1});
          singleLoan[0].set({ loan: dropDownOptions[0] },{silent: true});
        }

        $dropDowns.eq(0).append(options);

      } else {

        var modelHasNotBeenSaved = (theModel[0].get('loan') === '') ? true : false;
        
        // Gets called when clicking 'add another', and data hasn't been saved yet
        if( modelHasNotBeenSaved ) {
          
          $dropDowns.each(function(key, value) {
            $(this).append('<option>' + dropDownOptions[key] + '</option>');
          });

        } else {
          var options = '';
          
          $dropDowns.each(function(key, value){

            // If it's the first select, assign value of first model
            if( key === 0 ) {
              options = '<option selected="selected">' + theModel[0].get('loan') + '</option>';
            } else {

              // set model in case the use doesn't touch the dropdown 
              var singleLoan = theCollection.where({iteration: 2}),
                  loanType = singleLoan[0];

              if( dropDownOptions[0] == theModel[0].get('loan') ){
                options = '<option>' + dropDownOptions[1] + '</option>';
                loanType.set({ loan: dropDownOptions[1] }, {silent: true});
              } else {
                options = '<option>' + dropDownOptions[0] + '</option>';
                loanType.set({ loan: dropDownOptions[0] }, {silent: true});
              }
            }

            $(this).append(options);
          });          
        }
      }

      //initialize state dropdown
      var $dropDownState = this.$el.find('.dropdown-state');

      var optionState = '';
      optionState += '<option value="other">Other</option>';
      optionState += '<option value="AK">Alaska</option>';
      optionState += '<option value="HI">Hawaii</option>';
      $dropDownState.eq(0).append(optionState);

      $('div.state select option[value="' + theModel[0].get('state') + '"]').attr('selected', true);

      // initialize custom dropdowns
      var elView = this;

      this.$el.find('.dropdown-large').dropkick({theme: 'large', change: function(value, label){
        var loanType = this.parents('.loan-type');
        elView.trigger('loanTypeChange', loanType, label);
      }});

      this.$el.find('.dropdown-state').dropkick({theme: 'large', change: function(value, label){
        SALT.trigger('state:changed', value);
      }});
      
    },
    resetStateDropdown: function (state) {
      var $option = $('#dk_container_'+ 'state-options' +' .dk_options a[data-dk-dropdown-value="'+ state +'"]');
      $option.trigger('click');
    },    
    events: {
      'click .btn-remove': 'removeOneTimeExpense',
      'keyup #total-balance': 'handleTotalBalanceChange',
      'change #income': 'handleIncomeChange',
      'change #dependents': 'handleDependentsChange',

      'blur #interestRate': 'handleInterestChange',
      'click #interest-less': 'handleInterestLess',
      'click #interest-more': 'handleInterestMore',
      'keyup #interestRate': 'handleInterestKeyUp'
    },
    loanTypeChange: function(loanType, label) {
      var loanTypeID = parseInt($(loanType).attr('data-iteration'), 10);
      var singleLoan = this.collection.where({iteration: loanTypeID});
      singleLoan[0].set({ loan: label },{silent: true});
    },
    handleIncomeChange: function(event){
      var element = $(event.target);
      element.val(element.val().replace(/[^0-9]+/g,""));

      if (isNaN(element.val())  || element.val() === '') {
        element.val(this.collection.models[0].get('income'));
        this.setIncomeDisplayField(element.val());
      }

      element.formatCurrency({ roundToDecimalPlace: 0 });
      var cleanValue = parseInt(element.val().replace(/[^0-9\.-]+/g,""), 10);
      SALT.trigger('income:changed', cleanValue);
    },
    handleDependentsChange: function(event){
      var element = $(event.target);
      element.val(element.val().replace(/[^0-9]+/g,""));

      if (isNaN(element.val()) || element.val() === '') {
        element.val(this.collection.models[0].get('dependents'));
        this.setDependentsDisplayField(element.val());
      }

      element.toNumber();
      var cleanValue = parseInt(element.val().replace(/[^0-9\.-]+/g,""), 10);
      SALT.trigger('dependents:changed', cleanValue);
    },
    handleTotalBalanceChange: function(event){
      $(event.target).toNumber().formatCurrency({ roundToDecimalPlace: 0 });

      var loanTypeID = parseInt($(event.target).parents('.loan-type').attr('data-iteration'), 10);

      var cleanInput = Number($(event.target).val().replace(/[^0-9\.-]+/g,"")); // Unescaped '-'.

      var singleLoanValue = this.collection.where({iteration: loanTypeID});
      singleLoanValue[0].set({ value: cleanInput },{silent: true});

      this.sumTotalBalance();
    },
    handleInterestChange: function(event){
      console.log('handleInterestChange');
      var element = $('#interestRate');

      if (isNaN(element.val()) || element.val() === '' || (parseFloat(element.val()) * 0.1).toFixed(2) > 25) {
        element.val(this.collection.models[0].get('InterestRate'));
        console.log('handleInterestChange:NaN', element.val());
        this.setInterestRateDisplayField(element.val());
      }
      console.log('handleInterestChange:After NaN', element.val());

      if(element.val() < 0.01){
        element.val('0.01');
      }
      
      element.val(parseFloat(element.val()).toFixed(2) + '%');
      
      var interestRate = (parseFloat(element.val()) * 0.01).toFixed(4);
      SALT.trigger('interestRate:changed', interestRate);
    },
    handleInterestKeyUp: function(event){
      console.log('handleInterestKeyUp');
      var element = $(event.target);
      //ensure the data is a number only
      element.val(element.val()).toNumber();
    },
    handleInterestLess: function(event){
      console.log('handleInterestLess');
      var interestElement = $(event.target).siblings('input');
      var interest = parseFloat(interestElement.val());
      
      interest = (interest - 0.01);
      
      if(interest < 0){
        interest = 0.01;
      } else{
        interestElement.val(parseFloat(interest.toFixed(2) ));
      }
      
      this.handleInterestChange();
      return false;
    },
    handleInterestMore: function(event){
      console.log('handleInterestMore');
      var interestElement = $(event.target).siblings('input');
      var interest   = parseFloat(interestElement.val());
      
      interest = (interest + 0.01);
      
      interestElement.val(parseFloat(interest.toFixed(2) ));
      
      this.handleInterestChange();
      return false;
    },

    removeOneTimeExpense: function(event) {
      event.preventDefault();

      var loanRepaymentID = parseInt($(event.target).parents('.loan-type').attr('data-iteration'), 10);
      var singleLoanType = this.collection.where({iteration: loanRepaymentID});

      //destory local storage
      singleLoanType[0].destroy();
      this.collection.remove(singleLoanType[0]);

      this.sumTotalBalance();
      return false;
    },

    sumTotalBalance: function() {
      //Add all imported inputs
      var sumLoanValues = 0;
      $('.loan-type-value').each(function(key){
        var loanValue = Number($(this).val().replace(/[^0-9\.-]+/g,""));
        sumLoanValues += loanValue;
      });

      var singleLoanSum = this.collection.where({iteration: 1});
      singleLoanSum[0].set({ sum: sumLoanValues },{silent: true});

      //udpate footer value
      SALT.trigger('totalBalanceFooter:update');
      
      //trigger barGraph change
      this.parentView.trigger('updateBarGraph');

    }

  });

  return Views;

});
