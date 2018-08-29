GreeterTest = TestCase("GreeterTest");

GreeterTest.prototype.testGreet = function() {
  var greeter = new myapp.Greeter();
   assertEquals("Hello World!", greeter.greet("World"));
};

GreeterTest.prototype.testGreetWithTime = function() {
  var greeter = new myapp.Greeter();
  var currentTime = new Date();
  var hours = currentTime.getHours();
  var minutes = currentTime.getMinutes();
  if (minutes < 10){
     minutes = "0" + minutes;
  }
  var ToD =   (hours > 11) ? " PM" : " AM"
  var currentTimeString = hours + ":" + minutes + ToD
  
   assertEquals("Hello World!", greeter.greet("World"));
};
