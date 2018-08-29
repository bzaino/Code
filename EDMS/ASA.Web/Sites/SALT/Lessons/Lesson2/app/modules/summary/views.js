define([
  "lesson2/app",

  // Libs
  "backbone",
  "lesson2/plugins/asa-plugins"
],

function(app, Backbone, asaPlugins) {
  
  var Views = {};
  var importedExpenseNames = [];
  var importedExpenseValues = [];
 

  Views.summary = Backbone.View.extend({
    template: "content/summary",
    className: 'wrapper',
    
    initialize: function(options) {

    },
    afterRender: function(){
      
      $('#right-column').hide();
      
      
      /**
      	loop through imported collection and save out data to arrays.
      */
      $.each(app.router.collections.importedCollection.models, function(key,value){
        importedExpenseNames.push(value.get('name'));
        importedExpenseValues.push(value.get('value'));
        
      });
      
      
      
      
      this.initGraph();
      chart.series[0].setData(importedExpenseValues, false);
      chart.redraw();
    },
    initGraph: function(){
      chart = new Highcharts.Chart({
          chart: {
              renderTo: 'container',
              type: 'column',
              //zoomType: 'x'
          },
          credits: {
              enabled: false
          },
          exporting: {
              enabled: false
          },
          title: {
              text: ' '
          },
          legend: {
              enabled: false,
              borderWidth: 0
          },
          xAxis: {
              title: {
                text: ''
              },
              tickInterval: 1,
              labels: {
                  formatter: function() {
                    var inc;
                    
                    inc = importedExpenseNames[this.value];
                      
                    return inc;
                  }
              },
              gridLineDashStyle: 'Dot',
              gridLineColor: '#bfbfbf',
              gridLineWidth: 1
          },
          yAxis: {
              title: {
                  text: ''
              },
              labels: {
                  formatter: function() {
                      if (this.value < 0) {
                        //if number is negative change the placement of the minus sign to before the dollar sign
                        var num = this.value;
                        var str = num.toString();
                        var n = str.replace(/\-/gi, "\-\$");
                        return n;
                      } else {
                        return '$' + this.value;
                      }
                  }
              },
              gridLineDashStyle: 'Dot',
              gridLineColor: '#bfbfbf'
          },
          tooltip: {
              enabled: false
          },
          
          plotOptions: {
              area: {
                  stacking: 'normal',
                  marker: {
                      enabled: true,
                      symbol: 'circle',
                      radius: 2
                  }
              },
              series: {
                  pointPadding: 0,
                  groupPadding: 0
                  
              }
          },
          series: [
          {
              //Series 1
              name: 'Series 1',
              color: '#99f3c6',
              shadow: false,
              lineWidth: 3
          }
          ]
      });
      
    }
    
  });
 
  return Views;

});
