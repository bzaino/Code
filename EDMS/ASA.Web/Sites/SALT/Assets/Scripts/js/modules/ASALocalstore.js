define(function () {
	var asaLocalStore = {
		/* Add/update localstore. true if success false on fail.
		Requires key, value, expiration */
		setLocalStorage: function (key, value, expires) {
			if (checkExpireStatus(key) !== 'invalid') {
				setLocalStorage(key, value, expires);
				return true;
			}
			return false;
		},
		getLocalStorage: function (key) {
			// Status
			if (!checkExpireStatus(key)) {
				// Has Data
				return localStorage.getItem(key);
			}
			statusLog('No stored source found');
			return null;
		},
		removeLocalStorage: function (key) {
			removeLocalStorage(key);
		}
	};

	function removeLocalStorage(key) {
		localStorage.removeItem(key);
		localStorage.removeItem(key + '_expires');
	}

	//add store and expiration
	function setLocalStorage(key, value, expires) {
		// default: 30 days
		if (expires === undefined || expires === 'null') {
			expires = 30;
		}
		var date = new Date();
		var schedule = date.setDate(date.getDate() + expires);

		localStorage.setItem(key, value);
		/* separate key instead of combining. Makes expires more reliable */
		localStorage.setItem(key + '_expires', schedule);
	}

	/* returns false if localstorage exists and not expired.
	if expired remove storage and returns true. Otherwise return string "invalid" */
	function checkExpireStatus(key) {
		if (key && key !== '') {
			var current = new Date();
			/* Get Schedule */
			var storedExpiretime = localStorage.getItem(key + '_expires');
			/* Expired */
			if (storedExpiretime && storedExpiretime < current.getTime()) {
				/* Remove */
				removeLocalStorage(key);
				return true;
			}
			/* Storage still valid */
			return false;
		} else {
			/* couldn't validate */
			statusLog('checkExpireStatus error: required parameter "key" is null of empty. The localstore could not be validated.');
		}
		return 'invalid';
	}

	/*basic console log*/
	function statusLog(message) {
		if (console && message) {
			console.log(message);
		}
	}
    return asaLocalStore;
});
