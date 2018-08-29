define([
  "lesson3/app",

  // Libs
  "backbone",

  // Views
  "lesson3/modules/footer/views"

],

function(app, Backbone, Views) {

  // Create a new module
  var footer = app.module();

  // Our basic **Steps** model has `content` and `done` attributes.
  footer.Model = Backbone.Model.extend({
    // Default attributes for the todo.
    defaults: {
      amount: 0,
      description: 0,
      subtitle: 0,
      nextButton: 'next',
      next: '/content/media/Lesson/own-your-loans-wrapup/_/R-101-4488'
    },

    // Remove this Todo from *localStorage* and delete its view.
    clear: function() {
      this.destroy();
    }

  });

  // Attach the Views sub-module into this module.
  footer.Views = Views;

  // Required, return the module for AMD compliance
  return footer;

});
