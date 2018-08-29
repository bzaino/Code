/*global describe, it, chai, sinon */
define(['backbone', 'modules/FinancialStatus/Router'], function(Backbone, FsRouter) {

    var assert = chai.assert;

    /* WHAT I WANT TO TEST:

        -Backbone Router that define routes for the following pages
            -Landing page (default route)
            -Grid/Graph page
            -Federal Loan Detail page
    */

    var routerInstance = new FsRouter();

    describe('Router for Financial Status Tool', function () {
        it('should define routes for each of the "pages" in the tool', function () {
            assert.equal('landing', routerInstance.routes['']);
            assert.equal('grid', routerInstance.routes.grid);
            assert.equal('federalDetail', routerInstance.routes.detail);
        });
    });
});
