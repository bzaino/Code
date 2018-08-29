var expenses = expenses || {};

save = expenses;
var savedObject = {};

expenses.global = {
  step: 3,
	utils: {
    init: function(){
      // Include stylesheet & append ID for this page
      $('#content-container .content').attr('id','curriculum-expenses');
      
      LazyLoad.css('css/curriculum/expenses.css', function(){
        curriculum.global.viewport.animateViewport.normal();
      });
      
      
      // Update page context
      $('#total .value').text("0");
      curriculum.global.utils.paginate.updateContext("3", "Your Expenses", "things I spend money on", "Itemize expenses", "Keep Going");
      
      //trigger webtrends
      curriculum.global.tracking.trigger("lesson:step:start", {
        step: {
          number: save.global.step
        }
      });
      
      var path = 'expenses.html';
      for(i = 0; i< pages.length; i++){
        if(pages[i].path === path){
          curriculum.global.utils.paginate.currentPagePosition = i;
          break;
        }
      }
      
  		

      // Select Expenses
      $('ul.expenses-selections li').click(function(){
        $(this).toggleClass('selected');
        var numSelected = $('ul.expenses-selections li.selected').length;
        $('#total .value').text(numSelected).stop(true,true).effect("highlight", {color: '#faff00'}, 1500);
        
        if(numSelected === 1){
          $('.value-message').text('thing I spend my money on');
        } else {
          $('.value-message').text('things I spend my money on');
        }
        
        //if you've already selected expenses in the past
        var deselected = $(this).attr('data-expense');
        //var deselected = $(this).hasClass('selected');
        
        
        if(userData.expenseList != undefined){
          if(!$(this).hasClass('selected') && userData.expenseList[deselected] != undefined){
            userData.expenseList[deselected] = 0;
            curriculum.global.utils.animateGraph.refresh();
            
            //deleting data from actual expenses array if it gets taken out
            
            delete userData.expenses[deselected];
            delete userData.expenseList[deselected];
          }
        }
        
        if($('.selected').length != 0){
          $('#footer .error-msg').fadeOut();
        } else {
          //$('#footer .error-msg').fadeIn();
        }
        
      });
      
      
      expenses.global.utils.preloadData();

      curriculum.global.utils.animateGraph.refresh();
    },
		saveData: function(){
		  //rip out the expense pages from the pages array
  		pages.splice(3,expensePages.length);
  		
		  //collect the selected elements and put them into the emptied expensePages array
		  expensePages = [];
		  $('#curriculum-expenses ul.expenses-selections li.selected').each(function(){
		    current = $(this).attr('data-expense');
	    //expensePages.push('expenses/' + current + '.html');
		    
		    //newPages fix
		    currentPath = 'expenses/' + current + '.html';
		    currentJSPath = 'js/curriculum/' + current + '.js';
		    //fix this - the way the title is being created, could it be better?
		    expensePages.push({path: currentPath, title: pages[2].title, parent: 'expenses', remove: 'true', js: [currentJSPath]});
		    
		    //save the current into a local object that will get pushed into the real userData object
		    
		    
		    
		    if(userData.expenseList == undefined){
  		    userData.expenseList = {};
		    }
		    
  		  if(userData.expenseList[current] == undefined){
    		    //console.log('initing: ' + current);
    		    savedObject[current] = 0;
		    }
  		  
		    
  		   
		    
		    
		  });
		  
		  
		  //push into newPages the modified pages structure
		  for(i=0; i < expensePages.length; i++){
			  pages.splice(3+i, 0, expensePages[i]);
		  }


			curriculum.global.utils.server.saveToServer(userData);
		  
		}, //END saveData
		preloadData: function(){
		  //preloadExisting Data
		  //console.log('preloadData()');
		  if(!userData.expenses){
		    //console.log('first time so init userData.expenses');

        // step data was not preloaded; set tracking flag to false.
        curriculum.global.tracking.preloaded = false;

		    userData.expenses = {};
		  }
      if(userData.expenseList !== undefined){
        // step data was preloaded; set tracking flag to true.
        curriculum.global.tracking.preloaded = true;

        $.each(userData.expenseList,function(key, value){
          $('#curriculum-expenses ul.expenses-selections li.' + key +'').trigger('click');
        });
        
        
      }
      
      //$("#content-container .content").delay(360).animate({opacity: 1}, 1000);
    }, // END preloadData
    errors: function(){
      
      // check to see if pagePass is true
      if($('.selected').length != 0){
        pagePass = true;
        $('#footer .error-msg').fadeOut();
        curriculum.global.utils.paginate.next();
      } else {
        pagePass = false;
        $('#footer .error-msg').fadeIn();
      }
      
    }
	} // END: utils
	
}; // END expenses global var

$('document').ready(function(){
	// activate utils
	expenses.global.utils.init();
});
