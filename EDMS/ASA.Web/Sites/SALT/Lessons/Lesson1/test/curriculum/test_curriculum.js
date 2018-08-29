curriculumTest = TestCase("curriculumTest");

curriculumTest.prototype.testIntroUpdateTotal_Pass = function(){
   //Arrange
   /*:DOC += <div>
               <form>
                  <input class="large value" id="goal-value" type="text" value="$">
                  <input class="large time" id="goal-time" type="text">
               </form>
               <div id="total">
                   <div class="value" style=""></div>
                   <div class="value-message">monthly savings goal</div>
               </div>
            </div> */
   intro.global.utils.init();
   
   //Act
   $('#goal-time').val("10");
   $('#goal-value').val("1000");
   $('#goal-value').trigger('focus');

   //Assert
   var total = $('#total .value').text();
   assertTrue(total == "$100");
}

curriculumTest.prototype.testIntroUpdateTotal_0time = function(){
   //Arrange
   /*:DOC += <div>
               <form>
                  <input class="large value" id="goal-value" type="text" value="$">
                  <input class="large time" id="goal-time" type="text">
               </form>
               <div id="total">
                   <div class="value" style=""></div>
                   <div class="value-message">monthly savings goal</div>
               </div>
            </div> */
   intro.global.utils.init();
   
   //Act
   $('#goal-time').val("0");
   $('#goal-value').val("1000");
   $('#goal-value').trigger('focus');

   //Assert
   var total = $('#total .value').text();
   assertTrue(total == "$1,000");
}

curriculumTest.prototype.testIncomeUpdateTotal_Pass = function(){
   //Arrange
   /*:DOC += <form>
      <div class="box-wrapper clearfix">
         <input id="jobs" type="checkbox" value="Jobs" style="visibility: hidden; position: absolute; left: -90000px; " class="inputStyles" tabindex=""><div class="cbox styled-jobs checked" tabindex="0"><span></span></div>
         <label for="jobs" class="bold">Jobs &amp; Work Study</label>
         <div class="tip" style="display: block; ">
           <div class="wing"></div>
           <p>This is the total you make from any part-time or full-time jobs you may have.</p>
           <div class="details" style="display: block; ">
             <input id="jobs-income" type="text">
             <span>/</span>
             <select class="dropdown ddStyled" style="visibility: hidden; position: absolute; left: -90000px; ">
               <option selected="selected" value="week">Week</option>
               <option value="month">Month</option>
               <option value="semester">Semester</option>
               <option value="year">Year</option>
             </select><dl class="ddStyled dd"><dt><a href="#">Week</a></dt><dd><ul style="display: none; "><li data-name="week" class="hidden"><a href="#">Week</a></li><li data-name="month"><a href="#">Month</a></li><li data-name="semester"><a href="#">Semester</a></li><li data-name="year"><a href="#">Year</a></li></ul></dd></dl>
           </div>
         </div>
       </div>
   </form> 
   */
   income.global.utils.init();
   
   //Act
   $('#jobs-income').val('100');
   income.global.utils.gatherAllBoxes.init();

   //Assert
   var totalIncome = $('#total').find('.value');
   assertTrue(totalIncome == '$400');
}