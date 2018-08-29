  /*global describe, beforeEach,it, afterEach, chai, sinon, done*/
define([
	'jquery',
	'salt',
	'salt/schoolLookup'
], function ($, SALT, SchoolLookup) {
	var assert = chai.assert;
	describe('School Lookup', function () {
		describe('cleanup routine', function () {
			var spy;
			spy = sinon.spy(SchoolLookup, 'cleanup');

			before(function (done) {
				SchoolLookup.config.counter = 1;
				SchoolLookup.config.firstTime = false;
				SchoolLookup.config.itemFoundInList = true;
				done();
			});

			it('should set the schoolLookup.config correctly', function () {
				spy.reset();
				SchoolLookup.cleanup();

				assert.equal(0, SchoolLookup.config.counter, 'Counter should be zero');
				assert.isTrue(SchoolLookup.config.firstTime, 'firstTime should be true');
				assert.isFalse(SchoolLookup.config.itemFoundInList, 'itemFoundInList should be false');
			});
		});
	});
});
