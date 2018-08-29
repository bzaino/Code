
//Page object
function PageContent (step,chapter,valueMessage, upNext, nextButton, saveData) {
  //Properties
  this.step = step,
  this.chapter = chapter,
  this.valueMessage = valueMessage,
  this.upNext = upNext,
  this.nextButton = nextButton,
  this.saveData = saveData;


},

//update html
PageContent.prototype.updateContext = function(step,chapter,valueMessage, upNext, nextButton, saveData) {
  //update all the text respectively using the data that gets sent over from the local JS files
  //header
  $('#header-center .step1').text("Step " + step);

  //chapter
  $('#header-center .chapter').text(pages[curriculum.global.utils.paginate.currentPagePosition].title);

  //footer
  $('#total .value-message').text(valueMessage);
  $('#up-next .second').text(upNext);

  if(!nextButton){
    nextButton = "keep going";
  }

  $('#footer .right .right-button').text(nextButton);
},

//this doesn't make sense
PageContent.prototype.saveData = function(){
  userData.income
},

//New page
var pageIntro = new pageContent("1", "Introduction", "monthly savings goal", "What’s coming in?", "Let’s Go");

pageIntro.updateContext();
pageIntro.saveData();

//Expenses model
eating:
  displayName: "Eating"
  editValue: 50
  newValue: 50
  time: "month"
  value: 50

entertainment:
  displayName: "Entertainment"
  editValue: 40
  newValue: 40
  time: "month"
  value: 40

groceries:
  displayName: "Groceries"
  editValue: 100
  newValue: 100
  time: "month"
  value: 100

health:
  displayName: "Health"
  diet:
    displayName: "Diet"
    editValue: 5
    newValue: 5
    time: "month"
    value: 5
  gym:
    displayName: "Gym Membership"
    editValue: 5
    newValue: 5
    time: "month"
    value: 5
  other:
    displayName: "Other"
    editValue: 5
    newValue: 5
    time: "month"
    value: 5
  salon:
    displayName: "Salon"
    editValue: 5
    newValue: 5
    time: "month"
    value: 5
  tanning:
    displayName: "Tanning"
    editValue: 5
    newValue: 5
    time: "month"
    value: 5

housing:
  displayName: "Housing"
  selected: "own"
  cost:
    cleaning:
      displayName: "Cleaning / Lawn Maintenance"
      editValue: 5
      newValue: 5
      time: "month"
      value: 5
    hoa:
      displayName: "Home Owners Association"
      editValue: 5
      newValue: 5
      time: "month"
      value: 5
    insurance:
      displayName: "Insurance"
      editValue: 5
      newValue: 5
      time: "month"
      value: 5
    mortgage:
      displayName: "Mortgage"
      editValue: 5
      newValue: 5
      time: "month"
      value: 5
    repairs1:
      displayName: "Repairs / Renovations"
      editValue: 5
      newValue: 5
      time: "month"
      value: 5
    taxes:
      displayName: "Property Taxes"
      editValue: 5
      newValue: 5
      time: "month"
      value: 5

other:
  displayName: "Other"
  more-stuffs:
    displayName: "more stuffs"
    editValue: 21
    name: "more-stuffs"
    newValue: 21
    time: "month"
    value: 21
  stuffs:
    displayName: "stuffs"
    editValue: 50
    name: "stuffs"
    newValue: 50
    time: "month"
    value: 50

savings:
  displayName: "Savings"
  editValue: 200
  newValue: 200
  time: "month"
  value: 200

school:
  displayName: "School"
  cost:
    loan:
      displayName: "Loan Payments"
      editValue: 5
      newValue: 5
      time: "month"
      value: 5
    textbooks:
      displayName: "Textbooks / Supplies"
      editValue: 5
      newValue: 5
      time: "month"
      value: 5
    tuition:
      displayName: "Tuition & School Fees"
      editValue: 5
      newValue: 5
      time: "month"
      value: 5

technology:
  displayName: "Phone & Technology"
  editValue: 30
  newValue: 30
  time: "month"
  value: 30

transportation:
  bike_1:
    displayName: "Bike"
    editValue: 50
    newValue: 50
    time: "month"
    value: 50
  car_0:
    displayName: "Car"
    gas:
      displayName: "Gas"
      editValue: 5
      newValue: 5
      time: "month"
      value: 5
    insurance:
      displayName: "Insurance"
      editValue: 5
      newValue: 5
      time: "month"
      value: 5
    maintenance:
      displayName: "Maintenance"
      editValue: 5
      newValue: 5
      time: "month"
      value: 5
    parking:
      displayName: "Parking"
      editValue: 5
      newValue: 5
      time: "month"
      value: 5
    payment:
      displayName: "Payment / Lease"
      editValue: 5
      newValue: 5
      time: "month"
      value: 5
    taxes:
      displayName: "Taxes / Registration"
      editValue: 5
      newValue: 5
      time: "month"
      value: 5

utilities:
    displayName: "Utilities"
    cost:
      cable:
        displayName: "Cable / Satellite"
        editValue: 5
        newValue: 5
        time: "month"
        value: 5
      electric:
        displayName: "Electric"
        editValue: 5
        newValue: 5
        time: "month"
        value: 5
      garbage:
        displayName: "Garbage / Sanitation"
        editValue: 5
        newValue: 5
        time: "month"
        value: 5
      gas:
        displayName: "Gas / Heat"
        editValue: 5
        newValue: 5
        time: "month"
        value: 5
      laundry:
        displayName: "Laundry"
        editValue: 5
        newValue: 5
        time: "month"
        value: 5
      parking:
        displayName: "Parking"
        editValue: 5
        newValue: 5
        time: "month"
        value: 5
      phone:
        displayName: "Phone / Internet"
        editValue: 5
        newValue: 5
        time: "month"
        value: 5
      water:
        displayName: "Water"
        editValue: 5
        newValue: 5
        time: "month"
        value: 5

;
