define([
    'jquery',
    'underscore',
    'jquery.zglossary'
], function ($, _) {
    /* These are sorted by tag, numeric, alphebetically upper then lower - keep them in order of js-attach-glossary elments which are sorted below*/
    /* first tag found gets tip currently */
    var tips = [
        /*529 PLAN*/
        'Money from a 529 college savings plan allows you to pay for higher education expenses tax-free. According to <a href="https://salliemae.newshq.businesswire.com/sites/salliemae.newshq.businesswire.com/files/doc_library/file/HowAmericaSaves2015_FINAL.pdf" target="_blank">Sallie Mae</a>, parents have an average of $3,089 saved in 529 accounts.',
        /*BOOKS*/
        '<a href="http://trends.collegeboard.org/college-pricing/figures-tables/average-estimated-undergraduate-budgets-2015-16" target=_blank">The College Board</a> says students spent approximately $1,300 on books and supplies in 2015-2016. Save by buying used, renting, borrowing from a library or friend, or using eBooks.',
        /*Can’t Find Your Scholarship Or Grant?*/
        'Award letters can list grants and scholarships in several ways. Different schools and states even have their own names for some of them. Enter awards you’re unsure about as "Other."',
        /*COVERDELL ESA*/
        'Money from a Coverdell Education Savings Account (ESA) allows you to pay for elementary, secondary, or college expenses tax-free. According to <a href="https://salliemae.newshq.businesswire.com/sites/salliemae.newshq.businesswire.com/files/doc_library/file/HowAmericaSaves2015_FINAL.pdf" target="_blank">Sallie Mae</a>, parents have an average of $296 saved in Coverdell ESA accounts.',
        /*FEDERAL PLUS LOANS*/
        'Loans provided by the federal government that allow Graduate Students or parents (of dependent undergraduate students) to borrow up to the entire cost of the education. PLUS loans offer repayment benefits that generally aren’t available with private loans, and the borrower (parents/grad students) is solely responsible for repaying them.',
        /*FEDERAL STUDENT LOANS*/
        'Loans provided by the federal government that allow borrowers to adjust repayment terms, temporarily pause payments, and (in some cases) have balances forgiven. Because of these benefits, students should exhaust these funds before borrowing other loan types.',
        /*FEES*/
        'Schools usually require fees for student services, administrative functions, and other expenses. These fixed costs may be combined with tuition on award letters.', 
        /*Gifts*/
        'Money received as gifts for graduation, birthdays, and holidays, or one-time contributions from family members.',
        /*HOUSING*/
        'Housing costs may be part of "room and board" on award letters. According to <a href="http://trends.collegeboard.org/college-pricing/figures-tables/tuition-and-fees-and-room-and-board-over-time-1975-76-2015-16-selected-years" target= "_blank">The College Board</a>, room and board averaged $11,516 (private, 4-year school) and $10,138 (public, 4-year school) in 2015-2016. Students may save by living off campus or at home.',
        /*INVESTMENTS*/
        'Stocks, bonds, or CDs that will be cashed in to pay for this program of study. According to <a href="https://salliemae.newshq.businesswire.com/sites/salliemae.newshq.businesswire.com/files/doc_library/file/HowAmericaSaves2015_FINAL.pdf" target="_blank">Sallie Mae</a>, parents have an average of $2,633 in investments/CDs to pay for college.',
        /*MEALS*/
        'Meal costs may be included as part of \'room and board\' on award letters. Per <a href="http://trends.collegeboard.org/college-pricing/figures-tables/tuition-and-fees-and-room-and-board-over-time-1975-76-2015-16-selected-years" target=_blank">The College Board</a>, room and board averaged $11,516 (private, 4-year school) and $10,138 (public, 4-year school) in 2015-2016. Students may save by living off campus or at home. Look for a less expensive meal plan or prepare meals at home to reduce this amount.',
        /*Other*/
        'These are expenses not covered above, like a computer, insurance, clothing, and medications. <a href="http://trends.collegeboard.org/college-pricing/figures-tables/average-estimated-undergraduate-budgets-2015-16" target="_blank">The College Board</a> says students averaged about $2,000 on "other" expenses in 2015-2016.',
        /*Other Contributions*/
        'Any other money currently on hand to pay for this program of study.',
        /*PRIVATE/OTHER LOANS*/
        'Loans provided by banks, schools, or other private entities. Loan terms vary by lender, but generally offer less flexibility and fewer repayment options than federal loans.<br><br>Some families select this option over PLUS loans because parents may not be solely responsible for repaying private loans and they may be eligible for a lower interest rate.',
        /*Savings*/
        'Money set aside in a bank account to pay for this program of study. According to <a href="https://salliemae.newshq.businesswire.com/sites/salliemae.newshq.businesswire.com/files/doc_library/file/HowAmericaSaves2015_FINAL.pdf" target="_blank">Sallie Mae</a>, parents have an average of $1,638 in savings accounts for college.',
        /*SCHOOL YEARS LEFT TO FUND*/
        'Select the number of years remaining in the student\'s current program of study. Only include the current year if it hasn\'t yet been funded.',
        /*SINGLE YEAR COST*/
        'This estimates the total cost to attend this school for 1 year. For each additional year in your plan, we\'ll add 4% for inflation.',
        /*SINGLE YEAR FREE MONEY*/
        'This number estimates this free money for 1 year. While grant and scholarship amounts can change each year, we’ll assume this figure remains the same throughout this plan.',
        /*SUPPLIES*/
        'Supply costs include tools and materials required for classes (other than books). This amount can vary based on a students major and electives. According to <a href="http://trends.collegeboard.org/college-pricing/figures-tables/average-estimated-undergraduate-budgets-2015-16" target="_blank">The College Board</a>, students spent approximately $1,300 on books and supplies in 2015-2016.',
        /*Total Interest*/
        'This estimate uses the standard 10-year repayment plan using the current federal interest rates for Direct Stafford Unsubsidized (rates for undergraduates and graduates) and PLUS and 10% for private loans. Your actual rates will depend on your student loans.',        
        /*TRANSPORTATION*/
        'Per <a href="http://trends.collegeboard.org/college-pricing/figures-tables/average-estimated-undergraduate-budgets-2015-16" target="_blank">The College Board</a>, students averaged more than $1,000 on transportation in 2015-2016. This amount depends on factors like whether the student has a car on campus or commutes to school, as well as expenses like airfare and train tickets.',
        /*TUITION*/
        'Tuition is the cost of taking classes at a school. The <a href="http://trends.collegeboard.org/college-pricing/figures-tables/tuition-and-fees-and-room-and-board-over-time-1975-76-2015-16-selected-years" target="_blank">College Board</a> says tuition and fees averaged $32,405 (private, 4-year school) $9,410 (public, 4-year school), and $3,435 (public, 2-year school) in 2015-2016.',
        /*YEAR IN SCHOOL*/
        'Choose the student\'s current status. If the student just finished a school year, choose the year they\'re entering. High school students should select "Freshman."',
        /*add grants and scholarships*/
        'You can find some grants and scholarships on the award letter. If you or your child has others, add them here, too.',
        /*cost of attendence for 1 year*/
        'Look for the cost of attendance on the <a href="/content/media/Article/deconstructing-your-financial-aid-award-letter/_/R-101-15941" target="_blank">award letter</a>&mdash;or use the U.S. Department of Education\'s <a href="https://fafsa.ed.gov/FAFSA/app/f4cForm" target="_blank">FAFSA4caster</a> to estimate a school\'s costs.',
    ], 
    terms = [],
    termsTemp = [];

    _.each($('.js-attach-glossary'), function (element) {
        termsTemp.push({
            term: element.textContent
        });
    });
        
    termsTemp = _.chain(termsTemp).pluck('term').uniq().sortBy().value();

    _.each(termsTemp, function (value, index) {
        terms.push({
            term: value,
            definition: tips[index]
        });
    });

    $('.ccp-use-as-glossary').glossary(terms, {showForImage: true, hideTerm: true, showonce: true, excludetags: ['A', 'H1', 'H2', 'H3', 'H4', 'H5', 'H6', 'P', 'STRONG', 'tspan', 'text']});

    $('.glossaryTerm').click(function (e) {
        $(this).trigger('show-tip', e);
        e.stopPropagation();
    });

    $('.js-question-mark').click(function (e) {
        $(e.currentTarget).closest('h2').find('.glossaryTerm').trigger('show-tip', e);
        e.stopPropagation();
    });
});
