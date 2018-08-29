/*global describe, beforeEach,it, afterEach, chai, sinon, done*/
define([
    'jquery',
    'salt',
    'underscore',
    'backbone',
    'modules/VLC/VLCModels',
    'modules/VLC/VLCViews'
], function ($, SALT, _, Backbone, VLCModels, views) {
    var assert = chai.assert;
    var saltSpy;
    var progressModel;

    describe('VLCEvents', function () {
        describe('VLC Model Events', function () {

            beforeEach(function (done) {
                saltSpy = sinon.spy();
                progressModel = new VLCModels.ProgressModel();
                done();
            });

            it('should trigger calculation when model changes', function () {
                SALT.bind('calculation:needed', saltSpy);
                progressModel.set('Plan', 'foo');
                assert.ok(saltSpy.calledOnce);
            });
            it('should trigger 3 calculations, and only 3 when 3 watched values are altered', function () {
                SALT.bind('calculation:needed', saltSpy);
                progressModel.set('Plan', 'foo');
                progressModel.set('AmountBorrowed', 'foo');
                progressModel.set('InterestRate', 'foo');
                assert.ok(saltSpy.calledThrice);
            });
            it('should trigger 1 calculation when any number of unwatched values are altered but any one watched value is altered', function () {
                SALT.bind('calculation:needed', saltSpy);
                progressModel.set('Plan', 'foo');
                progressModel.set('EnrollmentStatus', 'foo');
                progressModel.set('GraduationDate', 'foo');
                assert.ok(saltSpy.calledOnce);
            });
            it('should trigger 5 calculations', function () {
                SALT.bind('calculation:needed', saltSpy);
                progressModel.set('Plan', 'foo');
                progressModel.set('AmountBorrowed', 'foo');
                progressModel.set('InterestRate', 'foo');
                progressModel.set('HouseholdIncome', 'foo');
                progressModel.set('PeopleInHouseholdCount', 'foo');
                assert.equal('5', saltSpy.callCount);
            });

        });

        describe('VLC View Events', function () {

            describe('Start Over Button', function(){
                var homeSpy = sinon.spy();
                var homeSpy1 = sinon.spy();
                var homeSpy2 = sinon.spy();

                it('should run the startOverClicked event when user clicks #startOver button', function () {
                    SALT.bind('startOverClicked', homeSpy);
                    var applicationView = new views.ApplicationView();
                    assert.equal('startOverClicked', applicationView.events['click #startOver']);
                });

                it('should fire navigate event when startOverClicked runs', function () {
                    SALT.bind('navigate', homeSpy1);
                    var applicationView = new views.ApplicationView();
                    applicationView.startOverClicked();
                    assert.ok(homeSpy1.calledOnce);
                });

                it('should run default route when navigate event fires', function () {
                    SALT.bind('navigate', homeSpy2);
                    SALT.trigger('navigate', '');
                    assert.ok(homeSpy2.calledOnce);
                });


            });

            describe('Contextual Help', function () {
                var ctxHelpView;
                var ctxHelpSpy;

                var saltSpy3 = sinon.spy();
                var saltSpy4 = sinon.spy();

                beforeEach(function (done) {
                    ctxHelpView = new views.ContextualHelp();
                    ctxHelpSpy = sinon.spy();
                    done();
                });
                /*
                it('should trigger renderTemplate:needed when render fires', function () {
                    SALT.bind('renderTemplate:needed', ctxHelpSpy);
                    ctxHelpView.render();
                    assert.ok(ctxHelpSpy.calledOnce);
                });
                */
                it('should fire render when slide:change triggers', function () {
                    var foundationStub = sinon.stub($.fn, 'foundation');
                    ctxHelpSpy = sinon.spy(views.ContextualHelp.prototype, 'render');
                    ctxHelpView = new views.ContextualHelp();
                    SALT.trigger('slide:change');
                    assert.ok(ctxHelpView.render.calledOnce);
                    ctxHelpSpy.restore();
                    foundationStub.restore();
                });

                afterEach(function () {
                    saltSpy3.reset();
                    saltSpy4.reset();
                });
            });

            describe('DoYouKnowHowMuch', function () {
                var doYouKnowView;
                var saltSpy1 = sinon.spy();
                var saltSpy2 = sinon.spy();

                beforeEach(function (done) {
                    doYouKnowView = new views.DoYouKnowHowMuchView();
                    done();
                });
                /*
                it('should fire render when slide:change triggers', function () {
                    SALT.bind('getDataAndRender:needed', saltSpy1);
                    SALT.bind('vlcWT:fire', saltSpy2);

                    var doYouKnow = new views.DoYouKnowHowMuchView();

                    assert.ok(saltSpy1.calledOnce);
                    assert.ok(saltSpy2.calledOnce);
                });
                */
                afterEach(function () {
                    saltSpy1.reset();
                    saltSpy2.reset();
                });
            });

            describe('FeaturedContent', function () {
                var saltSpy1;
                var saltSpy2;
                var featuredContentView;

                beforeEach(function () {
                    saltSpy1 = sinon.spy();
                    saltSpy2 = sinon.spy();
                    featuredContentView = new views.FeaturedContent();
                });

                it('should fire render when slide:change triggers', function () {
                    var foundationStub = sinon.stub($.fn, 'foundation');
                    saltSpy1 = sinon.spy(views.FeaturedContent.prototype, 'render');
                    var featuredContent = new views.FeaturedContent();
                    SALT.trigger('slide:change');
                    assert.ok(saltSpy1.calledOnce);
                    foundationStub.restore();
                });
                /*
                it('should trigger renderTemplate:needed when render fires', function () {
                    SALT.bind('renderTemplate:needed', saltSpy1);
                    featuredContentView.render();
                    assert.ok(saltSpy1.calledOnce);
                });
                */

                afterEach(function () {
                    saltSpy1.reset();
                    saltSpy2.reset();
                });

            });
        });
    });
});
