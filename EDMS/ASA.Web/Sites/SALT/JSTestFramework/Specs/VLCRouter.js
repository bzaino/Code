/*global describe, beforeEach, afterEach, it, chai, sinon, done */
define(['backbone',
    'modules/VLC/VLCRouter',
    'modules/VLC/VLCViews',
    'salt'
    ], function (Backbone, VLCRouter, views, SALT) {

    describe('VLC Router', function () {

        var assert = chai.assert;
        var router, routeSpy;

        var lastAttendedViewStub;

        beforeEach(function (done) {

            router = new VLCRouter();
            routeSpy = sinon.spy();

            lastAttendedViewStub = sinon.stub(views, 'LastAttendedView').returns(new Backbone.View());

            Backbone.history.start({ root: '/Navigator/' });
            router.navigate('FooStartingLocation', { trigger: true });

            done();
        });

        it('should define the proper handlers for each route', function () {
            assert.equal('changeSlide', router.routes['']);
            assert.equal('DoYouKnowHowMuchRoute', router.routes.DoYouKnowHowMuch);
            assert.equal('WhenWillYouGradRoute', router.routes.WhenWillYouGrad);
            assert.equal('LastAttendedRoute', router.routes.LastAttended);
            assert.equal('LastPaid270Route', router.routes.LastPaid270);
            assert.equal('ManageablePaymentYNRoute', router.routes.ManageablePaymentYN);
            assert.equal('InRepaymentRoute', router.routes.InRepayment);
            assert.equal('InSchoolYNRoute', router.routes.InSchoolYN);
            assert.equal('LoanImportRoute', router.routes.LoanImport);
            assert.equal('LoanUploadRoute', router.routes.LoanUpload);
            assert.equal('FindPinRoute', router.routes.FindPin);
            assert.equal('changeSlide', router.routes[':query']);
        });

        it('should fire default route event at proper times', function () {
            router.bind('route:changeSlide', routeSpy);
            router.navigate('RepaymentStandard1', {trigger: true});
            assert.ok(routeSpy.calledOnce);
            assert.ok(routeSpy.calledWith());
            router.navigate('AwayFromDefaultRoute', {trigger: true});
            router.navigate('');
            assert.ok(routeSpy.calledTwice);
        });

        it('should fire custom route events with correct arguments', function () {
            router.bind('route', routeSpy);
            router.navigate('LastPaid270', {trigger: true});
            assert.ok(routeSpy.calledOnce);
            assert.ok(routeSpy.calledWith('LastPaid270Route'));
        });

        it('should create view with correct DOM element for custom route', function () {
            router.navigate('LastAttended', { trigger: true });
            assert.ok(lastAttendedViewStub.calledOnce);
            assert.ok(lastAttendedViewStub.calledWithExactly({el: '#VLCBody'}));
        });

        it('should run route:changeSlide when user navigates to the main menu slide', function(){
            router.bind('route:changeSlide', routeSpy);
            SALT.trigger('navigate', '');
            assert.ok(routeSpy.calledOnce);
        });

        afterEach(function () {
            views.LastAttendedView.restore();
            Backbone.history.stop();
        });
    });
});
