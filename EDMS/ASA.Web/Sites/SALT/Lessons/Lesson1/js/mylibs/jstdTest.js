myapp = {};

myapp.Greeter = function() { };

myapp.Greeter.prototype.greet = function(name) {
  return "Hello " + name + "!";
};

myapp.Greeter.prototype.greetWithTime = function(name) {
  return "Hello " + name + "! The current time is";
};
