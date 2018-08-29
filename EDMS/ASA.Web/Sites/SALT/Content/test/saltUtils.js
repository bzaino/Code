var assert = require("assert");
var saltUtils = require('./../saltUtils');
var sinon = require('sinon');

describe('#cookieBaker', function() {
	it('should empty object if no cookies object on req.headers', function() {
		var fooRequest = {
			headers: {}
		};
		var returnedCookies = saltUtils.cookieBaker(fooRequest);
		//Use deep equal to test that it is an empty object
		assert.deepEqual({}, returnedCookies);
	});

	it('should return an object with the same amount of properties as cookies', function() {
		//Create foo request with 3 cookies
		var fooRequest = {
			headers: {
				cookie: 'ASP.NET_SessionId=1wgbf1dhhkdph5criq3crwzu; __atuvc=52%7C47; WT_FPC=id=98aef531-07b8-463b-b3eb-db1275838246:lv=1385157436567:ss=1385157399969'
			}
		};
		var returnedCookies = saltUtils.cookieBaker(fooRequest);
		assert.equal(3, Object.keys(returnedCookies).length);
	});

	it('should return an object with keys that are the same name as the cookie names', function() {
		//Create foo request with 3 cookies
		var fooRequest = {
			headers: {
				cookie: 'foo=bar; baz=bing;'
			}
		};
		var returnedCookies = saltUtils.cookieBaker(fooRequest);
		assert.ok(returnedCookies.foo);
		assert.ok(returnedCookies.baz);
	});
});


describe('creating SAL calls', function() {

	describe('#generateCalls', function() {
		it('returned object should have keys that match the object passed in', function() {
			var generatedCalls = saltUtils.generateCalls({}, {
				foo: {},
				baz: {}
			}, {});

			assert.ok(generatedCalls.foo);
			assert.ok(generatedCalls.baz);
		});
	});

	describe('#generateAsyncObj', function() {
		it('should return an object with member and alert as properties, and whatever other service calls were passed in', function() {
			//pass in empty options (1st arg) and a foo service call (2nd arg)
			var generatedCalls = saltUtils.generateAsyncObj({}, {
				baz: {}
			});

			assert.ok(generatedCalls.baz);
			assert.ok(generatedCalls.member);
			assert.ok(typeof generatedCalls.baz === 'function');
			assert.ok(typeof generatedCalls.member === 'function');
		});
	});

	describe('#generateAsyncObjUnauth', function() {
		it('should return an object whatever other service calls were passed in as properties, they should be functions', function() {
			//pass in empty options (1st arg) and a foo service call (2nd arg)
			var generatedCalls = saltUtils.generateAsyncObjUnauth({}, {
				baz: {}
			});

			assert.ok(generatedCalls.baz);
			assert.ok(typeof generatedCalls.baz === 'function');
		});
	});
});
