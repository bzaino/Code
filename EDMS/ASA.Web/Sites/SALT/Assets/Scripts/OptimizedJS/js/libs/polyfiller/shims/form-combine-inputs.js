webshims.register('form-combine-inputs', function($, webshims, window, document, undefined, options){
	var defaults = {
		type: 'sync', //'sync' || 'range
		syncProperty: 'valueAsNumber',
		syncInitialFrom: false, //selector of the element that needs to synced
		minRangeLength: 0,
		rangeBehavior: 'value' // 'value' || 'constrain' //value: changes corresponding inputs value || constrain: constrains using min/max
	};
	
	var syncElements = {
		_create: function(){
			console.log(this, arguments)
			this.bind(this);
		},
		bind: function(that){
			var timer;
			var callSync = function(){
				that.sync();
			};
			var onSync = function(e){
				clearTimeout(timer);
				that.syncElement = e.target;
				timer = setTimeout(callSync, e.type == 'input' ? 9 : 1);
			};
			$(this.options.element).on('input change wsvalchange', onSync)
		},
		sync: function(){
			var val = $.prop(this.syncElement, this.options.syncProperty);
			$('input', this.options.element)
				.getNativeElement()
				.not(this.syncElement)
				.prop(this.options.syncProperty, val)
			;
		}
	};
	
	$.fn.combineInputs = function(opts){
		opts = $.extend({}, defaults, opts);
		return this.each(function(){
			if(opts.type == 'sync'){
				webshims.objectCreate(syncElements, {}, $.extend({element: this}, opts));
			}
		});
	};
	
	$.fn.combineInputs.defaults = defaults;
	
	var convertValues = (function(){
		var types = {};
		var getConverter = function(type){
			if(typeof type != 'string'){
				type = $(type).prop('type');
			}
			var input;
			if(!types[type]){
				input = $('<input type="'+type+'" step="any" />');
				types[type] = {
					asNumber: function(val){
						var type = (typeof val == 'object') ? 'valueAsDate' : 'value';
						return input.prop(type, val).prop('valueAsNumber');
					},
					asValue: function(val){
						var type = (typeof val == 'object') ? 'valueAsDate' : 'valueAsNumber';
						return input.prop(type, val).prop('value');
					},
					stepAsNumber: function(val){
						var ret = 1;
						try {
							ret = input.prop('valueAsNumber', 0).prop('step', val).stepUp();
						} catch(er){}
						input.prop('step', 'any');
						return ret;
					}
				};
			}
		};
		return {
			asValue: function(type, val){
				getConverter(type).asValue(val);
			},
			asNumber: function(type, val){
				getConverter(type).asNumber(val);
			},
			stepAsNumber: function(type, val){
				getConverter(type).stepAsNumber(val);
			}
		};
	})();
});
