/*global describe, it, chai, sinon */
define([
	'modules/Sort/SortViews'
], function(views) {

	var assert = chai.assert;

	describe('SortViews', function() {
		/*comment out unit test for handleUserSegmentNavState, because this function no longer exists*/
		/*
		it('should add user segment to nav state', function () {
			var userSegment = 'Endeca_user_segments=School-Harvard';
			var navState = '&view=grid';
			var result = views.handleUserSegmentNavState(navState, userSegment, false);
			assert.equal(result, '?' + userSegment + navState);
		});
		it('should add user segment to nav state - with IE', function () {
			var userSegment = 'Endeca_user_segments=School-Harvard';
			var navState = '?view=grid';
			var navStateWithAmpersand = '&view=grid';
			var result = views.handleUserSegmentNavState(navState, userSegment, true);
			assert.equal(result, '?' + userSegment + navStateWithAmpersand);
		});
		it('should not add user segment to nav state', function () {
			var userSegment = '';
			var navState = 'N-15Z2f?view=grid';
			var result = views.handleUserSegmentNavState(navState, userSegment, false);
			assert.equal(result, navState);
		});*/
	});
});
