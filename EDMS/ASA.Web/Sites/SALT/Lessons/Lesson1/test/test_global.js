notificationTestAS = AsyncTestCase("notificationTestAS");


//This test will validate that hitting the close button on an individual notification causes it to hide
notificationTestAS.prototype.testCloseNotification = function(queue){
  //Arrange
//  expectAsserts(3);

  //Insert a notification to close as part of test setup
  /*:DOC += <div class="notification">
              <div class="notification-content clearfix">
                <span class="icon maintenance"></span>
                <span class="heading replaced heading-hey-now">HEY NOW!</span>
                <span class="message">SALT will be down on Saturday, March 17 from 2AM–6AM for maintenance</span>
                <a class="dismiss" href="#" title="Dismiss">Dismiss</a>
              </div>
            </div> */
  
  ASA.global.utils.notifications.init();
  var notification;
  var closeNotification;
  var callback = function() {};
  
  queue.call('Step 1: Arrange', function(callbacks) {
      notification = $('div.notification');
      assertTrue(notification.is(":visible"));
      closeNotification = $(notification).find('a.dismiss');
      assertTrue(closeNotification.is(":visible"));   
  });
  
  queue.call('Step 2: Act', function(callbacks) {
     $(closeNotification).trigger('click');     
     //register callback function
          var callbackWrapper = callbacks.add(callback);
          //set one second timer on the wrapper
          window.setTimeout(callbackWrapper, 1000);
  });
  
  queue.call('Step 3: Assert', function(callbacks) {
     console.log('test');
     assertFalse(notification.is(":visible"));     
  });

}


//GreeterTest = TestCase("GreeterTest");

/*
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
*/

var html_testCloseNotification = '';   