//     Underscore.js 1.6.0
//     http://underscorejs.org
//     (c) 2009-2014 Jeremy Ashkenas, DocumentCloud and Investigative Reporters & Editors
//     Underscore may be freely distributed under the MIT license.

(function() {

  // Baseline setup
  // --------------

  // Establish the root object, `window` in the browser, or `exports` on the server.
  var root = this;

  // Save the previous value of the `_` variable.
  var previousUnderscore = root._;

  // Establish the object that gets returned to break out of a loop iteration.
  var breaker = {};

  // Save bytes in the minified (but not gzipped) version:
  var ArrayProto = Array.prototype, ObjProto = Object.prototype, FuncProto = Function.prototype;

  // Create quick reference variables for speed access to core prototypes.
  var
    push             = ArrayProto.push,
    slice            = ArrayProto.slice,
    concat           = ArrayProto.concat,
    toString         = ObjProto.toString,
    hasOwnProperty   = ObjProto.hasOwnProperty;

  // All **ECMAScript 5** native function implementations that we hope to use
  // are declared here.
  var
    nativeIsArray      = Array.isArray,
    nativeKeys         = Object.keys,
    nativeBind         = FuncProto.bind;

  // Create a safe reference to the Underscore object for use below.
  var _ = function(obj) {
    if (obj instanceof _) return obj;
    if (!(this instanceof _)) return new _(obj);
    this._wrapped = obj;
  };

  // Export the Underscore object for **Node.js**, with
  // backwards-compatibility for the old `require()` API. If we're in
  // the browser, add `_` as a global object via a string identifier,
  // for Closure Compiler "advanced" mode.
  if (typeof exports !== 'undefined') {
    if (typeof module !== 'undefined' && module.exports) {
      exports = module.exports = _;
    }
    exports._ = _;
  } else {
    root._ = _;
  }

  // Current version.
  _.VERSION = '1.6.0';

  // Internal function: creates a callback bound to its context if supplied
  var createCallback = function(func, context, argCount) {
    if (!context) return func;
    switch (argCount == null ? 3 : argCount) {
      case 1: return function(value) {
        return func.call(context, value);
      };
      case 2: return function(value, other) {
        return func.call(context, value, other);
      };
      case 3: return function(value, index, collection) {
        return func.call(context, value, index, collection);
      };
      case 4: return function(accumulator, value, index, collection) {
        return func.call(context, accumulator, value, index, collection);
      };
    }
    return function() {
      return func.apply(this, arguments);
    };
  };

  // An internal function to generate lookup iterators.
  var lookupIterator = function(value, context, argCount) {
    if (value == null) return _.identity;
    if (_.isFunction(value)) return createCallback(value, context, argCount);
    if (_.isObject(value)) return _.matches(value);
    return _.property(value);
  };

  // Collection Functions
  // --------------------

  // The cornerstone, an `each` implementation, aka `forEach`.
  // Handles raw objects in addition to array-likes. Treats all
  // sparse array-likes as if they were dense.
  _.each = _.forEach = function(obj, iterator, context) {
    var i, length;
    if (obj == null) return obj;
    iterator = createCallback(iterator, context);
    if (obj.length === +obj.length) {
      for (i = 0, length = obj.length; i < length; i++) {
        if (iterator(obj[i], i, obj) === breaker) break;
      }
    } else {
      var keys = _.keys(obj);
      for (i = 0, length = keys.length; i < length; i++) {
        if (iterator(obj[keys[i]], keys[i], obj) === breaker) break;
      }
    }
    return obj;
  };

  // Return the results of applying the iterator to each element.
  _.map = _.collect = function(obj, iterator, context) {
    var results = [];
    if (obj == null) return results;
    iterator = lookupIterator(iterator, context);
    _.each(obj, function(value, index, list) {
      results.push(iterator(value, index, list));
    });
    return results;
  };

  var reduceError = 'Reduce of empty array with no initial value';

  // **Reduce** builds up a single result from a list of values, aka `inject`,
  // or `foldl`.
  _.reduce = _.foldl = _.inject = function(obj, iterator, memo, context) {
    var initial = arguments.length > 2;
    if (obj == null) obj = [];
    iterator = createCallback(iterator, context, 4);
    _.each(obj, function(value, index, list) {
      if (!initial) {
        memo = value;
        initial = true;
      } else {
        memo = iterator(memo, value, index, list);
      }
    });
    if (!initial) throw TypeError(reduceError);
    return memo;
  };

  // The right-associative version of reduce, also known as `foldr`.
  _.reduceRight = _.foldr = function(obj, iterator, memo, context) {
    var initial = arguments.length > 2;
    if (obj == null) obj = [];
    var length = obj.length;
    iterator = createCallback(iterator, context, 4);
    if (length !== +length) {
      var keys = _.keys(obj);
      length = keys.length;
    }
    _.each(obj, function(value, index, list) {
      index = keys ? keys[--length] : --length;
      if (!initial) {
        memo = obj[index];
        initial = true;
      } else {
        memo = iterator(memo, obj[index], index, list);
      }
    });
    if (!initial) throw TypeError(reduceError);
    return memo;
  };

  // Return the first value which passes a truth test. Aliased as `detect`.
  _.find = _.detect = function(obj, predicate, context) {
    var result;
    predicate = lookupIterator(predicate, context);
    _.some(obj, function(value, index, list) {
      if (predicate(value, index, list)) {
        result = value;
        return true;
      }
    });
    return result;
  };

  // Return all the elements that pass a truth test.
  // Aliased as `select`.
  _.filter = _.select = function(obj, predicate, context) {
    var results = [];
    if (obj == null) return results;
    predicate = lookupIterator(predicate, context);
    _.each(obj, function(value, index, list) {
      if (predicate(value, index, list)) results.push(value);
    });
    return results;
  };

  // Return all the elements for which a truth test fails.
  _.reject = function(obj, predicate, context) {
    return _.filter(obj, _.negate(lookupIterator(predicate)), context);
  };

  // Determine whether all of the elements match a truth test.
  // Aliased as `all`.
  _.every = _.all = function(obj, predicate, context) {
    var result = true;
    if (obj == null) return result;
    predicate = lookupIterator(predicate, context);
    _.each(obj, function(value, index, list) {
      result = predicate(value, index, list);
      if (!result) return breaker;
    });
    return !!result;
  };

  // Determine if at least one element in the object matches a truth test.
  // Aliased as `any`.
  _.some = _.any = function(obj, predicate, context) {
    var result = false;
    if (obj == null) return result;
    predicate = lookupIterator(predicate, context);
    _.each(obj, function(value, index, list) {
      result = predicate(value, index, list);
      if (result) return breaker;
    });
    return !!result;
  };

  // Determine if the array or object contains a given value (using `===`).
  // Aliased as `include`.
  _.contains = _.include = function(obj, target) {
    if (obj == null) return false;
    if (obj.length === +obj.length) return _.indexOf(obj, target) >= 0;
    return _.some(obj, function(value) {
      return value === target;
    });
  };

  // Invoke a method (with arguments) on every item in a collection.
  _.invoke = function(obj, method) {
    var args = slice.call(arguments, 2);
    var isFunc = _.isFunction(method);
    return _.map(obj, function(value) {
      return (isFunc ? method : value[method]).apply(value, args);
    });
  };

  // Convenience version of a common use case of `map`: fetching a property.
  _.pluck = function(obj, key) {
    return _.map(obj, _.property(key));
  };

  // Convenience version of a common use case of `filter`: selecting only objects
  // containing specific `key:value` pairs.
  _.where = function(obj, attrs) {
    return _.filter(obj, _.matches(attrs));
  };

  // Convenience version of a common use case of `find`: getting the first object
  // containing specific `key:value` pairs.
  _.findWhere = function(obj, attrs) {
    return _.find(obj, _.matches(attrs));
  };

  // Return the maximum element or (element-based computation).
  _.max = function(obj, iterator, context) {
    var result = -Infinity, lastComputed = -Infinity,
        value, computed;
    if (!iterator && _.isArray(obj)) {
      for (var i = 0, length = obj.length; i < length; i++) {
        value = obj[i];
        if (value > result) {
          result = value;
        }
      }
    } else {
      iterator = lookupIterator(iterator, context);
      _.each(obj, function(value, index, list) {
        computed = iterator ? iterator(value, index, list) : value;
        if (computed > lastComputed || computed === -Infinity && result === -Infinity) {
          result = value;
          lastComputed = computed;
        }
      });
    }
    return result;
  };

  // Return the minimum element (or element-based computation).
  _.min = function(obj, iterator, context) {
    var result = Infinity, lastComputed = Infinity,
        value, computed;
    if (!iterator && _.isArray(obj)) {
      for (var i = 0, length = obj.length; i < length; i++) {
        value = obj[i];
        if (value < result) {
          result = value;
        }
      }
    } else {
      iterator = lookupIterator(iterator, context);
      _.each(obj, function(value, index, list) {
        computed = iterator ? iterator(value, index, list) : value;
        if (computed < lastComputed || computed === Infinity && result === Infinity) {
          result = value;
          lastComputed = computed;
        }
      });
    }
    return result;
  };

  // Shuffle an array, using the modern version of the
  // [Fisher-Yates shuffle](http://en.wikipedia.org/wiki/Fisher–Yates_shuffle).
  _.shuffle = function(obj) {
    var rand;
    var index = 0;
    var shuffled = [];
    _.each(obj, function(value) {
      rand = _.random(index++);
      shuffled[index - 1] = shuffled[rand];
      shuffled[rand] = value;
    });
    return shuffled;
  };

  // Sample **n** random values from a collection.
  // If **n** is not specified, returns a single random element.
  // The internal `guard` argument allows it to work with `map`.
  _.sample = function(obj, n, guard) {
    if (n == null || guard) {
      if (obj.length !== +obj.length) obj = _.values(obj);
      return obj[_.random(obj.length - 1)];
    }
    return _.shuffle(obj).slice(0, Math.max(0, n));
  };

  // Sort the object's values by a criterion produced by an iterator.
  _.sortBy = function(obj, iterator, context) {
    iterator = lookupIterator(iterator, context);
    return _.pluck(_.map(obj, function(value, index, list) {
      return {
        value: value,
        index: index,
        criteria: iterator(value, index, list)
      };
    }).sort(function(left, right) {
      var a = left.criteria;
      var b = right.criteria;
      if (a !== b) {
        if (a > b || a === void 0) return 1;
        if (a < b || b === void 0) return -1;
      }
      return left.index - right.index;
    }), 'value');
  };

  // An internal function used for aggregate "group by" operations.
  var group = function(behavior) {
    return function(obj, iterator, context) {
      var result = {};
      iterator = lookupIterator(iterator, context);
      _.each(obj, function(value, index) {
        var key = iterator(value, index, obj);
        behavior(result, value, key);
      });
      return result;
    };
  };

  // Groups the object's values by a criterion. Pass either a string attribute
  // to group by, or a function that returns the criterion.
  _.groupBy = group(function(result, value, key) {
    if (_.has(result, key)) result[key].push(value); else result[key] = [value];
  });

  // Indexes the object's values by a criterion, similar to `groupBy`, but for
  // when you know that your index values will be unique.
  _.indexBy = group(function(result, value, key) {
    result[key] = value;
  });

  // Counts instances of an object that group by a certain criterion. Pass
  // either a string attribute to count by, or a function that returns the
  // criterion.
  _.countBy = group(function(result, value, key) {
    if (_.has(result, key)) result[key]++; else result[key] = 1;
  });

  // Use a comparator function to figure out the smallest index at which
  // an object should be inserted so as to maintain order. Uses binary search.
  _.sortedIndex = function(array, obj, iterator, context) {
    iterator = lookupIterator(iterator, context, 1);
    var value = iterator(obj);
    var low = 0, high = array.length;
    while (low < high) {
      var mid = (low + high) >>> 1;
      if (iterator(array[mid]) < value) low = mid + 1; else high = mid;
    }
    return low;
  };

  // Safely create a real, live array from anything iterable.
  _.toArray = function(obj) {
    if (!obj) return [];
    if (_.isArray(obj)) return slice.call(obj);
    if (obj.length === +obj.length) return _.map(obj, _.identity);
    return _.values(obj);
  };

  // Return the number of elements in an object.
  _.size = function(obj) {
    if (obj == null) return 0;
    return obj.length === +obj.length ? obj.length : _.keys(obj).length;
  };

  // Array Functions
  // ---------------

  // Get the first element of an array. Passing **n** will return the first N
  // values in the array. Aliased as `head` and `take`. The **guard** check
  // allows it to work with `_.map`.
  _.first = _.head = _.take = function(array, n, guard) {
    if (array == null) return void 0;
    if (n == null || guard) return array[0];
    if (n < 0) return [];
    return slice.call(array, 0, n);
  };

  // Returns everything but the last entry of the array. Especially useful on
  // the arguments object. Passing **n** will return all the values in
  // the array, excluding the last N. The **guard** check allows it to work with
  // `_.map`.
  _.initial = function(array, n, guard) {
    return slice.call(array, 0, Math.max(0, array.length - (n == null || guard ? 1 : n)));
  };

  // Get the last element of an array. Passing **n** will return the last N
  // values in the array. The **guard** check allows it to work with `_.map`.
  _.last = function(array, n, guard) {
    if (array == null) return void 0;
    if (n == null || guard) return array[array.length - 1];
    return slice.call(array, Math.max(array.length - n, 0));
  };

  // Returns everything but the first entry of the array. Aliased as `tail` and `drop`.
  // Especially useful on the arguments object. Passing an **n** will return
  // the rest N values in the array. The **guard**
  // check allows it to work with `_.map`.
  _.rest = _.tail = _.drop = function(array, n, guard) {
    return slice.call(array, n == null || guard ? 1 : n);
  };

  // Trim out all falsy values from an array.
  _.compact = function(array) {
    return _.filter(array, _.identity);
  };

  // Internal implementation of a recursive `flatten` function.
  var flatten = function(input, shallow, strict, output) {
    if (shallow && _.every(input, _.isArray)) {
      return concat.apply(output, input);
    }
    for (var i = 0, length = input.length; i < length; i++) {
      var value = input[i];
      if (!_.isArray(value) && !_.isArguments(value)) {
        if (!strict) output.push(value);
      } else if (shallow) {
        push.apply(output, value);
      } else {
        flatten(value, shallow, strict, output);
      }
    }
    return output;
  };

  // Flatten out an array, either recursively (by default), or just one level.
  _.flatten = function(array, shallow) {
    return flatten(array, shallow, false, []);
  };

  // Return a version of the array that does not contain the specified value(s).
  _.without = function(array) {
    return _.difference(array, slice.call(arguments, 1));
  };

  // Split an array into two arrays: one whose elements all satisfy the given
  // predicate, and one whose elements all do not satisfy the predicate.
  _.partition = function(obj, predicate, context) {
    predicate = lookupIterator(predicate, context, 1);
    var pass = [], fail = [];
    _.each(obj, function(value, key, obj) {
      (predicate(value, key, obj) ? pass : fail).push(value);
    });
    return [pass, fail];
  };

  // Produce a duplicate-free version of the array. If the array has already
  // been sorted, you have the option of using a faster algorithm.
  // Aliased as `unique`.
  _.uniq = _.unique = function(array, isSorted, iterator, context) {
    if (array == null) return [];
    if (_.isFunction(isSorted)) {
      context = iterator;
      iterator = isSorted;
      isSorted = false;
    }
    if (iterator) iterator = lookupIterator(iterator, context);
    var result = [];
    var seen = [];
    for (var i = 0, length = array.length; i < length; i++) {
      var value = array[i];
      if (iterator) value = iterator(value, i, array);
      if (isSorted ? !i || seen !== value : !_.contains(seen, value)) {
        if (isSorted) seen = value;
        else seen.push(value);
        result.push(array[i]);
      }
    }
    return result;
  };

  // Produce an array that contains the union: each distinct element from all of
  // the passed-in arrays.
  _.union = function() {
    return _.uniq(flatten(arguments, true, true, []));
  };

  // Produce an array that contains every item shared between all the
  // passed-in arrays.
  _.intersection = function(array) {
    if (array == null) return [];
    var result = [];
    var argsLength = arguments.length;
    for (var i = 0, length = array.length; i < length; i++) {
      var item = array[i];
      if (_.contains(result, item)) continue;
      for (var j = 1; j < argsLength; j++) {
        if (!_.contains(arguments[j], item)) break;
      }
      if (j === argsLength) result.push(item);
    }
    return result;
  };

  // Take the difference between one array and a number of other arrays.
  // Only the elements present in just the first array will remain.
  _.difference = function(array) {
    var rest = flatten(slice.call(arguments, 1), true, true, []);
    return _.filter(array, function(value){
      return !_.contains(rest, value);
    });
  };

  // Zip together multiple lists into a single array -- elements that share
  // an index go together.
  _.zip = function() {
    var length = _.max(_.pluck(arguments, 'length').concat(0));
    var results = Array(length);
    for (var i = 0; i < length; i++) {
      results[i] = _.pluck(arguments, '' + i);
    }
    return results;
  };

  // Converts lists into objects. Pass either a single array of `[key, value]`
  // pairs, or two parallel arrays of the same length -- one of keys, and one of
  // the corresponding values.
  _.object = function(list, values) {
    if (list == null) return {};
    var result = {};
    for (var i = 0, length = list.length; i < length; i++) {
      if (values) {
        result[list[i]] = values[i];
      } else {
        result[list[i][0]] = list[i][1];
      }
    }
    return result;
  };

  // Return the position of the first occurrence of an item in an array,
  // or -1 if the item is not included in the array.
  // If the array is large and already in sort order, pass `true`
  // for **isSorted** to use binary search.
  _.indexOf = function(array, item, isSorted) {
    if (array == null) return -1;
    var i = 0, length = array.length;
    if (isSorted) {
      if (typeof isSorted == 'number') {
        i = isSorted < 0 ? Math.max(0, length + isSorted) : isSorted;
      } else {
        i = _.sortedIndex(array, item);
        return array[i] === item ? i : -1;
      }
    }
    for (; i < length; i++) if (array[i] === item) return i;
    return -1;
  };

  _.lastIndexOf = function(array, item, from) {
    if (array == null) return -1;
    var i = from == null ? array.length : from;
    while (i--) if (array[i] === item) return i;
    return -1;
  };

  // Generate an integer Array containing an arithmetic progression. A port of
  // the native Python `range()` function. See
  // [the Python documentation](http://docs.python.org/library/functions.html#range).
  _.range = function(start, stop, step) {
    if (arguments.length <= 1) {
      stop = start || 0;
      start = 0;
    }
    step = arguments[2] || 1;

    var length = Math.max(Math.ceil((stop - start) / step), 0);
    var idx = 0;
    var range = Array(length);

    while (idx < length) {
      range[idx++] = start;
      start += step;
    }

    return range;
  };

  // Function (ahem) Functions
  // ------------------

  // Reusable constructor function for prototype setting.
  var Ctor = function(){};

  // Create a function bound to a given object (assigning `this`, and arguments,
  // optionally). Delegates to **ECMAScript 5**'s native `Function.bind` if
  // available.
  _.bind = function(func, context) {
    var args, bound;
    if (nativeBind && func.bind === nativeBind) return nativeBind.apply(func, slice.call(arguments, 1));
    if (!_.isFunction(func)) throw TypeError('Bind must be called on a function');
    args = slice.call(arguments, 2);
    bound = function() {
      if (!(this instanceof bound)) return func.apply(context, args.concat(slice.call(arguments)));
      Ctor.prototype = func.prototype;
      var self = new Ctor;
      Ctor.prototype = null;
      var result = func.apply(self, args.concat(slice.call(arguments)));
      if (Object(result) === result) return result;
      return self;
    };
    return bound;
  };

  // Partially apply a function by creating a version that has had some of its
  // arguments pre-filled, without changing its dynamic `this` context. _ acts
  // as a placeholder, allowing any combination of arguments to be pre-filled.
  _.partial = function(func) {
    var boundArgs = slice.call(arguments, 1);
    return function() {
      var position = 0;
      var args = boundArgs.slice();
      for (var i = 0, length = args.length; i < length; i++) {
        if (args[i] === _) args[i] = arguments[position++];
      }
      while (position < arguments.length) args.push(arguments[position++]);
      return func.apply(this, args);
    };
  };

  // Bind a number of an object's methods to that object. Remaining arguments
  // are the method names to be bound. Useful for ensuring that all callbacks
  // defined on an object belong to it.
  _.bindAll = function(obj) {
    var funcs = slice.call(arguments, 1);
    if (funcs.length === 0) throw Error('bindAll must be passed function names');
    _.each(funcs, function(f) {
      obj[f] = _.bind(obj[f], obj);
    });
    return obj;
  };

  // Memoize an expensive function by storing its results.
  _.memoize = function(func, hasher) {
    if (!hasher) hasher = _.identity;
    var memoize = function() {
      var cache = memoize.cache;
      var key = hasher.apply(this, arguments);
      if (!_.has(cache, key)) cache[key] = func.apply(this, arguments);
      return cache[key];
    };
    memoize.cache = {};
    return memoize;
  };

  // Delays a function for the given number of milliseconds, and then calls
  // it with the arguments supplied.
  _.delay = function(func, wait) {
    var args = slice.call(arguments, 2);
    return setTimeout(function(){
      return func.apply(null, args);
    }, wait);
  };

  // Defers a function, scheduling it to run after the current call stack has
  // cleared.
  _.defer = function(func) {
    return _.delay.apply(_, [func, 1].concat(slice.call(arguments, 1)));
  };

  // Returns a function, that, when invoked, will only be triggered at most once
  // during a given window of time. Normally, the throttled function will run
  // as much as it can, without ever going more than once per `wait` duration;
  // but if you'd like to disable the execution on the leading edge, pass
  // `{leading: false}`. To disable execution on the trailing edge, ditto.
  _.throttle = function(func, wait, options) {
    var context, args, result;
    var timeout = null;
    var previous = 0;
    if (!options) options = {};
    var later = function() {
      previous = options.leading === false ? 0 : _.now();
      timeout = null;
      result = func.apply(context, args);
      if (!timeout) context = args = null;
    };
    return function() {
      var now = _.now();
      if (!previous && options.leading === false) previous = now;
      var remaining = wait - (now - previous);
      context = this;
      args = arguments;
      if (remaining <= 0 || remaining > wait) {
        clearTimeout(timeout);
        timeout = null;
        previous = now;
        result = func.apply(context, args);
        if (!timeout) context = args = null;
      } else if (!timeout && options.trailing !== false) {
        timeout = setTimeout(later, remaining);
      }
      return result;
    };
  };

  // Returns a function, that, as long as it continues to be invoked, will not
  // be triggered. The function will be called after it stops being called for
  // N milliseconds. If `immediate` is passed, trigger the function on the
  // leading edge, instead of the trailing.
  _.debounce = function(func, wait, immediate) {
    var timeout, args, context, timestamp, result;

    var later = function() {
      var last = _.now() - timestamp;

      if (last < wait && last > 0) {
        timeout = setTimeout(later, wait - last);
      } else {
        timeout = null;
        if (!immediate) {
          result = func.apply(context, args);
          if (!timeout) context = args = null;
        }
      }
    };

    return function() {
      context = this;
      args = arguments;
      timestamp = _.now();
      var callNow = immediate && !timeout;
      if (!timeout) timeout = setTimeout(later, wait);
      if (callNow) {
        result = func.apply(context, args);
        context = args = null;
      }

      return result;
    };
  };

  // Returns a function that will be executed at most one time, no matter how
  // often you call it. Useful for lazy initialization.
  _.once = function(func) {
    var ran = false, memo;
    return function() {
      if (ran) return memo;
      ran = true;
      memo = func.apply(this, arguments);
      func = null;
      return memo;
    };
  };

  // Returns the first function passed as an argument to the second,
  // allowing you to adjust arguments, run code before and after, and
  // conditionally execute the original function.
  _.wrap = function(func, wrapper) {
    return _.partial(wrapper, func);
  };

  // Returns a negated version of the passed-in predicate.
  _.negate = function(predicate) {
    return function() {
      return !predicate.apply(this, arguments);
    };
  };

  // Returns a function that is the composition of a list of functions, each
  // consuming the return value of the function that follows.
  _.compose = function() {
    var funcs = arguments;
    return function() {
      var args = arguments;
      for (var i = funcs.length - 1; i >= 0; i--) {
        args = [funcs[i].apply(this, args)];
      }
      return args[0];
    };
  };

  // Returns a function that will only be executed after being called N times.
  _.after = function(times, func) {
    return function() {
      if (--times < 1) {
        return func.apply(this, arguments);
      }
    };
  };

  // Object Functions
  // ----------------

  // Retrieve the names of an object's properties.
  // Delegates to **ECMAScript 5**'s native `Object.keys`
  _.keys = function(obj) {
    if (!_.isObject(obj)) return [];
    if (nativeKeys) return nativeKeys(obj);
    var keys = [];
    for (var key in obj) if (_.has(obj, key)) keys.push(key);
    return keys;
  };

  // Retrieve the values of an object's properties.
  _.values = function(obj) {
    var keys = _.keys(obj);
    var length = keys.length;
    var values = Array(length);
    for (var i = 0; i < length; i++) {
      values[i] = obj[keys[i]];
    }
    return values;
  };

  // Convert an object into a list of `[key, value]` pairs.
  _.pairs = function(obj) {
    var keys = _.keys(obj);
    var length = keys.length;
    var pairs = Array(length);
    for (var i = 0; i < length; i++) {
      pairs[i] = [keys[i], obj[keys[i]]];
    }
    return pairs;
  };

  // Invert the keys and values of an object. The values must be serializable.
  _.invert = function(obj) {
    var result = {};
    var keys = _.keys(obj);
    for (var i = 0, length = keys.length; i < length; i++) {
      result[obj[keys[i]]] = keys[i];
    }
    return result;
  };

  // Return a sorted list of the function names available on the object.
  // Aliased as `methods`
  _.functions = _.methods = function(obj) {
    var names = [];
    for (var key in obj) {
      if (_.isFunction(obj[key])) names.push(key);
    }
    return names.sort();
  };

  // Extend a given object with all the properties in passed-in object(s).
  _.extend = function(obj) {
    if (!_.isObject(obj)) return obj;
    _.each(slice.call(arguments, 1), function(source) {
      for (var prop in source) {
        obj[prop] = source[prop];
      }
    });
    return obj;
  };

  // Return a copy of the object only containing the whitelisted properties.
  _.pick = function(obj, iterator, context) {
    var result = {}, key;
    if (_.isFunction(iterator)) {
      for (key in obj) {
        var value = obj[key];
        if (iterator.call(context, value, key, obj)) result[key] = value;
      }
    } else {
      var keys = concat.apply([], slice.call(arguments, 1));
      for (var i = 0, length = keys.length; i < length; i++) {
        key = keys[i];
        if (key in obj) result[key] = obj[key];
      }
    }
    return result;
  };

   // Return a copy of the object without the blacklisted properties.
  _.omit = function(obj, iterator, context) {
    var keys;
    if (_.isFunction(iterator)) {
      iterator = _.negate(iterator);
    } else {
      keys = _.map(concat.apply([], slice.call(arguments, 1)), String);
      iterator = function(value, key) {
        return !_.contains(keys, key);
      };
    }
    return _.pick(obj, iterator, context);
  };

  // Fill in a given object with default properties.
  _.defaults = function(obj) {
    if (!_.isObject(obj)) return obj;
    _.each(slice.call(arguments, 1), function(source) {
      for (var prop in source) {
        if (obj[prop] === void 0) obj[prop] = source[prop];
      }
    });
    return obj;
  };

  // Create a (shallow-cloned) duplicate of an object.
  _.clone = function(obj) {
    if (!_.isObject(obj)) return obj;
    return _.isArray(obj) ? obj.slice() : _.extend({}, obj);
  };

  // Invokes interceptor with the obj, and then returns obj.
  // The primary purpose of this method is to "tap into" a method chain, in
  // order to perform operations on intermediate results within the chain.
  _.tap = function(obj, interceptor) {
    interceptor(obj);
    return obj;
  };

  // Internal recursive comparison function for `isEqual`.
  var eq = function(a, b, aStack, bStack) {
    // Identical objects are equal. `0 === -0`, but they aren't identical.
    // See the [Harmony `egal` proposal](http://wiki.ecmascript.org/doku.php?id=harmony:egal).
    if (a === b) return a !== 0 || 1 / a === 1 / b;
    // A strict comparison is necessary because `null == undefined`.
    if (a == null || b == null) return a === b;
    // Unwrap any wrapped objects.
    if (a instanceof _) a = a._wrapped;
    if (b instanceof _) b = b._wrapped;
    // Compare `[[Class]]` names.
    var className = toString.call(a);
    if (className !== toString.call(b)) return false;
    switch (className) {
      // RegExps are coerced to strings for comparison.
      case '[object RegExp]':
      // Strings, numbers, dates, and booleans are compared by value.
      case '[object String]':
        // Primitives and their corresponding object wrappers are equivalent; thus, `"5"` is
        // equivalent to `String("5")`.
        return '' + a === '' + b;
      case '[object Number]':
        // `NaN`s are equivalent, but non-reflexive.
        if (a != +a) return b != +b;
        // An `egal` comparison is performed for other numeric values.
        return a == 0 ? 1 / a == 1 / b : a == +b;
      case '[object Date]':
      case '[object Boolean]':
        // Coerce dates and booleans to numeric primitive values. Dates are compared by their
        // millisecond representations. Note that invalid dates with millisecond representations
        // of `NaN` are not equivalent.
        return +a === +b;
    }
    if (typeof a != 'object' || typeof b != 'object') return false;
    // Assume equality for cyclic structures. The algorithm for detecting cyclic
    // structures is adapted from ES 5.1 section 15.12.3, abstract operation `JO`.
    var length = aStack.length;
    while (length--) {
      // Linear search. Performance is inversely proportional to the number of
      // unique nested structures.
      if (aStack[length] === a) return bStack[length] === b;
    }
    // Objects with different constructors are not equivalent, but `Object`s
    // from different frames are.
    var aCtor = a.constructor, bCtor = b.constructor;
    if (
      aCtor !== bCtor && 'constructor' in a && 'constructor' in b &&
      !(_.isFunction(aCtor) && aCtor instanceof aCtor &&
        _.isFunction(bCtor) && bCtor instanceof bCtor)
    ) {
      return false;
    }
    // Add the first object to the stack of traversed objects.
    aStack.push(a);
    bStack.push(b);
    var size = 0, result = true;
    // Recursively compare objects and arrays.
    if (className === '[object Array]') {
      // Compare array lengths to determine if a deep comparison is necessary.
      size = a.length;
      result = size === b.length;
      if (result) {
        // Deep compare the contents, ignoring non-numeric properties.
        while (size--) {
          if (!(result = eq(a[size], b[size], aStack, bStack))) break;
        }
      }
    } else {
      // Deep compare objects.
      for (var key in a) {
        if (_.has(a, key)) {
          // Count the expected number of properties.
          size++;
          // Deep compare each member.
          if (!(result = _.has(b, key) && eq(a[key], b[key], aStack, bStack))) break;
        }
      }
      // Ensure that both objects contain the same number of properties.
      if (result) {
        for (key in b) {
          if (_.has(b, key) && !size--) break;
        }
        result = !size;
      }
    }
    // Remove the first object from the stack of traversed objects.
    aStack.pop();
    bStack.pop();
    return result;
  };

  // Perform a deep comparison to check if two objects are equal.
  _.isEqual = function(a, b) {
    return eq(a, b, [], []);
  };

  // Is a given array, string, or object empty?
  // An "empty" object has no enumerable own-properties.
  _.isEmpty = function(obj) {
    if (obj == null) return true;
    if (_.isArray(obj) || _.isString(obj) || _.isArguments(obj)) return obj.length === 0;
    for (var key in obj) if (_.has(obj, key)) return false;
    return true;
  };

  // Is a given value a DOM element?
  _.isElement = function(obj) {
    return !!(obj && obj.nodeType === 1);
  };

  // Is a given value an array?
  // Delegates to ECMA5's native Array.isArray
  _.isArray = nativeIsArray || function(obj) {
    return toString.call(obj) === '[object Array]';
  };

  // Is a given variable an object?
  _.isObject = function(obj) {
    return obj === Object(obj);
  };

  // Add some isType methods: isArguments, isFunction, isString, isNumber, isDate, isRegExp.
  _.each(['Arguments', 'Function', 'String', 'Number', 'Date', 'RegExp'], function(name) {
    _['is' + name] = function(obj) {
      return toString.call(obj) === '[object ' + name + ']';
    };
  });

  // Define a fallback version of the method in browsers (ahem, IE), where
  // there isn't any inspectable "Arguments" type.
  if (!_.isArguments(arguments)) {
    _.isArguments = function(obj) {
      return _.has(obj, 'callee');
    };
  }

  // Optimize `isFunction` if appropriate.
  if (typeof /./ !== 'function') {
    _.isFunction = function(obj) {
      return typeof obj === 'function';
    };
  }

  // Is a given object a finite number?
  _.isFinite = function(obj) {
    return isFinite(obj) && !isNaN(parseFloat(obj));
  };

  // Is the given value `NaN`? (NaN is the only number which does not equal itself).
  _.isNaN = function(obj) {
    return _.isNumber(obj) && obj !== +obj;
  };

  // Is a given value a boolean?
  _.isBoolean = function(obj) {
    return obj === true || obj === false || toString.call(obj) === '[object Boolean]';
  };

  // Is a given value equal to null?
  _.isNull = function(obj) {
    return obj === null;
  };

  // Is a given variable undefined?
  _.isUndefined = function(obj) {
    return obj === void 0;
  };

  // Shortcut function for checking if an object has a given property directly
  // on itself (in other words, not on a prototype).
  _.has = function(obj, key) {
    return obj != null && hasOwnProperty.call(obj, key);
  };

  // Utility Functions
  // -----------------

  // Run Underscore.js in *noConflict* mode, returning the `_` variable to its
  // previous owner. Returns a reference to the Underscore object.
  _.noConflict = function() {
    root._ = previousUnderscore;
    return this;
  };

  // Keep the identity function around for default iterators.
  _.identity = function(value) {
    return value;
  };

  _.constant = function(value) {
    return function() {
      return value;
    };
  };

  _.noop = function(){};

  _.property = function(key) {
    return function(obj) {
      return obj[key];
    };
  };

  // Returns a predicate for checking whether an object has a given set of `key:value` pairs.
  _.matches = function(attrs) {
    return function(obj) {
      if (obj == null) return _.isEmpty(attrs);
      if (obj === attrs) return true;
      for (var key in attrs) if (attrs[key] !== obj[key]) return false;
      return true;
    };
  };

  // Run a function **n** times.
  _.times = function(n, iterator, context) {
    var accum = Array(Math.max(0, n));
    iterator = createCallback(iterator, context, 1);
    for (var i = 0; i < n; i++) accum[i] = iterator(i);
    return accum;
  };

  // Return a random integer between min and max (inclusive).
  _.random = function(min, max) {
    if (max == null) {
      max = min;
      min = 0;
    }
    return min + Math.floor(Math.random() * (max - min + 1));
  };

  // A (possibly faster) way to get the current timestamp as an integer.
  _.now = Date.now || function() {
    return new Date().getTime();
  };

  // List of HTML entities for escaping.
  var entityMap = {
    escape: {
      '&': '&amp;',
      '<': '&lt;',
      '>': '&gt;',
      '"': '&quot;',
      "'": '&#x27;'
    }
  };
  entityMap.unescape = _.invert(entityMap.escape);

  // Regexes containing the keys and values listed immediately above.
  var entityRegexes = {
    escape:   RegExp('[' + _.keys(entityMap.escape).join('') + ']', 'g'),
    unescape: RegExp('(' + _.keys(entityMap.unescape).join('|') + ')', 'g')
  };

  // Functions for escaping and unescaping strings to/from HTML interpolation.
  _.each(['escape', 'unescape'], function(method) {
    _[method] = function(string) {
      if (string == null) return '';
      return ('' + string).replace(entityRegexes[method], function(match) {
        return entityMap[method][match];
      });
    };
  });

  // If the value of the named `property` is a function then invoke it with the
  // `object` as context; otherwise, return it.
  _.result = function(object, property) {
    if (object == null) return void 0;
    var value = object[property];
    return _.isFunction(value) ? object[property]() : value;
  };

  // Generate a unique integer id (unique within the entire client session).
  // Useful for temporary DOM ids.
  var idCounter = 0;
  _.uniqueId = function(prefix) {
    var id = ++idCounter + '';
    return prefix ? prefix + id : id;
  };

  // By default, Underscore uses ERB-style template delimiters, change the
  // following template settings to use alternative delimiters.
  _.templateSettings = {
    evaluate    : /<%([\s\S]+?)%>/g,
    interpolate : /<%=([\s\S]+?)%>/g,
    escape      : /<%-([\s\S]+?)%>/g
  };

  // When customizing `templateSettings`, if you don't want to define an
  // interpolation, evaluation or escaping regex, we need one that is
  // guaranteed not to match.
  var noMatch = /(.)^/;

  // Certain characters need to be escaped so that they can be put into a
  // string literal.
  var escapes = {
    "'":      "'",
    '\\':     '\\',
    '\r':     'r',
    '\n':     'n',
    '\u2028': 'u2028',
    '\u2029': 'u2029'
  };

  var escaper = /\\|'|\r|\n|\u2028|\u2029/g;

  var escapeChar = function(match) {
    return '\\' + escapes[match];
  };

  // JavaScript micro-templating, similar to John Resig's implementation.
  // Underscore templating handles arbitrary delimiters, preserves whitespace,
  // and correctly escapes quotes within interpolated code.
  _.template = function(text, data, settings) {
    settings = _.defaults({}, settings, _.templateSettings);

    // Combine delimiters into one regular expression via alternation.
    var matcher = RegExp([
      (settings.escape || noMatch).source,
      (settings.interpolate || noMatch).source,
      (settings.evaluate || noMatch).source
    ].join('|') + '|$', 'g');

    // Compile the template source, escaping string literals appropriately.
    var index = 0;
    var source = "__p+='";
    text.replace(matcher, function(match, escape, interpolate, evaluate, offset) {
      source += text.slice(index, offset).replace(escaper, escapeChar);
      index = offset + match.length;

      if (escape) {
        source += "'+\n((__t=(" + escape + "))==null?'':_.escape(__t))+\n'";
      } else if (interpolate) {
        source += "'+\n((__t=(" + interpolate + "))==null?'':__t)+\n'";
      } else if (evaluate) {
        source += "';\n" + evaluate + "\n__p+='";
      }

      // Adobe VMs need the match returned to produce the correct offest.
      return match;
    });
    source += "';\n";

    // If a variable is not specified, place data values in local scope.
    if (!settings.variable) source = 'with(obj||{}){\n' + source + '}\n';

    source = "var __t,__p='',__j=Array.prototype.join," +
      "print=function(){__p+=__j.call(arguments,'');};\n" +
      source + 'return __p;\n';

    try {
      var render = Function(settings.variable || 'obj', '_', source);
    } catch (e) {
      e.source = source;
      throw e;
    }

    if (data) return render(data, _);
    var template = function(data) {
      return render.call(this, data, _);
    };

    // Provide the compiled source as a convenience for precompilation.
    var argument = settings.variable || 'obj';
    template.source = 'function(' + argument + '){\n' + source + '}';

    return template;
  };

  // Add a "chain" function, which will delegate to the wrapper.
  _.chain = function(obj) {
    return _(obj).chain();
  };

  // OOP
  // ---------------
  // If Underscore is called as a function, it returns a wrapped object that
  // can be used OO-style. This wrapper holds altered versions of all the
  // underscore functions. Wrapped objects may be chained.

  // Helper function to continue chaining intermediate results.
  var result = function(obj) {
    return this._chain ? _(obj).chain() : obj;
  };

  // Add your own custom functions to the Underscore object.
  _.mixin = function(obj) {
    _.each(_.functions(obj), function(name) {
      var func = _[name] = obj[name];
      _.prototype[name] = function() {
        var args = [this._wrapped];
        push.apply(args, arguments);
        return result.call(this, func.apply(_, args));
      };
    });
  };

  // Add all of the Underscore functions to the wrapper object.
  _.mixin(_);

  // Add all mutator Array functions to the wrapper.
  _.each(['pop', 'push', 'reverse', 'shift', 'sort', 'splice', 'unshift'], function(name) {
    var method = ArrayProto[name];
    _.prototype[name] = function() {
      var obj = this._wrapped;
      method.apply(obj, arguments);
      if ((name === 'shift' || name === 'splice') && obj.length === 0) delete obj[0];
      return result.call(this, obj);
    };
  });

  // Add all accessor Array functions to the wrapper.
  _.each(['concat', 'join', 'slice'], function(name) {
    var method = ArrayProto[name];
    _.prototype[name] = function() {
      return result.call(this, method.apply(this._wrapped, arguments));
    };
  });

  _.extend(_.prototype, {

    // Start chaining a wrapped Underscore object.
    chain: function() {
      this._chain = true;
      return this;
    },

    // Extracts the result from a wrapped and chained object.
    value: function() {
      return this._wrapped;
    }

  });

  // AMD registration happens at the end for compatibility with AMD loaders
  // that may not enforce next-turn semantics on modules. Even though general
  // practice for AMD registration is to be anonymous, underscore registers
  // as a named module because, like jQuery, it is a base library that is
  // popular enough to be bundled in a third party lib, but not be part of
  // an AMD load request. Those cases could generate an error when an
  // anonymous define() is called outside of a loader request.
  if (typeof define === 'function' && define.amd) {
    define('underscore', [], function() {
      return _;
    });
  }
}.call(this));


/*!
 * jQuery JavaScript Library v2.0.3
 * http://jquery.com/
 *
 * Includes Sizzle.js
 * http://sizzlejs.com/
 *
 * Copyright 2005, 2013 jQuery Foundation, Inc. and other contributors
 * Released under the MIT license
 * http://jquery.org/license
 *
 * Date: 2013-07-03T13:30Z
 */ (function(window, undefined) {

    // Can't do this because several apps including ASP.NET trace
    // the stack via arguments.caller.callee and Firefox dies if
    // you try to trace through "use strict" call chains. (#13335)
    // Support: Firefox 18+
    //
    var
    // A central reference to the root jQuery(document)
    rootjQuery,

    // The deferred used on DOM ready
    readyList,

    // Support: IE9
    // For `typeof xmlNode.method` instead of `xmlNode.method !== undefined`
    core_strundefined = typeof undefined,

        // Use the correct document accordingly with window argument (sandbox)
        location = window.location,
        document = window.document,
        docElem = document.documentElement,

        // Map over jQuery in case of overwrite
        _jQuery = window.jQuery,

        // Map over the $ in case of overwrite
        _$ = window.$,

        // [[Class]] -> type pairs
        class2type = {},

        // List of deleted data cache ids, so we can reuse them
        core_deletedIds = [],

        core_version = "2.0.3",

        // Save a reference to some core methods
        core_concat = core_deletedIds.concat,
        core_push = core_deletedIds.push,
        core_slice = core_deletedIds.slice,
        core_indexOf = core_deletedIds.indexOf,
        core_toString = class2type.toString,
        core_hasOwn = class2type.hasOwnProperty,
        core_trim = core_version.trim,

        // Define a local copy of jQuery
        jQuery = function(selector, context) {
            // The jQuery object is actually just the init constructor 'enhanced'
            return new jQuery.fn.init(selector, context, rootjQuery);
        },

        // Used for matching numbers
        core_pnum = /[+-]?(?:\d*\.|)\d+(?:[eE][+-]?\d+|)/.source,

        // Used for splitting on whitespace
        core_rnotwhite = /\S+/g,

        // A simple way to check for HTML strings
        // Prioritize #id over <tag> to avoid XSS via location.hash (#9521)
        // Strict HTML recognition (#11290: must start with <)
        rquickExpr = /^(?:\s*(<[\w\W]+>)[^>]*|#([\w-]*))$/,

        // Match a standalone tag
        rsingleTag = /^<(\w+)\s*\/?>(?:<\/\1>|)$/,

        // Matches dashed string for camelizing
        rmsPrefix = /^-ms-/,
        rdashAlpha = /-([\da-z])/gi,

        // Used by jQuery.camelCase as callback to replace()
        fcamelCase = function(all, letter) {
            return letter.toUpperCase();
        },

        // The ready event handler and self cleanup method
        completed = function() {
            document.removeEventListener("DOMContentLoaded", completed, false);
            window.removeEventListener("load", completed, false);
            jQuery.ready();
        };

    jQuery.fn = jQuery.prototype = {
        // The current version of jQuery being used
        jquery: core_version,

        constructor: jQuery,
        init: function(selector, context, rootjQuery) {
            var match, elem;

            // HANDLE: $(""), $(null), $(undefined), $(false)
            if (!selector) {
                return this;
            }

            // Handle HTML strings
            if (typeof selector === "string") {
                if (selector.charAt(0) === "<" && selector.charAt(selector.length - 1) === ">" && selector.length >= 3) {
                    // Assume that strings that start and end with <> are HTML and skip the regex check
                    match = [null, selector, null];

                } else {
                    match = rquickExpr.exec(selector);
                }

                // Match html or make sure no context is specified for #id
                if (match && (match[1] || !context)) {

                    // HANDLE: $(html) -> $(array)
                    if (match[1]) {
                        context = context instanceof jQuery ? context[0] : context;

                        // scripts is true for back-compat
                        jQuery.merge(this, jQuery.parseHTML(
                        match[1],
                        context && context.nodeType ? context.ownerDocument || context : document,
                        true));

                        // HANDLE: $(html, props)
                        if (rsingleTag.test(match[1]) && jQuery.isPlainObject(context)) {
                            for (match in context) {
                                // Properties of context are called as methods if possible
                                if (jQuery.isFunction(this[match])) {
                                    this[match](context[match]);

                                    // ...and otherwise set as attributes
                                } else {
                                    this.attr(match, context[match]);
                                }
                            }
                        }

                        return this;

                        // HANDLE: $(#id)
                    } else {
                        elem = document.getElementById(match[2]);

                        // Check parentNode to catch when Blackberry 4.6 returns
                        // nodes that are no longer in the document #6963
                        if (elem && elem.parentNode) {
                            // Inject the element directly into the jQuery object
                            this.length = 1;
                            this[0] = elem;
                        }

                        this.context = document;
                        this.selector = selector;
                        return this;
                    }

                    // HANDLE: $(expr, $(...))
                } else if (!context || context.jquery) {
                    return (context || rootjQuery).find(selector);

                    // HANDLE: $(expr, context)
                    // (which is just equivalent to: $(context).find(expr)
                } else {
                    return this.constructor(context).find(selector);
                }

                // HANDLE: $(DOMElement)
            } else if (selector.nodeType) {
                this.context = this[0] = selector;
                this.length = 1;
                return this;

                // HANDLE: $(function)
                // Shortcut for document ready
            } else if (jQuery.isFunction(selector)) {
                return rootjQuery.ready(selector);
            }

            if (selector.selector !== undefined) {
                this.selector = selector.selector;
                this.context = selector.context;
            }

            return jQuery.makeArray(selector, this);
        },

        // Start with an empty selector
        selector: "",

        // The default length of a jQuery object is 0
        length: 0,

        toArray: function() {
            return core_slice.call(this);
        },

        // Get the Nth element in the matched element set OR
        // Get the whole matched element set as a clean array
        get: function(num) {
            return num == null ?

            // Return a 'clean' array
            this.toArray() :

            // Return just the object
            (num < 0 ? this[this.length + num] : this[num]);
        },

        // Take an array of elements and push it onto the stack
        // (returning the new matched element set)
        pushStack: function(elems) {

            // Build a new jQuery matched element set
            var ret = jQuery.merge(this.constructor(), elems);

            // Add the old object onto the stack (as a reference)
            ret.prevObject = this;
            ret.context = this.context;

            // Return the newly-formed element set
            return ret;
        },

        // Execute a callback for every element in the matched set.
        // (You can seed the arguments with an array of args, but this is
        // only used internally.)
        each: function(callback, args) {
            return jQuery.each(this, callback, args);
        },

        ready: function(fn) {
            // Add the callback
            jQuery.ready.promise().done(fn);

            return this;
        },

        slice: function() {
            return this.pushStack(core_slice.apply(this, arguments));
        },

        first: function() {
            return this.eq(0);
        },

        last: function() {
            return this.eq(-1);
        },

        eq: function(i) {
            var len = this.length,
                j = +i + (i < 0 ? len : 0);
            return this.pushStack(j >= 0 && j < len ? [this[j]] : []);
        },

        map: function(callback) {
            return this.pushStack(jQuery.map(this, function(elem, i) {
                return callback.call(elem, i, elem);
            }));
        },

        end: function() {
            return this.prevObject || this.constructor(null);
        },

        // For internal use only.
        // Behaves like an Array's method, not like a jQuery method.
        push: core_push,
        sort: [].sort,
        splice: [].splice
    };

    // Give the init function the jQuery prototype for later instantiation
    jQuery.fn.init.prototype = jQuery.fn;

    jQuery.extend = jQuery.fn.extend = function() {
        var options, name, src, copy, copyIsArray, clone,
        target = arguments[0] || {},
        i = 1,
            length = arguments.length,
            deep = false;

        // Handle a deep copy situation
        if (typeof target === "boolean") {
            deep = target;
            target = arguments[1] || {};
            // skip the boolean and the target
            i = 2;
        }

        // Handle case when target is a string or something (possible in deep copy)
        if (typeof target !== "object" && !jQuery.isFunction(target)) {
            target = {};
        }

        // extend jQuery itself if only one argument is passed
        if (length === i) {
            target = this;
            --i;
        }

        for (; i < length; i++) {
            // Only deal with non-null/undefined values
            if ((options = arguments[i]) != null) {
                // Extend the base object
                for (name in options) {
                    src = target[name];
                    copy = options[name];

                    // Prevent never-ending loop
                    if (target === copy) {
                        continue;
                    }

                    // Recurse if we're merging plain objects or arrays
                    if (deep && copy && (jQuery.isPlainObject(copy) || (copyIsArray = jQuery.isArray(copy)))) {
                        if (copyIsArray) {
                            copyIsArray = false;
                            clone = src && jQuery.isArray(src) ? src : [];

                        } else {
                            clone = src && jQuery.isPlainObject(src) ? src : {};
                        }

                        // Never move original objects, clone them
                        target[name] = jQuery.extend(deep, clone, copy);

                        // Don't bring in undefined values
                    } else if (copy !== undefined) {
                        target[name] = copy;
                    }
                }
            }
        }

        // Return the modified object
        return target;
    };

    jQuery.extend({
        // Unique for each copy of jQuery on the page
        expando: "jQuery" + (core_version + Math.random()).replace(/\D/g, ""),

        noConflict: function(deep) {
            if (window.$ === jQuery) {
                window.$ = _$;
            }

            if (deep && window.jQuery === jQuery) {
                window.jQuery = _jQuery;
            }

            return jQuery;
        },

        // Is the DOM ready to be used? Set to true once it occurs.
        isReady: false,

        // A counter to track how many items to wait for before
        // the ready event fires. See #6781
        readyWait: 1,

        // Hold (or release) the ready event
        holdReady: function(hold) {
            if (hold) {
                jQuery.readyWait++;
            } else {
                jQuery.ready(true);
            }
        },

        // Handle when the DOM is ready
        ready: function(wait) {

            // Abort if there are pending holds or we're already ready
            if (wait === true ? --jQuery.readyWait : jQuery.isReady) {
                return;
            }

            // Remember that the DOM is ready
            jQuery.isReady = true;

            // If a normal DOM Ready event fired, decrement, and wait if need be
            if (wait !== true && --jQuery.readyWait > 0) {
                return;
            }

            // If there are functions bound, to execute
            readyList.resolveWith(document, [jQuery]);

            // Trigger any bound ready events
            if (jQuery.fn.trigger) {
                jQuery(document).trigger("ready").off("ready");
            }
        },

        // See test/unit/core.js for details concerning isFunction.
        // Since version 1.3, DOM methods and functions like alert
        // aren't supported. They return false on IE (#2968).
        isFunction: function(obj) {
            return jQuery.type(obj) === "function";
        },

        isArray: Array.isArray,

        isWindow: function(obj) {
            return obj != null && obj === obj.window;
        },

        isNumeric: function(obj) {
            return !isNaN(parseFloat(obj)) && isFinite(obj);
        },

        type: function(obj) {
            if (obj == null) {
                return String(obj);
            }
            // Support: Safari <= 5.1 (functionish RegExp)
            return typeof obj === "object" || typeof obj === "function" ? class2type[core_toString.call(obj)] || "object" : typeof obj;
        },

        isPlainObject: function(obj) {
            // Not plain objects:
            // - Any object or value whose internal [[Class]] property is not "[object Object]"
            // - DOM nodes
            // - window
            if (jQuery.type(obj) !== "object" || obj.nodeType || jQuery.isWindow(obj)) {
                return false;
            }

            // Support: Firefox <20
            // The try/catch suppresses exceptions thrown when attempting to access
            // the "constructor" property of certain host objects, ie. |window.location|
            // https://bugzilla.mozilla.org/show_bug.cgi?id=814622
            try {
                if (obj.constructor && !core_hasOwn.call(obj.constructor.prototype, "isPrototypeOf")) {
                    return false;
                }
            } catch (e) {
                return false;
            }

            // If the function hasn't returned already, we're confident that
            // |obj| is a plain object, created by {} or constructed with new Object
            return true;
        },

        isEmptyObject: function(obj) {
            var name;
            for (name in obj) {
                return false;
            }
            return true;
        },

        error: function(msg) {
            throw new Error(msg);
        },

        // data: string of html
        // context (optional): If specified, the fragment will be created in this context, defaults to document
        // keepScripts (optional): If true, will include scripts passed in the html string
        parseHTML: function(data, context, keepScripts) {
            if (!data || typeof data !== "string") {
                return null;
            }
            if (typeof context === "boolean") {
                keepScripts = context;
                context = false;
            }
            context = context || document;

            var parsed = rsingleTag.exec(data),
                scripts = !keepScripts && [];

            // Single tag
            if (parsed) {
                return [context.createElement(parsed[1])];
            }

            parsed = jQuery.buildFragment([data], context, scripts);

            if (scripts) {
                jQuery(scripts).remove();
            }

            return jQuery.merge([], parsed.childNodes);
        },

        parseJSON: JSON.parse,

        // Cross-browser xml parsing
        parseXML: function(data) {
            var xml, tmp;
            if (!data || typeof data !== "string") {
                return null;
            }

            // Support: IE9
            try {
                tmp = new DOMParser();
                xml = tmp.parseFromString(data, "text/xml");
            } catch (e) {
                xml = undefined;
            }

            if (!xml || xml.getElementsByTagName("parsererror").length) {
                jQuery.error("Invalid XML: " + data);
            }
            return xml;
        },

        noop: function() {},

        // Evaluates a script in a global context
        globalEval: function(code) {
            var script,
            indirect = eval;

            code = jQuery.trim(code);

            if (code) {
                // If the code includes a valid, prologue position
                // strict mode pragma, execute code by injecting a
                // script tag into the document.
                if (code.indexOf("use strict") === 1) {
                    script = document.createElement("script");
                    script.text = code;
                    document.head.appendChild(script).parentNode.removeChild(script);
                } else {
                    // Otherwise, avoid the DOM node creation, insertion
                    // and removal by using an indirect global eval
                    indirect(code);
                }
            }
        },

        // Convert dashed to camelCase; used by the css and data modules
        // Microsoft forgot to hump their vendor prefix (#9572)
        camelCase: function(string) {
            return string.replace(rmsPrefix, "ms-").replace(rdashAlpha, fcamelCase);
        },

        nodeName: function(elem, name) {
            return elem.nodeName && elem.nodeName.toLowerCase() === name.toLowerCase();
        },

        // args is for internal usage only
        each: function(obj, callback, args) {
            var value,
            i = 0,
                length = obj.length,
                isArray = isArraylike(obj);

            if (args) {
                if (isArray) {
                    for (; i < length; i++) {
                        value = callback.apply(obj[i], args);

                        if (value === false) {
                            break;
                        }
                    }
                } else {
                    for (i in obj) {
                        value = callback.apply(obj[i], args);

                        if (value === false) {
                            break;
                        }
                    }
                }

                // A special, fast, case for the most common use of each
            } else {
                if (isArray) {
                    for (; i < length; i++) {
                        value = callback.call(obj[i], i, obj[i]);

                        if (value === false) {
                            break;
                        }
                    }
                } else {
                    for (i in obj) {
                        value = callback.call(obj[i], i, obj[i]);

                        if (value === false) {
                            break;
                        }
                    }
                }
            }

            return obj;
        },

        trim: function(text) {
            return text == null ? "" : core_trim.call(text);
        },

        // results is for internal usage only
        makeArray: function(arr, results) {
            var ret = results || [];

            if (arr != null) {
                if (isArraylike(Object(arr))) {
                    jQuery.merge(ret,
                    typeof arr === "string" ? [arr] : arr);
                } else {
                    core_push.call(ret, arr);
                }
            }

            return ret;
        },

        inArray: function(elem, arr, i) {
            return arr == null ? -1 : core_indexOf.call(arr, elem, i);
        },

        merge: function(first, second) {
            var l = second.length,
                i = first.length,
                j = 0;

            if (typeof l === "number") {
                for (; j < l; j++) {
                    first[i++] = second[j];
                }
            } else {
                while (second[j] !== undefined) {
                    first[i++] = second[j++];
                }
            }

            first.length = i;

            return first;
        },

        grep: function(elems, callback, inv) {
            var retVal,
            ret = [],
                i = 0,
                length = elems.length;
            inv = !! inv;

            // Go through the array, only saving the items
            // that pass the validator function
            for (; i < length; i++) {
                retVal = !! callback(elems[i], i);
                if (inv !== retVal) {
                    ret.push(elems[i]);
                }
            }

            return ret;
        },

        // arg is for internal usage only
        map: function(elems, callback, arg) {
            var value,
            i = 0,
                length = elems.length,
                isArray = isArraylike(elems),
                ret = [];

            // Go through the array, translating each of the items to their
            if (isArray) {
                for (; i < length; i++) {
                    value = callback(elems[i], i, arg);

                    if (value != null) {
                        ret[ret.length] = value;
                    }
                }

                // Go through every key on the object,
            } else {
                for (i in elems) {
                    value = callback(elems[i], i, arg);

                    if (value != null) {
                        ret[ret.length] = value;
                    }
                }
            }

            // Flatten any nested arrays
            return core_concat.apply([], ret);
        },

        // A global GUID counter for objects
        guid: 1,

        // Bind a function to a context, optionally partially applying any
        // arguments.
        proxy: function(fn, context) {
            var tmp, args, proxy;

            if (typeof context === "string") {
                tmp = fn[context];
                context = fn;
                fn = tmp;
            }

            // Quick check to determine if target is callable, in the spec
            // this throws a TypeError, but we will just return undefined.
            if (!jQuery.isFunction(fn)) {
                return undefined;
            }

            // Simulated bind
            args = core_slice.call(arguments, 2);
            proxy = function() {
                return fn.apply(context || this, args.concat(core_slice.call(arguments)));
            };

            // Set the guid of unique handler to the same of original handler, so it can be removed
            proxy.guid = fn.guid = fn.guid || jQuery.guid++;

            return proxy;
        },

        // Multifunctional method to get and set values of a collection
        // The value/s can optionally be executed if it's a function
        access: function(elems, fn, key, value, chainable, emptyGet, raw) {
            var i = 0,
                length = elems.length,
                bulk = key == null;

            // Sets many values
            if (jQuery.type(key) === "object") {
                chainable = true;
                for (i in key) {
                    jQuery.access(elems, fn, i, key[i], true, emptyGet, raw);
                }

                // Sets one value
            } else if (value !== undefined) {
                chainable = true;

                if (!jQuery.isFunction(value)) {
                    raw = true;
                }

                if (bulk) {
                    // Bulk operations run against the entire set
                    if (raw) {
                        fn.call(elems, value);
                        fn = null;

                        // ...except when executing function values
                    } else {
                        bulk = fn;
                        fn = function(elem, key, value) {
                            return bulk.call(jQuery(elem), value);
                        };
                    }
                }

                if (fn) {
                    for (; i < length; i++) {
                        fn(elems[i], key, raw ? value : value.call(elems[i], i, fn(elems[i], key)));
                    }
                }
            }

            return chainable ? elems :

            // Gets
            bulk ? fn.call(elems) : length ? fn(elems[0], key) : emptyGet;
        },

        now: Date.now,

        // A method for quickly swapping in/out CSS properties to get correct calculations.
        // Note: this method belongs to the css module but it's needed here for the support module.
        // If support gets modularized, this method should be moved back to the css module.
        swap: function(elem, options, callback, args) {
            var ret, name,
            old = {};

            // Remember the old values, and insert the new ones
            for (name in options) {
                old[name] = elem.style[name];
                elem.style[name] = options[name];
            }

            ret = callback.apply(elem, args || []);

            // Revert the old values
            for (name in options) {
                elem.style[name] = old[name];
            }

            return ret;
        }
    });

    jQuery.ready.promise = function(obj) {
        if (!readyList) {

            readyList = jQuery.Deferred();

            // Catch cases where $(document).ready() is called after the browser event has already occurred.
            // we once tried to use readyState "interactive" here, but it caused issues like the one
            // discovered by ChrisS here: http://bugs.jquery.com/ticket/12282#comment:15
            if (document.readyState === "complete") {
                // Handle it asynchronously to allow scripts the opportunity to delay ready
                setTimeout(jQuery.ready);

            } else {

                // Use the handy event callback
                document.addEventListener("DOMContentLoaded", completed, false);

                // A fallback to window.onload, that will always work
                window.addEventListener("load", completed, false);
            }
        }
        return readyList.promise(obj);
    };

    // Populate the class2type map
    jQuery.each("Boolean Number String Function Array Date RegExp Object Error".split(" "), function(i, name) {
        class2type["[object " + name + "]"] = name.toLowerCase();
    });

    function isArraylike(obj) {
        var length = obj.length,
            type = jQuery.type(obj);

        if (jQuery.isWindow(obj)) {
            return false;
        }

        if (obj.nodeType === 1 && length) {
            return true;
        }

        return type === "array" || type !== "function" && (length === 0 || typeof length === "number" && length > 0 && (length - 1) in obj);
    }

    // All jQuery objects should point back to these
    rootjQuery = jQuery(document);
    /*!
     * Sizzle CSS Selector Engine v1.9.4-pre
     * http://sizzlejs.com/
     *
     * Copyright 2013 jQuery Foundation, Inc. and other contributors
     * Released under the MIT license
     * http://jquery.org/license
     *
     * Date: 2013-06-03
     */ (function(window, undefined) {

        var i,
        support,
        cachedruns,
        Expr,
        getText,
        isXML,
        compile,
        outermostContext,
        sortInput,

        // Local document vars
        setDocument,
        document,
        docElem,
        documentIsHTML,
        rbuggyQSA,
        rbuggyMatches,
        matches,
        contains,

        // Instance-specific data
        expando = "sizzle" + -(new Date()),
            preferredDoc = window.document,
            dirruns = 0,
            done = 0,
            classCache = createCache(),
            tokenCache = createCache(),
            compilerCache = createCache(),
            hasDuplicate = false,
            sortOrder = function(a, b) {
                if (a === b) {
                    hasDuplicate = true;
                    return 0;
                }
                return 0;
            },

            // General-purpose constants
            strundefined = typeof undefined,
            MAX_NEGATIVE = 1 << 31,

            // Instance methods
            hasOwn = ({}).hasOwnProperty,
            arr = [],
            pop = arr.pop,
            push_native = arr.push,
            push = arr.push,
            slice = arr.slice,
            // Use a stripped-down indexOf if we can't use a native one
            indexOf = arr.indexOf || function(elem) {
                var i = 0,
                    len = this.length;
                for (; i < len; i++) {
                    if (this[i] === elem) {
                        return i;
                    }
                }
                return -1;
            },

            booleans = "checked|selected|async|autofocus|autoplay|controls|defer|disabled|hidden|ismap|loop|multiple|open|readonly|required|scoped",

            // Regular expressions

            // Whitespace characters http://www.w3.org/TR/css3-selectors/#whitespace
            whitespace = "[\\x20\\t\\r\\n\\f]",
            // http://www.w3.org/TR/css3-syntax/#characters
            characterEncoding = "(?:\\\\.|[\\w-]|[^\\x00-\\xa0])+",

            // Loosely modeled on CSS identifier characters
            // An unquoted value should be a CSS identifier http://www.w3.org/TR/css3-selectors/#attribute-selectors
            // Proper syntax: http://www.w3.org/TR/CSS21/syndata.html#value-def-identifier
            identifier = characterEncoding.replace("w", "w#"),

            // Acceptable operators http://www.w3.org/TR/selectors/#attribute-selectors
            attributes = "\\[" + whitespace + "*(" + characterEncoding + ")" + whitespace + "*(?:([*^$|!~]?=)" + whitespace + "*(?:(['\"])((?:\\\\.|[^\\\\])*?)\\3|(" + identifier + ")|)|)" + whitespace + "*\\]",

            // Prefer arguments quoted,
            //   then not containing pseudos/brackets,
            //   then attribute selectors/non-parenthetical expressions,
            //   then anything else
            // These preferences are here to reduce the number of selectors
            //   needing tokenize in the PSEUDO preFilter
            pseudos = ":(" + characterEncoding + ")(?:\\(((['\"])((?:\\\\.|[^\\\\])*?)\\3|((?:\\\\.|[^\\\\()[\\]]|" + attributes.replace(3, 8) + ")*)|.*)\\)|)",

            // Leading and non-escaped trailing whitespace, capturing some non-whitespace characters preceding the latter
            rtrim = new RegExp("^" + whitespace + "+|((?:^|[^\\\\])(?:\\\\.)*)" + whitespace + "+$", "g"),

            rcomma = new RegExp("^" + whitespace + "*," + whitespace + "*"),
            rcombinators = new RegExp("^" + whitespace + "*([>+~]|" + whitespace + ")" + whitespace + "*"),

            rsibling = new RegExp(whitespace + "*[+~]"),
            rattributeQuotes = new RegExp("=" + whitespace + "*([^\\]'\"]*)" + whitespace + "*\\]", "g"),

            rpseudo = new RegExp(pseudos),
            ridentifier = new RegExp("^" + identifier + "$"),

            matchExpr = {
                "ID": new RegExp("^#(" + characterEncoding + ")"),
                "CLASS": new RegExp("^\\.(" + characterEncoding + ")"),
                "TAG": new RegExp("^(" + characterEncoding.replace("w", "w*") + ")"),
                "ATTR": new RegExp("^" + attributes),
                "PSEUDO": new RegExp("^" + pseudos),
                "CHILD": new RegExp("^:(only|first|last|nth|nth-last)-(child|of-type)(?:\\(" + whitespace + "*(even|odd|(([+-]|)(\\d*)n|)" + whitespace + "*(?:([+-]|)" + whitespace + "*(\\d+)|))" + whitespace + "*\\)|)", "i"),
                "bool": new RegExp("^(?:" + booleans + ")$", "i"),
                // For use in libraries implementing .is()
                // We use this for POS matching in `select`
                "needsContext": new RegExp("^" + whitespace + "*[>+~]|:(even|odd|eq|gt|lt|nth|first|last)(?:\\(" + whitespace + "*((?:-\\d)?\\d*)" + whitespace + "*\\)|)(?=[^-]|$)", "i")
            },

            rnative = /^[^{]+\{\s*\[native \w/,

            // Easily-parseable/retrievable ID or TAG or CLASS selectors
            rquickExpr = /^(?:#([\w-]+)|(\w+)|\.([\w-]+))$/,

            rinputs = /^(?:input|select|textarea|button)$/i,
            rheader = /^h\d$/i,

            rescape = /'|\\/g,

            // CSS escapes http://www.w3.org/TR/CSS21/syndata.html#escaped-characters
            runescape = new RegExp("\\\\([\\da-f]{1,6}" + whitespace + "?|(" + whitespace + ")|.)", "ig"),
            funescape = function(_, escaped, escapedWhitespace) {
                var high = "0x" + escaped - 0x10000;
                // NaN means non-codepoint
                // Support: Firefox
                // Workaround erroneous numeric interpretation of +"0x"
                return high !== high || escapedWhitespace ? escaped :
                // BMP codepoint
                high < 0 ? String.fromCharCode(high + 0x10000) :
                // Supplemental Plane codepoint (surrogate pair)
                String.fromCharCode(high >> 10 | 0xD800, high & 0x3FF | 0xDC00);
            };

        // Optimize for push.apply( _, NodeList )
        try {
            push.apply(
            (arr = slice.call(preferredDoc.childNodes)),
            preferredDoc.childNodes);
            // Support: Android<4.0
            // Detect silently failing push.apply
            arr[preferredDoc.childNodes.length].nodeType;
        } catch (e) {
            push = {
                apply: arr.length ?

                // Leverage slice if possible
                function(target, els) {
                    push_native.apply(target, slice.call(els));
                } :

                // Support: IE<9
                // Otherwise append directly
                function(target, els) {
                    var j = target.length,
                        i = 0;
                    // Can't trust NodeList.length
                    while ((target[j++] = els[i++])) {}
                    target.length = j - 1;
                }
            };
        }

        function Sizzle(selector, context, results, seed) {
            var match, elem, m, nodeType,
            // QSA vars
            i, groups, old, nid, newContext, newSelector;

            if ((context ? context.ownerDocument || context : preferredDoc) !== document) {
                setDocument(context);
            }

            context = context || document;
            results = results || [];

            if (!selector || typeof selector !== "string") {
                return results;
            }

            if ((nodeType = context.nodeType) !== 1 && nodeType !== 9) {
                return [];
            }

            if (documentIsHTML && !seed) {

                // Shortcuts
                if ((match = rquickExpr.exec(selector))) {
                    // Speed-up: Sizzle("#ID")
                    if ((m = match[1])) {
                        if (nodeType === 9) {
                            elem = context.getElementById(m);
                            // Check parentNode to catch when Blackberry 4.6 returns
                            // nodes that are no longer in the document #6963
                            if (elem && elem.parentNode) {
                                // Handle the case where IE, Opera, and Webkit return items
                                // by name instead of ID
                                if (elem.id === m) {
                                    results.push(elem);
                                    return results;
                                }
                            } else {
                                return results;
                            }
                        } else {
                            // Context is not a document
                            if (context.ownerDocument && (elem = context.ownerDocument.getElementById(m)) && contains(context, elem) && elem.id === m) {
                                results.push(elem);
                                return results;
                            }
                        }

                        // Speed-up: Sizzle("TAG")
                    } else if (match[2]) {
                        push.apply(results, context.getElementsByTagName(selector));
                        return results;

                        // Speed-up: Sizzle(".CLASS")
                    } else if ((m = match[3]) && support.getElementsByClassName && context.getElementsByClassName) {
                        push.apply(results, context.getElementsByClassName(m));
                        return results;
                    }
                }

                // QSA path
                if (support.qsa && (!rbuggyQSA || !rbuggyQSA.test(selector))) {
                    nid = old = expando;
                    newContext = context;
                    newSelector = nodeType === 9 && selector;

                    // qSA works strangely on Element-rooted queries
                    // We can work around this by specifying an extra ID on the root
                    // and working up from there (Thanks to Andrew Dupont for the technique)
                    // IE 8 doesn't work on object elements
                    if (nodeType === 1 && context.nodeName.toLowerCase() !== "object") {
                        groups = tokenize(selector);

                        if ((old = context.getAttribute("id"))) {
                            nid = old.replace(rescape, "\\$&");
                        } else {
                            context.setAttribute("id", nid);
                        }
                        nid = "[id='" + nid + "'] ";

                        i = groups.length;
                        while (i--) {
                            groups[i] = nid + toSelector(groups[i]);
                        }
                        newContext = rsibling.test(selector) && context.parentNode || context;
                        newSelector = groups.join(",");
                    }

                    if (newSelector) {
                        try {
                            push.apply(results,
                            newContext.querySelectorAll(newSelector));
                            return results;
                        } catch (qsaError) {} finally {
                            if (!old) {
                                context.removeAttribute("id");
                            }
                        }
                    }
                }
            }

            // All others
            return select(selector.replace(rtrim, "$1"), context, results, seed);
        }

        /**
         * Create key-value caches of limited size
         * @returns {Function(string, Object)} Returns the Object data after storing it on itself with
         *	property name the (space-suffixed) string and (if the cache is larger than Expr.cacheLength)
         *	deleting the oldest entry
         */
        function createCache() {
            var keys = [];

            function cache(key, value) {
                // Use (key + " ") to avoid collision with native prototype properties (see Issue #157)
                if (keys.push(key += " ") > Expr.cacheLength) {
                    // Only keep the most recent entries
                    delete cache[keys.shift()];
                }
                return (cache[key] = value);
            }
            return cache;
        }

        /**
         * Mark a function for special use by Sizzle
         * @param {Function} fn The function to mark
         */
        function markFunction(fn) {
            fn[expando] = true;
            return fn;
        }

        /**
         * Support testing using an element
         * @param {Function} fn Passed the created div and expects a boolean result
         */
        function assert(fn) {
            var div = document.createElement("div");

            try {
                return !!fn(div);
            } catch (e) {
                return false;
            } finally {
                // Remove from its parent by default
                if (div.parentNode) {
                    div.parentNode.removeChild(div);
                }
                // release memory in IE
                div = null;
            }
        }

        /**
         * Adds the same handler for all of the specified attrs
         * @param {String} attrs Pipe-separated list of attributes
         * @param {Function} handler The method that will be applied
         */
        function addHandle(attrs, handler) {
            var arr = attrs.split("|"),
                i = attrs.length;

            while (i--) {
                Expr.attrHandle[arr[i]] = handler;
            }
        }

        /**
         * Checks document order of two siblings
         * @param {Element} a
         * @param {Element} b
         * @returns {Number} Returns less than 0 if a precedes b, greater than 0 if a follows b
         */
        function siblingCheck(a, b) {
            var cur = b && a,
                diff = cur && a.nodeType === 1 && b.nodeType === 1 && (~b.sourceIndex || MAX_NEGATIVE) - (~a.sourceIndex || MAX_NEGATIVE);

            // Use IE sourceIndex if available on both nodes
            if (diff) {
                return diff;
            }

            // Check if b follows a
            if (cur) {
                while ((cur = cur.nextSibling)) {
                    if (cur === b) {
                        return -1;
                    }
                }
            }

            return a ? 1 : -1;
        }

        /**
         * Returns a function to use in pseudos for input types
         * @param {String} type
         */
        function createInputPseudo(type) {
            return function(elem) {
                var name = elem.nodeName.toLowerCase();
                return name === "input" && elem.type === type;
            };
        }

        /**
         * Returns a function to use in pseudos for buttons
         * @param {String} type
         */
        function createButtonPseudo(type) {
            return function(elem) {
                var name = elem.nodeName.toLowerCase();
                return (name === "input" || name === "button") && elem.type === type;
            };
        }

        /**
         * Returns a function to use in pseudos for positionals
         * @param {Function} fn
         */
        function createPositionalPseudo(fn) {
            return markFunction(function(argument) {
                argument = +argument;
                return markFunction(function(seed, matches) {
                    var j,
                    matchIndexes = fn([], seed.length, argument),
                        i = matchIndexes.length;

                    // Match elements found at the specified indexes
                    while (i--) {
                        if (seed[(j = matchIndexes[i])]) {
                            seed[j] = !(matches[j] = seed[j]);
                        }
                    }
                });
            });
        }

        /**
         * Detect xml
         * @param {Element|Object} elem An element or a document
         */
        isXML = Sizzle.isXML = function(elem) {
            // documentElement is verified for cases where it doesn't yet exist
            // (such as loading iframes in IE - #4833)
            var documentElement = elem && (elem.ownerDocument || elem).documentElement;
            return documentElement ? documentElement.nodeName !== "HTML" : false;
        };

        // Expose support vars for convenience
        support = Sizzle.support = {};

        /**
         * Sets document-related variables once based on the current document
         * @param {Element|Object} [doc] An element or document object to use to set the document
         * @returns {Object} Returns the current document
         */
        setDocument = Sizzle.setDocument = function(node) {
            var doc = node ? node.ownerDocument || node : preferredDoc,
                parent = doc.defaultView;

            // If no document and documentElement is available, return
            if (doc === document || doc.nodeType !== 9 || !doc.documentElement) {
                return document;
            }

            // Set our document
            document = doc;
            docElem = doc.documentElement;

            // Support tests
            documentIsHTML = !isXML(doc);

            // Support: IE>8
            // If iframe document is assigned to "document" variable and if iframe has been reloaded,
            // IE will throw "permission denied" error when accessing "document" variable, see jQuery #13936
            // IE6-8 do not support the defaultView property so parent will be undefined
            if (parent && parent.attachEvent && parent !== parent.top) {
                parent.attachEvent("onbeforeunload", function() {
                    setDocument();
                });
            }

            /* Attributes
	---------------------------------------------------------------------- */

            // Support: IE<8
            // Verify that getAttribute really returns attributes and not properties (excepting IE8 booleans)
            support.attributes = assert(function(div) {
                div.className = "i";
                return !div.getAttribute("className");
            });

            /* getElement(s)By*
	---------------------------------------------------------------------- */

            // Check if getElementsByTagName("*") returns only elements
            support.getElementsByTagName = assert(function(div) {
                div.appendChild(doc.createComment(""));
                return !div.getElementsByTagName("*").length;
            });

            // Check if getElementsByClassName can be trusted
            support.getElementsByClassName = assert(function(div) {
                div.innerHTML = "<div class='a'></div><div class='a i'></div>";

                // Support: Safari<4
                // Catch class over-caching
                div.firstChild.className = "i";
                // Support: Opera<10
                // Catch gEBCN failure to find non-leading classes
                return div.getElementsByClassName("i").length === 2;
            });

            // Support: IE<10
            // Check if getElementById returns elements by name
            // The broken getElementById methods don't pick up programatically-set names,
            // so use a roundabout getElementsByName test
            support.getById = assert(function(div) {
                docElem.appendChild(div).id = expando;
                return !doc.getElementsByName || !doc.getElementsByName(expando).length;
            });

            // ID find and filter
            if (support.getById) {
                Expr.find["ID"] = function(id, context) {
                    if (typeof context.getElementById !== strundefined && documentIsHTML) {
                        var m = context.getElementById(id);
                        // Check parentNode to catch when Blackberry 4.6 returns
                        // nodes that are no longer in the document #6963
                        return m && m.parentNode ? [m] : [];
                    }
                };
                Expr.filter["ID"] = function(id) {
                    var attrId = id.replace(runescape, funescape);
                    return function(elem) {
                        return elem.getAttribute("id") === attrId;
                    };
                };
            } else {
                // Support: IE6/7
                // getElementById is not reliable as a find shortcut
                delete Expr.find["ID"];

                Expr.filter["ID"] = function(id) {
                    var attrId = id.replace(runescape, funescape);
                    return function(elem) {
                        var node = typeof elem.getAttributeNode !== strundefined && elem.getAttributeNode("id");
                        return node && node.value === attrId;
                    };
                };
            }

            // Tag
            Expr.find["TAG"] = support.getElementsByTagName ? function(tag, context) {
                if (typeof context.getElementsByTagName !== strundefined) {
                    return context.getElementsByTagName(tag);
                }
            } : function(tag, context) {
                var elem,
                tmp = [],
                    i = 0,
                    results = context.getElementsByTagName(tag);

                // Filter out possible comments
                if (tag === "*") {
                    while ((elem = results[i++])) {
                        if (elem.nodeType === 1) {
                            tmp.push(elem);
                        }
                    }

                    return tmp;
                }
                return results;
            };

            // Class
            Expr.find["CLASS"] = support.getElementsByClassName && function(className, context) {
                if (typeof context.getElementsByClassName !== strundefined && documentIsHTML) {
                    return context.getElementsByClassName(className);
                }
            };

            /* QSA/matchesSelector
	---------------------------------------------------------------------- */

            // QSA and matchesSelector support

            // matchesSelector(:active) reports false when true (IE9/Opera 11.5)
            rbuggyMatches = [];

            // qSa(:focus) reports false when true (Chrome 21)
            // We allow this because of a bug in IE8/9 that throws an error
            // whenever `document.activeElement` is accessed on an iframe
            // So, we allow :focus to pass through QSA all the time to avoid the IE error
            // See http://bugs.jquery.com/ticket/13378
            rbuggyQSA = [];

            if ((support.qsa = rnative.test(doc.querySelectorAll))) {
                // Build QSA regex
                // Regex strategy adopted from Diego Perini
                assert(function(div) {
                    // Select is set to empty string on purpose
                    // This is to test IE's treatment of not explicitly
                    // setting a boolean content attribute,
                    // since its presence should be enough
                    // http://bugs.jquery.com/ticket/12359
                    div.innerHTML = "<select><option selected=''></option></select>";

                    // Support: IE8
                    // Boolean attributes and "value" are not treated correctly
                    if (!div.querySelectorAll("[selected]").length) {
                        rbuggyQSA.push("\\[" + whitespace + "*(?:value|" + booleans + ")");
                    }

                    // Webkit/Opera - :checked should return selected option elements
                    // http://www.w3.org/TR/2011/REC-css3-selectors-20110929/#checked
                    // IE8 throws error here and will not see later tests
                    if (!div.querySelectorAll(":checked").length) {
                        rbuggyQSA.push(":checked");
                    }
                });

                assert(function(div) {

                    // Support: Opera 10-12/IE8
                    // ^= $= *= and empty values
                    // Should not select anything
                    // Support: Windows 8 Native Apps
                    // The type attribute is restricted during .innerHTML assignment
                    var input = doc.createElement("input");
                    input.setAttribute("type", "hidden");
                    div.appendChild(input).setAttribute("t", "");

                    if (div.querySelectorAll("[t^='']").length) {
                        rbuggyQSA.push("[*^$]=" + whitespace + "*(?:''|\"\")");
                    }

                    // FF 3.5 - :enabled/:disabled and hidden elements (hidden elements are still enabled)
                    // IE8 throws error here and will not see later tests
                    if (!div.querySelectorAll(":enabled").length) {
                        rbuggyQSA.push(":enabled", ":disabled");
                    }

                    // Opera 10-11 does not throw on post-comma invalid pseudos
                    div.querySelectorAll("*,:x");
                    rbuggyQSA.push(",.*:");
                });
            }

            if ((support.matchesSelector = rnative.test((matches = docElem.webkitMatchesSelector || docElem.mozMatchesSelector || docElem.oMatchesSelector || docElem.msMatchesSelector)))) {

                assert(function(div) {
                    // Check to see if it's possible to do matchesSelector
                    // on a disconnected node (IE 9)
                    support.disconnectedMatch = matches.call(div, "div");

                    // This should fail with an exception
                    // Gecko does not error, returns false instead
                    matches.call(div, "[s!='']:x");
                    rbuggyMatches.push("!=", pseudos);
                });
            }

            rbuggyQSA = rbuggyQSA.length && new RegExp(rbuggyQSA.join("|"));
            rbuggyMatches = rbuggyMatches.length && new RegExp(rbuggyMatches.join("|"));

            /* Contains
	---------------------------------------------------------------------- */

            // Element contains another
            // Purposefully does not implement inclusive descendent
            // As in, an element does not contain itself
            contains = rnative.test(docElem.contains) || docElem.compareDocumentPosition ? function(a, b) {
                var adown = a.nodeType === 9 ? a.documentElement : a,
                    bup = b && b.parentNode;
                return a === bup || !! (bup && bup.nodeType === 1 && (
                adown.contains ? adown.contains(bup) : a.compareDocumentPosition && a.compareDocumentPosition(bup) & 16));
            } : function(a, b) {
                if (b) {
                    while ((b = b.parentNode)) {
                        if (b === a) {
                            return true;
                        }
                    }
                }
                return false;
            };

            /* Sorting
	---------------------------------------------------------------------- */

            // Document order sorting
            sortOrder = docElem.compareDocumentPosition ? function(a, b) {

                // Flag for duplicate removal
                if (a === b) {
                    hasDuplicate = true;
                    return 0;
                }

                var compare = b.compareDocumentPosition && a.compareDocumentPosition && a.compareDocumentPosition(b);

                if (compare) {
                    // Disconnected nodes
                    if (compare & 1 || (!support.sortDetached && b.compareDocumentPosition(a) === compare)) {

                        // Choose the first element that is related to our preferred document
                        if (a === doc || contains(preferredDoc, a)) {
                            return -1;
                        }
                        if (b === doc || contains(preferredDoc, b)) {
                            return 1;
                        }

                        // Maintain original order
                        return sortInput ? (indexOf.call(sortInput, a) - indexOf.call(sortInput, b)) : 0;
                    }

                    return compare & 4 ? -1 : 1;
                }

                // Not directly comparable, sort on existence of method
                return a.compareDocumentPosition ? -1 : 1;
            } : function(a, b) {
                var cur,
                i = 0,
                    aup = a.parentNode,
                    bup = b.parentNode,
                    ap = [a],
                    bp = [b];

                // Exit early if the nodes are identical
                if (a === b) {
                    hasDuplicate = true;
                    return 0;

                    // Parentless nodes are either documents or disconnected
                } else if (!aup || !bup) {
                    return a === doc ? -1 : b === doc ? 1 : aup ? -1 : bup ? 1 : sortInput ? (indexOf.call(sortInput, a) - indexOf.call(sortInput, b)) : 0;

                    // If the nodes are siblings, we can do a quick check
                } else if (aup === bup) {
                    return siblingCheck(a, b);
                }

                // Otherwise we need full lists of their ancestors for comparison
                cur = a;
                while ((cur = cur.parentNode)) {
                    ap.unshift(cur);
                }
                cur = b;
                while ((cur = cur.parentNode)) {
                    bp.unshift(cur);
                }

                // Walk down the tree looking for a discrepancy
                while (ap[i] === bp[i]) {
                    i++;
                }

                return i ?
                // Do a sibling check if the nodes have a common ancestor
                siblingCheck(ap[i], bp[i]) :

                // Otherwise nodes in our document sort first
                ap[i] === preferredDoc ? -1 : bp[i] === preferredDoc ? 1 : 0;
            };

            return doc;
        };

        Sizzle.matches = function(expr, elements) {
            return Sizzle(expr, null, null, elements);
        };

        Sizzle.matchesSelector = function(elem, expr) {
            // Set document vars if needed
            if ((elem.ownerDocument || elem) !== document) {
                setDocument(elem);
            }

            // Make sure that attribute selectors are quoted
            expr = expr.replace(rattributeQuotes, "='$1']");

            if (support.matchesSelector && documentIsHTML && (!rbuggyMatches || !rbuggyMatches.test(expr)) && (!rbuggyQSA || !rbuggyQSA.test(expr))) {

                try {
                    var ret = matches.call(elem, expr);

                    // IE 9's matchesSelector returns false on disconnected nodes
                    if (ret || support.disconnectedMatch ||
                    // As well, disconnected nodes are said to be in a document
                    // fragment in IE 9
                    elem.document && elem.document.nodeType !== 11) {
                        return ret;
                    }
                } catch (e) {}
            }

            return Sizzle(expr, document, null, [elem]).length > 0;
        };

        Sizzle.contains = function(context, elem) {
            // Set document vars if needed
            if ((context.ownerDocument || context) !== document) {
                setDocument(context);
            }
            return contains(context, elem);
        };

        Sizzle.attr = function(elem, name) {
            // Set document vars if needed
            if ((elem.ownerDocument || elem) !== document) {
                setDocument(elem);
            }

            var fn = Expr.attrHandle[name.toLowerCase()],
                // Don't get fooled by Object.prototype properties (jQuery #13807)
                val = fn && hasOwn.call(Expr.attrHandle, name.toLowerCase()) ? fn(elem, name, !documentIsHTML) : undefined;

            return val === undefined ? support.attributes || !documentIsHTML ? elem.getAttribute(name) : (val = elem.getAttributeNode(name)) && val.specified ? val.value : null : val;
        };

        Sizzle.error = function(msg) {
            throw new Error("Syntax error, unrecognized expression: " + msg);
        };

        /**
         * Document sorting and removing duplicates
         * @param {ArrayLike} results
         */
        Sizzle.uniqueSort = function(results) {
            var elem,
            duplicates = [],
                j = 0,
                i = 0;

            // Unless we *know* we can detect duplicates, assume their presence
            hasDuplicate = !support.detectDuplicates;
            sortInput = !support.sortStable && results.slice(0);
            results.sort(sortOrder);

            if (hasDuplicate) {
                while ((elem = results[i++])) {
                    if (elem === results[i]) {
                        j = duplicates.push(i);
                    }
                }
                while (j--) {
                    results.splice(duplicates[j], 1);
                }
            }

            return results;
        };

        /**
         * Utility function for retrieving the text value of an array of DOM nodes
         * @param {Array|Element} elem
         */
        getText = Sizzle.getText = function(elem) {
            var node,
            ret = "",
                i = 0,
                nodeType = elem.nodeType;

            if (!nodeType) {
                // If no nodeType, this is expected to be an array
                for (;
                (node = elem[i]); i++) {
                    // Do not traverse comment nodes
                    ret += getText(node);
                }
            } else if (nodeType === 1 || nodeType === 9 || nodeType === 11) {
                // Use textContent for elements
                // innerText usage removed for consistency of new lines (see #11153)
                if (typeof elem.textContent === "string") {
                    return elem.textContent;
                } else {
                    // Traverse its children
                    for (elem = elem.firstChild; elem; elem = elem.nextSibling) {
                        ret += getText(elem);
                    }
                }
            } else if (nodeType === 3 || nodeType === 4) {
                return elem.nodeValue;
            }
            // Do not include comment or processing instruction nodes

            return ret;
        };

        Expr = Sizzle.selectors = {

            // Can be adjusted by the user
            cacheLength: 50,

            createPseudo: markFunction,

            match: matchExpr,

            attrHandle: {},

            find: {},

            relative: {
                ">": {
                    dir: "parentNode",
                    first: true
                },
                " ": {
                    dir: "parentNode"
                },
                "+": {
                    dir: "previousSibling",
                    first: true
                },
                "~": {
                    dir: "previousSibling"
                }
            },

            preFilter: {
                "ATTR": function(match) {
                    match[1] = match[1].replace(runescape, funescape);

                    // Move the given value to match[3] whether quoted or unquoted
                    match[3] = (match[4] || match[5] || "").replace(runescape, funescape);

                    if (match[2] === "~=") {
                        match[3] = " " + match[3] + " ";
                    }

                    return match.slice(0, 4);
                },

                "CHILD": function(match) {
                    /* matches from matchExpr["CHILD"]
				1 type (only|nth|...)
				2 what (child|of-type)
				3 argument (even|odd|\d*|\d*n([+-]\d+)?|...)
				4 xn-component of xn+y argument ([+-]?\d*n|)
				5 sign of xn-component
				6 x of xn-component
				7 sign of y-component
				8 y of y-component
			*/
                    match[1] = match[1].toLowerCase();

                    if (match[1].slice(0, 3) === "nth") {
                        // nth-* requires argument
                        if (!match[3]) {
                            Sizzle.error(match[0]);
                        }

                        // numeric x and y parameters for Expr.filter.CHILD
                        // remember that false/true cast respectively to 0/1
                        match[4] = +(match[4] ? match[5] + (match[6] || 1) : 2 * (match[3] === "even" || match[3] === "odd"));
                        match[5] = +((match[7] + match[8]) || match[3] === "odd");

                        // other types prohibit arguments
                    } else if (match[3]) {
                        Sizzle.error(match[0]);
                    }

                    return match;
                },

                "PSEUDO": function(match) {
                    var excess,
                    unquoted = !match[5] && match[2];

                    if (matchExpr["CHILD"].test(match[0])) {
                        return null;
                    }

                    // Accept quoted arguments as-is
                    if (match[3] && match[4] !== undefined) {
                        match[2] = match[4];

                        // Strip excess characters from unquoted arguments
                    } else if (unquoted && rpseudo.test(unquoted) &&
                    // Get excess from tokenize (recursively)
                    (excess = tokenize(unquoted, true)) &&
                    // advance to the next closing parenthesis
                    (excess = unquoted.indexOf(")", unquoted.length - excess) - unquoted.length)) {

                        // excess is a negative index
                        match[0] = match[0].slice(0, excess);
                        match[2] = unquoted.slice(0, excess);
                    }

                    // Return only captures needed by the pseudo filter method (type and argument)
                    return match.slice(0, 3);
                }
            },

            filter: {

                "TAG": function(nodeNameSelector) {
                    var nodeName = nodeNameSelector.replace(runescape, funescape).toLowerCase();
                    return nodeNameSelector === "*" ? function() {
                        return true;
                    } : function(elem) {
                        return elem.nodeName && elem.nodeName.toLowerCase() === nodeName;
                    };
                },

                "CLASS": function(className) {
                    var pattern = classCache[className + " "];

                    return pattern || (pattern = new RegExp("(^|" + whitespace + ")" + className + "(" + whitespace + "|$)")) && classCache(className, function(elem) {
                        return pattern.test(typeof elem.className === "string" && elem.className || typeof elem.getAttribute !== strundefined && elem.getAttribute("class") || "");
                    });
                },

                "ATTR": function(name, operator, check) {
                    return function(elem) {
                        var result = Sizzle.attr(elem, name);

                        if (result == null) {
                            return operator === "!=";
                        }
                        if (!operator) {
                            return true;
                        }

                        result += "";

                        return operator === "=" ? result === check : operator === "!=" ? result !== check : operator === "^=" ? check && result.indexOf(check) === 0 : operator === "*=" ? check && result.indexOf(check) > -1 : operator === "$=" ? check && result.slice(-check.length) === check : operator === "~=" ? (" " + result + " ").indexOf(check) > -1 : operator === "|=" ? result === check || result.slice(0, check.length + 1) === check + "-" : false;
                    };
                },

                "CHILD": function(type, what, argument, first, last) {
                    var simple = type.slice(0, 3) !== "nth",
                        forward = type.slice(-4) !== "last",
                        ofType = what === "of-type";

                    return first === 1 && last === 0 ?

                    // Shortcut for :nth-*(n)
                    function(elem) {
                        return !!elem.parentNode;
                    } :

                    function(elem, context, xml) {
                        var cache, outerCache, node, diff, nodeIndex, start,
                        dir = simple !== forward ? "nextSibling" : "previousSibling",
                            parent = elem.parentNode,
                            name = ofType && elem.nodeName.toLowerCase(),
                            useCache = !xml && !ofType;

                        if (parent) {

                            // :(first|last|only)-(child|of-type)
                            if (simple) {
                                while (dir) {
                                    node = elem;
                                    while ((node = node[dir])) {
                                        if (ofType ? node.nodeName.toLowerCase() === name : node.nodeType === 1) {
                                            return false;
                                        }
                                    }
                                    // Reverse direction for :only-* (if we haven't yet done so)
                                    start = dir = type === "only" && !start && "nextSibling";
                                }
                                return true;
                            }

                            start = [forward ? parent.firstChild : parent.lastChild];

                            // non-xml :nth-child(...) stores cache data on `parent`
                            if (forward && useCache) {
                                // Seek `elem` from a previously-cached index
                                outerCache = parent[expando] || (parent[expando] = {});
                                cache = outerCache[type] || [];
                                nodeIndex = cache[0] === dirruns && cache[1];
                                diff = cache[0] === dirruns && cache[2];
                                node = nodeIndex && parent.childNodes[nodeIndex];

                                while ((node = ++nodeIndex && node && node[dir] ||

                                // Fallback to seeking `elem` from the start
                                (diff = nodeIndex = 0) || start.pop())) {

                                    // When found, cache indexes on `parent` and break
                                    if (node.nodeType === 1 && ++diff && node === elem) {
                                        outerCache[type] = [dirruns, nodeIndex, diff];
                                        break;
                                    }
                                }

                                // Use previously-cached element index if available
                            } else if (useCache && (cache = (elem[expando] || (elem[expando] = {}))[type]) && cache[0] === dirruns) {
                                diff = cache[1];

                                // xml :nth-child(...) or :nth-last-child(...) or :nth(-last)?-of-type(...)
                            } else {
                                // Use the same loop as above to seek `elem` from the start
                                while ((node = ++nodeIndex && node && node[dir] || (diff = nodeIndex = 0) || start.pop())) {

                                    if ((ofType ? node.nodeName.toLowerCase() === name : node.nodeType === 1) && ++diff) {
                                        // Cache the index of each encountered element
                                        if (useCache) {
                                            (node[expando] || (node[expando] = {}))[type] = [dirruns, diff];
                                        }

                                        if (node === elem) {
                                            break;
                                        }
                                    }
                                }
                            }

                            // Incorporate the offset, then check against cycle size
                            diff -= last;
                            return diff === first || (diff % first === 0 && diff / first >= 0);
                        }
                    };
                },

                "PSEUDO": function(pseudo, argument) {
                    // pseudo-class names are case-insensitive
                    // http://www.w3.org/TR/selectors/#pseudo-classes
                    // Prioritize by case sensitivity in case custom pseudos are added with uppercase letters
                    // Remember that setFilters inherits from pseudos
                    var args,
                    fn = Expr.pseudos[pseudo] || Expr.setFilters[pseudo.toLowerCase()] || Sizzle.error("unsupported pseudo: " + pseudo);

                    // The user may use createPseudo to indicate that
                    // arguments are needed to create the filter function
                    // just as Sizzle does
                    if (fn[expando]) {
                        return fn(argument);
                    }

                    // But maintain support for old signatures
                    if (fn.length > 1) {
                        args = [pseudo, pseudo, "", argument];
                        return Expr.setFilters.hasOwnProperty(pseudo.toLowerCase()) ? markFunction(function(seed, matches) {
                            var idx,
                            matched = fn(seed, argument),
                                i = matched.length;
                            while (i--) {
                                idx = indexOf.call(seed, matched[i]);
                                seed[idx] = !(matches[idx] = matched[i]);
                            }
                        }) : function(elem) {
                            return fn(elem, 0, args);
                        };
                    }

                    return fn;
                }
            },

            pseudos: {
                // Potentially complex pseudos
                "not": markFunction(function(selector) {
                    // Trim the selector passed to compile
                    // to avoid treating leading and trailing
                    // spaces as combinators
                    var input = [],
                        results = [],
                        matcher = compile(selector.replace(rtrim, "$1"));

                    return matcher[expando] ? markFunction(function(seed, matches, context, xml) {
                        var elem,
                        unmatched = matcher(seed, null, xml, []),
                            i = seed.length;

                        // Match elements unmatched by `matcher`
                        while (i--) {
                            if ((elem = unmatched[i])) {
                                seed[i] = !(matches[i] = elem);
                            }
                        }
                    }) : function(elem, context, xml) {
                        input[0] = elem;
                        matcher(input, null, xml, results);
                        return !results.pop();
                    };
                }),

                "has": markFunction(function(selector) {
                    return function(elem) {
                        return Sizzle(selector, elem).length > 0;
                    };
                }),

                "contains": markFunction(function(text) {
                    return function(elem) {
                        return (elem.textContent || elem.innerText || getText(elem)).indexOf(text) > -1;
                    };
                }),

                // "Whether an element is represented by a :lang() selector
                // is based solely on the element's language value
                // being equal to the identifier C,
                // or beginning with the identifier C immediately followed by "-".
                // The matching of C against the element's language value is performed case-insensitively.
                // The identifier C does not have to be a valid language name."
                // http://www.w3.org/TR/selectors/#lang-pseudo
                "lang": markFunction(function(lang) {
                    // lang value must be a valid identifier
                    if (!ridentifier.test(lang || "")) {
                        Sizzle.error("unsupported lang: " + lang);
                    }
                    lang = lang.replace(runescape, funescape).toLowerCase();
                    return function(elem) {
                        var elemLang;
                        do {
                            if ((elemLang = documentIsHTML ? elem.lang : elem.getAttribute("xml:lang") || elem.getAttribute("lang"))) {

                                elemLang = elemLang.toLowerCase();
                                return elemLang === lang || elemLang.indexOf(lang + "-") === 0;
                            }
                        } while ((elem = elem.parentNode) && elem.nodeType === 1);
                        return false;
                    };
                }),

                // Miscellaneous
                "target": function(elem) {
                    var hash = window.location && window.location.hash;
                    return hash && hash.slice(1) === elem.id;
                },

                "root": function(elem) {
                    return elem === docElem;
                },

                "focus": function(elem) {
                    return elem === document.activeElement && (!document.hasFocus || document.hasFocus()) && !! (elem.type || elem.href || ~elem.tabIndex);
                },

                // Boolean properties
                "enabled": function(elem) {
                    return elem.disabled === false;
                },

                "disabled": function(elem) {
                    return elem.disabled === true;
                },

                "checked": function(elem) {
                    // In CSS3, :checked should return both checked and selected elements
                    // http://www.w3.org/TR/2011/REC-css3-selectors-20110929/#checked
                    var nodeName = elem.nodeName.toLowerCase();
                    return (nodeName === "input" && !! elem.checked) || (nodeName === "option" && !! elem.selected);
                },

                "selected": function(elem) {
                    // Accessing this property makes selected-by-default
                    // options in Safari work properly
                    if (elem.parentNode) {
                        elem.parentNode.selectedIndex;
                    }

                    return elem.selected === true;
                },

                // Contents
                "empty": function(elem) {
                    // http://www.w3.org/TR/selectors/#empty-pseudo
                    // :empty is only affected by element nodes and content nodes(including text(3), cdata(4)),
                    //   not comment, processing instructions, or others
                    // Thanks to Diego Perini for the nodeName shortcut
                    //   Greater than "@" means alpha characters (specifically not starting with "#" or "?")
                    for (elem = elem.firstChild; elem; elem = elem.nextSibling) {
                        if (elem.nodeName > "@" || elem.nodeType === 3 || elem.nodeType === 4) {
                            return false;
                        }
                    }
                    return true;
                },

                "parent": function(elem) {
                    return !Expr.pseudos["empty"](elem);
                },

                // Element/input types
                "header": function(elem) {
                    return rheader.test(elem.nodeName);
                },

                "input": function(elem) {
                    return rinputs.test(elem.nodeName);
                },

                "button": function(elem) {
                    var name = elem.nodeName.toLowerCase();
                    return name === "input" && elem.type === "button" || name === "button";
                },

                "text": function(elem) {
                    var attr;
                    // IE6 and 7 will map elem.type to 'text' for new HTML5 types (search, etc)
                    // use getAttribute instead to test this case
                    return elem.nodeName.toLowerCase() === "input" && elem.type === "text" && ((attr = elem.getAttribute("type")) == null || attr.toLowerCase() === elem.type);
                },

                // Position-in-collection
                "first": createPositionalPseudo(function() {
                    return [0];
                }),

                "last": createPositionalPseudo(function(matchIndexes, length) {
                    return [length - 1];
                }),

                "eq": createPositionalPseudo(function(matchIndexes, length, argument) {
                    return [argument < 0 ? argument + length : argument];
                }),

                "even": createPositionalPseudo(function(matchIndexes, length) {
                    var i = 0;
                    for (; i < length; i += 2) {
                        matchIndexes.push(i);
                    }
                    return matchIndexes;
                }),

                "odd": createPositionalPseudo(function(matchIndexes, length) {
                    var i = 1;
                    for (; i < length; i += 2) {
                        matchIndexes.push(i);
                    }
                    return matchIndexes;
                }),

                "lt": createPositionalPseudo(function(matchIndexes, length, argument) {
                    var i = argument < 0 ? argument + length : argument;
                    for (; --i >= 0;) {
                        matchIndexes.push(i);
                    }
                    return matchIndexes;
                }),

                "gt": createPositionalPseudo(function(matchIndexes, length, argument) {
                    var i = argument < 0 ? argument + length : argument;
                    for (; ++i < length;) {
                        matchIndexes.push(i);
                    }
                    return matchIndexes;
                })
            }
        };

        Expr.pseudos["nth"] = Expr.pseudos["eq"];

        // Add button/input type pseudos
        for (i in {
            radio: true,
            checkbox: true,
            file: true,
            password: true,
            image: true
        }) {
            Expr.pseudos[i] = createInputPseudo(i);
        }
        for (i in {
            submit: true,
            reset: true
        }) {
            Expr.pseudos[i] = createButtonPseudo(i);
        }

        // Easy API for creating new setFilters
        function setFilters() {}
        setFilters.prototype = Expr.filters = Expr.pseudos;
        Expr.setFilters = new setFilters();

        function tokenize(selector, parseOnly) {
            var matched, match, tokens, type,
            soFar, groups, preFilters,
            cached = tokenCache[selector + " "];

            if (cached) {
                return parseOnly ? 0 : cached.slice(0);
            }

            soFar = selector;
            groups = [];
            preFilters = Expr.preFilter;

            while (soFar) {

                // Comma and first run
                if (!matched || (match = rcomma.exec(soFar))) {
                    if (match) {
                        // Don't consume trailing commas as valid
                        soFar = soFar.slice(match[0].length) || soFar;
                    }
                    groups.push(tokens = []);
                }

                matched = false;

                // Combinators
                if ((match = rcombinators.exec(soFar))) {
                    matched = match.shift();
                    tokens.push({
                        value: matched,
                        // Cast descendant combinators to space
                        type: match[0].replace(rtrim, " ")
                    });
                    soFar = soFar.slice(matched.length);
                }

                // Filters
                for (type in Expr.filter) {
                    if ((match = matchExpr[type].exec(soFar)) && (!preFilters[type] || (match = preFilters[type](match)))) {
                        matched = match.shift();
                        tokens.push({
                            value: matched,
                            type: type,
                            matches: match
                        });
                        soFar = soFar.slice(matched.length);
                    }
                }

                if (!matched) {
                    break;
                }
            }

            // Return the length of the invalid excess
            // if we're just parsing
            // Otherwise, throw an error or return tokens
            return parseOnly ? soFar.length : soFar ? Sizzle.error(selector) :
            // Cache the tokens
            tokenCache(selector, groups).slice(0);
        }

        function toSelector(tokens) {
            var i = 0,
                len = tokens.length,
                selector = "";
            for (; i < len; i++) {
                selector += tokens[i].value;
            }
            return selector;
        }

        function addCombinator(matcher, combinator, base) {
            var dir = combinator.dir,
                checkNonElements = base && dir === "parentNode",
                doneName = done++;

            return combinator.first ?
            // Check against closest ancestor/preceding element
            function(elem, context, xml) {
                while ((elem = elem[dir])) {
                    if (elem.nodeType === 1 || checkNonElements) {
                        return matcher(elem, context, xml);
                    }
                }
            } :

            // Check against all ancestor/preceding elements
            function(elem, context, xml) {
                var data, cache, outerCache,
                dirkey = dirruns + " " + doneName;

                // We can't set arbitrary data on XML nodes, so they don't benefit from dir caching
                if (xml) {
                    while ((elem = elem[dir])) {
                        if (elem.nodeType === 1 || checkNonElements) {
                            if (matcher(elem, context, xml)) {
                                return true;
                            }
                        }
                    }
                } else {
                    while ((elem = elem[dir])) {
                        if (elem.nodeType === 1 || checkNonElements) {
                            outerCache = elem[expando] || (elem[expando] = {});
                            if ((cache = outerCache[dir]) && cache[0] === dirkey) {
                                if ((data = cache[1]) === true || data === cachedruns) {
                                    return data === true;
                                }
                            } else {
                                cache = outerCache[dir] = [dirkey];
                                cache[1] = matcher(elem, context, xml) || cachedruns;
                                if (cache[1] === true) {
                                    return true;
                                }
                            }
                        }
                    }
                }
            };
        }

        function elementMatcher(matchers) {
            return matchers.length > 1 ? function(elem, context, xml) {
                var i = matchers.length;
                while (i--) {
                    if (!matchers[i](elem, context, xml)) {
                        return false;
                    }
                }
                return true;
            } : matchers[0];
        }

        function condense(unmatched, map, filter, context, xml) {
            var elem,
            newUnmatched = [],
                i = 0,
                len = unmatched.length,
                mapped = map != null;

            for (; i < len; i++) {
                if ((elem = unmatched[i])) {
                    if (!filter || filter(elem, context, xml)) {
                        newUnmatched.push(elem);
                        if (mapped) {
                            map.push(i);
                        }
                    }
                }
            }

            return newUnmatched;
        }

        function setMatcher(preFilter, selector, matcher, postFilter, postFinder, postSelector) {
            if (postFilter && !postFilter[expando]) {
                postFilter = setMatcher(postFilter);
            }
            if (postFinder && !postFinder[expando]) {
                postFinder = setMatcher(postFinder, postSelector);
            }
            return markFunction(function(seed, results, context, xml) {
                var temp, i, elem,
                preMap = [],
                    postMap = [],
                    preexisting = results.length,

                    // Get initial elements from seed or context
                    elems = seed || multipleContexts(selector || "*", context.nodeType ? [context] : context, []),

                    // Prefilter to get matcher input, preserving a map for seed-results synchronization
                    matcherIn = preFilter && (seed || !selector) ? condense(elems, preMap, preFilter, context, xml) : elems,

                    matcherOut = matcher ?
                    // If we have a postFinder, or filtered seed, or non-seed postFilter or preexisting results,
                    postFinder || (seed ? preFilter : preexisting || postFilter) ?

                    // ...intermediate processing is necessary
                    [] :

                    // ...otherwise use results directly
                    results : matcherIn;

                // Find primary matches
                if (matcher) {
                    matcher(matcherIn, matcherOut, context, xml);
                }

                // Apply postFilter
                if (postFilter) {
                    temp = condense(matcherOut, postMap);
                    postFilter(temp, [], context, xml);

                    // Un-match failing elements by moving them back to matcherIn
                    i = temp.length;
                    while (i--) {
                        if ((elem = temp[i])) {
                            matcherOut[postMap[i]] = !(matcherIn[postMap[i]] = elem);
                        }
                    }
                }

                if (seed) {
                    if (postFinder || preFilter) {
                        if (postFinder) {
                            // Get the final matcherOut by condensing this intermediate into postFinder contexts
                            temp = [];
                            i = matcherOut.length;
                            while (i--) {
                                if ((elem = matcherOut[i])) {
                                    // Restore matcherIn since elem is not yet a final match
                                    temp.push((matcherIn[i] = elem));
                                }
                            }
                            postFinder(null, (matcherOut = []), temp, xml);
                        }

                        // Move matched elements from seed to results to keep them synchronized
                        i = matcherOut.length;
                        while (i--) {
                            if ((elem = matcherOut[i]) && (temp = postFinder ? indexOf.call(seed, elem) : preMap[i]) > -1) {

                                seed[temp] = !(results[temp] = elem);
                            }
                        }
                    }

                    // Add elements to results, through postFinder if defined
                } else {
                    matcherOut = condense(
                    matcherOut === results ? matcherOut.splice(preexisting, matcherOut.length) : matcherOut);
                    if (postFinder) {
                        postFinder(null, results, matcherOut, xml);
                    } else {
                        push.apply(results, matcherOut);
                    }
                }
            });
        }

        function matcherFromTokens(tokens) {
            var checkContext, matcher, j,
            len = tokens.length,
                leadingRelative = Expr.relative[tokens[0].type],
                implicitRelative = leadingRelative || Expr.relative[" "],
                i = leadingRelative ? 1 : 0,

                // The foundational matcher ensures that elements are reachable from top-level context(s)
                matchContext = addCombinator(function(elem) {
                    return elem === checkContext;
                }, implicitRelative, true),
                matchAnyContext = addCombinator(function(elem) {
                    return indexOf.call(checkContext, elem) > -1;
                }, implicitRelative, true),
                matchers = [function(elem, context, xml) {
                    return (!leadingRelative && (xml || context !== outermostContext)) || (
                    (checkContext = context).nodeType ? matchContext(elem, context, xml) : matchAnyContext(elem, context, xml));
                }];

            for (; i < len; i++) {
                if ((matcher = Expr.relative[tokens[i].type])) {
                    matchers = [addCombinator(elementMatcher(matchers), matcher)];
                } else {
                    matcher = Expr.filter[tokens[i].type].apply(null, tokens[i].matches);

                    // Return special upon seeing a positional matcher
                    if (matcher[expando]) {
                        // Find the next relative operator (if any) for proper handling
                        j = ++i;
                        for (; j < len; j++) {
                            if (Expr.relative[tokens[j].type]) {
                                break;
                            }
                        }
                        return setMatcher(
                        i > 1 && elementMatcher(matchers),
                        i > 1 && toSelector(
                        // If the preceding token was a descendant combinator, insert an implicit any-element `*`
                        tokens.slice(0, i - 1).concat({
                            value: tokens[i - 2].type === " " ? "*" : ""
                        })).replace(rtrim, "$1"),
                        matcher,
                        i < j && matcherFromTokens(tokens.slice(i, j)),
                        j < len && matcherFromTokens((tokens = tokens.slice(j))),
                        j < len && toSelector(tokens));
                    }
                    matchers.push(matcher);
                }
            }

            return elementMatcher(matchers);
        }

        function matcherFromGroupMatchers(elementMatchers, setMatchers) {
            // A counter to specify which element is currently being matched
            var matcherCachedRuns = 0,
                bySet = setMatchers.length > 0,
                byElement = elementMatchers.length > 0,
                superMatcher = function(seed, context, xml, results, expandContext) {
                    var elem, j, matcher,
                    setMatched = [],
                        matchedCount = 0,
                        i = "0",
                        unmatched = seed && [],
                        outermost = expandContext != null,
                        contextBackup = outermostContext,
                        // We must always have either seed elements or context
                        elems = seed || byElement && Expr.find["TAG"]("*", expandContext && context.parentNode || context),
                        // Use integer dirruns iff this is the outermost matcher
                        dirrunsUnique = (dirruns += contextBackup == null ? 1 : Math.random() || 0.1);

                    if (outermost) {
                        outermostContext = context !== document && context;
                        cachedruns = matcherCachedRuns;
                    }

                    // Add elements passing elementMatchers directly to results
                    // Keep `i` a string if there are no elements so `matchedCount` will be "00" below
                    for (;
                    (elem = elems[i]) != null; i++) {
                        if (byElement && elem) {
                            j = 0;
                            while ((matcher = elementMatchers[j++])) {
                                if (matcher(elem, context, xml)) {
                                    results.push(elem);
                                    break;
                                }
                            }
                            if (outermost) {
                                dirruns = dirrunsUnique;
                                cachedruns = ++matcherCachedRuns;
                            }
                        }

                        // Track unmatched elements for set filters
                        if (bySet) {
                            // They will have gone through all possible matchers
                            if ((elem = !matcher && elem)) {
                                matchedCount--;
                            }

                            // Lengthen the array for every element, matched or not
                            if (seed) {
                                unmatched.push(elem);
                            }
                        }
                    }

                    // Apply set filters to unmatched elements
                    matchedCount += i;
                    if (bySet && i !== matchedCount) {
                        j = 0;
                        while ((matcher = setMatchers[j++])) {
                            matcher(unmatched, setMatched, context, xml);
                        }

                        if (seed) {
                            // Reintegrate element matches to eliminate the need for sorting
                            if (matchedCount > 0) {
                                while (i--) {
                                    if (!(unmatched[i] || setMatched[i])) {
                                        setMatched[i] = pop.call(results);
                                    }
                                }
                            }

                            // Discard index placeholder values to get only actual matches
                            setMatched = condense(setMatched);
                        }

                        // Add matches to results
                        push.apply(results, setMatched);

                        // Seedless set matches succeeding multiple successful matchers stipulate sorting
                        if (outermost && !seed && setMatched.length > 0 && (matchedCount + setMatchers.length) > 1) {

                            Sizzle.uniqueSort(results);
                        }
                    }

                    // Override manipulation of globals by nested matchers
                    if (outermost) {
                        dirruns = dirrunsUnique;
                        outermostContext = contextBackup;
                    }

                    return unmatched;
                };

            return bySet ? markFunction(superMatcher) : superMatcher;
        }

        compile = Sizzle.compile = function(selector, group /* Internal Use Only */ ) {
            var i,
            setMatchers = [],
                elementMatchers = [],
                cached = compilerCache[selector + " "];

            if (!cached) {
                // Generate a function of recursive functions that can be used to check each element
                if (!group) {
                    group = tokenize(selector);
                }
                i = group.length;
                while (i--) {
                    cached = matcherFromTokens(group[i]);
                    if (cached[expando]) {
                        setMatchers.push(cached);
                    } else {
                        elementMatchers.push(cached);
                    }
                }

                // Cache the compiled function
                cached = compilerCache(selector, matcherFromGroupMatchers(elementMatchers, setMatchers));
            }
            return cached;
        };

        function multipleContexts(selector, contexts, results) {
            var i = 0,
                len = contexts.length;
            for (; i < len; i++) {
                Sizzle(selector, contexts[i], results);
            }
            return results;
        }

        function select(selector, context, results, seed) {
            var i, tokens, token, type, find,
            match = tokenize(selector);

            if (!seed) {
                // Try to minimize operations if there is only one group
                if (match.length === 1) {

                    // Take a shortcut and set the context if the root selector is an ID
                    tokens = match[0] = match[0].slice(0);
                    if (tokens.length > 2 && (token = tokens[0]).type === "ID" && support.getById && context.nodeType === 9 && documentIsHTML && Expr.relative[tokens[1].type]) {

                        context = (Expr.find["ID"](token.matches[0].replace(runescape, funescape), context) || [])[0];
                        if (!context) {
                            return results;
                        }
                        selector = selector.slice(tokens.shift().value.length);
                    }

                    // Fetch a seed set for right-to-left matching
                    i = matchExpr["needsContext"].test(selector) ? 0 : tokens.length;
                    while (i--) {
                        token = tokens[i];

                        // Abort if we hit a combinator
                        if (Expr.relative[(type = token.type)]) {
                            break;
                        }
                        if ((find = Expr.find[type])) {
                            // Search, expanding context for leading sibling combinators
                            if ((seed = find(
                            token.matches[0].replace(runescape, funescape),
                            rsibling.test(tokens[0].type) && context.parentNode || context))) {

                                // If seed is empty or no tokens remain, we can return early
                                tokens.splice(i, 1);
                                selector = seed.length && toSelector(tokens);
                                if (!selector) {
                                    push.apply(results, seed);
                                    return results;
                                }

                                break;
                            }
                        }
                    }
                }
            }

            // Compile and execute a filtering function
            // Provide `match` to avoid retokenization if we modified the selector above
            compile(selector, match)(
            seed,
            context, !documentIsHTML,
            results,
            rsibling.test(selector));
            return results;
        }

        // One-time assignments

        // Sort stability
        support.sortStable = expando.split("").sort(sortOrder).join("") === expando;

        // Support: Chrome<14
        // Always assume duplicates if they aren't passed to the comparison function
        support.detectDuplicates = hasDuplicate;

        // Initialize against the default document
        setDocument();

        // Support: Webkit<537.32 - Safari 6.0.3/Chrome 25 (fixed in Chrome 27)
        // Detached nodes confoundingly follow *each other*
        support.sortDetached = assert(function(div1) {
            // Should return 1, but returns 4 (following)
            return div1.compareDocumentPosition(document.createElement("div")) & 1;
        });

        // Support: IE<8
        // Prevent attribute/property "interpolation"
        // http://msdn.microsoft.com/en-us/library/ms536429%28VS.85%29.aspx
        if (!assert(function(div) {
            div.innerHTML = "<a href='#'></a>";
            return div.firstChild.getAttribute("href") === "#";
        })) {
            addHandle("type|href|height|width", function(elem, name, isXML) {
                if (!isXML) {
                    return elem.getAttribute(name, name.toLowerCase() === "type" ? 1 : 2);
                }
            });
        }

        // Support: IE<9
        // Use defaultValue in place of getAttribute("value")
        if (!support.attributes || !assert(function(div) {
            div.innerHTML = "<input/>";
            div.firstChild.setAttribute("value", "");
            return div.firstChild.getAttribute("value") === "";
        })) {
            addHandle("value", function(elem, name, isXML) {
                if (!isXML && elem.nodeName.toLowerCase() === "input") {
                    return elem.defaultValue;
                }
            });
        }

        // Support: IE<9
        // Use getAttributeNode to fetch booleans when getAttribute lies
        if (!assert(function(div) {
            return div.getAttribute("disabled") == null;
        })) {
            addHandle(booleans, function(elem, name, isXML) {
                var val;
                if (!isXML) {
                    return (val = elem.getAttributeNode(name)) && val.specified ? val.value : elem[name] === true ? name.toLowerCase() : null;
                }
            });
        }

        jQuery.find = Sizzle;
        jQuery.expr = Sizzle.selectors;
        jQuery.expr[":"] = jQuery.expr.pseudos;
        jQuery.unique = Sizzle.uniqueSort;
        jQuery.text = Sizzle.getText;
        jQuery.isXMLDoc = Sizzle.isXML;
        jQuery.contains = Sizzle.contains;


    })(window);
    // String to Object options format cache
    var optionsCache = {};

    // Convert String-formatted options into Object-formatted ones and store in cache
    function createOptions(options) {
        var object = optionsCache[options] = {};
        jQuery.each(options.match(core_rnotwhite) || [], function(_, flag) {
            object[flag] = true;
        });
        return object;
    }

    /*
     * Create a callback list using the following parameters:
     *
     *	options: an optional list of space-separated options that will change how
     *			the callback list behaves or a more traditional option object
     *
     * By default a callback list will act like an event callback list and can be
     * "fired" multiple times.
     *
     * Possible options:
     *
     *	once:			will ensure the callback list can only be fired once (like a Deferred)
     *
     *	memory:			will keep track of previous values and will call any callback added
     *					after the list has been fired right away with the latest "memorized"
     *					values (like a Deferred)
     *
     *	unique:			will ensure a callback can only be added once (no duplicate in the list)
     *
     *	stopOnFalse:	interrupt callings when a callback returns false
     *
     */
    jQuery.Callbacks = function(options) {

        // Convert options from String-formatted to Object-formatted if needed
        // (we check in cache first)
        options = typeof options === "string" ? (optionsCache[options] || createOptions(options)) : jQuery.extend({}, options);

        var // Last fire value (for non-forgettable lists)
        memory,
        // Flag to know if list was already fired
        fired,
        // Flag to know if list is currently firing
        firing,
        // First callback to fire (used internally by add and fireWith)
        firingStart,
        // End of the loop when firing
        firingLength,
        // Index of currently firing callback (modified by remove if needed)
        firingIndex,
        // Actual callback list
        list = [],
            // Stack of fire calls for repeatable lists
            stack = !options.once && [],
            // Fire callbacks
            fire = function(data) {
                memory = options.memory && data;
                fired = true;
                firingIndex = firingStart || 0;
                firingStart = 0;
                firingLength = list.length;
                firing = true;
                for (; list && firingIndex < firingLength; firingIndex++) {
                    if (list[firingIndex].apply(data[0], data[1]) === false && options.stopOnFalse) {
                        memory = false; // To prevent further calls using add
                        break;
                    }
                }
                firing = false;
                if (list) {
                    if (stack) {
                        if (stack.length) {
                            fire(stack.shift());
                        }
                    } else if (memory) {
                        list = [];
                    } else {
                        self.disable();
                    }
                }
            },
            // Actual Callbacks object
            self = {
                // Add a callback or a collection of callbacks to the list
                add: function() {
                    if (list) {
                        // First, we save the current length
                        var start = list.length;
                        (function add(args) {
                            jQuery.each(args, function(_, arg) {
                                var type = jQuery.type(arg);
                                if (type === "function") {
                                    if (!options.unique || !self.has(arg)) {
                                        list.push(arg);
                                    }
                                } else if (arg && arg.length && type !== "string") {
                                    // Inspect recursively
                                    add(arg);
                                }
                            });
                        })(arguments);
                        // Do we need to add the callbacks to the
                        // current firing batch?
                        if (firing) {
                            firingLength = list.length;
                            // With memory, if we're not firing then
                            // we should call right away
                        } else if (memory) {
                            firingStart = start;
                            fire(memory);
                        }
                    }
                    return this;
                },
                // Remove a callback from the list
                remove: function() {
                    if (list) {
                        jQuery.each(arguments, function(_, arg) {
                            var index;
                            while ((index = jQuery.inArray(arg, list, index)) > -1) {
                                list.splice(index, 1);
                                // Handle firing indexes
                                if (firing) {
                                    if (index <= firingLength) {
                                        firingLength--;
                                    }
                                    if (index <= firingIndex) {
                                        firingIndex--;
                                    }
                                }
                            }
                        });
                    }
                    return this;
                },
                // Check if a given callback is in the list.
                // If no argument is given, return whether or not list has callbacks attached.
                has: function(fn) {
                    return fn ? jQuery.inArray(fn, list) > -1 : !! (list && list.length);
                },
                // Remove all callbacks from the list
                empty: function() {
                    list = [];
                    firingLength = 0;
                    return this;
                },
                // Have the list do nothing anymore
                disable: function() {
                    list = stack = memory = undefined;
                    return this;
                },
                // Is it disabled?
                disabled: function() {
                    return !list;
                },
                // Lock the list in its current state
                lock: function() {
                    stack = undefined;
                    if (!memory) {
                        self.disable();
                    }
                    return this;
                },
                // Is it locked?
                locked: function() {
                    return !stack;
                },
                // Call all callbacks with the given context and arguments
                fireWith: function(context, args) {
                    if (list && (!fired || stack)) {
                        args = args || [];
                        args = [context, args.slice ? args.slice() : args];
                        if (firing) {
                            stack.push(args);
                        } else {
                            fire(args);
                        }
                    }
                    return this;
                },
                // Call all the callbacks with the given arguments
                fire: function() {
                    self.fireWith(this, arguments);
                    return this;
                },
                // To know if the callbacks have already been called at least once
                fired: function() {
                    return !!fired;
                }
            };

        return self;
    };
    jQuery.extend({

        Deferred: function(func) {
            var tuples = [
            // action, add listener, listener list, final state
            ["resolve", "done", jQuery.Callbacks("once memory"), "resolved"],
                ["reject", "fail", jQuery.Callbacks("once memory"), "rejected"],
                ["notify", "progress", jQuery.Callbacks("memory")]
            ],
                state = "pending",
                promise = {
                    state: function() {
                        return state;
                    },
                    always: function() {
                        deferred.done(arguments).fail(arguments);
                        return this;
                    },
                    then: function( /* fnDone, fnFail, fnProgress */ ) {
                        var fns = arguments;
                        return jQuery.Deferred(function(newDefer) {
                            jQuery.each(tuples, function(i, tuple) {
                                var action = tuple[0],
                                    fn = jQuery.isFunction(fns[i]) && fns[i];
                                // deferred[ done | fail | progress ] for forwarding actions to newDefer
                                deferred[tuple[1]](function() {
                                    var returned = fn && fn.apply(this, arguments);
                                    if (returned && jQuery.isFunction(returned.promise)) {
                                        returned.promise()
                                            .done(newDefer.resolve)
                                            .fail(newDefer.reject)
                                            .progress(newDefer.notify);
                                    } else {
                                        newDefer[action + "With"](this === promise ? newDefer.promise() : this, fn ? [returned] : arguments);
                                    }
                                });
                            });
                            fns = null;
                        }).promise();
                    },
                    // Get a promise for this deferred
                    // If obj is provided, the promise aspect is added to the object
                    promise: function(obj) {
                        return obj != null ? jQuery.extend(obj, promise) : promise;
                    }
                },
                deferred = {};

            // Keep pipe for back-compat
            promise.pipe = promise.then;

            // Add list-specific methods
            jQuery.each(tuples, function(i, tuple) {
                var list = tuple[2],
                    stateString = tuple[3];

                // promise[ done | fail | progress ] = list.add
                promise[tuple[1]] = list.add;

                // Handle state
                if (stateString) {
                    list.add(function() {
                        // state = [ resolved | rejected ]
                        state = stateString;

                        // [ reject_list | resolve_list ].disable; progress_list.lock
                    }, tuples[i ^ 1][2].disable, tuples[2][2].lock);
                }

                // deferred[ resolve | reject | notify ]
                deferred[tuple[0]] = function() {
                    deferred[tuple[0] + "With"](this === deferred ? promise : this, arguments);
                    return this;
                };
                deferred[tuple[0] + "With"] = list.fireWith;
            });

            // Make the deferred a promise
            promise.promise(deferred);

            // Call given func if any
            if (func) {
                func.call(deferred, deferred);
            }

            // All done!
            return deferred;
        },

        // Deferred helper
        when: function(subordinate /* , ..., subordinateN */ ) {
            var i = 0,
                resolveValues = core_slice.call(arguments),
                length = resolveValues.length,

                // the count of uncompleted subordinates
                remaining = length !== 1 || (subordinate && jQuery.isFunction(subordinate.promise)) ? length : 0,

                // the master Deferred. If resolveValues consist of only a single Deferred, just use that.
                deferred = remaining === 1 ? subordinate : jQuery.Deferred(),

                // Update function for both resolve and progress values
                updateFunc = function(i, contexts, values) {
                    return function(value) {
                        contexts[i] = this;
                        values[i] = arguments.length > 1 ? core_slice.call(arguments) : value;
                        if (values === progressValues) {
                            deferred.notifyWith(contexts, values);
                        } else if (!(--remaining)) {
                            deferred.resolveWith(contexts, values);
                        }
                    };
                },

                progressValues, progressContexts, resolveContexts;

            // add listeners to Deferred subordinates; treat others as resolved
            if (length > 1) {
                progressValues = new Array(length);
                progressContexts = new Array(length);
                resolveContexts = new Array(length);
                for (; i < length; i++) {
                    if (resolveValues[i] && jQuery.isFunction(resolveValues[i].promise)) {
                        resolveValues[i].promise()
                            .done(updateFunc(i, resolveContexts, resolveValues))
                            .fail(deferred.reject)
                            .progress(updateFunc(i, progressContexts, progressValues));
                    } else {
                        --remaining;
                    }
                }
            }

            // if we're not waiting on anything, resolve the master
            if (!remaining) {
                deferred.resolveWith(resolveContexts, resolveValues);
            }

            return deferred.promise();
        }
    });
    jQuery.support = (function(support) {
        var input = document.createElement("input"),
            fragment = document.createDocumentFragment(),
            div = document.createElement("div"),
            select = document.createElement("select"),
            opt = select.appendChild(document.createElement("option"));

        // Finish early in limited environments
        if (!input.type) {
            return support;
        }

        input.type = "checkbox";

        // Support: Safari 5.1, iOS 5.1, Android 4.x, Android 2.3
        // Check the default checkbox/radio value ("" on old WebKit; "on" elsewhere)
        support.checkOn = input.value !== "";

        // Must access the parent to make an option select properly
        // Support: IE9, IE10
        support.optSelected = opt.selected;

        // Will be defined later
        support.reliableMarginRight = true;
        support.boxSizingReliable = true;
        support.pixelPosition = false;

        // Make sure checked status is properly cloned
        // Support: IE9, IE10
        input.checked = true;
        support.noCloneChecked = input.cloneNode(true).checked;

        // Make sure that the options inside disabled selects aren't marked as disabled
        // (WebKit marks them as disabled)
        select.disabled = true;
        support.optDisabled = !opt.disabled;

        // Check if an input maintains its value after becoming a radio
        // Support: IE9, IE10
        input = document.createElement("input");
        input.value = "t";
        input.type = "radio";
        support.radioValue = input.value === "t";

        // #11217 - WebKit loses check when the name is after the checked attribute
        input.setAttribute("checked", "t");
        input.setAttribute("name", "t");

        fragment.appendChild(input);

        // Support: Safari 5.1, Android 4.x, Android 2.3
        // old WebKit doesn't clone checked state correctly in fragments
        support.checkClone = fragment.cloneNode(true).cloneNode(true).lastChild.checked;

        // Support: Firefox, Chrome, Safari
        // Beware of CSP restrictions (https://developer.mozilla.org/en/Security/CSP)
        support.focusinBubbles = "onfocusin" in window;

        div.style.backgroundClip = "content-box";
        div.cloneNode(true).style.backgroundClip = "";
        support.clearCloneStyle = div.style.backgroundClip === "content-box";

        // Run tests that need a body at doc ready
        jQuery(function() {
            var container, marginDiv,
            // Support: Firefox, Android 2.3 (Prefixed box-sizing versions).
            divReset = "padding:0;margin:0;border:0;display:block;-webkit-box-sizing:content-box;-moz-box-sizing:content-box;box-sizing:content-box",
                body = document.getElementsByTagName("body")[0];

            if (!body) {
                // Return for frameset docs that don't have a body
                return;
            }

            container = document.createElement("div");
            container.style.cssText = "border:0;width:0;height:0;position:absolute;top:0;left:-9999px;margin-top:1px";

            // Check box-sizing and margin behavior.
            body.appendChild(container).appendChild(div);
            div.innerHTML = "";
            // Support: Firefox, Android 2.3 (Prefixed box-sizing versions).
            div.style.cssText = "-webkit-box-sizing:border-box;-moz-box-sizing:border-box;box-sizing:border-box;padding:1px;border:1px;display:block;width:4px;margin-top:1%;position:absolute;top:1%";

            // Workaround failing boxSizing test due to offsetWidth returning wrong value
            // with some non-1 values of body zoom, ticket #13543
            jQuery.swap(body, body.style.zoom != null ? {
                zoom: 1
            } : {}, function() {
                support.boxSizing = div.offsetWidth === 4;
            });

            // Use window.getComputedStyle because jsdom on node.js will break without it.
            if (window.getComputedStyle) {
                support.pixelPosition = (window.getComputedStyle(div, null) || {}).top !== "1%";
                support.boxSizingReliable = (window.getComputedStyle(div, null) || {
                    width: "4px"
                }).width === "4px";

                // Support: Android 2.3
                // Check if div with explicit width and no margin-right incorrectly
                // gets computed margin-right based on width of container. (#3333)
                // WebKit Bug 13343 - getComputedStyle returns wrong value for margin-right
                marginDiv = div.appendChild(document.createElement("div"));
                marginDiv.style.cssText = div.style.cssText = divReset;
                marginDiv.style.marginRight = marginDiv.style.width = "0";
                div.style.width = "1px";

                support.reliableMarginRight = !parseFloat((window.getComputedStyle(marginDiv, null) || {}).marginRight);
            }

            body.removeChild(container);
        });

        return support;
    })({});

    /*
	Implementation Summary

	1. Enforce API surface and semantic compatibility with 1.9.x branch
	2. Improve the module's maintainability by reducing the storage
		paths to a single mechanism.
	3. Use the same single mechanism to support "private" and "user" data.
	4. _Never_ expose "private" data to user code (TODO: Drop _data, _removeData)
	5. Avoid exposing implementation details on user objects (eg. expando properties)
	6. Provide a clear path for implementation upgrade to WeakMap in 2014
*/
    var data_user, data_priv,
    rbrace = /(?:\{[\s\S]*\}|\[[\s\S]*\])$/,
        rmultiDash = /([A-Z])/g;

    function Data() {
        // Support: Android < 4,
        // Old WebKit does not have Object.preventExtensions/freeze method,
        // return new empty object instead with no [[set]] accessor
        Object.defineProperty(this.cache = {}, 0, {
            get: function() {
                return {};
            }
        });

        this.expando = jQuery.expando + Math.random();
    }

    Data.uid = 1;

    Data.accepts = function(owner) {
        // Accepts only:
        //  - Node
        //    - Node.ELEMENT_NODE
        //    - Node.DOCUMENT_NODE
        //  - Object
        //    - Any
        return owner.nodeType ? owner.nodeType === 1 || owner.nodeType === 9 : true;
    };

    Data.prototype = {
        key: function(owner) {
            // We can accept data for non-element nodes in modern browsers,
            // but we should not, see #8335.
            // Always return the key for a frozen object.
            if (!Data.accepts(owner)) {
                return 0;
            }

            var descriptor = {},
            // Check if the owner object already has a cache key
            unlock = owner[this.expando];

            // If not, create one
            if (!unlock) {
                unlock = Data.uid++;

                // Secure it in a non-enumerable, non-writable property
                try {
                    descriptor[this.expando] = {
                        value: unlock
                    };
                    Object.defineProperties(owner, descriptor);

                    // Support: Android < 4
                    // Fallback to a less secure definition
                } catch (e) {
                    descriptor[this.expando] = unlock;
                    jQuery.extend(owner, descriptor);
                }
            }

            // Ensure the cache object
            if (!this.cache[unlock]) {
                this.cache[unlock] = {};
            }

            return unlock;
        },
        set: function(owner, data, value) {
            var prop,
            // There may be an unlock assigned to this node,
            // if there is no entry for this "owner", create one inline
            // and set the unlock as though an owner entry had always existed
            unlock = this.key(owner),
                cache = this.cache[unlock];

            // Handle: [ owner, key, value ] args
            if (typeof data === "string") {
                cache[data] = value;

                // Handle: [ owner, { properties } ] args
            } else {
                // Fresh assignments by object are shallow copied
                if (jQuery.isEmptyObject(cache)) {
                    jQuery.extend(this.cache[unlock], data);
                    // Otherwise, copy the properties one-by-one to the cache object
                } else {
                    for (prop in data) {
                        cache[prop] = data[prop];
                    }
                }
            }
            return cache;
        },
        get: function(owner, key) {
            // Either a valid cache is found, or will be created.
            // New caches will be created and the unlock returned,
            // allowing direct access to the newly created
            // empty data object. A valid owner object must be provided.
            var cache = this.cache[this.key(owner)];

            return key === undefined ? cache : cache[key];
        },
        access: function(owner, key, value) {
            var stored;
            // In cases where either:
            //
            //   1. No key was specified
            //   2. A string key was specified, but no value provided
            //
            // Take the "read" path and allow the get method to determine
            // which value to return, respectively either:
            //
            //   1. The entire cache object
            //   2. The data stored at the key
            //
            if (key === undefined || ((key && typeof key === "string") && value === undefined)) {

                stored = this.get(owner, key);

                return stored !== undefined ? stored : this.get(owner, jQuery.camelCase(key));
            }

            // [*]When the key is not a string, or both a key and value
            // are specified, set or extend (existing objects) with either:
            //
            //   1. An object of properties
            //   2. A key and value
            //
            this.set(owner, key, value);

            // Since the "set" path can have two possible entry points
            // return the expected data based on which path was taken[*]
            return value !== undefined ? value : key;
        },
        remove: function(owner, key) {
            var i, name, camel,
            unlock = this.key(owner),
                cache = this.cache[unlock];

            if (key === undefined) {
                this.cache[unlock] = {};

            } else {
                // Support array or space separated string of keys
                if (jQuery.isArray(key)) {
                    // If "name" is an array of keys...
                    // When data is initially created, via ("key", "val") signature,
                    // keys will be converted to camelCase.
                    // Since there is no way to tell _how_ a key was added, remove
                    // both plain key and camelCase key. #12786
                    // This will only penalize the array argument path.
                    name = key.concat(key.map(jQuery.camelCase));
                } else {
                    camel = jQuery.camelCase(key);
                    // Try the string as a key before any manipulation
                    if (key in cache) {
                        name = [key, camel];
                    } else {
                        // If a key with the spaces exists, use it.
                        // Otherwise, create an array by matching non-whitespace
                        name = camel;
                        name = name in cache ? [name] : (name.match(core_rnotwhite) || []);
                    }
                }

                i = name.length;
                while (i--) {
                    delete cache[name[i]];
                }
            }
        },
        hasData: function(owner) {
            return !jQuery.isEmptyObject(
            this.cache[owner[this.expando]] || {});
        },
        discard: function(owner) {
            if (owner[this.expando]) {
                delete this.cache[owner[this.expando]];
            }
        }
    };

    // These may be used throughout the jQuery core codebase
    data_user = new Data();
    data_priv = new Data();


    jQuery.extend({
        acceptData: Data.accepts,

        hasData: function(elem) {
            return data_user.hasData(elem) || data_priv.hasData(elem);
        },

        data: function(elem, name, data) {
            return data_user.access(elem, name, data);
        },

        removeData: function(elem, name) {
            data_user.remove(elem, name);
        },

        // TODO: Now that all calls to _data and _removeData have been replaced
        // with direct calls to data_priv methods, these can be deprecated.
        _data: function(elem, name, data) {
            return data_priv.access(elem, name, data);
        },

        _removeData: function(elem, name) {
            data_priv.remove(elem, name);
        }
    });

    jQuery.fn.extend({
        data: function(key, value) {
            var attrs, name,
            elem = this[0],
                i = 0,
                data = null;

            // Gets all values
            if (key === undefined) {
                if (this.length) {
                    data = data_user.get(elem);

                    if (elem.nodeType === 1 && !data_priv.get(elem, "hasDataAttrs")) {
                        attrs = elem.attributes;
                        for (; i < attrs.length; i++) {
                            name = attrs[i].name;

                            if (name.indexOf("data-") === 0) {
                                name = jQuery.camelCase(name.slice(5));
                                dataAttr(elem, name, data[name]);
                            }
                        }
                        data_priv.set(elem, "hasDataAttrs", true);
                    }
                }

                return data;
            }

            // Sets multiple values
            if (typeof key === "object") {
                return this.each(function() {
                    data_user.set(this, key);
                });
            }

            return jQuery.access(this, function(value) {
                var data,
                camelKey = jQuery.camelCase(key);

                // The calling jQuery object (element matches) is not empty
                // (and therefore has an element appears at this[ 0 ]) and the
                // `value` parameter was not undefined. An empty jQuery object
                // will result in `undefined` for elem = this[ 0 ] which will
                // throw an exception if an attempt to read a data cache is made.
                if (elem && value === undefined) {
                    // Attempt to get data from the cache
                    // with the key as-is
                    data = data_user.get(elem, key);
                    if (data !== undefined) {
                        return data;
                    }

                    // Attempt to get data from the cache
                    // with the key camelized
                    data = data_user.get(elem, camelKey);
                    if (data !== undefined) {
                        return data;
                    }

                    // Attempt to "discover" the data in
                    // HTML5 custom data-* attrs
                    data = dataAttr(elem, camelKey, undefined);
                    if (data !== undefined) {
                        return data;
                    }

                    // We tried really hard, but the data doesn't exist.
                    return;
                }

                // Set the data...
                this.each(function() {
                    // First, attempt to store a copy or reference of any
                    // data that might've been store with a camelCased key.
                    var data = data_user.get(this, camelKey);

                    // For HTML5 data-* attribute interop, we have to
                    // store property names with dashes in a camelCase form.
                    // This might not apply to all properties...*
                    data_user.set(this, camelKey, value);

                    // *... In the case of properties that might _actually_
                    // have dashes, we need to also store a copy of that
                    // unchanged property.
                    if (key.indexOf("-") !== -1 && data !== undefined) {
                        data_user.set(this, key, value);
                    }
                });
            }, null, value, arguments.length > 1, null, true);
        },

        removeData: function(key) {
            return this.each(function() {
                data_user.remove(this, key);
            });
        }
    });

    function dataAttr(elem, key, data) {
        var name;

        // If nothing was found internally, try to fetch any
        // data from the HTML5 data-* attribute
        if (data === undefined && elem.nodeType === 1) {
            name = "data-" + key.replace(rmultiDash, "-$1").toLowerCase();
            data = elem.getAttribute(name);

            if (typeof data === "string") {
                try {
                    data = data === "true" ? true : data === "false" ? false : data === "null" ? null :
                    // Only convert to a number if it doesn't change the string
                    +
                    data + "" === data ? +data : rbrace.test(data) ? JSON.parse(data) : data;
                } catch (e) {}

                // Make sure we set the data so it isn't changed later
                data_user.set(elem, key, data);
            } else {
                data = undefined;
            }
        }
        return data;
    }
    jQuery.extend({
        queue: function(elem, type, data) {
            var queue;

            if (elem) {
                type = (type || "fx") + "queue";
                queue = data_priv.get(elem, type);

                // Speed up dequeue by getting out quickly if this is just a lookup
                if (data) {
                    if (!queue || jQuery.isArray(data)) {
                        queue = data_priv.access(elem, type, jQuery.makeArray(data));
                    } else {
                        queue.push(data);
                    }
                }
                return queue || [];
            }
        },

        dequeue: function(elem, type) {
            type = type || "fx";

            var queue = jQuery.queue(elem, type),
                startLength = queue.length,
                fn = queue.shift(),
                hooks = jQuery._queueHooks(elem, type),
                next = function() {
                    jQuery.dequeue(elem, type);
                };

            // If the fx queue is dequeued, always remove the progress sentinel
            if (fn === "inprogress") {
                fn = queue.shift();
                startLength--;
            }

            if (fn) {

                // Add a progress sentinel to prevent the fx queue from being
                // automatically dequeued
                if (type === "fx") {
                    queue.unshift("inprogress");
                }

                // clear up the last queue stop function
                delete hooks.stop;
                fn.call(elem, next, hooks);
            }

            if (!startLength && hooks) {
                hooks.empty.fire();
            }
        },

        // not intended for public consumption - generates a queueHooks object, or returns the current one
        _queueHooks: function(elem, type) {
            var key = type + "queueHooks";
            return data_priv.get(elem, key) || data_priv.access(elem, key, {
                empty: jQuery.Callbacks("once memory").add(function() {
                    data_priv.remove(elem, [type + "queue", key]);
                })
            });
        }
    });

    jQuery.fn.extend({
        queue: function(type, data) {
            var setter = 2;

            if (typeof type !== "string") {
                data = type;
                type = "fx";
                setter--;
            }

            if (arguments.length < setter) {
                return jQuery.queue(this[0], type);
            }

            return data === undefined ? this : this.each(function() {
                var queue = jQuery.queue(this, type, data);

                // ensure a hooks for this queue
                jQuery._queueHooks(this, type);

                if (type === "fx" && queue[0] !== "inprogress") {
                    jQuery.dequeue(this, type);
                }
            });
        },
        dequeue: function(type) {
            return this.each(function() {
                jQuery.dequeue(this, type);
            });
        },
        // Based off of the plugin by Clint Helfers, with permission.
        // http://blindsignals.com/index.php/2009/07/jquery-delay/
        delay: function(time, type) {
            time = jQuery.fx ? jQuery.fx.speeds[time] || time : time;
            type = type || "fx";

            return this.queue(type, function(next, hooks) {
                var timeout = setTimeout(next, time);
                hooks.stop = function() {
                    clearTimeout(timeout);
                };
            });
        },
        clearQueue: function(type) {
            return this.queue(type || "fx", []);
        },
        // Get a promise resolved when queues of a certain type
        // are emptied (fx is the type by default)
        promise: function(type, obj) {
            var tmp,
            count = 1,
                defer = jQuery.Deferred(),
                elements = this,
                i = this.length,
                resolve = function() {
                    if (!(--count)) {
                        defer.resolveWith(elements, [elements]);
                    }
                };

            if (typeof type !== "string") {
                obj = type;
                type = undefined;
            }
            type = type || "fx";

            while (i--) {
                tmp = data_priv.get(elements[i], type + "queueHooks");
                if (tmp && tmp.empty) {
                    count++;
                    tmp.empty.add(resolve);
                }
            }
            resolve();
            return defer.promise(obj);
        }
    });
    var nodeHook, boolHook,
    rclass = /[\t\r\n\f]/g,
        rreturn = /\r/g,
        rfocusable = /^(?:input|select|textarea|button)$/i;

    jQuery.fn.extend({
        attr: function(name, value) {
            return jQuery.access(this, jQuery.attr, name, value, arguments.length > 1);
        },

        removeAttr: function(name) {
            return this.each(function() {
                jQuery.removeAttr(this, name);
            });
        },

        prop: function(name, value) {
            return jQuery.access(this, jQuery.prop, name, value, arguments.length > 1);
        },

        removeProp: function(name) {
            return this.each(function() {
                delete this[jQuery.propFix[name] || name];
            });
        },

        addClass: function(value) {
            var classes, elem, cur, clazz, j,
            i = 0,
                len = this.length,
                proceed = typeof value === "string" && value;

            if (jQuery.isFunction(value)) {
                return this.each(function(j) {
                    jQuery(this).addClass(value.call(this, j, this.className));
                });
            }

            if (proceed) {
                // The disjunction here is for better compressibility (see removeClass)
                classes = (value || "").match(core_rnotwhite) || [];

                for (; i < len; i++) {
                    elem = this[i];
                    cur = elem.nodeType === 1 && (elem.className ? (" " + elem.className + " ").replace(rclass, " ") : " ");

                    if (cur) {
                        j = 0;
                        while ((clazz = classes[j++])) {
                            if (cur.indexOf(" " + clazz + " ") < 0) {
                                cur += clazz + " ";
                            }
                        }
                        elem.className = jQuery.trim(cur);

                    }
                }
            }

            return this;
        },

        removeClass: function(value) {
            var classes, elem, cur, clazz, j,
            i = 0,
                len = this.length,
                proceed = arguments.length === 0 || typeof value === "string" && value;

            if (jQuery.isFunction(value)) {
                return this.each(function(j) {
                    jQuery(this).removeClass(value.call(this, j, this.className));
                });
            }
            if (proceed) {
                classes = (value || "").match(core_rnotwhite) || [];

                for (; i < len; i++) {
                    elem = this[i];
                    // This expression is here for better compressibility (see addClass)
                    cur = elem.nodeType === 1 && (elem.className ? (" " + elem.className + " ").replace(rclass, " ") : "");

                    if (cur) {
                        j = 0;
                        while ((clazz = classes[j++])) {
                            // Remove *all* instances
                            while (cur.indexOf(" " + clazz + " ") >= 0) {
                                cur = cur.replace(" " + clazz + " ", " ");
                            }
                        }
                        elem.className = value ? jQuery.trim(cur) : "";
                    }
                }
            }

            return this;
        },

        toggleClass: function(value, stateVal) {
            var type = typeof value;

            if (typeof stateVal === "boolean" && type === "string") {
                return stateVal ? this.addClass(value) : this.removeClass(value);
            }

            if (jQuery.isFunction(value)) {
                return this.each(function(i) {
                    jQuery(this).toggleClass(value.call(this, i, this.className, stateVal), stateVal);
                });
            }

            return this.each(function() {
                if (type === "string") {
                    // toggle individual class names
                    var className,
                    i = 0,
                        self = jQuery(this),
                        classNames = value.match(core_rnotwhite) || [];

                    while ((className = classNames[i++])) {
                        // check each className given, space separated list
                        if (self.hasClass(className)) {
                            self.removeClass(className);
                        } else {
                            self.addClass(className);
                        }
                    }

                    // Toggle whole class name
                } else if (type === core_strundefined || type === "boolean") {
                    if (this.className) {
                        // store className if set
                        data_priv.set(this, "__className__", this.className);
                    }

                    // If the element has a class name or if we're passed "false",
                    // then remove the whole classname (if there was one, the above saved it).
                    // Otherwise bring back whatever was previously saved (if anything),
                    // falling back to the empty string if nothing was stored.
                    this.className = this.className || value === false ? "" : data_priv.get(this, "__className__") || "";
                }
            });
        },

        hasClass: function(selector) {
            var className = " " + selector + " ",
                i = 0,
                l = this.length;
            for (; i < l; i++) {
                if (this[i].nodeType === 1 && (" " + this[i].className + " ").replace(rclass, " ").indexOf(className) >= 0) {
                    return true;
                }
            }

            return false;
        },

        val: function(value) {
            var hooks, ret, isFunction,
            elem = this[0];

            if (!arguments.length) {
                if (elem) {
                    hooks = jQuery.valHooks[elem.type] || jQuery.valHooks[elem.nodeName.toLowerCase()];

                    if (hooks && "get" in hooks && (ret = hooks.get(elem, "value")) !== undefined) {
                        return ret;
                    }

                    ret = elem.value;

                    return typeof ret === "string" ?
                    // handle most common string cases
                    ret.replace(rreturn, "") :
                    // handle cases where value is null/undef or number
                    ret == null ? "" : ret;
                }

                return;
            }

            isFunction = jQuery.isFunction(value);

            return this.each(function(i) {
                var val;

                if (this.nodeType !== 1) {
                    return;
                }

                if (isFunction) {
                    val = value.call(this, i, jQuery(this).val());
                } else {
                    val = value;
                }

                // Treat null/undefined as ""; convert numbers to string
                if (val == null) {
                    val = "";
                } else if (typeof val === "number") {
                    val += "";
                } else if (jQuery.isArray(val)) {
                    val = jQuery.map(val, function(value) {
                        return value == null ? "" : value + "";
                    });
                }

                hooks = jQuery.valHooks[this.type] || jQuery.valHooks[this.nodeName.toLowerCase()];

                // If set returns undefined, fall back to normal setting
                if (!hooks || !("set" in hooks) || hooks.set(this, val, "value") === undefined) {
                    this.value = val;
                }
            });
        }
    });

    jQuery.extend({
        valHooks: {
            option: {
                get: function(elem) {
                    // attributes.value is undefined in Blackberry 4.7 but
                    // uses .value. See #6932
                    var val = elem.attributes.value;
                    return !val || val.specified ? elem.value : elem.text;
                }
            },
            select: {
                get: function(elem) {
                    var value, option,
                    options = elem.options,
                        index = elem.selectedIndex,
                        one = elem.type === "select-one" || index < 0,
                        values = one ? null : [],
                        max = one ? index + 1 : options.length,
                        i = index < 0 ? max : one ? index : 0;

                    // Loop through all the selected options
                    for (; i < max; i++) {
                        option = options[i];

                        // IE6-9 doesn't update selected after form reset (#2551)
                        if ((option.selected || i === index) &&
                        // Don't return options that are disabled or in a disabled optgroup
                        (jQuery.support.optDisabled ? !option.disabled : option.getAttribute("disabled") === null) && (!option.parentNode.disabled || !jQuery.nodeName(option.parentNode, "optgroup"))) {

                            // Get the specific value for the option
                            value = jQuery(option).val();

                            // We don't need an array for one selects
                            if (one) {
                                return value;
                            }

                            // Multi-Selects return an array
                            values.push(value);
                        }
                    }

                    return values;
                },

                set: function(elem, value) {
                    var optionSet, option,
                    options = elem.options,
                        values = jQuery.makeArray(value),
                        i = options.length;

                    while (i--) {
                        option = options[i];
                        if ((option.selected = jQuery.inArray(jQuery(option).val(), values) >= 0)) {
                            optionSet = true;
                        }
                    }

                    // force browsers to behave consistently when non-matching value is set
                    if (!optionSet) {
                        elem.selectedIndex = -1;
                    }
                    return values;
                }
            }
        },

        attr: function(elem, name, value) {
            var hooks, ret,
            nType = elem.nodeType;

            // don't get/set attributes on text, comment and attribute nodes
            if (!elem || nType === 3 || nType === 8 || nType === 2) {
                return;
            }

            // Fallback to prop when attributes are not supported
            if (typeof elem.getAttribute === core_strundefined) {
                return jQuery.prop(elem, name, value);
            }

            // All attributes are lowercase
            // Grab necessary hook if one is defined
            if (nType !== 1 || !jQuery.isXMLDoc(elem)) {
                name = name.toLowerCase();
                hooks = jQuery.attrHooks[name] || (jQuery.expr.match.bool.test(name) ? boolHook : nodeHook);
            }

            if (value !== undefined) {

                if (value === null) {
                    jQuery.removeAttr(elem, name);

                } else if (hooks && "set" in hooks && (ret = hooks.set(elem, value, name)) !== undefined) {
                    return ret;

                } else {
                    elem.setAttribute(name, value + "");
                    return value;
                }

            } else if (hooks && "get" in hooks && (ret = hooks.get(elem, name)) !== null) {
                return ret;

            } else {
                ret = jQuery.find.attr(elem, name);

                // Non-existent attributes return null, we normalize to undefined
                return ret == null ? undefined : ret;
            }
        },

        removeAttr: function(elem, value) {
            var name, propName,
            i = 0,
                attrNames = value && value.match(core_rnotwhite);

            if (attrNames && elem.nodeType === 1) {
                while ((name = attrNames[i++])) {
                    propName = jQuery.propFix[name] || name;

                    // Boolean attributes get special treatment (#10870)
                    if (jQuery.expr.match.bool.test(name)) {
                        // Set corresponding property to false
                        elem[propName] = false;
                    }

                    elem.removeAttribute(name);
                }
            }
        },

        attrHooks: {
            type: {
                set: function(elem, value) {
                    if (!jQuery.support.radioValue && value === "radio" && jQuery.nodeName(elem, "input")) {
                        // Setting the type on a radio button after the value resets the value in IE6-9
                        // Reset value to default in case type is set after value during creation
                        var val = elem.value;
                        elem.setAttribute("type", value);
                        if (val) {
                            elem.value = val;
                        }
                        return value;
                    }
                }
            }
        },

        propFix: {
            "for": "htmlFor",
            "class": "className"
        },

        prop: function(elem, name, value) {
            var ret, hooks, notxml,
            nType = elem.nodeType;

            // don't get/set properties on text, comment and attribute nodes
            if (!elem || nType === 3 || nType === 8 || nType === 2) {
                return;
            }

            notxml = nType !== 1 || !jQuery.isXMLDoc(elem);

            if (notxml) {
                // Fix name and attach hooks
                name = jQuery.propFix[name] || name;
                hooks = jQuery.propHooks[name];
            }

            if (value !== undefined) {
                return hooks && "set" in hooks && (ret = hooks.set(elem, value, name)) !== undefined ? ret : (elem[name] = value);

            } else {
                return hooks && "get" in hooks && (ret = hooks.get(elem, name)) !== null ? ret : elem[name];
            }
        },

        propHooks: {
            tabIndex: {
                get: function(elem) {
                    return elem.hasAttribute("tabindex") || rfocusable.test(elem.nodeName) || elem.href ? elem.tabIndex : -1;
                }
            }
        }
    });

    // Hooks for boolean attributes
    boolHook = {
        set: function(elem, value, name) {
            if (value === false) {
                // Remove boolean attributes when set to false
                jQuery.removeAttr(elem, name);
            } else {
                elem.setAttribute(name, name);
            }
            return name;
        }
    };
    jQuery.each(jQuery.expr.match.bool.source.match(/\w+/g), function(i, name) {
        var getter = jQuery.expr.attrHandle[name] || jQuery.find.attr;

        jQuery.expr.attrHandle[name] = function(elem, name, isXML) {
            var fn = jQuery.expr.attrHandle[name],
                ret = isXML ? undefined :
                /* jshint eqeqeq: false */
                // Temporarily disable this handler to check existence
                (jQuery.expr.attrHandle[name] = undefined) != getter(elem, name, isXML) ?

                name.toLowerCase() : null;

            // Restore handler
            jQuery.expr.attrHandle[name] = fn;

            return ret;
        };
    });

    // Support: IE9+
    // Selectedness for an option in an optgroup can be inaccurate
    if (!jQuery.support.optSelected) {
        jQuery.propHooks.selected = {
            get: function(elem) {
                var parent = elem.parentNode;
                if (parent && parent.parentNode) {
                    parent.parentNode.selectedIndex;
                }
                return null;
            }
        };
    }

    jQuery.each(["tabIndex", "readOnly", "maxLength", "cellSpacing", "cellPadding", "rowSpan", "colSpan", "useMap", "frameBorder", "contentEditable"], function() {
        jQuery.propFix[this.toLowerCase()] = this;
    });

    // Radios and checkboxes getter/setter
    jQuery.each(["radio", "checkbox"], function() {
        jQuery.valHooks[this] = {
            set: function(elem, value) {
                if (jQuery.isArray(value)) {
                    return (elem.checked = jQuery.inArray(jQuery(elem).val(), value) >= 0);
                }
            }
        };
        if (!jQuery.support.checkOn) {
            jQuery.valHooks[this].get = function(elem) {
                // Support: Webkit
                // "" is returned instead of "on" if a value isn't specified
                return elem.getAttribute("value") === null ? "on" : elem.value;
            };
        }
    });
    var rkeyEvent = /^key/,
        rmouseEvent = /^(?:mouse|contextmenu)|click/,
        rfocusMorph = /^(?:focusinfocus|focusoutblur)$/,
        rtypenamespace = /^([^.]*)(?:\.(.+)|)$/;

    function returnTrue() {
        return true;
    }

    function returnFalse() {
        return false;
    }

    function safeActiveElement() {
        try {
            return document.activeElement;
        } catch (err) {}
    }

    /*
     * Helper functions for managing events -- not part of the public interface.
     * Props to Dean Edwards' addEvent library for many of the ideas.
     */
    jQuery.event = {

        global: {},

        add: function(elem, types, handler, data, selector) {

            var handleObjIn, eventHandle, tmp,
            events, t, handleObj,
            special, handlers, type, namespaces, origType,
            elemData = data_priv.get(elem);

            // Don't attach events to noData or text/comment nodes (but allow plain objects)
            if (!elemData) {
                return;
            }

            // Caller can pass in an object of custom data in lieu of the handler
            if (handler.handler) {
                handleObjIn = handler;
                handler = handleObjIn.handler;
                selector = handleObjIn.selector;
            }

            // Make sure that the handler has a unique ID, used to find/remove it later
            if (!handler.guid) {
                handler.guid = jQuery.guid++;
            }

            // Init the element's event structure and main handler, if this is the first
            if (!(events = elemData.events)) {
                events = elemData.events = {};
            }
            if (!(eventHandle = elemData.handle)) {
                eventHandle = elemData.handle = function(e) {
                    // Discard the second event of a jQuery.event.trigger() and
                    // when an event is called after a page has unloaded
                    return typeof jQuery !== core_strundefined && (!e || jQuery.event.triggered !== e.type) ? jQuery.event.dispatch.apply(eventHandle.elem, arguments) : undefined;
                };
                // Add elem as a property of the handle fn to prevent a memory leak with IE non-native events
                eventHandle.elem = elem;
            }

            // Handle multiple events separated by a space
            types = (types || "").match(core_rnotwhite) || [""];
            t = types.length;
            while (t--) {
                tmp = rtypenamespace.exec(types[t]) || [];
                type = origType = tmp[1];
                namespaces = (tmp[2] || "").split(".").sort();

                // There *must* be a type, no attaching namespace-only handlers
                if (!type) {
                    continue;
                }

                // If event changes its type, use the special event handlers for the changed type
                special = jQuery.event.special[type] || {};

                // If selector defined, determine special event api type, otherwise given type
                type = (selector ? special.delegateType : special.bindType) || type;

                // Update special based on newly reset type
                special = jQuery.event.special[type] || {};

                // handleObj is passed to all event handlers
                handleObj = jQuery.extend({
                    type: type,
                    origType: origType,
                    data: data,
                    handler: handler,
                    guid: handler.guid,
                    selector: selector,
                    needsContext: selector && jQuery.expr.match.needsContext.test(selector),
                    namespace: namespaces.join(".")
                }, handleObjIn);

                // Init the event handler queue if we're the first
                if (!(handlers = events[type])) {
                    handlers = events[type] = [];
                    handlers.delegateCount = 0;

                    // Only use addEventListener if the special events handler returns false
                    if (!special.setup || special.setup.call(elem, data, namespaces, eventHandle) === false) {
                        if (elem.addEventListener) {
                            elem.addEventListener(type, eventHandle, false);
                        }
                    }
                }

                if (special.add) {
                    special.add.call(elem, handleObj);

                    if (!handleObj.handler.guid) {
                        handleObj.handler.guid = handler.guid;
                    }
                }

                // Add to the element's handler list, delegates in front
                if (selector) {
                    handlers.splice(handlers.delegateCount++, 0, handleObj);
                } else {
                    handlers.push(handleObj);
                }

                // Keep track of which events have ever been used, for event optimization
                jQuery.event.global[type] = true;
            }

            // Nullify elem to prevent memory leaks in IE
            elem = null;
        },

        // Detach an event or set of events from an element
        remove: function(elem, types, handler, selector, mappedTypes) {

            var j, origCount, tmp,
            events, t, handleObj,
            special, handlers, type, namespaces, origType,
            elemData = data_priv.hasData(elem) && data_priv.get(elem);

            if (!elemData || !(events = elemData.events)) {
                return;
            }

            // Once for each type.namespace in types; type may be omitted
            types = (types || "").match(core_rnotwhite) || [""];
            t = types.length;
            while (t--) {
                tmp = rtypenamespace.exec(types[t]) || [];
                type = origType = tmp[1];
                namespaces = (tmp[2] || "").split(".").sort();

                // Unbind all events (on this namespace, if provided) for the element
                if (!type) {
                    for (type in events) {
                        jQuery.event.remove(elem, type + types[t], handler, selector, true);
                    }
                    continue;
                }

                special = jQuery.event.special[type] || {};
                type = (selector ? special.delegateType : special.bindType) || type;
                handlers = events[type] || [];
                tmp = tmp[2] && new RegExp("(^|\\.)" + namespaces.join("\\.(?:.*\\.|)") + "(\\.|$)");

                // Remove matching events
                origCount = j = handlers.length;
                while (j--) {
                    handleObj = handlers[j];

                    if ((mappedTypes || origType === handleObj.origType) && (!handler || handler.guid === handleObj.guid) && (!tmp || tmp.test(handleObj.namespace)) && (!selector || selector === handleObj.selector || selector === "**" && handleObj.selector)) {
                        handlers.splice(j, 1);

                        if (handleObj.selector) {
                            handlers.delegateCount--;
                        }
                        if (special.remove) {
                            special.remove.call(elem, handleObj);
                        }
                    }
                }

                // Remove generic event handler if we removed something and no more handlers exist
                // (avoids potential for endless recursion during removal of special event handlers)
                if (origCount && !handlers.length) {
                    if (!special.teardown || special.teardown.call(elem, namespaces, elemData.handle) === false) {
                        jQuery.removeEvent(elem, type, elemData.handle);
                    }

                    delete events[type];
                }
            }

            // Remove the expando if it's no longer used
            if (jQuery.isEmptyObject(events)) {
                delete elemData.handle;
                data_priv.remove(elem, "events");
            }
        },

        trigger: function(event, data, elem, onlyHandlers) {

            var i, cur, tmp, bubbleType, ontype, handle, special,
            eventPath = [elem || document],
                type = core_hasOwn.call(event, "type") ? event.type : event,
                namespaces = core_hasOwn.call(event, "namespace") ? event.namespace.split(".") : [];

            cur = tmp = elem = elem || document;

            // Don't do events on text and comment nodes
            if (elem.nodeType === 3 || elem.nodeType === 8) {
                return;
            }

            // focus/blur morphs to focusin/out; ensure we're not firing them right now
            if (rfocusMorph.test(type + jQuery.event.triggered)) {
                return;
            }

            if (type.indexOf(".") >= 0) {
                // Namespaced trigger; create a regexp to match event type in handle()
                namespaces = type.split(".");
                type = namespaces.shift();
                namespaces.sort();
            }
            ontype = type.indexOf(":") < 0 && "on" + type;

            // Caller can pass in a jQuery.Event object, Object, or just an event type string
            event = event[jQuery.expando] ? event : new jQuery.Event(type, typeof event === "object" && event);

            // Trigger bitmask: & 1 for native handlers; & 2 for jQuery (always true)
            event.isTrigger = onlyHandlers ? 2 : 3;
            event.namespace = namespaces.join(".");
            event.namespace_re = event.namespace ? new RegExp("(^|\\.)" + namespaces.join("\\.(?:.*\\.|)") + "(\\.|$)") : null;

            // Clean up the event in case it is being reused
            event.result = undefined;
            if (!event.target) {
                event.target = elem;
            }

            // Clone any incoming data and prepend the event, creating the handler arg list
            data = data == null ? [event] : jQuery.makeArray(data, [event]);

            // Allow special events to draw outside the lines
            special = jQuery.event.special[type] || {};
            if (!onlyHandlers && special.trigger && special.trigger.apply(elem, data) === false) {
                return;
            }

            // Determine event propagation path in advance, per W3C events spec (#9951)
            // Bubble up to document, then to window; watch for a global ownerDocument var (#9724)
            if (!onlyHandlers && !special.noBubble && !jQuery.isWindow(elem)) {

                bubbleType = special.delegateType || type;
                if (!rfocusMorph.test(bubbleType + type)) {
                    cur = cur.parentNode;
                }
                for (; cur; cur = cur.parentNode) {
                    eventPath.push(cur);
                    tmp = cur;
                }

                // Only add window if we got to document (e.g., not plain obj or detached DOM)
                if (tmp === (elem.ownerDocument || document)) {
                    eventPath.push(tmp.defaultView || tmp.parentWindow || window);
                }
            }

            // Fire handlers on the event path
            i = 0;
            while ((cur = eventPath[i++]) && !event.isPropagationStopped()) {

                event.type = i > 1 ? bubbleType : special.bindType || type;

                // jQuery handler
                handle = (data_priv.get(cur, "events") || {})[event.type] && data_priv.get(cur, "handle");
                if (handle) {
                    handle.apply(cur, data);
                }

                // Native handler
                handle = ontype && cur[ontype];
                if (handle && jQuery.acceptData(cur) && handle.apply && handle.apply(cur, data) === false) {
                    event.preventDefault();
                }
            }
            event.type = type;

            // If nobody prevented the default action, do it now
            if (!onlyHandlers && !event.isDefaultPrevented()) {

                if ((!special._default || special._default.apply(eventPath.pop(), data) === false) && jQuery.acceptData(elem)) {

                    // Call a native DOM method on the target with the same name name as the event.
                    // Don't do default actions on window, that's where global variables be (#6170)
                    if (ontype && jQuery.isFunction(elem[type]) && !jQuery.isWindow(elem)) {

                        // Don't re-trigger an onFOO event when we call its FOO() method
                        tmp = elem[ontype];

                        if (tmp) {
                            elem[ontype] = null;
                        }

                        // Prevent re-triggering of the same event, since we already bubbled it above
                        jQuery.event.triggered = type;
                        elem[type]();
                        jQuery.event.triggered = undefined;

                        if (tmp) {
                            elem[ontype] = tmp;
                        }
                    }
                }
            }

            return event.result;
        },

        dispatch: function(event) {

            // Make a writable jQuery.Event from the native event object
            event = jQuery.event.fix(event);

            var i, j, ret, matched, handleObj,
            handlerQueue = [],
                args = core_slice.call(arguments),
                handlers = (data_priv.get(this, "events") || {})[event.type] || [],
                special = jQuery.event.special[event.type] || {};

            // Use the fix-ed jQuery.Event rather than the (read-only) native event
            args[0] = event;
            event.delegateTarget = this;

            // Call the preDispatch hook for the mapped type, and let it bail if desired
            if (special.preDispatch && special.preDispatch.call(this, event) === false) {
                return;
            }

            // Determine handlers
            handlerQueue = jQuery.event.handlers.call(this, event, handlers);

            // Run delegates first; they may want to stop propagation beneath us
            i = 0;
            while ((matched = handlerQueue[i++]) && !event.isPropagationStopped()) {
                event.currentTarget = matched.elem;

                j = 0;
                while ((handleObj = matched.handlers[j++]) && !event.isImmediatePropagationStopped()) {

                    // Triggered event must either 1) have no namespace, or
                    // 2) have namespace(s) a subset or equal to those in the bound event (both can have no namespace).
                    if (!event.namespace_re || event.namespace_re.test(handleObj.namespace)) {

                        event.handleObj = handleObj;
                        event.data = handleObj.data;

                        ret = ((jQuery.event.special[handleObj.origType] || {}).handle || handleObj.handler)
                            .apply(matched.elem, args);

                        if (ret !== undefined) {
                            if ((event.result = ret) === false) {
                                event.preventDefault();
                                event.stopPropagation();
                            }
                        }
                    }
                }
            }

            // Call the postDispatch hook for the mapped type
            if (special.postDispatch) {
                special.postDispatch.call(this, event);
            }

            return event.result;
        },

        handlers: function(event, handlers) {
            var i, matches, sel, handleObj,
            handlerQueue = [],
                delegateCount = handlers.delegateCount,
                cur = event.target;

            // Find delegate handlers
            // Black-hole SVG <use> instance trees (#13180)
            // Avoid non-left-click bubbling in Firefox (#3861)
            if (delegateCount && cur.nodeType && (!event.button || event.type !== "click")) {

                for (; cur !== this; cur = cur.parentNode || this) {

                    // Don't process clicks on disabled elements (#6911, #8165, #11382, #11764)
                    if (cur.disabled !== true || event.type !== "click") {
                        matches = [];
                        for (i = 0; i < delegateCount; i++) {
                            handleObj = handlers[i];

                            // Don't conflict with Object.prototype properties (#13203)
                            sel = handleObj.selector + " ";

                            if (matches[sel] === undefined) {
                                matches[sel] = handleObj.needsContext ? jQuery(sel, this).index(cur) >= 0 : jQuery.find(sel, this, null, [cur]).length;
                            }
                            if (matches[sel]) {
                                matches.push(handleObj);
                            }
                        }
                        if (matches.length) {
                            handlerQueue.push({
                                elem: cur,
                                handlers: matches
                            });
                        }
                    }
                }
            }

            // Add the remaining (directly-bound) handlers
            if (delegateCount < handlers.length) {
                handlerQueue.push({
                    elem: this,
                    handlers: handlers.slice(delegateCount)
                });
            }

            return handlerQueue;
        },

        // Includes some event props shared by KeyEvent and MouseEvent
        props: "altKey bubbles cancelable ctrlKey currentTarget eventPhase metaKey relatedTarget shiftKey target timeStamp view which".split(" "),

        fixHooks: {},

        keyHooks: {
            props: "char charCode key keyCode".split(" "),
            filter: function(event, original) {

                // Add which for key events
                if (event.which == null) {
                    event.which = original.charCode != null ? original.charCode : original.keyCode;
                }

                return event;
            }
        },

        mouseHooks: {
            props: "button buttons clientX clientY offsetX offsetY pageX pageY screenX screenY toElement".split(" "),
            filter: function(event, original) {
                var eventDoc, doc, body,
                button = original.button;

                // Calculate pageX/Y if missing and clientX/Y available
                if (event.pageX == null && original.clientX != null) {
                    eventDoc = event.target.ownerDocument || document;
                    doc = eventDoc.documentElement;
                    body = eventDoc.body;

                    event.pageX = original.clientX + (doc && doc.scrollLeft || body && body.scrollLeft || 0) - (doc && doc.clientLeft || body && body.clientLeft || 0);
                    event.pageY = original.clientY + (doc && doc.scrollTop || body && body.scrollTop || 0) - (doc && doc.clientTop || body && body.clientTop || 0);
                }

                // Add which for click: 1 === left; 2 === middle; 3 === right
                // Note: button is not normalized, so don't use it
                if (!event.which && button !== undefined) {
                    event.which = (button & 1 ? 1 : (button & 2 ? 3 : (button & 4 ? 2 : 0)));
                }

                return event;
            }
        },

        fix: function(event) {
            if (event[jQuery.expando]) {
                return event;
            }

            // Create a writable copy of the event object and normalize some properties
            var i, prop, copy,
            type = event.type,
                originalEvent = event,
                fixHook = this.fixHooks[type];

            if (!fixHook) {
                this.fixHooks[type] = fixHook = rmouseEvent.test(type) ? this.mouseHooks : rkeyEvent.test(type) ? this.keyHooks : {};
            }
            copy = fixHook.props ? this.props.concat(fixHook.props) : this.props;

            event = new jQuery.Event(originalEvent);

            i = copy.length;
            while (i--) {
                prop = copy[i];
                event[prop] = originalEvent[prop];
            }

            // Support: Cordova 2.5 (WebKit) (#13255)
            // All events should have a target; Cordova deviceready doesn't
            if (!event.target) {
                event.target = document;
            }

            // Support: Safari 6.0+, Chrome < 28
            // Target should not be a text node (#504, #13143)
            if (event.target.nodeType === 3) {
                event.target = event.target.parentNode;
            }

            return fixHook.filter ? fixHook.filter(event, originalEvent) : event;
        },

        special: {
            load: {
                // Prevent triggered image.load events from bubbling to window.load
                noBubble: true
            },
            focus: {
                // Fire native event if possible so blur/focus sequence is correct
                trigger: function() {
                    if (this !== safeActiveElement() && this.focus) {
                        this.focus();
                        return false;
                    }
                },
                delegateType: "focusin"
            },
            blur: {
                trigger: function() {
                    if (this === safeActiveElement() && this.blur) {
                        this.blur();
                        return false;
                    }
                },
                delegateType: "focusout"
            },
            click: {
                // For checkbox, fire native event so checked state will be right
                trigger: function() {
                    if (this.type === "checkbox" && this.click && jQuery.nodeName(this, "input")) {
                        this.click();
                        return false;
                    }
                },

                // For cross-browser consistency, don't fire native .click() on links
                _default: function(event) {
                    return jQuery.nodeName(event.target, "a");
                }
            },

            beforeunload: {
                postDispatch: function(event) {

                    // Support: Firefox 20+
                    // Firefox doesn't alert if the returnValue field is not set.
                    if (event.result !== undefined) {
                        event.originalEvent.returnValue = event.result;
                    }
                }
            }
        },

        simulate: function(type, elem, event, bubble) {
            // Piggyback on a donor event to simulate a different one.
            // Fake originalEvent to avoid donor's stopPropagation, but if the
            // simulated event prevents default then we do the same on the donor.
            var e = jQuery.extend(
            new jQuery.Event(),
            event, {
                type: type,
                isSimulated: true,
                originalEvent: {}
            });
            if (bubble) {
                jQuery.event.trigger(e, null, elem);
            } else {
                jQuery.event.dispatch.call(elem, e);
            }
            if (e.isDefaultPrevented()) {
                event.preventDefault();
            }
        }
    };

    jQuery.removeEvent = function(elem, type, handle) {
        if (elem.removeEventListener) {
            elem.removeEventListener(type, handle, false);
        }
    };

    jQuery.Event = function(src, props) {
        // Allow instantiation without the 'new' keyword
        if (!(this instanceof jQuery.Event)) {
            return new jQuery.Event(src, props);
        }

        // Event object
        if (src && src.type) {
            this.originalEvent = src;
            this.type = src.type;

            // Events bubbling up the document may have been marked as prevented
            // by a handler lower down the tree; reflect the correct value.
            this.isDefaultPrevented = (src.defaultPrevented || src.getPreventDefault && src.getPreventDefault()) ? returnTrue : returnFalse;

            // Event type
        } else {
            this.type = src;
        }

        // Put explicitly provided properties onto the event object
        if (props) {
            jQuery.extend(this, props);
        }

        // Create a timestamp if incoming event doesn't have one
        this.timeStamp = src && src.timeStamp || jQuery.now();

        // Mark it as fixed
        this[jQuery.expando] = true;
    };

    // jQuery.Event is based on DOM3 Events as specified by the ECMAScript Language Binding
    // http://www.w3.org/TR/2003/WD-DOM-Level-3-Events-20030331/ecma-script-binding.html
    jQuery.Event.prototype = {
        isDefaultPrevented: returnFalse,
        isPropagationStopped: returnFalse,
        isImmediatePropagationStopped: returnFalse,

        preventDefault: function() {
            var e = this.originalEvent;

            this.isDefaultPrevented = returnTrue;

            if (e && e.preventDefault) {
                e.preventDefault();
            }
        },
        stopPropagation: function() {
            var e = this.originalEvent;

            this.isPropagationStopped = returnTrue;

            if (e && e.stopPropagation) {
                e.stopPropagation();
            }
        },
        stopImmediatePropagation: function() {
            this.isImmediatePropagationStopped = returnTrue;
            this.stopPropagation();
        }
    };

    // Create mouseenter/leave events using mouseover/out and event-time checks
    // Support: Chrome 15+
    jQuery.each({
        mouseenter: "mouseover",
        mouseleave: "mouseout"
    }, function(orig, fix) {
        jQuery.event.special[orig] = {
            delegateType: fix,
            bindType: fix,

            handle: function(event) {
                var ret,
                target = this,
                    related = event.relatedTarget,
                    handleObj = event.handleObj;

                // For mousenter/leave call the handler if related is outside the target.
                // NB: No relatedTarget if the mouse left/entered the browser window
                if (!related || (related !== target && !jQuery.contains(target, related))) {
                    event.type = handleObj.origType;
                    ret = handleObj.handler.apply(this, arguments);
                    event.type = fix;
                }
                return ret;
            }
        };
    });

    // Create "bubbling" focus and blur events
    // Support: Firefox, Chrome, Safari
    if (!jQuery.support.focusinBubbles) {
        jQuery.each({
            focus: "focusin",
            blur: "focusout"
        }, function(orig, fix) {

            // Attach a single capturing handler while someone wants focusin/focusout
            var attaches = 0,
                handler = function(event) {
                    jQuery.event.simulate(fix, event.target, jQuery.event.fix(event), true);
                };

            jQuery.event.special[fix] = {
                setup: function() {
                    if (attaches++ === 0) {
                        document.addEventListener(orig, handler, true);
                    }
                },
                teardown: function() {
                    if (--attaches === 0) {
                        document.removeEventListener(orig, handler, true);
                    }
                }
            };
        });
    }

    jQuery.fn.extend({

        on: function(types, selector, data, fn, /*INTERNAL*/ one) {
            var origFn, type;

            // Types can be a map of types/handlers
            if (typeof types === "object") {
                // ( types-Object, selector, data )
                if (typeof selector !== "string") {
                    // ( types-Object, data )
                    data = data || selector;
                    selector = undefined;
                }
                for (type in types) {
                    this.on(type, selector, data, types[type], one);
                }
                return this;
            }

            if (data == null && fn == null) {
                // ( types, fn )
                fn = selector;
                data = selector = undefined;
            } else if (fn == null) {
                if (typeof selector === "string") {
                    // ( types, selector, fn )
                    fn = data;
                    data = undefined;
                } else {
                    // ( types, data, fn )
                    fn = data;
                    data = selector;
                    selector = undefined;
                }
            }
            if (fn === false) {
                fn = returnFalse;
            } else if (!fn) {
                return this;
            }

            if (one === 1) {
                origFn = fn;
                fn = function(event) {
                    // Can use an empty set, since event contains the info
                    jQuery().off(event);
                    return origFn.apply(this, arguments);
                };
                // Use same guid so caller can remove using origFn
                fn.guid = origFn.guid || (origFn.guid = jQuery.guid++);
            }
            return this.each(function() {
                jQuery.event.add(this, types, fn, data, selector);
            });
        },
        one: function(types, selector, data, fn) {
            return this.on(types, selector, data, fn, 1);
        },
        off: function(types, selector, fn) {
            var handleObj, type;
            if (types && types.preventDefault && types.handleObj) {
                // ( event )  dispatched jQuery.Event
                handleObj = types.handleObj;
                jQuery(types.delegateTarget).off(
                handleObj.namespace ? handleObj.origType + "." + handleObj.namespace : handleObj.origType,
                handleObj.selector,
                handleObj.handler);
                return this;
            }
            if (typeof types === "object") {
                // ( types-object [, selector] )
                for (type in types) {
                    this.off(type, selector, types[type]);
                }
                return this;
            }
            if (selector === false || typeof selector === "function") {
                // ( types [, fn] )
                fn = selector;
                selector = undefined;
            }
            if (fn === false) {
                fn = returnFalse;
            }
            return this.each(function() {
                jQuery.event.remove(this, types, fn, selector);
            });
        },

        trigger: function(type, data) {
            return this.each(function() {
                jQuery.event.trigger(type, data, this);
            });
        },
        triggerHandler: function(type, data) {
            var elem = this[0];
            if (elem) {
                return jQuery.event.trigger(type, data, elem, true);
            }
        }
    });
    var isSimple = /^.[^:#\[\.,]*$/,
        rparentsprev = /^(?:parents|prev(?:Until|All))/,
        rneedsContext = jQuery.expr.match.needsContext,
        // methods guaranteed to produce a unique set when starting from a unique set
        guaranteedUnique = {
            children: true,
            contents: true,
            next: true,
            prev: true
        };

    jQuery.fn.extend({
        find: function(selector) {
            var i,
            ret = [],
                self = this,
                len = self.length;

            if (typeof selector !== "string") {
                return this.pushStack(jQuery(selector).filter(function() {
                    for (i = 0; i < len; i++) {
                        if (jQuery.contains(self[i], this)) {
                            return true;
                        }
                    }
                }));
            }

            for (i = 0; i < len; i++) {
                jQuery.find(selector, self[i], ret);
            }

            // Needed because $( selector, context ) becomes $( context ).find( selector )
            ret = this.pushStack(len > 1 ? jQuery.unique(ret) : ret);
            ret.selector = this.selector ? this.selector + " " + selector : selector;
            return ret;
        },

        has: function(target) {
            var targets = jQuery(target, this),
                l = targets.length;

            return this.filter(function() {
                var i = 0;
                for (; i < l; i++) {
                    if (jQuery.contains(this, targets[i])) {
                        return true;
                    }
                }
            });
        },

        not: function(selector) {
            return this.pushStack(winnow(this, selector || [], true));
        },

        filter: function(selector) {
            return this.pushStack(winnow(this, selector || [], false));
        },

        is: function(selector) {
            return !!winnow(
            this,

            // If this is a positional/relative selector, check membership in the returned set
            // so $("p:first").is("p:last") won't return true for a doc with two "p".
            typeof selector === "string" && rneedsContext.test(selector) ? jQuery(selector) : selector || [],
            false).length;
        },

        closest: function(selectors, context) {
            var cur,
            i = 0,
                l = this.length,
                matched = [],
                pos = (rneedsContext.test(selectors) || typeof selectors !== "string") ? jQuery(selectors, context || this.context) : 0;

            for (; i < l; i++) {
                for (cur = this[i]; cur && cur !== context; cur = cur.parentNode) {
                    // Always skip document fragments
                    if (cur.nodeType < 11 && (pos ? pos.index(cur) > -1 :

                    // Don't pass non-elements to Sizzle
                    cur.nodeType === 1 && jQuery.find.matchesSelector(cur, selectors))) {

                        cur = matched.push(cur);
                        break;
                    }
                }
            }

            return this.pushStack(matched.length > 1 ? jQuery.unique(matched) : matched);
        },

        // Determine the position of an element within
        // the matched set of elements
        index: function(elem) {

            // No argument, return index in parent
            if (!elem) {
                return (this[0] && this[0].parentNode) ? this.first().prevAll().length : -1;
            }

            // index in selector
            if (typeof elem === "string") {
                return core_indexOf.call(jQuery(elem), this[0]);
            }

            // Locate the position of the desired element
            return core_indexOf.call(this,

            // If it receives a jQuery object, the first element is used
            elem.jquery ? elem[0] : elem);
        },

        add: function(selector, context) {
            var set = typeof selector === "string" ? jQuery(selector, context) : jQuery.makeArray(selector && selector.nodeType ? [selector] : selector),
                all = jQuery.merge(this.get(), set);

            return this.pushStack(jQuery.unique(all));
        },

        addBack: function(selector) {
            return this.add(selector == null ? this.prevObject : this.prevObject.filter(selector));
        }
    });

    function sibling(cur, dir) {
        while ((cur = cur[dir]) && cur.nodeType !== 1) {}

        return cur;
    }

    jQuery.each({
        parent: function(elem) {
            var parent = elem.parentNode;
            return parent && parent.nodeType !== 11 ? parent : null;
        },
        parents: function(elem) {
            return jQuery.dir(elem, "parentNode");
        },
        parentsUntil: function(elem, i, until) {
            return jQuery.dir(elem, "parentNode", until);
        },
        next: function(elem) {
            return sibling(elem, "nextSibling");
        },
        prev: function(elem) {
            return sibling(elem, "previousSibling");
        },
        nextAll: function(elem) {
            return jQuery.dir(elem, "nextSibling");
        },
        prevAll: function(elem) {
            return jQuery.dir(elem, "previousSibling");
        },
        nextUntil: function(elem, i, until) {
            return jQuery.dir(elem, "nextSibling", until);
        },
        prevUntil: function(elem, i, until) {
            return jQuery.dir(elem, "previousSibling", until);
        },
        siblings: function(elem) {
            return jQuery.sibling((elem.parentNode || {}).firstChild, elem);
        },
        children: function(elem) {
            return jQuery.sibling(elem.firstChild);
        },
        contents: function(elem) {
            return elem.contentDocument || jQuery.merge([], elem.childNodes);
        }
    }, function(name, fn) {
        jQuery.fn[name] = function(until, selector) {
            var matched = jQuery.map(this, fn, until);

            if (name.slice(-5) !== "Until") {
                selector = until;
            }

            if (selector && typeof selector === "string") {
                matched = jQuery.filter(selector, matched);
            }

            if (this.length > 1) {
                // Remove duplicates
                if (!guaranteedUnique[name]) {
                    jQuery.unique(matched);
                }

                // Reverse order for parents* and prev-derivatives
                if (rparentsprev.test(name)) {
                    matched.reverse();
                }
            }

            return this.pushStack(matched);
        };
    });

    jQuery.extend({
        filter: function(expr, elems, not) {
            var elem = elems[0];

            if (not) {
                expr = ":not(" + expr + ")";
            }

            return elems.length === 1 && elem.nodeType === 1 ? jQuery.find.matchesSelector(elem, expr) ? [elem] : [] : jQuery.find.matches(expr, jQuery.grep(elems, function(elem) {
                return elem.nodeType === 1;
            }));
        },

        dir: function(elem, dir, until) {
            var matched = [],
                truncate = until !== undefined;

            while ((elem = elem[dir]) && elem.nodeType !== 9) {
                if (elem.nodeType === 1) {
                    if (truncate && jQuery(elem).is(until)) {
                        break;
                    }
                    matched.push(elem);
                }
            }
            return matched;
        },

        sibling: function(n, elem) {
            var matched = [];

            for (; n; n = n.nextSibling) {
                if (n.nodeType === 1 && n !== elem) {
                    matched.push(n);
                }
            }

            return matched;
        }
    });

    // Implement the identical functionality for filter and not
    function winnow(elements, qualifier, not) {
        if (jQuery.isFunction(qualifier)) {
            return jQuery.grep(elements, function(elem, i) {
                /* jshint -W018 */
                return !!qualifier.call(elem, i, elem) !== not;
            });

        }

        if (qualifier.nodeType) {
            return jQuery.grep(elements, function(elem) {
                return (elem === qualifier) !== not;
            });

        }

        if (typeof qualifier === "string") {
            if (isSimple.test(qualifier)) {
                return jQuery.filter(qualifier, elements, not);
            }

            qualifier = jQuery.filter(qualifier, elements);
        }

        return jQuery.grep(elements, function(elem) {
            return (core_indexOf.call(qualifier, elem) >= 0) !== not;
        });
    }
    var rxhtmlTag = /<(?!area|br|col|embed|hr|img|input|link|meta|param)(([\w:]+)[^>]*)\/>/gi,
        rtagName = /<([\w:]+)/,
        rhtml = /<|&#?\w+;/,
        rnoInnerhtml = /<(?:script|style|link)/i,
        manipulation_rcheckableType = /^(?:checkbox|radio)$/i,
        // checked="checked" or checked
        rchecked = /checked\s*(?:[^=]|=\s*.checked.)/i,
        rscriptType = /^$|\/(?:java|ecma)script/i,
        rscriptTypeMasked = /^true\/(.*)/,
        rcleanScript = /^\s*<!(?:\[CDATA\[|--)|(?:\]\]|--)>\s*$/g,

        // We have to close these tags to support XHTML (#13200)
        wrapMap = {

            // Support: IE 9
            option: [1, "<select multiple='multiple'>", "</select>"],

            thead: [1, "<table>", "</table>"],
            col: [2, "<table><colgroup>", "</colgroup></table>"],
            tr: [2, "<table><tbody>", "</tbody></table>"],
            td: [3, "<table><tbody><tr>", "</tr></tbody></table>"],

            _default: [0, "", ""]
        };

    // Support: IE 9
    wrapMap.optgroup = wrapMap.option;

    wrapMap.tbody = wrapMap.tfoot = wrapMap.colgroup = wrapMap.caption = wrapMap.thead;
    wrapMap.th = wrapMap.td;

    jQuery.fn.extend({
        text: function(value) {
            return jQuery.access(this, function(value) {
                return value === undefined ? jQuery.text(this) : this.empty().append((this[0] && this[0].ownerDocument || document).createTextNode(value));
            }, null, value, arguments.length);
        },

        append: function() {
            return this.domManip(arguments, function(elem) {
                if (this.nodeType === 1 || this.nodeType === 11 || this.nodeType === 9) {
                    var target = manipulationTarget(this, elem);
                    target.appendChild(elem);
                }
            });
        },

        prepend: function() {
            return this.domManip(arguments, function(elem) {
                if (this.nodeType === 1 || this.nodeType === 11 || this.nodeType === 9) {
                    var target = manipulationTarget(this, elem);
                    target.insertBefore(elem, target.firstChild);
                }
            });
        },

        before: function() {
            return this.domManip(arguments, function(elem) {
                if (this.parentNode) {
                    this.parentNode.insertBefore(elem, this);
                }
            });
        },

        after: function() {
            return this.domManip(arguments, function(elem) {
                if (this.parentNode) {
                    this.parentNode.insertBefore(elem, this.nextSibling);
                }
            });
        },

        // keepData is for internal use only--do not document
        remove: function(selector, keepData) {
            var elem,
            elems = selector ? jQuery.filter(selector, this) : this,
                i = 0;

            for (;
            (elem = elems[i]) != null; i++) {
                if (!keepData && elem.nodeType === 1) {
                    jQuery.cleanData(getAll(elem));
                }

                if (elem.parentNode) {
                    if (keepData && jQuery.contains(elem.ownerDocument, elem)) {
                        setGlobalEval(getAll(elem, "script"));
                    }
                    elem.parentNode.removeChild(elem);
                }
            }

            return this;
        },

        empty: function() {
            var elem,
            i = 0;

            for (;
            (elem = this[i]) != null; i++) {
                if (elem.nodeType === 1) {

                    // Prevent memory leaks
                    jQuery.cleanData(getAll(elem, false));

                    // Remove any remaining nodes
                    elem.textContent = "";
                }
            }

            return this;
        },

        clone: function(dataAndEvents, deepDataAndEvents) {
            dataAndEvents = dataAndEvents == null ? false : dataAndEvents;
            deepDataAndEvents = deepDataAndEvents == null ? dataAndEvents : deepDataAndEvents;

            return this.map(function() {
                return jQuery.clone(this, dataAndEvents, deepDataAndEvents);
            });
        },

        html: function(value) {
            return jQuery.access(this, function(value) {
                var elem = this[0] || {},
                i = 0,
                    l = this.length;

                if (value === undefined && elem.nodeType === 1) {
                    return elem.innerHTML;
                }

                // See if we can take a shortcut and just use innerHTML
                if (typeof value === "string" && !rnoInnerhtml.test(value) && !wrapMap[(rtagName.exec(value) || ["", ""])[1].toLowerCase()]) {

                    value = value.replace(rxhtmlTag, "<$1></$2>");

                    try {
                        for (; i < l; i++) {
                            elem = this[i] || {};

                            // Remove element nodes and prevent memory leaks
                            if (elem.nodeType === 1) {
                                jQuery.cleanData(getAll(elem, false));
                                elem.innerHTML = value;
                            }
                        }

                        elem = 0;

                        // If using innerHTML throws an exception, use the fallback method
                    } catch (e) {}
                }

                if (elem) {
                    this.empty().append(value);
                }
            }, null, value, arguments.length);
        },

        replaceWith: function() {
            var
            // Snapshot the DOM in case .domManip sweeps something relevant into its fragment
            args = jQuery.map(this, function(elem) {
                return [elem.nextSibling, elem.parentNode];
            }),
                i = 0;

            // Make the changes, replacing each context element with the new content
            this.domManip(arguments, function(elem) {
                var next = args[i++],
                    parent = args[i++];

                if (parent) {
                    // Don't use the snapshot next if it has moved (#13810)
                    if (next && next.parentNode !== parent) {
                        next = this.nextSibling;
                    }
                    jQuery(this).remove();
                    parent.insertBefore(elem, next);
                }
                // Allow new content to include elements from the context set
            }, true);

            // Force removal if there was no new content (e.g., from empty arguments)
            return i ? this : this.remove();
        },

        detach: function(selector) {
            return this.remove(selector, true);
        },

        domManip: function(args, callback, allowIntersection) {

            // Flatten any nested arrays
            args = core_concat.apply([], args);

            var fragment, first, scripts, hasScripts, node, doc,
            i = 0,
                l = this.length,
                set = this,
                iNoClone = l - 1,
                value = args[0],
                isFunction = jQuery.isFunction(value);

            // We can't cloneNode fragments that contain checked, in WebKit
            if (isFunction || !(l <= 1 || typeof value !== "string" || jQuery.support.checkClone || !rchecked.test(value))) {
                return this.each(function(index) {
                    var self = set.eq(index);
                    if (isFunction) {
                        args[0] = value.call(this, index, self.html());
                    }
                    self.domManip(args, callback, allowIntersection);
                });
            }

            if (l) {
                fragment = jQuery.buildFragment(args, this[0].ownerDocument, false, !allowIntersection && this);
                first = fragment.firstChild;

                if (fragment.childNodes.length === 1) {
                    fragment = first;
                }

                if (first) {
                    scripts = jQuery.map(getAll(fragment, "script"), disableScript);
                    hasScripts = scripts.length;

                    // Use the original fragment for the last item instead of the first because it can end up
                    // being emptied incorrectly in certain situations (#8070).
                    for (; i < l; i++) {
                        node = fragment;

                        if (i !== iNoClone) {
                            node = jQuery.clone(node, true, true);

                            // Keep references to cloned scripts for later restoration
                            if (hasScripts) {
                                // Support: QtWebKit
                                // jQuery.merge because core_push.apply(_, arraylike) throws
                                jQuery.merge(scripts, getAll(node, "script"));
                            }
                        }

                        callback.call(this[i], node, i);
                    }

                    if (hasScripts) {
                        doc = scripts[scripts.length - 1].ownerDocument;

                        // Reenable scripts
                        jQuery.map(scripts, restoreScript);

                        // Evaluate executable scripts on first document insertion
                        for (i = 0; i < hasScripts; i++) {
                            node = scripts[i];
                            if (rscriptType.test(node.type || "") && !data_priv.access(node, "globalEval") && jQuery.contains(doc, node)) {

                                if (node.src) {
                                    // Hope ajax is available...
                                    jQuery._evalUrl(node.src);
                                } else {
                                    jQuery.globalEval(node.textContent.replace(rcleanScript, ""));
                                }
                            }
                        }
                    }
                }
            }

            return this;
        }
    });

    jQuery.each({
        appendTo: "append",
        prependTo: "prepend",
        insertBefore: "before",
        insertAfter: "after",
        replaceAll: "replaceWith"
    }, function(name, original) {
        jQuery.fn[name] = function(selector) {
            var elems,
            ret = [],
                insert = jQuery(selector),
                last = insert.length - 1,
                i = 0;

            for (; i <= last; i++) {
                elems = i === last ? this : this.clone(true);
                jQuery(insert[i])[original](elems);

                // Support: QtWebKit
                // .get() because core_push.apply(_, arraylike) throws
                core_push.apply(ret, elems.get());
            }

            return this.pushStack(ret);
        };
    });

    jQuery.extend({
        clone: function(elem, dataAndEvents, deepDataAndEvents) {
            var i, l, srcElements, destElements,
            clone = elem.cloneNode(true),
                inPage = jQuery.contains(elem.ownerDocument, elem);

            // Support: IE >= 9
            // Fix Cloning issues
            if (!jQuery.support.noCloneChecked && (elem.nodeType === 1 || elem.nodeType === 11) && !jQuery.isXMLDoc(elem)) {

                // We eschew Sizzle here for performance reasons: http://jsperf.com/getall-vs-sizzle/2
                destElements = getAll(clone);
                srcElements = getAll(elem);

                for (i = 0, l = srcElements.length; i < l; i++) {
                    fixInput(srcElements[i], destElements[i]);
                }
            }

            // Copy the events from the original to the clone
            if (dataAndEvents) {
                if (deepDataAndEvents) {
                    srcElements = srcElements || getAll(elem);
                    destElements = destElements || getAll(clone);

                    for (i = 0, l = srcElements.length; i < l; i++) {
                        cloneCopyEvent(srcElements[i], destElements[i]);
                    }
                } else {
                    cloneCopyEvent(elem, clone);
                }
            }

            // Preserve script evaluation history
            destElements = getAll(clone, "script");
            if (destElements.length > 0) {
                setGlobalEval(destElements, !inPage && getAll(elem, "script"));
            }

            // Return the cloned set
            return clone;
        },

        buildFragment: function(elems, context, scripts, selection) {
            var elem, tmp, tag, wrap, contains, j,
            i = 0,
                l = elems.length,
                fragment = context.createDocumentFragment(),
                nodes = [];

            for (; i < l; i++) {
                elem = elems[i];

                if (elem || elem === 0) {

                    // Add nodes directly
                    if (jQuery.type(elem) === "object") {
                        // Support: QtWebKit
                        // jQuery.merge because core_push.apply(_, arraylike) throws
                        jQuery.merge(nodes, elem.nodeType ? [elem] : elem);

                        // Convert non-html into a text node
                    } else if (!rhtml.test(elem)) {
                        nodes.push(context.createTextNode(elem));

                        // Convert html into DOM nodes
                    } else {
                        tmp = tmp || fragment.appendChild(context.createElement("div"));

                        // Deserialize a standard representation
                        tag = (rtagName.exec(elem) || ["", ""])[1].toLowerCase();
                        wrap = wrapMap[tag] || wrapMap._default;
                        tmp.innerHTML = wrap[1] + elem.replace(rxhtmlTag, "<$1></$2>") + wrap[2];

                        // Descend through wrappers to the right content
                        j = wrap[0];
                        while (j--) {
                            tmp = tmp.lastChild;
                        }

                        // Support: QtWebKit
                        // jQuery.merge because core_push.apply(_, arraylike) throws
                        jQuery.merge(nodes, tmp.childNodes);

                        // Remember the top-level container
                        tmp = fragment.firstChild;

                        // Fixes #12346
                        // Support: Webkit, IE
                        tmp.textContent = "";
                    }
                }
            }

            // Remove wrapper from fragment
            fragment.textContent = "";

            i = 0;
            while ((elem = nodes[i++])) {

                // #4087 - If origin and destination elements are the same, and this is
                // that element, do not do anything
                if (selection && jQuery.inArray(elem, selection) !== -1) {
                    continue;
                }

                contains = jQuery.contains(elem.ownerDocument, elem);

                // Append to fragment
                tmp = getAll(fragment.appendChild(elem), "script");

                // Preserve script evaluation history
                if (contains) {
                    setGlobalEval(tmp);
                }

                // Capture executables
                if (scripts) {
                    j = 0;
                    while ((elem = tmp[j++])) {
                        if (rscriptType.test(elem.type || "")) {
                            scripts.push(elem);
                        }
                    }
                }
            }

            return fragment;
        },

        cleanData: function(elems) {
            var data, elem, events, type, key, j,
            special = jQuery.event.special,
                i = 0;

            for (;
            (elem = elems[i]) !== undefined; i++) {
                if (Data.accepts(elem)) {
                    key = elem[data_priv.expando];

                    if (key && (data = data_priv.cache[key])) {
                        events = Object.keys(data.events || {});
                        if (events.length) {
                            for (j = 0;
                            (type = events[j]) !== undefined; j++) {
                                if (special[type]) {
                                    jQuery.event.remove(elem, type);

                                    // This is a shortcut to avoid jQuery.event.remove's overhead
                                } else {
                                    jQuery.removeEvent(elem, type, data.handle);
                                }
                            }
                        }
                        if (data_priv.cache[key]) {
                            // Discard any remaining `private` data
                            delete data_priv.cache[key];
                        }
                    }
                }
                // Discard any remaining `user` data
                delete data_user.cache[elem[data_user.expando]];
            }
        },

        _evalUrl: function(url) {
            return jQuery.ajax({
                url: url,
                type: "GET",
                dataType: "script",
                async: false,
                global: false,
                "throws": true
            });
        }
    });

    // Support: 1.x compatibility
    // Manipulating tables requires a tbody
    function manipulationTarget(elem, content) {
        return jQuery.nodeName(elem, "table") && jQuery.nodeName(content.nodeType === 1 ? content : content.firstChild, "tr") ?

        elem.getElementsByTagName("tbody")[0] || elem.appendChild(elem.ownerDocument.createElement("tbody")) : elem;
    }

    // Replace/restore the type attribute of script elements for safe DOM manipulation
    function disableScript(elem) {
        elem.type = (elem.getAttribute("type") !== null) + "/" + elem.type;
        return elem;
    }

    function restoreScript(elem) {
        var match = rscriptTypeMasked.exec(elem.type);

        if (match) {
            elem.type = match[1];
        } else {
            elem.removeAttribute("type");
        }

        return elem;
    }

    // Mark scripts as having already been evaluated
    function setGlobalEval(elems, refElements) {
        var l = elems.length,
            i = 0;

        for (; i < l; i++) {
            data_priv.set(
            elems[i], "globalEval", !refElements || data_priv.get(refElements[i], "globalEval"));
        }
    }

    function cloneCopyEvent(src, dest) {
        var i, l, type, pdataOld, pdataCur, udataOld, udataCur, events;

        if (dest.nodeType !== 1) {
            return;
        }

        // 1. Copy private data: events, handlers, etc.
        if (data_priv.hasData(src)) {
            pdataOld = data_priv.access(src);
            pdataCur = data_priv.set(dest, pdataOld);
            events = pdataOld.events;

            if (events) {
                delete pdataCur.handle;
                pdataCur.events = {};

                for (type in events) {
                    for (i = 0, l = events[type].length; i < l; i++) {
                        jQuery.event.add(dest, type, events[type][i]);
                    }
                }
            }
        }

        // 2. Copy user data
        if (data_user.hasData(src)) {
            udataOld = data_user.access(src);
            udataCur = jQuery.extend({}, udataOld);

            data_user.set(dest, udataCur);
        }
    }


    function getAll(context, tag) {
        var ret = context.getElementsByTagName ? context.getElementsByTagName(tag || "*") : context.querySelectorAll ? context.querySelectorAll(tag || "*") : [];

        return tag === undefined || tag && jQuery.nodeName(context, tag) ? jQuery.merge([context], ret) : ret;
    }

    // Support: IE >= 9
    function fixInput(src, dest) {
        var nodeName = dest.nodeName.toLowerCase();

        // Fails to persist the checked state of a cloned checkbox or radio button.
        if (nodeName === "input" && manipulation_rcheckableType.test(src.type)) {
            dest.checked = src.checked;

            // Fails to return the selected option to the default selected state when cloning options
        } else if (nodeName === "input" || nodeName === "textarea") {
            dest.defaultValue = src.defaultValue;
        }
    }
    jQuery.fn.extend({
        wrapAll: function(html) {
            var wrap;

            if (jQuery.isFunction(html)) {
                return this.each(function(i) {
                    jQuery(this).wrapAll(html.call(this, i));
                });
            }

            if (this[0]) {

                // The elements to wrap the target around
                wrap = jQuery(html, this[0].ownerDocument).eq(0).clone(true);

                if (this[0].parentNode) {
                    wrap.insertBefore(this[0]);
                }

                wrap.map(function() {
                    var elem = this;

                    while (elem.firstElementChild) {
                        elem = elem.firstElementChild;
                    }

                    return elem;
                }).append(this);
            }

            return this;
        },

        wrapInner: function(html) {
            if (jQuery.isFunction(html)) {
                return this.each(function(i) {
                    jQuery(this).wrapInner(html.call(this, i));
                });
            }

            return this.each(function() {
                var self = jQuery(this),
                    contents = self.contents();

                if (contents.length) {
                    contents.wrapAll(html);

                } else {
                    self.append(html);
                }
            });
        },

        wrap: function(html) {
            var isFunction = jQuery.isFunction(html);

            return this.each(function(i) {
                jQuery(this).wrapAll(isFunction ? html.call(this, i) : html);
            });
        },

        unwrap: function() {
            return this.parent().each(function() {
                if (!jQuery.nodeName(this, "body")) {
                    jQuery(this).replaceWith(this.childNodes);
                }
            }).end();
        }
    });
    var curCSS, iframe,
    // swappable if display is none or starts with table except "table", "table-cell", or "table-caption"
    // see here for display values: https://developer.mozilla.org/en-US/docs/CSS/display
    rdisplayswap = /^(none|table(?!-c[ea]).+)/,
        rmargin = /^margin/,
        rnumsplit = new RegExp("^(" + core_pnum + ")(.*)$", "i"),
        rnumnonpx = new RegExp("^(" + core_pnum + ")(?!px)[a-z%]+$", "i"),
        rrelNum = new RegExp("^([+-])=(" + core_pnum + ")", "i"),
        elemdisplay = {
            BODY: "block"
        },

        cssShow = {
            position: "absolute",
            visibility: "hidden",
            display: "block"
        },
        cssNormalTransform = {
            letterSpacing: 0,
            fontWeight: 400
        },

        cssExpand = ["Top", "Right", "Bottom", "Left"],
        cssPrefixes = ["Webkit", "O", "Moz", "ms"];

    // return a css property mapped to a potentially vendor prefixed property
    function vendorPropName(style, name) {

        // shortcut for names that are not vendor prefixed
        if (name in style) {
            return name;
        }

        // check for vendor prefixed names
        var capName = name.charAt(0).toUpperCase() + name.slice(1),
            origName = name,
            i = cssPrefixes.length;

        while (i--) {
            name = cssPrefixes[i] + capName;
            if (name in style) {
                return name;
            }
        }

        return origName;
    }

    function isHidden(elem, el) {
        // isHidden might be called from jQuery#filter function;
        // in that case, element will be second argument
        elem = el || elem;
        return jQuery.css(elem, "display") === "none" || !jQuery.contains(elem.ownerDocument, elem);
    }

    // NOTE: we've included the "window" in window.getComputedStyle
    // because jsdom on node.js will break without it.
    function getStyles(elem) {
        return window.getComputedStyle(elem, null);
    }

    function showHide(elements, show) {
        var display, elem, hidden,
        values = [],
            index = 0,
            length = elements.length;

        for (; index < length; index++) {
            elem = elements[index];
            if (!elem.style) {
                continue;
            }

            values[index] = data_priv.get(elem, "olddisplay");
            display = elem.style.display;
            if (show) {
                // Reset the inline display of this element to learn if it is
                // being hidden by cascaded rules or not
                if (!values[index] && display === "none") {
                    elem.style.display = "";
                }

                // Set elements which have been overridden with display: none
                // in a stylesheet to whatever the default browser style is
                // for such an element
                if (elem.style.display === "" && isHidden(elem)) {
                    values[index] = data_priv.access(elem, "olddisplay", css_defaultDisplay(elem.nodeName));
                }
            } else {

                if (!values[index]) {
                    hidden = isHidden(elem);

                    if (display && display !== "none" || !hidden) {
                        data_priv.set(elem, "olddisplay", hidden ? display : jQuery.css(elem, "display"));
                    }
                }
            }
        }

        // Set the display of most of the elements in a second loop
        // to avoid the constant reflow
        for (index = 0; index < length; index++) {
            elem = elements[index];
            if (!elem.style) {
                continue;
            }
            if (!show || elem.style.display === "none" || elem.style.display === "") {
                elem.style.display = show ? values[index] || "" : "none";
            }
        }

        return elements;
    }

    jQuery.fn.extend({
        css: function(name, value) {
            return jQuery.access(this, function(elem, name, value) {
                var styles, len,
                map = {},
                i = 0;

                if (jQuery.isArray(name)) {
                    styles = getStyles(elem);
                    len = name.length;

                    for (; i < len; i++) {
                        map[name[i]] = jQuery.css(elem, name[i], false, styles);
                    }

                    return map;
                }

                return value !== undefined ? jQuery.style(elem, name, value) : jQuery.css(elem, name);
            }, name, value, arguments.length > 1);
        },
        show: function() {
            return showHide(this, true);
        },
        hide: function() {
            return showHide(this);
        },
        toggle: function(state) {
            if (typeof state === "boolean") {
                return state ? this.show() : this.hide();
            }

            return this.each(function() {
                if (isHidden(this)) {
                    jQuery(this).show();
                } else {
                    jQuery(this).hide();
                }
            });
        }
    });

    jQuery.extend({
        // Add in style property hooks for overriding the default
        // behavior of getting and setting a style property
        cssHooks: {
            opacity: {
                get: function(elem, computed) {
                    if (computed) {
                        // We should always get a number back from opacity
                        var ret = curCSS(elem, "opacity");
                        return ret === "" ? "1" : ret;
                    }
                }
            }
        },

        // Don't automatically add "px" to these possibly-unitless properties
        cssNumber: {
            "columnCount": true,
            "fillOpacity": true,
            "fontWeight": true,
            "lineHeight": true,
            "opacity": true,
            "order": true,
            "orphans": true,
            "widows": true,
            "zIndex": true,
            "zoom": true
        },

        // Add in properties whose names you wish to fix before
        // setting or getting the value
        cssProps: {
            // normalize float css property
            "float": "cssFloat"
        },

        // Get and set the style property on a DOM Node
        style: function(elem, name, value, extra) {
            // Don't set styles on text and comment nodes
            if (!elem || elem.nodeType === 3 || elem.nodeType === 8 || !elem.style) {
                return;
            }

            // Make sure that we're working with the right name
            var ret, type, hooks,
            origName = jQuery.camelCase(name),
                style = elem.style;

            name = jQuery.cssProps[origName] || (jQuery.cssProps[origName] = vendorPropName(style, origName));

            // gets hook for the prefixed version
            // followed by the unprefixed version
            hooks = jQuery.cssHooks[name] || jQuery.cssHooks[origName];

            // Check if we're setting a value
            if (value !== undefined) {
                type = typeof value;

                // convert relative number strings (+= or -=) to relative numbers. #7345
                if (type === "string" && (ret = rrelNum.exec(value))) {
                    value = (ret[1] + 1) * ret[2] + parseFloat(jQuery.css(elem, name));
                    // Fixes bug #9237
                    type = "number";
                }

                // Make sure that NaN and null values aren't set. See: #7116
                if (value == null || type === "number" && isNaN(value)) {
                    return;
                }

                // If a number was passed in, add 'px' to the (except for certain CSS properties)
                if (type === "number" && !jQuery.cssNumber[origName]) {
                    value += "px";
                }

                // Fixes #8908, it can be done more correctly by specifying setters in cssHooks,
                // but it would mean to define eight (for every problematic property) identical functions
                if (!jQuery.support.clearCloneStyle && value === "" && name.indexOf("background") === 0) {
                    style[name] = "inherit";
                }

                // If a hook was provided, use that value, otherwise just set the specified value
                if (!hooks || !("set" in hooks) || (value = hooks.set(elem, value, extra)) !== undefined) {
                    style[name] = value;
                }

            } else {
                // If a hook was provided get the non-computed value from there
                if (hooks && "get" in hooks && (ret = hooks.get(elem, false, extra)) !== undefined) {
                    return ret;
                }

                // Otherwise just get the value from the style object
                return style[name];
            }
        },

        css: function(elem, name, extra, styles) {
            var val, num, hooks,
            origName = jQuery.camelCase(name);

            // Make sure that we're working with the right name
            name = jQuery.cssProps[origName] || (jQuery.cssProps[origName] = vendorPropName(elem.style, origName));

            // gets hook for the prefixed version
            // followed by the unprefixed version
            hooks = jQuery.cssHooks[name] || jQuery.cssHooks[origName];

            // If a hook was provided get the computed value from there
            if (hooks && "get" in hooks) {
                val = hooks.get(elem, true, extra);
            }

            // Otherwise, if a way to get the computed value exists, use that
            if (val === undefined) {
                val = curCSS(elem, name, styles);
            }

            //convert "normal" to computed value
            if (val === "normal" && name in cssNormalTransform) {
                val = cssNormalTransform[name];
            }

            // Return, converting to number if forced or a qualifier was provided and val looks numeric
            if (extra === "" || extra) {
                num = parseFloat(val);
                return extra === true || jQuery.isNumeric(num) ? num || 0 : val;
            }
            return val;
        }
    });

    curCSS = function(elem, name, _computed) {
        var width, minWidth, maxWidth,
        computed = _computed || getStyles(elem),

            // Support: IE9
            // getPropertyValue is only needed for .css('filter') in IE9, see #12537
            ret = computed ? computed.getPropertyValue(name) || computed[name] : undefined,
            style = elem.style;

        if (computed) {

            if (ret === "" && !jQuery.contains(elem.ownerDocument, elem)) {
                ret = jQuery.style(elem, name);
            }

            // Support: Safari 5.1
            // A tribute to the "awesome hack by Dean Edwards"
            // Safari 5.1.7 (at least) returns percentage for a larger set of values, but width seems to be reliably pixels
            // this is against the CSSOM draft spec: http://dev.w3.org/csswg/cssom/#resolved-values
            if (rnumnonpx.test(ret) && rmargin.test(name)) {

                // Remember the original values
                width = style.width;
                minWidth = style.minWidth;
                maxWidth = style.maxWidth;

                // Put in the new values to get a computed value out
                style.minWidth = style.maxWidth = style.width = ret;
                ret = computed.width;

                // Revert the changed values
                style.width = width;
                style.minWidth = minWidth;
                style.maxWidth = maxWidth;
            }
        }

        return ret;
    };


    function setPositiveNumber(elem, value, subtract) {
        var matches = rnumsplit.exec(value);
        return matches ?
        // Guard against undefined "subtract", e.g., when used as in cssHooks
        Math.max(0, matches[1] - (subtract || 0)) + (matches[2] || "px") : value;
    }

    function augmentWidthOrHeight(elem, name, extra, isBorderBox, styles) {
        var i = extra === (isBorderBox ? "border" : "content") ?
        // If we already have the right measurement, avoid augmentation
        4 :
        // Otherwise initialize for horizontal or vertical properties
        name === "width" ? 1 : 0,

            val = 0;

        for (; i < 4; i += 2) {
            // both box models exclude margin, so add it if we want it
            if (extra === "margin") {
                val += jQuery.css(elem, extra + cssExpand[i], true, styles);
            }

            if (isBorderBox) {
                // border-box includes padding, so remove it if we want content
                if (extra === "content") {
                    val -= jQuery.css(elem, "padding" + cssExpand[i], true, styles);
                }

                // at this point, extra isn't border nor margin, so remove border
                if (extra !== "margin") {
                    val -= jQuery.css(elem, "border" + cssExpand[i] + "Width", true, styles);
                }
            } else {
                // at this point, extra isn't content, so add padding
                val += jQuery.css(elem, "padding" + cssExpand[i], true, styles);

                // at this point, extra isn't content nor padding, so add border
                if (extra !== "padding") {
                    val += jQuery.css(elem, "border" + cssExpand[i] + "Width", true, styles);
                }
            }
        }

        return val;
    }

    function getWidthOrHeight(elem, name, extra) {

        // Start with offset property, which is equivalent to the border-box value
        var valueIsBorderBox = true,
            val = name === "width" ? elem.offsetWidth : elem.offsetHeight,
            styles = getStyles(elem),
            isBorderBox = jQuery.support.boxSizing && jQuery.css(elem, "boxSizing", false, styles) === "border-box";

        // some non-html elements return undefined for offsetWidth, so check for null/undefined
        // svg - https://bugzilla.mozilla.org/show_bug.cgi?id=649285
        // MathML - https://bugzilla.mozilla.org/show_bug.cgi?id=491668
        if (val <= 0 || val == null) {
            // Fall back to computed then uncomputed css if necessary
            val = curCSS(elem, name, styles);
            if (val < 0 || val == null) {
                val = elem.style[name];
            }

            // Computed unit is not pixels. Stop here and return.
            if (rnumnonpx.test(val)) {
                return val;
            }

            // we need the check for style in case a browser which returns unreliable values
            // for getComputedStyle silently falls back to the reliable elem.style
            valueIsBorderBox = isBorderBox && (jQuery.support.boxSizingReliable || val === elem.style[name]);

            // Normalize "", auto, and prepare for extra
            val = parseFloat(val) || 0;
        }

        // use the active box-sizing model to add/subtract irrelevant styles
        return (val + augmentWidthOrHeight(
        elem,
        name,
        extra || (isBorderBox ? "border" : "content"),
        valueIsBorderBox,
        styles)) + "px";
    }

    // Try to determine the default display value of an element
    function css_defaultDisplay(nodeName) {
        var doc = document,
            display = elemdisplay[nodeName];

        if (!display) {
            display = actualDisplay(nodeName, doc);

            // If the simple way fails, read from inside an iframe
            if (display === "none" || !display) {
                // Use the already-created iframe if possible
                iframe = (iframe || jQuery("<iframe frameborder='0' width='0' height='0'/>")
                    .css("cssText", "display:block !important")).appendTo(doc.documentElement);

                // Always write a new HTML skeleton so Webkit and Firefox don't choke on reuse
                doc = (iframe[0].contentWindow || iframe[0].contentDocument).document;
                doc.write("<!doctype html><html><body>");
                doc.close();

                display = actualDisplay(nodeName, doc);
                iframe.detach();
            }

            // Store the correct default display
            elemdisplay[nodeName] = display;
        }

        return display;
    }

    // Called ONLY from within css_defaultDisplay
    function actualDisplay(name, doc) {
        var elem = jQuery(doc.createElement(name)).appendTo(doc.body),
            display = jQuery.css(elem[0], "display");
        elem.remove();
        return display;
    }

    jQuery.each(["height", "width"], function(i, name) {
        jQuery.cssHooks[name] = {
            get: function(elem, computed, extra) {
                if (computed) {
                    // certain elements can have dimension info if we invisibly show them
                    // however, it must have a current display style that would benefit from this
                    return elem.offsetWidth === 0 && rdisplayswap.test(jQuery.css(elem, "display")) ? jQuery.swap(elem, cssShow, function() {
                        return getWidthOrHeight(elem, name, extra);
                    }) : getWidthOrHeight(elem, name, extra);
                }
            },

            set: function(elem, value, extra) {
                var styles = extra && getStyles(elem);
                return setPositiveNumber(elem, value, extra ? augmentWidthOrHeight(
                elem,
                name,
                extra,
                jQuery.support.boxSizing && jQuery.css(elem, "boxSizing", false, styles) === "border-box",
                styles) : 0);
            }
        };
    });

    // These hooks cannot be added until DOM ready because the support test
    // for it is not run until after DOM ready
    jQuery(function() {
        // Support: Android 2.3
        if (!jQuery.support.reliableMarginRight) {
            jQuery.cssHooks.marginRight = {
                get: function(elem, computed) {
                    if (computed) {
                        // Support: Android 2.3
                        // WebKit Bug 13343 - getComputedStyle returns wrong value for margin-right
                        // Work around by temporarily setting element display to inline-block
                        return jQuery.swap(elem, {
                            "display": "inline-block"
                        },
                        curCSS, [elem, "marginRight"]);
                    }
                }
            };
        }

        // Webkit bug: https://bugs.webkit.org/show_bug.cgi?id=29084
        // getComputedStyle returns percent when specified for top/left/bottom/right
        // rather than make the css module depend on the offset module, we just check for it here
        if (!jQuery.support.pixelPosition && jQuery.fn.position) {
            jQuery.each(["top", "left"], function(i, prop) {
                jQuery.cssHooks[prop] = {
                    get: function(elem, computed) {
                        if (computed) {
                            computed = curCSS(elem, prop);
                            // if curCSS returns percentage, fallback to offset
                            return rnumnonpx.test(computed) ? jQuery(elem).position()[prop] + "px" : computed;
                        }
                    }
                };
            });
        }

    });

    if (jQuery.expr && jQuery.expr.filters) {
        jQuery.expr.filters.hidden = function(elem) {
            // Support: Opera <= 12.12
            // Opera reports offsetWidths and offsetHeights less than zero on some elements
            return elem.offsetWidth <= 0 && elem.offsetHeight <= 0;
        };

        jQuery.expr.filters.visible = function(elem) {
            return !jQuery.expr.filters.hidden(elem);
        };
    }

    // These hooks are used by animate to expand properties
    jQuery.each({
        margin: "",
        padding: "",
        border: "Width"
    }, function(prefix, suffix) {
        jQuery.cssHooks[prefix + suffix] = {
            expand: function(value) {
                var i = 0,
                    expanded = {},

                    // assumes a single number if not a string
                    parts = typeof value === "string" ? value.split(" ") : [value];

                for (; i < 4; i++) {
                    expanded[prefix + cssExpand[i] + suffix] = parts[i] || parts[i - 2] || parts[0];
                }

                return expanded;
            }
        };

        if (!rmargin.test(prefix)) {
            jQuery.cssHooks[prefix + suffix].set = setPositiveNumber;
        }
    });
    var r20 = /%20/g,
        rbracket = /\[\]$/,
        rCRLF = /\r?\n/g,
        rsubmitterTypes = /^(?:submit|button|image|reset|file)$/i,
        rsubmittable = /^(?:input|select|textarea|keygen)/i;

    jQuery.fn.extend({
        serialize: function() {
            return jQuery.param(this.serializeArray());
        },
        serializeArray: function() {
            return this.map(function() {
                // Can add propHook for "elements" to filter or add form elements
                var elements = jQuery.prop(this, "elements");
                return elements ? jQuery.makeArray(elements) : this;
            })
                .filter(function() {
                var type = this.type;
                // Use .is(":disabled") so that fieldset[disabled] works
                return this.name && !jQuery(this).is(":disabled") && rsubmittable.test(this.nodeName) && !rsubmitterTypes.test(type) && (this.checked || !manipulation_rcheckableType.test(type));
            })
                .map(function(i, elem) {
                var val = jQuery(this).val();

                return val == null ? null : jQuery.isArray(val) ? jQuery.map(val, function(val) {
                    return {
                        name: elem.name,
                        value: val.replace(rCRLF, "\r\n")
                    };
                }) : {
                    name: elem.name,
                    value: val.replace(rCRLF, "\r\n")
                };
            }).get();
        }
    });

    //Serialize an array of form elements or a set of
    //key/values into a query string
    jQuery.param = function(a, traditional) {
        var prefix,
        s = [],
            add = function(key, value) {
                // If value is a function, invoke it and return its value
                value = jQuery.isFunction(value) ? value() : (value == null ? "" : value);
                s[s.length] = encodeURIComponent(key) + "=" + encodeURIComponent(value);
            };

        // Set traditional to true for jQuery <= 1.3.2 behavior.
        if (traditional === undefined) {
            traditional = jQuery.ajaxSettings && jQuery.ajaxSettings.traditional;
        }

        // If an array was passed in, assume that it is an array of form elements.
        if (jQuery.isArray(a) || (a.jquery && !jQuery.isPlainObject(a))) {
            // Serialize the form elements
            jQuery.each(a, function() {
                add(this.name, this.value);
            });

        } else {
            // If traditional, encode the "old" way (the way 1.3.2 or older
            // did it), otherwise encode params recursively.
            for (prefix in a) {
                buildParams(prefix, a[prefix], traditional, add);
            }
        }

        // Return the resulting serialization
        return s.join("&").replace(r20, "+");
    };

    function buildParams(prefix, obj, traditional, add) {
        var name;

        if (jQuery.isArray(obj)) {
            // Serialize array item.
            jQuery.each(obj, function(i, v) {
                if (traditional || rbracket.test(prefix)) {
                    // Treat each array item as a scalar.
                    add(prefix, v);

                } else {
                    // Item is non-scalar (array or object), encode its numeric index.
                    buildParams(prefix + "[" + (typeof v === "object" ? i : "") + "]", v, traditional, add);
                }
            });

        } else if (!traditional && jQuery.type(obj) === "object") {
            // Serialize object item.
            for (name in obj) {
                buildParams(prefix + "[" + name + "]", obj[name], traditional, add);
            }

        } else {
            // Serialize scalar item.
            add(prefix, obj);
        }
    }
    jQuery.each(("blur focus focusin focusout load resize scroll unload click dblclick " + "mousedown mouseup mousemove mouseover mouseout mouseenter mouseleave " + "change select submit keydown keypress keyup error contextmenu").split(" "), function(i, name) {

        // Handle event binding
        jQuery.fn[name] = function(data, fn) {
            return arguments.length > 0 ? this.on(name, null, data, fn) : this.trigger(name);
        };
    });

    jQuery.fn.extend({
        hover: function(fnOver, fnOut) {
            return this.mouseenter(fnOver).mouseleave(fnOut || fnOver);
        },

        bind: function(types, data, fn) {
            return this.on(types, null, data, fn);
        },
        unbind: function(types, fn) {
            return this.off(types, null, fn);
        },

        delegate: function(selector, types, data, fn) {
            return this.on(types, selector, data, fn);
        },
        undelegate: function(selector, types, fn) {
            // ( namespace ) or ( selector, types [, fn] )
            return arguments.length === 1 ? this.off(selector, "**") : this.off(types, selector || "**", fn);
        }
    });
    var
    // Document location
    ajaxLocParts,
    ajaxLocation,

    ajax_nonce = jQuery.now(),

        ajax_rquery = /\?/,
        rhash = /#.*$/,
        rts = /([?&])_=[^&]*/,
        rheaders = /^(.*?):[ \t]*([^\r\n]*)$/mg,
        // #7653, #8125, #8152: local protocol detection
        rlocalProtocol = /^(?:about|app|app-storage|.+-extension|file|res|widget):$/,
        rnoContent = /^(?:GET|HEAD)$/,
        rprotocol = /^\/\//,
        rurl = /^([\w.+-]+:)(?:\/\/([^\/?#:]*)(?::(\d+)|)|)/,

        // Keep a copy of the old load method
        _load = jQuery.fn.load,

        /* Prefilters
         * 1) They are useful to introduce custom dataTypes (see ajax/jsonp.js for an example)
         * 2) These are called:
         *    - BEFORE asking for a transport
         *    - AFTER param serialization (s.data is a string if s.processData is true)
         * 3) key is the dataType
         * 4) the catchall symbol "*" can be used
         * 5) execution will start with transport dataType and THEN continue down to "*" if needed
         */
        prefilters = {},

        /* Transports bindings
         * 1) key is the dataType
         * 2) the catchall symbol "*" can be used
         * 3) selection will start with transport dataType and THEN go to "*" if needed
         */
        transports = {},

        // Avoid comment-prolog char sequence (#10098); must appease lint and evade compression
        allTypes = "*/".concat("*");

    // #8138, IE may throw an exception when accessing
    // a field from window.location if document.domain has been set
    try {
        ajaxLocation = location.href;
    } catch (e) {
        // Use the href attribute of an A element
        // since IE will modify it given document.location
        ajaxLocation = document.createElement("a");
        ajaxLocation.href = "";
        ajaxLocation = ajaxLocation.href;
    }

    // Segment location into parts
    ajaxLocParts = rurl.exec(ajaxLocation.toLowerCase()) || [];

    // Base "constructor" for jQuery.ajaxPrefilter and jQuery.ajaxTransport
    function addToPrefiltersOrTransports(structure) {

        // dataTypeExpression is optional and defaults to "*"
        return function(dataTypeExpression, func) {

            if (typeof dataTypeExpression !== "string") {
                func = dataTypeExpression;
                dataTypeExpression = "*";
            }

            var dataType,
            i = 0,
                dataTypes = dataTypeExpression.toLowerCase().match(core_rnotwhite) || [];

            if (jQuery.isFunction(func)) {
                // For each dataType in the dataTypeExpression
                while ((dataType = dataTypes[i++])) {
                    // Prepend if requested
                    if (dataType[0] === "+") {
                        dataType = dataType.slice(1) || "*";
                        (structure[dataType] = structure[dataType] || []).unshift(func);

                        // Otherwise append
                    } else {
                        (structure[dataType] = structure[dataType] || []).push(func);
                    }
                }
            }
        };
    }

    // Base inspection function for prefilters and transports
    function inspectPrefiltersOrTransports(structure, options, originalOptions, jqXHR) {

        var inspected = {},
        seekingTransport = (structure === transports);

        function inspect(dataType) {
            var selected;
            inspected[dataType] = true;
            jQuery.each(structure[dataType] || [], function(_, prefilterOrFactory) {
                var dataTypeOrTransport = prefilterOrFactory(options, originalOptions, jqXHR);
                if (typeof dataTypeOrTransport === "string" && !seekingTransport && !inspected[dataTypeOrTransport]) {
                    options.dataTypes.unshift(dataTypeOrTransport);
                    inspect(dataTypeOrTransport);
                    return false;
                } else if (seekingTransport) {
                    return !(selected = dataTypeOrTransport);
                }
            });
            return selected;
        }

        return inspect(options.dataTypes[0]) || !inspected["*"] && inspect("*");
    }

    // A special extend for ajax options
    // that takes "flat" options (not to be deep extended)
    // Fixes #9887
    function ajaxExtend(target, src) {
        var key, deep,
        flatOptions = jQuery.ajaxSettings.flatOptions || {};

        for (key in src) {
            if (src[key] !== undefined) {
                (flatOptions[key] ? target : (deep || (deep = {})))[key] = src[key];
            }
        }
        if (deep) {
            jQuery.extend(true, target, deep);
        }

        return target;
    }

    jQuery.fn.load = function(url, params, callback) {
        if (typeof url !== "string" && _load) {
            return _load.apply(this, arguments);
        }

        var selector, type, response,
        self = this,
            off = url.indexOf(" ");

        if (off >= 0) {
            selector = url.slice(off);
            url = url.slice(0, off);
        }

        // If it's a function
        if (jQuery.isFunction(params)) {

            // We assume that it's the callback
            callback = params;
            params = undefined;

            // Otherwise, build a param string
        } else if (params && typeof params === "object") {
            type = "POST";
        }

        // If we have elements to modify, make the request
        if (self.length > 0) {
            jQuery.ajax({
                url: url,

                // if "type" variable is undefined, then "GET" method will be used
                type: type,
                dataType: "html",
                data: params
            }).done(function(responseText) {

                // Save response for use in complete callback
                response = arguments;

                self.html(selector ?

                // If a selector was specified, locate the right elements in a dummy div
                // Exclude scripts to avoid IE 'Permission Denied' errors
                jQuery("<div>").append(jQuery.parseHTML(responseText)).find(selector) :

                // Otherwise use the full result
                responseText);

            }).complete(callback && function(jqXHR, status) {
                self.each(callback, response || [jqXHR.responseText, status, jqXHR]);
            });
        }

        return this;
    };

    // Attach a bunch of functions for handling common AJAX events
    jQuery.each(["ajaxStart", "ajaxStop", "ajaxComplete", "ajaxError", "ajaxSuccess", "ajaxSend"], function(i, type) {
        jQuery.fn[type] = function(fn) {
            return this.on(type, fn);
        };
    });

    jQuery.extend({

        // Counter for holding the number of active queries
        active: 0,

        // Last-Modified header cache for next request
        lastModified: {},
        etag: {},

        ajaxSettings: {
            url: ajaxLocation,
            type: "GET",
            isLocal: rlocalProtocol.test(ajaxLocParts[1]),
            global: true,
            processData: true,
            async: true,
            contentType: "application/x-www-form-urlencoded; charset=UTF-8",
            /*
		timeout: 0,
		data: null,
		dataType: null,
		username: null,
		password: null,
		cache: null,
		throws: false,
		traditional: false,
		headers: {},
		*/

            accepts: {
                "*": allTypes,
                text: "text/plain",
                html: "text/html",
                xml: "application/xml, text/xml",
                json: "application/json, text/javascript"
            },

            contents: {
                xml: /xml/,
                html: /html/,
                json: /json/
            },

            responseFields: {
                xml: "responseXML",
                text: "responseText",
                json: "responseJSON"
            },

            // Data converters
            // Keys separate source (or catchall "*") and destination types with a single space
            converters: {

                // Convert anything to text
                "* text": String,

                // Text to html (true = no transformation)
                "text html": true,

                // Evaluate text as a json expression
                "text json": jQuery.parseJSON,

                // Parse text as xml
                "text xml": jQuery.parseXML
            },

            // For options that shouldn't be deep extended:
            // you can add your own custom options here if
            // and when you create one that shouldn't be
            // deep extended (see ajaxExtend)
            flatOptions: {
                url: true,
                context: true
            }
        },

        // Creates a full fledged settings object into target
        // with both ajaxSettings and settings fields.
        // If target is omitted, writes into ajaxSettings.
        ajaxSetup: function(target, settings) {
            return settings ?

            // Building a settings object
            ajaxExtend(ajaxExtend(target, jQuery.ajaxSettings), settings) :

            // Extending ajaxSettings
            ajaxExtend(jQuery.ajaxSettings, target);
        },

        ajaxPrefilter: addToPrefiltersOrTransports(prefilters),
        ajaxTransport: addToPrefiltersOrTransports(transports),

        // Main method
        ajax: function(url, options) {

            // If url is an object, simulate pre-1.5 signature
            if (typeof url === "object") {
                options = url;
                url = undefined;
            }

            // Force options to be an object
            options = options || {};

            var transport,
            // URL without anti-cache param
            cacheURL,
            // Response headers
            responseHeadersString,
            responseHeaders,
            // timeout handle
            timeoutTimer,
            // Cross-domain detection vars
            parts,
            // To know if global events are to be dispatched
            fireGlobals,
            // Loop variable
            i,
            // Create the final options object
            s = jQuery.ajaxSetup({}, options),
                // Callbacks context
                callbackContext = s.context || s,
                // Context for global events is callbackContext if it is a DOM node or jQuery collection
                globalEventContext = s.context && (callbackContext.nodeType || callbackContext.jquery) ? jQuery(callbackContext) : jQuery.event,
                // Deferreds
                deferred = jQuery.Deferred(),
                completeDeferred = jQuery.Callbacks("once memory"),
                // Status-dependent callbacks
                statusCode = s.statusCode || {},
                // Headers (they are sent all at once)
                requestHeaders = {},
                requestHeadersNames = {},
                // The jqXHR state
                state = 0,
                // Default abort message
                strAbort = "canceled",
                // Fake xhr
                jqXHR = {
                    readyState: 0,

                    // Builds headers hashtable if needed
                    getResponseHeader: function(key) {
                        var match;
                        if (state === 2) {
                            if (!responseHeaders) {
                                responseHeaders = {};
                                while ((match = rheaders.exec(responseHeadersString))) {
                                    responseHeaders[match[1].toLowerCase()] = match[2];
                                }
                            }
                            match = responseHeaders[key.toLowerCase()];
                        }
                        return match == null ? null : match;
                    },

                    // Raw string
                    getAllResponseHeaders: function() {
                        return state === 2 ? responseHeadersString : null;
                    },

                    // Caches the header
                    setRequestHeader: function(name, value) {
                        var lname = name.toLowerCase();
                        if (!state) {
                            name = requestHeadersNames[lname] = requestHeadersNames[lname] || name;
                            requestHeaders[name] = value;
                        }
                        return this;
                    },

                    // Overrides response content-type header
                    overrideMimeType: function(type) {
                        if (!state) {
                            s.mimeType = type;
                        }
                        return this;
                    },

                    // Status-dependent callbacks
                    statusCode: function(map) {
                        var code;
                        if (map) {
                            if (state < 2) {
                                for (code in map) {
                                    // Lazy-add the new callback in a way that preserves old ones
                                    statusCode[code] = [statusCode[code], map[code]];
                                }
                            } else {
                                // Execute the appropriate callbacks
                                jqXHR.always(map[jqXHR.status]);
                            }
                        }
                        return this;
                    },

                    // Cancel the request
                    abort: function(statusText) {
                        var finalText = statusText || strAbort;
                        if (transport) {
                            transport.abort(finalText);
                        }
                        done(0, finalText);
                        return this;
                    }
                };

            // Attach deferreds
            deferred.promise(jqXHR).complete = completeDeferred.add;
            jqXHR.success = jqXHR.done;
            jqXHR.error = jqXHR.fail;

            // Remove hash character (#7531: and string promotion)
            // Add protocol if not provided (prefilters might expect it)
            // Handle falsy url in the settings object (#10093: consistency with old signature)
            // We also use the url parameter if available
            s.url = ((url || s.url || ajaxLocation) + "").replace(rhash, "")
                .replace(rprotocol, ajaxLocParts[1] + "//");

            // Alias method option to type as per ticket #12004
            s.type = options.method || options.type || s.method || s.type;

            // Extract dataTypes list
            s.dataTypes = jQuery.trim(s.dataType || "*").toLowerCase().match(core_rnotwhite) || [""];

            // A cross-domain request is in order when we have a protocol:host:port mismatch
            if (s.crossDomain == null) {
                parts = rurl.exec(s.url.toLowerCase());
                s.crossDomain = !! (parts && (parts[1] !== ajaxLocParts[1] || parts[2] !== ajaxLocParts[2] || (parts[3] || (parts[1] === "http:" ? "80" : "443")) !== (ajaxLocParts[3] || (ajaxLocParts[1] === "http:" ? "80" : "443"))));
            }

            // Convert data if not already a string
            if (s.data && s.processData && typeof s.data !== "string") {
                s.data = jQuery.param(s.data, s.traditional);
            }

            // Apply prefilters
            inspectPrefiltersOrTransports(prefilters, s, options, jqXHR);

            // If request was aborted inside a prefilter, stop there
            if (state === 2) {
                return jqXHR;
            }

            // We can fire global events as of now if asked to
            fireGlobals = s.global;

            // Watch for a new set of requests
            if (fireGlobals && jQuery.active++ === 0) {
                jQuery.event.trigger("ajaxStart");
            }

            // Uppercase the type
            s.type = s.type.toUpperCase();

            // Determine if request has content
            s.hasContent = !rnoContent.test(s.type);

            // Save the URL in case we're toying with the If-Modified-Since
            // and/or If-None-Match header later on
            cacheURL = s.url;

            // More options handling for requests with no content
            if (!s.hasContent) {

                // If data is available, append data to url
                if (s.data) {
                    cacheURL = (s.url += (ajax_rquery.test(cacheURL) ? "&" : "?") + s.data);
                    // #9682: remove data so that it's not used in an eventual retry
                    delete s.data;
                }

                // Add anti-cache in url if needed
                if (s.cache === false) {
                    s.url = rts.test(cacheURL) ?

                    // If there is already a '_' parameter, set its value
                    cacheURL.replace(rts, "$1_=" + ajax_nonce++) :

                    // Otherwise add one to the end
                    cacheURL + (ajax_rquery.test(cacheURL) ? "&" : "?") + "_=" + ajax_nonce++;
                }
            }

            // Set the If-Modified-Since and/or If-None-Match header, if in ifModified mode.
            if (s.ifModified) {
                if (jQuery.lastModified[cacheURL]) {
                    jqXHR.setRequestHeader("If-Modified-Since", jQuery.lastModified[cacheURL]);
                }
                if (jQuery.etag[cacheURL]) {
                    jqXHR.setRequestHeader("If-None-Match", jQuery.etag[cacheURL]);
                }
            }

            // Set the correct header, if data is being sent
            if (s.data && s.hasContent && s.contentType !== false || options.contentType) {
                jqXHR.setRequestHeader("Content-Type", s.contentType);
            }

            // Set the Accepts header for the server, depending on the dataType
            jqXHR.setRequestHeader("Accept",
            s.dataTypes[0] && s.accepts[s.dataTypes[0]] ? s.accepts[s.dataTypes[0]] + (s.dataTypes[0] !== "*" ? ", " + allTypes + "; q=0.01" : "") : s.accepts["*"]);

            // Check for headers option
            for (i in s.headers) {
                jqXHR.setRequestHeader(i, s.headers[i]);
            }

            // Allow custom headers/mimetypes and early abort
            if (s.beforeSend && (s.beforeSend.call(callbackContext, jqXHR, s) === false || state === 2)) {
                // Abort if not done already and return
                return jqXHR.abort();
            }

            // aborting is no longer a cancellation
            strAbort = "abort";

            // Install callbacks on deferreds
            for (i in {
                success: 1,
                error: 1,
                complete: 1
            }) {
                jqXHR[i](s[i]);
            }

            // Get transport
            transport = inspectPrefiltersOrTransports(transports, s, options, jqXHR);

            // If no transport, we auto-abort
            if (!transport) {
                done(-1, "No Transport");
            } else {
                jqXHR.readyState = 1;

                // Send global event
                if (fireGlobals) {
                    globalEventContext.trigger("ajaxSend", [jqXHR, s]);
                }
                // Timeout
                if (s.async && s.timeout > 0) {
                    timeoutTimer = setTimeout(function() {
                        jqXHR.abort("timeout");
                    }, s.timeout);
                }

                try {
                    state = 1;
                    transport.send(requestHeaders, done);
                } catch (e) {
                    // Propagate exception as error if not done
                    if (state < 2) {
                        done(-1, e);
                        // Simply rethrow otherwise
                    } else {
                        throw e;
                    }
                }
            }

            // Callback for when everything is done
            function done(status, nativeStatusText, responses, headers) {
                var isSuccess, success, error, response, modified,
                statusText = nativeStatusText;

                // Called once
                if (state === 2) {
                    return;
                }

                // State is "done" now
                state = 2;

                // Clear timeout if it exists
                if (timeoutTimer) {
                    clearTimeout(timeoutTimer);
                }

                // Dereference transport for early garbage collection
                // (no matter how long the jqXHR object will be used)
                transport = undefined;

                // Cache response headers
                responseHeadersString = headers || "";

                // Set readyState
                jqXHR.readyState = status > 0 ? 4 : 0;

                // Determine if successful
                isSuccess = status >= 200 && status < 300 || status === 304;

                // Get response data
                if (responses) {
                    response = ajaxHandleResponses(s, jqXHR, responses);
                }

                // Convert no matter what (that way responseXXX fields are always set)
                response = ajaxConvert(s, response, jqXHR, isSuccess);

                // If successful, handle type chaining
                if (isSuccess) {

                    // Set the If-Modified-Since and/or If-None-Match header, if in ifModified mode.
                    if (s.ifModified) {
                        modified = jqXHR.getResponseHeader("Last-Modified");
                        if (modified) {
                            jQuery.lastModified[cacheURL] = modified;
                        }
                        modified = jqXHR.getResponseHeader("etag");
                        if (modified) {
                            jQuery.etag[cacheURL] = modified;
                        }
                    }

                    // if no content
                    if (status === 204 || s.type === "HEAD") {
                        statusText = "nocontent";

                        // if not modified
                    } else if (status === 304) {
                        statusText = "notmodified";

                        // If we have data, let's convert it
                    } else {
                        statusText = response.state;
                        success = response.data;
                        error = response.error;
                        isSuccess = !error;
                    }
                } else {
                    // We extract error from statusText
                    // then normalize statusText and status for non-aborts
                    error = statusText;
                    if (status || !statusText) {
                        statusText = "error";
                        if (status < 0) {
                            status = 0;
                        }
                    }
                }

                // Set data for the fake xhr object
                jqXHR.status = status;
                jqXHR.statusText = (nativeStatusText || statusText) + "";

                // Success/Error
                if (isSuccess) {
                    deferred.resolveWith(callbackContext, [success, statusText, jqXHR]);
                } else {
                    deferred.rejectWith(callbackContext, [jqXHR, statusText, error]);
                }

                // Status-dependent callbacks
                jqXHR.statusCode(statusCode);
                statusCode = undefined;

                if (fireGlobals) {
                    globalEventContext.trigger(isSuccess ? "ajaxSuccess" : "ajaxError", [jqXHR, s, isSuccess ? success : error]);
                }

                // Complete
                completeDeferred.fireWith(callbackContext, [jqXHR, statusText]);

                if (fireGlobals) {
                    globalEventContext.trigger("ajaxComplete", [jqXHR, s]);
                    // Handle the global AJAX counter
                    if (!(--jQuery.active)) {
                        jQuery.event.trigger("ajaxStop");
                    }
                }
            }

            return jqXHR;
        },

        getJSON: function(url, data, callback) {
            return jQuery.get(url, data, callback, "json");
        },

        getScript: function(url, callback) {
            return jQuery.get(url, undefined, callback, "script");
        }
    });

    jQuery.each(["get", "post"], function(i, method) {
        jQuery[method] = function(url, data, callback, type) {
            // shift arguments if data argument was omitted
            if (jQuery.isFunction(data)) {
                type = type || callback;
                callback = data;
                data = undefined;
            }

            return jQuery.ajax({
                url: url,
                type: method,
                dataType: type,
                data: data,
                success: callback
            });
        };
    });

    /* Handles responses to an ajax request:
     * - finds the right dataType (mediates between content-type and expected dataType)
     * - returns the corresponding response
     */
    function ajaxHandleResponses(s, jqXHR, responses) {

        var ct, type, finalDataType, firstDataType,
        contents = s.contents,
            dataTypes = s.dataTypes;

        // Remove auto dataType and get content-type in the process
        while (dataTypes[0] === "*") {
            dataTypes.shift();
            if (ct === undefined) {
                ct = s.mimeType || jqXHR.getResponseHeader("Content-Type");
            }
        }

        // Check if we're dealing with a known content-type
        if (ct) {
            for (type in contents) {
                if (contents[type] && contents[type].test(ct)) {
                    dataTypes.unshift(type);
                    break;
                }
            }
        }

        // Check to see if we have a response for the expected dataType
        if (dataTypes[0] in responses) {
            finalDataType = dataTypes[0];
        } else {
            // Try convertible dataTypes
            for (type in responses) {
                if (!dataTypes[0] || s.converters[type + " " + dataTypes[0]]) {
                    finalDataType = type;
                    break;
                }
                if (!firstDataType) {
                    firstDataType = type;
                }
            }
            // Or just use first one
            finalDataType = finalDataType || firstDataType;
        }

        // If we found a dataType
        // We add the dataType to the list if needed
        // and return the corresponding response
        if (finalDataType) {
            if (finalDataType !== dataTypes[0]) {
                dataTypes.unshift(finalDataType);
            }
            return responses[finalDataType];
        }
    }

    /* Chain conversions given the request and the original response
     * Also sets the responseXXX fields on the jqXHR instance
     */
    function ajaxConvert(s, response, jqXHR, isSuccess) {
        var conv2, current, conv, tmp, prev,
        converters = {},
        // Work with a copy of dataTypes in case we need to modify it for conversion
        dataTypes = s.dataTypes.slice();

        // Create converters map with lowercased keys
        if (dataTypes[1]) {
            for (conv in s.converters) {
                converters[conv.toLowerCase()] = s.converters[conv];
            }
        }

        current = dataTypes.shift();

        // Convert to each sequential dataType
        while (current) {

            if (s.responseFields[current]) {
                jqXHR[s.responseFields[current]] = response;
            }

            // Apply the dataFilter if provided
            if (!prev && isSuccess && s.dataFilter) {
                response = s.dataFilter(response, s.dataType);
            }

            prev = current;
            current = dataTypes.shift();

            if (current) {

                // There's only work to do if current dataType is non-auto
                if (current === "*") {

                    current = prev;

                    // Convert response if prev dataType is non-auto and differs from current
                } else if (prev !== "*" && prev !== current) {

                    // Seek a direct converter
                    conv = converters[prev + " " + current] || converters["* " + current];

                    // If none found, seek a pair
                    if (!conv) {
                        for (conv2 in converters) {

                            // If conv2 outputs current
                            tmp = conv2.split(" ");
                            if (tmp[1] === current) {

                                // If prev can be converted to accepted input
                                conv = converters[prev + " " + tmp[0]] || converters["* " + tmp[0]];
                                if (conv) {
                                    // Condense equivalence converters
                                    if (conv === true) {
                                        conv = converters[conv2];

                                        // Otherwise, insert the intermediate dataType
                                    } else if (converters[conv2] !== true) {
                                        current = tmp[0];
                                        dataTypes.unshift(tmp[1]);
                                    }
                                    break;
                                }
                            }
                        }
                    }

                    // Apply converter (if not an equivalence)
                    if (conv !== true) {

                        // Unless errors are allowed to bubble, catch and return them
                        if (conv && s["throws"]) {
                            response = conv(response);
                        } else {
                            try {
                                response = conv(response);
                            } catch (e) {
                                return {
                                    state: "parsererror",
                                    error: conv ? e : "No conversion from " + prev + " to " + current
                                };
                            }
                        }
                    }
                }
            }
        }

        return {
            state: "success",
            data: response
        };
    }
    // Install script dataType
    jQuery.ajaxSetup({
        accepts: {
            script: "text/javascript, application/javascript, application/ecmascript, application/x-ecmascript"
        },
        contents: {
            script: /(?:java|ecma)script/
        },
        converters: {
            "text script": function(text) {
                jQuery.globalEval(text);
                return text;
            }
        }
    });

    // Handle cache's special case and crossDomain
    jQuery.ajaxPrefilter("script", function(s) {
        if (s.cache === undefined) {
            s.cache = false;
        }
        if (s.crossDomain) {
            s.type = "GET";
        }
    });

    // Bind script tag hack transport
    jQuery.ajaxTransport("script", function(s) {
        // This transport only deals with cross domain requests
        if (s.crossDomain) {
            var script, callback;
            return {
                send: function(_, complete) {
                    script = jQuery("<script>").prop({
                        async: true,
                        charset: s.scriptCharset,
                        src: s.url
                    }).on("load error",
                    callback = function(evt) {
                        script.remove();
                        callback = null;
                        if (evt) {
                            complete(evt.type === "error" ? 404 : 200, evt.type);
                        }
                    });
                    document.head.appendChild(script[0]);
                },
                abort: function() {
                    if (callback) {
                        callback();
                    }
                }
            };
        }
    });
    var oldCallbacks = [],
        rjsonp = /(=)\?(?=&|$)|\?\?/;

    // Default jsonp settings
    jQuery.ajaxSetup({
        jsonp: "callback",
        jsonpCallback: function() {
            var callback = oldCallbacks.pop() || (jQuery.expando + "_" + (ajax_nonce++));
            this[callback] = true;
            return callback;
        }
    });

    // Detect, normalize options and install callbacks for jsonp requests
    jQuery.ajaxPrefilter("json jsonp", function(s, originalSettings, jqXHR) {

        var callbackName, overwritten, responseContainer,
        jsonProp = s.jsonp !== false && (rjsonp.test(s.url) ? "url" : typeof s.data === "string" && !(s.contentType || "").indexOf("application/x-www-form-urlencoded") && rjsonp.test(s.data) && "data");

        // Handle iff the expected data type is "jsonp" or we have a parameter to set
        if (jsonProp || s.dataTypes[0] === "jsonp") {

            // Get callback name, remembering preexisting value associated with it
            callbackName = s.jsonpCallback = jQuery.isFunction(s.jsonpCallback) ? s.jsonpCallback() : s.jsonpCallback;

            // Insert callback into url or form data
            if (jsonProp) {
                s[jsonProp] = s[jsonProp].replace(rjsonp, "$1" + callbackName);
            } else if (s.jsonp !== false) {
                s.url += (ajax_rquery.test(s.url) ? "&" : "?") + s.jsonp + "=" + callbackName;
            }

            // Use data converter to retrieve json after script execution
            s.converters["script json"] = function() {
                if (!responseContainer) {
                    jQuery.error(callbackName + " was not called");
                }
                return responseContainer[0];
            };

            // force json dataType
            s.dataTypes[0] = "json";

            // Install callback
            overwritten = window[callbackName];
            window[callbackName] = function() {
                responseContainer = arguments;
            };

            // Clean-up function (fires after converters)
            jqXHR.always(function() {
                // Restore preexisting value
                window[callbackName] = overwritten;

                // Save back as free
                if (s[callbackName]) {
                    // make sure that re-using the options doesn't screw things around
                    s.jsonpCallback = originalSettings.jsonpCallback;

                    // save the callback name for future use
                    oldCallbacks.push(callbackName);
                }

                // Call if it was a function and we have a response
                if (responseContainer && jQuery.isFunction(overwritten)) {
                    overwritten(responseContainer[0]);
                }

                responseContainer = overwritten = undefined;
            });

            // Delegate to script
            return "script";
        }
    });
    jQuery.ajaxSettings.xhr = function() {
        try {
            return new XMLHttpRequest();
        } catch (e) {}
    };

    var xhrSupported = jQuery.ajaxSettings.xhr(),
        xhrSuccessStatus = {
            // file protocol always yields status code 0, assume 200
            0: 200,
            // Support: IE9
            // #1450: sometimes IE returns 1223 when it should be 204
            1223: 204
        },
        // Support: IE9
        // We need to keep track of outbound xhr and abort them manually
        // because IE is not smart enough to do it all by itself
        xhrId = 0,
        xhrCallbacks = {};

    if (window.ActiveXObject) {
        jQuery(window).on("unload", function() {
            for (var key in xhrCallbacks) {
                xhrCallbacks[key]();
            }
            xhrCallbacks = undefined;
        });
    }

    jQuery.support.cors = !! xhrSupported && ("withCredentials" in xhrSupported);
    jQuery.support.ajax = xhrSupported = !! xhrSupported;

    jQuery.ajaxTransport(function(options) {
        var callback;
        // Cross domain only allowed if supported through XMLHttpRequest
        if (jQuery.support.cors || xhrSupported && !options.crossDomain) {
            return {
                send: function(headers, complete) {
                    var i, id,
                    xhr = options.xhr();
                    xhr.open(options.type, options.url, options.async, options.username, options.password);
                    // Apply custom fields if provided
                    if (options.xhrFields) {
                        for (i in options.xhrFields) {
                            xhr[i] = options.xhrFields[i];
                        }
                    }
                    // Override mime type if needed
                    if (options.mimeType && xhr.overrideMimeType) {
                        xhr.overrideMimeType(options.mimeType);
                    }
                    // X-Requested-With header
                    // For cross-domain requests, seeing as conditions for a preflight are
                    // akin to a jigsaw puzzle, we simply never set it to be sure.
                    // (it can always be set on a per-request basis or even using ajaxSetup)
                    // For same-domain requests, won't change header if already provided.
                    if (!options.crossDomain && !headers["X-Requested-With"]) {
                        headers["X-Requested-With"] = "XMLHttpRequest";
                    }
                    // Set headers
                    for (i in headers) {
                        xhr.setRequestHeader(i, headers[i]);
                    }
                    // Callback
                    callback = function(type) {
                        return function() {
                            if (callback) {
                                delete xhrCallbacks[id];
                                callback = xhr.onload = xhr.onerror = null;
                                if (type === "abort") {
                                    xhr.abort();
                                } else if (type === "error") {
                                    complete(
                                    // file protocol always yields status 0, assume 404
                                    xhr.status || 404,
                                    xhr.statusText);
                                } else {
                                    complete(
                                    xhrSuccessStatus[xhr.status] || xhr.status,
                                    xhr.statusText,
                                    // Support: IE9
                                    // #11426: When requesting binary data, IE9 will throw an exception
                                    // on any attempt to access responseText
                                    typeof xhr.responseText === "string" ? {
                                        text: xhr.responseText
                                    } : undefined,
                                    xhr.getAllResponseHeaders());
                                }
                            }
                        };
                    };
                    // Listen to events
                    xhr.onload = callback();
                    xhr.onerror = callback("error");
                    // Create the abort callback
                    callback = xhrCallbacks[(id = xhrId++)] = callback("abort");
                    // Do send the request
                    // This may raise an exception which is actually
                    // handled in jQuery.ajax (so no try/catch here)
                    xhr.send(options.hasContent && options.data || null);
                },
                abort: function() {
                    if (callback) {
                        callback();
                    }
                }
            };
        }
    });
    var fxNow, timerId,
    rfxtypes = /^(?:toggle|show|hide)$/,
        rfxnum = new RegExp("^(?:([+-])=|)(" + core_pnum + ")([a-z%]*)$", "i"),
        rrun = /queueHooks$/,
        animationPrefilters = [defaultPrefilter],
        tweeners = {
            "*": [function(prop, value) {
                var tween = this.createTween(prop, value),
                    target = tween.cur(),
                    parts = rfxnum.exec(value),
                    unit = parts && parts[3] || (jQuery.cssNumber[prop] ? "" : "px"),

                    // Starting value computation is required for potential unit mismatches
                    start = (jQuery.cssNumber[prop] || unit !== "px" && +target) && rfxnum.exec(jQuery.css(tween.elem, prop)),
                    scale = 1,
                    maxIterations = 20;

                if (start && start[3] !== unit) {
                    // Trust units reported by jQuery.css
                    unit = unit || start[3];

                    // Make sure we update the tween properties later on
                    parts = parts || [];

                    // Iteratively approximate from a nonzero starting point
                    start = +target || 1;

                    do {
                        // If previous iteration zeroed out, double until we get *something*
                        // Use a string for doubling factor so we don't accidentally see scale as unchanged below
                        scale = scale || ".5";

                        // Adjust and apply
                        start = start / scale;
                        jQuery.style(tween.elem, prop, start + unit);

                        // Update scale, tolerating zero or NaN from tween.cur()
                        // And breaking the loop if scale is unchanged or perfect, or if we've just had enough
                    } while (scale !== (scale = tween.cur() / target) && scale !== 1 && --maxIterations);
                }

                // Update tween properties
                if (parts) {
                    start = tween.start = +start || +target || 0;
                    tween.unit = unit;
                    // If a +=/-= token was provided, we're doing a relative animation
                    tween.end = parts[1] ? start + (parts[1] + 1) * parts[2] : +parts[2];
                }

                return tween;
            }]
        };

    // Animations created synchronously will run synchronously
    function createFxNow() {
        setTimeout(function() {
            fxNow = undefined;
        });
        return (fxNow = jQuery.now());
    }

    function createTween(value, prop, animation) {
        var tween,
        collection = (tweeners[prop] || []).concat(tweeners["*"]),
            index = 0,
            length = collection.length;
        for (; index < length; index++) {
            if ((tween = collection[index].call(animation, prop, value))) {

                // we're done with this property
                return tween;
            }
        }
    }

    function Animation(elem, properties, options) {
        var result,
        stopped,
        index = 0,
            length = animationPrefilters.length,
            deferred = jQuery.Deferred().always(function() {
                // don't match elem in the :animated selector
                delete tick.elem;
            }),
            tick = function() {
                if (stopped) {
                    return false;
                }
                var currentTime = fxNow || createFxNow(),
                    remaining = Math.max(0, animation.startTime + animation.duration - currentTime),
                    // archaic crash bug won't allow us to use 1 - ( 0.5 || 0 ) (#12497)
                    temp = remaining / animation.duration || 0,
                    percent = 1 - temp,
                    index = 0,
                    length = animation.tweens.length;

                for (; index < length; index++) {
                    animation.tweens[index].run(percent);
                }

                deferred.notifyWith(elem, [animation, percent, remaining]);

                if (percent < 1 && length) {
                    return remaining;
                } else {
                    deferred.resolveWith(elem, [animation]);
                    return false;
                }
            },
            animation = deferred.promise({
                elem: elem,
                props: jQuery.extend({}, properties),
                opts: jQuery.extend(true, {
                    specialEasing: {}
                }, options),
                originalProperties: properties,
                originalOptions: options,
                startTime: fxNow || createFxNow(),
                duration: options.duration,
                tweens: [],
                createTween: function(prop, end) {
                    var tween = jQuery.Tween(elem, animation.opts, prop, end,
                    animation.opts.specialEasing[prop] || animation.opts.easing);
                    animation.tweens.push(tween);
                    return tween;
                },
                stop: function(gotoEnd) {
                    var index = 0,
                        // if we are going to the end, we want to run all the tweens
                        // otherwise we skip this part
                        length = gotoEnd ? animation.tweens.length : 0;
                    if (stopped) {
                        return this;
                    }
                    stopped = true;
                    for (; index < length; index++) {
                        animation.tweens[index].run(1);
                    }

                    // resolve when we played the last frame
                    // otherwise, reject
                    if (gotoEnd) {
                        deferred.resolveWith(elem, [animation, gotoEnd]);
                    } else {
                        deferred.rejectWith(elem, [animation, gotoEnd]);
                    }
                    return this;
                }
            }),
            props = animation.props;

        propFilter(props, animation.opts.specialEasing);

        for (; index < length; index++) {
            result = animationPrefilters[index].call(animation, elem, props, animation.opts);
            if (result) {
                return result;
            }
        }

        jQuery.map(props, createTween, animation);

        if (jQuery.isFunction(animation.opts.start)) {
            animation.opts.start.call(elem, animation);
        }

        jQuery.fx.timer(
        jQuery.extend(tick, {
            elem: elem,
            anim: animation,
            queue: animation.opts.queue
        }));

        // attach callbacks from options
        return animation.progress(animation.opts.progress)
            .done(animation.opts.done, animation.opts.complete)
            .fail(animation.opts.fail)
            .always(animation.opts.always);
    }

    function propFilter(props, specialEasing) {
        var index, name, easing, value, hooks;

        // camelCase, specialEasing and expand cssHook pass
        for (index in props) {
            name = jQuery.camelCase(index);
            easing = specialEasing[name];
            value = props[index];
            if (jQuery.isArray(value)) {
                easing = value[1];
                value = props[index] = value[0];
            }

            if (index !== name) {
                props[name] = value;
                delete props[index];
            }

            hooks = jQuery.cssHooks[name];
            if (hooks && "expand" in hooks) {
                value = hooks.expand(value);
                delete props[name];

                // not quite $.extend, this wont overwrite keys already present.
                // also - reusing 'index' from above because we have the correct "name"
                for (index in value) {
                    if (!(index in props)) {
                        props[index] = value[index];
                        specialEasing[index] = easing;
                    }
                }
            } else {
                specialEasing[name] = easing;
            }
        }
    }

    jQuery.Animation = jQuery.extend(Animation, {

        tweener: function(props, callback) {
            if (jQuery.isFunction(props)) {
                callback = props;
                props = ["*"];
            } else {
                props = props.split(" ");
            }

            var prop,
            index = 0,
                length = props.length;

            for (; index < length; index++) {
                prop = props[index];
                tweeners[prop] = tweeners[prop] || [];
                tweeners[prop].unshift(callback);
            }
        },

        prefilter: function(callback, prepend) {
            if (prepend) {
                animationPrefilters.unshift(callback);
            } else {
                animationPrefilters.push(callback);
            }
        }
    });

    function defaultPrefilter(elem, props, opts) {
        /* jshint validthis: true */
        var prop, value, toggle, tween, hooks, oldfire,
        anim = this,
            orig = {},
            style = elem.style,
            hidden = elem.nodeType && isHidden(elem),
            dataShow = data_priv.get(elem, "fxshow");

        // handle queue: false promises
        if (!opts.queue) {
            hooks = jQuery._queueHooks(elem, "fx");
            if (hooks.unqueued == null) {
                hooks.unqueued = 0;
                oldfire = hooks.empty.fire;
                hooks.empty.fire = function() {
                    if (!hooks.unqueued) {
                        oldfire();
                    }
                };
            }
            hooks.unqueued++;

            anim.always(function() {
                // doing this makes sure that the complete handler will be called
                // before this completes
                anim.always(function() {
                    hooks.unqueued--;
                    if (!jQuery.queue(elem, "fx").length) {
                        hooks.empty.fire();
                    }
                });
            });
        }

        // height/width overflow pass
        if (elem.nodeType === 1 && ("height" in props || "width" in props)) {
            // Make sure that nothing sneaks out
            // Record all 3 overflow attributes because IE9-10 do not
            // change the overflow attribute when overflowX and
            // overflowY are set to the same value
            opts.overflow = [style.overflow, style.overflowX, style.overflowY];

            // Set display property to inline-block for height/width
            // animations on inline elements that are having width/height animated
            if (jQuery.css(elem, "display") === "inline" && jQuery.css(elem, "float") === "none") {

                style.display = "inline-block";
            }
        }

        if (opts.overflow) {
            style.overflow = "hidden";
            anim.always(function() {
                style.overflow = opts.overflow[0];
                style.overflowX = opts.overflow[1];
                style.overflowY = opts.overflow[2];
            });
        }


        // show/hide pass
        for (prop in props) {
            value = props[prop];
            if (rfxtypes.exec(value)) {
                delete props[prop];
                toggle = toggle || value === "toggle";
                if (value === (hidden ? "hide" : "show")) {

                    // If there is dataShow left over from a stopped hide or show and we are going to proceed with show, we should pretend to be hidden
                    if (value === "show" && dataShow && dataShow[prop] !== undefined) {
                        hidden = true;
                    } else {
                        continue;
                    }
                }
                orig[prop] = dataShow && dataShow[prop] || jQuery.style(elem, prop);
            }
        }

        if (!jQuery.isEmptyObject(orig)) {
            if (dataShow) {
                if ("hidden" in dataShow) {
                    hidden = dataShow.hidden;
                }
            } else {
                dataShow = data_priv.access(elem, "fxshow", {});
            }

            // store state if its toggle - enables .stop().toggle() to "reverse"
            if (toggle) {
                dataShow.hidden = !hidden;
            }
            if (hidden) {
                jQuery(elem).show();
            } else {
                anim.done(function() {
                    jQuery(elem).hide();
                });
            }
            anim.done(function() {
                var prop;

                data_priv.remove(elem, "fxshow");
                for (prop in orig) {
                    jQuery.style(elem, prop, orig[prop]);
                }
            });
            for (prop in orig) {
                tween = createTween(hidden ? dataShow[prop] : 0, prop, anim);

                if (!(prop in dataShow)) {
                    dataShow[prop] = tween.start;
                    if (hidden) {
                        tween.end = tween.start;
                        tween.start = prop === "width" || prop === "height" ? 1 : 0;
                    }
                }
            }
        }
    }

    function Tween(elem, options, prop, end, easing) {
        return new Tween.prototype.init(elem, options, prop, end, easing);
    }
    jQuery.Tween = Tween;

    Tween.prototype = {
        constructor: Tween,
        init: function(elem, options, prop, end, easing, unit) {
            this.elem = elem;
            this.prop = prop;
            this.easing = easing || "swing";
            this.options = options;
            this.start = this.now = this.cur();
            this.end = end;
            this.unit = unit || (jQuery.cssNumber[prop] ? "" : "px");
        },
        cur: function() {
            var hooks = Tween.propHooks[this.prop];

            return hooks && hooks.get ? hooks.get(this) : Tween.propHooks._default.get(this);
        },
        run: function(percent) {
            var eased,
            hooks = Tween.propHooks[this.prop];

            if (this.options.duration) {
                this.pos = eased = jQuery.easing[this.easing](
                percent, this.options.duration * percent, 0, 1, this.options.duration);
            } else {
                this.pos = eased = percent;
            }
            this.now = (this.end - this.start) * eased + this.start;

            if (this.options.step) {
                this.options.step.call(this.elem, this.now, this);
            }

            if (hooks && hooks.set) {
                hooks.set(this);
            } else {
                Tween.propHooks._default.set(this);
            }
            return this;
        }
    };

    Tween.prototype.init.prototype = Tween.prototype;

    Tween.propHooks = {
        _default: {
            get: function(tween) {
                var result;

                if (tween.elem[tween.prop] != null && (!tween.elem.style || tween.elem.style[tween.prop] == null)) {
                    return tween.elem[tween.prop];
                }

                // passing an empty string as a 3rd parameter to .css will automatically
                // attempt a parseFloat and fallback to a string if the parse fails
                // so, simple values such as "10px" are parsed to Float.
                // complex values such as "rotate(1rad)" are returned as is.
                result = jQuery.css(tween.elem, tween.prop, "");
                // Empty strings, null, undefined and "auto" are converted to 0.
                return !result || result === "auto" ? 0 : result;
            },
            set: function(tween) {
                // use step hook for back compat - use cssHook if its there - use .style if its
                // available and use plain properties where available
                if (jQuery.fx.step[tween.prop]) {
                    jQuery.fx.step[tween.prop](tween);
                } else if (tween.elem.style && (tween.elem.style[jQuery.cssProps[tween.prop]] != null || jQuery.cssHooks[tween.prop])) {
                    jQuery.style(tween.elem, tween.prop, tween.now + tween.unit);
                } else {
                    tween.elem[tween.prop] = tween.now;
                }
            }
        }
    };

    // Support: IE9
    // Panic based approach to setting things on disconnected nodes

    Tween.propHooks.scrollTop = Tween.propHooks.scrollLeft = {
        set: function(tween) {
            if (tween.elem.nodeType && tween.elem.parentNode) {
                tween.elem[tween.prop] = tween.now;
            }
        }
    };

    jQuery.each(["toggle", "show", "hide"], function(i, name) {
        var cssFn = jQuery.fn[name];
        jQuery.fn[name] = function(speed, easing, callback) {
            return speed == null || typeof speed === "boolean" ? cssFn.apply(this, arguments) : this.animate(genFx(name, true), speed, easing, callback);
        };
    });

    jQuery.fn.extend({
        fadeTo: function(speed, to, easing, callback) {

            // show any hidden elements after setting opacity to 0
            return this.filter(isHidden).css("opacity", 0).show()

            // animate to the value specified
            .end().animate({
                opacity: to
            }, speed, easing, callback);
        },
        animate: function(prop, speed, easing, callback) {
            var empty = jQuery.isEmptyObject(prop),
                optall = jQuery.speed(speed, easing, callback),
                doAnimation = function() {
                    // Operate on a copy of prop so per-property easing won't be lost
                    var anim = Animation(this, jQuery.extend({}, prop), optall);

                    // Empty animations, or finishing resolves immediately
                    if (empty || data_priv.get(this, "finish")) {
                        anim.stop(true);
                    }
                };
            doAnimation.finish = doAnimation;

            return empty || optall.queue === false ? this.each(doAnimation) : this.queue(optall.queue, doAnimation);
        },
        stop: function(type, clearQueue, gotoEnd) {
            var stopQueue = function(hooks) {
                var stop = hooks.stop;
                delete hooks.stop;
                stop(gotoEnd);
            };

            if (typeof type !== "string") {
                gotoEnd = clearQueue;
                clearQueue = type;
                type = undefined;
            }
            if (clearQueue && type !== false) {
                this.queue(type || "fx", []);
            }

            return this.each(function() {
                var dequeue = true,
                    index = type != null && type + "queueHooks",
                    timers = jQuery.timers,
                    data = data_priv.get(this);

                if (index) {
                    if (data[index] && data[index].stop) {
                        stopQueue(data[index]);
                    }
                } else {
                    for (index in data) {
                        if (data[index] && data[index].stop && rrun.test(index)) {
                            stopQueue(data[index]);
                        }
                    }
                }

                for (index = timers.length; index--;) {
                    if (timers[index].elem === this && (type == null || timers[index].queue === type)) {
                        timers[index].anim.stop(gotoEnd);
                        dequeue = false;
                        timers.splice(index, 1);
                    }
                }

                // start the next in the queue if the last step wasn't forced
                // timers currently will call their complete callbacks, which will dequeue
                // but only if they were gotoEnd
                if (dequeue || !gotoEnd) {
                    jQuery.dequeue(this, type);
                }
            });
        },
        finish: function(type) {
            if (type !== false) {
                type = type || "fx";
            }
            return this.each(function() {
                var index,
                data = data_priv.get(this),
                    queue = data[type + "queue"],
                    hooks = data[type + "queueHooks"],
                    timers = jQuery.timers,
                    length = queue ? queue.length : 0;

                // enable finishing flag on private data
                data.finish = true;

                // empty the queue first
                jQuery.queue(this, type, []);

                if (hooks && hooks.stop) {
                    hooks.stop.call(this, true);
                }

                // look for any active animations, and finish them
                for (index = timers.length; index--;) {
                    if (timers[index].elem === this && timers[index].queue === type) {
                        timers[index].anim.stop(true);
                        timers.splice(index, 1);
                    }
                }

                // look for any animations in the old queue and finish them
                for (index = 0; index < length; index++) {
                    if (queue[index] && queue[index].finish) {
                        queue[index].finish.call(this);
                    }
                }

                // turn off finishing flag
                delete data.finish;
            });
        }
    });

    // Generate parameters to create a standard animation
    function genFx(type, includeWidth) {
        var which,
        attrs = {
            height: type
        },
        i = 0;

        // if we include width, step value is 1 to do all cssExpand values,
        // if we don't include width, step value is 2 to skip over Left and Right
        includeWidth = includeWidth ? 1 : 0;
        for (; i < 4; i += 2 - includeWidth) {
            which = cssExpand[i];
            attrs["margin" + which] = attrs["padding" + which] = type;
        }

        if (includeWidth) {
            attrs.opacity = attrs.width = type;
        }

        return attrs;
    }

    // Generate shortcuts for custom animations
    jQuery.each({
        slideDown: genFx("show"),
        slideUp: genFx("hide"),
        slideToggle: genFx("toggle"),
        fadeIn: {
            opacity: "show"
        },
        fadeOut: {
            opacity: "hide"
        },
        fadeToggle: {
            opacity: "toggle"
        }
    }, function(name, props) {
        jQuery.fn[name] = function(speed, easing, callback) {
            return this.animate(props, speed, easing, callback);
        };
    });

    jQuery.speed = function(speed, easing, fn) {
        var opt = speed && typeof speed === "object" ? jQuery.extend({}, speed) : {
            complete: fn || !fn && easing || jQuery.isFunction(speed) && speed,
            duration: speed,
            easing: fn && easing || easing && !jQuery.isFunction(easing) && easing
        };

        opt.duration = jQuery.fx.off ? 0 : typeof opt.duration === "number" ? opt.duration : opt.duration in jQuery.fx.speeds ? jQuery.fx.speeds[opt.duration] : jQuery.fx.speeds._default;

        // normalize opt.queue - true/undefined/null -> "fx"
        if (opt.queue == null || opt.queue === true) {
            opt.queue = "fx";
        }

        // Queueing
        opt.old = opt.complete;

        opt.complete = function() {
            if (jQuery.isFunction(opt.old)) {
                opt.old.call(this);
            }

            if (opt.queue) {
                jQuery.dequeue(this, opt.queue);
            }
        };

        return opt;
    };

    jQuery.easing = {
        linear: function(p) {
            return p;
        },
        swing: function(p) {
            return 0.5 - Math.cos(p * Math.PI) / 2;
        }
    };

    jQuery.timers = [];
    jQuery.fx = Tween.prototype.init;
    jQuery.fx.tick = function() {
        var timer,
        timers = jQuery.timers,
            i = 0;

        fxNow = jQuery.now();

        for (; i < timers.length; i++) {
            timer = timers[i];
            // Checks the timer has not already been removed
            if (!timer() && timers[i] === timer) {
                timers.splice(i--, 1);
            }
        }

        if (!timers.length) {
            jQuery.fx.stop();
        }
        fxNow = undefined;
    };

    jQuery.fx.timer = function(timer) {
        if (timer() && jQuery.timers.push(timer)) {
            jQuery.fx.start();
        }
    };

    jQuery.fx.interval = 13;

    jQuery.fx.start = function() {
        if (!timerId) {
            timerId = setInterval(jQuery.fx.tick, jQuery.fx.interval);
        }
    };

    jQuery.fx.stop = function() {
        clearInterval(timerId);
        timerId = null;
    };

    jQuery.fx.speeds = {
        slow: 600,
        fast: 200,
        // Default speed
        _default: 400
    };

    // Back Compat <1.8 extension point
    jQuery.fx.step = {};

    if (jQuery.expr && jQuery.expr.filters) {
        jQuery.expr.filters.animated = function(elem) {
            return jQuery.grep(jQuery.timers, function(fn) {
                return elem === fn.elem;
            }).length;
        };
    }
    jQuery.fn.offset = function(options) {
        if (arguments.length) {
            return options === undefined ? this : this.each(function(i) {
                jQuery.offset.setOffset(this, options, i);
            });
        }

        var docElem, win,
        elem = this[0],
            box = {
                top: 0,
                left: 0
            },
            doc = elem && elem.ownerDocument;

        if (!doc) {
            return;
        }

        docElem = doc.documentElement;

        // Make sure it's not a disconnected DOM node
        if (!jQuery.contains(docElem, elem)) {
            return box;
        }

        // If we don't have gBCR, just use 0,0 rather than error
        // BlackBerry 5, iOS 3 (original iPhone)
        if (typeof elem.getBoundingClientRect !== core_strundefined) {
            box = elem.getBoundingClientRect();
        }
        win = getWindow(doc);
        return {
            top: box.top + win.pageYOffset - docElem.clientTop,
            left: box.left + win.pageXOffset - docElem.clientLeft
        };
    };

    jQuery.offset = {

        setOffset: function(elem, options, i) {
            var curPosition, curLeft, curCSSTop, curTop, curOffset, curCSSLeft, calculatePosition,
            position = jQuery.css(elem, "position"),
                curElem = jQuery(elem),
                props = {};

            // Set position first, in-case top/left are set even on static elem
            if (position === "static") {
                elem.style.position = "relative";
            }

            curOffset = curElem.offset();
            curCSSTop = jQuery.css(elem, "top");
            curCSSLeft = jQuery.css(elem, "left");
            calculatePosition = (position === "absolute" || position === "fixed") && (curCSSTop + curCSSLeft).indexOf("auto") > -1;

            // Need to be able to calculate position if either top or left is auto and position is either absolute or fixed
            if (calculatePosition) {
                curPosition = curElem.position();
                curTop = curPosition.top;
                curLeft = curPosition.left;

            } else {
                curTop = parseFloat(curCSSTop) || 0;
                curLeft = parseFloat(curCSSLeft) || 0;
            }

            if (jQuery.isFunction(options)) {
                options = options.call(elem, i, curOffset);
            }

            if (options.top != null) {
                props.top = (options.top - curOffset.top) + curTop;
            }
            if (options.left != null) {
                props.left = (options.left - curOffset.left) + curLeft;
            }

            if ("using" in options) {
                options.using.call(elem, props);

            } else {
                curElem.css(props);
            }
        }
    };


    jQuery.fn.extend({

        position: function() {
            if (!this[0]) {
                return;
            }

            var offsetParent, offset,
            elem = this[0],
                parentOffset = {
                    top: 0,
                    left: 0
                };

            // Fixed elements are offset from window (parentOffset = {top:0, left: 0}, because it is it's only offset parent
            if (jQuery.css(elem, "position") === "fixed") {
                // We assume that getBoundingClientRect is available when computed position is fixed
                offset = elem.getBoundingClientRect();

            } else {
                // Get *real* offsetParent
                offsetParent = this.offsetParent();

                // Get correct offsets
                offset = this.offset();
                if (!jQuery.nodeName(offsetParent[0], "html")) {
                    parentOffset = offsetParent.offset();
                }

                // Add offsetParent borders
                parentOffset.top += jQuery.css(offsetParent[0], "borderTopWidth", true);
                parentOffset.left += jQuery.css(offsetParent[0], "borderLeftWidth", true);
            }

            // Subtract parent offsets and element margins
            return {
                top: offset.top - parentOffset.top - jQuery.css(elem, "marginTop", true),
                left: offset.left - parentOffset.left - jQuery.css(elem, "marginLeft", true)
            };
        },

        offsetParent: function() {
            return this.map(function() {
                var offsetParent = this.offsetParent || docElem;

                while (offsetParent && (!jQuery.nodeName(offsetParent, "html") && jQuery.css(offsetParent, "position") === "static")) {
                    offsetParent = offsetParent.offsetParent;
                }

                return offsetParent || docElem;
            });
        }
    });


    // Create scrollLeft and scrollTop methods
    jQuery.each({
        scrollLeft: "pageXOffset",
        scrollTop: "pageYOffset"
    }, function(method, prop) {
        var top = "pageYOffset" === prop;

        jQuery.fn[method] = function(val) {
            return jQuery.access(this, function(elem, method, val) {
                var win = getWindow(elem);

                if (val === undefined) {
                    return win ? win[prop] : elem[method];
                }

                if (win) {
                    win.scrollTo(!top ? val : window.pageXOffset,
                    top ? val : window.pageYOffset);

                } else {
                    elem[method] = val;
                }
            }, method, val, arguments.length, null);
        };
    });

    function getWindow(elem) {
        return jQuery.isWindow(elem) ? elem : elem.nodeType === 9 && elem.defaultView;
    }
    // Create innerHeight, innerWidth, height, width, outerHeight and outerWidth methods
    jQuery.each({
        Height: "height",
        Width: "width"
    }, function(name, type) {
        jQuery.each({
            padding: "inner" + name,
            content: type,
            "": "outer" + name
        }, function(defaultExtra, funcName) {
            // margin is only for outerHeight, outerWidth
            jQuery.fn[funcName] = function(margin, value) {
                var chainable = arguments.length && (defaultExtra || typeof margin !== "boolean"),
                    extra = defaultExtra || (margin === true || value === true ? "margin" : "border");

                return jQuery.access(this, function(elem, type, value) {
                    var doc;

                    if (jQuery.isWindow(elem)) {
                        // As of 5/8/2012 this will yield incorrect results for Mobile Safari, but there
                        // isn't a whole lot we can do. See pull request at this URL for discussion:
                        // https://github.com/jquery/jquery/pull/764
                        return elem.document.documentElement["client" + name];
                    }

                    // Get document width or height
                    if (elem.nodeType === 9) {
                        doc = elem.documentElement;

                        // Either scroll[Width/Height] or offset[Width/Height] or client[Width/Height],
                        // whichever is greatest
                        return Math.max(
                        elem.body["scroll" + name], doc["scroll" + name],
                        elem.body["offset" + name], doc["offset" + name],
                        doc["client" + name]);
                    }

                    return value === undefined ?
                    // Get width or height on the element, requesting but not forcing parseFloat
                    jQuery.css(elem, type, extra) :

                    // Set width or height on the element
                    jQuery.style(elem, type, value, extra);
                }, type, chainable ? margin : undefined, chainable, null);
            };
        });
    });
    // Limit scope pollution from any deprecated API
    // (function() {

    // The number of elements contained in the matched element set
    jQuery.fn.size = function() {
        return this.length;
    };

    jQuery.fn.andSelf = jQuery.fn.addBack;

    // })();
    if (typeof module === "object" && module && typeof module.exports === "object") {
        // Expose jQuery as module.exports in loaders that implement the Node
        // module pattern (including browserify). Do not create the global, since
        // the user will be storing it themselves locally, and globals are frowned
        // upon in the Node module world.
        module.exports = jQuery;
    } else {
        // Register as a named AMD module, since jQuery can be concatenated with other
        // files that may use define, but not via a proper concatenation script that
        // understands anonymous AMD modules. A named AMD is safest and most robust
        // way to register. Lowercase jquery is used because AMD module names are
        // derived from file names, and jQuery is normally delivered in a lowercase
        // file name. Do this after creating the global so that if an AMD module wants
        // to call noConflict to hide this version of jQuery, it will work.
        if (typeof define === "function" && define.amd) {
            define("jquery", [], function() {
                return jQuery;
            });
        }
    }

    // If there is a window object, that at least has a document property,
    // define jQuery and $ identifiers
    if (typeof window === "object" && typeof window.document === "object") {
        window.jQuery = window.$ = jQuery;
    }

})(window); 


/*!
 * jQuery Migrate - v1.2.1 - 2013-05-08
 * https://github.com/jquery/jquery-migrate
 * Copyright 2005, 2013 jQuery Foundation, Inc. and other contributors; Licensed MIT
 */ (function(jQuery, window, undefined) {
    // See http://bugs.jquery.com/ticket/13335
    // 


    var warnedAbout = {};

    // List of warnings already given; public read only
    jQuery.migrateWarnings = [];

    // Set to true to prevent console output; migrateWarnings still maintained
    // jQuery.migrateMute = false;

    // Show a message on the console so devs know we're active
    if (!jQuery.migrateMute && window.console && window.console.log) {
        window.console.log("JQMIGRATE: Logging is active");
    }

    // Set to false to disable traces that appear with warnings
    if (jQuery.migrateTrace === undefined) {
        jQuery.migrateTrace = true;
    }

    // Forget any warnings we've already given; public
    jQuery.migrateReset = function() {
        warnedAbout = {};
        jQuery.migrateWarnings.length = 0;
    };

    function migrateWarn(msg) {
        var console = window.console;
        if (!warnedAbout[msg]) {
            warnedAbout[msg] = true;
            jQuery.migrateWarnings.push(msg);
            if (console && console.warn && !jQuery.migrateMute) {
                console.warn("JQMIGRATE: " + msg);
                if (jQuery.migrateTrace && console.trace) {
                    console.trace();
                }
            }
        }
    }

    function migrateWarnProp(obj, prop, value, msg) {
        if (Object.defineProperty) {
            // On ES5 browsers (non-oldIE), warn if the code tries to get prop;
            // allow property to be overwritten in case some other plugin wants it
            try {
                Object.defineProperty(obj, prop, {
                    configurable: true,
                    enumerable: true,
                    get: function() {
                        migrateWarn(msg);
                        return value;
                    },
                    set: function(newValue) {
                        migrateWarn(msg);
                        value = newValue;
                    }
                });
                return;
            } catch (err) {
                // IE8 is a dope about Object.defineProperty, can't warn there
            }
        }

        // Non-ES5 (or broken) browser; just set the property
        jQuery._definePropertyBroken = true;
        obj[prop] = value;
    }

    if (document.compatMode === "BackCompat") {
        // jQuery has never supported or tested Quirks Mode
        migrateWarn("jQuery is not compatible with Quirks Mode");
    }


    var attrFn = jQuery("<input/>", {
        size: 1
    }).attr("size") && jQuery.attrFn,
        oldAttr = jQuery.attr,
        valueAttrGet = jQuery.attrHooks.value && jQuery.attrHooks.value.get || function() {
            return null;
        },
        valueAttrSet = jQuery.attrHooks.value && jQuery.attrHooks.value.set || function() {
            return undefined;
        },
        rnoType = /^(?:input|button)$/i,
        rnoAttrNodeType = /^[238]$/,
        rboolean = /^(?:autofocus|autoplay|async|checked|controls|defer|disabled|hidden|loop|multiple|open|readonly|required|scoped|selected)$/i,
        ruseDefault = /^(?:checked|selected)$/i;

    // jQuery.attrFn
    migrateWarnProp(jQuery, "attrFn", attrFn || {}, "jQuery.attrFn is deprecated");

    jQuery.attr = function(elem, name, value, pass) {
        var lowerName = name.toLowerCase(),
            nType = elem && elem.nodeType;

        if (pass) {
            // Since pass is used internally, we only warn for new jQuery
            // versions where there isn't a pass arg in the formal params
            if (oldAttr.length < 4) {
                migrateWarn("jQuery.fn.attr( props, pass ) is deprecated");
            }
            if (elem && !rnoAttrNodeType.test(nType) && (attrFn ? name in attrFn : jQuery.isFunction(jQuery.fn[name]))) {
                return jQuery(elem)[name](value);
            }
        }

        // Warn if user tries to set `type`, since it breaks on IE 6/7/8; by checking
        // for disconnected elements we don't warn on $( "<button>", { type: "button" } ).
        if (name === "type" && value !== undefined && rnoType.test(elem.nodeName) && elem.parentNode) {
            migrateWarn("Can't change the 'type' of an input or button in IE 6/7/8");
        }

        // Restore boolHook for boolean property/attribute synchronization
        if (!jQuery.attrHooks[lowerName] && rboolean.test(lowerName)) {
            jQuery.attrHooks[lowerName] = {
                get: function(elem, name) {
                    // Align boolean attributes with corresponding properties
                    // Fall back to attribute presence where some booleans are not supported
                    var attrNode,
                    property = jQuery.prop(elem, name);
                    return property === true || typeof property !== "boolean" && (attrNode = elem.getAttributeNode(name)) && attrNode.nodeValue !== false ?

                    name.toLowerCase() : undefined;
                },
                set: function(elem, value, name) {
                    var propName;
                    if (value === false) {
                        // Remove boolean attributes when set to false
                        jQuery.removeAttr(elem, name);
                    } else {
                        // value is true since we know at this point it's type boolean and not false
                        // Set boolean attributes to the same name and set the DOM property
                        propName = jQuery.propFix[name] || name;
                        if (propName in elem) {
                            // Only set the IDL specifically if it already exists on the element
                            elem[propName] = true;
                        }

                        elem.setAttribute(name, name.toLowerCase());
                    }
                    return name;
                }
            };

            // Warn only for attributes that can remain distinct from their properties post-1.9
            if (ruseDefault.test(lowerName)) {
                migrateWarn("jQuery.fn.attr('" + lowerName + "') may use property instead of attribute");
            }
        }

        return oldAttr.call(jQuery, elem, name, value);
    };

    // attrHooks: value
    jQuery.attrHooks.value = {
        get: function(elem, name) {
            var nodeName = (elem.nodeName || "").toLowerCase();
            if (nodeName === "button") {
                return valueAttrGet.apply(this, arguments);
            }
            if (nodeName !== "input" && nodeName !== "option") {
                migrateWarn("jQuery.fn.attr('value') no longer gets properties");
            }
            return name in elem ? elem.value : null;
        },
        set: function(elem, value) {
            var nodeName = (elem.nodeName || "").toLowerCase();
            if (nodeName === "button") {
                return valueAttrSet.apply(this, arguments);
            }
            if (nodeName !== "input" && nodeName !== "option") {
                migrateWarn("jQuery.fn.attr('value', val) no longer sets properties");
            }
            // Does not return so that setAttribute is also used
            elem.value = value;
        }
    };


    var matched, browser,
    oldInit = jQuery.fn.init,
        oldParseJSON = jQuery.parseJSON,
        // Note: XSS check is done below after string is trimmed
        rquickExpr = /^([^<]*)(<[\w\W]+>)([^>]*)$/;

    // $(html) "looks like html" rule change
    jQuery.fn.init = function(selector, context, rootjQuery) {
        var match;

        if (selector && typeof selector === "string" && !jQuery.isPlainObject(context) && (match = rquickExpr.exec(jQuery.trim(selector))) && match[0]) {
            // This is an HTML string according to the "old" rules; is it still?
            if (selector.charAt(0) !== "<") {
                migrateWarn("$(html) HTML strings must start with '<' character");
            }
            if (match[3]) {
                migrateWarn("$(html) HTML text after last tag is ignored");
            }
            // Consistently reject any HTML-like string starting with a hash (#9521)
            // Note that this may break jQuery 1.6.x code that otherwise would work.
            if (match[0].charAt(0) === "#") {
                migrateWarn("HTML string cannot start with a '#' character");
                jQuery.error("JQMIGRATE: Invalid selector string (XSS)");
            }
            // Now process using loose rules; let pre-1.8 play too
            if (context && context.context) {
                // jQuery object as context; parseHTML expects a DOM object
                context = context.context;
            }
            if (jQuery.parseHTML) {
                return oldInit.call(this, jQuery.parseHTML(match[2], context, true),
                context, rootjQuery);
            }
        }
        return oldInit.apply(this, arguments);
    };
    jQuery.fn.init.prototype = jQuery.fn;

    // Let $.parseJSON(falsy_value) return null
    jQuery.parseJSON = function(json) {
        if (!json && json !== null) {
            migrateWarn("jQuery.parseJSON requires a valid JSON string");
            return null;
        }
        return oldParseJSON.apply(this, arguments);
    };

    jQuery.uaMatch = function(ua) {
        ua = ua.toLowerCase();

        var match = /(chrome)[ \/]([\w.]+)/.exec(ua) || /(webkit)[ \/]([\w.]+)/.exec(ua) || /(opera)(?:.*version|)[ \/]([\w.]+)/.exec(ua) || /(msie) ([\w.]+)/.exec(ua) || ua.indexOf("compatible") < 0 && /(mozilla)(?:.*? rv:([\w.]+)|)/.exec(ua) || [];

        return {
            browser: match[1] || "",
            version: match[2] || "0"
        };
    };

    // Don't clobber any existing jQuery.browser in case it's different
    if (!jQuery.browser) {
        matched = jQuery.uaMatch(navigator.userAgent);
        browser = {};

        if (matched.browser) {
            browser[matched.browser] = true;
            browser.version = matched.version;
        }

        // Chrome is Webkit, but Webkit is also Safari.
        if (browser.chrome) {
            browser.webkit = true;
        } else if (browser.webkit) {
            browser.safari = true;
        }

        jQuery.browser = browser;
    }

    // Warn if the code tries to get jQuery.browser
    migrateWarnProp(jQuery, "browser", jQuery.browser, "jQuery.browser is deprecated");

    jQuery.sub = function() {
        function jQuerySub(selector, context) {
            return new jQuerySub.fn.init(selector, context);
        }
        jQuery.extend(true, jQuerySub, this);
        jQuerySub.superclass = this;
        jQuerySub.fn = jQuerySub.prototype = this();
        jQuerySub.fn.constructor = jQuerySub;
        jQuerySub.sub = this.sub;
        jQuerySub.fn.init = function init(selector, context) {
            if (context && context instanceof jQuery && !(context instanceof jQuerySub)) {
                context = jQuerySub(context);
            }

            return jQuery.fn.init.call(this, selector, context, rootjQuerySub);
        };
        jQuerySub.fn.init.prototype = jQuerySub.fn;
        var rootjQuerySub = jQuerySub(document);
        migrateWarn("jQuery.sub() is deprecated");
        return jQuerySub;
    };


    // Ensure that $.ajax gets the new parseJSON defined in core.js
    jQuery.ajaxSetup({
        converters: {
            "text json": jQuery.parseJSON
        }
    });


    var oldFnData = jQuery.fn.data;

    jQuery.fn.data = function(name) {
        var ret, evt,
        elem = this[0];

        // Handles 1.7 which has this behavior and 1.8 which doesn't
        if (elem && name === "events" && arguments.length === 1) {
            ret = jQuery.data(elem, name);
            evt = jQuery._data(elem, name);
            if ((ret === undefined || ret === evt) && evt !== undefined) {
                migrateWarn("Use of jQuery.fn.data('events') is deprecated");
                return evt;
            }
        }
        return oldFnData.apply(this, arguments);
    };


    var rscriptType = /\/(java|ecma)script/i,
        oldSelf = jQuery.fn.andSelf || jQuery.fn.addBack;

    jQuery.fn.andSelf = function() {
        migrateWarn("jQuery.fn.andSelf() replaced by jQuery.fn.addBack()");
        return oldSelf.apply(this, arguments);
    };

    // Since jQuery.clean is used internally on older versions, we only shim if it's missing
    if (!jQuery.clean) {
        jQuery.clean = function(elems, context, fragment, scripts) {
            // Set context per 1.8 logic
            context = context || document;
            context = !context.nodeType && context[0] || context;
            context = context.ownerDocument || context;

            migrateWarn("jQuery.clean() is deprecated");

            var i, elem, handleScript, jsTags,
            ret = [];

            jQuery.merge(ret, jQuery.buildFragment(elems, context).childNodes);

            // Complex logic lifted directly from jQuery 1.8
            if (fragment) {
                // Special handling of each script element
                handleScript = function(elem) {
                    // Check if we consider it executable
                    if (!elem.type || rscriptType.test(elem.type)) {
                        // Detach the script and store it in the scripts array (if provided) or the fragment
                        // Return truthy to indicate that it has been handled
                        return scripts ? scripts.push(elem.parentNode ? elem.parentNode.removeChild(elem) : elem) : fragment.appendChild(elem);
                    }
                };

                for (i = 0;
                (elem = ret[i]) != null; i++) {
                    // Check if we're done after handling an executable script
                    if (!(jQuery.nodeName(elem, "script") && handleScript(elem))) {
                        // Append to fragment and handle embedded scripts
                        fragment.appendChild(elem);
                        if (typeof elem.getElementsByTagName !== "undefined") {
                            // handleScript alters the DOM, so use jQuery.merge to ensure snapshot iteration
                            jsTags = jQuery.grep(jQuery.merge([], elem.getElementsByTagName("script")), handleScript);

                            // Splice the scripts into ret after their former ancestor and advance our index beyond them
                            ret.splice.apply(ret, [i + 1, 0].concat(jsTags));
                            i += jsTags.length;
                        }
                    }
                }
            }

            return ret;
        };
    }

    var eventAdd = jQuery.event.add,
        eventRemove = jQuery.event.remove,
        eventTrigger = jQuery.event.trigger,
        oldToggle = jQuery.fn.toggle,
        oldLive = jQuery.fn.live,
        oldDie = jQuery.fn.die,
        ajaxEvents = "ajaxStart|ajaxStop|ajaxSend|ajaxComplete|ajaxError|ajaxSuccess",
        rajaxEvent = new RegExp("\\b(?:" + ajaxEvents + ")\\b"),
        rhoverHack = /(?:^|\s)hover(\.\S+|)\b/,
        hoverHack = function(events) {
            if (typeof(events) !== "string" || jQuery.event.special.hover) {
                return events;
            }
            if (rhoverHack.test(events)) {
                migrateWarn("'hover' pseudo-event is deprecated, use 'mouseenter mouseleave'");
            }
            return events && events.replace(rhoverHack, "mouseenter$1 mouseleave$1");
        };

    // Event props removed in 1.9, put them back if needed; no practical way to warn them
    if (jQuery.event.props && jQuery.event.props[0] !== "attrChange") {
        jQuery.event.props.unshift("attrChange", "attrName", "relatedNode", "srcElement");
    }

    // Undocumented jQuery.event.handle was "deprecated" in jQuery 1.7
    if (jQuery.event.dispatch) {
        migrateWarnProp(jQuery.event, "handle", jQuery.event.dispatch, "jQuery.event.handle is undocumented and deprecated");
    }

    // Support for 'hover' pseudo-event and ajax event warnings
    jQuery.event.add = function(elem, types, handler, data, selector) {
        if (elem !== document && rajaxEvent.test(types)) {
            migrateWarn("AJAX events should be attached to document: " + types);
        }
        eventAdd.call(this, elem, hoverHack(types || ""), handler, data, selector);
    };
    jQuery.event.remove = function(elem, types, handler, selector, mappedTypes) {
        eventRemove.call(this, elem, hoverHack(types) || "", handler, selector, mappedTypes);
    };

    jQuery.fn.error = function() {
        var args = Array.prototype.slice.call(arguments, 0);
        migrateWarn("jQuery.fn.error() is deprecated");
        args.splice(0, 0, "error");
        if (arguments.length) {
            return this.bind.apply(this, args);
        }
        // error event should not bubble to window, although it does pre-1.7
        this.triggerHandler.apply(this, args);
        return this;
    };

    jQuery.fn.toggle = function(fn, fn2) {

        // Don't mess with animation or css toggles
        if (!jQuery.isFunction(fn) || !jQuery.isFunction(fn2)) {
            return oldToggle.apply(this, arguments);
        }
        migrateWarn("jQuery.fn.toggle(handler, handler...) is deprecated");

        // Save reference to arguments for access in closure
        var args = arguments,
            guid = fn.guid || jQuery.guid++,
            i = 0,
            toggler = function(event) {
                // Figure out which function to execute
                var lastToggle = (jQuery._data(this, "lastToggle" + fn.guid) || 0) % i;
                jQuery._data(this, "lastToggle" + fn.guid, lastToggle + 1);

                // Make sure that clicks stop
                event.preventDefault();

                // and execute the function
                return args[lastToggle].apply(this, arguments) || false;
            };

        // link all the functions, so any of them can unbind this click handler
        toggler.guid = guid;
        while (i < args.length) {
            args[i++].guid = guid;
        }

        return this.click(toggler);
    };

    jQuery.fn.live = function(types, data, fn) {
        migrateWarn("jQuery.fn.live() is deprecated");
        if (oldLive) {
            return oldLive.apply(this, arguments);
        }
        jQuery(this.context).on(types, this.selector, data, fn);
        return this;
    };

    jQuery.fn.die = function(types, fn) {
        migrateWarn("jQuery.fn.die() is deprecated");
        if (oldDie) {
            return oldDie.apply(this, arguments);
        }
        jQuery(this.context).off(types, this.selector || "**", fn);
        return this;
    };

    // Turn global events into document-triggered events
    jQuery.event.trigger = function(event, data, elem, onlyHandlers) {
        if (!elem && !rajaxEvent.test(event)) {
            migrateWarn("Global events are undocumented and deprecated");
        }
        return eventTrigger.call(this, event, data, elem || document, onlyHandlers);
    };
    jQuery.each(ajaxEvents.split("|"),

    function(_, name) {
        jQuery.event.special[name] = {
            setup: function() {
                var elem = this;

                // The document needs no shimming; must be !== for oldIE
                if (elem !== document) {
                    jQuery.event.add(document, name + "." + jQuery.guid, function() {
                        jQuery.event.trigger(name, null, elem, true);
                    });
                    jQuery._data(this, name, jQuery.guid++);
                }
                return false;
            },
            teardown: function() {
                if (this !== document) {
                    jQuery.event.remove(document, name + "." + jQuery._data(this, name));
                }
                return false;
            }
        };
    });


})(jQuery, window); 
//     Backbone.js 1.1.2

//     (c) 2010-2014 Jeremy Ashkenas, DocumentCloud and Investigative Reporters & Editors
//     Backbone may be freely distributed under the MIT license.
//     For all details and documentation:
//     http://backbonejs.org

(function(root, factory) {

  // Set up Backbone appropriately for the environment. Start with AMD.
  if (typeof define === 'function' && define.amd) {
    define('backbone',['underscore', 'jquery', 'exports'], function(_, $, exports) {
      // Export global even in AMD case in case this script is loaded with
      // others that may still expect a global Backbone.
      root.Backbone = factory(root, exports, _, $);
    });

  // Next for Node.js or CommonJS. jQuery may not be needed as a module.
  } else if (typeof exports !== 'undefined') {
    var _ = require('underscore');
    factory(root, exports, _);

  // Finally, as a browser global.
  } else {
    root.Backbone = factory(root, {}, root._, (root.jQuery || root.Zepto || root.ender || root.$));
  }

}(this, function(root, Backbone, _, $) {

  // Initial Setup
  // -------------

  // Save the previous value of the `Backbone` variable, so that it can be
  // restored later on, if `noConflict` is used.
  var previousBackbone = root.Backbone;

  // Create local references to array methods we'll want to use later.
  var array = [];
  var push = array.push;
  var slice = array.slice;
  var splice = array.splice;

  // Current version of the library. Keep in sync with `package.json`.
  Backbone.VERSION = '1.1.2';

  // For Backbone's purposes, jQuery, Zepto, Ender, or My Library (kidding) owns
  // the `$` variable.
  Backbone.$ = $;

  // Runs Backbone.js in *noConflict* mode, returning the `Backbone` variable
  // to its previous owner. Returns a reference to this Backbone object.
  Backbone.noConflict = function() {
    root.Backbone = previousBackbone;
    return this;
  };

  // Turn on `emulateHTTP` to support legacy HTTP servers. Setting this option
  // will fake `"PATCH"`, `"PUT"` and `"DELETE"` requests via the `_method` parameter and
  // set a `X-Http-Method-Override` header.
  Backbone.emulateHTTP = false;

  // Turn on `emulateJSON` to support legacy servers that can't deal with direct
  // `application/json` requests ... will encode the body as
  // `application/x-www-form-urlencoded` instead and will send the model in a
  // form param named `model`.
  Backbone.emulateJSON = false;

  // Backbone.Events
  // ---------------

  // A module that can be mixed in to *any object* in order to provide it with
  // custom events. You may bind with `on` or remove with `off` callback
  // functions to an event; `trigger`-ing an event fires all callbacks in
  // succession.
  //
  //     var object = {};
  //     _.extend(object, Backbone.Events);
  //     object.on('expand', function(){ alert('expanded'); });
  //     object.trigger('expand');
  //
  var Events = Backbone.Events = {

    // Bind an event to a `callback` function. Passing `"all"` will bind
    // the callback to all events fired.
    on: function(name, callback, context) {
      if (!eventsApi(this, 'on', name, [callback, context]) || !callback) return this;
      this._events || (this._events = {});
      var events = this._events[name] || (this._events[name] = []);
      events.push({callback: callback, context: context, ctx: context || this});
      return this;
    },

    // Bind an event to only be triggered a single time. After the first time
    // the callback is invoked, it will be removed.
    once: function(name, callback, context) {
      if (!eventsApi(this, 'once', name, [callback, context]) || !callback) return this;
      var self = this;
      var once = _.once(function() {
        self.off(name, once);
        callback.apply(this, arguments);
      });
      once._callback = callback;
      return this.on(name, once, context);
    },

    // Remove one or many callbacks. If `context` is null, removes all
    // callbacks with that function. If `callback` is null, removes all
    // callbacks for the event. If `name` is null, removes all bound
    // callbacks for all events.
    off: function(name, callback, context) {
      var retain, ev, events, names, i, l, j, k;
      if (!this._events || !eventsApi(this, 'off', name, [callback, context])) return this;
      if (!name && !callback && !context) {
        this._events = void 0;
        return this;
      }
      names = name ? [name] : _.keys(this._events);
      for (i = 0, l = names.length; i < l; i++) {
        name = names[i];
        if (events = this._events[name]) {
          this._events[name] = retain = [];
          if (callback || context) {
            for (j = 0, k = events.length; j < k; j++) {
              ev = events[j];
              if ((callback && callback !== ev.callback && callback !== ev.callback._callback) ||
                  (context && context !== ev.context)) {
                retain.push(ev);
              }
            }
          }
          if (!retain.length) delete this._events[name];
        }
      }

      return this;
    },

    // Trigger one or many events, firing all bound callbacks. Callbacks are
    // passed the same arguments as `trigger` is, apart from the event name
    // (unless you're listening on `"all"`, which will cause your callback to
    // receive the true name of the event as the first argument).
    trigger: function(name) {
      if (!this._events) return this;
      var args = slice.call(arguments, 1);
      if (!eventsApi(this, 'trigger', name, args)) return this;
      var events = this._events[name];
      var allEvents = this._events.all;
      if (events) triggerEvents(events, args);
      if (allEvents) triggerEvents(allEvents, arguments);
      return this;
    },

    // Tell this object to stop listening to either specific events ... or
    // to every object it's currently listening to.
    stopListening: function(obj, name, callback) {
      var listeningTo = this._listeningTo;
      if (!listeningTo) return this;
      var remove = !name && !callback;
      if (!callback && typeof name === 'object') callback = this;
      if (obj) (listeningTo = {})[obj._listenId] = obj;
      for (var id in listeningTo) {
        obj = listeningTo[id];
        obj.off(name, callback, this);
        if (remove || _.isEmpty(obj._events)) delete this._listeningTo[id];
      }
      return this;
    }

  };

  // Regular expression used to split event strings.
  var eventSplitter = /\s+/;

  // Implement fancy features of the Events API such as multiple event
  // names `"change blur"` and jQuery-style event maps `{change: action}`
  // in terms of the existing API.
  var eventsApi = function(obj, action, name, rest) {
    if (!name) return true;

    // Handle event maps.
    if (typeof name === 'object') {
      for (var key in name) {
        obj[action].apply(obj, [key, name[key]].concat(rest));
      }
      return false;
    }

    // Handle space separated event names.
    if (eventSplitter.test(name)) {
      var names = name.split(eventSplitter);
      for (var i = 0, l = names.length; i < l; i++) {
        obj[action].apply(obj, [names[i]].concat(rest));
      }
      return false;
    }

    return true;
  };

  // A difficult-to-believe, but optimized internal dispatch function for
  // triggering events. Tries to keep the usual cases speedy (most internal
  // Backbone events have 3 arguments).
  var triggerEvents = function(events, args) {
    var ev, i = -1, l = events.length, a1 = args[0], a2 = args[1], a3 = args[2];
    switch (args.length) {
      case 0: while (++i < l) (ev = events[i]).callback.call(ev.ctx); return;
      case 1: while (++i < l) (ev = events[i]).callback.call(ev.ctx, a1); return;
      case 2: while (++i < l) (ev = events[i]).callback.call(ev.ctx, a1, a2); return;
      case 3: while (++i < l) (ev = events[i]).callback.call(ev.ctx, a1, a2, a3); return;
      default: while (++i < l) (ev = events[i]).callback.apply(ev.ctx, args); return;
    }
  };

  var listenMethods = {listenTo: 'on', listenToOnce: 'once'};

  // Inversion-of-control versions of `on` and `once`. Tell *this* object to
  // listen to an event in another object ... keeping track of what it's
  // listening to.
  _.each(listenMethods, function(implementation, method) {
    Events[method] = function(obj, name, callback) {
      var listeningTo = this._listeningTo || (this._listeningTo = {});
      var id = obj._listenId || (obj._listenId = _.uniqueId('l'));
      listeningTo[id] = obj;
      if (!callback && typeof name === 'object') callback = this;
      obj[implementation](name, callback, this);
      return this;
    };
  });

  // Aliases for backwards compatibility.
  Events.bind   = Events.on;
  Events.unbind = Events.off;

  // Allow the `Backbone` object to serve as a global event bus, for folks who
  // want global "pubsub" in a convenient place.
  _.extend(Backbone, Events);

  // Backbone.Model
  // --------------

  // Backbone **Models** are the basic data object in the framework --
  // frequently representing a row in a table in a database on your server.
  // A discrete chunk of data and a bunch of useful, related methods for
  // performing computations and transformations on that data.

  // Create a new model with the specified attributes. A client id (`cid`)
  // is automatically generated and assigned for you.
  var Model = Backbone.Model = function(attributes, options) {
    var attrs = attributes || {};
    options || (options = {});
    this.cid = _.uniqueId('c');
    this.attributes = {};
    if (options.collection) this.collection = options.collection;
    if (options.parse) attrs = this.parse(attrs, options) || {};
    attrs = _.defaults({}, attrs, _.result(this, 'defaults'));
    this.set(attrs, options);
    this.changed = {};
    this.initialize.apply(this, arguments);
  };

  // Attach all inheritable methods to the Model prototype.
  _.extend(Model.prototype, Events, {

    // A hash of attributes whose current and previous value differ.
    changed: null,

    // The value returned during the last failed validation.
    validationError: null,

    // The default name for the JSON `id` attribute is `"id"`. MongoDB and
    // CouchDB users may want to set this to `"_id"`.
    idAttribute: 'id',

    // Initialize is an empty function by default. Override it with your own
    // initialization logic.
    initialize: function(){},

    // Return a copy of the model's `attributes` object.
    toJSON: function(options) {
      return _.clone(this.attributes);
    },

    // Proxy `Backbone.sync` by default -- but override this if you need
    // custom syncing semantics for *this* particular model.
    sync: function() {
      return Backbone.sync.apply(this, arguments);
    },

    // Get the value of an attribute.
    get: function(attr) {
      return this.attributes[attr];
    },

    // Get the HTML-escaped value of an attribute.
    escape: function(attr) {
      return _.escape(this.get(attr));
    },

    // Returns `true` if the attribute contains a value that is not null
    // or undefined.
    has: function(attr) {
      return this.get(attr) != null;
    },

    // Set a hash of model attributes on the object, firing `"change"`. This is
    // the core primitive operation of a model, updating the data and notifying
    // anyone who needs to know about the change in state. The heart of the beast.
    set: function(key, val, options) {
      var attr, attrs, unset, changes, silent, changing, prev, current;
      if (key == null) return this;

      // Handle both `"key", value` and `{key: value}` -style arguments.
      if (typeof key === 'object') {
        attrs = key;
        options = val;
      } else {
        (attrs = {})[key] = val;
      }

      options || (options = {});

      // Run validation.
      if (!this._validate(attrs, options)) return false;

      // Extract attributes and options.
      unset           = options.unset;
      silent          = options.silent;
      changes         = [];
      changing        = this._changing;
      this._changing  = true;

      if (!changing) {
        this._previousAttributes = _.clone(this.attributes);
        this.changed = {};
      }
      current = this.attributes, prev = this._previousAttributes;

      // Check for changes of `id`.
      if (this.idAttribute in attrs) this.id = attrs[this.idAttribute];

      // For each `set` attribute, update or delete the current value.
      for (attr in attrs) {
        val = attrs[attr];
        if (!_.isEqual(current[attr], val)) changes.push(attr);
        if (!_.isEqual(prev[attr], val)) {
          this.changed[attr] = val;
        } else {
          delete this.changed[attr];
        }
        unset ? delete current[attr] : current[attr] = val;
      }

      // Trigger all relevant attribute changes.
      if (!silent) {
        if (changes.length) this._pending = options;
        for (var i = 0, l = changes.length; i < l; i++) {
          this.trigger('change:' + changes[i], this, current[changes[i]], options);
        }
      }

      // You might be wondering why there's a `while` loop here. Changes can
      // be recursively nested within `"change"` events.
      if (changing) return this;
      if (!silent) {
        while (this._pending) {
          options = this._pending;
          this._pending = false;
          this.trigger('change', this, options);
        }
      }
      this._pending = false;
      this._changing = false;
      return this;
    },

    // Remove an attribute from the model, firing `"change"`. `unset` is a noop
    // if the attribute doesn't exist.
    unset: function(attr, options) {
      return this.set(attr, void 0, _.extend({}, options, {unset: true}));
    },

    // Clear all attributes on the model, firing `"change"`.
    clear: function(options) {
      var attrs = {};
      for (var key in this.attributes) attrs[key] = void 0;
      return this.set(attrs, _.extend({}, options, {unset: true}));
    },

    // Determine if the model has changed since the last `"change"` event.
    // If you specify an attribute name, determine if that attribute has changed.
    hasChanged: function(attr) {
      if (attr == null) return !_.isEmpty(this.changed);
      return _.has(this.changed, attr);
    },

    // Return an object containing all the attributes that have changed, or
    // false if there are no changed attributes. Useful for determining what
    // parts of a view need to be updated and/or what attributes need to be
    // persisted to the server. Unset attributes will be set to undefined.
    // You can also pass an attributes object to diff against the model,
    // determining if there *would be* a change.
    changedAttributes: function(diff) {
      if (!diff) return this.hasChanged() ? _.clone(this.changed) : false;
      var val, changed = false;
      var old = this._changing ? this._previousAttributes : this.attributes;
      for (var attr in diff) {
        if (_.isEqual(old[attr], (val = diff[attr]))) continue;
        (changed || (changed = {}))[attr] = val;
      }
      return changed;
    },

    // Get the previous value of an attribute, recorded at the time the last
    // `"change"` event was fired.
    previous: function(attr) {
      if (attr == null || !this._previousAttributes) return null;
      return this._previousAttributes[attr];
    },

    // Get all of the attributes of the model at the time of the previous
    // `"change"` event.
    previousAttributes: function() {
      return _.clone(this._previousAttributes);
    },

    // Fetch the model from the server. If the server's representation of the
    // model differs from its current attributes, they will be overridden,
    // triggering a `"change"` event.
    fetch: function(options) {
      options = options ? _.clone(options) : {};
      if (options.parse === void 0) options.parse = true;
      var model = this;
      var success = options.success;
      options.success = function(resp) {
        if (!model.set(model.parse(resp, options), options)) return false;
        if (success) success(model, resp, options);
        model.trigger('sync', model, resp, options);
      };
      wrapError(this, options);
      return this.sync('read', this, options);
    },

    // Set a hash of model attributes, and sync the model to the server.
    // If the server returns an attributes hash that differs, the model's
    // state will be `set` again.
    save: function(key, val, options) {
      var attrs, method, xhr, attributes = this.attributes;

      // Handle both `"key", value` and `{key: value}` -style arguments.
      if (key == null || typeof key === 'object') {
        attrs = key;
        options = val;
      } else {
        (attrs = {})[key] = val;
      }

      options = _.extend({validate: true}, options);

      // If we're not waiting and attributes exist, save acts as
      // `set(attr).save(null, opts)` with validation. Otherwise, check if
      // the model will be valid when the attributes, if any, are set.
      if (attrs && !options.wait) {
        if (!this.set(attrs, options)) return false;
      } else {
        if (!this._validate(attrs, options)) return false;
      }

      // Set temporary attributes if `{wait: true}`.
      if (attrs && options.wait) {
        this.attributes = _.extend({}, attributes, attrs);
      }

      // After a successful server-side save, the client is (optionally)
      // updated with the server-side state.
      if (options.parse === void 0) options.parse = true;
      var model = this;
      var success = options.success;
      options.success = function(resp) {
        // Ensure attributes are restored during synchronous saves.
        model.attributes = attributes;
        var serverAttrs = model.parse(resp, options);
        if (options.wait) serverAttrs = _.extend(attrs || {}, serverAttrs);
        if (_.isObject(serverAttrs) && !model.set(serverAttrs, options)) {
          return false;
        }
        if (success) success(model, resp, options);
        model.trigger('sync', model, resp, options);
      };
      wrapError(this, options);

      method = this.isNew() ? 'create' : (options.patch ? 'patch' : 'update');
      if (method === 'patch') options.attrs = attrs;
      xhr = this.sync(method, this, options);

      // Restore attributes.
      if (attrs && options.wait) this.attributes = attributes;

      return xhr;
    },

    // Destroy this model on the server if it was already persisted.
    // Optimistically removes the model from its collection, if it has one.
    // If `wait: true` is passed, waits for the server to respond before removal.
    destroy: function(options) {
      options = options ? _.clone(options) : {};
      var model = this;
      var success = options.success;

      var destroy = function() {
        model.trigger('destroy', model, model.collection, options);
      };

      options.success = function(resp) {
        if (options.wait || model.isNew()) destroy();
        if (success) success(model, resp, options);
        if (!model.isNew()) model.trigger('sync', model, resp, options);
      };

      if (this.isNew()) {
        options.success();
        return false;
      }
      wrapError(this, options);

      var xhr = this.sync('delete', this, options);
      if (!options.wait) destroy();
      return xhr;
    },

    // Default URL for the model's representation on the server -- if you're
    // using Backbone's restful methods, override this to change the endpoint
    // that will be called.
    url: function() {
      var base =
        _.result(this, 'urlRoot') ||
        _.result(this.collection, 'url') ||
        urlError();
      if (this.isNew()) return base;
      return base.replace(/([^\/])$/, '$1/') + encodeURIComponent(this.id);
    },

    // **parse** converts a response into the hash of attributes to be `set` on
    // the model. The default implementation is just to pass the response along.
    parse: function(resp, options) {
      return resp;
    },

    // Create a new model with identical attributes to this one.
    clone: function() {
      return new this.constructor(this.attributes);
    },

    // A model is new if it has never been saved to the server, and lacks an id.
    isNew: function() {
      return !this.has(this.idAttribute);
    },

    // Check if the model is currently in a valid state.
    isValid: function(options) {
      return this._validate({}, _.extend(options || {}, { validate: true }));
    },

    // Run validation against the next complete set of model attributes,
    // returning `true` if all is well. Otherwise, fire an `"invalid"` event.
    _validate: function(attrs, options) {
      if (!options.validate || !this.validate) return true;
      attrs = _.extend({}, this.attributes, attrs);
      var error = this.validationError = this.validate(attrs, options) || null;
      if (!error) return true;
      this.trigger('invalid', this, error, _.extend(options, {validationError: error}));
      return false;
    }

  });

  // Underscore methods that we want to implement on the Model.
  var modelMethods = ['keys', 'values', 'pairs', 'invert', 'pick', 'omit'];

  // Mix in each Underscore method as a proxy to `Model#attributes`.
  _.each(modelMethods, function(method) {
    Model.prototype[method] = function() {
      var args = slice.call(arguments);
      args.unshift(this.attributes);
      return _[method].apply(_, args);
    };
  });

  // Backbone.Collection
  // -------------------

  // If models tend to represent a single row of data, a Backbone Collection is
  // more analagous to a table full of data ... or a small slice or page of that
  // table, or a collection of rows that belong together for a particular reason
  // -- all of the messages in this particular folder, all of the documents
  // belonging to this particular author, and so on. Collections maintain
  // indexes of their models, both in order, and for lookup by `id`.

  // Create a new **Collection**, perhaps to contain a specific type of `model`.
  // If a `comparator` is specified, the Collection will maintain
  // its models in sort order, as they're added and removed.
  var Collection = Backbone.Collection = function(models, options) {
    options || (options = {});
    if (options.model) this.model = options.model;
    if (options.comparator !== void 0) this.comparator = options.comparator;
    this._reset();
    this.initialize.apply(this, arguments);
    if (models) this.reset(models, _.extend({silent: true}, options));
  };

  // Default options for `Collection#set`.
  var setOptions = {add: true, remove: true, merge: true};
  var addOptions = {add: true, remove: false};

  // Define the Collection's inheritable methods.
  _.extend(Collection.prototype, Events, {

    // The default model for a collection is just a **Backbone.Model**.
    // This should be overridden in most cases.
    model: Model,

    // Initialize is an empty function by default. Override it with your own
    // initialization logic.
    initialize: function(){},

    // The JSON representation of a Collection is an array of the
    // models' attributes.
    toJSON: function(options) {
      return this.map(function(model){ return model.toJSON(options); });
    },

    // Proxy `Backbone.sync` by default.
    sync: function() {
      return Backbone.sync.apply(this, arguments);
    },

    // Add a model, or list of models to the set.
    add: function(models, options) {
      return this.set(models, _.extend({merge: false}, options, addOptions));
    },

    // Remove a model, or a list of models from the set.
    remove: function(models, options) {
      var singular = !_.isArray(models);
      models = singular ? [models] : _.clone(models);
      options || (options = {});
      var i, l, index, model;
      for (i = 0, l = models.length; i < l; i++) {
        model = models[i] = this.get(models[i]);
        if (!model) continue;
        delete this._byId[model.id];
        delete this._byId[model.cid];
        index = this.indexOf(model);
        this.models.splice(index, 1);
        this.length--;
        if (!options.silent) {
          options.index = index;
          model.trigger('remove', model, this, options);
        }
        this._removeReference(model, options);
      }
      return singular ? models[0] : models;
    },

    // Update a collection by `set`-ing a new list of models, adding new ones,
    // removing models that are no longer present, and merging models that
    // already exist in the collection, as necessary. Similar to **Model#set**,
    // the core operation for updating the data contained by the collection.
    set: function(models, options) {
      options = _.defaults({}, options, setOptions);
      if (options.parse) models = this.parse(models, options);
      var singular = !_.isArray(models);
      models = singular ? (models ? [models] : []) : _.clone(models);
      var i, l, id, model, attrs, existing, sort;
      var at = options.at;
      var targetModel = this.model;
      var sortable = this.comparator && (at == null) && options.sort !== false;
      var sortAttr = _.isString(this.comparator) ? this.comparator : null;
      var toAdd = [], toRemove = [], modelMap = {};
      var add = options.add, merge = options.merge, remove = options.remove;
      var order = !sortable && add && remove ? [] : false;

      // Turn bare objects into model references, and prevent invalid models
      // from being added.
      for (i = 0, l = models.length; i < l; i++) {
        attrs = models[i] || {};
        if (attrs instanceof Model) {
          id = model = attrs;
        } else {
          id = attrs[targetModel.prototype.idAttribute || 'id'];
        }

        // If a duplicate is found, prevent it from being added and
        // optionally merge it into the existing model.
        if (existing = this.get(id)) {
          if (remove) modelMap[existing.cid] = true;
          if (merge) {
            attrs = attrs === model ? model.attributes : attrs;
            if (options.parse) attrs = existing.parse(attrs, options);
            existing.set(attrs, options);
            if (sortable && !sort && existing.hasChanged(sortAttr)) sort = true;
          }
          models[i] = existing;

        // If this is a new, valid model, push it to the `toAdd` list.
        } else if (add) {
          model = models[i] = this._prepareModel(attrs, options);
          if (!model) continue;
          toAdd.push(model);
          this._addReference(model, options);
        }

        // Do not add multiple models with the same `id`.
        model = existing || model;
        if (order && (model.isNew() || !modelMap[model.id])) order.push(model);
        modelMap[model.id] = true;
      }

      // Remove nonexistent models if appropriate.
      if (remove) {
        for (i = 0, l = this.length; i < l; ++i) {
          if (!modelMap[(model = this.models[i]).cid]) toRemove.push(model);
        }
        if (toRemove.length) this.remove(toRemove, options);
      }

      // See if sorting is needed, update `length` and splice in new models.
      if (toAdd.length || (order && order.length)) {
        if (sortable) sort = true;
        this.length += toAdd.length;
        if (at != null) {
          for (i = 0, l = toAdd.length; i < l; i++) {
            this.models.splice(at + i, 0, toAdd[i]);
          }
        } else {
          if (order) this.models.length = 0;
          var orderedModels = order || toAdd;
          for (i = 0, l = orderedModels.length; i < l; i++) {
            this.models.push(orderedModels[i]);
          }
        }
      }

      // Silently sort the collection if appropriate.
      if (sort) this.sort({silent: true});

      // Unless silenced, it's time to fire all appropriate add/sort events.
      if (!options.silent) {
        for (i = 0, l = toAdd.length; i < l; i++) {
          (model = toAdd[i]).trigger('add', model, this, options);
        }
        if (sort || (order && order.length)) this.trigger('sort', this, options);
      }

      // Return the added (or merged) model (or models).
      return singular ? models[0] : models;
    },

    // When you have more items than you want to add or remove individually,
    // you can reset the entire set with a new list of models, without firing
    // any granular `add` or `remove` events. Fires `reset` when finished.
    // Useful for bulk operations and optimizations.
    reset: function(models, options) {
      options || (options = {});
      for (var i = 0, l = this.models.length; i < l; i++) {
        this._removeReference(this.models[i], options);
      }
      options.previousModels = this.models;
      this._reset();
      models = this.add(models, _.extend({silent: true}, options));
      if (!options.silent) this.trigger('reset', this, options);
      return models;
    },

    // Add a model to the end of the collection.
    push: function(model, options) {
      return this.add(model, _.extend({at: this.length}, options));
    },

    // Remove a model from the end of the collection.
    pop: function(options) {
      var model = this.at(this.length - 1);
      this.remove(model, options);
      return model;
    },

    // Add a model to the beginning of the collection.
    unshift: function(model, options) {
      return this.add(model, _.extend({at: 0}, options));
    },

    // Remove a model from the beginning of the collection.
    shift: function(options) {
      var model = this.at(0);
      this.remove(model, options);
      return model;
    },

    // Slice out a sub-array of models from the collection.
    slice: function() {
      return slice.apply(this.models, arguments);
    },

    // Get a model from the set by id.
    get: function(obj) {
      if (obj == null) return void 0;
      return this._byId[obj] || this._byId[obj.id] || this._byId[obj.cid];
    },

    // Get the model at the given index.
    at: function(index) {
      return this.models[index];
    },

    // Return models with matching attributes. Useful for simple cases of
    // `filter`.
    where: function(attrs, first) {
      if (_.isEmpty(attrs)) return first ? void 0 : [];
      return this[first ? 'find' : 'filter'](function(model) {
        for (var key in attrs) {
          if (attrs[key] !== model.get(key)) return false;
        }
        return true;
      });
    },

    // Return the first model with matching attributes. Useful for simple cases
    // of `find`.
    findWhere: function(attrs) {
      return this.where(attrs, true);
    },

    // Force the collection to re-sort itself. You don't need to call this under
    // normal circumstances, as the set will maintain sort order as each item
    // is added.
    sort: function(options) {
      if (!this.comparator) throw new Error('Cannot sort a set without a comparator');
      options || (options = {});

      // Run sort based on type of `comparator`.
      if (_.isString(this.comparator) || this.comparator.length === 1) {
        this.models = this.sortBy(this.comparator, this);
      } else {
        this.models.sort(_.bind(this.comparator, this));
      }

      if (!options.silent) this.trigger('sort', this, options);
      return this;
    },

    // Pluck an attribute from each model in the collection.
    pluck: function(attr) {
      return _.invoke(this.models, 'get', attr);
    },

    // Fetch the default set of models for this collection, resetting the
    // collection when they arrive. If `reset: true` is passed, the response
    // data will be passed through the `reset` method instead of `set`.
    fetch: function(options) {
      options = options ? _.clone(options) : {};
      if (options.parse === void 0) options.parse = true;
      var success = options.success;
      var collection = this;
      options.success = function(resp) {
        var method = options.reset ? 'reset' : 'set';
        collection[method](resp, options);
        if (success) success(collection, resp, options);
        collection.trigger('sync', collection, resp, options);
      };
      wrapError(this, options);
      return this.sync('read', this, options);
    },

    // Create a new instance of a model in this collection. Add the model to the
    // collection immediately, unless `wait: true` is passed, in which case we
    // wait for the server to agree.
    create: function(model, options) {
      options = options ? _.clone(options) : {};
      if (!(model = this._prepareModel(model, options))) return false;
      if (!options.wait) this.add(model, options);
      var collection = this;
      var success = options.success;
      options.success = function(model, resp) {
        if (options.wait) collection.add(model, options);
        if (success) success(model, resp, options);
      };
      model.save(null, options);
      return model;
    },

    // **parse** converts a response into a list of models to be added to the
    // collection. The default implementation is just to pass it through.
    parse: function(resp, options) {
      return resp;
    },

    // Create a new collection with an identical list of models as this one.
    clone: function() {
      return new this.constructor(this.models);
    },

    // Private method to reset all internal state. Called when the collection
    // is first initialized or reset.
    _reset: function() {
      this.length = 0;
      this.models = [];
      this._byId  = {};
    },

    // Prepare a hash of attributes (or other model) to be added to this
    // collection.
    _prepareModel: function(attrs, options) {
      if (attrs instanceof Model) return attrs;
      options = options ? _.clone(options) : {};
      options.collection = this;
      var model = new this.model(attrs, options);
      if (!model.validationError) return model;
      this.trigger('invalid', this, model.validationError, options);
      return false;
    },

    // Internal method to create a model's ties to a collection.
    _addReference: function(model, options) {
      this._byId[model.cid] = model;
      if (model.id != null) this._byId[model.id] = model;
      if (!model.collection) model.collection = this;
      model.on('all', this._onModelEvent, this);
    },

    // Internal method to sever a model's ties to a collection.
    _removeReference: function(model, options) {
      if (this === model.collection) delete model.collection;
      model.off('all', this._onModelEvent, this);
    },

    // Internal method called every time a model in the set fires an event.
    // Sets need to update their indexes when models change ids. All other
    // events simply proxy through. "add" and "remove" events that originate
    // in other collections are ignored.
    _onModelEvent: function(event, model, collection, options) {
      if ((event === 'add' || event === 'remove') && collection !== this) return;
      if (event === 'destroy') this.remove(model, options);
      if (model && event === 'change:' + model.idAttribute) {
        delete this._byId[model.previous(model.idAttribute)];
        if (model.id != null) this._byId[model.id] = model;
      }
      this.trigger.apply(this, arguments);
    }

  });

  // Underscore methods that we want to implement on the Collection.
  // 90% of the core usefulness of Backbone Collections is actually implemented
  // right here:
  var methods = ['forEach', 'each', 'map', 'collect', 'reduce', 'foldl',
    'inject', 'reduceRight', 'foldr', 'find', 'detect', 'filter', 'select',
    'reject', 'every', 'all', 'some', 'any', 'include', 'contains', 'invoke',
    'max', 'min', 'toArray', 'size', 'first', 'head', 'take', 'initial', 'rest',
    'tail', 'drop', 'last', 'without', 'difference', 'indexOf', 'shuffle',
    'lastIndexOf', 'isEmpty', 'chain', 'sample'];

  // Mix in each Underscore method as a proxy to `Collection#models`.
  _.each(methods, function(method) {
    Collection.prototype[method] = function() {
      var args = slice.call(arguments);
      args.unshift(this.models);
      return _[method].apply(_, args);
    };
  });

  // Underscore methods that take a property name as an argument.
  var attributeMethods = ['groupBy', 'countBy', 'sortBy', 'indexBy'];

  // Use attributes instead of properties.
  _.each(attributeMethods, function(method) {
    Collection.prototype[method] = function(value, context) {
      var iterator = _.isFunction(value) ? value : function(model) {
        return model.get(value);
      };
      return _[method](this.models, iterator, context);
    };
  });

  // Backbone.View
  // -------------

  // Backbone Views are almost more convention than they are actual code. A View
  // is simply a JavaScript object that represents a logical chunk of UI in the
  // DOM. This might be a single item, an entire list, a sidebar or panel, or
  // even the surrounding frame which wraps your whole app. Defining a chunk of
  // UI as a **View** allows you to define your DOM events declaratively, without
  // having to worry about render order ... and makes it easy for the view to
  // react to specific changes in the state of your models.

  // Creating a Backbone.View creates its initial element outside of the DOM,
  // if an existing element is not provided...
  var View = Backbone.View = function(options) {
    this.cid = _.uniqueId('view');
    options || (options = {});
    _.extend(this, _.pick(options, viewOptions));
    this._ensureElement();
    this.initialize.apply(this, arguments);
    this.delegateEvents();
  };

  // Cached regex to split keys for `delegate`.
  var delegateEventSplitter = /^(\S+)\s*(.*)$/;

  // List of view options to be merged as properties.
  var viewOptions = ['model', 'collection', 'el', 'id', 'attributes', 'className', 'tagName', 'events'];

  // Set up all inheritable **Backbone.View** properties and methods.
  _.extend(View.prototype, Events, {

    // The default `tagName` of a View's element is `"div"`.
    tagName: 'div',

    // jQuery delegate for element lookup, scoped to DOM elements within the
    // current view. This should be preferred to global lookups where possible.
    $: function(selector) {
      return this.$el.find(selector);
    },

    // Initialize is an empty function by default. Override it with your own
    // initialization logic.
    initialize: function(){},

    // **render** is the core function that your view should override, in order
    // to populate its element (`this.el`), with the appropriate HTML. The
    // convention is for **render** to always return `this`.
    render: function() {
      return this;
    },

    // Remove this view by taking the element out of the DOM, and removing any
    // applicable Backbone.Events listeners.
    remove: function() {
      this.$el.remove();
      this.stopListening();
      return this;
    },

    // Change the view's element (`this.el` property), including event
    // re-delegation.
    setElement: function(element, delegate) {
      if (this.$el) this.undelegateEvents();
      this.$el = element instanceof Backbone.$ ? element : Backbone.$(element);
      this.el = this.$el[0];
      if (delegate !== false) this.delegateEvents();
      return this;
    },

    // Set callbacks, where `this.events` is a hash of
    //
    // *{"event selector": "callback"}*
    //
    //     {
    //       'mousedown .title':  'edit',
    //       'click .button':     'save',
    //       'click .open':       function(e) { ... }
    //     }
    //
    // pairs. Callbacks will be bound to the view, with `this` set properly.
    // Uses event delegation for efficiency.
    // Omitting the selector binds the event to `this.el`.
    // This only works for delegate-able events: not `focus`, `blur`, and
    // not `change`, `submit`, and `reset` in Internet Explorer.
    delegateEvents: function(events) {
      if (!(events || (events = _.result(this, 'events')))) return this;
      this.undelegateEvents();
      for (var key in events) {
        var method = events[key];
        if (!_.isFunction(method)) method = this[events[key]];
        if (!method) continue;

        var match = key.match(delegateEventSplitter);
        var eventName = match[1], selector = match[2];
        method = _.bind(method, this);
        eventName += '.delegateEvents' + this.cid;
        if (selector === '') {
          this.$el.on(eventName, method);
        } else {
          this.$el.on(eventName, selector, method);
        }
      }
      return this;
    },

    // Clears all callbacks previously bound to the view with `delegateEvents`.
    // You usually don't need to use this, but may wish to if you have multiple
    // Backbone views attached to the same DOM element.
    undelegateEvents: function() {
      this.$el.off('.delegateEvents' + this.cid);
      return this;
    },

    // Ensure that the View has a DOM element to render into.
    // If `this.el` is a string, pass it through `$()`, take the first
    // matching element, and re-assign it to `el`. Otherwise, create
    // an element from the `id`, `className` and `tagName` properties.
    _ensureElement: function() {
      if (!this.el) {
        var attrs = _.extend({}, _.result(this, 'attributes'));
        if (this.id) attrs.id = _.result(this, 'id');
        if (this.className) attrs['class'] = _.result(this, 'className');
        var $el = Backbone.$('<' + _.result(this, 'tagName') + '>').attr(attrs);
        this.setElement($el, false);
      } else {
        this.setElement(_.result(this, 'el'), false);
      }
    }

  });

  // Backbone.sync
  // -------------

  // Override this function to change the manner in which Backbone persists
  // models to the server. You will be passed the type of request, and the
  // model in question. By default, makes a RESTful Ajax request
  // to the model's `url()`. Some possible customizations could be:
  //
  // * Use `setTimeout` to batch rapid-fire updates into a single request.
  // * Send up the models as XML instead of JSON.
  // * Persist models via WebSockets instead of Ajax.
  //
  // Turn on `Backbone.emulateHTTP` in order to send `PUT` and `DELETE` requests
  // as `POST`, with a `_method` parameter containing the true HTTP method,
  // as well as all requests with the body as `application/x-www-form-urlencoded`
  // instead of `application/json` with the model in a param named `model`.
  // Useful when interfacing with server-side languages like **PHP** that make
  // it difficult to read the body of `PUT` requests.
  Backbone.sync = function(method, model, options) {
    var type = methodMap[method];

    // Default options, unless specified.
    _.defaults(options || (options = {}), {
      emulateHTTP: Backbone.emulateHTTP,
      emulateJSON: Backbone.emulateJSON
    });

    // Default JSON-request options.
    var params = {type: type, dataType: 'json'};

    // Ensure that we have a URL.
    if (!options.url) {
      params.url = _.result(model, 'url') || urlError();
    }

    // Ensure that we have the appropriate request data.
    if (options.data == null && model && (method === 'create' || method === 'update' || method === 'patch')) {
      params.contentType = 'application/json';
      params.data = JSON.stringify(options.attrs || model.toJSON(options));
    }

    // For older servers, emulate JSON by encoding the request into an HTML-form.
    if (options.emulateJSON) {
      params.contentType = 'application/x-www-form-urlencoded';
      params.data = params.data ? {model: params.data} : {};
    }

    // For older servers, emulate HTTP by mimicking the HTTP method with `_method`
    // And an `X-HTTP-Method-Override` header.
    if (options.emulateHTTP && (type === 'PUT' || type === 'DELETE' || type === 'PATCH')) {
      params.type = 'POST';
      if (options.emulateJSON) params.data._method = type;
      var beforeSend = options.beforeSend;
      options.beforeSend = function(xhr) {
        xhr.setRequestHeader('X-HTTP-Method-Override', type);
        if (beforeSend) return beforeSend.apply(this, arguments);
      };
    }

    // Don't process data on a non-GET request.
    if (params.type !== 'GET' && !options.emulateJSON) {
      params.processData = false;
    }

    // If we're sending a `PATCH` request, and we're in an old Internet Explorer
    // that still has ActiveX enabled by default, override jQuery to use that
    // for XHR instead. Remove this line when jQuery supports `PATCH` on IE8.
    if (params.type === 'PATCH' && noXhrPatch) {
      params.xhr = function() {
        return new ActiveXObject("Microsoft.XMLHTTP");
      };
    }

    // Make the request, allowing the user to override any Ajax options.
    var xhr = options.xhr = Backbone.ajax(_.extend(params, options));
    model.trigger('request', model, xhr, options);
    return xhr;
  };

  var noXhrPatch =
    typeof window !== 'undefined' && !!window.ActiveXObject &&
      !(window.XMLHttpRequest && (new XMLHttpRequest).dispatchEvent);

  // Map from CRUD to HTTP for our default `Backbone.sync` implementation.
  var methodMap = {
    'create': 'POST',
    'update': 'PUT',
    'patch':  'PATCH',
    'delete': 'DELETE',
    'read':   'GET'
  };

  // Set the default implementation of `Backbone.ajax` to proxy through to `$`.
  // Override this if you'd like to use a different library.
  Backbone.ajax = function() {
    return Backbone.$.ajax.apply(Backbone.$, arguments);
  };

  // Backbone.Router
  // ---------------

  // Routers map faux-URLs to actions, and fire events when routes are
  // matched. Creating a new one sets its `routes` hash, if not set statically.
  var Router = Backbone.Router = function(options) {
    options || (options = {});
    if (options.routes) this.routes = options.routes;
    this._bindRoutes();
    this.initialize.apply(this, arguments);
  };

  // Cached regular expressions for matching named param parts and splatted
  // parts of route strings.
  var optionalParam = /\((.*?)\)/g;
  var namedParam    = /(\(\?)?:\w+/g;
  var splatParam    = /\*\w+/g;
  var escapeRegExp  = /[\-{}\[\]+?.,\\\^$|#\s]/g;

  // Set up all inheritable **Backbone.Router** properties and methods.
  _.extend(Router.prototype, Events, {

    // Initialize is an empty function by default. Override it with your own
    // initialization logic.
    initialize: function(){},

    // Manually bind a single named route to a callback. For example:
    //
    //     this.route('search/:query/p:num', 'search', function(query, num) {
    //       ...
    //     });
    //
    route: function(route, name, callback) {
      if (!_.isRegExp(route)) route = this._routeToRegExp(route);
      if (_.isFunction(name)) {
        callback = name;
        name = '';
      }
      if (!callback) callback = this[name];
      var router = this;
      Backbone.history.route(route, function(fragment) {
        var args = router._extractParameters(route, fragment);
        router.execute(callback, args);
        router.trigger.apply(router, ['route:' + name].concat(args));
        router.trigger('route', name, args);
        Backbone.history.trigger('route', router, name, args);
      });
      return this;
    },

    // Execute a route handler with the provided parameters.  This is an
    // excellent place to do pre-route setup or post-route cleanup.
    execute: function(callback, args) {
      if (callback) callback.apply(this, args);
    },

    // Simple proxy to `Backbone.history` to save a fragment into the history.
    navigate: function(fragment, options) {
      Backbone.history.navigate(fragment, options);
      return this;
    },

    // Bind all defined routes to `Backbone.history`. We have to reverse the
    // order of the routes here to support behavior where the most general
    // routes can be defined at the bottom of the route map.
    _bindRoutes: function() {
      if (!this.routes) return;
      this.routes = _.result(this, 'routes');
      var route, routes = _.keys(this.routes);
      while ((route = routes.pop()) != null) {
        this.route(route, this.routes[route]);
      }
    },

    // Convert a route string into a regular expression, suitable for matching
    // against the current location hash.
    _routeToRegExp: function(route) {
      route = route.replace(escapeRegExp, '\\$&')
                   .replace(optionalParam, '(?:$1)?')
                   .replace(namedParam, function(match, optional) {
                     return optional ? match : '([^/?]+)';
                   })
                   .replace(splatParam, '([^?]*?)');
      return new RegExp('^' + route + '(?:\\?([\\s\\S]*))?$');
    },

    // Given a route, and a URL fragment that it matches, return the array of
    // extracted decoded parameters. Empty or unmatched parameters will be
    // treated as `null` to normalize cross-browser behavior.
    _extractParameters: function(route, fragment) {
      var params = route.exec(fragment).slice(1);
      return _.map(params, function(param, i) {
        // Don't decode the search params.
        if (i === params.length - 1) return param || null;
        return param ? decodeURIComponent(param) : null;
      });
    }

  });

  // Backbone.History
  // ----------------

  // Handles cross-browser history management, based on either
  // [pushState](http://diveintohtml5.info/history.html) and real URLs, or
  // [onhashchange](https://developer.mozilla.org/en-US/docs/DOM/window.onhashchange)
  // and URL fragments. If the browser supports neither (old IE, natch),
  // falls back to polling.
  var History = Backbone.History = function() {
    this.handlers = [];
    _.bindAll(this, 'checkUrl');

    // Ensure that `History` can be used outside of the browser.
    if (typeof window !== 'undefined') {
      this.location = window.location;
      this.history = window.history;
    }
  };

  // Cached regex for stripping a leading hash/slash and trailing space.
  var routeStripper = /^[#\/]|\s+$/g;

  // Cached regex for stripping leading and trailing slashes.
  var rootStripper = /^\/+|\/+$/g;

  // Cached regex for detecting MSIE.
  var isExplorer = /msie [\w.]+/;

  // Cached regex for removing a trailing slash.
  var trailingSlash = /\/$/;

  // Cached regex for stripping urls of hash.
  var pathStripper = /#.*$/;

  // Has the history handling already been started?
  History.started = false;

  // Set up all inheritable **Backbone.History** properties and methods.
  _.extend(History.prototype, Events, {

    // The default interval to poll for hash changes, if necessary, is
    // twenty times a second.
    interval: 50,

    // Are we at the app root?
    atRoot: function() {
      return this.location.pathname.replace(/[^\/]$/, '$&/') === this.root;
    },

    // Gets the true hash value. Cannot use location.hash directly due to bug
    // in Firefox where location.hash will always be decoded.
    getHash: function(window) {
      var match = (window || this).location.href.match(/#(.*)$/);
      return match ? match[1] : '';
    },

    // Get the cross-browser normalized URL fragment, either from the URL,
    // the hash, or the override.
    getFragment: function(fragment, forcePushState) {
      if (fragment == null) {
        if (this._hasPushState || !this._wantsHashChange || forcePushState) {
          fragment = decodeURI(this.location.pathname + this.location.search);
          var root = this.root.replace(trailingSlash, '');
          if (!fragment.indexOf(root)) fragment = fragment.slice(root.length);
        } else {
          fragment = this.getHash();
        }
      }
      return fragment.replace(routeStripper, '');
    },

    // Start the hash change handling, returning `true` if the current URL matches
    // an existing route, and `false` otherwise.
    start: function(options) {
      if (History.started) throw new Error("Backbone.history has already been started");
      History.started = true;

      // Figure out the initial configuration. Do we need an iframe?
      // Is pushState desired ... is it available?
      this.options          = _.extend({root: '/'}, this.options, options);
      this.root             = this.options.root;
      this._wantsHashChange = this.options.hashChange !== false;
      this._wantsPushState  = !!this.options.pushState;
      this._hasPushState    = !!(this.options.pushState && this.history && this.history.pushState);
      var fragment          = this.getFragment();
      var docMode           = document.documentMode;
      var oldIE             = (isExplorer.exec(navigator.userAgent.toLowerCase()) && (!docMode || docMode <= 7));

      // Normalize root to always include a leading and trailing slash.
      this.root = ('/' + this.root + '/').replace(rootStripper, '/');

      if (oldIE && this._wantsHashChange) {
        var frame = Backbone.$('<iframe src="javascript:0" tabindex="-1">');
        this.iframe = frame.hide().appendTo('body')[0].contentWindow;
        this.navigate(fragment);
      }

      // Depending on whether we're using pushState or hashes, and whether
      // 'onhashchange' is supported, determine how we check the URL state.
      if (this._hasPushState) {
        Backbone.$(window).on('popstate', this.checkUrl);
      } else if (this._wantsHashChange && ('onhashchange' in window) && !oldIE) {
        Backbone.$(window).on('hashchange', this.checkUrl);
      } else if (this._wantsHashChange) {
        this._checkUrlInterval = setInterval(this.checkUrl, this.interval);
      }

      // Determine if we need to change the base url, for a pushState link
      // opened by a non-pushState browser.
      this.fragment = fragment;
      var loc = this.location;

      // Transition from hashChange to pushState or vice versa if both are
      // requested.
      if (this._wantsHashChange && this._wantsPushState) {

        // If we've started off with a route from a `pushState`-enabled
        // browser, but we're currently in a browser that doesn't support it...
        if (!this._hasPushState && !this.atRoot()) {
          this.fragment = this.getFragment(null, true);
          this.location.replace(this.root + '#' + this.fragment);
          // Return immediately as browser will do redirect to new url
          return true;

        // Or if we've started out with a hash-based route, but we're currently
        // in a browser where it could be `pushState`-based instead...
        } else if (this._hasPushState && this.atRoot() && loc.hash) {
          this.fragment = this.getHash().replace(routeStripper, '');
          this.history.replaceState({}, document.title, this.root + this.fragment);
        }

      }

      if (!this.options.silent) return this.loadUrl();
    },

    // Disable Backbone.history, perhaps temporarily. Not useful in a real app,
    // but possibly useful for unit testing Routers.
    stop: function() {
      Backbone.$(window).off('popstate', this.checkUrl).off('hashchange', this.checkUrl);
      if (this._checkUrlInterval) clearInterval(this._checkUrlInterval);
      History.started = false;
    },

    // Add a route to be tested when the fragment changes. Routes added later
    // may override previous routes.
    route: function(route, callback) {
      this.handlers.unshift({route: route, callback: callback});
    },

    // Checks the current URL to see if it has changed, and if it has,
    // calls `loadUrl`, normalizing across the hidden iframe.
    checkUrl: function(e) {
      var current = this.getFragment();
      if (current === this.fragment && this.iframe) {
        current = this.getFragment(this.getHash(this.iframe));
      }
      if (current === this.fragment) return false;
      if (this.iframe) this.navigate(current);
      this.loadUrl();
    },

    // Attempt to load the current URL fragment. If a route succeeds with a
    // match, returns `true`. If no defined routes matches the fragment,
    // returns `false`.
    loadUrl: function(fragment) {
      fragment = this.fragment = this.getFragment(fragment);
      return _.any(this.handlers, function(handler) {
        if (handler.route.test(fragment)) {
          handler.callback(fragment);
          return true;
        }
      });
    },

    // Save a fragment into the hash history, or replace the URL state if the
    // 'replace' option is passed. You are responsible for properly URL-encoding
    // the fragment in advance.
    //
    // The options object can contain `trigger: true` if you wish to have the
    // route callback be fired (not usually desirable), or `replace: true`, if
    // you wish to modify the current URL without adding an entry to the history.
    navigate: function(fragment, options) {
      if (!History.started) return false;
      if (!options || options === true) options = {trigger: !!options};

      var url = this.root + (fragment = this.getFragment(fragment || ''));

      // Strip the hash for matching.
      fragment = fragment.replace(pathStripper, '');

      if (this.fragment === fragment) return;
      this.fragment = fragment;

      // Don't include a trailing slash on the root.
      if (fragment === '' && url !== '/') url = url.slice(0, -1);

      // If pushState is available, we use it to set the fragment as a real URL.
      if (this._hasPushState) {
        this.history[options.replace ? 'replaceState' : 'pushState']({}, document.title, url);

      // If hash changes haven't been explicitly disabled, update the hash
      // fragment to store history.
      } else if (this._wantsHashChange) {
        this._updateHash(this.location, fragment, options.replace);
        if (this.iframe && (fragment !== this.getFragment(this.getHash(this.iframe)))) {
          // Opening and closing the iframe tricks IE7 and earlier to push a
          // history entry on hash-tag change.  When replace is true, we don't
          // want this.
          if(!options.replace) this.iframe.document.open().close();
          this._updateHash(this.iframe.location, fragment, options.replace);
        }

      // If you've told us that you explicitly don't want fallback hashchange-
      // based history, then `navigate` becomes a page refresh.
      } else {
        return this.location.assign(url);
      }
      if (options.trigger) return this.loadUrl(fragment);
    },

    // Update the hash location, either replacing the current entry, or adding
    // a new one to the browser history.
    _updateHash: function(location, fragment, replace) {
      if (replace) {
        var href = location.href.replace(/(javascript:|#).*$/, '');
        location.replace(href + '#' + fragment);
      } else {
        // Some browsers require that `hash` contains a leading #.
        location.hash = '#' + fragment;
      }
    }

  });

  // Create the default Backbone.history.
  Backbone.history = new History;

  // Helpers
  // -------

  // Helper function to correctly set up the prototype chain, for subclasses.
  // Similar to `goog.inherits`, but uses a hash of prototype properties and
  // class properties to be extended.
  var extend = function(protoProps, staticProps) {
    var parent = this;
    var child;

    // The constructor function for the new subclass is either defined by you
    // (the "constructor" property in your `extend` definition), or defaulted
    // by us to simply call the parent's constructor.
    if (protoProps && _.has(protoProps, 'constructor')) {
      child = protoProps.constructor;
    } else {
      child = function(){ return parent.apply(this, arguments); };
    }

    // Add static properties to the constructor function, if supplied.
    _.extend(child, parent, staticProps);

    // Set the prototype chain to inherit from `parent`, without calling
    // `parent`'s constructor function.
    var Surrogate = function(){ this.constructor = child; };
    Surrogate.prototype = parent.prototype;
    child.prototype = new Surrogate;

    // Add prototype properties (instance properties) to the subclass,
    // if supplied.
    if (protoProps) _.extend(child.prototype, protoProps);

    // Set a convenience property in case the parent's prototype is needed
    // later.
    child.__super__ = parent.prototype;

    return child;
  };

  // Set up inheritance for the model, collection, router, view and history.
  Model.extend = Collection.extend = Router.extend = View.extend = History.extend = extend;

  // Throw an error when a URL is needed, and none is supplied.
  var urlError = function() {
    throw new Error('A "url" property or function must be specified');
  };

  // Wrap an optional error callback with a fallback error event.
  var wrapError = function(model, options) {
    var error = options.error;
    options.error = function(resp) {
      if (error) error(model, resp, options);
      model.trigger('error', model, resp, options);
    };
  };

  return Backbone;

}));
/*!
 * SALT JavaScript Library
 * =======================
*/
define('salt/salt',[
    'underscore',
    'backbone'
], function (_, Backbone) {
    /**
     * Create an object that inherits from backbone events to serve as mediator for the application.
     */
    var SALT = _.extend({}, Backbone.Events);

    /**
     * Provide pub/sub vocabulary.
     */
    SALT.subscribe = SALT.sub = SALT.on;
    SALT.publish = SALT.pub = SALT.trigger;

    return SALT;
});

define('salt', ['salt/salt'], function (main) { return main; });

//
// Dust - Asynchronous Templating v2.0.3
// http://akdubya.github.com/dustjs
//
// Copyright (c) 2010, Aleksander Williams
// Released under the MIT License.
//

var dust = {};

function getGlobal(){
  return (function(){
    return this.dust;
  }).call(null);
}

(function(dust) {

dust.helpers = {};

dust.cache = {};

dust.register = function(name, tmpl) {
  if (!name) return;
  dust.cache[name] = tmpl;
};

dust.render = function(name, context, callback) {
  var chunk = new Stub(callback).head;
  dust.load(name, chunk, Context.wrap(context, name)).end();
};

dust.stream = function(name, context) {
  var stream = new Stream();
  dust.nextTick(function() {
    dust.load(name, stream.head, Context.wrap(context, name)).end();
  });
  return stream;
};

dust.renderSource = function(source, context, callback) {
  return dust.compileFn(source)(context, callback);
};

dust.compileFn = function(source, name) {
  var tmpl = dust.loadSource(dust.compile(source, name));
  return function(context, callback) {
    var master = callback ? new Stub(callback) : new Stream();
    dust.nextTick(function() {
      tmpl(master.head, Context.wrap(context, name)).end();
    });
    return master;
  };
};

dust.load = function(name, chunk, context) {
  var tmpl = dust.cache[name];
  if (tmpl) {
    return tmpl(chunk, context);
  } else {
    if (dust.onLoad) {
      return chunk.map(function(chunk) {
        dust.onLoad(name, function(err, src) {
          if (err) return chunk.setError(err);
          if (!dust.cache[name]) dust.loadSource(dust.compile(src, name));
          dust.cache[name](chunk, context).end();
        });
      });
    }
    return chunk.setError(new Error("Template Not Found: " + name));
  }
};

dust.loadSource = function(source, path) {
  return eval(source);
};

if (Array.isArray) {
  dust.isArray = Array.isArray;
} else {
  dust.isArray = function(arr) {
    return Object.prototype.toString.call(arr) == "[object Array]";
  };
}

dust.nextTick = (function() {
  if (typeof process !== "undefined") {
    return process.nextTick;
  } else {
    return function(callback) {
      setTimeout(callback,0);
    };
  }
} )();

dust.isEmpty = function(value) {
  if (dust.isArray(value) && !value.length) return true;
  if (value === 0) return false;
  return (!value);
};

// apply the filter chain and return the output string
dust.filter = function(string, auto, filters) {
  if (filters) {
    for (var i=0, len=filters.length; i<len; i++) {
      var name = filters[i];
      if (name === "s") {
        auto = null;
      }
      // fail silently for invalid filters
      else if (typeof dust.filters[name] === 'function') {
        string = dust.filters[name](string);
      }
    }
  }
  // by default always apply the h filter, unless asked to unescape with |s
  if (auto) {
    string = dust.filters[auto](string);
  }
  return string;
};

dust.filters = {
  h: function(value) { return dust.escapeHtml(value); },
  j: function(value) { return dust.escapeJs(value); },
  u: encodeURI,
  uc: encodeURIComponent,
  js: function(value) { if (!JSON) { return value; } return JSON.stringify(value); },
  jp: function(value) { if (!JSON) { return value; } return JSON.parse(value); }
};

function Context(stack, global, blocks, templateName) {
  this.stack  = stack;
  this.global = global;
  this.blocks = blocks;
  this.templateName = templateName;
}

dust.makeBase = function(global) {
  return new Context(new Stack(), global);
};

Context.wrap = function(context, name) {
  if (context instanceof Context) {
    return context;
  }
  return new Context(new Stack(context), {}, null, name);
};

Context.prototype.get = function(key) {
  var ctx = this.stack, value;

  while(ctx) {
    if (ctx.isObject) {
      value = ctx.head[key];
      if (!(value === undefined)) {
        return value;
      }
    }
    ctx = ctx.tail;
  }
  return this.global ? this.global[key] : undefined;
};

//supports dot path resolution, function wrapped apply, and searching global paths
Context.prototype.getPath = function(cur, down) {
  var ctx = this.stack, ctxThis,
      len = down.length,      
      tail = cur ? undefined : this.stack.tail; 

  if (cur && len === 0) return ctx.head;
  ctx = ctx.head;
  var i = 0;
  while(ctx && i < len) {
    ctxThis = ctx;
    ctx = ctx[down[i]];
    i++;
    while (!ctx && !cur){
  // i is the count of number of path elements matched. If > 1 then we have a partial match
  // and do not continue to search for the rest of the path.
  // Note: a falsey value at the end of a matched path also comes here.
  // This returns the value or undefined if we just have a partial match.
      if (i > 1) return ctx;
      if (tail){
        ctx = tail.head;
        tail = tail.tail;
        i=0;
      } else if (!cur) {
        //finally search this.global.  we set cur to true to halt after
          ctx = this.global;
          cur = true;
        i=0;
      }
    }   
  }
  if (typeof ctx == 'function'){
    //wrap to preserve context 'this' see #174
    return function(){ 
      return ctx.apply(ctxThis,arguments); 
    };
  }
  else {
    return ctx;
  }
};

Context.prototype.push = function(head, idx, len) {
  return new Context(new Stack(head, this.stack, idx, len), this.global, this.blocks, this.templateName);
};

Context.prototype.rebase = function(head) {
  return new Context(new Stack(head), this.global, this.blocks, this.templateName);
};

Context.prototype.current = function() {
  return this.stack.head;
};

Context.prototype.getBlock = function(key, chk, ctx) {
  if (typeof key === "function") {
    key = key(chk, ctx).data.join("");
    chk.data = []; //ie7 perf
  }

  var blocks = this.blocks;

  if (!blocks) return;
  var len = blocks.length, fn;
  while (len--) {
    fn = blocks[len][key];
    if (fn) return fn;
  }
};

Context.prototype.shiftBlocks = function(locals) {
  var blocks = this.blocks,
      newBlocks;

  if (locals) {
    if (!blocks) {
      newBlocks = [locals];
    } else {
      newBlocks = blocks.concat([locals]);
    }
    return new Context(this.stack, this.global, newBlocks, this.templateName);
  }
  return this;
};

function Stack(head, tail, idx, len) {
  this.tail = tail;
  this.isObject = !dust.isArray(head) && head && typeof head === "object";
  this.head = head;
  this.index = idx;
  this.of = len;
}

function Stub(callback) {
  this.head = new Chunk(this);
  this.callback = callback;
  this.out = '';
}

Stub.prototype.flush = function() {
  var chunk = this.head;

  while (chunk) {
    if (chunk.flushable) {
      this.out += chunk.data.join(""); //ie7 perf
    } else if (chunk.error) {
      this.callback(chunk.error);
      this.flush = function() {};
      return;
    } else {
      return;
    }
    chunk = chunk.next;
    this.head = chunk;
  }
  this.callback(null, this.out);
};

function Stream() {
  this.head = new Chunk(this);
}

Stream.prototype.flush = function() {
  var chunk = this.head;

  while(chunk) {
    if (chunk.flushable) {
      this.emit('data', chunk.data.join("")); //ie7 perf
    } else if (chunk.error) {
      this.emit('error', chunk.error);
      this.flush = function() {};
      return;
    } else {
      return;
    }
    chunk = chunk.next;
    this.head = chunk;
  }
  this.emit('end');
};

Stream.prototype.emit = function(type, data) {
  if (!this.events) return false;
  var handler = this.events[type];
  if (!handler) return false;
  if (typeof handler == 'function') {
    handler(data);
  } else {
    var listeners = handler.slice(0);
    for (var i = 0, l = listeners.length; i < l; i++) {
      listeners[i](data);
    }
  }
};

Stream.prototype.on = function(type, callback) {
  if (!this.events) {
    this.events = {};
  }
  if (!this.events[type]) {
    this.events[type] = callback;
  } else if(typeof this.events[type] === 'function') {
    this.events[type] = [this.events[type], callback];
  } else {
    this.events[type].push(callback);
  }
  return this;
};

Stream.prototype.pipe = function(stream) {
  this.on("data", function(data) {
    stream.write(data, "utf8");
  }).on("end", function() {
    stream.end();
  }).on("error", function(err) {
    stream.error(err);
  });
  return this;
};

function Chunk(root, next, taps) {
  this.root = root;
  this.next = next;
  this.data = []; //ie7 perf
  this.flushable = false;
  this.taps = taps;
}

Chunk.prototype.write = function(data) {
  var taps  = this.taps;

  if (taps) {
    data = taps.go(data);
  }
  this.data.push(data);
  return this;
};

Chunk.prototype.end = function(data) {
  if (data) {
    this.write(data);
  }
  this.flushable = true;
  this.root.flush();
  return this;
};

Chunk.prototype.map = function(callback) {
  var cursor = new Chunk(this.root, this.next, this.taps),
      branch = new Chunk(this.root, cursor, this.taps);

  this.next = branch;
  this.flushable = true;
  callback(branch);
  return cursor;
};

Chunk.prototype.tap = function(tap) {
  var taps = this.taps;

  if (taps) {
    this.taps = taps.push(tap);
  } else {
    this.taps = new Tap(tap);
  }
  return this;
};

Chunk.prototype.untap = function() {
  this.taps = this.taps.tail;
  return this;
};

Chunk.prototype.render = function(body, context) {
  return body(this, context);
};

Chunk.prototype.reference = function(elem, context, auto, filters) {
  if (typeof elem === "function") {
    elem.isFunction = true;
    // Changed the function calling to use apply with the current context to make sure 
    // that "this" is wat we expect it to be inside the function
    elem = elem.apply(context.current(), [this, context, null, {auto: auto, filters: filters}]);
    if (elem instanceof Chunk) {
      return elem;
    }
  }
  if (!dust.isEmpty(elem)) {
    return this.write(dust.filter(elem, auto, filters));
  } else {
    return this;
  }
};

Chunk.prototype.section = function(elem, context, bodies, params) {
  // anonymous functions
  if (typeof elem === "function") {
    elem = elem.apply(context.current(), [this, context, bodies, params]);
    // functions that return chunks are assumed to have handled the body and/or have modified the chunk
    // use that return value as the current chunk and go to the next method in the chain
    if (elem instanceof Chunk) {
      return elem;
    }
  }
  var body = bodies.block,
      skip = bodies['else'];

  // a.k.a Inline parameters in the Dust documentations
  if (params) {
    context = context.push(params);
  }

  /*
  Dust's default behavior is to enumerate over the array elem, passing each object in the array to the block.
  When elem resolves to a value or object instead of an array, Dust sets the current context to the value 
  and renders the block one time.
  */
  //non empty array is truthy, empty array is falsy
  if (dust.isArray(elem)) {
     if (body) {
      var len = elem.length, chunk = this;
      if (len > 0) {
        // any custom helper can blow up the stack 
        // and store a flattened context, guard defensively
        if(context.stack.head) {
         context.stack.head['$len'] = len;
        }
        for (var i=0; i<len; i++) {
          if(context.stack.head) {
           context.stack.head['$idx'] = i;
          }
          chunk = body(chunk, context.push(elem[i], i, len));
        }
        if(context.stack.head) {
         context.stack.head['$idx'] = undefined;
         context.stack.head['$len'] = undefined;
        }
        return chunk;
      } 
      else if (skip) {
         return skip(this, context);
      }
     }
   }
   // true is truthy but does not change context
   else if (elem  === true) {
     if (body) { 
        return body(this, context);
     }
   }
   // everything that evaluates to true are truthy ( e.g. Non-empty strings and Empty objects are truthy. )
   // zero is truthy
   // for anonymous functions that did not returns a chunk, truthiness is evaluated based on the return value
   //
   else if (elem || elem === 0) {
     if (body) return body(this, context.push(elem));
   // nonexistent, scalar false value, scalar empty string, null,
   // undefined are all falsy
  } else if (skip) {
     return skip(this, context);
   }  
  return this;
};

Chunk.prototype.exists = function(elem, context, bodies) {
  var body = bodies.block,
      skip = bodies['else'];

  if (!dust.isEmpty(elem)) {
    if (body) return body(this, context);
  } else if (skip) {
    return skip(this, context);
  }
  return this;
};

Chunk.prototype.notexists = function(elem, context, bodies) {
  var body = bodies.block,
      skip = bodies['else'];

  if (dust.isEmpty(elem)) {
    if (body) return body(this, context);
  } else if (skip) {
    return skip(this, context);
  }
  return this;
};

Chunk.prototype.block = function(elem, context, bodies) {
  var body = bodies.block;

  if (elem) {
    body = elem;
  }

  if (body) {
    return body(this, context);
  }
  return this;
};

Chunk.prototype.partial = function(elem, context, params) {
  var partialContext;
  //put the params context second to match what section does. {.} matches the current context without parameters
  // start with an empty context
  partialContext = dust.makeBase(context.global);
  partialContext.blocks = context.blocks;
  if (context.stack && context.stack.tail){
    // grab the stack(tail) off of the previous context if we have it
    partialContext.stack = context.stack.tail;
  }
  if (params){
    //put params on
    partialContext = partialContext.push(params);
  }

  if(typeof elem === "string") {
    partialContext.templateName = elem;
  }

  //reattach the head
  partialContext = partialContext.push(context.stack.head);

  var partialChunk;
   if (typeof elem === "function") {
     partialChunk = this.capture(elem, partialContext, function(name, chunk) {
       dust.load(name, chunk, partialContext).end();
     });
   }
   else {
     partialChunk = dust.load(elem, this, partialContext);
   }
   return partialChunk;
};

Chunk.prototype.helper = function(name, context, bodies, params) {
  // handle invalid helpers, similar to invalid filters
  if( dust.helpers[name]){
   return dust.helpers[name](this, context, bodies, params);
  } else {
    return this;
  }
};

Chunk.prototype.capture = function(body, context, callback) {
  return this.map(function(chunk) {
    var stub = new Stub(function(err, out) {
      if (err) {
        chunk.setError(err);
      } else {
        callback(out, chunk);
      }
    });
    body(stub.head, context).end();
  });
};

Chunk.prototype.setError = function(err) {
  this.error = err;
  this.root.flush();
  return this;
};

function Tap(head, tail) {
  this.head = head;
  this.tail = tail;
}

Tap.prototype.push = function(tap) {
  return new Tap(tap, this);
};

Tap.prototype.go = function(value) {
  var tap = this;

  while(tap) {
    value = tap.head(value);
    tap = tap.tail;
  }
  return value;
};

var HCHARS = new RegExp(/[&<>\"\']/),
    AMP    = /&/g,
    LT     = /</g,
    GT     = />/g,
    QUOT   = /\"/g,
    SQUOT  = /\'/g;

dust.escapeHtml = function(s) {
  if (typeof s === "string") {
    if (!HCHARS.test(s)) {
      return s;
    }
    return s.replace(AMP,'&amp;').replace(LT,'&lt;').replace(GT,'&gt;').replace(QUOT,'&quot;').replace(SQUOT, '&#39;');
  }
  return s;
};

var BS = /\\/g,
    FS = /\//g,
    CR = /\r/g,
    LS = /\u2028/g,
    PS = /\u2029/g,
    NL = /\n/g,
    LF = /\f/g,
    SQ = /'/g,
    DQ = /"/g,
    TB = /\t/g;

dust.escapeJs = function(s) {
  if (typeof s === "string") {
    return s
      .replace(BS, '\\\\')
      .replace(FS, '\\/')
      .replace(DQ, '\\"')
      .replace(SQ, "\\'")
      .replace(CR, '\\r')
      .replace(LS, '\\u2028')
      .replace(PS, '\\u2029')
      .replace(NL, '\\n')
      .replace(LF, '\\f')
      .replace(TB, "\\t");
  }
  return s;
};

})(dust);

if (typeof exports !== "undefined") {
  if (typeof process !== "undefined") {
      require('./server')(dust);
  }
  module.exports = dust;
}
var dustCompiler = (function(dust) {

dust.compile = function(source, name) {
  try {
    var ast = filterAST(dust.parse(source));
    return compile(ast, name);
  }
  catch(err)
  {
    if(!err.line || !err.column) throw err;    
    throw new SyntaxError(err.message + " At line : " + err.line + ", column : " + err.column);
  }
};

function filterAST(ast) {
  var context = {};
  return dust.filterNode(context, ast);
};

dust.filterNode = function(context, node) {
  return dust.optimizers[node[0]](context, node);
};

dust.optimizers = {
  body:      compactBuffers,
  buffer:    noop,
  special:   convertSpecial,
  format:    nullify,        // TODO: convert format
  reference: visit,
  "#":       visit,
  "?":       visit,
  "^":       visit,
  "<":       visit,
  "+":       visit,
  "@":       visit,
  "%":       visit,
  partial:   visit,
  context:   visit,
  params:    visit,
  bodies:    visit,
  param:     visit,
  filters:   noop,
  key:       noop,
  path:      noop,
  literal:   noop,
  comment:   nullify,
  line: nullify,
  col: nullify
};

dust.pragmas = {
  esc: function(compiler, context, bodies, params) {
    var old = compiler.auto;
    if (!context) context = 'h';
    compiler.auto = (context === 's') ? '' : context;
    var out = compileParts(compiler, bodies.block);
    compiler.auto = old;
    return out;
  }
};

function visit(context, node) {
  var out = [node[0]];
  for (var i=1, len=node.length; i<len; i++) {
    var res = dust.filterNode(context, node[i]);
    if (res) out.push(res);
  }
  return out;
};

// Compacts consecutive buffer nodes into a single node
function compactBuffers(context, node) {
  var out = [node[0]], memo;
  for (var i=1, len=node.length; i<len; i++) {
    var res = dust.filterNode(context, node[i]);
    if (res) {
      if (res[0] === 'buffer') {
        if (memo) {
          memo[1] += res[1];
        } else {
          memo = res;
          out.push(res);
        }
      } else {
        memo = null;
        out.push(res);
      }
    }
  }
  return out;
};

var specialChars = {
  "s": " ",
  "n": "\n",
  "r": "\r",
  "lb": "{",
  "rb": "}"
};

function convertSpecial(context, node) { return ['buffer', specialChars[node[1]]] };
function noop(context, node) { return node };
function nullify(){};

function compile(ast, name) {
  var context = {
    name: name,
    bodies: [],
    blocks: {},
    index: 0,
    auto: "h"
  }

  return "(function(){dust.register("
    + (name ? "\"" + name + "\"" : "null") + ","
    + dust.compileNode(context, ast)
    + ");"
    + compileBlocks(context)
    + compileBodies(context)
    + "return body_0;"
    + "})();";
};

function compileBlocks(context) {
  var out = [],
      blocks = context.blocks;

  for (var name in blocks) {
    out.push("'" + name + "':" + blocks[name]);
  }
  if (out.length) {
    context.blocks = "ctx=ctx.shiftBlocks(blocks);";
    return "var blocks={" + out.join(',') + "};";
  }
  return context.blocks = "";
};

function compileBodies(context) {
  var out = [],
      bodies = context.bodies,
      blx = context.blocks;

  for (var i=0, len=bodies.length; i<len; i++) {
    out[i] = "function body_" + i + "(chk,ctx){"
      + blx + "return chk" + bodies[i] + ";}";
  }
  return out.join('');
};

function compileParts(context, body) {
  var parts = '';
  for (var i=1, len=body.length; i<len; i++) {
    parts += dust.compileNode(context, body[i]);
  }
  return parts;
};

dust.compileNode = function(context, node) {
  return dust.nodes[node[0]](context, node);
};

dust.nodes = {
  body: function(context, node) {
    var id = context.index++, name = "body_" + id;
    context.bodies[id] = compileParts(context, node);
    return name;
  },

  buffer: function(context, node) {
    return ".write(" + escape(node[1]) + ")";
  },

  format: function(context, node) {
    return ".write(" + escape(node[1] + node[2]) + ")";
  },

  reference: function(context, node) {
    return ".reference(" + dust.compileNode(context, node[1])
      + ",ctx," + dust.compileNode(context, node[2]) + ")";
  },

  "#": function(context, node) {
    return compileSection(context, node, "section");
  },

  "?": function(context, node) {
    return compileSection(context, node, "exists");
  },

  "^": function(context, node) {
    return compileSection(context, node, "notexists");
  },

  "<": function(context, node) {
    var bodies = node[4];
    for (var i=1, len=bodies.length; i<len; i++) {
      var param = bodies[i],
          type = param[1][1];
      if (type === "block") {
        context.blocks[node[1].text] = dust.compileNode(context, param[2]);
        return '';
      }
    }
    return '';
  },

  "+": function(context, node) {
    if(typeof(node[1].text) === "undefined"  && typeof(node[4]) === "undefined"){
      return ".block(ctx.getBlock("
      + dust.compileNode(context, node[1])
      + ",chk, ctx)," + dust.compileNode(context, node[2]) + ", {},"
      + dust.compileNode(context, node[3])
      + ")";
    }else {
      return ".block(ctx.getBlock("
      + escape(node[1].text)
      + ")," + dust.compileNode(context, node[2]) + ","
      + dust.compileNode(context, node[4]) + ","
      + dust.compileNode(context, node[3])
      + ")";
    }
  },

  "@": function(context, node) {
    return ".helper("
      + escape(node[1].text)
      + "," + dust.compileNode(context, node[2]) + ","
      + dust.compileNode(context, node[4]) + ","
      + dust.compileNode(context, node[3])
      + ")";
  },

  "%": function(context, node) {
    // TODO: Move these hacks into pragma precompiler
    var name = node[1][1];
    if (!dust.pragmas[name]) return '';

    var rawBodies = node[4];
    var bodies = {};
    for (var i=1, len=rawBodies.length; i<len; i++) {
      var b = rawBodies[i];
      bodies[b[1][1]] = b[2];
    }

    var rawParams = node[3];
    var params = {};
    for (var i=1, len=rawParams.length; i<len; i++) {
      var p = rawParams[i];
      params[p[1][1]] = p[2][1];
    }

    var ctx = node[2][1] ? node[2][1].text : null;

    return dust.pragmas[name](context, ctx, bodies, params);
  },

  partial: function(context, node) {
    return ".partial("
      + dust.compileNode(context, node[1])
      + "," + dust.compileNode(context, node[2])
      + "," + dust.compileNode(context, node[3]) + ")";
  },

  context: function(context, node) {
    if (node[1]) {
      return "ctx.rebase(" + dust.compileNode(context, node[1]) + ")";
    }
    return "ctx";
  },

  params: function(context, node) {
    var out = [];
    for (var i=1, len=node.length; i<len; i++) {
      out.push(dust.compileNode(context, node[i]));
    }
    if (out.length) {
      return "{" + out.join(',') + "}";
    }
    return "null";
  },

  bodies: function(context, node) {
    var out = [];
    for (var i=1, len=node.length; i<len; i++) {
      out.push(dust.compileNode(context, node[i]));
    }
    return "{" + out.join(',') + "}";
  },

  param: function(context, node) {
    return dust.compileNode(context, node[1]) + ":" + dust.compileNode(context, node[2]);
  },

  filters: function(context, node) {
    var list = [];
    for (var i=1, len=node.length; i<len; i++) {
      var filter = node[i];
      list.push("\"" + filter + "\"");
    }
    return "\"" + context.auto + "\""
      + (list.length ? ",[" + list.join(',') + "]" : '');
  },

  key: function(context, node) {
    return "ctx.get(\"" + node[1] + "\")";
  },

  path: function(context, node) {
    var current = node[1],
        keys = node[2],
        list = [];

    for (var i=0,len=keys.length; i<len; i++) {
      if (dust.isArray(keys[i]))
        list.push(dust.compileNode(context, keys[i]));
      else
        list.push("\"" + keys[i] + "\"");
    }
    return "ctx.getPath(" + current + ",[" + list.join(',') + "])";
  },

  literal: function(context, node) {
    return escape(node[1]);
  }
};

function compileSection(context, node, cmd) {
  return "." + cmd + "("
    + dust.compileNode(context, node[1])
    + "," + dust.compileNode(context, node[2]) + ","
    + dust.compileNode(context, node[4]) + ","
    + dust.compileNode(context, node[3])
    + ")";
};

var escape = (typeof JSON === "undefined")
  ? function(str) { return "\"" + dust.escapeJs(str) + "\"" }
  : JSON.stringify;

  return dust;

});

if (typeof exports !== 'undefined') {
  module.exports = dustCompiler;
} else {
  dustCompiler(getGlobal());
}
(function(dust){

var parser = (function(){
  /*
   * Generated by PEG.js 0.7.0.
   *
   * http://pegjs.majda.cz/
   */
  
  function quote(s) {
    /*
     * ECMA-262, 5th ed., 7.8.4: All characters may appear literally in a
     * string literal except for the closing quote character, backslash,
     * carriage return, line separator, paragraph separator, and line feed.
     * Any character may appear in the form of an escape sequence.
     *
     * For portability, we also escape escape all control and non-ASCII
     * characters. Note that "\0" and "\v" escape sequences are not used
     * because JSHint does not like the first and IE the second.
     */
     return '"' + s
      .replace(/\\/g, '\\\\')  // backslash
      .replace(/"/g, '\\"')    // closing quote character
      .replace(/\x08/g, '\\b') // backspace
      .replace(/\t/g, '\\t')   // horizontal tab
      .replace(/\n/g, '\\n')   // line feed
      .replace(/\f/g, '\\f')   // form feed
      .replace(/\r/g, '\\r')   // carriage return
      .replace(/[\x00-\x07\x0B\x0E-\x1F\x80-\uFFFF]/g, escape)
      + '"';
  }
  
  var result = {
    /*
     * Parses the input with a generated parser. If the parsing is successfull,
     * returns a value explicitly or implicitly specified by the grammar from
     * which the parser was generated (see |PEG.buildParser|). If the parsing is
     * unsuccessful, throws |PEG.parser.SyntaxError| describing the error.
     */
    parse: function(input, startRule) {
      var parseFunctions = {
        "body": parse_body,
        "part": parse_part,
        "section": parse_section,
        "sec_tag_start": parse_sec_tag_start,
        "end_tag": parse_end_tag,
        "context": parse_context,
        "params": parse_params,
        "bodies": parse_bodies,
        "reference": parse_reference,
        "partial": parse_partial,
        "filters": parse_filters,
        "special": parse_special,
        "identifier": parse_identifier,
        "number": parse_number,
        "float": parse_float,
        "integer": parse_integer,
        "path": parse_path,
        "key": parse_key,
        "array": parse_array,
        "array_part": parse_array_part,
        "inline": parse_inline,
        "inline_part": parse_inline_part,
        "buffer": parse_buffer,
        "literal": parse_literal,
        "esc": parse_esc,
        "comment": parse_comment,
        "tag": parse_tag,
        "ld": parse_ld,
        "rd": parse_rd,
        "lb": parse_lb,
        "rb": parse_rb,
        "eol": parse_eol,
        "ws": parse_ws
      };
      
      if (startRule !== undefined) {
        if (parseFunctions[startRule] === undefined) {
          throw new Error("Invalid rule name: " + quote(startRule) + ".");
        }
      } else {
        startRule = "body";
      }
      
      var pos = { offset: 0, line: 1, column: 1, seenCR: false };
      var reportFailures = 0;
      var rightmostFailuresPos = { offset: 0, line: 1, column: 1, seenCR: false };
      var rightmostFailuresExpected = [];
      
      function padLeft(input, padding, length) {
        var result = input;
        
        var padLength = length - input.length;
        for (var i = 0; i < padLength; i++) {
          result = padding + result;
        }
        
        return result;
      }
      
      function escape(ch) {
        var charCode = ch.charCodeAt(0);
        var escapeChar;
        var length;
        
        if (charCode <= 0xFF) {
          escapeChar = 'x';
          length = 2;
        } else {
          escapeChar = 'u';
          length = 4;
        }
        
        return '\\' + escapeChar + padLeft(charCode.toString(16).toUpperCase(), '0', length);
      }
      
      function clone(object) {
        var result = {};
        for (var key in object) {
          result[key] = object[key];
        }
        return result;
      }
      
      function advance(pos, n) {
        var endOffset = pos.offset + n;
        
        for (var offset = pos.offset; offset < endOffset; offset++) {
          var ch = input.charAt(offset);
          if (ch === "\n") {
            if (!pos.seenCR) { pos.line++; }
            pos.column = 1;
            pos.seenCR = false;
          } else if (ch === "\r" || ch === "\u2028" || ch === "\u2029") {
            pos.line++;
            pos.column = 1;
            pos.seenCR = true;
          } else {
            pos.column++;
            pos.seenCR = false;
          }
        }
        
        pos.offset += n;
      }
      
      function matchFailed(failure) {
        if (pos.offset < rightmostFailuresPos.offset) {
          return;
        }
        
        if (pos.offset > rightmostFailuresPos.offset) {
          rightmostFailuresPos = clone(pos);
          rightmostFailuresExpected = [];
        }
        
        rightmostFailuresExpected.push(failure);
      }
      
      function parse_body() {
        var result0, result1;
        var pos0;
        
        pos0 = clone(pos);
        result0 = [];
        result1 = parse_part();
        while (result1 !== null) {
          result0.push(result1);
          result1 = parse_part();
        }
        if (result0 !== null) {
          result0 = (function(offset, line, column, p) { return ["body"].concat(p).concat([['line', line], ['col', column]]) })(pos0.offset, pos0.line, pos0.column, result0);
        }
        if (result0 === null) {
          pos = clone(pos0);
        }
        return result0;
      }
      
      function parse_part() {
        var result0;
        
        result0 = parse_comment();
        if (result0 === null) {
          result0 = parse_section();
          if (result0 === null) {
            result0 = parse_partial();
            if (result0 === null) {
              result0 = parse_special();
              if (result0 === null) {
                result0 = parse_reference();
                if (result0 === null) {
                  result0 = parse_buffer();
                }
              }
            }
          }
        }
        return result0;
      }
      
      function parse_section() {
        var result0, result1, result2, result3, result4, result5, result6;
        var pos0, pos1;
        
        reportFailures++;
        pos0 = clone(pos);
        pos1 = clone(pos);
        result0 = parse_sec_tag_start();
        if (result0 !== null) {
          result1 = [];
          result2 = parse_ws();
          while (result2 !== null) {
            result1.push(result2);
            result2 = parse_ws();
          }
          if (result1 !== null) {
            result2 = parse_rd();
            if (result2 !== null) {
              result3 = parse_body();
              if (result3 !== null) {
                result4 = parse_bodies();
                if (result4 !== null) {
                  result5 = parse_end_tag();
                  result5 = result5 !== null ? result5 : "";
                  if (result5 !== null) {
                    result6 = (function(offset, line, column, t, b, e, n) {if( (!n) || (t[1].text !== n.text) ) { throw new Error("Expected end tag for "+t[1].text+" but it was not found. At line : "+line+", column : " + column)} return true;})(pos.offset, pos.line, pos.column, result0, result3, result4, result5) ? "" : null;
                    if (result6 !== null) {
                      result0 = [result0, result1, result2, result3, result4, result5, result6];
                    } else {
                      result0 = null;
                      pos = clone(pos1);
                    }
                  } else {
                    result0 = null;
                    pos = clone(pos1);
                  }
                } else {
                  result0 = null;
                  pos = clone(pos1);
                }
              } else {
                result0 = null;
                pos = clone(pos1);
              }
            } else {
              result0 = null;
              pos = clone(pos1);
            }
          } else {
            result0 = null;
            pos = clone(pos1);
          }
        } else {
          result0 = null;
          pos = clone(pos1);
        }
        if (result0 !== null) {
          result0 = (function(offset, line, column, t, b, e, n) { e.push(["param", ["literal", "block"], b]); t.push(e); return t.concat([['line', line], ['col', column]]) })(pos0.offset, pos0.line, pos0.column, result0[0], result0[3], result0[4], result0[5]);
        }
        if (result0 === null) {
          pos = clone(pos0);
        }
        if (result0 === null) {
          pos0 = clone(pos);
          pos1 = clone(pos);
          result0 = parse_sec_tag_start();
          if (result0 !== null) {
            result1 = [];
            result2 = parse_ws();
            while (result2 !== null) {
              result1.push(result2);
              result2 = parse_ws();
            }
            if (result1 !== null) {
              if (input.charCodeAt(pos.offset) === 47) {
                result2 = "/";
                advance(pos, 1);
              } else {
                result2 = null;
                if (reportFailures === 0) {
                  matchFailed("\"/\"");
                }
              }
              if (result2 !== null) {
                result3 = parse_rd();
                if (result3 !== null) {
                  result0 = [result0, result1, result2, result3];
                } else {
                  result0 = null;
                  pos = clone(pos1);
                }
              } else {
                result0 = null;
                pos = clone(pos1);
              }
            } else {
              result0 = null;
              pos = clone(pos1);
            }
          } else {
            result0 = null;
            pos = clone(pos1);
          }
          if (result0 !== null) {
            result0 = (function(offset, line, column, t) { t.push(["bodies"]); return t.concat([['line', line], ['col', column]]) })(pos0.offset, pos0.line, pos0.column, result0[0]);
          }
          if (result0 === null) {
            pos = clone(pos0);
          }
        }
        reportFailures--;
        if (reportFailures === 0 && result0 === null) {
          matchFailed("section");
        }
        return result0;
      }
      
      function parse_sec_tag_start() {
        var result0, result1, result2, result3, result4, result5;
        var pos0, pos1;
        
        pos0 = clone(pos);
        pos1 = clone(pos);
        result0 = parse_ld();
        if (result0 !== null) {
          if (/^[#?^<+@%]/.test(input.charAt(pos.offset))) {
            result1 = input.charAt(pos.offset);
            advance(pos, 1);
          } else {
            result1 = null;
            if (reportFailures === 0) {
              matchFailed("[#?^<+@%]");
            }
          }
          if (result1 !== null) {
            result2 = [];
            result3 = parse_ws();
            while (result3 !== null) {
              result2.push(result3);
              result3 = parse_ws();
            }
            if (result2 !== null) {
              result3 = parse_identifier();
              if (result3 !== null) {
                result4 = parse_context();
                if (result4 !== null) {
                  result5 = parse_params();
                  if (result5 !== null) {
                    result0 = [result0, result1, result2, result3, result4, result5];
                  } else {
                    result0 = null;
                    pos = clone(pos1);
                  }
                } else {
                  result0 = null;
                  pos = clone(pos1);
                }
              } else {
                result0 = null;
                pos = clone(pos1);
              }
            } else {
              result0 = null;
              pos = clone(pos1);
            }
          } else {
            result0 = null;
            pos = clone(pos1);
          }
        } else {
          result0 = null;
          pos = clone(pos1);
        }
        if (result0 !== null) {
          result0 = (function(offset, line, column, t, n, c, p) { return [t, n, c, p] })(pos0.offset, pos0.line, pos0.column, result0[1], result0[3], result0[4], result0[5]);
        }
        if (result0 === null) {
          pos = clone(pos0);
        }
        return result0;
      }
      
      function parse_end_tag() {
        var result0, result1, result2, result3, result4, result5;
        var pos0, pos1;
        
        reportFailures++;
        pos0 = clone(pos);
        pos1 = clone(pos);
        result0 = parse_ld();
        if (result0 !== null) {
          if (input.charCodeAt(pos.offset) === 47) {
            result1 = "/";
            advance(pos, 1);
          } else {
            result1 = null;
            if (reportFailures === 0) {
              matchFailed("\"/\"");
            }
          }
          if (result1 !== null) {
            result2 = [];
            result3 = parse_ws();
            while (result3 !== null) {
              result2.push(result3);
              result3 = parse_ws();
            }
            if (result2 !== null) {
              result3 = parse_identifier();
              if (result3 !== null) {
                result4 = [];
                result5 = parse_ws();
                while (result5 !== null) {
                  result4.push(result5);
                  result5 = parse_ws();
                }
                if (result4 !== null) {
                  result5 = parse_rd();
                  if (result5 !== null) {
                    result0 = [result0, result1, result2, result3, result4, result5];
                  } else {
                    result0 = null;
                    pos = clone(pos1);
                  }
                } else {
                  result0 = null;
                  pos = clone(pos1);
                }
              } else {
                result0 = null;
                pos = clone(pos1);
              }
            } else {
              result0 = null;
              pos = clone(pos1);
            }
          } else {
            result0 = null;
            pos = clone(pos1);
          }
        } else {
          result0 = null;
          pos = clone(pos1);
        }
        if (result0 !== null) {
          result0 = (function(offset, line, column, n) { return n })(pos0.offset, pos0.line, pos0.column, result0[3]);
        }
        if (result0 === null) {
          pos = clone(pos0);
        }
        reportFailures--;
        if (reportFailures === 0 && result0 === null) {
          matchFailed("end tag");
        }
        return result0;
      }
      
      function parse_context() {
        var result0, result1;
        var pos0, pos1, pos2;
        
        pos0 = clone(pos);
        pos1 = clone(pos);
        pos2 = clone(pos);
        if (input.charCodeAt(pos.offset) === 58) {
          result0 = ":";
          advance(pos, 1);
        } else {
          result0 = null;
          if (reportFailures === 0) {
            matchFailed("\":\"");
          }
        }
        if (result0 !== null) {
          result1 = parse_identifier();
          if (result1 !== null) {
            result0 = [result0, result1];
          } else {
            result0 = null;
            pos = clone(pos2);
          }
        } else {
          result0 = null;
          pos = clone(pos2);
        }
        if (result0 !== null) {
          result0 = (function(offset, line, column, n) {return n})(pos1.offset, pos1.line, pos1.column, result0[1]);
        }
        if (result0 === null) {
          pos = clone(pos1);
        }
        result0 = result0 !== null ? result0 : "";
        if (result0 !== null) {
          result0 = (function(offset, line, column, n) { return n ? ["context", n] : ["context"] })(pos0.offset, pos0.line, pos0.column, result0);
        }
        if (result0 === null) {
          pos = clone(pos0);
        }
        return result0;
      }
      
      function parse_params() {
        var result0, result1, result2, result3, result4;
        var pos0, pos1, pos2;
        
        reportFailures++;
        pos0 = clone(pos);
        result0 = [];
        pos1 = clone(pos);
        pos2 = clone(pos);
        result2 = parse_ws();
        if (result2 !== null) {
          result1 = [];
          while (result2 !== null) {
            result1.push(result2);
            result2 = parse_ws();
          }
        } else {
          result1 = null;
        }
        if (result1 !== null) {
          result2 = parse_key();
          if (result2 !== null) {
            if (input.charCodeAt(pos.offset) === 61) {
              result3 = "=";
              advance(pos, 1);
            } else {
              result3 = null;
              if (reportFailures === 0) {
                matchFailed("\"=\"");
              }
            }
            if (result3 !== null) {
              result4 = parse_number();
              if (result4 === null) {
                result4 = parse_identifier();
                if (result4 === null) {
                  result4 = parse_inline();
                }
              }
              if (result4 !== null) {
                result1 = [result1, result2, result3, result4];
              } else {
                result1 = null;
                pos = clone(pos2);
              }
            } else {
              result1 = null;
              pos = clone(pos2);
            }
          } else {
            result1 = null;
            pos = clone(pos2);
          }
        } else {
          result1 = null;
          pos = clone(pos2);
        }
        if (result1 !== null) {
          result1 = (function(offset, line, column, k, v) {return ["param", ["literal", k], v]})(pos1.offset, pos1.line, pos1.column, result1[1], result1[3]);
        }
        if (result1 === null) {
          pos = clone(pos1);
        }
        while (result1 !== null) {
          result0.push(result1);
          pos1 = clone(pos);
          pos2 = clone(pos);
          result2 = parse_ws();
          if (result2 !== null) {
            result1 = [];
            while (result2 !== null) {
              result1.push(result2);
              result2 = parse_ws();
            }
          } else {
            result1 = null;
          }
          if (result1 !== null) {
            result2 = parse_key();
            if (result2 !== null) {
              if (input.charCodeAt(pos.offset) === 61) {
                result3 = "=";
                advance(pos, 1);
              } else {
                result3 = null;
                if (reportFailures === 0) {
                  matchFailed("\"=\"");
                }
              }
              if (result3 !== null) {
                result4 = parse_number();
                if (result4 === null) {
                  result4 = parse_identifier();
                  if (result4 === null) {
                    result4 = parse_inline();
                  }
                }
                if (result4 !== null) {
                  result1 = [result1, result2, result3, result4];
                } else {
                  result1 = null;
                  pos = clone(pos2);
                }
              } else {
                result1 = null;
                pos = clone(pos2);
              }
            } else {
              result1 = null;
              pos = clone(pos2);
            }
          } else {
            result1 = null;
            pos = clone(pos2);
          }
          if (result1 !== null) {
            result1 = (function(offset, line, column, k, v) {return ["param", ["literal", k], v]})(pos1.offset, pos1.line, pos1.column, result1[1], result1[3]);
          }
          if (result1 === null) {
            pos = clone(pos1);
          }
        }
        if (result0 !== null) {
          result0 = (function(offset, line, column, p) { return ["params"].concat(p) })(pos0.offset, pos0.line, pos0.column, result0);
        }
        if (result0 === null) {
          pos = clone(pos0);
        }
        reportFailures--;
        if (reportFailures === 0 && result0 === null) {
          matchFailed("params");
        }
        return result0;
      }
      
      function parse_bodies() {
        var result0, result1, result2, result3, result4, result5;
        var pos0, pos1, pos2;
        
        reportFailures++;
        pos0 = clone(pos);
        result0 = [];
        pos1 = clone(pos);
        pos2 = clone(pos);
        result1 = parse_ld();
        if (result1 !== null) {
          if (input.charCodeAt(pos.offset) === 58) {
            result2 = ":";
            advance(pos, 1);
          } else {
            result2 = null;
            if (reportFailures === 0) {
              matchFailed("\":\"");
            }
          }
          if (result2 !== null) {
            result3 = parse_key();
            if (result3 !== null) {
              result4 = parse_rd();
              if (result4 !== null) {
                result5 = parse_body();
                if (result5 !== null) {
                  result1 = [result1, result2, result3, result4, result5];
                } else {
                  result1 = null;
                  pos = clone(pos2);
                }
              } else {
                result1 = null;
                pos = clone(pos2);
              }
            } else {
              result1 = null;
              pos = clone(pos2);
            }
          } else {
            result1 = null;
            pos = clone(pos2);
          }
        } else {
          result1 = null;
          pos = clone(pos2);
        }
        if (result1 !== null) {
          result1 = (function(offset, line, column, k, v) {return ["param", ["literal", k], v]})(pos1.offset, pos1.line, pos1.column, result1[2], result1[4]);
        }
        if (result1 === null) {
          pos = clone(pos1);
        }
        while (result1 !== null) {
          result0.push(result1);
          pos1 = clone(pos);
          pos2 = clone(pos);
          result1 = parse_ld();
          if (result1 !== null) {
            if (input.charCodeAt(pos.offset) === 58) {
              result2 = ":";
              advance(pos, 1);
            } else {
              result2 = null;
              if (reportFailures === 0) {
                matchFailed("\":\"");
              }
            }
            if (result2 !== null) {
              result3 = parse_key();
              if (result3 !== null) {
                result4 = parse_rd();
                if (result4 !== null) {
                  result5 = parse_body();
                  if (result5 !== null) {
                    result1 = [result1, result2, result3, result4, result5];
                  } else {
                    result1 = null;
                    pos = clone(pos2);
                  }
                } else {
                  result1 = null;
                  pos = clone(pos2);
                }
              } else {
                result1 = null;
                pos = clone(pos2);
              }
            } else {
              result1 = null;
              pos = clone(pos2);
            }
          } else {
            result1 = null;
            pos = clone(pos2);
          }
          if (result1 !== null) {
            result1 = (function(offset, line, column, k, v) {return ["param", ["literal", k], v]})(pos1.offset, pos1.line, pos1.column, result1[2], result1[4]);
          }
          if (result1 === null) {
            pos = clone(pos1);
          }
        }
        if (result0 !== null) {
          result0 = (function(offset, line, column, p) { return ["bodies"].concat(p) })(pos0.offset, pos0.line, pos0.column, result0);
        }
        if (result0 === null) {
          pos = clone(pos0);
        }
        reportFailures--;
        if (reportFailures === 0 && result0 === null) {
          matchFailed("bodies");
        }
        return result0;
      }
      
      function parse_reference() {
        var result0, result1, result2, result3;
        var pos0, pos1;
        
        reportFailures++;
        pos0 = clone(pos);
        pos1 = clone(pos);
        result0 = parse_ld();
        if (result0 !== null) {
          result1 = parse_identifier();
          if (result1 !== null) {
            result2 = parse_filters();
            if (result2 !== null) {
              result3 = parse_rd();
              if (result3 !== null) {
                result0 = [result0, result1, result2, result3];
              } else {
                result0 = null;
                pos = clone(pos1);
              }
            } else {
              result0 = null;
              pos = clone(pos1);
            }
          } else {
            result0 = null;
            pos = clone(pos1);
          }
        } else {
          result0 = null;
          pos = clone(pos1);
        }
        if (result0 !== null) {
          result0 = (function(offset, line, column, n, f) { return ["reference", n, f].concat([['line', line], ['col', column]]) })(pos0.offset, pos0.line, pos0.column, result0[1], result0[2]);
        }
        if (result0 === null) {
          pos = clone(pos0);
        }
        reportFailures--;
        if (reportFailures === 0 && result0 === null) {
          matchFailed("reference");
        }
        return result0;
      }
      
      function parse_partial() {
        var result0, result1, result2, result3, result4, result5, result6, result7, result8;
        var pos0, pos1, pos2;
        
        reportFailures++;
        pos0 = clone(pos);
        pos1 = clone(pos);
        result0 = parse_ld();
        if (result0 !== null) {
          if (input.charCodeAt(pos.offset) === 62) {
            result1 = ">";
            advance(pos, 1);
          } else {
            result1 = null;
            if (reportFailures === 0) {
              matchFailed("\">\"");
            }
          }
          if (result1 === null) {
            if (input.charCodeAt(pos.offset) === 43) {
              result1 = "+";
              advance(pos, 1);
            } else {
              result1 = null;
              if (reportFailures === 0) {
                matchFailed("\"+\"");
              }
            }
          }
          if (result1 !== null) {
            result2 = [];
            result3 = parse_ws();
            while (result3 !== null) {
              result2.push(result3);
              result3 = parse_ws();
            }
            if (result2 !== null) {
              pos2 = clone(pos);
              result3 = parse_key();
              if (result3 !== null) {
                result3 = (function(offset, line, column, k) {return ["literal", k]})(pos2.offset, pos2.line, pos2.column, result3);
              }
              if (result3 === null) {
                pos = clone(pos2);
              }
              if (result3 === null) {
                result3 = parse_inline();
              }
              if (result3 !== null) {
                result4 = parse_context();
                if (result4 !== null) {
                  result5 = parse_params();
                  if (result5 !== null) {
                    result6 = [];
                    result7 = parse_ws();
                    while (result7 !== null) {
                      result6.push(result7);
                      result7 = parse_ws();
                    }
                    if (result6 !== null) {
                      if (input.charCodeAt(pos.offset) === 47) {
                        result7 = "/";
                        advance(pos, 1);
                      } else {
                        result7 = null;
                        if (reportFailures === 0) {
                          matchFailed("\"/\"");
                        }
                      }
                      if (result7 !== null) {
                        result8 = parse_rd();
                        if (result8 !== null) {
                          result0 = [result0, result1, result2, result3, result4, result5, result6, result7, result8];
                        } else {
                          result0 = null;
                          pos = clone(pos1);
                        }
                      } else {
                        result0 = null;
                        pos = clone(pos1);
                      }
                    } else {
                      result0 = null;
                      pos = clone(pos1);
                    }
                  } else {
                    result0 = null;
                    pos = clone(pos1);
                  }
                } else {
                  result0 = null;
                  pos = clone(pos1);
                }
              } else {
                result0 = null;
                pos = clone(pos1);
              }
            } else {
              result0 = null;
              pos = clone(pos1);
            }
          } else {
            result0 = null;
            pos = clone(pos1);
          }
        } else {
          result0 = null;
          pos = clone(pos1);
        }
        if (result0 !== null) {
          result0 = (function(offset, line, column, s, n, c, p) { var key = (s ===">")? "partial" : s; return [key, n, c, p].concat([['line', line], ['col', column]]) })(pos0.offset, pos0.line, pos0.column, result0[1], result0[3], result0[4], result0[5]);
        }
        if (result0 === null) {
          pos = clone(pos0);
        }
        reportFailures--;
        if (reportFailures === 0 && result0 === null) {
          matchFailed("partial");
        }
        return result0;
      }
      
      function parse_filters() {
        var result0, result1, result2;
        var pos0, pos1, pos2;
        
        reportFailures++;
        pos0 = clone(pos);
        result0 = [];
        pos1 = clone(pos);
        pos2 = clone(pos);
        if (input.charCodeAt(pos.offset) === 124) {
          result1 = "|";
          advance(pos, 1);
        } else {
          result1 = null;
          if (reportFailures === 0) {
            matchFailed("\"|\"");
          }
        }
        if (result1 !== null) {
          result2 = parse_key();
          if (result2 !== null) {
            result1 = [result1, result2];
          } else {
            result1 = null;
            pos = clone(pos2);
          }
        } else {
          result1 = null;
          pos = clone(pos2);
        }
        if (result1 !== null) {
          result1 = (function(offset, line, column, n) {return n})(pos1.offset, pos1.line, pos1.column, result1[1]);
        }
        if (result1 === null) {
          pos = clone(pos1);
        }
        while (result1 !== null) {
          result0.push(result1);
          pos1 = clone(pos);
          pos2 = clone(pos);
          if (input.charCodeAt(pos.offset) === 124) {
            result1 = "|";
            advance(pos, 1);
          } else {
            result1 = null;
            if (reportFailures === 0) {
              matchFailed("\"|\"");
            }
          }
          if (result1 !== null) {
            result2 = parse_key();
            if (result2 !== null) {
              result1 = [result1, result2];
            } else {
              result1 = null;
              pos = clone(pos2);
            }
          } else {
            result1 = null;
            pos = clone(pos2);
          }
          if (result1 !== null) {
            result1 = (function(offset, line, column, n) {return n})(pos1.offset, pos1.line, pos1.column, result1[1]);
          }
          if (result1 === null) {
            pos = clone(pos1);
          }
        }
        if (result0 !== null) {
          result0 = (function(offset, line, column, f) { return ["filters"].concat(f) })(pos0.offset, pos0.line, pos0.column, result0);
        }
        if (result0 === null) {
          pos = clone(pos0);
        }
        reportFailures--;
        if (reportFailures === 0 && result0 === null) {
          matchFailed("filters");
        }
        return result0;
      }
      
      function parse_special() {
        var result0, result1, result2, result3;
        var pos0, pos1;
        
        reportFailures++;
        pos0 = clone(pos);
        pos1 = clone(pos);
        result0 = parse_ld();
        if (result0 !== null) {
          if (input.charCodeAt(pos.offset) === 126) {
            result1 = "~";
            advance(pos, 1);
          } else {
            result1 = null;
            if (reportFailures === 0) {
              matchFailed("\"~\"");
            }
          }
          if (result1 !== null) {
            result2 = parse_key();
            if (result2 !== null) {
              result3 = parse_rd();
              if (result3 !== null) {
                result0 = [result0, result1, result2, result3];
              } else {
                result0 = null;
                pos = clone(pos1);
              }
            } else {
              result0 = null;
              pos = clone(pos1);
            }
          } else {
            result0 = null;
            pos = clone(pos1);
          }
        } else {
          result0 = null;
          pos = clone(pos1);
        }
        if (result0 !== null) {
          result0 = (function(offset, line, column, k) { return ["special", k].concat([['line', line], ['col', column]]) })(pos0.offset, pos0.line, pos0.column, result0[2]);
        }
        if (result0 === null) {
          pos = clone(pos0);
        }
        reportFailures--;
        if (reportFailures === 0 && result0 === null) {
          matchFailed("special");
        }
        return result0;
      }
      
      function parse_identifier() {
        var result0;
        var pos0;
        
        reportFailures++;
        pos0 = clone(pos);
        result0 = parse_path();
        if (result0 !== null) {
          result0 = (function(offset, line, column, p) { var arr = ["path"].concat(p); arr.text = p[1].join('.'); return arr; })(pos0.offset, pos0.line, pos0.column, result0);
        }
        if (result0 === null) {
          pos = clone(pos0);
        }
        if (result0 === null) {
          pos0 = clone(pos);
          result0 = parse_key();
          if (result0 !== null) {
            result0 = (function(offset, line, column, k) { var arr = ["key", k]; arr.text = k; return arr; })(pos0.offset, pos0.line, pos0.column, result0);
          }
          if (result0 === null) {
            pos = clone(pos0);
          }
        }
        reportFailures--;
        if (reportFailures === 0 && result0 === null) {
          matchFailed("identifier");
        }
        return result0;
      }
      
      function parse_number() {
        var result0;
        var pos0;
        
        reportFailures++;
        pos0 = clone(pos);
        result0 = parse_float();
        if (result0 === null) {
          result0 = parse_integer();
        }
        if (result0 !== null) {
          result0 = (function(offset, line, column, n) { return ['literal', n]; })(pos0.offset, pos0.line, pos0.column, result0);
        }
        if (result0 === null) {
          pos = clone(pos0);
        }
        reportFailures--;
        if (reportFailures === 0 && result0 === null) {
          matchFailed("number");
        }
        return result0;
      }
      
      function parse_float() {
        var result0, result1, result2, result3;
        var pos0, pos1;
        
        reportFailures++;
        pos0 = clone(pos);
        pos1 = clone(pos);
        result0 = parse_integer();
        if (result0 !== null) {
          if (input.charCodeAt(pos.offset) === 46) {
            result1 = ".";
            advance(pos, 1);
          } else {
            result1 = null;
            if (reportFailures === 0) {
              matchFailed("\".\"");
            }
          }
          if (result1 !== null) {
            result3 = parse_integer();
            if (result3 !== null) {
              result2 = [];
              while (result3 !== null) {
                result2.push(result3);
                result3 = parse_integer();
              }
            } else {
              result2 = null;
            }
            if (result2 !== null) {
              result0 = [result0, result1, result2];
            } else {
              result0 = null;
              pos = clone(pos1);
            }
          } else {
            result0 = null;
            pos = clone(pos1);
          }
        } else {
          result0 = null;
          pos = clone(pos1);
        }
        if (result0 !== null) {
          result0 = (function(offset, line, column, l, r) { return parseFloat(l + "." + r.join('')); })(pos0.offset, pos0.line, pos0.column, result0[0], result0[2]);
        }
        if (result0 === null) {
          pos = clone(pos0);
        }
        reportFailures--;
        if (reportFailures === 0 && result0 === null) {
          matchFailed("float");
        }
        return result0;
      }
      
      function parse_integer() {
        var result0, result1;
        var pos0;
        
        reportFailures++;
        pos0 = clone(pos);
        if (/^[0-9]/.test(input.charAt(pos.offset))) {
          result1 = input.charAt(pos.offset);
          advance(pos, 1);
        } else {
          result1 = null;
          if (reportFailures === 0) {
            matchFailed("[0-9]");
          }
        }
        if (result1 !== null) {
          result0 = [];
          while (result1 !== null) {
            result0.push(result1);
            if (/^[0-9]/.test(input.charAt(pos.offset))) {
              result1 = input.charAt(pos.offset);
              advance(pos, 1);
            } else {
              result1 = null;
              if (reportFailures === 0) {
                matchFailed("[0-9]");
              }
            }
          }
        } else {
          result0 = null;
        }
        if (result0 !== null) {
          result0 = (function(offset, line, column, digits) { return parseInt(digits.join(""), 10); })(pos0.offset, pos0.line, pos0.column, result0);
        }
        if (result0 === null) {
          pos = clone(pos0);
        }
        reportFailures--;
        if (reportFailures === 0 && result0 === null) {
          matchFailed("integer");
        }
        return result0;
      }
      
      function parse_path() {
        var result0, result1, result2;
        var pos0, pos1;
        
        reportFailures++;
        pos0 = clone(pos);
        pos1 = clone(pos);
        result0 = parse_key();
        result0 = result0 !== null ? result0 : "";
        if (result0 !== null) {
          result2 = parse_array_part();
          if (result2 === null) {
            result2 = parse_array();
          }
          if (result2 !== null) {
            result1 = [];
            while (result2 !== null) {
              result1.push(result2);
              result2 = parse_array_part();
              if (result2 === null) {
                result2 = parse_array();
              }
            }
          } else {
            result1 = null;
          }
          if (result1 !== null) {
            result0 = [result0, result1];
          } else {
            result0 = null;
            pos = clone(pos1);
          }
        } else {
          result0 = null;
          pos = clone(pos1);
        }
        if (result0 !== null) {
          result0 = (function(offset, line, column, k, d) {
            d = d[0];
            if (k && d) {
              d.unshift(k);
              return [false, d].concat([['line', line], ['col', column]]);
            }
            return [true, d].concat([['line', line], ['col', column]]);
          })(pos0.offset, pos0.line, pos0.column, result0[0], result0[1]);
        }
        if (result0 === null) {
          pos = clone(pos0);
        }
        if (result0 === null) {
          pos0 = clone(pos);
          pos1 = clone(pos);
          if (input.charCodeAt(pos.offset) === 46) {
            result0 = ".";
            advance(pos, 1);
          } else {
            result0 = null;
            if (reportFailures === 0) {
              matchFailed("\".\"");
            }
          }
          if (result0 !== null) {
            result1 = [];
            result2 = parse_array_part();
            if (result2 === null) {
              result2 = parse_array();
            }
            while (result2 !== null) {
              result1.push(result2);
              result2 = parse_array_part();
              if (result2 === null) {
                result2 = parse_array();
              }
            }
            if (result1 !== null) {
              result0 = [result0, result1];
            } else {
              result0 = null;
              pos = clone(pos1);
            }
          } else {
            result0 = null;
            pos = clone(pos1);
          }
          if (result0 !== null) {
            result0 = (function(offset, line, column, d) {
              if (d.length > 0) {
                return [true, d[0]].concat([['line', line], ['col', column]]);
              }
              return [true, []].concat([['line', line], ['col', column]]);
            })(pos0.offset, pos0.line, pos0.column, result0[1]);
          }
          if (result0 === null) {
            pos = clone(pos0);
          }
        }
        reportFailures--;
        if (reportFailures === 0 && result0 === null) {
          matchFailed("path");
        }
        return result0;
      }
      
      function parse_key() {
        var result0, result1, result2;
        var pos0, pos1;
        
        reportFailures++;
        pos0 = clone(pos);
        pos1 = clone(pos);
        if (/^[a-zA-Z_$]/.test(input.charAt(pos.offset))) {
          result0 = input.charAt(pos.offset);
          advance(pos, 1);
        } else {
          result0 = null;
          if (reportFailures === 0) {
            matchFailed("[a-zA-Z_$]");
          }
        }
        if (result0 !== null) {
          result1 = [];
          if (/^[0-9a-zA-Z_$\-]/.test(input.charAt(pos.offset))) {
            result2 = input.charAt(pos.offset);
            advance(pos, 1);
          } else {
            result2 = null;
            if (reportFailures === 0) {
              matchFailed("[0-9a-zA-Z_$\\-]");
            }
          }
          while (result2 !== null) {
            result1.push(result2);
            if (/^[0-9a-zA-Z_$\-]/.test(input.charAt(pos.offset))) {
              result2 = input.charAt(pos.offset);
              advance(pos, 1);
            } else {
              result2 = null;
              if (reportFailures === 0) {
                matchFailed("[0-9a-zA-Z_$\\-]");
              }
            }
          }
          if (result1 !== null) {
            result0 = [result0, result1];
          } else {
            result0 = null;
            pos = clone(pos1);
          }
        } else {
          result0 = null;
          pos = clone(pos1);
        }
        if (result0 !== null) {
          result0 = (function(offset, line, column, h, t) { return h + t.join('') })(pos0.offset, pos0.line, pos0.column, result0[0], result0[1]);
        }
        if (result0 === null) {
          pos = clone(pos0);
        }
        reportFailures--;
        if (reportFailures === 0 && result0 === null) {
          matchFailed("key");
        }
        return result0;
      }
      
      function parse_array() {
        var result0, result1, result2;
        var pos0, pos1, pos2, pos3, pos4;
        
        reportFailures++;
        pos0 = clone(pos);
        pos1 = clone(pos);
        pos2 = clone(pos);
        pos3 = clone(pos);
        result0 = parse_lb();
        if (result0 !== null) {
          pos4 = clone(pos);
          if (/^[0-9]/.test(input.charAt(pos.offset))) {
            result2 = input.charAt(pos.offset);
            advance(pos, 1);
          } else {
            result2 = null;
            if (reportFailures === 0) {
              matchFailed("[0-9]");
            }
          }
          if (result2 !== null) {
            result1 = [];
            while (result2 !== null) {
              result1.push(result2);
              if (/^[0-9]/.test(input.charAt(pos.offset))) {
                result2 = input.charAt(pos.offset);
                advance(pos, 1);
              } else {
                result2 = null;
                if (reportFailures === 0) {
                  matchFailed("[0-9]");
                }
              }
            }
          } else {
            result1 = null;
          }
          if (result1 !== null) {
            result1 = (function(offset, line, column, n) {return n.join('')})(pos4.offset, pos4.line, pos4.column, result1);
          }
          if (result1 === null) {
            pos = clone(pos4);
          }
          if (result1 === null) {
            result1 = parse_identifier();
          }
          if (result1 !== null) {
            result2 = parse_rb();
            if (result2 !== null) {
              result0 = [result0, result1, result2];
            } else {
              result0 = null;
              pos = clone(pos3);
            }
          } else {
            result0 = null;
            pos = clone(pos3);
          }
        } else {
          result0 = null;
          pos = clone(pos3);
        }
        if (result0 !== null) {
          result0 = (function(offset, line, column, a) {return a; })(pos2.offset, pos2.line, pos2.column, result0[1]);
        }
        if (result0 === null) {
          pos = clone(pos2);
        }
        if (result0 !== null) {
          result1 = parse_array_part();
          result1 = result1 !== null ? result1 : "";
          if (result1 !== null) {
            result0 = [result0, result1];
          } else {
            result0 = null;
            pos = clone(pos1);
          }
        } else {
          result0 = null;
          pos = clone(pos1);
        }
        if (result0 !== null) {
          result0 = (function(offset, line, column, i, nk) { if(nk) { nk.unshift(i); } else {nk = [i] } return nk; })(pos0.offset, pos0.line, pos0.column, result0[0], result0[1]);
        }
        if (result0 === null) {
          pos = clone(pos0);
        }
        reportFailures--;
        if (reportFailures === 0 && result0 === null) {
          matchFailed("array");
        }
        return result0;
      }
      
      function parse_array_part() {
        var result0, result1, result2;
        var pos0, pos1, pos2, pos3;
        
        reportFailures++;
        pos0 = clone(pos);
        pos1 = clone(pos);
        pos2 = clone(pos);
        pos3 = clone(pos);
        if (input.charCodeAt(pos.offset) === 46) {
          result1 = ".";
          advance(pos, 1);
        } else {
          result1 = null;
          if (reportFailures === 0) {
            matchFailed("\".\"");
          }
        }
        if (result1 !== null) {
          result2 = parse_key();
          if (result2 !== null) {
            result1 = [result1, result2];
          } else {
            result1 = null;
            pos = clone(pos3);
          }
        } else {
          result1 = null;
          pos = clone(pos3);
        }
        if (result1 !== null) {
          result1 = (function(offset, line, column, k) {return k})(pos2.offset, pos2.line, pos2.column, result1[1]);
        }
        if (result1 === null) {
          pos = clone(pos2);
        }
        if (result1 !== null) {
          result0 = [];
          while (result1 !== null) {
            result0.push(result1);
            pos2 = clone(pos);
            pos3 = clone(pos);
            if (input.charCodeAt(pos.offset) === 46) {
              result1 = ".";
              advance(pos, 1);
            } else {
              result1 = null;
              if (reportFailures === 0) {
                matchFailed("\".\"");
              }
            }
            if (result1 !== null) {
              result2 = parse_key();
              if (result2 !== null) {
                result1 = [result1, result2];
              } else {
                result1 = null;
                pos = clone(pos3);
              }
            } else {
              result1 = null;
              pos = clone(pos3);
            }
            if (result1 !== null) {
              result1 = (function(offset, line, column, k) {return k})(pos2.offset, pos2.line, pos2.column, result1[1]);
            }
            if (result1 === null) {
              pos = clone(pos2);
            }
          }
        } else {
          result0 = null;
        }
        if (result0 !== null) {
          result1 = parse_array();
          result1 = result1 !== null ? result1 : "";
          if (result1 !== null) {
            result0 = [result0, result1];
          } else {
            result0 = null;
            pos = clone(pos1);
          }
        } else {
          result0 = null;
          pos = clone(pos1);
        }
        if (result0 !== null) {
          result0 = (function(offset, line, column, d, a) { if (a) { return d.concat(a); } else { return d; } })(pos0.offset, pos0.line, pos0.column, result0[0], result0[1]);
        }
        if (result0 === null) {
          pos = clone(pos0);
        }
        reportFailures--;
        if (reportFailures === 0 && result0 === null) {
          matchFailed("array_part");
        }
        return result0;
      }
      
      function parse_inline() {
        var result0, result1, result2;
        var pos0, pos1;
        
        reportFailures++;
        pos0 = clone(pos);
        pos1 = clone(pos);
        if (input.charCodeAt(pos.offset) === 34) {
          result0 = "\"";
          advance(pos, 1);
        } else {
          result0 = null;
          if (reportFailures === 0) {
            matchFailed("\"\\\"\"");
          }
        }
        if (result0 !== null) {
          if (input.charCodeAt(pos.offset) === 34) {
            result1 = "\"";
            advance(pos, 1);
          } else {
            result1 = null;
            if (reportFailures === 0) {
              matchFailed("\"\\\"\"");
            }
          }
          if (result1 !== null) {
            result0 = [result0, result1];
          } else {
            result0 = null;
            pos = clone(pos1);
          }
        } else {
          result0 = null;
          pos = clone(pos1);
        }
        if (result0 !== null) {
          result0 = (function(offset, line, column) { return ["literal", ""].concat([['line', line], ['col', column]]) })(pos0.offset, pos0.line, pos0.column);
        }
        if (result0 === null) {
          pos = clone(pos0);
        }
        if (result0 === null) {
          pos0 = clone(pos);
          pos1 = clone(pos);
          if (input.charCodeAt(pos.offset) === 34) {
            result0 = "\"";
            advance(pos, 1);
          } else {
            result0 = null;
            if (reportFailures === 0) {
              matchFailed("\"\\\"\"");
            }
          }
          if (result0 !== null) {
            result1 = parse_literal();
            if (result1 !== null) {
              if (input.charCodeAt(pos.offset) === 34) {
                result2 = "\"";
                advance(pos, 1);
              } else {
                result2 = null;
                if (reportFailures === 0) {
                  matchFailed("\"\\\"\"");
                }
              }
              if (result2 !== null) {
                result0 = [result0, result1, result2];
              } else {
                result0 = null;
                pos = clone(pos1);
              }
            } else {
              result0 = null;
              pos = clone(pos1);
            }
          } else {
            result0 = null;
            pos = clone(pos1);
          }
          if (result0 !== null) {
            result0 = (function(offset, line, column, l) { return ["literal", l].concat([['line', line], ['col', column]]) })(pos0.offset, pos0.line, pos0.column, result0[1]);
          }
          if (result0 === null) {
            pos = clone(pos0);
          }
          if (result0 === null) {
            pos0 = clone(pos);
            pos1 = clone(pos);
            if (input.charCodeAt(pos.offset) === 34) {
              result0 = "\"";
              advance(pos, 1);
            } else {
              result0 = null;
              if (reportFailures === 0) {
                matchFailed("\"\\\"\"");
              }
            }
            if (result0 !== null) {
              result2 = parse_inline_part();
              if (result2 !== null) {
                result1 = [];
                while (result2 !== null) {
                  result1.push(result2);
                  result2 = parse_inline_part();
                }
              } else {
                result1 = null;
              }
              if (result1 !== null) {
                if (input.charCodeAt(pos.offset) === 34) {
                  result2 = "\"";
                  advance(pos, 1);
                } else {
                  result2 = null;
                  if (reportFailures === 0) {
                    matchFailed("\"\\\"\"");
                  }
                }
                if (result2 !== null) {
                  result0 = [result0, result1, result2];
                } else {
                  result0 = null;
                  pos = clone(pos1);
                }
              } else {
                result0 = null;
                pos = clone(pos1);
              }
            } else {
              result0 = null;
              pos = clone(pos1);
            }
            if (result0 !== null) {
              result0 = (function(offset, line, column, p) { return ["body"].concat(p).concat([['line', line], ['col', column]]) })(pos0.offset, pos0.line, pos0.column, result0[1]);
            }
            if (result0 === null) {
              pos = clone(pos0);
            }
          }
        }
        reportFailures--;
        if (reportFailures === 0 && result0 === null) {
          matchFailed("inline");
        }
        return result0;
      }
      
      function parse_inline_part() {
        var result0;
        var pos0;
        
        result0 = parse_special();
        if (result0 === null) {
          result0 = parse_reference();
          if (result0 === null) {
            pos0 = clone(pos);
            result0 = parse_literal();
            if (result0 !== null) {
              result0 = (function(offset, line, column, l) { return ["buffer", l] })(pos0.offset, pos0.line, pos0.column, result0);
            }
            if (result0 === null) {
              pos = clone(pos0);
            }
          }
        }
        return result0;
      }
      
      function parse_buffer() {
        var result0, result1, result2, result3, result4;
        var pos0, pos1, pos2, pos3;
        
        reportFailures++;
        pos0 = clone(pos);
        pos1 = clone(pos);
        result0 = parse_eol();
        if (result0 !== null) {
          result1 = [];
          result2 = parse_ws();
          while (result2 !== null) {
            result1.push(result2);
            result2 = parse_ws();
          }
          if (result1 !== null) {
            result0 = [result0, result1];
          } else {
            result0 = null;
            pos = clone(pos1);
          }
        } else {
          result0 = null;
          pos = clone(pos1);
        }
        if (result0 !== null) {
          result0 = (function(offset, line, column, e, w) { return ["format", e, w.join('')].concat([['line', line], ['col', column]]) })(pos0.offset, pos0.line, pos0.column, result0[0], result0[1]);
        }
        if (result0 === null) {
          pos = clone(pos0);
        }
        if (result0 === null) {
          pos0 = clone(pos);
          pos1 = clone(pos);
          pos2 = clone(pos);
          pos3 = clone(pos);
          reportFailures++;
          result1 = parse_tag();
          reportFailures--;
          if (result1 === null) {
            result1 = "";
          } else {
            result1 = null;
            pos = clone(pos3);
          }
          if (result1 !== null) {
            pos3 = clone(pos);
            reportFailures++;
            result2 = parse_comment();
            reportFailures--;
            if (result2 === null) {
              result2 = "";
            } else {
              result2 = null;
              pos = clone(pos3);
            }
            if (result2 !== null) {
              pos3 = clone(pos);
              reportFailures++;
              result3 = parse_eol();
              reportFailures--;
              if (result3 === null) {
                result3 = "";
              } else {
                result3 = null;
                pos = clone(pos3);
              }
              if (result3 !== null) {
                if (input.length > pos.offset) {
                  result4 = input.charAt(pos.offset);
                  advance(pos, 1);
                } else {
                  result4 = null;
                  if (reportFailures === 0) {
                    matchFailed("any character");
                  }
                }
                if (result4 !== null) {
                  result1 = [result1, result2, result3, result4];
                } else {
                  result1 = null;
                  pos = clone(pos2);
                }
              } else {
                result1 = null;
                pos = clone(pos2);
              }
            } else {
              result1 = null;
              pos = clone(pos2);
            }
          } else {
            result1 = null;
            pos = clone(pos2);
          }
          if (result1 !== null) {
            result1 = (function(offset, line, column, c) {return c})(pos1.offset, pos1.line, pos1.column, result1[3]);
          }
          if (result1 === null) {
            pos = clone(pos1);
          }
          if (result1 !== null) {
            result0 = [];
            while (result1 !== null) {
              result0.push(result1);
              pos1 = clone(pos);
              pos2 = clone(pos);
              pos3 = clone(pos);
              reportFailures++;
              result1 = parse_tag();
              reportFailures--;
              if (result1 === null) {
                result1 = "";
              } else {
                result1 = null;
                pos = clone(pos3);
              }
              if (result1 !== null) {
                pos3 = clone(pos);
                reportFailures++;
                result2 = parse_comment();
                reportFailures--;
                if (result2 === null) {
                  result2 = "";
                } else {
                  result2 = null;
                  pos = clone(pos3);
                }
                if (result2 !== null) {
                  pos3 = clone(pos);
                  reportFailures++;
                  result3 = parse_eol();
                  reportFailures--;
                  if (result3 === null) {
                    result3 = "";
                  } else {
                    result3 = null;
                    pos = clone(pos3);
                  }
                  if (result3 !== null) {
                    if (input.length > pos.offset) {
                      result4 = input.charAt(pos.offset);
                      advance(pos, 1);
                    } else {
                      result4 = null;
                      if (reportFailures === 0) {
                        matchFailed("any character");
                      }
                    }
                    if (result4 !== null) {
                      result1 = [result1, result2, result3, result4];
                    } else {
                      result1 = null;
                      pos = clone(pos2);
                    }
                  } else {
                    result1 = null;
                    pos = clone(pos2);
                  }
                } else {
                  result1 = null;
                  pos = clone(pos2);
                }
              } else {
                result1 = null;
                pos = clone(pos2);
              }
              if (result1 !== null) {
                result1 = (function(offset, line, column, c) {return c})(pos1.offset, pos1.line, pos1.column, result1[3]);
              }
              if (result1 === null) {
                pos = clone(pos1);
              }
            }
          } else {
            result0 = null;
          }
          if (result0 !== null) {
            result0 = (function(offset, line, column, b) { return ["buffer", b.join('')].concat([['line', line], ['col', column]]) })(pos0.offset, pos0.line, pos0.column, result0);
          }
          if (result0 === null) {
            pos = clone(pos0);
          }
        }
        reportFailures--;
        if (reportFailures === 0 && result0 === null) {
          matchFailed("buffer");
        }
        return result0;
      }
      
      function parse_literal() {
        var result0, result1, result2;
        var pos0, pos1, pos2, pos3;
        
        reportFailures++;
        pos0 = clone(pos);
        pos1 = clone(pos);
        pos2 = clone(pos);
        pos3 = clone(pos);
        reportFailures++;
        result1 = parse_tag();
        reportFailures--;
        if (result1 === null) {
          result1 = "";
        } else {
          result1 = null;
          pos = clone(pos3);
        }
        if (result1 !== null) {
          result2 = parse_esc();
          if (result2 === null) {
            if (/^[^"]/.test(input.charAt(pos.offset))) {
              result2 = input.charAt(pos.offset);
              advance(pos, 1);
            } else {
              result2 = null;
              if (reportFailures === 0) {
                matchFailed("[^\"]");
              }
            }
          }
          if (result2 !== null) {
            result1 = [result1, result2];
          } else {
            result1 = null;
            pos = clone(pos2);
          }
        } else {
          result1 = null;
          pos = clone(pos2);
        }
        if (result1 !== null) {
          result1 = (function(offset, line, column, c) {return c})(pos1.offset, pos1.line, pos1.column, result1[1]);
        }
        if (result1 === null) {
          pos = clone(pos1);
        }
        if (result1 !== null) {
          result0 = [];
          while (result1 !== null) {
            result0.push(result1);
            pos1 = clone(pos);
            pos2 = clone(pos);
            pos3 = clone(pos);
            reportFailures++;
            result1 = parse_tag();
            reportFailures--;
            if (result1 === null) {
              result1 = "";
            } else {
              result1 = null;
              pos = clone(pos3);
            }
            if (result1 !== null) {
              result2 = parse_esc();
              if (result2 === null) {
                if (/^[^"]/.test(input.charAt(pos.offset))) {
                  result2 = input.charAt(pos.offset);
                  advance(pos, 1);
                } else {
                  result2 = null;
                  if (reportFailures === 0) {
                    matchFailed("[^\"]");
                  }
                }
              }
              if (result2 !== null) {
                result1 = [result1, result2];
              } else {
                result1 = null;
                pos = clone(pos2);
              }
            } else {
              result1 = null;
              pos = clone(pos2);
            }
            if (result1 !== null) {
              result1 = (function(offset, line, column, c) {return c})(pos1.offset, pos1.line, pos1.column, result1[1]);
            }
            if (result1 === null) {
              pos = clone(pos1);
            }
          }
        } else {
          result0 = null;
        }
        if (result0 !== null) {
          result0 = (function(offset, line, column, b) { return b.join('') })(pos0.offset, pos0.line, pos0.column, result0);
        }
        if (result0 === null) {
          pos = clone(pos0);
        }
        reportFailures--;
        if (reportFailures === 0 && result0 === null) {
          matchFailed("literal");
        }
        return result0;
      }
      
      function parse_esc() {
        var result0;
        var pos0;
        
        pos0 = clone(pos);
        if (input.substr(pos.offset, 2) === "\\\"") {
          result0 = "\\\"";
          advance(pos, 2);
        } else {
          result0 = null;
          if (reportFailures === 0) {
            matchFailed("\"\\\\\\\"\"");
          }
        }
        if (result0 !== null) {
          result0 = (function(offset, line, column) { return '"' })(pos0.offset, pos0.line, pos0.column);
        }
        if (result0 === null) {
          pos = clone(pos0);
        }
        return result0;
      }
      
      function parse_comment() {
        var result0, result1, result2, result3;
        var pos0, pos1, pos2, pos3, pos4;
        
        reportFailures++;
        pos0 = clone(pos);
        pos1 = clone(pos);
        if (input.substr(pos.offset, 2) === "{!") {
          result0 = "{!";
          advance(pos, 2);
        } else {
          result0 = null;
          if (reportFailures === 0) {
            matchFailed("\"{!\"");
          }
        }
        if (result0 !== null) {
          result1 = [];
          pos2 = clone(pos);
          pos3 = clone(pos);
          pos4 = clone(pos);
          reportFailures++;
          if (input.substr(pos.offset, 2) === "!}") {
            result2 = "!}";
            advance(pos, 2);
          } else {
            result2 = null;
            if (reportFailures === 0) {
              matchFailed("\"!}\"");
            }
          }
          reportFailures--;
          if (result2 === null) {
            result2 = "";
          } else {
            result2 = null;
            pos = clone(pos4);
          }
          if (result2 !== null) {
            if (input.length > pos.offset) {
              result3 = input.charAt(pos.offset);
              advance(pos, 1);
            } else {
              result3 = null;
              if (reportFailures === 0) {
                matchFailed("any character");
              }
            }
            if (result3 !== null) {
              result2 = [result2, result3];
            } else {
              result2 = null;
              pos = clone(pos3);
            }
          } else {
            result2 = null;
            pos = clone(pos3);
          }
          if (result2 !== null) {
            result2 = (function(offset, line, column, c) {return c})(pos2.offset, pos2.line, pos2.column, result2[1]);
          }
          if (result2 === null) {
            pos = clone(pos2);
          }
          while (result2 !== null) {
            result1.push(result2);
            pos2 = clone(pos);
            pos3 = clone(pos);
            pos4 = clone(pos);
            reportFailures++;
            if (input.substr(pos.offset, 2) === "!}") {
              result2 = "!}";
              advance(pos, 2);
            } else {
              result2 = null;
              if (reportFailures === 0) {
                matchFailed("\"!}\"");
              }
            }
            reportFailures--;
            if (result2 === null) {
              result2 = "";
            } else {
              result2 = null;
              pos = clone(pos4);
            }
            if (result2 !== null) {
              if (input.length > pos.offset) {
                result3 = input.charAt(pos.offset);
                advance(pos, 1);
              } else {
                result3 = null;
                if (reportFailures === 0) {
                  matchFailed("any character");
                }
              }
              if (result3 !== null) {
                result2 = [result2, result3];
              } else {
                result2 = null;
                pos = clone(pos3);
              }
            } else {
              result2 = null;
              pos = clone(pos3);
            }
            if (result2 !== null) {
              result2 = (function(offset, line, column, c) {return c})(pos2.offset, pos2.line, pos2.column, result2[1]);
            }
            if (result2 === null) {
              pos = clone(pos2);
            }
          }
          if (result1 !== null) {
            if (input.substr(pos.offset, 2) === "!}") {
              result2 = "!}";
              advance(pos, 2);
            } else {
              result2 = null;
              if (reportFailures === 0) {
                matchFailed("\"!}\"");
              }
            }
            if (result2 !== null) {
              result0 = [result0, result1, result2];
            } else {
              result0 = null;
              pos = clone(pos1);
            }
          } else {
            result0 = null;
            pos = clone(pos1);
          }
        } else {
          result0 = null;
          pos = clone(pos1);
        }
        if (result0 !== null) {
          result0 = (function(offset, line, column, c) { return ["comment", c.join('')].concat([['line', line], ['col', column]]) })(pos0.offset, pos0.line, pos0.column, result0[1]);
        }
        if (result0 === null) {
          pos = clone(pos0);
        }
        reportFailures--;
        if (reportFailures === 0 && result0 === null) {
          matchFailed("comment");
        }
        return result0;
      }
      
      function parse_tag() {
        var result0, result1, result2, result3, result4, result5, result6, result7;
        var pos0, pos1, pos2;
        
        pos0 = clone(pos);
        result0 = parse_ld();
        if (result0 !== null) {
          result1 = [];
          result2 = parse_ws();
          while (result2 !== null) {
            result1.push(result2);
            result2 = parse_ws();
          }
          if (result1 !== null) {
            if (/^[#?^><+%:@\/~%]/.test(input.charAt(pos.offset))) {
              result2 = input.charAt(pos.offset);
              advance(pos, 1);
            } else {
              result2 = null;
              if (reportFailures === 0) {
                matchFailed("[#?^><+%:@\\/~%]");
              }
            }
            if (result2 !== null) {
              result3 = [];
              result4 = parse_ws();
              while (result4 !== null) {
                result3.push(result4);
                result4 = parse_ws();
              }
              if (result3 !== null) {
                pos1 = clone(pos);
                pos2 = clone(pos);
                reportFailures++;
                result5 = parse_rd();
                reportFailures--;
                if (result5 === null) {
                  result5 = "";
                } else {
                  result5 = null;
                  pos = clone(pos2);
                }
                if (result5 !== null) {
                  pos2 = clone(pos);
                  reportFailures++;
                  result6 = parse_eol();
                  reportFailures--;
                  if (result6 === null) {
                    result6 = "";
                  } else {
                    result6 = null;
                    pos = clone(pos2);
                  }
                  if (result6 !== null) {
                    if (input.length > pos.offset) {
                      result7 = input.charAt(pos.offset);
                      advance(pos, 1);
                    } else {
                      result7 = null;
                      if (reportFailures === 0) {
                        matchFailed("any character");
                      }
                    }
                    if (result7 !== null) {
                      result5 = [result5, result6, result7];
                    } else {
                      result5 = null;
                      pos = clone(pos1);
                    }
                  } else {
                    result5 = null;
                    pos = clone(pos1);
                  }
                } else {
                  result5 = null;
                  pos = clone(pos1);
                }
                if (result5 !== null) {
                  result4 = [];
                  while (result5 !== null) {
                    result4.push(result5);
                    pos1 = clone(pos);
                    pos2 = clone(pos);
                    reportFailures++;
                    result5 = parse_rd();
                    reportFailures--;
                    if (result5 === null) {
                      result5 = "";
                    } else {
                      result5 = null;
                      pos = clone(pos2);
                    }
                    if (result5 !== null) {
                      pos2 = clone(pos);
                      reportFailures++;
                      result6 = parse_eol();
                      reportFailures--;
                      if (result6 === null) {
                        result6 = "";
                      } else {
                        result6 = null;
                        pos = clone(pos2);
                      }
                      if (result6 !== null) {
                        if (input.length > pos.offset) {
                          result7 = input.charAt(pos.offset);
                          advance(pos, 1);
                        } else {
                          result7 = null;
                          if (reportFailures === 0) {
                            matchFailed("any character");
                          }
                        }
                        if (result7 !== null) {
                          result5 = [result5, result6, result7];
                        } else {
                          result5 = null;
                          pos = clone(pos1);
                        }
                      } else {
                        result5 = null;
                        pos = clone(pos1);
                      }
                    } else {
                      result5 = null;
                      pos = clone(pos1);
                    }
                  }
                } else {
                  result4 = null;
                }
                if (result4 !== null) {
                  result5 = [];
                  result6 = parse_ws();
                  while (result6 !== null) {
                    result5.push(result6);
                    result6 = parse_ws();
                  }
                  if (result5 !== null) {
                    result6 = parse_rd();
                    if (result6 !== null) {
                      result0 = [result0, result1, result2, result3, result4, result5, result6];
                    } else {
                      result0 = null;
                      pos = clone(pos0);
                    }
                  } else {
                    result0 = null;
                    pos = clone(pos0);
                  }
                } else {
                  result0 = null;
                  pos = clone(pos0);
                }
              } else {
                result0 = null;
                pos = clone(pos0);
              }
            } else {
              result0 = null;
              pos = clone(pos0);
            }
          } else {
            result0 = null;
            pos = clone(pos0);
          }
        } else {
          result0 = null;
          pos = clone(pos0);
        }
        if (result0 === null) {
          result0 = parse_reference();
        }
        return result0;
      }
      
      function parse_ld() {
        var result0;
        
        if (input.charCodeAt(pos.offset) === 123) {
          result0 = "{";
          advance(pos, 1);
        } else {
          result0 = null;
          if (reportFailures === 0) {
            matchFailed("\"{\"");
          }
        }
        return result0;
      }
      
      function parse_rd() {
        var result0;
        
        if (input.charCodeAt(pos.offset) === 125) {
          result0 = "}";
          advance(pos, 1);
        } else {
          result0 = null;
          if (reportFailures === 0) {
            matchFailed("\"}\"");
          }
        }
        return result0;
      }
      
      function parse_lb() {
        var result0;
        
        if (input.charCodeAt(pos.offset) === 91) {
          result0 = "[";
          advance(pos, 1);
        } else {
          result0 = null;
          if (reportFailures === 0) {
            matchFailed("\"[\"");
          }
        }
        return result0;
      }
      
      function parse_rb() {
        var result0;
        
        if (input.charCodeAt(pos.offset) === 93) {
          result0 = "]";
          advance(pos, 1);
        } else {
          result0 = null;
          if (reportFailures === 0) {
            matchFailed("\"]\"");
          }
        }
        return result0;
      }
      
      function parse_eol() {
        var result0;
        
        if (input.charCodeAt(pos.offset) === 10) {
          result0 = "\n";
          advance(pos, 1);
        } else {
          result0 = null;
          if (reportFailures === 0) {
            matchFailed("\"\\n\"");
          }
        }
        if (result0 === null) {
          if (input.substr(pos.offset, 2) === "\r\n") {
            result0 = "\r\n";
            advance(pos, 2);
          } else {
            result0 = null;
            if (reportFailures === 0) {
              matchFailed("\"\\r\\n\"");
            }
          }
          if (result0 === null) {
            if (input.charCodeAt(pos.offset) === 13) {
              result0 = "\r";
              advance(pos, 1);
            } else {
              result0 = null;
              if (reportFailures === 0) {
                matchFailed("\"\\r\"");
              }
            }
            if (result0 === null) {
              if (input.charCodeAt(pos.offset) === 8232) {
                result0 = "\u2028";
                advance(pos, 1);
              } else {
                result0 = null;
                if (reportFailures === 0) {
                  matchFailed("\"\\u2028\"");
                }
              }
              if (result0 === null) {
                if (input.charCodeAt(pos.offset) === 8233) {
                  result0 = "\u2029";
                  advance(pos, 1);
                } else {
                  result0 = null;
                  if (reportFailures === 0) {
                    matchFailed("\"\\u2029\"");
                  }
                }
              }
            }
          }
        }
        return result0;
      }
      
      function parse_ws() {
        var result0;
        
        if (/^[\t\x0B\f \xA0\uFEFF]/.test(input.charAt(pos.offset))) {
          result0 = input.charAt(pos.offset);
          advance(pos, 1);
        } else {
          result0 = null;
          if (reportFailures === 0) {
            matchFailed("[\\t\\x0B\\f \\xA0\\uFEFF]");
          }
        }
        if (result0 === null) {
          result0 = parse_eol();
        }
        return result0;
      }
      
      
      function cleanupExpected(expected) {
        expected.sort();
        
        var lastExpected = null;
        var cleanExpected = [];
        for (var i = 0; i < expected.length; i++) {
          if (expected[i] !== lastExpected) {
            cleanExpected.push(expected[i]);
            lastExpected = expected[i];
          }
        }
        return cleanExpected;
      }
      
      
      
      var result = parseFunctions[startRule]();
      
      /*
       * The parser is now in one of the following three states:
       *
       * 1. The parser successfully parsed the whole input.
       *
       *    - |result !== null|
       *    - |pos.offset === input.length|
       *    - |rightmostFailuresExpected| may or may not contain something
       *
       * 2. The parser successfully parsed only a part of the input.
       *
       *    - |result !== null|
       *    - |pos.offset < input.length|
       *    - |rightmostFailuresExpected| may or may not contain something
       *
       * 3. The parser did not successfully parse any part of the input.
       *
       *   - |result === null|
       *   - |pos.offset === 0|
       *   - |rightmostFailuresExpected| contains at least one failure
       *
       * All code following this comment (including called functions) must
       * handle these states.
       */
      if (result === null || pos.offset !== input.length) {
        var offset = Math.max(pos.offset, rightmostFailuresPos.offset);
        var found = offset < input.length ? input.charAt(offset) : null;
        var errorPosition = pos.offset > rightmostFailuresPos.offset ? pos : rightmostFailuresPos;
        
        throw new parser.SyntaxError(
          cleanupExpected(rightmostFailuresExpected),
          found,
          offset,
          errorPosition.line,
          errorPosition.column
        );
      }
      
      return result;
    },
    
    /* Returns the parser source code. */
    toSource: function() { return this._source; }
  };
  
  /* Thrown when a parser encounters a syntax error. */
  
  result.SyntaxError = function(expected, found, offset, line, column) {
    function buildMessage(expected, found) {
      var expectedHumanized, foundHumanized;
      
      switch (expected.length) {
        case 0:
          expectedHumanized = "end of input";
          break;
        case 1:
          expectedHumanized = expected[0];
          break;
        default:
          expectedHumanized = expected.slice(0, expected.length - 1).join(", ")
            + " or "
            + expected[expected.length - 1];
      }
      
      foundHumanized = found ? quote(found) : "end of input";
      
      return "Expected " + expectedHumanized + " but " + foundHumanized + " found.";
    }
    
    this.name = "SyntaxError";
    this.expected = expected;
    this.found = found;
    this.message = buildMessage(expected, found);
    this.offset = offset;
    this.line = line;
    this.column = column;
  };
  
  result.SyntaxError.prototype = Error.prototype;
  
  return result;
})();

dust.parse = parser.parse;

})(typeof exports !== 'undefined' ? exports : getGlobal());
define("dust", (function (global) {
    return function () {
        var ret, fn;
        return ret || global.dust;
    };
}(this)));

/*!
 * jQuery Cookie Plugin v1.3.1
 * https://github.com/carhartl/jquery-cookie
 *
 * Copyright 2013 Klaus Hartl
 * Released under the MIT license
 */
(function (factory) {
	if (typeof define === 'function' && define.amd && define.amd.jQuery) {
		// AMD. Register as anonymous module.
		define('jquery.cookie',['jquery'], factory);
	} else {
		// Browser globals.
		factory(jQuery);
	}
}(function ($) {

	var pluses = /\+/g;

	function raw(s) {
		return s;
	}

	function decoded(s) {
		return decodeURIComponent(s.replace(pluses, ' '));
	}

	function converted(s) {
		if (s.indexOf('"') === 0) {
			// This is a quoted cookie as according to RFC2068, unescape
			s = s.slice(1, -1).replace(/\\"/g, '"').replace(/\\\\/g, '\\');
		}
		try {
			return config.json ? JSON.parse(s) : s;
		} catch(er) {}
	}

	var config = $.cookie = function (key, value, options) {

		// write
		if (value !== undefined) {
			options = $.extend({}, config.defaults, options);

			if (typeof options.expires === 'number') {
				var days = options.expires, t = options.expires = new Date();
				t.setDate(t.getDate() + days);
			}

			value = config.json ? JSON.stringify(value) : String(value);

			return (document.cookie = [
				encodeURIComponent(key), '=', config.raw ? value : encodeURIComponent(value),
				options.expires ? '; expires=' + options.expires.toUTCString() : '', // use expires attribute, max-age is not supported by IE
				options.path    ? '; path=' + options.path : '',
				options.domain  ? '; domain=' + options.domain : '',
				options.secure  ? '; secure' : ''
			].join(''));
		}

		// read
		var decode = config.raw ? raw : decoded;
		var cookies = document.cookie.split('; ');
		var result = key ? undefined : {};
		for (var i = 0, l = cookies.length; i < l; i++) {
			var parts = cookies[i].split('=');
			var name = decode(parts.shift());
			var cookie = decode(parts.join('='));

			if (key && key === name) {
				result = converted(cookie);
				break;
			}

			if (!key) {
				result[name] = converted(cookie);
			}
		}

		return result;
	};

	config.defaults = {};

	$.removeCookie = function (key, options) {
		if ($.cookie(key) !== undefined) {
			$.cookie(key, '', $.extend(options, { expires: -1 }));
			return true;
		}
		return false;
	};

}));

/* jshint maxstatements: 50 */
define('asa/ASAUtilities',[
    'jquery',
    'dust',
    'underscore',
    'salt',
    'configuration',
    'jquery.cookie'
], function ($, dust, _, SALT, Configuration) {
    //See the following for the structure of a sort comparison function
    //https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Global_Objects/Array/sort
    function lastModifiedComparer(a, b) {
        //a, the current item is more recent, dont swap
        if (a.attributes.LastModified < b.attributes.LastModified) {
            return 1;
        }
        //the item we are comparing with, b, is more recent, swap the order
        if (a.attributes.LastModified > b.attributes.LastModified) {
            return -1;
        }
        return 0;
    }

    function buildEndecaIgnoreString(tags) {
        //Changes the format we get from configuration: "budgeting,banking,credit,taxes,insurance"
        //into the format the Endeca "Nr" parameter expects: 'NOT(Tags:budgeting),NOT(Tags:banking),NOT(Tags:credit),NOT(Tags:taxes),NOT(Tags:insurance),'
        return tags.split(',').map(function (tag) {
            return 'NOT(Tags:' + tag + '),';
        }).join('');
    }

    var terms = [],
        utility = {
        SOURCE_IMPORTED_REP_NAV : 1,
        SOURCE_SELF_REPORTED_REP_NAV : 2,
        SOURCE_IMPORTED_KWYO : 3,
        SOURCE_SELF_REPORTED_KWYO : 4,
        itemsAlreadyRendered : {
            MySALT: {},
            RepayStudentDebt: {},
            MasterMoney: {},
            PayForSchool: {},
            FindAJob: {}
        },
        paramToGoalInfo : {
            MM: {
                baseUrl: Configuration.apiEndpointBases.GenericEndeca + Configuration.apiEndpointBases.MasterMoney,
                url: '',
                name: 'MasterMoney',
                tags: buildEndecaIgnoreString(Configuration.goalTags.MasterMoney),
                paramName: 'MM'
            },
            RSD: {
                baseUrl: Configuration.apiEndpointBases.GenericEndeca + Configuration.apiEndpointBases.RepayStudentDebt,
                url: '',
                name: 'RepayStudentDebt',
                tags: buildEndecaIgnoreString(Configuration.goalTags.RepayStudentDebt),
                paramName: 'RSD'
            },
            FJ: {
                baseUrl: Configuration.apiEndpointBases.GenericEndeca + Configuration.apiEndpointBases.FindAJob,
                url: '',
                name: 'FindAJob',
                tags: buildEndecaIgnoreString(Configuration.goalTags.FindAJob),
                paramName: 'FJ'
            },
            PS: {
                baseUrl: Configuration.apiEndpointBases.GenericEndeca + Configuration.apiEndpointBases.PayForSchool,
                url: '',
                name: 'PayForSchool',
                tags: buildEndecaIgnoreString(Configuration.goalTags.PayForSchool),
                paramName: 'PS'
            }
        },
        //////////////////////////////////////////////////////////////////////////
        //  WCF Data Formatting Utilities
        //////////////////////////////////////////////////////////////////////////
        convertFromJsonDate: function (input) {
            var convertedDate = input;

            if (input && typeof input === 'string' && input.indexOf('\/Date') !== -1) //if input is a JSON formatted Date string
            {
                var substringedDate = input.substring(6); //substringedDate= 1291548407008)/
                var parsedIntDate = parseInt(substringedDate, 10); //parsedIntDate= 1291548407008
                var dt = new Date(parsedIntDate);  // parsedIntDate passed to date constructor

                convertedDate = dt.getMonth() + 1 + '/' + dt.getDate() + '/' + dt.getFullYear();
            }

            return convertedDate;
        },
        formatDate: function (dateObj) {
            var monthNames = ['January', 'February', 'March', 'April', 'May', 'June', 'July', 'August', 'September', 'October', 'November', 'December'];
            return monthNames[dateObj.getMonth()] + ' ' + dateObj.getDate() + ', ' + dateObj.getFullYear();
        },

        /////////////////////////////////////////////////////////
        // getParameterByName
        // Parse URL QueryString for parameters (cmak)
        /////////////////////////////////////////////////////////
        getParameterByName: function (name) {
            name = name.replace(/[\[]/, '\\[').replace(/[\]]/, '\\]');
            var regexS = '[\\?&]' + name + '=([^&#]*)';
            var regex = new RegExp(regexS);
            var results = regex.exec(window.location.href.replace('&amp;', '&'));
            if (!results) {
                return '';
            } else {
                return decodeURIComponent(results[1].replace(/\+/g, ' '));
            }
        },
        getParameterByNameFromString: function (name, querystring) {
            if (querystring && querystring.indexOf('?') === -1) {
                querystring = '?' + querystring;
            }
            name = name.replace(/[\[]/, '\\[').replace(/[\]]/, '\\]');
            var regex = new RegExp('[\\?&]' + name + '=([^&#]*)'),
                results = regex.exec(querystring);
            return !results ? '' : decodeURIComponent(results[1]);
        },
        getParamStringFromURL: function (param, searchScope) {
            return param + '=' + this.getParameterByNameFromString(param, searchScope);
        },
        getLocationHash: function (linkString, isIE) {
            if (isIE && linkString.hash) {
                var hashString = linkString.hash,
                pathString = linkString.pathname;
                if (hashString.indexOf(pathString) === 1) {
                    hashString = hashString.replace(pathString, '');
                }
                return hashString.replace('#', '').replace('/', '');
            }
            return linkString.search;
        },

        getPathnameHash: function (linkString, isIE) {
            if (isIE && linkString.hash) {
                var a = document.createElement('a');
                a.href = linkString.hash.replace('#', '');
                return a.pathname;
            }
            return linkString.pathname;
        },
        getHostname: function (url) {
            var hostname;
            //find & remove protocol (http, ftp, etc.) and get hostname

            if (url.indexOf("://") > -1) {
                hostname = url.split('/')[2];
            }
            else {
                hostname = url.split('/')[0];
            }

            //find & remove port number
            hostname = hostname.split(':')[0];
            //find & remove "?"
            hostname = hostname.split('?')[0];

            return hostname;
        },
        getCopyrightYear: function () {
            var date = new Date();
            document.getElementById('copyrightyear').innerHTML = date.getFullYear();
        },

        convertDate: function (date) {
            var jsDate = utility.convertSALtoJSdate(date);
            var yyyy = jsDate.getFullYear().toString();
            var mm = (jsDate.getMonth() + 1).toString(); // getMonth() is zero-based
            var dd  = jsDate.getDate().toString();
            // return YYYY-MM-DD and make sure MM and DD are in the double digit format
            return yyyy + '-' + (mm[1] ? mm : '0' + mm[0]) + '-' + (dd[1] ? dd : '0' + dd[0]);
        },
        convertSALtoJSdate: function (dateString) {
            return new Date(parseInt(dateString.substr(6), 10));
        },

        deepLinkToCourse: function (coursePath) {
            //This implementation is based on Remote Learner's recommendation
            var courseLink = 'https://' + utility.getHostname(Configuration.mm101.url);
            if (coursePath) {
                courseLink = courseLink + coursePath.trim();
            }
            //update js-Courses-form action url, with Courses url
            $('#js-Courses-form').attr('action', Configuration.mm101.url);
            document.forms['js-Courses-form'].wantsurl.value = courseLink;
            document.forms['js-Courses-form'].submit();
        },

        // TODO: Find a better wat to scroll to top when needed isntead of using scrollToTop flag
        renderDustTemplate: function (tmplName, data, cbk, element, scrollToTop) {
            //If no callback was passed set it to a no-op
            if (!_.isFunction(cbk)) {
                cbk = function () {};
            }
            require(['Compiled/' + tmplName], function () {
                dust.render(tmplName, data, function (err, out) {
                    if (err) {
                        console.error('Rendering failed: ', err);
                    }
                    if (element) {
                        $(element).html(out);
                    }
                    if (scrollToTop) {
                        $(window).scrollTop(0);
                    }
                    cbk(err, out);
                });
            });
        },
        getLocationSearch: function () {
            if ($.browser.msie) {
                return utility.getLocationHash(location, true);
            } else {
                return utility.getLocationHash(location, false);
            }
        },
        getlocationPath: function () {
            if ($.browser.msie) {
                return utility.getPathnameHash(location, true);
            } else {
                return utility.getPathnameHash(location, false);
            }
        },
        handleUserConfiguration: function (apiUrl, userSegment) {
            if (userSegment) {
                apiUrl = this.updateQueryString(apiUrl, 'Endeca_user_segments', userSegment);
            }
            return apiUrl;
        },
        getDimensionString: function (dimsArray) {
            //On search page, definitions should always be part of the contents
            if (location.href.indexOf('searchCriteria') > -1) {
                dimsArray.push('159');
            }
            return dimsArray.join(',');
        },
        updateQueryString: function (url, param, paramVal) {
            if (typeof param === 'object') {
                url = this.updateQueryStringByObject(url, param);
            } else {
                url = this.updateQueryStringByString(url, param, paramVal);
            }
            return url;
        },
        updateQueryStringByString: function (currentURL, paramName, paramValue) {
            var url = currentURL || '';
            paramName += '=';
            if (url.indexOf(paramName) >= 0) {
                var prefix = url.substring(0, url.indexOf(paramName));
                var suffix = url.substring(url.indexOf(paramName));
                suffix = suffix.substring(suffix.indexOf('=') + 1);
                suffix = (suffix.indexOf('&') >= 0) ? suffix.substring(suffix.indexOf('&')) : '';
                url = prefix + paramName + paramValue + suffix;
            } else {
                if (url.indexOf('?') < 0) {
                    url += '?' + paramName + paramValue;
                } else {
                    url += '&' + paramName + paramValue;
                }
            }
            return url;
        },
        updateQueryStringByObject: function (url, paramObject) {
            var _this = this;
            _.each(_.keys(paramObject), function (paramName) {
                url = _this.updateQueryStringByString(url, paramName, paramObject[paramName]);
            });
            return url;
        },
        popMessage: function (element, time) {
            time = time || 2000;
            element.fadeIn();
            setTimeout(function () {
                element.fadeOut();
            }, time);
        },
        clearURLParams: function (url, paramArray) {
            var _this = this;
            _.each(paramArray, function (paramName) {
                url = _this.updateQueryStringByString(url, paramName, '');
            });
            return url;
        },
        SetSortBarParameterForURL: function (apiUrl, querystring) {
            apiUrl = this.updateQueryStringByObject(apiUrl, {
                'Ns': this.getParameterByNameFromString('Ns', querystring),
                'Dims': this.getParameterByNameFromString('Dims', querystring),
                'Type': this.getParameterByNameFromString('Type', querystring)
            });
            return apiUrl;
        },
        CheckAgeValidity: function (selectedYear) {
            selectedYear = parseInt(selectedYear, 10);
            if (new Date().getFullYear() - selectedYear <= 13) {
                return false;
            }
            return true;
        },
        waitForAsyncScript: function (name, callback) {
            var interval = 500,
                timeWaited = 0;
            function loadScripts() {
                if (timeWaited < 10000) {
                    //if window[name] is undefined we want to run this function again, but 0 will also be evaluated as falsy.
                    //So if the value is 0 that means this window[name] has been populated, we wanna trigger callback() in this case.
                    if (!window[name] && window[name] !== 0) {
                        setTimeout(function () {
                            timeWaited += interval;
                            loadScripts();
                        }, interval);
                    } else {
                        callback();
                    }
                } else {
                    console.error('Timed out on loading: ' + name);
                    return;
                }
            }
            loadScripts(interval);
        },
        setUrlToHideRecords: function (apiUrl, siteMember, excludeDefs) {
            var beginPart = 'AND(NOT(P_Primary_Key:',
                primaryKeysToExclude = utility.checkContentParticipation(siteMember),
                lastPart = excludeDefs ? '),NOT(ContentTypes:Definition))' : '))';

            if (primaryKeysToExclude.length > 0) {
                return this.updateQueryString(apiUrl, 'HideRecord', beginPart + primaryKeysToExclude.join('),NOT(P_Primary_Key:') + lastPart);
            } else {
                if (excludeDefs) {
                    apiUrl += '&HideRecord=NOT(ContentTypes:Definition)';
                }
                return apiUrl;
            }
        },
        isSubscribedToProduct: function (siteMember, productId) {
            var orgSubscribed = _.findWhere(siteMember.OrganizationProducts, {ProductID: productId, IsOrgProductActive: true});
            var memberSubscribed = _.findWhere(siteMember.Products, {RefProductID: productId, IsMemberProductActive: true});

            if (orgSubscribed || memberSubscribed) {
                return true;
            }
            return false;
        },
        isMemberSubscribedToProduct: function (siteMember, productId) {
            var memberSubscribed = _.findWhere(siteMember.Products, {RefProductID: productId, IsMemberProductActive: true});

            if (memberSubscribed) {
                return true;
            }
            return false;
        },
        isRoleActive: function (siteMember, roleId) {
            var roleActive = _.findWhere(siteMember.Roles, {RoleId: roleId, IsMemberRoleActive: true});

            if (roleActive) {
                return true;
            }
            return false;
        },
        checkContentParticipation: function (siteMember) {
            var primaryKeysToExclude = [];
            if (siteMember.IsAuthenticated === 'true') {
                primaryKeysToExclude = ['101-7416', '101-23826'];

                //if one organization praticipates they all get it.
                if (this.isSubscribedToProduct(siteMember, 4)) {
                    //If they are opted into ScholarshipSearch remove that primary key from the array to exclude
                    primaryKeysToExclude.splice($.inArray('101-7416', primaryKeysToExclude), 1);
                }
                if (this.isSubscribedToProduct(siteMember, 7)) {
                    //If they are opted into CCPTool remove that primary key from the array to exclude
                    primaryKeysToExclude.splice($.inArray('101-23826', primaryKeysToExclude), 1);
                }
            }
            return primaryKeysToExclude;
        },
        instantiateGlossaryTerms: function () {
            require(['configuration', 'jquery.zglossary'], function (Configuration) {
            // Build a JSON object with all definitions, to be used with the glossary jquery plugin.
                if (!terms.length) {
                    $.getJSON(Configuration.apiEndpointBases.GenericEndeca + 'glossary', function (data) {
                        _.each(data.secondaryContent[0].records, function (record) {
                            terms.push({
                                'term': record.attributes.term[0],
                                'definition': record.attributes.body[0]
                            });
                        });
                        $('.js-glossary-scope').glossary(terms);
                        SALT.trigger('glossary:fired');
                    });
                } else {
                    $('.js-glossary-scope').glossary(terms);
                    SALT.trigger('glossary:fired');
                }
            });
        },
        IsCurrentPage: function (page) {
            return (location.pathname.toLowerCase().indexOf(page ? page.toLowerCase() : '') === 0);
        },
        IsReferrerByCookie: function (ref) {
            if ($.cookie('referUrl')) {
                return ($.cookie('referUrl').indexOf(ref) !== -1);
            }
            return false;
        },
        IsReferrerByUrl: function (ref) {
            var referrer = document.referrer.toLowerCase();
            if (referrer) {
                return (referrer.indexOf(ref) >= 0);
            }
            return false;
        },
        trackLastVisitedURL: function () {
            if (this.IsReferrerByUrl('saltmoney') && !this.IsReferrerByUrl('logon') &&
                !this.IsReferrerByUrl('register') && !$.cookie('referUrl')) {
                $.cookie('referUrl', document.referrer, {path: '/', domain: '.saltmoney.org'});
            }
        },
        lastvisitedURL: function () {
            var url = $.cookie('referUrl');
            if (!url) {
                /* when empty return default */
                url = '/index.html';
            }

            /* SWD-7345: special events routing */
            return url;
        },
        getRerouteURL: function () {
            if (this.IsCurrentPage('/logon') || 
                this.IsCurrentPage('/register')) {
                return this.lastvisitedURL();
            } else {
                return this.getParameterByName('ReturnUrl');
            }
        },
        // checks the validity of deeply-nested object properties via dot
        // based on https://github.com/wilmoore/selectn
        checkNested: function (obj, query) {
            // normalize query to `.property` access (i.e. `a.b[0]` becomes `a.b.0`)
            query = query.replace(/\[(\d+)\]/g, '.$1');
            var parts = query.split('.'),
                len = parts.length;

            // iteratively save each segment's reference
            for (var i = 0; i < len; i++) {
                if (obj) {
                    obj = obj[parts[i]];
                }
            }
            return obj;
        },
        // convert string to title case, where each the first letter after a space is capitalized
        toTitleCase: function (toCase) {
            var titleCased;
            titleCased = toCase.toLowerCase().replace(/^(.)|\s(.)/g, function ($1) {
                return $1.toUpperCase();
            });

            return titleCased;
        },
        handleRegWallWT: function (action, removeCookie) {

            if ($('.js-reg-wall').is(':visible')) {
                if (!($.cookie('regWall'))) {
                    $.cookie('regWall', true);
                    action = 'Wall_Popup';
                }
            }

            if ($.cookie('regWall')) {
                SALT.publish('regWall', {
                    action: action
                });
            }
        },
        //updates target selector text from matching targetVal to new replaceVal text
        updateSelectorText: function (targetVal, replaceVal) {
            if (typeof targetVal === 'string' && typeof replaceVal === 'string') {
                $('.target-text').text(function () {
                    return $(this).text().replace(targetVal, replaceVal);
                });
            }
        },
        currencyComma: function (value) {
            return value.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ',');
        },
        // If you update this, you should also update the function of the same name in saltUtils.js
        sortOrganizations: function (organizationsIn) {

            if (organizationsIn && (typeof organizationsIn === 'object')) {
                var sortOrgs = organizationsIn.sort(function (a, b) {
                    return b.EffectiveStartDate.localeCompare(a.EffectiveStartDate);
                });
                return sortOrgs;
            }
            else {
                return organizationsIn;
            }
        },
        // If you update this, you should also update the function of the same name in saltUtils.js
        returnBrandedOrganizations: function (organizationsIn) {
            if (organizationsIn && (typeof organizationsIn === 'object')) {

                var logoOrgs = organizationsIn.filter(function (el) {
                    return (el.Brand !== 'nologo');
                });
                return logoOrgs;
            }
            else {
                return organizationsIn;
            }
        },
        isScrolledIntoView: function ($elem) {
            var $window = $(window);

            var docViewTop = $window.scrollTop();
            var docViewBottom = docViewTop + $window.height();

            var elemTop = $elem.offset().top;
            var elemBottom = elemTop + $elem.height();

            return ((elemBottom <= docViewBottom) && (elemTop >= docViewTop));
        },
        parseQuestionsAnswers: function (data) {
            // building a JSON object to pass to the backbone views to use when rendering dust
            var questionsAnswers = {},
                responses = {};
            // for each question in data...
            _.each(data.questions, function (question) {
                // from the questions object, build an object with QuestionID as the key, and the value is an object the contains:
                // Question = QuestionDescription(question text)
                // Answers = all available answers (options) for this question

                // If we've already concatenated the question and answer IDs, don't try to do it again.
                if (question.Answers && question.Answers.length > 0 && !isNaN(question.Answers[0].AnsID)) {
                    _.each(question.Answers, function (answer) {
                        answer.AnsID = question.QuestionID + '-' + answer.AnsID;
                    });
                }

                questionsAnswers[question.QuestionID] = { Question: question.QuestionDescription, Answers: question.Answers, QuestionID: question.QuestionID };
                // from the responses object, build an object with all user responses to the current question
                responses = _.where(this.responses, { QuestionExternalId: question.QuestionID });
                questionsAnswers[question.QuestionID].Responses = responses;
                // check if the question has a custom value stored, and add it to the question object at the base level for ease of access.
                var customValue = _.find(responses, function (response) {
                    return response.CustomValue;
                });
                questionsAnswers[question.QuestionID].otherAnswer = customValue ? customValue.CustomValue : null;
            }, data);
            return questionsAnswers;
        },
        setMemberResponses: function (formData) {
            var formAnswers = _.pairs(formData),
                profQandA = [];

            // pick the question we want to add to Sitemember.ProfialQsAndAs
            _.each(formAnswers, function (element) {
                // element is an array of size 2. [0] is the element's name, [1] is the element's value which is the unique ID of the answer.
                var currentValue = element[1];
                // element[0] (element ID) consists of 3 parts separated by "-":
                // 1) question type (multi answer, single asnwer)
                // 2) the string 'qid' by which we could tell if this question is in the RefProfileQuestion table or not
                // 3) the questions's unique ID in the RefProfileQuestion table.
                if (element[0].split('-')[1] === 'qid') {
                    // if an "other" question was asnwered the custom value is added to the element's value, read it, otherwise set it to null
                    var customVal = null;
                    if (element[0].split('-')[0] !== 'multi' || typeof currentValue === 'string') {
                        customVal = currentValue.split('-')[2] || null;
                        profQandA.push({ AnsExternalId: parseInt(currentValue.split('-')[1], 10), QuestionExternalId: parseInt(element[0].split('-')[2], 10), CustomValue: customVal });
                    } else {
                        _.each(currentValue, function (currentAnswerID) {
                            customVal = currentAnswerID.split('-')[2] || null;
                            profQandA.push({ AnsExternalId: parseInt(currentAnswerID.split('-')[1], 10), QuestionExternalId: parseInt(element[0].split('-')[2], 10), CustomValue: customVal });
                        });
                    }
                }
            });

            SALT.trigger('set:ProfileQandAs', profQandA);
            return profQandA;
        },
        prepareDashboardContent: function (json, goalRankResponses) {
            var potentialRecommendedTasks = {};
            var potentialLibraryTasks = {};
            var featuredTasks = {};
            var qAndARecords = {};
            var contentPrimarySelections = {};
            var contentSecondarySelections = {};
            var topicsSelectedPrimary = {};
            var topicsSelectedSecondary = {};

            var hasQAresponses = false;
            var hasTopicSelections = false;

            //We need to empty the already rendered objects for the MySALT section, as we might be rendering within an SPA session
            //And dont want to ignore duplicates from the previous sort/filter state
            utility.itemsAlreadyRendered.MySALT = {};

            json.goalRankResponses = goalRankResponses;

            //TODO, probably need a better data structure for this than simply adding properties to an object in order, we cant count on all browsers keeping that order
            //Add empty arrays for each goal, in the order the goals were ranked, so that when we combine these later, they are in order we will render them on the page
            _.each(goalRankResponses, function (response, ind, arr) {
                var goalWithNoWhitespace = response.AnsName.replace(new RegExp(' ', 'g'), '');
                qAndARecords[goalWithNoWhitespace] = [];
                //We need to empty the already rendered objects for the browse sections, as we might be rendering browse from an SPA session
                //And dont want to ignore duplicates from the previous sort/filter state
                utility.itemsAlreadyRendered[goalWithNoWhitespace] = {};
                potentialRecommendedTasks[goalWithNoWhitespace] = [];
                contentSecondarySelections[goalWithNoWhitespace] = [];
                contentPrimarySelections[goalWithNoWhitespace] = [];
                potentialLibraryTasks[goalWithNoWhitespace] = [];
                topicsSelectedPrimary[goalWithNoWhitespace] = [];
                topicsSelectedSecondary[goalWithNoWhitespace] = [];
            });

            //Populate featured tasks
            _.each(json.secondaryContent[0].contents, function (obj, ind, arr) {
                if (json.SiteMember.enabledGoals[obj.title]) {
                    featuredTasks[obj.title] = obj.records;
                }
            });

            //This cartridge contains organization specific featured task items.
            _.each(json.secondaryContent[7].contents, function (obj, ind, arr) {
                if (json.SiteMember.enabledGoals[obj.title]) {
                    //Put the org specific records first when combining
                    featuredTasks[obj.title] = obj.records.concat(featuredTasks[obj.title]);
                }
            });

            //Populate q+a tasks
            _.each(json.secondaryContent[1].contents, function (obj, ind, arr) {
                if (json.SiteMember.enabledGoals[obj.title] && obj.records.length) {
                    //The contents array would be empty and we wouldnt get into this function if there were no QA responses
                    //set the bool to be used in determining which scenario we are in
                    hasQAresponses = true;
                    //TODO implement a concat here rather than pushing just one record, in case any answers in the future link to multiple records rather than one
                    qAndARecords[obj.title].push(obj.records[0]);
                }
            });

            //Populate content selection secondary items
            _.each(json.secondaryContent[2].contents, function (obj, ind, arr) {
                if (json.SiteMember.enabledGoals[obj.title]) {
                    contentSecondarySelections[obj.title] = obj.records;
                }
            });

            //Populate content selection primary items
            _.each(json.secondaryContent[3].contents, function (obj, ind, arr) {
                if (json.SiteMember.enabledGoals[obj.title]) {
                    contentPrimarySelections[obj.title] = obj.records;
                }
            });

            //Populate topic selection primary items
            _.each(json.secondaryContent[4].contents, function (obj, ind, arr) {
                if (json.SiteMember.enabledGoals[obj.title] && obj.records.length) {
                    //The contents array would be empty and we wouldnt get into this function if there were no topics
                    //set the bool to be used in determining which scenario we are in
                    hasTopicSelections = true;
                    //TODO implement a concat here rather than pushing just one record, in case any answers in the future link to multiple records rather than one
                    topicsSelectedPrimary[obj.title].push(obj.records[0]);
                }
            });

            //Populate topic selection secondary items
            _.each(json.secondaryContent[5].contents, function (obj, ind, arr) {
                if (json.SiteMember.enabledGoals[obj.title] && obj.records.length) {
                    //TODO implement a concat here rather than pushing just one record, in case any answers in the future link to multiple records rather than one
                    topicsSelectedSecondary[obj.title].push(obj.records[0]);
                }
            });

            if (hasQAresponses && !hasTopicSelections) {
                //Loop through one of our goal rank ordered objects, using the property name parameter to combine the necessary cartridge data according to goal
                _.each(featuredTasks, function (goal, property, obj) {
                    potentialRecommendedTasks[property] = featuredTasks[property].concat(qAndARecords[property]);
                    potentialLibraryTasks[property] = qAndARecords[property].concat(contentSecondarySelections[property]);
                });

                //Transform an object like { MasterMoney: [1,2], FindAJob: [3,4]} into a flat array like [1,2,3,4] so that our dust template can simply loop through the array and render records as tiles
                potentialRecommendedTasks = _.flatten(_.values(potentialRecommendedTasks));
                //Add the ordered secondarySelections to the end of the recommended section
                //See the comment a couple lines above for what the .flatten & .values calls are doing
                potentialRecommendedTasks = potentialRecommendedTasks.concat(_.flatten(_.values(contentSecondarySelections)));
            } else if (!hasQAresponses && !hasTopicSelections) {
                _.each(featuredTasks, function (goal, property, obj) {
                    potentialRecommendedTasks[property] = featuredTasks[property].concat(contentPrimarySelections[property]);
                    potentialLibraryTasks[property] = contentPrimarySelections[property].concat(contentSecondarySelections[property]);
                });
                //Transform an object like { MasterMoney: [1,2], FindAJob: [3,4]} into a flat array like [1,2,3,4] so that our dust template can simply loop through the array and render records as tiles
                potentialRecommendedTasks = _.flatten(_.values(potentialRecommendedTasks));
                //Add the ordered secondarySelections to the end of the recommended section
                //See the comment a couple lines above for what the .flatten & .values calls are doing
                potentialRecommendedTasks = potentialRecommendedTasks.concat(_.flatten(_.values(contentSecondarySelections)));
            } else if (!hasQAresponses && hasTopicSelections) {
                _.each(featuredTasks, function (goal, property, obj) {
                    potentialRecommendedTasks[property] = featuredTasks[property].concat(topicsSelectedPrimary[property]).concat(topicsSelectedSecondary[property]);

                    //For goals where the user didnt select any topics, use the contentPrimarySelections list
                    if (!topicsSelectedPrimary[property].length) {
                        potentialRecommendedTasks[property] = potentialRecommendedTasks[property].concat(contentPrimarySelections[property]);
                    }

                    potentialLibraryTasks[property] = topicsSelectedPrimary[property].concat(topicsSelectedSecondary[property]);
                });

                potentialRecommendedTasks = _.flatten(_.values(potentialRecommendedTasks));
            } else {
                _.each(featuredTasks, function (goal, property, obj) {
                    potentialRecommendedTasks[property] = featuredTasks[property].concat(qAndARecords[property]).concat(topicsSelectedPrimary[property]).concat(topicsSelectedSecondary[property]);
                    potentialLibraryTasks[property] = qAndARecords[property].concat(topicsSelectedPrimary[property]).concat(topicsSelectedSecondary[property]);
                });

                potentialRecommendedTasks = _.flatten(_.values(potentialRecommendedTasks));
            }

            //Get rid of any added, complete, inProgress or duplicates items, they won't be shown on MySALT
            var incompletePotentialTasks = utility.removeItems(potentialRecommendedTasks);

            //Populate the two sections of the To-do/tasks tab (incomplete and complete are the two sections)
            json.openTasks = _.filter(json.mainContent[0].records, function (record) {
                return record.attributes.RefToDoStatusID === 1 || record.attributes.RefToDoStatusID === 4;
            });
            //Sort the open tasks by date, most recent at the top
            json.openTasks = json.openTasks.sort(lastModifiedComparer);
            //count the open todo tasks. If tasks more than 0 apply class active counter else null.
            json.openTodoCount = json.openTasks.length;
            json.openTodoCountClass = json.openTodoCount ? 'active-counter' : '';

            json.completedTasks = _.filter(json.mainContent[0].records, function (record) {
                return record.attributes.RefToDoStatusID === 2;
            });

            //Sort the completed tasks by date, most recent at the top
            json.completedTasks = json.completedTasks.sort(lastModifiedComparer);

            json.libraryTasks = potentialLibraryTasks;

            //Loop through the potential MySALT tasks, finding the index of the first item that is not in progress
            var firstMatchIndex = 0;
            _.find(incompletePotentialTasks, function (obj) {
                if (obj.attributes.RefToDoStatusID !== 4) {
                    return true;
                }
                firstMatchIndex++;
                return false;
            });
            //Use the index found above to splice that record out of the array and set it as the featured task
            json.featuredTask = incompletePotentialTasks.splice(firstMatchIndex, 1)[0];
            //The remaining "incompletePotentialTasks" object becomes the "Recommended" list on MySALT
            json.recommendedTasks = incompletePotentialTasks;

            //Populate the map object for the MySALT/Recommended page, which contains all of the primary keys already shown
            //so that we dont render duplicates when load more is clicked
            _.each(json.recommendedTasks, function (obj, ind, arr) {
                utility.itemsAlreadyRendered.MySALT[obj.attributes.P_Primary_Key[0]] = true;
            });
            if (json.featuredTask !== undefined) {
                utility.itemsAlreadyRendered.MySALT[json.featuredTask.attributes.P_Primary_Key[0]] = true;   
            }
            //Populate the map object for each of the browse page sections, which contains all of the primary keys already shown, and remove duplicates within a section
            _.each(json.libraryTasks, function (goal, goalName, libraryTasks) {
                //Using _.filter to filter out records that are duplicates
                goal = _.filter(goal, function (task) {
                    if (utility.itemsAlreadyRendered[goalName][task.attributes.P_Primary_Key[0]]) {
                        //We've already added this item to this goal/section, return false so that this copy of the record is not added to the list (we don't want duplicates)
                        return false;
                    } else {
                        //Not in the map yet, its a new item, add the primary key to the list of items we have already seen and return true so that we dont filter out this record
                        utility.itemsAlreadyRendered[goalName][task.attributes.P_Primary_Key[0]] = true;
                        return true;
                    }
                });
                //"goal" is a copy of the array that we want to be operating on, set the value to libraryTasks, which is a reference to the json.libraryTasks object
                libraryTasks[goalName] = goal;
            });
            return json;
        },
        removeItems: function (tasksToFilter) {
            //Map to keep track of the ids in our result set so we can get rid of duplicates
            var primaryKeyMap = {};

            return _.filter(tasksToFilter, function (task) {
                var notADuplicate = false;
                //console.log(task);
                if (!primaryKeyMap[task.attributes.P_Primary_Key[0]]) {
                    notADuplicate = true;
                    //Add it to the map so that we will ignore any future instances of this item
                    primaryKeyMap[task.attributes.P_Primary_Key[0]] = true;
                }
                //exclude Added, Complete and InProgress items
                var excludeStatusId = task.attributes.RefToDoStatusID === 1 || task.attributes.RefToDoStatusID === 2 || task.attributes.RefToDoStatusID === 4;
                return !excludeStatusId && notADuplicate;
            });
        },
        topScroll: function () {
            if (document.body.scrollHeight > 200) {
                $('html, body').animate({ scrollTop: 0 }, 500);
            }
        },
        lookupUserSegment: function (data, userSegmentValue, org) {
            //Default to using brand name as the path lookup in the data
            var keyPath = org.Brand;
            if (keyPath === 'nologo') {
                //If this org is branded no-logo it still may be using user segments.  Try OrgName as the key rather than brand.
                keyPath = org.OrganizationName;
            }
            if (data[keyPath]) {
                if (userSegmentValue) {
                    userSegmentValue += '|' + data[keyPath];
                } else {
                    userSegmentValue = 'UserSegment=' + data[keyPath];
                }
            }

            return userSegmentValue;
        },
        findUserSegment: function (responseObj) {
            $.getJSON('/Assets/Scripts/js/salt/userSegment.json')
                .done(function (json) {
                    var userSegmentValue;

                    _.each(responseObj.Organizations, function (org, ind, list) {
                        userSegmentValue = utility.lookupUserSegment(json, userSegmentValue, org);
                    });

                    if (userSegmentValue) {
                        document.cookie = userSegmentValue + '; PATH=/';
                    } else {
                        document.cookie = 'UserSegment=Default; PATH=/';
                    }
                })
                .fail(function (jqxhr, textStatus, error) {
                    var err = textStatus + ', ' + error;
                });
        },
        getInitialOrder: function (obj) {
            var num = 1;
            $(obj).each(function () {
                //Set the value of the input to its position in the array, so the first label gets "1", second "2" and so on.
                $(this).find('input').val(num);

                //We need to be able to tie the ranking buttons with how they have been ranked for automated testing
                //Set the data-externalid property from the ranking button onto the ranking label
                var $goalButtonAtThisIndex = $('.js-goal-rank').eq(num - 1);
                if ($goalButtonAtThisIndex.length) {
                    $(this).find('input').attr('data-externalid', $goalButtonAtThisIndex.attr('data-externalid'));
                }
                num++;
            });
        }
    };
    return utility;
});

define('asa/ASAWebService',[
    'jquery',
    'salt',
    'configuration'
], function ($, SALT, configuration) {
    // Make sure SALT.services is only defined once.
    if (!SALT.services) {
        SALT.services = {
            //////////////////////////////////////////////////////////////////////////
            // BASE FUNCTIONALITY
            //////////////////////////////////////////////////////////////////////////
            asaConfig: null,

            ASAServiceCall: function (callType, endpointBase, endptOperation, successMethod, errorMethod, jsonData, ctx, contenttype, datatype, cacheValue, asyncValue) {

                if (!ctx) {
                    ctx = this;
                }

                if (!cacheValue) {
                    cacheValue = true;
                }

                if (!asyncValue) {
                    asyncValue = true;
                }

                jQuery.support.cors = true;

                var endpoint = endpointBase + endptOperation;

                if (!contenttype) {
                    contenttype = 'application/json; charset=utf-8';
                }
                if (!datatype) {
                    datatype = 'JSON';
                }

                $.ajax({
                    type: callType,
                    contentType: contenttype,
                    context: ctx,
                    url: endpoint,
                    cache: cacheValue,
                    data: jsonData,
                    dataType: datatype,
                    async: asyncValue,
                    success: function (data, textStatus, jqXHR) {
                        var self = this,
                            args = arguments;

                        require(['salt/global'], function () {
                            //check that they exist first before trying to clear/reset.
                            if (successMethod) {
                                successMethod.apply(self, args); //where 'this' refers to the 'ctx' object passed-in
                            }
                        });
                    },
                    error: function (data, textStatus, jqXHR) {
                        var self = this,
                            args = arguments;

                        require(['salt/global'], function () {
                            //check that they exist first before trying to clear/reset.
                            errorMethod.apply(self, args); //where 'this' refers to the 'ctx' object passed-in
                        });
                    },
                    jsonp: null,
                    jsonpCallback: null
                });
            },

            GetMemberByIndividual: function (successCallback, errorCallback, ctx) {
                SALT.services.ASAServiceCall('GET', configuration.apiEndpointBases.MemberService, '/GetMember/Individual', successCallback, errorCallback, '{}', ctx, null, null, false, true);
            },

            // PUT RegisterMember
            RegisterMember: function (formData, successCallback, errorCallback, ctx, cache, async) {
                formData.Emails = [{
                    EmailAddress : formData.EmailAddress,
                    Type : 'Primary',
                    TypeID : 'P'
                }];

                var sessionString = '?Session=true';
                if (formData.SpecialEvent) {
                    sessionString = '?Session=false';
                }

                formData = JSON.stringify(formData);

                SALT.services.ASAServiceCall('PUT', configuration.apiEndpointBases.MemberService, '/Member/' + sessionString, successCallback, errorCallback, formData, ctx, null, null, cache, async);
            },

            DeleteMember: function (memberToDelete, successCbk, errCbk, ctx) {
                SALT.services.ASAServiceCall('DELETE', configuration.apiEndpointBases.MemberService, '/Member/' + memberToDelete.get('MembershipId'), successCbk, function () {}, null, ctx, null, null, false, true);
            },

            LogOff: function (successCbk, errCbk, ctx) {
                SALT.services.ASAServiceCall('POST', '/Account/LogOff', '', '', successCbk, function () {}, '{}', ctx, null, null, false, true);
            },

            // POST AskMe
            AskMe: function (firstName, lastName, subject, emailAddress, yourQuestion, membershipId, successCallback, errorCallback, ctx, cache, async) {

                var askMeRequest = JSON.stringify({
                    MembershipId: membershipId,
                    YourQuestion: yourQuestion,
                    FromEmailAddress: emailAddress,
                    Subject: subject,
                    LastName: lastName,
                    FirstName: firstName
                });

                SALT.services.ASAServiceCall('POST', configuration.apiEndpointBases.AlertServiceEndpoint, '/JustAsk', successCallback, errorCallback, askMeRequest, ctx, null, null, cache, async);

            },
            UpdateMemberOrganizations: function (memberId, organizations, successCallback, errorCallback, ctx) {
                var organizationsJSON = JSON.stringify(organizations);
                SALT.services.ASAServiceCall('PUT', configuration.apiEndpointBases.MemberService + '/Member/' + memberId, '/OrganizationsAffiliation', successCallback, errorCallback, organizationsJSON, ctx, null, null, false, false);
            },
            GetUserSegment: function (userSegmentCookie, successCallback, errorCallback, ctx) {
                SALT.services.ASAServiceCall('GET', configuration.apiEndpointBases.CustomLanding, '/courses?Endeca_user_segments=' + userSegmentCookie, successCallback, errorCallback, null, ctx, null, null, false, false);
            },
            ManageAccount: function (member, successCallback, errorCallback, ctx) {
                var memberJSON = JSON.stringify(member);
                SALT.services.ASAServiceCall('POST', '/Account/ManageAccount/', null, successCallback, errorCallback, memberJSON, ctx, null, null, false, false);
            },
            // POST content feedback Email Us
            contentFeedbackEmail: function (formObject, successCallback, ctx) {
                SALT.services.ASAServiceCall('POST', configuration.apiEndpointBases.AlertServiceEndpoint, '/ContentFeedbackEmail', successCallback, function () {}, formObject, ctx, null, null, false, true);
            },
            GetMemberCourses: function (successCallback, ctx, fromMoodle) {
                fromMoodle = fromMoodle || false; //default to false if not specified
                var sourceString = '?source=' + fromMoodle;
                SALT.services.ASAServiceCall('GET', configuration.apiEndpointBases.MemberService, '/Member/Individual/Courses' + sourceString, successCallback, function () {}, {}, ctx, 'application/json', null, false, true);
            },
            SyncCoursesCompletion: function (successCallback, ctx) {
                SALT.services.ASAServiceCall('POST', configuration.apiEndpointBases.MemberService, '/Member/Individual/SyncCourses', successCallback, function () {}, {}, ctx, 'application/json', null, false, true);
            },
            GetMemberQuestionAnswer: function (memberId, sourceId, successCallback, ctx) {
                SALT.services.ASAServiceCall('GET', configuration.apiEndpointBases.MemberService, '/GetMemberQuestionAnswer/' + memberId + '/' + sourceId, successCallback, function () {}, '', ctx, null, null, false, true);
            },
            UpsertQuestionAnswer: function (successCallback, data, ctx) {
                SALT.services.ASAServiceCall('PUT', configuration.apiEndpointBases.MemberService, '/QuestionAnswerResponse', successCallback, function () {}, data, ctx, null, null, false, true);
            },
            GetScholarshipDetail: function (scholarshipId, successCallback, ctx) {
                SALT.services.ASAServiceCall('GET', configuration.apiEndpointBases.SearchServiceEndpoint + '/Scholarships/', scholarshipId, successCallback, function () {}, '', ctx, 'application/text', null, false, true);
            },
            GetUnigoAnswers: function (successCallback, ctx) {
                SALT.services.ASAServiceCall('GET', '/Scholarships/', 'answers', successCallback, function () {}, '', ctx, 'application/text', null, false, true);
            },
            GetScholarships: function (successCallback, data, ctx) {
                SALT.services.ASAServiceCall('POST', configuration.apiEndpointBases.SearchServiceEndpoint + '/submitChoicesToUnigo', '', successCallback, function () {}, data, ctx, 'application/json', null, false, true);
            },
            SchoolLookup: function (oe, schoolname, successCallback, errorCallback, ctx) {
                var schoolInfoStr = JSON.stringify({
                    oeCode: oe,
                    school: schoolname
                });

                SALT.services.ASAServiceCall('POST', configuration.apiEndpointBases.SearchServiceEndpoint, '', successCallback, errorCallback, schoolInfoStr, ctx, null, null, true, true);
            },
            SetLocation: function (successCallback, data, ctx) {
                SALT.services.ASAServiceCall('PUT', configuration.apiEndpointBases.SearchServiceEndpoint, '/SetLocation', successCallback, function () {}, data, ctx, null, null, false, true);
            },
            PostSurvey: function (surveyID, surveyQuestionID, individualID, response, successCallback, errorCallback, ctx) {
                var surveyInfoStr = JSON.stringify({
                    SurveyId: surveyID,
                    SurveyQuestionId: surveyQuestionID,
                    IndividualId: individualID,
                    Response: response
                });

                SALT.services.ASAServiceCall('POST', configuration.apiEndpointBases.SurveyServiceEndpoint, '/Survey', successCallback, errorCallback, surveyInfoStr, ctx, null, null, true, true);
            },
            getTodos: function (successCallback, ctx) {
                SALT.services.ASAServiceCall('GET', configuration.apiEndpointBases.MemberService, '/Todos', successCallback, function () {}, {}, ctx, 'application/json', null, false, true);
            },
            upsertTodo: function (successCallback, data, ctx) {
                SALT.services.ASAServiceCall('POST', configuration.apiEndpointBases.MemberService, '/Todos', successCallback, function () {}, data, ctx, 'application/json', null, false, true);
            },
            deleteTodo: function (successCallback, data, ctx) {
                SALT.services.ASAServiceCall('DELETE', configuration.apiEndpointBases.MemberService, '/Todos', successCallback, function () {}, data, ctx, 'application/json', null, false, true);
            },
            upsertProfileResponse: function (memberID, data, successCallback, ctx) {
                SALT.services.ASAServiceCall('PUT', configuration.apiEndpointBases.MemberService, '/Member/' + memberID + '/ProfileResponses', successCallback, function () {}, data, ctx, 'application/json', null, false, true);
            }
        };
    }
});

/*!
 * SALT / SiteMember Model
 */
//Declare a global variable outside of the require statement, so that it will stay in scope
//This variable is necessary for the SiteIntercept features to work
var SiteMemberInfo = {};

define('salt/models/SiteMember',[
    'jquery',
    'backbone',
    'underscore',
    'salt',
    'asa/ASAUtilities',
    'asa/ASAWebService',
    'jquery.cookie'
], function ($, Backbone, _, SALT, Utility) {

    var modelDeferred = $.Deferred();

    function getGoalRankResponses(responses, enabledGoals) {
        // lowest question ID must be first in this list
        var goalQuestionIDs = [17, 18, 19, 20];
        var changedEnabledGoals = false;
        //Filter the responses down to answers to the goal ranking questions
        var goalRankResponses = _.filter(responses, function (el) {
            return goalQuestionIDs.indexOf(el.QuestionExternalId) > -1;
        });


        _.each(goalRankResponses, function (gr) {
            gr.nameWithNoSpaces = gr.AnsName.replace(/[\W_]+/g, '');
        });

        //Loop through the enabled goals, if the goal rank responses dont match the enabled goals, they have changed and we'll want to re-onboard
        //Set the "changedEnabledGoals" flag to true in this case
        _.each(enabledGoals, function (isEnabled, goal, list) {
            var responseExists = _.findWhere(goalRankResponses, {nameWithNoSpaces: goal});
            if (isEnabled && !responseExists  || !isEnabled && responseExists) {
                changedEnabledGoals = true;
            }
        });

        if (changedEnabledGoals) {
            return [];
        }

        //We need to filter out responses for goals that are no longer enabled for this user.  The organization may have disabled a goal after the user already ranked it
        //The object that this function returns gets used for building sections of html, we dont want to include disabled goals
        goalRankResponses = _.filter(goalRankResponses, function (goal) {
            if (enabledGoals[goal.nameWithNoSpaces]) {
                return true;
            }
        });

        return goalRankResponses;
    }

    return new (Backbone.Model.extend({
        defaults: {
            ErrorList: [],
            ActiveDirectoryKey: '',
            Addresses: [
                {
                    ErrorList: [],
                    AddressKey: '',
                    AddressLine1: '',
                    AddressLine2: '',
                    City: '',
                    Country: '',
                    CountryID: null,
                    ForeignPostalCode: null,
                    IsPrimary: true,
                    IsValidated: true,
                    State: '',
                    StateID: null,
                    Type: null,
                    TypeID: null,
                    Zip: ''
                }
            ],
            ContactFrequency: false,
            ContactFrequencyKey: null,
            CustomerId: 0,
            DashboardEnabled: null,
            DOB: '',
            Emails: [
                {
                    ErrorList: [],
                    EmailAddress: '',
                    EmailKey: '',
                    IndividualId: null,
                    IsPrimary: true,
                    Type: '',
                    TypeID: null
                }
            ],
            EnrollmentStatus: '',
            EnrollmentStatusEffective: null,
            ExternalId: null,
            FirstName: '',
            GradeLevel: '',
            goalRankResponses: [],
            IndividualId: '',
            IsAuthenticated: 'false',
            LastName: '',
            LegalFirstName: '',
            MemberShipFlag: false,
            MemberToken: '',
            MembershipId: '',
            MembershipStartDate: '',
            MiddleInitial: '',
            PartTimeDate: null,
            PersonId: 0,
            Phones: [
                {
                    ErrorList: [],
                    IsPrimary: true,
                    PhoneKey: '',
                    PhoneNumber: '',
                    Type: '',
                    TypeID: null
                }
            ],
            PrimaryAddressKey: '',
            PrimaryEmailKey: '',
            PrimaryOrganizationKey: '',
            PrimaryPhoneKey: '',
            Products: [
                {
                    MemberID: null,
                    RefProductID: null,
                    MemberProductValue: '',
                    IsMemberProductActive: false
                }
            ],
            Organizations: [
                {
                    ErrorList: [],
                    BranchCode: '',
                    Brand: '',
                    ExpectedGraduationYear: null,
                    EffectiveStartDate: null,
                    OECode: '',
                    SchoolId: '',
                    OrganizationName: '',
                    SchoolType: null,
                    OrganizationId : ''
                }
            ],
            OrganizationProducts: [
                {
                    ProductID: null,
                    IsOrgProductActive: false
                }
            ],
            Source: '',
            WithdrawalDate: null
        },

        idAttribute: 'IndividualId',

        initialize: function () {
            modelDeferred.promise(this);

            this.on('sync', function (model) {
                var obj = model.toJSON();

                if (obj.DashboardEnabled) {
                    //Set some client side only properties (not in SAL model) used by dust templates and front end scripts to include certain features on a per-member basis
                    obj.OnboardingEnabled = Utility.isSubscribedToProduct(obj, 6);
                    obj.BrowseDisabled = !Utility.isSubscribedToProduct(obj, 12);
                    obj.SearchDisabled = !Utility.isSubscribedToProduct(obj, 13);
                }
                obj.ShowMM101 = Utility.isSubscribedToProduct(obj, 2);


                obj.OrgAdmin = Utility.isRoleActive(obj, '3');
                obj.ShowAdminPortal = Utility.isRoleActive(obj, '4');

                modelDeferred.resolve(obj);

                if (obj.FirstName) {
                    SiteMemberInfo = {
                        membershipId: obj.MembershipId,
                        school: obj.Organizations[0],
                        firstName: obj.FirstName,
                        lastName: obj.LastName,
                        email: obj.PrimaryEmailKey,
                        //Contact Frequency boolean needs to be inverted because of some back-end logic where false means contact
                        contactFrequency: !obj.ContactFrequency,
                        yearOfBirth: obj.YearOfBirth,
                    };
                }
                SALT.publish('siteMember:sync', obj);
            });

            this.on('error', function (model, resp) {
                var obj = {
                    message: resp
                };

                modelDeferred.reject(obj);
                console.error(obj.message);
            });

            this.fetch();
        },

        sync: function (method, model, options) {
            var me = this;

            if (method === 'read') {
                //Check for the IndiviualId cookie before trying to make call to back end
                //This cookie is set upon login, if they dont have it, they aren't logged in.
                if ($.cookie('IndividualId')) {
                    SALT.services.GetMemberByIndividual(function (data, textStatus, jqXHR) {
                        var errorMessages = [];

                        if (data.ErrorList.length === 0) {
                            data.IsAuthenticated = 'true';
                            data.enabledGoals = {};
                            data.enabledGoals.RepayStudentDebt = Utility.isSubscribedToProduct(data, 8);
                            data.enabledGoals.FindAJob = Utility.isSubscribedToProduct(data, 9);
                            data.enabledGoals.PayForSchool = Utility.isSubscribedToProduct(data, 10);
                            data.enabledGoals.MasterMoney = Utility.isSubscribedToProduct(data, 11);

                            //update goalRankResponses property
                            var rankResponses = _.last(data.ProfileQAs).Responses;
                            data.goalRankResponses = getGoalRankResponses(rankResponses, data.enabledGoals);
                            options.success(data, textStatus, jqXHR);
                        } else {
                            _.each(data.ErrorList, function (element) {
                                errorMessages.push(element.BusinessMessage);
                            }, this);

                            options.error('Unable to get site member (' + errorMessages.join(', ') + ')');
                        }
                    }, function (jqXHR, textStatus, errorThrown) {
                        options.success(model.defaults, '', {});
                    }, me);
                } else {
                    //We know we are logged out with no IndividualId cookie, set data to unauthenticated
                    options.success(model.defaults, '', {});
                }
            } else {
                options.error('Method unsupported (' + method + ')');
            }
        }
    }))();
});

define('salt/analytics/webtrends-config',[
    'jquery'
], function ($) {

    var wtTags = {
        // Standard
        ONSITE_PROMOTION_CLICK:            'WT.ac',
        ONSITE_PROMOTION_VIEW:             'WT.ad',
        CONTENT_GROUP:                     'WT.cg_n',
        CONTENT_SUBGROUP:                  'WT.cg_s',
        EXTERNAL_VISITOR_ID:               'WT.dcsvid',
        SERVER_CALL_IDENTIFIER:            'WT.dl',
        PAGE_VIEW_IDENTIFIER:              'WT.dl',
        EVENT_SOURCE:                      'WT.es',
        CAMPAIGN_ID:                       'WT.mc_id',
        ONSITE_SEARCH_WORD:                'WT.oss',
        SEARCH_PAGE_NUMBER:                'WT.oss_p',
        NUM_SEARCH_RESULTS:                'WT.oss_r',
        REGISTERED_VISITOR:                'WT.rv',
        SITE:                              'WT.site',
        SCENARIO_ANALYSIS_CONVERSION_STEP: 'WT.si_cs',
        SCENARIO_ANALYSIS_NAME:            'WT.si_n',
        SCENARIO_ANALYSIS_STEP_NUM:        'WT.si_x',
        PAGE_TITLE:                        'WT.ti',
        VIDEO_NAME:                        'WT.clip_n',
        VIDEO_PERCENT_COMPLETE:            'WT.clip_ev',
        PRODUCT_SKU:                       'WT.pn_sku',
        TRANSACTION_EVENT:                 'WT.tx_e',

        // SmartSource Data Collector
        // Override Parameters
        // Parameter names from: http://kb.webtrends.com/support/solutiondetail.aspx?Id=50140000000a92kAAA
        DCS_REFERRER:               'DCS.dcsref',
        DCS_DOMAIN:                 'DCS.dcssip',
        DCS_URI_STEM:               'DCS.dcsuri',
        DCS_PROTOCOL:               'DCS.dcspro',
        DCS_URI_QUERY:              'DCS.dcsqry',
        DCS_AUTHENTICATED_USERNAME: 'DCS.dcsaut',
        DCS_METHOD:                 'DCS.dcsmet',
        DCS_STATUS:                 'DCS.dcssta',
        DCS_BYTES:                  'DCS.dcsbyt',
        DCS_IP_ADDRESS:             'DCS.dcscip',
        DCS_USER_AGENT:             'DCS.dcsua',

        // Custom
        TYPE:                                'WT.z_type',
        ACTIVITY_NAME:                       'WT.z_actname',
        ACTIVITY_SUB_NAME:                   'WT.z_actsub',
        ACTIVITY_SUB_NAME_2:                 'WT.z_actsub2',
        ACTIVITY_TYPE:                       'WT.z_acttype',
        ACTIVITY_TRANSACTION:                'WT.z_acttx',
        ACTIVITY_ACTION:                     'WT.z_action',
        STATE:                               'WT.z_state',
        CAROUSEL_NAME:                       'WT.z_cn',
        CAROUSEL_ROTATE:                     'WT.z_cr',
        CAROUSEL_SELECT:                     'WT.z_cs',
        LESSON_DATA_RESTORED:                'WT.z_lrstrd',
        SCENARIO_ANALYSIS_STEP_PREPOPULATED: 'WT.z_si_p',
        CONTENT_GROUP_TYPE:                  'WT.z_cg_t',
        CG_TYPE:                             'WT.z_cg_type',
        AUTHOR_NAME:                         'WT.z_author',
        REG_WALL:                            'WT.z_Wall'
    };

    var WT = {

        IIC_FIRED_KEY: 'initialInputChange-fired',

        resetInitialInputChange: function (obj) {
            $(obj).data(this.IIC_FIRED_KEY, false);
        },

        initialInputChange: function (obj, handler) {
            var FIRED_KEY = this.IIC_FIRED_KEY,
                PREV_VAL_KEY = 'initialInputChange-prevVal';

            var onEvent = ($(obj).is('select') ? 'change' : 'keyup');
            var onEventHandler = function (eventObject) {
                var fired = !!$(this).data(FIRED_KEY);
                var prevVal = $(this).data(PREV_VAL_KEY) || '';
                var inputChanged = (prevVal !== $(this).val());

                if (inputChanged) {
                    if (!fired) {
                        if (typeof handler === 'function') {
                            handler.call($(this), eventObject);
                        }

                        $(this).data(FIRED_KEY, true);
                    }

                    $(this).data(PREV_VAL_KEY, $(this).val());
                }
            };

            if (typeof obj === 'string') {
                $(document).on(onEvent, obj, onEventHandler);
            } else {
                $(obj).bind(onEvent, onEventHandler);
            }
        },

        tags : wtTags,

        SERVER_CALL_IDENTIFIERS : {
            STANDARD_PAGE_VIEW: '0',
            DOWNLOAD:           '20',
            MAILTO:             '23',
            OFFSITE_LINK:       '24',
            SALT_CUSTOM_EVENT:  '90',
            NAMED_ANCHOR:       '99'
        },

        USER_STATE_IDENTIFIERS : {
            LOGGED_OUT:          '00',
            LOGGED_IN_UNMATCHED: '01',
            LOGGED_IN_MATCHED:   '11'
        },

        ID_TAGS  : [
            wtTags.DCS_AUTHENTICATED_USERNAME,
            wtTags.EXTERNAL_VISITOR_ID
        ],

        // Noncumulative (aka "Normally Empty") Tags
        NORMALLY_EMPTY_TAGS : [
            wtTags.SCENARIO_ANALYSIS_CONVERSION_STEP,
            wtTags.SCENARIO_ANALYSIS_NAME,
            wtTags.SCENARIO_ANALYSIS_STEP_NUM,
            wtTags.ACTIVITY_NAME,
            wtTags.ACTIVITY_TYPE,
            wtTags.ACTIVITY_TRANSACTION,
            wtTags.TRANSACTION_EVENT,
            wtTags.CONTENT_GROUP_TYPE,
            wtTags.CG_TYPE,
            wtTags.AUTHOR_NAME
        ],

        getTagIndex: function (tags, tagName) {
            var tagIndex = -1;

            for (var i = 0; tagIndex === -1 && i < tags.length; i += 2) {
                if (tags[i] === tagName) {
                    tagIndex = i;
                }
            }

            return tagIndex;
        },

        // adds (or replaces) a tag to a list of tags
        setTag: function (tags, tagName, tagValue) {
            if (!tags) {
                tags = [];
            }

            var tagIndex = this.getTagIndex(tags, tagName);

            if (tagIndex === -1) {
                tags.push(tagName);
                tags.push(tagValue);
            } else {
                tags[tagIndex + 1] = tagValue;
            }

            return tags;
        }
    };

    return WT;
});


define('modules/ReportingStatus',[
    'jquery',
    'underscore',
    'salt',
    'salt/models/SiteMember',
    'asa/ASAUtilities',
    'jquery.cookie'
], function ($, _, SALT, SiteMember, Utility) {

    $(document.body).on('click', '.js-salt-courses-link', function () {
        var _this = this;
        //SWD-8550: Show login windowshare if someone is in the remembered state but not logged in
        if (!($.cookie('IndividualId')) && $.cookie('RememberMe')) {
            SALT.trigger('open:customOverlay', 'loginOverlay');
        }
        else if ($.cookie('IndividualId')) {
            SiteMember.done(function (siteMember) {
                if (ReportingStatus.showOverlay || ReportingStatus.checkReportIdStatus(siteMember)) {
                    var enrollmentStatus = SiteMember.get('EnrollmentStatus');
                    $('#' + enrollmentStatus + '-selected').attr('selected', '');
                    //targetCoursePath captures the current (in context) link's 
                    //original href to eventually send the user to
                    overlayHolderObject.targetCoursePath = $(_this).data('coursepath');
                    $(overlayHolderObject.overlay).foundation('reveal', 'open');
                } else {
                    //reporting ID entered or not needed, go to courses
                    Utility.deepLinkToCourse($(_this).data('coursepath'));
                }
            });
        }
    });

    $(document.body).on('valid', '#FrmReportId', function () {
        var bGotoSALTCourses = ReportingStatus.submitChanges();
        if (bGotoSALTCourses === true) {
            Utility.deepLinkToCourse(overlayHolderObject.targetCoursePath);
        }
    });

    $(document.body).on('change', '#selectEnrollmentStatusOverlay', function () {
        $('#js-reporting-id-status-message').text('');
        //if status in not one of the excluding answers and no reportingId has been collected
        if (!ReportingStatus.validateStatus($(overlayHolderObject.selectBoxEnrollmentStatus).val()) && ReportingStatus.showOverlay) {
            $('.js-reporting-id-block').show();
        } else {
            $('.js-reporting-id-block').hide();
        }
    });

    var ReportingStatus = {
        showOverlay: false, //initially assume no overlay
        overlayHolderObject: '',
        initOverlay: function (bShowReportIdQuestion) {
            //if enrollment status answer is not one of these (there was no valid answer)
            //or no reportingId collected show overlay
            if (!ReportingStatus.validateStatus(this.overlayHolderObject.EnrollmentStatus) && bShowReportIdQuestion) {
                ReportingStatus.showOverlay = true;
            }
            //************************************************
            //Hide if reportingId already collected, or no enrollemnt status collected
            //reportingId box depends on enrollment status answer
            //show overlay with enrollment status but hide reportingId box until it is needed
            //************************************************
            if (!bShowReportIdQuestion || this.overlayHolderObject.EnrollmentStatus === '') {
                $('.js-reporting-id-block').hide();
            }
            //************************************************
            // overlay setup for all mm101 links
            //************************************************
            if (ReportingStatus.showOverlay) {
                $('.js-salt-courses-link').attr({
                    href: '#IDFrm'
                });
                if ($.cookie('IndividualId')) {
                    $(this.overlayHolderObject.selectBoxEnrollmentStatus).val(this.overlayHolderObject.EnrollmentStatus);
                }
            }
        },
        submitChanges: function () {
            var bGotoSALTCourses = true;
            objectPlaceHolder = $(overlayHolderObject.selectBoxEnrollmentStatus).val();

            var $reportingId = $('.js-reporting-id');

            //if no valid enrollment status provided
            if (objectPlaceHolder === 'Q' || objectPlaceHolder === null) {
                //prompt user for valid enrollment status
                $('#js-reporting-id-status-message').text('Please select a status from the drop down');
                bGotoSALTCourses = false;
            } else {
                //user has selected an enrollment status
                //hide overlay
                $('.close-reveal-modal').click();

                _.each($('.js-organization-id'), function (element, ind, arr) {
                    var organizationId = parseInt($('.js-organization-id')[ind].value, 10);
                    var reportingId = $reportingId[ind].value || null;
                    if (reportingId) {
                        overlayHolderObject.validReportingIdSubmitted = true;
                        _.findWhere(overlayHolderObject.Organizations, {'OrganizationId': organizationId}).ReportingId = reportingId;
                    }
                });
                //if at least one valid reportingId submitted, update reportingId
                if (overlayHolderObject.validReportingIdSubmitted) {
                    SALT.services.UpdateMemberOrganizations(overlayHolderObject.MemberId, overlayHolderObject.Organizations, ReportingStatus.reportIDSuccess, ReportingStatus.serviceCallFailure);
                }
                
                SiteMember.done(function (siteMember) {
                    //update enrollment status only if there's a valid change 
                    if (siteMember.EnrollmentStatus !== objectPlaceHolder) {
                        ReportingStatus.updateEnrollmentStatus(siteMember);
                    }
                });  
            }
            return bGotoSALTCourses;
        },
        checkReportIdStatus: function (siteMember) {
            var showReportIdQuestion = false;
            //check that all Member's Organizations have a reportingId for organizations that participate
            var organizations = _.filter(siteMember.Organizations, function (organization) {
                return _.findWhere(siteMember.OrganizationProducts, {OrganizationId: organization.OrganizationId, ProductID: 1, IsOrgProductActive: true});
            });
            if (organizations) {
                var reportingID = _.findWhere(organizations, {ReportingId: null});
                if (reportingID) {
                    showReportIdQuestion = true;
                }
            }

            return showReportIdQuestion;
        },
        showReportingIdQuestion: function () {
            SiteMember.done(function (siteMember) {
                if (siteMember.IsAuthenticated === 'true') {
                    var showReportIdQuestion = ReportingStatus.checkReportIdStatus(siteMember);
                    ReportingStatus.setupReportingIdOverlay(showReportIdQuestion, siteMember);
                }
            });
        },
        setupReportingIdOverlay: function (bRptIdShow, siteMember) {
            overlayHolderObject = {
                targetCoursePath: '',
                validReportingIdSubmitted: false,
                overlay: '#reportModal',
                selectBoxEnrollmentStatus: '#selectEnrollmentStatusOverlay',
                MemberId: siteMember.MembershipId,
                EnrollmentStatus: siteMember.EnrollmentStatus,
                Organizations: siteMember.Organizations,
                OrganizationProducts: _.where(siteMember.OrganizationProducts, {ProductID: 1, IsOrgProductActive: true})
            };

            ReportingStatus.populateReportingId();
            ReportingStatus.initOverlay(bRptIdShow);
        },
        validateStatus: function (selectEnrollmentStatus) {
            //these values don't require reportingId . no textbox should show
            if (selectEnrollmentStatus === 'X' || selectEnrollmentStatus === 'Q' || selectEnrollmentStatus === 'G' || selectEnrollmentStatus === 'W' || selectEnrollmentStatus === 'N') {
                return true;
            }
            return false;
        },
        populateReportingId: function () {
            //check if organizations participates in reportingId product
            var organizations = _.filter(overlayHolderObject.Organizations, function (organization) {
                return _.findWhere(overlayHolderObject.OrganizationProducts, {OrganizationId: organization.OrganizationId});
            });
            if (organizations.length > 0) {
                context = {
                    Organizations : organizations
                };
                Utility.renderDustTemplate('Overlays/ReportingIdRow', context, function (error, out) {
                    $('.js-individual-reporting-id').append(out);
                    ReportingStatus.getUserSegment();
                }, null); 
            }
        },
        updateEnrollmentStatus: function (member) {
            //here objectPlaceHolder is a string value
            member.EnrollmentStatus = objectPlaceHolder;

            // treating YOB as a string as a workaround a .NET bug with smallint type
            var yob = member.YearOfBirth;
            if (yob && typeof yob === 'number') {
                member.YearOfBirth = yob.toString();
            }
            SALT.services.ManageAccount(member, ReportingStatus.statusUpdateSuccess, ReportingStatus.serviceCallFailure);
        },
        reportIDSuccess: function (updateSuccessful) {
            //update the UI links immediately up on successful valid reportingid save
            if (updateSuccessful && overlayHolderObject.validReportingIdSubmitted) { 
                $('.js-salt-courses-link').each(function (index) {
                    $(this).attr('href', '#');
                });
                ReportingStatus.showOverlay = false;
            }
        },
        statusUpdateSuccess: function (data) {
            //console.log('EnrollmentStatus updated successfully');
        },
        getUserSegment: function () {
            if ($.cookie('UserSegment')) {
                SALT.services.GetUserSegment($.cookie('UserSegment'), ReportingStatus.updateLabel, ReportingStatus.serviceCallFailure);
            }
        },
        updateLabel: function (data) {
            try {
                var endecaDefaultLabel = _.findWhere(data.secondaryContent[4].contents, {name: 'Default'});
                _.each(overlayHolderObject.OrganizationProducts, function (element, ind, arr) {
                    organization = _.findWhere(overlayHolderObject.Organizations, {OrganizationId: element.OrganizationId});
                    var endecaContent = _.findWhere(data.secondaryContent[4].contents, {name: organization.OrganizationLogoName});
                    var labelIndentifier = '#js-reporting-id-label' + organization.OrganizationId;
                    if (endecaContent) {
                        $('' + labelIndentifier + '').html($(endecaContent.content));
                    } else if (endecaDefaultLabel) {
                        $('' + labelIndentifier + '').html($(endecaDefaultLabel.content));
                    }
                    else {
                        $('' + labelIndentifier + '').html('<p>Enter your Organization/School ID to ensure you receive credit for the SALT Courses you complete.</p>');
                    }
                });
            } catch (e) {
                if (console && console.log) {
                    console.log('updateLabel Error:' + e.message);
                }
            }
        },
        serviceCallFailure: function (jqxhr, textStatus, error) 
        {
            if (console && console.log) {
                console.log('JSON Request Failed--- TextStatus:' + textStatus + ' -- Error: ' + error);
            }
        }
    };
    return ReportingStatus;
});

define('modules/ASALocalStore',[],function () {
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

/* jshint maxstatements: 32 */
define('salt/LoginManager',[
    'jquery',
    'salt',
    'asa/ASAUtilities',
    'salt/models/SiteMember',
    'modules/ReportingStatus',
    'configuration',
    'underscore',
    'modules/ASALocalStore',
    'jquery.cookie'
], function ($, SALT, Utility, SiteMember, ReportingStatus, Configuration, _, asaLocalStore) {
    SALT.LoginManager = {
        isInitialized: false,
        logonForm: null,
        errorField: null,
        returnURLOverride: null,
        init: function (config) {

            this.logonForm = $(config.logonFormName);
            this.errorField = $(config.errorFieldName);

            this.returnURLOverride = config.returnURLOverride || this.returnURLOverride;
            //This init function can get called many times.  Make sure we aren't firing the login start webtrends repeatedly
            if (!this.isInitialized) {
                //Use "one" because the login form can be opened and closed multiple times, we dont want to fire multiple events for this
                $('#UserName').one('focus', function () {
                    SALT.publish('LoginManager:Start');
                });
                this.isInitialized = true;
            }

            $('#loginForm, #loginSideForm').on('valid', function () {
                SALT.LoginManager.authenticate();
            });

            SiteMember.done(function (siteMember) {
                if (siteMember.IsAuthenticated === 'true') {
                    if (SiteMember.get('PrimaryEmailKey')) {
                        $('input[name="UserName"]').val(SiteMember.get('PrimaryEmailKey'));
                        $('input[name="Password"]').focus();
                    }
                } else if ($.cookie('RememberMe')) {
                    $('input[name="UserName"]').val($.cookie('RememberMeEmail'));
                    $('input[name="Password"]').focus();
                }
            });

        },
        removeTourFromUrl: function () {
            //remove tour from URL so SALT tour video will not replay when logging in
            var locationSearch = Utility.getLocationSearch();
            locationSearch = locationSearch.replace('tour=true', '');
            locationSearch = locationSearch.replace('?&', '?');
            if (locationSearch === '?') {
                locationSearch = '';
            }
            return locationSearch;
        },
        authenticate: function () {
            var loginForm = SALT.LoginManager.logonForm;
            var errorCode = SALT.LoginManager.errorField;
            errorCode.html('');

            var data = loginForm.serialize();
            var returnURLstring = '';
            var returnUrl = Utility.getParameterByName('ReturnUrl');
            var rememberMeVal = SALT.LoginManager.logonForm[0];

            if (SALT.LoginManager.returnURLOverride) {
                returnUrl = encodeURIComponent(SALT.LoginManager.returnURLOverride);
            }

            if (returnUrl !== '') {
                returnURLstring = '&returnUrl=' + returnUrl;
            // added logic to redirect to homepage when logon occurs on registration page
            } else {
                returnURLstring = '&returnUrl=' + location.pathname;
            }

            if (location.pathname === '/register/') {
                returnURLstring = '&returnUrl=' + location.host;
            } else if (location.pathname.toLowerCase() === '/home/newpassword') {
                returnURLstring = '&returnUrl=/home';
                // SWD-1663 -- Sitewide content  search. If a user logs in from their search results ("searchCriteria"),
                // return them to the same location and query string they searched for. -khennings
            } else if (location.href.indexOf('searchCriteria') > 0) {
                returnURLstring = '&returnUrl=' + location.pathname + location.search;
            } else if (location.pathname.indexOf('index.html') > -1 || location.href.indexOf('Type=') > -1) {
                //remove tour from URL so SALT tour video will not replay when logging in
                var updatedURL = SALT.LoginManager.removeTourFromUrl();
                if (updatedURL.indexOf('?ReturnUrl=/') === 0) {
                    returnURLstring = '&returnUrl=/' + updatedURL.substr(12, updatedURL.length);
                } else {
                    returnURLstring = '&returnUrl=/home' + updatedURL;
                }
            }

            if (rememberMeVal.checkRememberMe.checked && $.cookie('RememberMe') && $.cookie('RememberMeUrl')) {
                returnURLstring = '&returnUrl=' + encodeURIComponent($.cookie('RememberMeUrl'));
            }

            data += returnURLstring;

            // standard spinner cannot be used- it prevents error fixing
            $('.loading').show();
            if (loginForm.attr('id') === 'loginForm') {
                $('#loginBtn span').text('Logging In...');

            } else {
                $('.loginBtn span').text('Logging In...');
            }


            $.ajax({
                type: 'POST',
                url: '/Account/LogOn',
                data: data,
                dataType: 'json'
            })
            .done(function (result) {
                if (result.Success && result.ReturnUrl) {
                    //If the user is dashboard enabled, set a cookie that is used by the front-end to render the new todo-tile style
                    if (result.Member.DashboardEnabled) {
                        document.cookie = 'useTodoTileDesign = true; path=/';
                    }
                    var profileQandAresponses = _.last(result.Member.ProfileQAs).Responses;
                    _.each(profileQandAresponses, function (response) {
                        //Set specific cookie for the TopRankedGoal response, otherwise set using Question and Answer external ids
                        if (response.QuestionExternalId === 17) {
                            document.cookie = 'TopGoal=' + 'Dashboard-TopGoal-' + response.AnsName + '; PATH=/';
                        } else {
                            document.cookie = 'ans-' + response.QuestionExternalId + '-' + response.AnsExternalId + '=' + 'Dashboard-QA-' + response.QuestionExternalId + '-' + response.AnsName + '; PATH=/';
                        }
                    });
                    SiteMember.set(_.extend(result.Member, {IsAuthenticated: true}));
                    SALT.publish('siteMember:signin:complete', {
                        location: {
                            pathname: '/SignIn.html'
                        },
                        document: {
                            title: 'SignIn'
                        },
                        siteMember: result.Member
                    });

                    Utility.handleRegWallWT('Wall_Login_Complete');
                    $.removeCookie('regWall');

                    if (result.Member.IndividualId) {
                        document.cookie = 'IndividualId=' + result.Member.IndividualId + '; PATH=/; Domain=saltmoney.org;';
                    }
                    if ($.cookie('RememberMe')) {
                        $.removeCookie('RememberMe', { path: '/', domain: 'saltmoney.org' });
                        $.removeCookie('RememberMeEmail', { path: '/', domain: 'saltmoney.org' });
                        $.removeCookie('RememberMeUrl', { path: '/', domain: 'saltmoney.org' });
                    }

                    if (rememberMeVal.checkRememberMe.checked && result.Member.PrimaryEmailKey) {
                        var exdate = new Date();
                        exdate.setDate(exdate.getDate() + 730);
                        document.cookie = 'RememberMe=true; PATH=/; Domain=saltmoney.org; Expires=' + exdate.toString() + ';';
                        document.cookie = 'RememberMeEmail=' + result.Member.PrimaryEmailKey + '; PATH=/; Domain=saltmoney.org; Expires=' + exdate.toString() + ';';
                        //webtrends call for Rememberme // orig call //dcsMultiTrack('WT.z_actname', 'Remember Me', 'WT.z_acttype', 'save', 'WT.z_acttx', '1');
                        SALT.publish('LoginManager:RememberMe');
                    }

                    //remove any previously cached courses data
                    if (asaLocalStore.getLocalStorage('coursesCompletion')) {
                        asaLocalStore.removeLocalStorage('coursesCompletion');
                    }

                    Utility.findUserSegment(result.Member);

                    // TODO: add analytics logger callback/promise pattern to guarantee
                    //       we don't navigate away from the page until all logging is complete
                    // increased to 1000 for lessons save on exit
                    setTimeout(function () {
                        if (SALT.LoginManager.repaymentNavigatorOverride) {
                            location.href = SALT.LoginManager.repaymentNavigatorOverride;
                            // Delete navigatorPath cookie if it exists
                            if ($.cookie('navigatorPath')) {
                                $.removeCookie('navigatorPath');
                            }
                        } else {
                            var locName = Utility.getParameterByName('ReturnUrl'),
                            validLink = ['/home#fndtn-MySALT', '/home#fndtn-Todos', '/home#fndtn-Library'];
                            //directing the pages with dashboard link to the correct pages. 
                            if (validLink.indexOf(locName) !== -1) {
                                location.href = location.origin + locName;
                            } else if (Utility.getParameterByNameFromString('ReturnUrl', result.ReturnUrl)) {
                                location.href = Utility.getParameterByNameFromString('ReturnUrl', result.ReturnUrl);
                            } else if (Utility.getHostname(document.referrer) === Utility.getHostname(Configuration.mm101.url)) {
                                /*SWD-9522, user came for authentication, re route back while triggering SSO */
                                location.href = Configuration.mm101.url;
                            } else if (SALT.LoginManager.returnURLOverride && Utility.IsCurrentPage('/logon')) {
                                /* SWD-7217: fix index routing to home page with url querystring*/
                                location.href = SALT.LoginManager.returnURLOverride.replace('index.html', 'home');
                            } else {
                                location.href = result.ReturnUrl;
                            }
                        }
                    }, 1000);
                } else {
                    $('.loading').hide();
                    if (loginForm.attr('id') === 'loginForm') {
                        $('#loginBtn span').text('Log in');
                    } else {
                        $('.loginBtn span').text('Log in');
                    }
                    errorCode.html(result.Message);
                }
            });

            return false;
        }
    };

    return SALT.LoginManager;
});

// Backbone.Wreqr, v0.0.0
// Copyright (c)2012 Derick Bailey, Muted Solutions, LLC.
// Distributed under MIT license
// http://github.com/marionettejs/backbone.wreqr
(function (root, factory) {
  if (typeof exports === 'object') {

    var underscore = require('underscore');
    var backbone = require('backbone');

    module.exports = factory(underscore, backbone);

  } else if (typeof define === 'function' && define.amd) {

    define('backbone.wreqr',['underscore', 'backbone'], factory);

  } 
}(this, function (_, Backbone) {
  "option strict";

  Backbone.Wreqr = (function(Backbone, Marionette, _){
    "option strict";
    var Wreqr = {};
  
    Wreqr.Handlers = (function(Backbone, _){
      "option strict";
      
      var Handlers = function(){
        
        this._handlers = {};
      };
    
      Handlers.extend = Backbone.Model.extend;
    
      _.extend(Handlers.prototype, {
        addHandler: function(name, handler, context){
          var config = {
            callback: handler,
            context: context
          };
    
          this._handlers[name] = config;
        },
    
        getHandler: function(name){
          var config = this._handlers[name];
    
          if (!config){
            throw new Error("Handler not found for '" + name + "'");
          }
    
          return function(){
            return config.callback.apply(config.context, arguments);
          };
        },
    
        removeHandler: function(name){
          delete this._handlers[name];
        },
    
        removeAllHandlers: function(){
          this._handlers = {};
        }
      });
    
      return Handlers;
    })(Backbone, _);
    
    // Wreqr.Commands
    // --------------
    //
    // A simple command pattern implementation. Register a command
    // handler and execute it.
    Wreqr.Commands = (function(Wreqr){
      "option strict";
    
      return Wreqr.Handlers.extend({
        execute: function(name, args){
          this.getHandler(name)(args);
        }
      });
    
    })(Wreqr);
    
    // Wreqr.RequestResponse
    // ---------------------
    //
    // A simple request/response implementation. Register a
    // request handler, and return a response from it
    Wreqr.RequestResponse = (function(Wreqr){
      "option strict";
    
      return Wreqr.Handlers.extend({
        request: function(name, args){
          return this.getHandler(name)(args);
        }
      });
    
    })(Wreqr);
    
    // Event Aggregator
    // ----------------
    // A pub-sub object that can be used to decouple various parts
    // of an application through event-driven architecture.
    
    Wreqr.EventAggregator = (function(Backbone, _){
      "option strict";
      var EA = function(){};
    
      // Copy the `extend` function used by Backbone's classes
      EA.extend = Backbone.Model.extend;
    
      // Copy the basic Backbone.Events on to the event aggregator
      _.extend(EA.prototype, Backbone.Events);
    
      return EA;
    })(Backbone, _);
    
  
    return Wreqr;
  })(Backbone, Backbone.Marionette, _);
  
  return Backbone.Wreqr; 

}));

/*!
 * SALT / Analytics Abstraction Layer
 */

define('salt/analytics',[
    'jquery',
    'salt',
    'salt/models/SiteMember',
    'underscore',
    'backbone',
    'dust',
    'backbone.wreqr'
], function ($, SALT, SiteMember, _, Backbone, dust) {

    var loggers = [],
        baseTemplateContext = {};
    // Add window.location properties to base context
    _.each(['hash', 'host', 'hostname', 'href', 'pathname', 'port', 'protocol', 'search'], function (prop) {
        if (!baseTemplateContext.location) {
            baseTemplateContext.location = {};
        }

        baseTemplateContext.location[prop] = window.location[prop];
    });
    // Add document properties to base context
    _.each(['URL', 'baseURI', 'compatMode', 'contentType', 'designMode', 'dir', 'documentURI', 'domain', 'inputEncoding', 'referrer', 'title'], function (prop) {
        if (!baseTemplateContext.document) {
            baseTemplateContext.document = {};
        }

        baseTemplateContext.document[prop] = document[prop];
    });
    /**
     * Recursively renders all string elements as Dust templates using message as template context.
     */
    function renderArray(array, message, callback) {
        var first, rest;

        if (array.length) {
            first = _.first(array);
            rest  = _.rest(array);

            renderConfig(first, message, function (first) {
                if (rest.length) {
                    renderArray(rest, message, function (rest) {
                        if (_.isFunction(callback)) {
                            callback([first].concat(rest));
                        }
                    });
                } else {
                    if (_.isFunction(callback)) {
                        callback([first]);
                    }
                }
            });
        } else {
            callback(array);
        }
    }

    /**
     * Recursively renders all string properties as Dust templates using message as template context.
     */
    function renderObject(object, message, callback) {
        var randomKey = _.first(_.keys(object)),
            random = _.clone(object[randomKey]),
            rest = {}, restKeys = [];

        for (var key in object) {
            if (key !== randomKey) {
                rest[key] = _.clone(object[key]);
                restKeys.push(key);
            }
        }

        renderConfig(random, message, function (random) {
            var renderedRandom = {};
            renderedRandom[randomKey] = random;

            if (restKeys.length) {
                renderObject(rest, message, function (rest) {
                    if (_.isFunction(callback)) {
                        callback(_.extend({}, renderedRandom, rest));
                    }
                });
            } else {
                if (_.isFunction(callback)) {
                    callback(renderedRandom);
                }
            }
        });
    }

    /**
     * Recursively renders all map config strings as Dust templates using message as template context.
     * Supported map types: bare strings, arrays, object literals
     */
    function renderConfig(config, message, callback) {
        if (_.isString(config)) {
            // Render strings
            dust.renderSource(config, message, function (error, string) {
                if (!error && _.isFunction(callback)) {
                    callback(string);
                }
            });
        } else if (_.isArray(config)) {
            // Render strings contained in arrays
            renderArray(config, message, function (array) {
                if (_.isFunction(callback)) {
                    callback(array);
                }
            });
        } else if (_.isObject(config) && !_.isFunction(config) && !_.isDate(config) && !_.isBoolean(config) && !_.isNumber(config) && !_.isRegExp(config) && !_.isArguments(config)) {
            // Render strings contained in object literals
            renderObject(config, message, function (object) {
                if (_.isFunction(callback)) {
                    callback(object);
                }
            });
        } else {
            // Everything else passes through as-is
            if (_.isFunction(callback)) {
                callback(config);
            }
        }
    }

    function registerSubscriptions(logger, subscriptions) {
        var subscribing = $.Deferred();

        function subSuccess() {
            subscribing.resolve();
        }

        function subFailure() {
            subscribing.reject();
        }

        if (_.isString(subscriptions)) {

            require(['json!' + subscriptions], function (subscriptionsJSON) {
                registerSubscriptions(logger, subscriptionsJSON).done(subSuccess).fail(subFailure);
            });
        } else if (_.isArray(subscriptions)) {
            _.each(subscriptions, function (subscription) {
                registerSubscriptions(logger, subscription).done(subSuccess).fail(subFailure);
            });
        } else if (_.isObject(subscriptions)) {
            $.when.apply($, _.map(subscriptions, function (config, topic) {
                return logger.subscribe(topic, config);
            })).done(subSuccess).fail(subFailure);
        } else {
            subscribing.reject();
        }

        return subscribing.promise();
    }

    var Analytics = new (Backbone.View.extend({
        /**
         * Starts logging on all application-level events.
         */
        initialize: function () {
            SALT.subscribe('all', function (topic, message) {
                this.log(topic, message);
            }, this);
        },

        /**
         * Logs a topic and message with all registered loggers.
         */
        log: function (topic, message) {
            _.each(loggers, function (logger) {
                logger.publish(topic, message);
            });
        },

        /**
         * Registers a new logger with the list of registered analytics loggers.
         * If the logger is already registered, the new logger replaces the old logger.
         *
         * All logger map strings are passed through the Dust renderer using the published message as context.
         */
        registerLogger: function (loggerConfig) {
            var newLogger,
                subscriptions   = loggerConfig.map,
                templateContext = $.extend(true, {}, baseTemplateContext, loggerConfig.context);

            loggerConfig.map     = undefined;
            loggerConfig.context = undefined;

            // Add tracker/track vocabulary alongside logger/log.
            if (loggerConfig.log) {
                loggerConfig.track = loggerConfig.log;
            } else {
                loggerConfig.log   = loggerConfig.track;
            }

            // Create the new logger instance as its own sandboxed event aggregator,
            // capable of managing its own set of event maps.
            newLogger = new (Backbone.Wreqr.EventAggregator.extend(loggerConfig))();
            newLogger.publish = newLogger.pub = newLogger.trigger;

            // Provide ability to add new subscriptions on-the-fly.
            // Format: [event name], [logger map config], [optional context]
            newLogger.subscribe = newLogger.sub = function (topic, config, context) {
                var subscribing;

                if (arguments.length === 1) {
                    return registerSubscriptions(newLogger, arguments[0]);
                } else {
                    context = context ? context : this;

                    subscribing = $.Deferred();

                    newLogger.on(topic, function (message) {
                        if (_.isFunction(newLogger.log)) {
                            renderConfig(config, $.extend(true, {}, templateContext, message, { siteMember: SiteMember.toJSON() }), function (renderedConfig) {
                                newLogger.log(renderedConfig);
                            });
                        }
                    }, context);

                    subscribing.resolve();
                    return subscribing.promise();
                }
            };

            // Remove the old logger, if it exists.
            loggers = _.reject(loggers, function (logger) {
                return (logger.id === newLogger.id);
            });

            // Register all configured subscriptions to the new logger.
            registerSubscriptions(newLogger, subscriptions);

            // Add the new logger.
            loggers.push(newLogger);

            // Call the new logger's initialize method, if it exists.
            if (_.isFunction(newLogger.initialize)) {
                newLogger.initialize();
            }

            return newLogger;
        }
    }))();

    return Analytics;
});

/**
 * @license RequireJS text 2.0.3 Copyright (c) 2010-2012, The Dojo Foundation All Rights Reserved.
 * Available via the MIT or new BSD license.
 * see: http://github.com/requirejs/text for details
 */
/*jslint regexp: true */
/*global require: false, XMLHttpRequest: false, ActiveXObject: false,
  define: false, window: false, process: false, Packages: false,
  java: false, location: false */

define('text',['module'], function (module) {
    

    var text, fs,
        progIds = ['Msxml2.XMLHTTP', 'Microsoft.XMLHTTP', 'Msxml2.XMLHTTP.4.0'],
        xmlRegExp = /^\s*<\?xml(\s)+version=[\'\"](\d)*.(\d)*[\'\"](\s)*\?>/im,
        bodyRegExp = /<body[^>]*>\s*([\s\S]+)\s*<\/body>/im,
        hasLocation = typeof location !== 'undefined' && location.href,
        defaultProtocol = hasLocation && location.protocol && location.protocol.replace(/\:/, ''),
        defaultHostName = hasLocation && location.hostname,
        defaultPort = hasLocation && (location.port || undefined),
        buildMap = [],
        masterConfig = (module.config && module.config()) || {};

    text = {
        version: '2.0.3',

        strip: function (content) {
            //Strips <?xml ...?> declarations so that external SVG and XML
            //documents can be added to a document without worry. Also, if the string
            //is an HTML document, only the part inside the body tag is returned.
            if (content) {
                content = content.replace(xmlRegExp, "");
                var matches = content.match(bodyRegExp);
                if (matches) {
                    content = matches[1];
                }
            } else {
                content = "";
            }
            return content;
        },

        jsEscape: function (content) {
            return content.replace(/(['\\])/g, '\\$1')
                .replace(/[\f]/g, "\\f")
                .replace(/[\b]/g, "\\b")
                .replace(/[\n]/g, "\\n")
                .replace(/[\t]/g, "\\t")
                .replace(/[\r]/g, "\\r")
                .replace(/[\u2028]/g, "\\u2028")
                .replace(/[\u2029]/g, "\\u2029");
        },

        createXhr: masterConfig.createXhr || function () {
            //Would love to dump the ActiveX crap in here. Need IE 6 to die first.
            var xhr, i, progId;
            if (typeof XMLHttpRequest !== "undefined") {
                return new XMLHttpRequest();
            } else if (typeof ActiveXObject !== "undefined") {
                for (i = 0; i < 3; i += 1) {
                    progId = progIds[i];
                    try {
                        xhr = new ActiveXObject(progId);
                    } catch (e) {}

                    if (xhr) {
                        progIds = [progId];  // so faster next time
                        break;
                    }
                }
            }

            return xhr;
        },

        /**
         * Parses a resource name into its component parts. Resource names
         * look like: module/name.ext!strip, where the !strip part is
         * optional.
         * @param {String} name the resource name
         * @returns {Object} with properties "moduleName", "ext" and "strip"
         * where strip is a boolean.
         */
        parseName: function (name) {
            var strip = false, index = name.indexOf("."),
                modName = name.substring(0, index),
                ext = name.substring(index + 1, name.length);

            index = ext.indexOf("!");
            if (index !== -1) {
                //Pull off the strip arg.
                strip = ext.substring(index + 1, ext.length);
                strip = strip === "strip";
                ext = ext.substring(0, index);
            }

            return {
                moduleName: modName,
                ext: ext,
                strip: strip
            };
        },

        xdRegExp: /^((\w+)\:)?\/\/([^\/\\]+)/,

        /**
         * Is an URL on another domain. Only works for browser use, returns
         * false in non-browser environments. Only used to know if an
         * optimized .js version of a text resource should be loaded
         * instead.
         * @param {String} url
         * @returns Boolean
         */
        useXhr: function (url, protocol, hostname, port) {
            var uProtocol, uHostName, uPort,
                match = text.xdRegExp.exec(url);
            if (!match) {
                return true;
            }
            uProtocol = match[2];
            uHostName = match[3];

            uHostName = uHostName.split(':');
            uPort = uHostName[1];
            uHostName = uHostName[0];

            return (!uProtocol || uProtocol === protocol) &&
                   (!uHostName || uHostName.toLowerCase() === hostname.toLowerCase()) &&
                   ((!uPort && !uHostName) || uPort === port);
        },

        finishLoad: function (name, strip, content, onLoad) {
            content = strip ? text.strip(content) : content;
            if (masterConfig.isBuild) {
                buildMap[name] = content;
            }
            onLoad(content);
        },

        load: function (name, req, onLoad, config) {
            //Name has format: some.module.filext!strip
            //The strip part is optional.
            //if strip is present, then that means only get the string contents
            //inside a body tag in an HTML string. For XML/SVG content it means
            //removing the <?xml ...?> declarations so the content can be inserted
            //into the current doc without problems.

            // Do not bother with the work if a build and text will
            // not be inlined.
            if (config.isBuild && !config.inlineText) {
                onLoad();
                return;
            }

            masterConfig.isBuild = config.isBuild;

            var parsed = text.parseName(name),
                nonStripName = parsed.moduleName + '.' + parsed.ext,
                url = req.toUrl(nonStripName),
                useXhr = (masterConfig.useXhr) ||
                         text.useXhr;

            //Load the text. Use XHR if possible and in a browser.
            if (!hasLocation || useXhr(url, defaultProtocol, defaultHostName, defaultPort)) {
                text.get(url, function (content) {
                    text.finishLoad(name, parsed.strip, content, onLoad);
                }, function (err) {
                    if (onLoad.error) {
                        onLoad.error(err);
                    }
                });
            } else {
                //Need to fetch the resource across domains. Assume
                //the resource has been optimized into a JS module. Fetch
                //by the module name + extension, but do not include the
                //!strip part to avoid file system issues.
                req([nonStripName], function (content) {
                    text.finishLoad(parsed.moduleName + '.' + parsed.ext,
                                    parsed.strip, content, onLoad);
                });
            }
        },

        write: function (pluginName, moduleName, write, config) {
            if (buildMap.hasOwnProperty(moduleName)) {
                var content = text.jsEscape(buildMap[moduleName]);
                write.asModule(pluginName + "!" + moduleName,
                               "define(function () { return '" +
                                   content +
                               "';});\n");
            }
        },

        writeFile: function (pluginName, moduleName, req, write, config) {
            var parsed = text.parseName(moduleName),
                nonStripName = parsed.moduleName + '.' + parsed.ext,
                //Use a '.js' file name so that it indicates it is a
                //script that can be loaded across domains.
                fileName = req.toUrl(parsed.moduleName + '.' +
                                     parsed.ext) + '.js';

            //Leverage own load() method to load plugin value, but only
            //write out values that do not have the strip argument,
            //to avoid any potential issues with ! in file names.
            text.load(nonStripName, req, function (value) {
                //Use own write() method to construct full module value.
                //But need to create shell that translates writeFile's
                //write() to the right interface.
                var textWrite = function (contents) {
                    return write(fileName, contents);
                };
                textWrite.asModule = function (moduleName, contents) {
                    return write.asModule(moduleName, fileName, contents);
                };

                text.write(pluginName, nonStripName, textWrite, config);
            }, config);
        }
    };

    if (masterConfig.env === 'node' || (!masterConfig.env &&
            typeof process !== "undefined" &&
            process.versions &&
            !!process.versions.node)) {
        //Using special require.nodeRequire, something added by r.js.
        fs = require.nodeRequire('fs');

        text.get = function (url, callback) {
            var file = fs.readFileSync(url, 'utf8');
            //Remove BOM (Byte Mark Order) from utf8 files if it is there.
            if (file.indexOf('\uFEFF') === 0) {
                file = file.substring(1);
            }
            callback(file);
        };
    } else if (masterConfig.env === 'xhr' || (!masterConfig.env &&
            text.createXhr())) {
        text.get = function (url, callback, errback) {
            var xhr = text.createXhr();
            xhr.open('GET', url, true);

            //Allow overrides specified in config
            if (masterConfig.onXhr) {
                masterConfig.onXhr(xhr, url);
            }

            xhr.onreadystatechange = function (evt) {
                var status, err;
                //Do not explicitly handle errors, those should be
                //visible via console output in the browser.
                if (xhr.readyState === 4) {
                    status = xhr.status;
                    if (status > 399 && status < 600) {
                        //An http 4xx or 5xx error. Signal an error.
                        err = new Error(url + ' HTTP status: ' + status);
                        err.xhr = xhr;
                        errback(err);
                    } else {
                        callback(xhr.responseText);
                    }
                }
            };
            xhr.send(null);
        };
    } else if (masterConfig.env === 'rhino' || (!masterConfig.env &&
            typeof Packages !== 'undefined' && typeof java !== 'undefined')) {
        //Why Java, why is this so awkward?
        text.get = function (url, callback) {
            var stringBuffer, line,
                encoding = "utf-8",
                file = new java.io.File(url),
                lineSeparator = java.lang.System.getProperty("line.separator"),
                input = new java.io.BufferedReader(new java.io.InputStreamReader(new java.io.FileInputStream(file), encoding)),
                content = '';
            try {
                stringBuffer = new java.lang.StringBuffer();
                line = input.readLine();

                // Byte Order Mark (BOM) - The Unicode Standard, version 3.0, page 324
                // http://www.unicode.org/faq/utf_bom.html

                // Note that when we use utf-8, the BOM should appear as "EF BB BF", but it doesn't due to this bug in the JDK:
                // http://bugs.sun.com/bugdatabase/view_bug.do?bug_id=4508058
                if (line && line.length() && line.charAt(0) === 0xfeff) {
                    // Eat the BOM, since we've already found the encoding on this file,
                    // and we plan to concatenating this buffer with others; the BOM should
                    // only appear at the top of a file.
                    line = line.substring(1);
                }

                stringBuffer.append(line);

                while ((line = input.readLine()) !== null) {
                    stringBuffer.append(lineSeparator);
                    stringBuffer.append(line);
                }
                //Make sure we return a JavaScript string and not a Java string.
                content = String(stringBuffer.toString()); //String
            } finally {
                input.close();
            }
            callback(content);
        };
    }

    return text;
});

/** @license
 * RequireJS plugin for loading JSON files
 * - depends on Text plugin and it was HEAVILY "inspired" by it as well.
 * Author: Miller Medeiros
 * Version: 0.3.0 (2012/10/29)
 * Released under the MIT license
 */
define('json',['text'], function(text){

    var CACHE_BUST_QUERY_PARAM = 'bust',
        CACHE_BUST_FLAG = '!bust',
        jsonParse = (typeof JSON !== 'undefined' && typeof JSON.parse === 'function')? JSON.parse : function(val){
            return eval('('+ val +')'); //quick and dirty
        },
        buildMap = {};

    function cacheBust(url){
        url = url.replace(CACHE_BUST_FLAG, '');
        url += (url.indexOf('?') < 0)? '?' : '&';
        return url + CACHE_BUST_QUERY_PARAM +'='+ Math.round(2147483647 * Math.random());
    }

    //API
    return {

        load : function(name, req, onLoad, config) {
            if ( config.isBuild && (config.inlineJSON === false || name.indexOf(CACHE_BUST_QUERY_PARAM +'=') !== -1) ) {
                //avoid inlining cache busted JSON or if inlineJSON:false
                onLoad(null);
            } else {
                text.get(req.toUrl(name), function(data){
                    if (config.isBuild) {
                        buildMap[name] = data;
                        onLoad(data);
                    } else {
                        onLoad(jsonParse(data));
                    }
                },
                  onLoad.error
                );
            }
        },

        normalize : function (name, normalize) {
            //used normalize to avoid caching references to a "cache busted" request
            return (name.indexOf(CACHE_BUST_FLAG) === -1)? name : cacheBust(name);
        },

        //write method based on RequireJS official text plugin by James Burke
        //https://github.com/jrburke/requirejs/blob/master/text.js
        write : function(pluginName, moduleName, write){
            if(moduleName in buildMap){
                var content = buildMap[moduleName];
                write('define("'+ pluginName +'!'+ moduleName +'", function(){ return '+ content +';});\n');
            }
        }

    };
});

define("json!salt/analytics/webtrends/maps/main.json", function(){ return {
    "loanimport:upload:success": [
        "{tags.DCS_URI_STEM}",               "{location.pathname}",
        "{tags.SCENARIO_ANALYSIS_NAME}",     "mydataimport",
        "{tags.SCENARIO_ANALYSIS_STEP_NUM}", "7",
        "{tags.ACTIVITY_NAME}",              "fedloantool",
        "{tags.ACTIVITY_TYPE}",              "complete",
        "{tags.ACTIVITY_TRANSACTION}",       "1",
        "{tags.SERVER_CALL_IDENTIFIER}",     "{serverCallIdentifiers.SALT_CUSTOM_EVENT}"
    ],

    "carousel:rotate": [
        "{tags.CAROUSEL_NAME}",          "{carousel.name}",
        "{tags.CAROUSEL_ROTATE}",        "1",
        "{tags.CAROUSEL_SELECT}",        "",
        "{tags.SERVER_CALL_IDENTIFIER}", "{serverCallIdentifiers.SALT_CUSTOM_EVENT}"
    ],

    "carousel:button:click": [
        "{tags.CAROUSEL_NAME}",          "{carousel.name}",
        "{tags.CAROUSEL_ROTATE}",        "",
        "{tags.CAROUSEL_SELECT}",        "{carousel.activeSlide}",
        "{tags.SERVER_CALL_IDENTIFIER}", "{serverCallIdentifiers.SALT_CUSTOM_EVENT}"
    ],

    "vlcAction:fire": [
        "{tags.CONTENT_SUBGROUP}",           "{location}",
        "{tags.ACTIVITY_NAME}",              "tool",
        "{tags.ACTIVITY_SUB_NAME}",          "virtualcounselor",
        "{tags.ACTIVITY_TYPE}",              "{actType}",
        "{tags.ACTIVITY_TRANSACTION}",       "1",
        "{tags.SERVER_CALL_IDENTIFIER}",     "{serverCallIdentifiers.SALT_CUSTOM_EVENT}"
    ],

    "sortNavigationWT:fire": [
        "{tags.PAGE_TITLE}",                "{sortTitle}",
        "{tags.EVENT_SOURCE}",              "{sortEventSource}",
        "{tags.DCS_URI_STEM}",              "{sortUriStem}",
        "{tags.SERVER_CALL_IDENTIFIER}",    "{serverCallIdentifiers.SALT_CUSTOM_EVENT}"
    ],

    "unauthpersona:button:click": [
        "{tags.ACTIVITY_NAME}",              "{activity_name}",
        "{tags.ACTIVITY_TYPE}",              "{activity_type}",
        "{tags.ACTIVITY_TRANSACTION}",       "1",
        "{tags.SERVER_CALL_IDENTIFIER}",     "{serverCallIdentifiers.SALT_CUSTOM_EVENT}"
    ],

    "KWYO:seeloans": [
        "{tags.ACTIVITY_NAME}",          "KWYO View Loans",
        "{tags.ACTIVITY_TRANSACTION}",   "1",
        "{tags.SERVER_CALL_IDENTIFIER}", "{serverCallIdentifiers.SALT_CUSTOM_EVENT}"
    ],

    "KWYO:importLoans": [
        "{tags.ACTIVITY_NAME}",          "KWYO Import",
        "{tags.ACTIVITY_TRANSACTION}",   "1",
        "{tags.SERVER_CALL_IDENTIFIER}", "{serverCallIdentifiers.SALT_CUSTOM_EVENT}"
    ],

    "KWYO:addloans:start": [
        "{tags.ACTIVITY_NAME}",          "Add Loans",
        "{tags.ACTIVITY_TYPE}",          "Loan Start",
        "{tags.ACTIVITY_TRANSACTION}",   "1",
        "{tags.SERVER_CALL_IDENTIFIER}", "{serverCallIdentifiers.SALT_CUSTOM_EVENT}"
    ],

    "KWYO:addloans:complete": [
        "{tags.ACTIVITY_NAME}",          "{debtType}",
        "{tags.ACTIVITY_TYPE}",          "Loan Complete",
        "{tags.ACTIVITY_TRANSACTION}",   "1",
        "{tags.SERVER_CALL_IDENTIFIER}", "{serverCallIdentifiers.SALT_CUSTOM_EVENT}"
    ],

    "KWYO:profile:start": [
        "{tags.ACTIVITY_NAME}",          "Profile Start",
        "{tags.ACTIVITY_TYPE}",          "start",
        "{tags.ACTIVITY_TRANSACTION}",   "1",
        "WT.z_state",                    "01",
        "{tags.SERVER_CALL_IDENTIFIER}", "{serverCallIdentifiers.SALT_CUSTOM_EVENT}"
    ],

    "KWYO:switchView": [
        "{tags.CONTENT_SUBGROUP}",       "Switch View",
        "{tags.ACTIVITY_NAME}",          "KWYO Flip",
        "{tags.ACTIVITY_TRANSACTION}",   "1",
        "{tags.SERVER_CALL_IDENTIFIER}", "{serverCallIdentifiers.SALT_CUSTOM_EVENT}"
    ],

    "KWYO:glossary": [
        "{tags.CONTENT_SUBGROUP}",       "Glossary",
        "{tags.ACTIVITY_NAME}",          "KWYO Glossary",
        "{tags.ACTIVITY_TRANSACTION}",   "1",
        "{tags.SERVER_CALL_IDENTIFIER}", "{serverCallIdentifiers.SALT_CUSTOM_EVENT}"
    ],

    "KWYO:import:nav": [
        "{tags.ACTIVITY_NAME}",          "KWYO NavImport",
        "{tags.ACTIVITY_TRANSACTION}",   "1",
        "{tags.SERVER_CALL_IDENTIFIER}", "{serverCallIdentifiers.SALT_CUSTOM_EVENT}"
    ],

    "KWYO:tour:start": [
        "{tags.ACTIVITY_NAME}",          "KWYO Tour",
        "{tags.ACTIVITY_TYPE}",          "Start",
        "{tags.ACTIVITY_TRANSACTION}",   "1",
        "{tags.SERVER_CALL_IDENTIFIER}", "{serverCallIdentifiers.SALT_CUSTOM_EVENT}"
    ],

    "Registration:Start": [
        "{tags.TYPE}",                      "Tool Usage",
        "{tags.SCENARIO_ANALYSIS_NAME}",    "Member Registration",
        "{tags.SCENARIO_ANALYSIS_STEP_NUM}","1",
        "{tags.ACTIVITY_NAME}",             "activation",
        "{tags.ACTIVITY_TYPE}",             "start",
        "{tags.ACTIVITY_TRANSACTION}",      "1",
        "{tags.DCS_URI_STEM}",              "{location.pathname}",
        "{tags.PAGE_TITLE}",                "{document.title}",
        "{tags.EXTERNAL_VISITOR_ID}",       null,
        "{tags.DCS_AUTHENTICATED_USERNAME}","0",
        "{tags.REGISTERED_VISITOR}",        "0",
        "{tags.SERVER_CALL_IDENTIFIER}",    "{serverCallIdentifiers.SALT_CUSTOM_EVENT}"
    ],

    "Registration:Steps": [
        "{tags.TYPE}",                      "Tool Usage",
        "{tags.SCENARIO_ANALYSIS_NAME}",    "Member Registration",
        "{tags.SCENARIO_ANALYSIS_STEP_NUM}","{stepNumber}",
        "{tags.SERVER_CALL_IDENTIFIER}",    "{serverCallIdentifiers.SALT_CUSTOM_EVENT}"
    ],

    "LoginManager:Start": [
        "{tags.CONTENT_GROUP}",             "Member",
        "{tags.CONTENT_SUBGROUP}",          "Login",
        "{tags.SITE}",                      "salt",
        "{tags.DCS_URI_STEM}",              "/login/",
        "{tags.PAGE_TITLE}",                "Login",
        "{tags.SERVER_CALL_IDENTIFIER}",    "{serverCallIdentifiers.SALT_CUSTOM_EVENT}"
    ],

	"LoginManager:RememberMe": [
        "{tags.ACTIVITY_NAME}",             "Remember Me",
        "{tags.ACTIVITY_TYPE}",             "save",
        "{tags.ACTIVITY_TRANSACTION}",      "1",
        "{tags.SERVER_CALL_IDENTIFIER}",    "{serverCallIdentifiers.SALT_CUSTOM_EVENT}"
    ],
    "Standard:Action:Generic": [
        "{tags.ACTIVITY_NAME}",             "{activity_name}",
        "{tags.ACTIVITY_TYPE}",             "{activity_type}",
        "{tags.ACTIVITY_TRANSACTION}",      "{activity_transaction}",
        "{tags.SERVER_CALL_IDENTIFIER}",    "{serverCallIdentifiers.SALT_CUSTOM_EVENT}",
        "{tags.CONTENT_GROUP_TYPE}",        "{content_group_type}"
   ],
    "Standard:Action:Start": [
        "{tags.ACTIVITY_NAME}",             "{activity_name}",
        "{tags.ACTIVITY_TYPE}",             "start",
        "{tags.ACTIVITY_TRANSACTION}",      "1",
        "{tags.SERVER_CALL_IDENTIFIER}",    "{serverCallIdentifiers.SALT_CUSTOM_EVENT}"
   ],
   "reminder:start": [
        "{tags.DCS_URI_STEM}",               "/AddReminderStart.html",
        "{tags.PAGE_TITLE}",                 "AddReminderStart",
        "{tags.ACTIVITY_NAME}",              "addreminder",
        "{tags.ACTIVITY_TYPE}",              "start",
        "{tags.ACTIVITY_TRANSACTION}",       "1",
        "{tags.SERVER_CALL_IDENTIFIER}",     "{serverCallIdentifiers.SALT_CUSTOM_EVENT}"
   ],
   "deactivation:start": [
       "{tags.ACTIVITY_NAME}",          "manageprofileclose",
       "{tags.ACTIVITY_TYPE}",          "Self Service",
       "{tags.ACTIVITY_ACTION}",        "start",
       "{tags.ACTIVITY_TRANSACTION}",   "1",
       "{tags.SERVER_CALL_IDENTIFIER}", "{serverCallIdentifiers.SALT_CUSTOM_EVENT}"
   ],
   "deactivation:abandon": [
       "{tags.ACTIVITY_NAME}",          "Deactivation",
       "{tags.ACTIVITY_TYPE}",          "Self Service",
       "{tags.ACTIVITY_ACTION}",        "Abandon",
       "{tags.SERVER_CALL_IDENTIFIER}", "{serverCallIdentifiers.SALT_CUSTOM_EVENT}"
   ],
   "deactivation:complete": [
       "{tags.ACTIVITY_NAME}",          "Deactivation",
       "{tags.ACTIVITY_TYPE}",          "Self Service",
       "{tags.ACTIVITY_ACTION}",        "Deactivate",
       "{tags.SERVER_CALL_IDENTIFIER}", "{serverCallIdentifiers.SALT_CUSTOM_EVENT}"
   ],
   "COL:UI:Event": [
        "{tags.ACTIVITY_NAME}",          "{activityName}",
        "{tags.ACTIVITY_TRANSACTION}",   "1",
        "{tags.SERVER_CALL_IDENTIFIER}", "{serverCallIdentifiers.SALT_CUSTOM_EVENT}",
        "{tags.CONTENT_GROUP_TYPE}",     "Tool"
   ],
   "regWall": [
        "{tags.REG_WALL}",               "{action}",
        "{tags.SERVER_CALL_IDENTIFIER}", "{serverCallIdentifiers.SALT_CUSTOM_EVENT}"
   ],
   "siteMember:activation:complete": [
        "{tags.TYPE}",    "Tool Usage",
        "{tags.DCS_URI_STEM}",                      "{location.pathname}",
        "{tags.PAGE_TITLE}",                        "{document.title}",
        "{tags.EXTERNAL_VISITOR_ID}",               "{siteMember.MembershipId}",
        "{tags.DCS_AUTHENTICATED_USERNAME}",        "{siteMember.MembershipId}",
        "{tags.REGISTERED_VISITOR}",                "1",
        "{tags.ACTIVITY_NAME}",                     "activation",
        "{tags.ACTIVITY_TYPE}",                     "complete",
        "{tags.ACTIVITY_TRANSACTION}",              "1",
        "{tags.SCENARIO_ANALYSIS_NAME}",            "{registration.name}",
        "{tags.SCENARIO_ANALYSIS_STEP_NUM}",        "{registration.step}",
        "{tags.SCENARIO_ANALYSIS_CONVERSION_STEP}", "1",
        "{tags.SERVER_CALL_IDENTIFIER}",            "{serverCallIdentifiers.SALT_CUSTOM_EVENT}"
    ],

    "siteMember:signin:complete": [
        "{tags.DCS_URI_STEM}",               "{location.pathname}",
        "{tags.PAGE_TITLE}",                 "{document.title}",
        "{tags.EXTERNAL_VISITOR_ID}",        "{siteMember.MembershipId}",
        "{tags.DCS_AUTHENTICATED_USERNAME}", "{siteMember.MembershipId}",
        "{tags.REGISTERED_VISITOR}",         "1",
        "{tags.ACTIVITY_NAME}",              "signin",
        "{tags.ACTIVITY_TYPE}",              "complete",
        "{tags.ACTIVITY_TRANSACTION}",       "1",
        "{tags.SERVER_CALL_IDENTIFIER}",     "{serverCallIdentifiers.SALT_CUSTOM_EVENT}"
    ],
    "dashboard:action:taken": [
        "{tags.ACTIVITY_NAME}",             "{activity_name}",
        "{tags.ACTIVITY_SUB_NAME}",         "{activity_sub_name}",
        "{tags.ACTIVITY_TYPE}",             "{activity_type}",
        "{tags.PRODUCT_SKU}",               "{content_id}",
        "{tags.ACTIVITY_TRANSACTION}",      "SALT3",
        "{tags.SERVER_CALL_IDENTIFIER}",    "{serverCallIdentifiers.NAMED_ANCHOR}"
    ],
    "onboarding:action:taken": [
        "{tags.ACTIVITY_NAME}",                     "{activity_name}",
        "{tags.ACTIVITY_SUB_NAME}",                 "{activity_sub_name}",
        "{tags.ACTIVITY_SUB_NAME_2}",               "{activity_sub_name_2}",
        "{tags.ACTIVITY_TYPE}",                     "{activity_type}",
        "{tags.PRODUCT_SKU}",                       "{content_id}",
        "{tags.SCENARIO_ANALYSIS_NAME}",            "onboard",
        "{tags.SCENARIO_ANALYSIS_STEP_NUM}",        "{step_num}",
        "{tags.SCENARIO_ANALYSIS_CONVERSION_STEP}", "{is_final_step}",
        "{tags.ACTIVITY_TRANSACTION}",              "SALT3",
        "{tags.SERVER_CALL_IDENTIFIER}",            "{serverCallIdentifiers.NAMED_ANCHOR}"
    ]
}
;});

/*!
 * SALT / Analytics Abstraction Layer / Webtrends API Wrapper and Analytics Framework Event Logger
 */
define('salt/analytics/webtrends',[
    'jquery',
    'underscore',
    'salt/analytics/webtrends-config',
    'salt',
    'salt/models/SiteMember',
    'salt/analytics',
    'asa/ASAUtilities',
    'json!salt/analytics/webtrends/maps/main.json'
], function ($, _, WTConfig, SALT, SiteMember, Analytics, Utility, MainMap) {

    // BEGIN: Constants
    var LOGGER_ID          = 'webtrends',
        LOGGER_NAME        = 'Webtrends Event Tracker',
        LOGGER_DESCRIPTION = 'Logs select application events to the Webtrends Data Collection Server',
        Webtrends = window.Webtrends;
    // END: Constants
    if (Webtrends) {
        var dcs = Webtrends.dcss['dcsobj_' + 0],
        _wtReady = $.Deferred();

        var WT = Analytics.registerLogger({
                id:          LOGGER_ID,
                name:        LOGGER_NAME,
                description: LOGGER_DESCRIPTION,

                map: [
                    MainMap
                ],

                // Values accessible via JavaScript
                tags:                  WTConfig.tags,
                serverCallIdentifiers: WTConfig.SERVER_CALL_IDENTIFIERS,
                userStateIdentifiers:  WTConfig.USER_STATE_IDENTIFIERS,

                // Values accessible via Dust template
                context: {
                    tags:                  WTConfig.tags,
                    serverCallIdentifiers: WTConfig.SERVER_CALL_IDENTIFIERS,
                    userStateIdentifiers:  WTConfig.USER_STATE_IDENTIFIERS
                },

                getTagValue: function (tags, tagName) {
                    var tagValue;
                    var tagIndex = WTConfig.getTagIndex(tags, tagName);

                    if (tagIndex !== -1) {
                        tagValue = tags[tagIndex + 1];
                    }

                    return tagValue;
                },

                meta: function (name, value) {
                    var $meta = $('meta[name=' + name.replace(/\./g, '\\.') + ']');

                    if ($meta.length === 0) {
                        $meta = $('<meta name="' + name + '" />').appendTo('head');
                    }

                    $meta.attr('content', value);

                    return $meta;
                },

                // updates meta tags, and calls track(). Accepts an array of elements.
                updateMeta: function (parsedHTML) {
                    var tags = [],
                        name = '';

                    parsedHTML.forEach(function (element, index, array) {
                        name = element.getAttribute('name');
                        WTConfig.setTag(tags, name, element.getAttribute('content'));
                        $('meta[name=' + name.replace(/\./g, '\\.') + ']').remove();
                        $('head').append(element);
                    });

                    this.track(tags);
                },

                // END: Helper methods

                // accepts an array of tag names/values or an exploded list of tag arguments
                track: function () {
                    var self = this,
                        args = arguments,
                        tags;

                    if (args.length === 1 && typeof args[0] !== 'string') {
                        tags = args[0];
                    } else {
                        tags = [];
                        for (var i = 0; i < args.length; i++) {
                            tags.push(args[i]);
                        }
                    }

                    _wtReady.done(_.bind(function () {
                        var title = tags.indexOf(self.tags.PAGE_TITLE) === -1 ? document.title : tags[tags.indexOf(self.tags.PAGE_TITLE) + 1],
                            uriStem = tags.indexOf(self.tags.DCS_URI_STEM) === -1 ? document.location.pathname : tags[tags.indexOf(self.tags.PAGE_TITLE) + 1];
                        WTConfig.setTag(tags, self.tags.SITE, 'salt');
                        WTConfig.setTag(tags, self.tags.DCS_URI_STEM, uriStem);
                        WTConfig.setTag(tags, self.tags.EVENT_SOURCE, document.location.hostname + document.location.pathname);
                        WTConfig.setTag(tags, self.tags.DCS_URI_QUERY, Utility.getLocationSearch());
                        WTConfig.setTag(tags, self.tags.DCS_REFERRER, document.referrer);
                        WTConfig.setTag(tags, self.tags.PAGE_TITLE, title);

                        // Make sure normally empty tags are cleared if they're not explicitly defined.
                        for (var i = 0; i < WTConfig.NORMALLY_EMPTY_TAGS.length; i++) {
                            var normallyEmptyTag = WTConfig.NORMALLY_EMPTY_TAGS[i];

                            if (WTConfig.getTagIndex(tags, normallyEmptyTag) === -1) {
                                WTConfig.setTag(tags, normallyEmptyTag, '');
                            }
                        }

                        window.dcsMultiTrack.apply(window, tags);

                        // cleanup Webtrends DCS tags any time an activity has completed
                        if (self.getTagValue(tags, self.tags.ACTIVITY_TYPE) === 'complete') {
                            dcs.dcsCleanUp();
                        }
                    }, this)).fail(function () {
                        console.log(self.name + ': Webtrends prerequisites failed. Not calling dcsMultiTrack(' + tags.join(', ') + ').');
                    });
                },

                initialize: function () {
                    var self = this;

                    var _dcsCreateImage = dcs.dcsCreateImage;
                    dcs.dcsCreateImage = function (dcsSrc) {
                        _dcsCreateImage.call(dcs, dcsSrc);
                    };

                    // Set global info about user (logged in or otherwise)
                    SiteMember.done(function (siteMember) {
                        // Only proceed with population and template use if user is authenticated.
                        if (siteMember.IsAuthenticated === 'true') {
                            //Populate Webtrends field from 'siteMember' object
                            if (siteMember.Source !== null) {
                                if (siteMember.Source === 'Self-registered no match') {
                                    self.meta(self.tags.STATE, self.userStateIdentifiers.LOGGED_IN_UNMATCHED);
                                } else {
                                    self.meta(self.tags.STATE, self.userStateIdentifiers.LOGGED_IN_MATCHED);
                                }
                            }

                            if (siteMember.MembershipId !== null) {
                                $.each(WTConfig.ID_TAGS, function (i, tagName) {
                                    self.meta(tagName, siteMember.MembershipId);
                                });
                            }
                        } else {
                            self.meta(self.tags.STATE, self.userStateIdentifiers.LOGGED_OUT);
                        }

                        //Resolve promise for WT, marking as ready to start firing events, now that we have member data
                        _wtReady.resolve();
                    });
                }
            });

        // Legacy support
        if (!!SALT.global) {
            SALT.global.WT = WT;
        }

        return WT;
    }
});

/*
 * Foundation Responsive Library
 * http://foundation.zurb.com
 * Copyright 2014, ZURB
 * Free to use under the MIT license.
 * http://www.opensource.org/licenses/mit-license.php
*/

(function ($, window, document, undefined) {
  

  var header_helpers = function (class_array) {
    var i = class_array.length;
    var head = $('head');

    while (i--) {
      if($('head').has('.' + class_array[i]).length === 0) {
        $('head').append('<meta class="' + class_array[i] + '" />');
      }
    }
  };

  header_helpers([
    'foundation-mq-small',
    'foundation-mq-medium',
    'foundation-mq-large',
    'foundation-mq-xlarge',
    'foundation-mq-xxlarge',
    'foundation-data-attribute-namespace']);

  // Enable FastClick if present

  $(function() {
    if (typeof FastClick !== 'undefined') {
      // Don't attach to body if undefined
      if (typeof document.body !== 'undefined') {
        FastClick.attach(document.body);
      }
    }
  });

  // private Fast Selector wrapper,
  // returns jQuery object. Only use where
  // getElementById is not available.
  var S = function (selector, context) {
    if (typeof selector === 'string') {
      if (context) {
        var cont;
        if (context.jquery) {
          cont = context[0];
          if (!cont) return context;
        } else {
          cont = context;
        }
        return $(cont.querySelectorAll(selector));
      }

      return $(document.querySelectorAll(selector));
    }

    return $(selector, context);
  };

  // Namespace functions.

  var attr_name = function (init) {
    var arr = [];
    if (!init) arr.push('data');
    if (this.namespace.length > 0) arr.push(this.namespace);
    arr.push(this.name);

    return arr.join('-');
  };

  var add_namespace = function (str) {
    var parts = str.split('-'),
        i = parts.length,
        arr = [];

    while (i--) {
      if (i !== 0) {
        arr.push(parts[i]);
      } else {
        if (this.namespace.length > 0) {
          arr.push(this.namespace, parts[i]);
        } else {
          arr.push(parts[i]);
        }
      }
    }

    return arr.reverse().join('-');
  };

  // Event binding and data-options updating.

  var bindings = function (method, options) {
    var self = this,
        should_bind_events = !S(this).data(this.attr_name(true));

    if (typeof method === 'string') {
      return this[method].call(this, options);
    }

    if (S(this.scope).is('[' + this.attr_name() +']')) {
      S(this.scope).data(this.attr_name(true) + '-init', $.extend({}, this.settings, (options || method), this.data_options(S(this.scope))));

      if (should_bind_events) {
        this.events(this.scope);
      }

    } else {
      S('[' + this.attr_name() +']', this.scope).each(function () {
        var should_bind_events = !S(this).data(self.attr_name(true) + '-init');
        S(this).data(self.attr_name(true) + '-init', $.extend({}, self.settings, (options || method), self.data_options(S(this))));

        if (should_bind_events) {
          self.events(this);
        }
      });
    }
  };

  var single_image_loaded = function (image, callback) {
    function loaded () {
      callback(image[0]);
    }

    function bindLoad () {
      this.one('load', loaded);

      if (/MSIE (\d+\.\d+);/.test(navigator.userAgent)) {
        var src = this.attr( 'src' ),
            param = src.match( /\?/ ) ? '&' : '?';

        param += 'random=' + (new Date()).getTime();
        this.attr('src', src + param);
      }
    }

    if (!image.attr('src')) {
      loaded();
      return;
    }

    if (image[0].complete || image[0].readyState === 4) {
      loaded();
    } else {
      bindLoad.call(image);
    }
  };
  
  /*
    https://github.com/paulirish/matchMedia.js
  */

  window.matchMedia = window.matchMedia || (function( doc, undefined ) {

    

    var bool,
        docElem = doc.documentElement,
        refNode = docElem.firstElementChild || docElem.firstChild,
        // fakeBody required for <FF4 when executed in <head>
        fakeBody = doc.createElement( "body" ),
        div = doc.createElement( "div" );

    div.id = "mq-test-1";
    div.style.cssText = "position:absolute;top:-100em";
    fakeBody.style.background = "none";
    fakeBody.appendChild(div);

    return function (q) {

      div.innerHTML = "&shy;<style media=\"" + q + "\"> #mq-test-1 { width: 42px; }</style>";

      docElem.insertBefore( fakeBody, refNode );
      bool = div.offsetWidth === 42;
      docElem.removeChild( fakeBody );

      return {
        matches: bool,
        media: q
      };

    };

  }( document ));

  /*
   * jquery.requestAnimationFrame
   * https://github.com/gnarf37/jquery-requestAnimationFrame
   * Requires jQuery 1.8+
   *
   * Copyright (c) 2012 Corey Frang
   * Licensed under the MIT license.
   */

  (function($) {

  // requestAnimationFrame polyfill adapted from Erik Möller
  // fixes from Paul Irish and Tino Zijdel
  // http://paulirish.com/2011/requestanimationframe-for-smart-animating/
  // http://my.opera.com/emoller/blog/2011/12/20/requestanimationframe-for-smart-er-animating

  var animating,
      lastTime = 0,
      vendors = ['webkit', 'moz'],
      requestAnimationFrame = window.requestAnimationFrame,
      cancelAnimationFrame = window.cancelAnimationFrame,
      jqueryFxAvailable = 'undefined' !== typeof jQuery.fx;

  for (; lastTime < vendors.length && !requestAnimationFrame; lastTime++) {
    requestAnimationFrame = window[ vendors[lastTime] + "RequestAnimationFrame" ];
    cancelAnimationFrame = cancelAnimationFrame ||
      window[ vendors[lastTime] + "CancelAnimationFrame" ] || 
      window[ vendors[lastTime] + "CancelRequestAnimationFrame" ];
  }

  function raf() {
    if (animating) {
      requestAnimationFrame(raf);
      
      if (jqueryFxAvailable) {
        jQuery.fx.tick();
      }
    }
  }

  if (requestAnimationFrame) {
    // use rAF
    window.requestAnimationFrame = requestAnimationFrame;
    window.cancelAnimationFrame = cancelAnimationFrame;
    
    if (jqueryFxAvailable) {
      jQuery.fx.timer = function (timer) {
        if (timer() && jQuery.timers.push(timer) && !animating) {
          animating = true;
          raf();
        }
      };

      jQuery.fx.stop = function () {
        animating = false;
      };
    }
  } else {
    // polyfill
    window.requestAnimationFrame = function (callback, element) {
      var currTime = new Date().getTime(),
        timeToCall = Math.max(0, 16 - (currTime - lastTime)),
        id = window.setTimeout(function () {
          callback(currTime + timeToCall);
        }, timeToCall);
      lastTime = currTime + timeToCall;
      return id;
    };

    window.cancelAnimationFrame = function (id) {
      clearTimeout(id);
    };

  }

  }( jQuery ));


  function removeQuotes (string) {
    if (typeof string === 'string' || string instanceof String) {
      string = string.replace(/^['\\/"]+|(;\s?})+|['\\/"]+$/g, '');
    }

    return string;
  }

  window.Foundation = {
    name : 'Foundation',

    version : '5.2.1',

    media_queries : {
      small : S('.foundation-mq-small').css('font-family').replace(/^[\/\\'"]+|(;\s?})+|[\/\\'"]+$/g, ''),
      medium : S('.foundation-mq-medium').css('font-family').replace(/^[\/\\'"]+|(;\s?})+|[\/\\'"]+$/g, ''),
      large : S('.foundation-mq-large').css('font-family').replace(/^[\/\\'"]+|(;\s?})+|[\/\\'"]+$/g, ''),
      xlarge: S('.foundation-mq-xlarge').css('font-family').replace(/^[\/\\'"]+|(;\s?})+|[\/\\'"]+$/g, ''),
      xxlarge: S('.foundation-mq-xxlarge').css('font-family').replace(/^[\/\\'"]+|(;\s?})+|[\/\\'"]+$/g, '')
    },

    stylesheet : $('<style></style>').appendTo('head')[0].sheet,

    global: {
      namespace: ''
    },

    init : function (scope, libraries, method, options, response) {
      var library_arr,
          args = [scope, method, options, response],
          responses = [];

      // check RTL
      this.rtl = /rtl/i.test(S('html').attr('dir'));

      // set foundation global scope
      this.scope = scope || this.scope;

      this.set_namespace();

      if (libraries && typeof libraries === 'string' && !/reflow/i.test(libraries)) {
        if (this.libs.hasOwnProperty(libraries)) {
          responses.push(this.init_lib(libraries, args));
        }
      } else {
        for (var lib in this.libs) {
          responses.push(this.init_lib(lib, libraries));
        }
      }

      return scope;
    },

    init_lib : function (lib, args) {
      if (this.libs.hasOwnProperty(lib)) {
        this.patch(this.libs[lib]);

        if (args && args.hasOwnProperty(lib)) {
            if (typeof this.libs[lib].settings !== 'undefined') {
                $.extend(true, this.libs[lib].settings, args[lib]);
            }
            else if (typeof this.libs[lib].defaults !== 'undefined') {
                $.extend(true, this.libs[lib].defaults, args[lib]);
            }
          return this.libs[lib].init.apply(this.libs[lib], [this.scope, args[lib]]);
        }

        args = args instanceof Array ? args : Array(args);    // PATCH: added this line
        return this.libs[lib].init.apply(this.libs[lib], args);
      }

      return function () {};
    },

    patch : function (lib) {
      lib.scope = this.scope;
      lib.namespace = this.global.namespace;
      lib.rtl = this.rtl;
      lib['data_options'] = this.utils.data_options;
      lib['attr_name'] = attr_name;
      lib['add_namespace'] = add_namespace;
      lib['bindings'] = bindings;
      lib['S'] = this.utils.S;
    },

    inherit : function (scope, methods) {
      var methods_arr = methods.split(' '),
          i = methods_arr.length;

      while (i--) {
        if (this.utils.hasOwnProperty(methods_arr[i])) {
          scope[methods_arr[i]] = this.utils[methods_arr[i]];
        }
      }
    },

    set_namespace: function () {

      // Don't bother reading the namespace out of the meta tag
      // if the namespace has been set globally in javascript
      //
      // Example: something like Foundation.global.namespace = 'my-namespace';
      //
      // Otherwise, if the namespace hasn't been set globally,
      // read it out of the meta tag
      //
      var namespace = this.global.namespace || $('.foundation-data-attribute-namespace').css('font-family');

      if (/false/i.test(namespace)) return;
      
      this.global.namespace = namespace;
    },

    libs : {},

    // methods that can be inherited in libraries
    utils : {

      // Description:
      //    Fast Selector wrapper returns jQuery object. Only use where getElementById 
      //    is not available.
      //
      // Arguments:
      //    Selector (String): CSS selector describing the element(s) to be 
      //    returned as a jQuery object.
      //
      //    Scope (String): CSS selector describing the area to be searched. Default 
      //    is document.
      //
      // Returns:
      //    Element (jQuery Object): jQuery object containing elements matching the 
      //    selector within the scope.
      S : S,

      // Description:
      //    Executes a function a max of once every n milliseconds 
      //
      // Arguments:
      //    Func (Function): Function to be throttled.
      //
      //    Delay (Integer): Function execution threshold in milliseconds.
      //
      // Returns:
      //    Lazy_function (Function): Function with throttling applied.
      throttle : function (func, delay) {
        var timer = null;

        return function () {
          var context = this, args = arguments;

          clearTimeout(timer);
          timer = setTimeout(function () {
            func.apply(context, args);
          }, delay);
        };
      },

      // Description:
      //    Executes a function when it stops being invoked for n seconds
      //    Modified version of _.debounce() http://underscorejs.org
      //
      // Arguments:
      //    Func (Function): Function to be debounced.
      //
      //    Delay (Integer): Function execution threshold in milliseconds.
      // 
      //    Immediate (Bool): Whether the function should be called at the beginning 
      //    of the delay instead of the end. Default is false.
      //
      // Returns:
      //    Lazy_function (Function): Function with debouncing applied.
      debounce : function (func, delay, immediate) {
        var timeout, result;
        return function () {
          var context = this, args = arguments;
          var later = function () {
            timeout = null;
            if (!immediate) result = func.apply(context, args);
          };
          var callNow = immediate && !timeout;
          clearTimeout(timeout);
          timeout = setTimeout(later, delay);
          if (callNow) result = func.apply(context, args);
          return result;
        };
      },

      // Description:
      //    Parses data-options attribute
      //
      // Arguments:
      //    El (jQuery Object): Element to be parsed.
      //
      // Returns:
      //    Options (Javascript Object): Contents of the element's data-options 
      //    attribute.
      data_options : function (el) {
        var opts = {}, ii, p, opts_arr,
            data_options = function (el) {
              var namespace = Foundation.global.namespace;

              if (namespace.length > 0) {
                return el.data(namespace + '-options');
              }

              return el.data('options');
            };

        var cached_options = data_options(el);

        if (typeof cached_options === 'object') {
          return cached_options;
        }

        opts_arr = (cached_options || ':').split(';'),
        ii = opts_arr.length;

        function isNumber (o) {
          return ! isNaN (o-0) && o !== null && o !== "" && o !== false && o !== true;
        }

        function trim (str) {
          if (typeof str === 'string') return $.trim(str);
          return str;
        }

        while (ii--) {
          p = opts_arr[ii].split(':');

          if (/true/i.test(p[1])) p[1] = true;
          if (/false/i.test(p[1])) p[1] = false;
          if (isNumber(p[1])) {
            if (p[1].indexOf('.') === -1) {
              p[1] = parseInt(p[1], 10);
            } else {
              p[1] = parseFloat(p[1], 10);
            }
          }

          if (p.length === 2 && p[0].length > 0) {
            opts[trim(p[0])] = trim(p[1]);
          }
        }

        return opts;
      },

      // Description:
      //    Adds JS-recognizable media queries
      //
      // Arguments:
      //    Media (String): Key string for the media query to be stored as in 
      //    Foundation.media_queries
      //
      //    Class (String): Class name for the generated <meta> tag
      register_media : function (media, media_class) {
        if(Foundation.media_queries[media] === undefined) {
          $('head').append('<meta class="' + media_class + '">');
          Foundation.media_queries[media] = removeQuotes($('.' + media_class).css('font-family'));
        }
      },

      // Description:
      //    Add custom CSS within a JS-defined media query
      //
      // Arguments:
      //    Rule (String): CSS rule to be appended to the document.
      //
      //    Media (String): Optional media query string for the CSS rule to be 
      //    nested under.
      add_custom_rule : function (rule, media) {
        if (media === undefined) {
          Foundation.stylesheet.insertRule(rule, Foundation.stylesheet.cssRules.length);
        } else {
          var query = Foundation.media_queries[media];

          if (query !== undefined) {
            Foundation.stylesheet.insertRule('@media ' + 
              Foundation.media_queries[media] + '{ ' + rule + ' }');
          }
        }
      },

      // Description:
      //    Performs a callback function when an image is fully loaded
      //
      // Arguments:
      //    Image (jQuery Object): Image(s) to check if loaded.
      //
      //    Callback (Function): Fundation to execute when image is fully loaded.
      image_loaded : function (images, callback) {
        var self = this,
            unloaded = images.length;

        if (unloaded === 0) {
          callback(images);
        }

        images.each(function () {
          single_image_loaded(self.S(this), function () {
            unloaded -= 1;
            if (unloaded === 0) {
              callback(images);
            }
          });
        });
      },

      // Description:
      //    Returns a random, alphanumeric string
      //
      // Arguments:
      //    Length (Integer): Length of string to be generated. Defaults to random 
      //    integer.
      //
      // Returns:
      //    Rand (String): Pseudo-random, alphanumeric string.
      random_str : function () {
        if (!this.fidx) this.fidx = 0;
        this.prefix = this.prefix || [(this.name || 'F'), (+new Date).toString(36)].join('-');

        return this.prefix + (this.fidx++).toString(36);
      }
    }
  };

  $.fn.foundation = function () {
    var args = Array.prototype.slice.call(arguments, 0);

    return this.each(function () {
      Foundation.init.apply(Foundation, [this].concat(args));
      return this;
    });
  };

}(jQuery, this, this.document));

define("foundation.lib", ["jquery"], function(){});

;(function ($, window, document, undefined) {
  

  Foundation.libs.abide = {
    name : 'abide',

    version : '5.2.1',

    settings : {
      live_validate : true,
      focus_on_invalid : true,
      error_labels: true, // labels with a for="inputId" will recieve an `error` class
      timeout : 1000,
      patterns : {
        alpha: /^[a-zA-Z]+$/,
        alpha_numeric : /^[a-zA-Z0-9]+$/,
        integer: /^\d+$/,
        number: /-?(?:\d+|\d{1,3}(?:,\d{3})+)?(?:\.\d+)?/,

        // amex, visa, diners
        card : /^(?:4[0-9]{12}(?:[0-9]{3})?|5[1-5][0-9]{14}|6(?:011|5[0-9][0-9])[0-9]{12}|3[47][0-9]{13}|3(?:0[0-5]|[68][0-9])[0-9]{11}|(?:2131|1800|35\d{3})\d{11})$/,
        cvv : /^([0-9]){3,4}$/,

        // http://www.whatwg.org/specs/web-apps/current-work/multipage/states-of-the-type-attribute.html#valid-e-mail-address
        email : /^[a-zA-Z0-9.!#$%&'*+\/=?^_`{|}~-]+@[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?(?:\.[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?)*$/,

        url: /(https?|ftp|file|ssh):\/\/(((([a-zA-Z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(%[\da-f]{2})|[!\$&'\(\)\*\+,;=]|:)*@)?(((\d|[1-9]\d|1\d\d|2[0-4]\d|25[0-5])\.(\d|[1-9]\d|1\d\d|2[0-4]\d|25[0-5])\.(\d|[1-9]\d|1\d\d|2[0-4]\d|25[0-5])\.(\d|[1-9]\d|1\d\d|2[0-4]\d|25[0-5]))|((([a-zA-Z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-zA-Z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-zA-Z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-zA-Z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.)+(([a-zA-Z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-zA-Z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-zA-Z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-zA-Z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.?)(:\d*)?)(\/((([a-zA-Z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(%[\da-f]{2})|[!\$&'\(\)\*\+,;=]|:|@)+(\/(([a-zA-Z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(%[\da-f]{2})|[!\$&'\(\)\*\+,;=]|:|@)*)*)?)?(\?((([a-zA-Z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(%[\da-f]{2})|[!\$&'\(\)\*\+,;=]|:|@)|[\uE000-\uF8FF]|\/|\?)*)?(\#((([a-zA-Z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(%[\da-f]{2})|[!\$&'\(\)\*\+,;=]|:|@)|\/|\?)*)?/,
        // abc.de
        domain: /^([a-zA-Z0-9]([a-zA-Z0-9\-]{0,61}[a-zA-Z0-9])?\.)+[a-zA-Z]{2,6}$/,

        datetime: /([0-2][0-9]{3})\-([0-1][0-9])\-([0-3][0-9])T([0-5][0-9])\:([0-5][0-9])\:([0-5][0-9])(Z|([\-\+]([0-1][0-9])\:00))/,
        // YYYY-MM-DD
        date: /(?:19|20)[0-9]{2}-(?:(?:0[1-9]|1[0-2])-(?:0[1-9]|1[0-9]|2[0-9])|(?:(?!02)(?:0[1-9]|1[0-2])-(?:30))|(?:(?:0[13578]|1[02])-31))/,
        // HH:MM:SS
        time : /(0[0-9]|1[0-9]|2[0-3])(:[0-5][0-9]){2}/,
        dateISO: /\d{4}[\/\-]\d{1,2}[\/\-]\d{1,2}/,
        // MM/DD/YYYY
        month_day_year : /(0[1-9]|1[012])[- \/.](0[1-9]|[12][0-9]|3[01])[- \/.](19|20)\d\d/,

        // #FFF or #FFFFFF
        color: /^#?([a-fA-F0-9]{6}|[a-fA-F0-9]{3})$/
      },
      validators : {
        equalTo: function(el, required, parent) {
          var from  = document.getElementById(el.getAttribute(this.add_namespace('data-equalto'))).value,
              to    = el.value,
              valid = (from === to);

          return valid;
        }
      }
    },

    timer : null,

    init : function (scope, method, options) {
      this.bindings(method, options);
    },

    events : function (scope) {
      var self = this,
          form = self.S(scope).attr('novalidate', 'novalidate'),
          settings = form.data(this.attr_name(true) + '-init');

      this.invalid_attr = this.add_namespace('data-invalid');

      form
        .off('.abide')
        .on('submit.fndtn.abide validate.fndtn.abide', function (e) {
          var is_ajax = /ajax/i.test(self.S(this).attr(self.attr_name()));
          return self.validate(self.S(this).find('input, textarea, select').get(), e, is_ajax);
        })
        .on('reset', function() {
          return self.reset($(this));
        })
        .find('input, textarea, select')
          .off('.abide')
          .on('change.fndtn.abide', function (e) {
            self.validate([this], e);
          })
          .on('keydown.fndtn.abide', function (e) {
            var settings = $(this).closest('form').data(self.attr_name(true) + '-init');
            if (settings.live_validate === true) {
              clearTimeout(self.timer);
              self.timer = setTimeout(function () {
                self.validate([this], e);
              }.bind(this), settings.timeout);
            }
          });
    },

    reset : function (form) {
      form.removeAttr(this.invalid_attr);
      $(this.invalid_attr, form).removeAttr(this.invalid_attr);
      $('.error', form).not('small').removeClass('error');
    },

    validate : function (els, e, is_ajax) {
      if ($.browser.msie) {
        for (var i = 0; i < els.length; i++) {
          if (els[i].value) {
            els[i].value = $(els[i]).val();
          }
        }
      }
      var validations = this.parse_patterns(els),
          validation_count = validations.length,
          form = this.S(els[0]).closest('form'),
          submit_event = /submit/.test(e.type);

      // Has to count up to make sure the focus gets applied to the top error
      for (var i=0; i < validation_count; i++) {
        if (!validations[i] && (submit_event || is_ajax)) {
          if (this.settings.focus_on_invalid) els[i].focus();
          form.trigger('invalid');
          this.S(els[i]).closest('form').attr(this.invalid_attr, '');
          return false;
        }
      }

      if (submit_event || is_ajax) {
        form.trigger('valid');
      }

      form.removeAttr(this.invalid_attr);

      if (is_ajax) return false;

      return true;
    },

    parse_patterns : function (els) {
      var i = els.length,
          el_patterns = [];

      while (i--) {
        el_patterns.push(this.pattern(els[i]));
      }

      return this.check_validation_and_apply_styles(el_patterns);
    },

    pattern : function (el) {
      var type = el.getAttribute('type'),
          required = typeof el.getAttribute('required') === 'string';

      var pattern = el.getAttribute('pattern') || '';

      if (this.settings.patterns.hasOwnProperty(pattern) && pattern.length > 0) {
        return [el, this.settings.patterns[pattern], required];
      } else if (pattern.length > 0) {
        return [el, new RegExp(pattern), required];
      }

      if (this.settings.patterns.hasOwnProperty(type)) {
        return [el, this.settings.patterns[type], required];
      }

      pattern = /.*/;

      return [el, pattern, required];
    },

    check_validation_and_apply_styles : function (el_patterns) {
      var i = el_patterns.length,
          validations = [];

      while (i--) {
        var el = el_patterns[i][0],
            required = el_patterns[i][2],
            value = el.value,
            direct_parent = this.S(el).parent(),
            validator = el.getAttribute(this.add_namespace('data-abide-validator')),
            is_radio = el.type === "radio",
            is_checkbox = el.type === "checkbox",
            label = this.S('label[for="' + el.getAttribute('id') + '"]'),
            valid_length = (required) ? (el.value.length > 0) : true;

        var parent, valid;

        // support old way to do equalTo validations
        if(el.getAttribute(this.add_namespace('data-equalto'))) { validator = "equalTo" }

        if (!direct_parent.is('label')) {
          parent = direct_parent;
        } else {
          parent = direct_parent.parent();
        }

        if (is_radio && required) {
          validations.push(this.valid_radio(el, required));
        } else if (is_checkbox && required) {
          validations.push(this.valid_checkbox(el, required));
        } else if (validator) {
          valid = this.settings.validators[validator].apply(this, [el, required, parent])
          validations.push(valid);

          if (valid) {
            this.S(el).removeAttr(this.invalid_attr);
            parent.removeClass('error');
          } else {
            this.S(el).attr(this.invalid_attr, '');
            parent.addClass('error');
          }
        } else {

          if (el_patterns[i][1].test(value) && valid_length ||
            !required && el.value.length < 1 || $(el).attr('disabled')) {
            this.S(el).removeAttr(this.invalid_attr);
            parent.removeClass('error');
            if (label.length > 0 && this.settings.error_labels) label.removeClass('error');

            validations.push(true);
            $(el).triggerHandler('valid');
          } else {
            this.S(el).attr(this.invalid_attr, '');
            parent.addClass('error');
            if (label.length > 0 && this.settings.error_labels) label.addClass('error');

            validations.push(false);
            $(el).triggerHandler('invalid');
          }
        }
      }

      return validations;
    },

    valid_checkbox : function(el, required) {
      var el = this.S(el),
          valid = (el.is(':checked') || !required);

      if (valid) {
        el.removeAttr(this.invalid_attr).parent().removeClass('error');
      } else {
        el.attr(this.invalid_attr, '').parent().addClass('error');
      }

      return valid;
    },

    valid_radio : function (el, required) {
      var name = el.getAttribute('name'),
          group = document.getElementsByName(name),
          count = group.length,
          valid = false;

      // Has to count up to make sure the focus gets applied to the top error
      for (var i=0; i < count; i++) {
        if (group[i].checked) valid = true;
      }

      // Has to count up to make sure the focus gets applied to the top error
      for (var i=0; i < count; i++) {
        if (valid) {
          this.S(group[i]).removeAttr(this.invalid_attr).parent().removeClass('error');
        } else {
          this.S(group[i]).attr(this.invalid_attr, '').parent().addClass('error');
        }
      }

      return valid;
    }
  };
}(jQuery, this, this.document));

;(function ($, window, document, undefined) {
  

  Foundation.libs.accordion = {
    name : 'accordion',

    version : '5.2.1',

    settings : {
      active_class: 'active',
      toggleable: true
    },

    init : function (scope, method, options) {
      this.bindings(method, options);
    },

    events : function () {
      var self = this;
      var S = this.S;
      S(this.scope)
      .off('.fndtn.accordion')
      .on('click.fndtn.accordion', '[' + this.attr_name() + '] dd > a', function (e) {
        var accordion = S(this).closest('[' + self.attr_name() + ']'),
            target = S('#' + this.href.split('#')[1]),
            siblings = S('dd > .content', accordion),
            aunts = $('> dd', accordion),
            settings = accordion.data(self.attr_name(true) + '-init'),
            active_content = S('dd > .content.' + settings.active_class, accordion),
            active_parent = S('dd.' + settings.active_class, accordion);
        e.preventDefault();

        if (! S(this).closest('dl').is(accordion)) { return; }

        if (active_content[0] == target[0] && settings.toggleable) {
          active_parent.toggleClass(settings.active_class, false);
          return target.toggleClass(settings.active_class, false);
        }

        siblings.removeClass(settings.active_class);
        aunts.removeClass(settings.active_class);
        target.addClass(settings.active_class).parent().addClass(settings.active_class);
      });
    },

    off : function () {},

    reflow : function () {}
  };
}(jQuery, this, this.document));

;(function ($, window, document, undefined) {
  

  Foundation.libs.alert = {
    name : 'alert',

    version : '5.5.3',

    settings : {
      callback : function () {}
    },

    init : function (scope, method, options) {
      this.bindings(method, options);
    },

    events : function () {
      var self = this,
          S = this.S;

      $(this.scope).off('.alert').on('click.fndtn.alert', '[' + this.attr_name() + '] .close', function (e) {
        var alertBox = S(this).closest('[' + self.attr_name() + ']'),
            settings = alertBox.data(self.attr_name(true) + '-init') || self.settings;

        e.preventDefault();
        if (Modernizr.csstransitions) {
          alertBox.addClass('alert-close');
          alertBox.on('transitionend webkitTransitionEnd oTransitionEnd', function (e) {
            S(this).trigger('close.fndtn.alert').remove();
            settings.callback();
          });
        } else {
          alertBox.fadeOut(300, function () {
            S(this).trigger('close.fndtn.alert').remove();
            settings.callback();
          });
        }
      });
    },

    reflow : function () {}
  };
}(jQuery, window, window.document));

;(function ($, window, document, undefined) {
  

  Foundation.libs.dropdown = {
    name : 'dropdown',

    version : '5.5.3',

    settings : {
      active_class : 'open',
      disabled_class : 'disabled',
      mega_class : 'mega',
      align : 'bottom',
      is_hover : false,
      hover_timeout : 150,
      opened : function () {},
      closed : function () {}
    },

    init : function (scope, method, options) {
      Foundation.inherit(this, 'throttle');

      $.extend(true, this.settings, method, options);
      this.bindings(method, options);
    },

    events : function (scope) {
      var self = this,
          S = self.S;

      S(this.scope)
        .off('.dropdown')
        .on('click.fndtn.dropdown', '[' + this.attr_name() + ']', function (e) {
          var settings = S(this).data(self.attr_name(true) + '-init') || self.settings;
          if (!settings.is_hover || Modernizr.touch) {
            e.preventDefault();
            if (S(this).parent('[data-reveal-id]').length) {
              e.stopPropagation();
            }
            self.toggle($(this));
          }
        })
        .on('mouseenter.fndtn.dropdown', '[' + this.attr_name() + '], [' + this.attr_name() + '-content]', function (e) {
          var $this = S(this),
              dropdown,
              target;

          clearTimeout(self.timeout);

          if ($this.data(self.data_attr())) {
            dropdown = S('#' + $this.data(self.data_attr()));
            target = $this;
          } else {
            dropdown = $this;
            target = S('[' + self.attr_name() + '="' + dropdown.attr('id') + '"]');
          }

          var settings = target.data(self.attr_name(true) + '-init') || self.settings;

          if (S(e.currentTarget).data(self.data_attr()) && settings.is_hover) {
            self.closeall.call(self);
          }

          if (settings.is_hover) {
            self.open.apply(self, [dropdown, target]);
          }
        })
        .on('mouseleave.fndtn.dropdown', '[' + this.attr_name() + '], [' + this.attr_name() + '-content]', function (e) {
          var $this = S(this);
          var settings;

          if ($this.data(self.data_attr())) {
              settings = $this.data(self.data_attr(true) + '-init') || self.settings;
          } else {
              var target   = S('[' + self.attr_name() + '="' + S(this).attr('id') + '"]'),
                  settings = target.data(self.attr_name(true) + '-init') || self.settings;
          }

          self.timeout = setTimeout(function () {
            if ($this.data(self.data_attr())) {
              if (settings.is_hover) {
                self.close.call(self, S('#' + $this.data(self.data_attr())));
              }
            } else {
              if (settings.is_hover) {
                self.close.call(self, $this);
              }
            }
          }.bind(this), settings.hover_timeout);
        })
        .on('click.fndtn.dropdown', function (e) {
          var parent = S(e.target).closest('[' + self.attr_name() + '-content]');
          var links  = parent.find('a');

          if (links.length > 0 && parent.attr('aria-autoclose') !== 'false') {
              self.close.call(self, S('[' + self.attr_name() + '-content]'));
          }

          if (e.target !== document && !$.contains(document.documentElement, e.target)) {
            return;
          }

          if (S(e.target).closest('[' + self.attr_name() + ']').length > 0) {
            return;
          }

          if (!(S(e.target).data('revealId')) &&
            (parent.length > 0 && (S(e.target).is('[' + self.attr_name() + '-content]') ||
              $.contains(parent.first()[0], e.target)))) {
            e.stopPropagation();
            return;
          }

          self.close.call(self, S('[' + self.attr_name() + '-content]'));
        })
        .on('opened.fndtn.dropdown', '[' + self.attr_name() + '-content]', function () {
          self.settings.opened.call(this);
        })
        .on('closed.fndtn.dropdown', '[' + self.attr_name() + '-content]', function () {
          self.settings.closed.call(this);
        });

      S(window)
        .off('.dropdown')
        .on('resize.fndtn.dropdown', self.throttle(function () {
          self.resize.call(self);
        }, 50));

      this.resize();
    },

    close : function (dropdown) {
      var self = this;
      dropdown.each(function (idx) {
        var original_target = $('[' + self.attr_name() + '=' + dropdown[idx].id + ']') || $('aria-controls=' + dropdown[idx].id + ']');
        original_target.attr('aria-expanded', 'false');
        if (self.S(this).hasClass(self.settings.active_class)) {
          self.S(this)
            .css(Foundation.rtl ? 'right' : 'left', '-99999px')
            .attr('aria-hidden', 'true')
            .removeClass(self.settings.active_class)
            .prev('[' + self.attr_name() + ']')
            .removeClass(self.settings.active_class)
            .removeData('target');

          self.S(this).trigger('closed.fndtn.dropdown', [dropdown]);
        }
      });
      dropdown.removeClass('f-open-' + this.attr_name(true));
    },

    closeall : function () {
      var self = this;
      $.each(self.S('.f-open-' + this.attr_name(true)), function () {
        self.close.call(self, self.S(this));
      });
    },

    open : function (dropdown, target) {
      this
        .css(dropdown
        .addClass(this.settings.active_class), target);
      dropdown.prev('[' + this.attr_name() + ']').addClass(this.settings.active_class);
      dropdown.data('target', target.get(0)).trigger('opened.fndtn.dropdown', [dropdown, target]);
      dropdown.attr('aria-hidden', 'false');
      target.attr('aria-expanded', 'true');
      dropdown.focus();
      dropdown.addClass('f-open-' + this.attr_name(true));
    },

    data_attr : function () {
      if (this.namespace.length > 0) {
        return this.namespace + '-' + this.name;
      }

      return this.name;
    },

    toggle : function (target) {
      if (target.hasClass(this.settings.disabled_class)) {
        return;
      }
      var dropdown = this.S('#' + target.data(this.data_attr()));
      if (dropdown.length === 0) {
        // No dropdown found, not continuing
        return;
      }

      this.close.call(this, this.S('[' + this.attr_name() + '-content]').not(dropdown));

      if (dropdown.hasClass(this.settings.active_class)) {
        this.close.call(this, dropdown);
        if (dropdown.data('target') !== target.get(0)) {
          this.open.call(this, dropdown, target);
        }
      } else {
        this.open.call(this, dropdown, target);
      }
    },

    resize : function () {
      var dropdown = this.S('[' + this.attr_name() + '-content].open');
      var target = $(dropdown.data("target"));

      if (dropdown.length && target.length) {
        this.css(dropdown, target);
      }
    },

    css : function (dropdown, target) {
      var left_offset = Math.max((target.width() - dropdown.width()) / 2, 8),
          settings = target.data(this.attr_name(true) + '-init') || this.settings,
          parentOverflow = dropdown.parent().css('overflow-y') || dropdown.parent().css('overflow');

      this.clear_idx();



      if (this.small()) {
        var p = this.dirs.bottom.call(dropdown, target, settings);

        dropdown.attr('style', '').removeClass('drop-left drop-right drop-top').css({
          position : 'absolute',
          width : '95%',
          'max-width' : 'none',
          top : p.top
        });

        dropdown.css(Foundation.rtl ? 'right' : 'left', left_offset);
      }
      // detect if dropdown is in an overflow container
      else if (parentOverflow !== 'visible') {
        var offset = target[0].offsetTop + target[0].offsetHeight;

        dropdown.attr('style', '').css({
          position : 'absolute',
          top : offset
        });

        dropdown.css(Foundation.rtl ? 'right' : 'left', left_offset);
      }
      else {

        this.style(dropdown, target, settings);
      }

      return dropdown;
    },

    style : function (dropdown, target, settings) {
      var css = $.extend({position : 'absolute'},
        this.dirs[settings.align].call(dropdown, target, settings));

      dropdown.attr('style', '').css(css);
    },

    // return CSS property object
    // `this` is the dropdown
    dirs : {
      // Calculate target offset
      _base : function (t, s) {
        var o_p = this.offsetParent(),
            o = o_p.offset(),
            p = t.offset();

        p.top -= o.top;
        p.left -= o.left;

        //set some flags on the p object to pass along
        p.missRight = false;
        p.missTop = false;
        p.missLeft = false;
        p.leftRightFlag = false;

        //lets see if the panel will be off the screen
        //get the actual width of the page and store it
        var actualBodyWidth;
        var windowWidth = window.innerWidth;
        
        if (document.getElementsByClassName('row')[0]) {
          actualBodyWidth = document.getElementsByClassName('row')[0].clientWidth;
        } else {
          actualBodyWidth = windowWidth;
        }

        var actualMarginWidth = (windowWidth - actualBodyWidth) / 2;
        var actualBoundary = actualBodyWidth;

        if (!this.hasClass('mega') && !s.ignore_repositioning) {
          var outerWidth = this.outerWidth();
          var o_left = t.offset().left;
      
          //miss top
          if (t.offset().top <= this.outerHeight()) {
            p.missTop = true;
            actualBoundary = windowWidth - actualMarginWidth;
            p.leftRightFlag = true;
          }

          //miss right
          if (o_left + outerWidth > o_left + actualMarginWidth && o_left - actualMarginWidth > outerWidth) {
            p.missRight = true;
            p.missLeft = false;
          }

          //miss left
          if (o_left - outerWidth <= 0) {
            p.missLeft = true;
            p.missRight = false;
          }
        }

        return p;
      },

      top : function (t, s) {
        var self = Foundation.libs.dropdown,
            p = self.dirs._base.call(this, t, s);

        this.addClass('drop-top');

        if (p.missTop == true) {
          p.top = p.top + t.outerHeight() + this.outerHeight();
          this.removeClass('drop-top');
        }

        if (p.missRight == true) {
          p.left = p.left - this.outerWidth() + t.outerWidth();
        }

        if (t.outerWidth() < this.outerWidth() || self.small() || this.hasClass(s.mega_menu)) {
          self.adjust_pip(this, t, s, p);
        }

        if (Foundation.rtl) {
          return {left : p.left - this.outerWidth() + t.outerWidth(),
            top : p.top - this.outerHeight()};
        }

        return {left : p.left, top : p.top - this.outerHeight()};
      },

      bottom : function (t, s) {
        var self = Foundation.libs.dropdown,
            p = self.dirs._base.call(this, t, s);

        if (p.missRight == true) {
          p.left = p.left - this.outerWidth() + t.outerWidth();
        }

        if (t.outerWidth() < this.outerWidth() || self.small() || this.hasClass(s.mega_menu)) {
          self.adjust_pip(this, t, s, p);
        }

        if (self.rtl) {
          return {left : p.left - this.outerWidth() + t.outerWidth(), top : p.top + t.outerHeight()};
        }

        return {left : p.left, top : p.top + t.outerHeight()};
      },

      left : function (t, s) {
        var p = Foundation.libs.dropdown.dirs._base.call(this, t, s);

        this.addClass('drop-left');

        if (p.missLeft == true) {
          p.left =  p.left + this.outerWidth();
          p.top = p.top + t.outerHeight();
          this.removeClass('drop-left');
        }

        return {left : p.left - this.outerWidth(), top : p.top};
      },

      right : function (t, s) {
        var p = Foundation.libs.dropdown.dirs._base.call(this, t, s);

        this.addClass('drop-right');

        if (p.missRight == true) {
          p.left = p.left - this.outerWidth();
          p.top = p.top + t.outerHeight();
          this.removeClass('drop-right');
        } else {
          p.triggeredRight = true;
        }

        var self = Foundation.libs.dropdown;

        if (t.outerWidth() < this.outerWidth() || self.small() || this.hasClass(s.mega_menu)) {
          self.adjust_pip(this, t, s, p);
        }

        return {left : p.left + t.outerWidth(), top : p.top};
      }
    },

    // Insert rule to style psuedo elements
    adjust_pip : function (dropdown, target, settings, position) {
      var sheet = Foundation.stylesheet,
          pip_offset_base = 8;

      if (dropdown.hasClass(settings.mega_class)) {
        pip_offset_base = position.left + (target.outerWidth() / 2) - 8;
      } else if (this.small()) {
        pip_offset_base += position.left - 8;
      }

      this.rule_idx = sheet.cssRules.length;

      //default
      var sel_before = '.f-dropdown.open:before',
          sel_after  = '.f-dropdown.open:after',
          css_before = 'left: ' + pip_offset_base + 'px;',
          css_after  = 'left: ' + (pip_offset_base - 1) + 'px;';

      if (position.missRight == true) {
        pip_offset_base = dropdown.outerWidth() - 23;
        sel_before = '.f-dropdown.open:before',
        sel_after  = '.f-dropdown.open:after',
        css_before = 'left: ' + pip_offset_base + 'px;',
        css_after  = 'left: ' + (pip_offset_base - 1) + 'px;';
      }

      //just a case where right is fired, but its not missing right
      if (position.triggeredRight == true) {
        sel_before = '.f-dropdown.open:before',
        sel_after  = '.f-dropdown.open:after',
        css_before = 'left:-12px;',
        css_after  = 'left:-14px;';
      }

      if (sheet.insertRule) {
        sheet.insertRule([sel_before, '{', css_before, '}'].join(' '), this.rule_idx);
        sheet.insertRule([sel_after, '{', css_after, '}'].join(' '), this.rule_idx + 1);
      } else {
        sheet.addRule(sel_before, css_before, this.rule_idx);
        sheet.addRule(sel_after, css_after, this.rule_idx + 1);
      }
    },

    // Remove old dropdown rule index
    clear_idx : function () {
      var sheet = Foundation.stylesheet;

      if (typeof this.rule_idx !== 'undefined') {
        sheet.deleteRule(this.rule_idx);
        sheet.deleteRule(this.rule_idx);
        delete this.rule_idx;
      }
    },

    small : function () {
      return matchMedia(Foundation.media_queries.small).matches &&
        !matchMedia(Foundation.media_queries.medium).matches;
    },

    off : function () {
      this.S(this.scope).off('.fndtn.dropdown');
      this.S('html, body').off('.fndtn.dropdown');
      this.S(window).off('.fndtn.dropdown');
      this.S('[data-dropdown-content]').off('.fndtn.dropdown');
    },

    reflow : function () {}
  };
}(jQuery, window, window.document));

;(function ($, window, document, undefined) {
  

  Foundation.libs.equalizer = {
    name : 'equalizer',

    version : '5.2.1',

    settings : {
      use_tallest: true,
      before_height_change: $.noop,
      after_height_change: $.noop
    },

    init : function (scope, method, options) {
      this.bindings(method, options);
      this.reflow();
    },

    events : function () {
      this.S(window).off('.equalizer').on('resize.fndtn.equalizer', function(e){
        this.reflow();
      }.bind(this));
    },

    equalize: function(equalizer) {
      var isStacked = false,
          vals = equalizer.find('[' + this.attr_name() + '-watch]'),
          firstTopOffset = vals.first().offset().top,
          settings = equalizer.data(this.attr_name(true)+'-init');

      if (vals.length === 0) return;
      settings.before_height_change();
      equalizer.trigger('before-height-change');
      vals.height('inherit');
      vals.each(function(){
        var el = $(this);
        if (el.offset().top !== firstTopOffset) {
          isStacked = true;
        }
      });
      if (isStacked) return;

      var heights = vals.map(function(){ return $(this).outerHeight() }).get();
      if (settings.use_tallest) {
        var max = Math.max.apply(null, heights);
        vals.css('height', max);
      } else {
        var min = Math.min.apply(null, heights);
        vals.css('height', min);
      }
      settings.after_height_change();
      equalizer.trigger('after-height-change');
    },

    reflow : function () {
      var self = this;

      this.S('[' + this.attr_name() + ']', this.scope).each(function(){
        self.equalize($(this));
      });
    }
  };
}(jQuery, this, this.document));

;(function ($, window, document, undefined) {
  

  var Modernizr = Modernizr || false;

  Foundation.libs.joyride = {
    name : 'joyride',

    version : '5.2.1',

    defaults : {
      expose                   : false,     // turn on or off the expose feature
      modal                    : true,      // Whether to cover page with modal during the tour
      tip_location             : 'bottom',  // 'top' or 'bottom' in relation to parent
      nub_position             : 'auto',    // override on a per tooltip bases
      scroll_speed             : 1500,      // Page scrolling speed in milliseconds, 0 = no scroll animation
      scroll_animation         : 'linear',  // supports 'swing' and 'linear', extend with jQuery UI.
      timer                    : 0,         // 0 = no timer , all other numbers = timer in milliseconds
      start_timer_on_click     : true,      // true or false - true requires clicking the first button start the timer
      start_offset             : 0,         // the index of the tooltip you want to start on (index of the li)
      next_button              : true,      // true or false to control whether a next button is used
      tip_animation            : 'fade',    // 'pop' or 'fade' in each tip
      pause_after              : [],        // array of indexes where to pause the tour after
      exposed                  : [],        // array of expose elements
      tip_animation_fade_speed : 300,       // when tipAnimation = 'fade' this is speed in milliseconds for the transition
      cookie_monster           : false,     // true or false to control whether cookies are used
      cookie_name              : 'joyride', // Name the cookie you'll use
      cookie_domain            : false,     // Will this cookie be attached to a domain, ie. '.notableapp.com'
      cookie_expires           : 365,       // set when you would like the cookie to expire.
      tip_container            : 'body',    // Where will the tip be attached
      tip_location_patterns    : {
        top: ['bottom'],
        bottom: [], // bottom should not need to be repositioned
        left: ['right', 'top', 'bottom'],
        right: ['left', 'top', 'bottom']
      },
      post_ride_callback     : function (){},    // A method to call once the tour closes (canceled or complete)
      post_step_callback     : function (){},    // A method to call after each step
      pre_step_callback      : function (){},    // A method to call before each step
      pre_ride_callback      : function (){},    // A method to call before the tour starts (passed index, tip, and cloned exposed element)
      post_expose_callback   : function (){},    // A method to call after an element has been exposed
      template : { // HTML segments for tip layout
        link    : '<a href="#close" class="joyride-close-tip">&times;</a>',
        timer   : '<div class="joyride-timer-indicator-wrap"><span class="joyride-timer-indicator"></span></div>',
        tip     : '<div class="joyride-tip-guide"><span class="joyride-nub"></span></div>',
        wrapper : '<div class="joyride-content-wrapper"></div>',
        button  : '<a href="#" class="small button joyride-next-tip"></a>',
        modal   : '<div class="joyride-modal-bg"></div>',
        expose  : '<div class="joyride-expose-wrapper"></div>',
        expose_cover: '<div class="joyride-expose-cover"></div>'
      },
      expose_add_class : '' // One or more space-separated class names to be added to exposed element
    },

    init : function (scope, method, options) {
      Foundation.inherit(this, 'throttle random_str');

      this.settings = this.settings || $.extend({}, this.defaults, (options || method));

      this.bindings(method, options)
    },

    events : function () {
      var self = this;

      $(this.scope)
        .off('.joyride')
        .on('click.fndtn.joyride', '.joyride-next-tip, .joyride-modal-bg', function (e) {
          e.preventDefault();

          if (this.settings.$li.next().length < 1) {
            this.end();
          } else if (this.settings.timer > 0) {
            clearTimeout(this.settings.automate);
            this.hide();
            this.show();
            this.startTimer();
          } else {
            this.hide();
            this.show();
          }

        }.bind(this))

        .on('click.fndtn.joyride', '.joyride-close-tip', function (e) {
          e.preventDefault();
          this.end();
        }.bind(this));

      $(window)
        .off('.joyride')
        .on('resize.fndtn.joyride', self.throttle(function () {
          if ($('[' + self.attr_name() + ']').length > 0 && self.settings.$next_tip) {
            if (self.settings.exposed.length > 0) {
              var $els = $(self.settings.exposed);

              $els.each(function () {
                var $this = $(this);
                self.un_expose($this);
                self.expose($this);
              });
            }

            if (self.is_phone()) {
              self.pos_phone();
            } else {
              self.pos_default(false, true);
            }
          }
        }, 100));
    },

    start : function () {
      var self = this,
          $this = $('[' + this.attr_name() + ']', this.scope),
          integer_settings = ['timer', 'scrollSpeed', 'startOffset', 'tipAnimationFadeSpeed', 'cookieExpires'],
          int_settings_count = integer_settings.length;

      if (!$this.length > 0) return;

      if (!this.settings.init) this.events();

      this.settings = $this.data(this.attr_name(true) + '-init');

      // non configureable settings
      this.settings.$content_el = $this;
      this.settings.$body = $(this.settings.tip_container);
      this.settings.body_offset = $(this.settings.tip_container).position();
      this.settings.$tip_content = this.settings.$content_el.find('> li');
      this.settings.paused = false;
      this.settings.attempts = 0;

      // can we create cookies?
      if (typeof $.cookie !== 'function') {
        this.settings.cookie_monster = false;
      }

      // generate the tips and insert into dom.
      if (!this.settings.cookie_monster || this.settings.cookie_monster && !$.cookie(this.settings.cookie_name)) {
        this.settings.$tip_content.each(function (index) {
          var $this = $(this);
          this.settings = $.extend({}, self.defaults, self.data_options($this))

          // Make sure that settings parsed from data_options are integers where necessary
          var i = int_settings_count;
          while (i--) {
            self.settings[integer_settings[i]] = parseInt(self.settings[integer_settings[i]], 10);
          }
          self.create({$li : $this, index : index});
        });

        // show first tip
        if (!this.settings.start_timer_on_click && this.settings.timer > 0) {
          this.show('init');
          this.startTimer();
        } else {
          this.show('init');
        }

      }
    },

    resume : function () {
      this.set_li();
      this.show();
    },

    tip_template : function (opts) {
      var $blank, content;

      opts.tip_class = opts.tip_class || '';

      $blank = $(this.settings.template.tip).addClass(opts.tip_class);
      content = $.trim($(opts.li).html()) +
        this.button_text(opts.button_text) +
        this.settings.template.link +
        this.timer_instance(opts.index);

      $blank.append($(this.settings.template.wrapper));
      $blank.first().attr(this.add_namespace('data-index'), opts.index);
      $('.joyride-content-wrapper', $blank).append(content);

      return $blank[0];
    },

    timer_instance : function (index) {
      var txt;

      if ((index === 0 && this.settings.start_timer_on_click && this.settings.timer > 0) || this.settings.timer === 0) {
        txt = '';
      } else {
        txt = $(this.settings.template.timer)[0].outerHTML;
      }
      return txt;
    },

    button_text : function (txt) {
      if (this.settings.next_button) {
        txt = $.trim(txt) || 'Next';
        txt = $(this.settings.template.button).append(txt)[0].outerHTML;
      } else {
        txt = '';
      }
      return txt;
    },

    create : function (opts) {
      var buttonText = opts.$li.attr(this.add_namespace('data-button')) 
        || opts.$li.attr(this.add_namespace('data-text')),
        tipClass = opts.$li.attr('class'),
        $tip_content = $(this.tip_template({
          tip_class : tipClass,
          index : opts.index,
          button_text : buttonText,
          li : opts.$li
        }));

      $(this.settings.tip_container).append($tip_content);
    },

    show : function (init) {
      var $timer = null;

      // are we paused?
      if (this.settings.$li === undefined
        || ($.inArray(this.settings.$li.index(), this.settings.pause_after) === -1)) {

        // don't go to the next li if the tour was paused
        if (this.settings.paused) {
          this.settings.paused = false;
        } else {
          this.set_li(init);
        }

        this.settings.attempts = 0;

        if (this.settings.$li.length && this.settings.$target.length > 0) {
          if (init) { //run when we first start
            this.settings.pre_ride_callback(this.settings.$li.index(), this.settings.$next_tip);
            if (this.settings.modal) {
              this.show_modal();
            }
          }

          this.settings.pre_step_callback(this.settings.$li.index(), this.settings.$next_tip);

          if (this.settings.modal && this.settings.expose) {
            this.expose();
          }

          this.settings.tip_settings = $.extend({}, this.settings, this.data_options(this.settings.$li));

          this.settings.timer = parseInt(this.settings.timer, 10);

          this.settings.tip_settings.tip_location_pattern = this.settings.tip_location_patterns[this.settings.tip_settings.tip_location];

          // scroll if not modal
          if (!/body/i.test(this.settings.$target.selector)) {
            this.scroll_to();
          }

          if (this.is_phone()) {
            this.pos_phone(true);
          } else {
            this.pos_default(true);
          }

          $timer = this.settings.$next_tip.find('.joyride-timer-indicator');

          if (/pop/i.test(this.settings.tip_animation)) {

            $timer.width(0);

            if (this.settings.timer > 0) {

              this.settings.$next_tip.show();

              setTimeout(function () {
                $timer.animate({
                  width: $timer.parent().width()
                }, this.settings.timer, 'linear');
              }.bind(this), this.settings.tip_animation_fade_speed);

            } else {
              this.settings.$next_tip.show();

            }


          } else if (/fade/i.test(this.settings.tip_animation)) {

            $timer.width(0);

            if (this.settings.timer > 0) {

              this.settings.$next_tip
                .fadeIn(this.settings.tip_animation_fade_speed)
                .show();

              setTimeout(function () {
                $timer.animate({
                  width: $timer.parent().width()
                }, this.settings.timer, 'linear');
              }.bind(this), this.settings.tip_animation_fadeSpeed);

            } else {
              this.settings.$next_tip.fadeIn(this.settings.tip_animation_fade_speed);
            }
          }

          this.settings.$current_tip = this.settings.$next_tip;

        // skip non-existant targets
        } else if (this.settings.$li && this.settings.$target.length < 1) {

          this.show();

        } else {

          this.end();

        }
      } else {

        this.settings.paused = true;

      }

    },

    is_phone : function () {
      return matchMedia(Foundation.media_queries.small).matches &&
        !matchMedia(Foundation.media_queries.medium).matches;
    },

    hide : function () {
      if (this.settings.modal && this.settings.expose) {
        this.un_expose();
      }

      if (!this.settings.modal) {
        $('.joyride-modal-bg').hide();
      }

      // Prevent scroll bouncing...wait to remove from layout
      this.settings.$current_tip.css('visibility', 'hidden');
      setTimeout($.proxy(function() {
        this.hide();
        this.css('visibility', 'visible');
      }, this.settings.$current_tip), 0);
      this.settings.post_step_callback(this.settings.$li.index(),
        this.settings.$current_tip);
    },

    set_li : function (init) {
      if (init) {
        this.settings.$li = this.settings.$tip_content.eq(this.settings.start_offset);
        this.set_next_tip();
        this.settings.$current_tip = this.settings.$next_tip;
      } else {
        this.settings.$li = this.settings.$li.next();
        this.set_next_tip();
      }

      this.set_target();
    },

    set_next_tip : function () {
      this.settings.$next_tip = $(".joyride-tip-guide").eq(this.settings.$li.index());
      this.settings.$next_tip.data('closed', '');
    },

    set_target : function () {
      var cl = this.settings.$li.attr(this.add_namespace('data-class')),
          id = this.settings.$li.attr(this.add_namespace('data-id')),
          $sel = function () {
            if (id) {
              return $(document.getElementById(id));
            } else if (cl) {
              return $('.' + cl).first();
            } else {
              return $('body');
            }
          };

      this.settings.$target = $sel();
    },

    scroll_to : function () {
      var window_half, tipOffset;

      window_half = $(window).height() / 2;
      tipOffset = Math.ceil(this.settings.$target.offset().top - window_half - 50 + this.settings.$next_tip.outerHeight());

      if (tipOffset != 0) {
        $('html, body').animate({
          scrollTop: tipOffset
        }, this.settings.scroll_speed, 'swing');
      }
    },

    paused : function () {
      return ($.inArray((this.settings.$li.index() + 1), this.settings.pause_after) === -1);
    },

    restart : function () {
      this.hide();
      this.settings.$li = undefined;
      this.show('init');
    },

    pos_default : function (init, resizing) {
      var half_fold = Math.ceil($(window).height() / 2),
          tip_position = this.settings.$next_tip.offset(),
          $nub = this.settings.$next_tip.find('.joyride-nub'),
          nub_width = Math.ceil($nub.outerWidth() / 2),
          nub_height = Math.ceil($nub.outerHeight() / 2),
          toggle = init || false;

      // tip must not be "display: none" to calculate position
      if (toggle) {
        this.settings.$next_tip.css('visibility', 'hidden');
        this.settings.$next_tip.show();
      }

      if (typeof resizing === 'undefined') {
        resizing = false;
      }

      $nub.css('left', '');

      if (!/body/i.test(this.settings.$target.selector)) {
          if (this.bottom()) {
            if (this.rtl) {
              this.settings.$next_tip.css({
                top: (this.settings.$target.offset().top + nub_height + this.settings.$target.outerHeight()),
                left: this.settings.$target.offset().left + this.settings.$target.outerWidth() - this.settings.$next_tip.outerWidth()});
            } else {
              this.settings.$next_tip.css({
                top: (this.settings.$target.offset().top + nub_height + this.settings.$target.outerHeight()),
                left: this.settings.$target.offset().left});
            }

            this.nub_position($nub, this.settings.tip_settings.nub_position, 'top');

          } else if (this.top()) {
            if (this.rtl) {
              this.settings.$next_tip.css({
                top: (this.settings.$target.offset().top - this.settings.$next_tip.outerHeight() - nub_height),
                left: this.settings.$target.offset().left + this.settings.$target.outerWidth() - this.settings.$next_tip.outerWidth()});
            } else {
              this.settings.$next_tip.css({
                top: (this.settings.$target.offset().top - this.settings.$next_tip.outerHeight() - nub_height),
                left: this.settings.$target.offset().left});
            }

            this.nub_position($nub, this.settings.tip_settings.nub_position, 'bottom');

          } else if (this.right()) {

            this.settings.$next_tip.css({
              top: this.settings.$target.offset().top,
              left: (this.settings.$target.outerWidth() + this.settings.$target.offset().left + nub_width)});

            this.nub_position($nub, this.settings.tip_settings.nub_position, 'left');

          } else if (this.left()) {

            this.settings.$next_tip.css({
              top: this.settings.$target.offset().top,
              left: (this.settings.$target.offset().left - this.settings.$next_tip.outerWidth() - nub_width)});

            this.nub_position($nub, this.settings.tip_settings.nub_position, 'right');

          }

          if (!this.visible(this.corners(this.settings.$next_tip)) && this.settings.attempts < this.settings.tip_settings.tip_location_pattern.length) {

            $nub.removeClass('bottom')
              .removeClass('top')
              .removeClass('right')
              .removeClass('left');

            this.settings.tip_settings.tip_location = this.settings.tip_settings.tip_location_pattern[this.settings.attempts];

            this.settings.attempts++;

            this.pos_default();

          }

      } else if (this.settings.$li.length) {

        this.pos_modal($nub);

      }

      if (toggle) {
        this.settings.$next_tip.hide();
        this.settings.$next_tip.css('visibility', 'visible');
      }

    },

    pos_phone : function (init) {
      var tip_height = this.settings.$next_tip.outerHeight(),
          tip_offset = this.settings.$next_tip.offset(),
          target_height = this.settings.$target.outerHeight(),
          $nub = $('.joyride-nub', this.settings.$next_tip),
          nub_height = Math.ceil($nub.outerHeight() / 2),
          toggle = init || false;

      $nub.removeClass('bottom')
        .removeClass('top')
        .removeClass('right')
        .removeClass('left');

      if (toggle) {
        this.settings.$next_tip.css('visibility', 'hidden');
        this.settings.$next_tip.show();
      }

      if (!/body/i.test(this.settings.$target.selector)) {

        if (this.top()) {

            this.settings.$next_tip.offset({top: this.settings.$target.offset().top - tip_height - nub_height});
            $nub.addClass('bottom');

        } else {

          this.settings.$next_tip.offset({top: this.settings.$target.offset().top + target_height + nub_height});
          $nub.addClass('top');

        }

      } else if (this.settings.$li.length) {
        this.pos_modal($nub);
      }

      if (toggle) {
        this.settings.$next_tip.hide();
        this.settings.$next_tip.css('visibility', 'visible');
      }
      $nub.css('left', this.settings.$target.offset().left);
    },

    pos_modal : function ($nub) {
      this.center();
      $nub.hide();

      this.show_modal();
    },

    show_modal : function () {
      if (!this.settings.$next_tip.data('closed')) {
        var joyridemodalbg =  $('.joyride-modal-bg');
        if (joyridemodalbg.length < 1) {
          $('body').append(this.settings.template.modal).show();
        }

        if (/pop/i.test(this.settings.tip_animation)) {
            joyridemodalbg.show();
        } else {
            joyridemodalbg.fadeIn(this.settings.tip_animation_fade_speed);
        }
      }
    },

    expose : function () {
      var expose,
          exposeCover,
          el,
          origCSS,
          origClasses,
          randId = 'expose-' + this.random_str(6);

      if (arguments.length > 0 && arguments[0] instanceof $) {
        el = arguments[0];
      } else if(this.settings.$target && !/body/i.test(this.settings.$target.selector)){
        el = this.settings.$target;
      }  else {
        return false;
      }

      if(el.length < 1){
        if(window.console){
          console.error('element not valid', el);
        }
        return false;
      }

      expose = $(this.settings.template.expose);
      this.settings.$body.append(expose);
      expose.css({
        top: el.offset().top,
        left: el.offset().left,
        width: el.outerWidth(true),
        height: el.outerHeight(true)
      });

      exposeCover = $(this.settings.template.expose_cover);

      origCSS = {
        zIndex: el.css('z-index'),
        position: el.css('position')
      };

      origClasses = el.attr('class') == null ? '' : el.attr('class');

      el.css('z-index',parseInt(expose.css('z-index'))+1);

      if (origCSS.position == 'static') {
        el.css('position','relative');
      }

      el.data('expose-css',origCSS);
      el.data('orig-class', origClasses);
      el.attr('class', origClasses + ' ' + this.settings.expose_add_class);

      exposeCover.css({
        top: el.offset().top,
        left: el.offset().left,
        width: el.outerWidth(true),
        height: el.outerHeight(true)
      });

      if (this.settings.modal) this.show_modal();

      this.settings.$body.append(exposeCover);
      expose.addClass(randId);
      exposeCover.addClass(randId);
      el.data('expose', randId);
      this.settings.post_expose_callback(this.settings.$li.index(), this.settings.$next_tip, el);
      this.add_exposed(el);
    },

    un_expose : function () {
      var exposeId,
          el,
          expose ,
          origCSS,
          origClasses,
          clearAll = false;

      if (arguments.length > 0 && arguments[0] instanceof $) {
        el = arguments[0];
      } else if(this.settings.$target && !/body/i.test(this.settings.$target.selector)){
        el = this.settings.$target;
      }  else {
        return false;
      }

      if(el.length < 1){
        if (window.console) {
          console.error('element not valid', el);
        }
        return false;
      }

      exposeId = el.data('expose');
      expose = $('.' + exposeId);

      if (arguments.length > 1) {
        clearAll = arguments[1];
      }

      if (clearAll === true) {
        $('.joyride-expose-wrapper,.joyride-expose-cover').remove();
      } else {
        expose.remove();
      }

      origCSS = el.data('expose-css');

      if (origCSS.zIndex == 'auto') {
        el.css('z-index', '');
      } else {
        el.css('z-index', origCSS.zIndex);
      }

      if (origCSS.position != el.css('position')) {
        if(origCSS.position == 'static') {// this is default, no need to set it.
          el.css('position', '');
        } else {
          el.css('position', origCSS.position);
        }
      }

      origClasses = el.data('orig-class');
      el.attr('class', origClasses);
      el.removeData('orig-classes');

      el.removeData('expose');
      el.removeData('expose-z-index');
      this.remove_exposed(el);
    },

    add_exposed: function(el){
      this.settings.exposed = this.settings.exposed || [];
      if (el instanceof $ || typeof el === 'object') {
        this.settings.exposed.push(el[0]);
      } else if (typeof el == 'string') {
        this.settings.exposed.push(el);
      }
    },

    remove_exposed: function(el){
      var search, i;
      if (el instanceof $) {
        search = el[0]
      } else if (typeof el == 'string'){
        search = el;
      }

      this.settings.exposed = this.settings.exposed || [];
      i = this.settings.exposed.length;

      while (i--) {
        if (this.settings.exposed[i] == search) {
          this.settings.exposed.splice(i, 1);
          return;
        }
      }
    },

    center : function () {
      var $w = $(window);

      this.settings.$next_tip.css({
        top : ((($w.height() - this.settings.$next_tip.outerHeight()) / 2) + $w.scrollTop()),
        left : ((($w.width() - this.settings.$next_tip.outerWidth()) / 2) + $w.scrollLeft())
      });

      return true;
    },

    bottom : function () {
      return /bottom/i.test(this.settings.tip_settings.tip_location);
    },

    top : function () {
      return /top/i.test(this.settings.tip_settings.tip_location);
    },

    right : function () {
      return /right/i.test(this.settings.tip_settings.tip_location);
    },

    left : function () {
      return /left/i.test(this.settings.tip_settings.tip_location);
    },

    corners : function (el) {
      var w = $(window),
          window_half = w.height() / 2,
          //using this to calculate since scroll may not have finished yet.
          tipOffset = Math.ceil(this.settings.$target.offset().top - window_half + this.settings.$next_tip.outerHeight()),
          right = w.width() + w.scrollLeft(),
          offsetBottom =  w.height() + tipOffset,
          bottom = w.height() + w.scrollTop(),
          top = w.scrollTop();

      if (tipOffset < top) {
        if (tipOffset < 0) {
          top = 0;
        } else {
          top = tipOffset;
        }
      }

      if (offsetBottom > bottom) {
        bottom = offsetBottom;
      }

      return [
        el.offset().top < top,
        right < el.offset().left + el.outerWidth(),
        bottom < el.offset().top + el.outerHeight(),
        w.scrollLeft() > el.offset().left
      ];
    },

    visible : function (hidden_corners) {
      var i = hidden_corners.length;

      while (i--) {
        if (hidden_corners[i]) return false;
      }

      return true;
    },

    nub_position : function (nub, pos, def) {
      if (pos === 'auto') {
        nub.addClass(def);
      } else {
        nub.addClass(pos);
      }
    },

    startTimer : function () {
      if (this.settings.$li.length) {
        this.settings.automate = setTimeout(function () {
          this.hide();
          this.show();
          this.startTimer();
        }.bind(this), this.settings.timer);
      } else {
        clearTimeout(this.settings.automate);
      }
    },

    end : function () {
      if (this.settings.cookie_monster) {
        $.cookie(this.settings.cookie_name, 'ridden', { expires: this.settings.cookie_expires, domain: this.settings.cookie_domain });
      }

      if (this.settings.timer > 0) {
        clearTimeout(this.settings.automate);
      }

      if (this.settings.modal && this.settings.expose) {
        this.un_expose();
      }

      this.settings.$next_tip.data('closed', true);

      $('.joyride-modal-bg').hide();
      this.settings.$current_tip.hide();
      this.settings.post_step_callback(this.settings.$li.index(), this.settings.$current_tip);
      this.settings.post_ride_callback(this.settings.$li.index(), this.settings.$current_tip);
      $('.joyride-tip-guide').remove();
    },

    off : function () {
      $(this.scope).off('.joyride');
      $(window).off('.joyride');
      $('.joyride-close-tip, .joyride-next-tip, .joyride-modal-bg').off('.joyride');
      $('.joyride-tip-guide, .joyride-modal-bg').remove();
      clearTimeout(this.settings.automate);
      this.settings = {};
    },

    reflow : function () {}
  };
}(jQuery, this, this.document));
;(function ($, window, document, undefined) {
  

  Foundation.libs['magellan-expedition'] = {
    name : 'magellan-expedition',

    version : '5.3.0',

    settings : {
      active_class: 'active',
      threshold: 0, // pixels from the top of the expedition for it to become fixes
      destination_threshold: 20, // pixels from the top of destination for it to be considered active
      throttle_delay: 30, // calculation throttling to increase framerate
      fixed_top: 0 // top distance in pixels assigend to the fixed element on scroll
    },

    init : function (scope, method, options) {
      Foundation.inherit(this, 'throttle');
      this.bindings(method, options);
    },

    events : function () {
      var self = this,
          S = self.S,
          settings = self.settings;

      // initialize expedition offset
      self.set_expedition_position();

      S(self.scope)
        .off('.magellan')
        .on('click.fndtn.magellan', '[' + self.add_namespace('data-magellan-arrival') + '] a[href^="#"]', function (e) {
          e.preventDefault();
          var expedition = $(this).closest('[' + self.attr_name() + ']'),
              settings = expedition.data('magellan-expedition-init'),
              hash = this.hash.split('#').join(''),
              target = $("a[name='"+hash+"']");

          if (target.length === 0) {
            target = $('#'+hash);

          }


          // Account for expedition height if fixed position
          var scroll_top = target.offset().top - settings.destination_threshold + 1;
          scroll_top = scroll_top - expedition.outerHeight();

          $('html, body').stop().animate({
            'scrollTop': scroll_top
          }, 700, 'swing', function () {
            if(history.pushState) {
              history.pushState(null, null, '#'+hash);
            }
            else {
              location.hash = '#'+hash;
            }
          });
        })
        .on('scroll.fndtn.magellan', self.throttle(this.check_for_arrivals.bind(this), settings.throttle_delay));

      $(window)
        .on('resize.fndtn.magellan', self.throttle(this.set_expedition_position.bind(this), settings.throttle_delay));
    },

    check_for_arrivals : function() {
      var self = this;
      self.update_arrivals();
      self.update_expedition_positions();
    },

    set_expedition_position : function() {
      var self = this;
      $('[' + this.attr_name() + '=fixed]', self.scope).each(function(idx, el) {
        var expedition = $(this),
            settings = expedition.data('magellan-expedition-init'),
            styles = expedition.attr('styles'), // save styles
            top_offset;

        expedition.attr('style', '');
        top_offset = expedition.offset().top + settings.threshold;

        expedition.data(self.data_attr('magellan-top-offset'), top_offset);
        expedition.attr('style', styles);
      });
    },

    update_expedition_positions : function() {
      var self = this,
          window_top_offset = $(window).scrollTop();

      $('[' + this.attr_name() + '=fixed]', self.scope).each(function() {
        var expedition = $(this),
            settings = expedition.data('magellan-expedition-init'),
            top_offset = expedition.data('magellan-top-offset');

        if (window_top_offset >= top_offset) {
          // Placeholder allows height calculations to be consistent even when
          // appearing to switch between fixed/non-fixed placement
          var placeholder = expedition.prev('[' + self.add_namespace('data-magellan-expedition-clone') + ']');
          if (placeholder.length === 0) {
            placeholder = expedition.clone();
            placeholder.removeAttr(self.attr_name());
            placeholder.attr(self.add_namespace('data-magellan-expedition-clone'),'');
            expedition.before(placeholder);
          }
          expedition.css({position:'fixed', top: settings.fixed_top});
        } else {
          expedition.prev('[' + self.add_namespace('data-magellan-expedition-clone') + ']').remove();
          expedition.attr('style','').removeClass('fixed');
        }
      });
    },

    update_arrivals : function() {
      var self = this,
          window_top_offset = $(window).scrollTop();

      $('[' + this.attr_name() + ']', self.scope).each(function() {
        var expedition = $(this),
            settings = expedition.data(self.attr_name(true) + '-init'),
            offsets = self.offsets(expedition, window_top_offset),
            arrivals = expedition.find('[' + self.add_namespace('data-magellan-arrival') + ']'),
            active_item = false;
        offsets.each(function(idx, item) {
          if (item.viewport_offset >= item.top_offset) {
            var arrivals = expedition.find('[' + self.add_namespace('data-magellan-arrival') + ']');
            arrivals.not(item.arrival).removeClass(settings.active_class);
            item.arrival.addClass(settings.active_class);
            active_item = true;
            return true;
          }
        });

        if (!active_item) arrivals.removeClass(settings.active_class);
      });
    },

    offsets : function(expedition, window_offset) {
      var self = this,
          settings = expedition.data(self.attr_name(true) + '-init'),
          viewport_offset = window_offset;

      return expedition.find('[' + self.add_namespace('data-magellan-arrival') + ']').map(function(idx, el) {
        var name = $(this).data(self.data_attr('magellan-arrival')),
            dest = $('[' + self.add_namespace('data-magellan-destination') + '=' + name + ']');
        if (dest.length > 0) {
          var top_offset = Math.floor(dest.offset().top - settings.destination_threshold - expedition.outerHeight());
          return {
            destination : dest,
            arrival : $(this),
            top_offset : top_offset,
            viewport_offset : viewport_offset
          }
        }
      }).sort(function(a, b) {
        if (a.top_offset < b.top_offset) return -1;
        if (a.top_offset > b.top_offset) return 1;
        return 0;
      });
    },

    data_attr: function (str) {
      if (this.namespace.length > 0) {
        return this.namespace + '-' + str;
      }

      return str;
    },

    off : function () {
      this.S(this.scope).off('.magellan');
      this.S(window).off('.magellan');
    },

    reflow : function () {
      var self = this;
      // remove placeholder expeditions used for height calculation purposes
      $('[' + self.add_namespace('data-magellan-expedition-clone') + ']', self.scope).remove();
    }
  };
}(jQuery, window, window.document));

;(function ($, window, document, undefined) {
  

  Foundation.libs.offcanvas = {
    name : 'offcanvas',

    version : '5.2.1',

    settings : {},

    init : function (scope, method, options) {
      this.events();
    },

    events : function () {
      var S = this.S;

      S(this.scope).off('.offcanvas')
        .on('click.fndtn.offcanvas', '.left-off-canvas-toggle', function (e) {
          e.preventDefault();
          S(this).closest('.off-canvas-wrap').toggleClass('move-right');
        })
        .on('click.fndtn.offcanvas', '.exit-off-canvas', function (e) {
          e.preventDefault();
          S(".off-canvas-wrap").removeClass("move-right");
        })
        .on('click.fndtn.offcanvas', '.left-off-canvas-menu a', function (e) {
          if ($(this).attr('target') !== '_blank') {
            e.preventDefault();
            var href = $(this).attr('href');
            var isSPA = $(e.currentTarget).hasClass('js-SPA');
            S('.off-canvas-wrap').on('transitionend webkitTransitionEnd oTransitionEnd', function(e) {
                if (!$('.js-SPA-enabled').length || !isSPA) {
                    window.location = href
                }
                S('.off-canvas-wrap').off('transitionend webkitTransitionEnd oTransitionEnd');
            });
            S(".off-canvas-wrap").removeClass("move-right");
          }
        })
        .on('click.fndtn.offcanvas', '.right-off-canvas-toggle', function (e) {
          e.preventDefault();
          S(this).closest(".off-canvas-wrap").toggleClass("move-left");
        })
        .on('click.fndtn.offcanvas', '.exit-off-canvas', function (e) {
          e.preventDefault();
          S(".off-canvas-wrap").removeClass("move-left");
        })
        .on('click.fndtn.offcanvas', '.right-off-canvas-menu a', function (e) {
          e.preventDefault();
          var href = $(this).attr('href');
          S('.off-canvas-wrap').on('transitionend webkitTransitionEnd oTransitionEnd', function(e) {
              window.location = href
              S('.off-canvas-wrap').off('transitionend webkitTransitionEnd oTransitionEnd');
          });
          S(".off-canvas-wrap").removeClass("move-left");
        });
    },

    reflow : function () {}
  };
}(jQuery, this, this.document));

;(function ($, window, document, undefined) {
  

  var noop = function() {};

  var Orbit = function(el, settings) {
    // Don't reinitialize plugin
    if (el.hasClass(settings.slides_container_class)) {
      return this;
    }

    var self = this,
        container,
        slides_container = el,
        number_container,
        bullets_container,
        timer_container,
        idx = 0,
        animate,
        adjust_height_after = false;

    self.cache = {};

    self.slides = function() {
      return slides_container.children(settings.slide_selector);
    };

    self.slides().first().addClass(settings.active_slide_class);

    self.update_slide_number = function(index) {
      if (settings.slide_number) {
        number_container.find('span:first').text(parseInt(index)+1);
        number_container.find('span:last').text(self.slides().length);
      }
      if (settings.bullets) {
        bullets_container.children().removeClass(settings.bullets_active_class);
        $(bullets_container.children().get(index)).addClass(settings.bullets_active_class);
      }
    };

    self.update_active_link = function(index) {
      var link = $('[data-orbit-link="'+self.slides().eq(index).attr('data-orbit-slide')+'"]');
      link.siblings().removeClass(settings.bullets_active_class);
      link.addClass(settings.bullets_active_class);
    };

    self.build_markup = function() {
      slides_container.wrap('<div class="'+settings.container_class+'"></div>');
      container = slides_container.parent();
      slides_container.addClass(settings.slides_container_class);
      
      if (settings.navigation_arrows) {
        container.append($('<a href="#"><span></span></a>').addClass(settings.prev_class));
        container.append($('<a href="#"><span></span></a>').addClass(settings.next_class));
      }

      if (settings.timer) {
        timer_container = $('<div>').addClass(settings.timer_container_class);
        timer_container.append('<span>');
        timer_container.append($('<div>').addClass(settings.timer_progress_class));
        timer_container.addClass(settings.timer_paused_class);
        container.append(timer_container);
      }

      if (settings.slide_number) {
        number_container = $('<div>').addClass(settings.slide_number_class);
        number_container.append('<span></span> ' + settings.slide_number_text + ' <span></span>');
        container.append(number_container);
      }

      if (settings.bullets) {
        bullets_container = $('<ol>').addClass(settings.bullets_container_class);
        container.append(bullets_container);
        bullets_container.wrap('<div class="orbit-bullets-container"></div>');
        self.slides().each(function(idx, el) {
          var bullet = $('<li>').attr('data-orbit-slide', idx);
          bullets_container.append(bullet);
        });
      }

      if (settings.stack_on_small) {
        container.addClass(settings.stack_on_small_class);
      }
    };

    self._prepare_direction = function(next_idx, current_direction) {
      var dir = 'next';
      if (next_idx <= idx) { dir = 'prev'; }
      
      if (settings.animation === 'slide') {    
        setTimeout(function(){
          slides_container.removeClass("swipe-prev swipe-next");
          if (dir === 'next') {slides_container.addClass("swipe-next");}
          else if (dir === 'prev') {slides_container.addClass("swipe-prev");}
        },0);
      }
      
      var slides = self.slides();
      if (next_idx >= slides.length) {
        if (!settings.circular) return false;
        next_idx = 0;
      } else if (next_idx < 0) {
        if (!settings.circular) return false;
        next_idx = slides.length - 1;
      }
      var current = $(slides.get(idx))
        , next = $(slides.get(next_idx));
      

      return [dir, current, next, next_idx];
    };

    self._goto = function(next_idx, start_timer) {
      if (next_idx === null) {return false;}
      if (self.cache.animating) {return false;}
      if (next_idx === idx) {return false;}
      if (typeof self.cache.timer === 'object') {self.cache.timer.restart();}
      
      var slides = self.slides();
      self.cache.animating = true;
      var res = self._prepare_direction(next_idx)
        , dir = res[0]
        , current = res[1]
        , next = res[2]
        , next_idx = res[3];

      slides_container.trigger('before-slide-change.fndtn.orbit');
      settings.before_slide_change();
      idx = next_idx;

      current.css("transitionDuration", settings.animation_speed+"ms");
      next.css("transitionDuration", settings.animation_speed+"ms");
      
      var callback = function() {
        var unlock = function() {
          if (start_timer === true) {self.cache.timer.restart();}
          self.update_slide_number(idx);
          next.addClass(settings.active_slide_class);
          self.update_active_link(next_idx);
          slides_container.trigger('after-slide-change.fndtn.orbit',[{slide_number: idx, total_slides: slides.length}]);
          settings.after_slide_change(idx, slides.length);
          setTimeout(function(){
            self.cache.animating = false;
          }, 100);
          
        };
        if (slides_container.height() != next.height() && settings.variable_height) {
          slides_container.animate({'height': next.height()}, 250, 'linear', unlock);
        } else {
          unlock();
        }
      };

      if (slides.length === 1) {callback(); return false;}

      var start_animation = function() {
        if (dir === 'next') {animate.next(current, next, callback);}
        if (dir === 'prev') {animate.prev(current, next, callback);}        
      };

      if (next.height() > slides_container.height() && settings.variable_height) {
        slides_container.animate({'height': next.height()}, 250, 'linear', start_animation);
      } else {
        start_animation();
      }
    };
    
    self.next = function(e) {
      e.stopImmediatePropagation();
      e.preventDefault();
      self._prepare_direction(idx + 1);
      setTimeout(function(){
        self._goto(idx + 1);
    }, 100);
    };
    
    self.prev = function(e) {
      e.stopImmediatePropagation();
      e.preventDefault();
      self._prepare_direction(idx - 1);
      setTimeout(function(){
        self._goto(idx - 1)
      }, 100);
    };

    self.link_custom = function(e) {
      e.preventDefault();
      var link = $(this).attr('data-orbit-link');
      if ((typeof link === 'string') && (link = $.trim(link)) != "") {
        var slide = container.find('[data-orbit-slide='+link+']');
        if (slide.index() != -1) {
          setTimeout(function(){
            self._goto(slide.index());
          },100);
        }
      }
    };

    self.link_bullet = function(e) {    
      var index = $(this).attr('data-orbit-slide');
      if ((typeof index === 'string') && (index = $.trim(index)) != "") {
        if(isNaN(parseInt(index)))
        {
          var slide = container.find('[data-orbit-slide='+index+']');
          if (slide.index() != -1) {
            setTimeout(function(){
              self._goto(slide.index() + 1);
            },100);
          }
        }
        else
        {
          setTimeout(function(){
            self._goto(parseInt(index));
          },100);
        }
      }

    }

    self.timer_callback = function() {
      self._goto(idx + 1, true);
    }
    
    self.compute_dimensions = function() {
      var current = $(self.slides().get(idx));
      var h = current.height();
      if (!settings.variable_height) {
        self.slides().each(function(){
          if ($(this).height() > h) { h = $(this).height(); }
        });
      }
      slides_container.height(h);
    };

    self.create_timer = function() {
      var t = new Timer(
        container.find('.'+settings.timer_container_class), 
        settings, 
        self.timer_callback
      );
      return t;
    };

    self.stop_timer = function() {
      if (typeof self.cache.timer === 'object') self.cache.timer.stop();
    };

    self.toggle_timer = function() {
      var t = container.find('.'+settings.timer_container_class);
      if (t.hasClass(settings.timer_paused_class)) {
        if (typeof self.cache.timer === 'undefined') {self.cache.timer = self.create_timer();}
        self.cache.timer.start();     
      }
      else {
        if (typeof self.cache.timer === 'object') {self.cache.timer.stop();}
      }
    };

    self.init = function() {
      self.build_markup();
      if (settings.timer) {
        self.cache.timer = self.create_timer(); 
        Foundation.utils.image_loaded(this.slides().children('img'), self.cache.timer.start);
      }
      // animate = new FadeAnimation(settings, slides_container);
      // if (settings.animation === 'slide') 
      //   animate = new SlideAnimation(settings, slides_container);
      if(settings.animation === 'fade') {slides_container.addClass('fade');}
      animate = new CSSAnimation(settings, slides_container);
      container.on('click', '.'+settings.next_class, self.next);
      container.on('click', '.'+settings.prev_class, self.prev);

      container.on('click', '[data-orbit-slide]', self.link_bullet);
      container.on('click', self.toggle_timer);
      if (settings.swipe) {
        slides_container.on('touchstart.fndtn.orbit',function(e) {
          if (self.cache.animating) {return;}
          if (!e.touches) {e = e.originalEvent;}

          self.cache.start_page_x = e.touches[0].pageX;
          self.cache.start_page_y = e.touches[0].pageY;
          self.cache.start_time = (new Date()).getTime();
          self.cache.delta_x = 0;
          self.cache.is_scrolling = null;
          self.cache.direction = null;
          
          self.stop_timer(); // does not appear to prevent callback from occurring          
        })
        .on('touchmove.fndtn.orbit',function(e) {
          if (Math.abs(self.cache.delta_x) > 5) {
            e.preventDefault();
            e.stopPropagation();
          }

          if (self.cache.animating) {return;}          
          requestAnimationFrame(function(){
            if (!e.touches) { e = e.originalEvent; }

            // Ignore pinch/zoom events
            if(e.touches.length > 1 || e.scale && e.scale !== 1) return;

            self.cache.delta_x = e.touches[0].pageX - self.cache.start_page_x;

            if (self.cache.is_scrolling === null) {
              self.cache.is_scrolling = !!( self.cache.is_scrolling || Math.abs(self.cache.delta_x) < Math.abs(e.touches[0].pageY - self.cache.start_page_y) );
            }

            if (self.cache.is_scrolling) {
              return;
            }
            
            var direction = (self.cache.delta_x < 0) ? (idx+1) : (idx-1);
            if (self.cache.direction !== direction) {
              var res = self._prepare_direction(direction);
              self.cache.direction = direction;
              self.cache.dir = res[0];
              self.cache.current = res[1];
              self.cache.next = res[2];
            }

            if (settings.animation === 'slide') {
              var offset, next_offset;
              
              offset = (self.cache.delta_x / container.width()) * 100;
              if (offset >= 0) {next_offset = -(100 - offset);}
              else {next_offset = 100 + offset;}

              self.cache.current.css("transform","translate3d("+offset+"%,0,0)");
              self.cache.next.css("transform","translate3d("+next_offset+"%,0,0)");
            }
          });
        })
        .on('touchend.fndtn.orbit', function(e) {
          if (self.cache.animating) {return;}
          e.preventDefault();
          e.stopPropagation();
          setTimeout(function(){
            self._goto(self.cache.direction);
          }, 50);
        });
      }
      container.on('mouseenter.fndtn.orbit', function(e) {
        if (settings.timer && settings.pause_on_hover) {
          self.stop_timer();
        }
      })
      .on('mouseleave.fndtn.orbit', function(e) {
        if (settings.timer && settings.resume_on_mouseout) {
          self.cache.timer.start();
        }
      });
      
      $(document).on('click', '[data-orbit-link]', self.link_custom);
      $(window).on('resize', self.compute_dimensions);
      Foundation.utils.image_loaded(this.slides().children('img'), self.compute_dimensions);
      Foundation.utils.image_loaded(this.slides().children('img'), function() {
        container.prev('.preloader').css('display', 'none');
        self.update_slide_number(0);
        self.update_active_link(0);
        slides_container.trigger('ready.fndtn.orbit');
      });
    };

    self.init();
  };

  var Timer = function(el, settings, callback) {
    var self = this,
        duration = settings.timer_speed,
        progress = el.find('.'+settings.timer_progress_class),
        start, 
        timeout,
        left = -1;

    this.update_progress = function(w) {
      var new_progress = progress.clone();
      new_progress.attr('style', '');
      new_progress.css('width', w+'%');
      progress.replaceWith(new_progress);
      progress = new_progress;
    };

    this.restart = function() {
      clearTimeout(timeout);
      el.addClass(settings.timer_paused_class);
      left = -1;
      self.update_progress(0);
      self.start();
    };

    this.start = function() {
      if (!el.hasClass(settings.timer_paused_class)) {return true;}
      left = (left === -1) ? duration : left;
      el.removeClass(settings.timer_paused_class);
      start = new Date().getTime();
      progress.animate({'width': '100%'}, left, 'linear');
      timeout = setTimeout(function() {
        self.restart();
        callback();
      }, left);
      el.trigger('timer-started.fndtn.orbit')
    };

    this.stop = function() {
      if (el.hasClass(settings.timer_paused_class)) {return true;}
      clearTimeout(timeout);
      el.addClass(settings.timer_paused_class);
      var end = new Date().getTime();
      left = left - (end - start);
      var w = 100 - ((left / duration) * 100);
      self.update_progress(w);
      el.trigger('timer-stopped.fndtn.orbit');
    };
  };

  var CSSAnimation = function(settings, container) {
    var animation_end = "webkitTransitionEnd otransitionend oTransitionEnd msTransitionEnd transitionend";

    this.next = function(current, next, callback) {
      next.on(animation_end, function(e){
        next.unbind(animation_end);
        current.removeClass("active animate-out");
        next.removeClass("animate-in");
        callback();
      });
      container.children().css({
        "transform":"", 
        "transitionDuration":""
      });
      current.addClass("animate-out");
      next.addClass("animate-in");
    };

    this.prev = function(current, prev, callback) {
      prev.on(animation_end, function(e){
        prev.unbind(animation_end);
        current.removeClass("active animate-out");
        prev.removeClass("animate-in");
        callback();
      });
      current.css({"transform":"", "transitionDuration":""}).addClass("animate-out");
      prev.css({"transform":"", "transitionDuration":""}).addClass("animate-in");
    };
  };


  Foundation.libs = Foundation.libs || {};

  Foundation.libs.orbit = {
    name: 'orbit',

    version: '5.2.1',

    settings: {
      animation: 'slide',
      timer_speed: 10000,
      pause_on_hover: true,
      resume_on_mouseout: false,
      animation_speed: 500,
      stack_on_small: false,
      navigation_arrows: true,
      slide_number: true,
      slide_number_text: 'of',
      container_class: 'orbit-container',
      stack_on_small_class: 'orbit-stack-on-small',
      next_class: 'orbit-next',
      prev_class: 'orbit-prev',
      timer_container_class: 'orbit-timer',
      timer_paused_class: 'paused',
      timer_progress_class: 'orbit-progress',
      slides_container_class: 'orbit-slides-container',
      slide_selector: '*',
      bullets_container_class: 'orbit-bullets',
      bullets_active_class: 'active',
      slide_number_class: 'orbit-slide-number',
      caption_class: 'orbit-caption',
      active_slide_class: 'active',
      orbit_transition_class: 'orbit-transitioning',
      bullets: true,
      circular: true,
      timer: true,
      variable_height: false,
      swipe: true,
      before_slide_change: noop,
      after_slide_change: noop
    },

    init : function (scope, method, options) {
      var self = this;
      this.bindings(method, options);
    },

    events : function (instance) {
      var orbit_instance = new Orbit(this.S(instance), this.S(instance).data('orbit-init'));
      this.S(instance).data(self.name + '-instance', orbit_instance);
    },

    reflow : function () {
      var self = this;

      if (self.S(self.scope).is('[data-orbit]')) {
        var $el = self.S(self.scope);
        var instance = $el.data(self.name + '-instance');
        instance.compute_dimensions();
      } else {
        self.S('[data-orbit]', self.scope).each(function(idx, el) {
          var $el = self.S(el);
          var opts = self.data_options($el);
          var instance = $el.data(self.name + '-instance');
          instance.compute_dimensions();
        });
      }
    }
  };

    
}(jQuery, this, this.document));

;(function ($, window, document, undefined) {
  

  Foundation.libs.reveal = {
    name : 'reveal',

    version : '5.2.1',

    locked : false,

    settings : {
      animation: 'fadeAndPop',
      animation_speed: 250,
      close_on_background_click: true,
      close_on_esc: true,
      dismiss_modal_class: 'close-reveal-modal',
      bg_class: 'reveal-modal-bg',
      open: function(){},
      opened: function(){},
      close: function(){},
      closed: function(){},
      bg : $('.reveal-modal-bg'),
      css : {
        open : {
          'opacity': 0,
          'visibility': 'visible',
          'display' : 'block'
        },
        close : {
          'opacity': 1,
          'visibility': 'hidden',
          'display': 'none'
        }
      }
    },

    init : function (scope, method, options) {
      $.extend(true, this.settings, method, options);
      this.bindings(method, options);
    },

    events : function (scope) {
      var self = this,
          S = self.S;

      S(this.scope)
        .off('.reveal')
        .on('click.fndtn.reveal', '[' + this.add_namespace('data-reveal-id') + ']', function (e) {
          e.preventDefault();

          if (!self.locked) {
            var element = S(this),
                ajax = element.data(self.data_attr('reveal-ajax'));

            self.locked = true;

            if (typeof ajax === 'undefined') {
              self.open.call(self, element);
            } else {
              var url = ajax === true ? element.attr('href') : ajax;

              self.open.call(self, element, {url: url});
            }
          }
        });

      S(document)
        .on('touchend.fndtn.reveal click.fndtn.reveal', this.close_targets(), function (e) {

          e.preventDefault();

          if (!self.locked) {
            var settings = S('[' + self.attr_name() + '].open').data(self.attr_name(true) + '-init'),
                bg_clicked = S(e.target)[0] === S('.' + settings.bg_class)[0];

            if (bg_clicked) {
              if (settings.close_on_background_click) {
                e.stopPropagation();
              } else {
                return;
              }
            }

            self.locked = true;
            self.close.call(self, bg_clicked ? S('[' + self.attr_name() + '].open') : S(this).closest('[' + self.attr_name() + ']'));
          }
        });

      if(S('[' + self.attr_name() + ']', this.scope).length > 0) {
        S(this.scope)
          // .off('.reveal')
          .on('open.fndtn.reveal', this.settings.open)
          .on('opened.fndtn.reveal', this.settings.opened)
          .on('opened.fndtn.reveal', this.open_video)
          .on('close.fndtn.reveal', this.settings.close)
          .on('closed.fndtn.reveal', this.settings.closed)
          .on('closed.fndtn.reveal', this.close_video);
      } else {
        S(this.scope)
          // .off('.reveal')
          .on('open.fndtn.reveal', '[' + self.attr_name() + ']', this.settings.open)
          .on('opened.fndtn.reveal', '[' + self.attr_name() + ']', this.settings.opened)
          .on('opened.fndtn.reveal', '[' + self.attr_name() + ']', this.open_video)
          .on('close.fndtn.reveal', '[' + self.attr_name() + ']', this.settings.close)
          .on('closed.fndtn.reveal', '[' + self.attr_name() + ']', this.settings.closed)
          .on('closed.fndtn.reveal', '[' + self.attr_name() + ']', this.close_video);
      }

      return true;
    },

    // PATCH #3: turning on key up capture only when a reveal window is open
    key_up_on : function (scope) {
      var self = this;

      // PATCH #1: fixing multiple keyup event trigger from single key press
      self.S('body').off('keyup.fndtn.reveal').on('keyup.fndtn.reveal', function ( event ) {
        var open_modal = self.S('[' + self.attr_name() + '].open'),
            settings = open_modal.data(self.attr_name(true) + '-init');
        // PATCH #2: making sure that the close event can be called only while unlocked,
        //           so that multiple keyup.fndtn.reveal events don't prevent clean closing of the reveal window.
        if ( settings && event.which === 27  && settings.close_on_esc && !self.locked) { // 27 is the keycode for the Escape key
          self.close.call(self, open_modal);
        }
      });

      return true;
    },

    // PATCH #3: turning on key up capture only when a reveal window is open
    key_up_off : function (scope) {
      this.S('body').off('keyup.fndtn.reveal');
      return true;
    },

    open : function (target, ajax_settings) {
      var self = this;
      if (target) {
        if (typeof target.selector !== 'undefined') {
          var modal = self.S('#' + target.data(self.data_attr('reveal-id')));
        } else {
          var modal = self.S(this.scope);

          ajax_settings = target;
        }
      } else {
        var modal = self.S(this.scope);
      }

      var settings = modal.data(self.attr_name(true) + '-init');

      if (!modal.hasClass('open')) {
        var open_modal = self.S('[' + self.attr_name() + '].open');

        if (typeof modal.data('css-top') === 'undefined') {
          modal.data('css-top', parseInt(modal.css('top'), 10))
            .data('offset', this.cache_offset(modal));
        }

        this.key_up_on(modal);    // PATCH #3: turning on key up capture only when a reveal window is open
        modal.trigger('open');

        if (open_modal.length < 1) {
          this.toggle_bg(modal);
        }

        if (typeof ajax_settings === 'string') {
          ajax_settings = {
            url: ajax_settings
          };
        }

        if (typeof ajax_settings === 'undefined' || !ajax_settings.url) {
          if (open_modal.length > 0) {
            this.hide(open_modal, settings.css.close);
          }

          this.show(modal, settings.css.open);
        } else {
          var old_success = typeof ajax_settings.success !== 'undefined' ? ajax_settings.success : null;

          $.extend(ajax_settings, {
            success: function (data, textStatus, jqXHR) {
              if ( $.isFunction(old_success) ) {
                old_success(data, textStatus, jqXHR);
              }

              modal.html(data);
              self.S(modal).foundation('section', 'reflow');

              if (open_modal.length > 0) {
                self.hide(open_modal, settings.css.close);
              }
              self.show(modal, settings.css.open);
            }
          });

          $.ajax(ajax_settings);
        }
      }
    },

    close : function (modal) {
      var modal = modal && modal.length ? modal : this.S(this.scope),
          open_modals = this.S('[' + this.attr_name() + '].open'),
          settings = modal.data(this.attr_name(true) + '-init');

      if (open_modals.length > 0) {
        this.locked = true;
        this.key_up_off(modal);   // PATCH #3: turning on key up capture only when a reveal window is open
        modal.trigger('close');
        this.toggle_bg(modal);
        this.hide(open_modals, settings.css.close, settings);
      }
    },

    close_targets : function () {
      var base = '.' + this.settings.dismiss_modal_class;

      if (this.settings.close_on_background_click) {
        return base + ', .' + this.settings.bg_class;
      }

      return base;
    },

    toggle_bg : function (modal) {
      var settings = modal.data(this.attr_name(true));

      if (this.S('.' + this.settings.bg_class).length === 0) {
        this.settings.bg = $('<div />', {'class': this.settings.bg_class})
          .appendTo('body').hide();
      }

      if (this.settings.bg.filter(':visible').length > 0) {
        this.hide(this.settings.bg);
      } else {
        this.show(this.settings.bg);
      }
    },

    show : function (el, css) {
      // is modal
      if (css) {
        var settings = el.data(this.attr_name(true) + '-init');
        if (el.parent('body').length === 0) {
          var placeholder = el.wrap('<div style="display: none;" />').parent(),
              rootElement = this.settings.rootElement || 'body';

          el.on('closed.fndtn.reveal.wrapped', function() {
            el.detach().appendTo(placeholder);
            el.unwrap().unbind('closed.fndtn.reveal.wrapped');
          });

          el.detach().appendTo(rootElement);
        }

        var animData = getAnimationData(settings.animation);
        if (!animData.animate) {
          this.locked = false;
        }
        if (animData.pop) {
          css.top = $(window).scrollTop() - el.data('offset') + 'px';
          var end_css = {
            top: $(window).scrollTop() + el.data('css-top') + 'px',
            opacity: 1
          };

          return setTimeout(function () {
            return el
              .css(css)
              .animate(end_css, settings.animation_speed, 'linear', function () {
                this.locked = false;
                el.trigger('opened');
              }.bind(this))
              .addClass('open');
          }.bind(this), settings.animation_speed / 2);
        }

        if (animData.fade) {
          css.top = $(window).scrollTop() + el.data('css-top') + 'px';
          var end_css = {opacity: 1};

          return setTimeout(function () {
            return el
              .css(css)
              .animate(end_css, settings.animation_speed, 'linear', function () {
                this.locked = false;
                el.trigger('opened');
              }.bind(this))
              .addClass('open');
          }.bind(this), settings.animation_speed / 2);
        }

        return el.css(css).show().css({opacity: 1}).addClass('open').trigger('opened');
      }

      var settings = this.settings;

      // should we animate the background?
      if (getAnimationData(settings.animation).fade) {
        return el.fadeIn(settings.animation_speed / 2);
      }

      this.locked = false;

      return el.show();
    },

    hide : function (el, css) {
      // is modal
      if (css) {
        var settings = el.data(this.attr_name(true) + '-init');
        var animData = getAnimationData(settings.animation);
        if (!animData.animate) {
          this.locked = false;
        }
        if (animData.pop) {
          var end_css = {
            top: - $(window).scrollTop() - el.data('offset') + 'px',
            opacity: 0
          };

          return setTimeout(function () {
            return el
              .animate(end_css, settings.animation_speed, 'linear', function () {
                this.locked = false;
                el.css(css).trigger('closed');
              }.bind(this))
              .removeClass('open');
          }.bind(this), settings.animation_speed / 2);
        }

        if (animData.fade) {
          var end_css = {opacity: 0};

          return setTimeout(function () {
            return el
              .animate(end_css, settings.animation_speed, 'linear', function () {
                this.locked = false;
                el.css(css).trigger('closed');
              }.bind(this))
              .removeClass('open');
          }.bind(this), settings.animation_speed / 2);
        }

        return el.hide().css(css).removeClass('open').trigger('closed');
      }

      var settings = this.settings;

      // should we animate the background?
      if (getAnimationData(settings.animation).fade) {
        return el.fadeOut(settings.animation_speed / 2);
      }

      return el.hide();
    },

    close_video : function (e) {
      var video = $('.flex-video', e.target),
          iframe = $('iframe', video);

      if (iframe.length > 0) {
        iframe.attr('data-src', iframe[0].src);
        iframe.attr('src', 'about:blank');
        video.hide();
      }
    },

    open_video : function (e) {
      var video = $('.flex-video', e.target),
          iframe = video.find('iframe');

      if (iframe.length > 0) {
        var data_src = iframe.attr('data-src');
        if (typeof data_src === 'string') {
          iframe[0].src = iframe.attr('data-src');
        } else {
          var src = iframe[0].src;
          iframe[0].src = undefined;
          iframe[0].src = src;
        }
        video.show();
      }
    },

    data_attr: function (str) {
      if (this.namespace.length > 0) {
        return this.namespace + '-' + str;
      }

      return str;
    },

    cache_offset : function (modal) {
      var offset = modal.show().height() + parseInt(modal.css('top'), 10);

      modal.hide();

      return offset;
    },

    off : function () {
      $(this.scope).off('.fndtn.reveal');
    },

    reflow : function () {}
  };

  /*
   * getAnimationData('popAndFade') // {animate: true,  pop: true,  fade: true}
   * getAnimationData('fade')       // {animate: true,  pop: false, fade: true}
   * getAnimationData('pop')        // {animate: true,  pop: true,  fade: false}
   * getAnimationData('foo')        // {animate: false, pop: false, fade: false}
   * getAnimationData(null)         // {animate: false, pop: false, fade: false}
   */
  function getAnimationData(str) {
    var fade = /fade/i.test(str);
    var pop = /pop/i.test(str);
    return {
      animate: fade || pop,
      pop: pop,
      fade: fade
    };
  }
}(jQuery, this, this.document));

;(function ($, window, document, undefined) {
  

  Foundation.libs.tab = {
    name : 'tab',

    version : '5.5.2',

    settings : {
      active_class : 'active',
      callback : function () {},
      deep_linking : false,
      scroll_to_content : true,
      is_hover : false
    },

    default_tab_hashes : [],

    init : function (scope, method, options) {
      var self = this,
          S = this.S;

	  // Store the default active tabs which will be referenced when the
	  // location hash is absent, as in the case of navigating the tabs and
	  // returning to the first viewing via the browser Back button.
	  S('[' + this.attr_name() + '] > .active > a', this.scope).each(function () {
	    self.default_tab_hashes.push(this.hash);
	  });

      // store the initial href, which is used to allow correct behaviour of the
      // browser back button when deep linking is turned on.
      self.entry_location = window.location.href;

      this.bindings(method, options);
      this.handle_location_hash_change();
    },

    events : function () {
      var self = this,
          S = this.S;

      var usual_tab_behavior =  function (e, target) {
          var settings = S(target).closest('[' + self.attr_name() + ']').data(self.attr_name(true) + '-init');
          if (!settings.is_hover || Modernizr.touch) {
            e.preventDefault();
            e.stopPropagation();
            self.toggle_active_tab(S(target).parent());
          }
        };

      S(this.scope)
        .off('.tab')
        // Key event: focus/tab key
        .on('keydown.fndtn.tab', '[' + this.attr_name() + '] > * > a', function(e) {
          var el = this;
          var keyCode = e.keyCode || e.which;
            // if user pressed tab key
            if (keyCode == 9) { 
              e.preventDefault();
              // TODO: Change usual_tab_behavior into accessibility function?
              usual_tab_behavior(e, el);
            } 
        })
        // Click event: tab title
        .on('click.fndtn.tab', '[' + this.attr_name() + '] > * > a', function(e) {
          var el = this;
          usual_tab_behavior(e, el);
        })
        // Hover event: tab title
        .on('mouseenter.fndtn.tab', '[' + this.attr_name() + '] > * > a', function (e) {
          var settings = S(this).closest('[' + self.attr_name() + ']').data(self.attr_name(true) + '-init');
          if (settings.is_hover) {
            self.toggle_active_tab(S(this).parent());
          }
        });

      // Location hash change event
      S(window).on('hashchange.fndtn.tab', function (e) {
        e.preventDefault();
        self.handle_location_hash_change();
      });
    },

    handle_location_hash_change : function () {

      var self = this,
          S = this.S;

      S('[' + this.attr_name() + ']', this.scope).each(function () {
        var settings = S(this).data(self.attr_name(true) + '-init');
        if (settings.deep_linking) {
          // Match the location hash to a label
          var hash;
          if (settings.scroll_to_content) {
            hash = self.scope.location.hash;
          } else {
            // prefix the hash to prevent anchor scrolling
            hash = self.scope.location.hash.replace('fndtn-', '');
          }
          if (hash != '') {
            // Check whether the location hash references a tab content div or
            // another element on the page (inside or outside the tab content div)
            var hash_element = S(hash);
            if (hash_element.hasClass('content') && hash_element.parent().hasClass('tabs-content')) {
              // Tab content div
              self.toggle_active_tab($('[' + self.attr_name() + '] > * > a[href=' + hash + ']').parent());
            } else {
              // Not the tab content div. If inside the tab content, find the
              // containing tab and toggle it as active.
              var hash_tab_container_id = hash_element.closest('.content').attr('id');
              if (hash_tab_container_id != undefined) {
                self.toggle_active_tab($('[' + self.attr_name() + '] > * > a[href=#' + hash_tab_container_id + ']').parent(), hash);
              }
            }
          } else {
            // Reference the default tab hashes which were initialized in the init function
            for (var ind = 0; ind < self.default_tab_hashes.length; ind++) {
              self.toggle_active_tab($('[' + self.attr_name() + '] > * > a[href=' + self.default_tab_hashes[ind] + ']').parent());
            }
          }
        }
       });
     },

    toggle_active_tab : function (tab, location_hash) {
      var self = this,
          S = self.S,
          tabs = tab.closest('[' + this.attr_name() + ']'),
          tab_link = tab.find('a'),
          anchor = tab.children('a').first(),
          target_hash = '#' + anchor.attr('href').split('#')[1],
          target = S(target_hash),
          siblings = tab.siblings(),
          settings = tabs.data(this.attr_name(true) + '-init'),
          interpret_keyup_action = function (e) {
            // Light modification of Heydon Pickering's Practical ARIA Examples: http://heydonworks.com/practical_aria_examples/js/a11y.js

            // define current, previous and next (possible) tabs

            var $original = $(this);
            var $prev = $(this).parents('li').prev().children('[role="tab"]');
            var $next = $(this).parents('li').next().children('[role="tab"]');
            var $target;

            // find the direction (prev or next)

            switch (e.keyCode) {
              case 37:
                $target = $prev;
                break;
              case 39:
                $target = $next;
                break;
              default:
                $target = false
                  break;
            }

            if ($target.length) {
              $original.attr({
                'tabindex' : '-1',
                'aria-selected' : null
              });
              $target.attr({
                'tabindex' : '0',
                'aria-selected' : true
              }).focus();
            }

            // Hide panels

            $('[role="tabpanel"]')
              .attr('aria-hidden', 'true');

            // Show panel which corresponds to target

            $('#' + $(document.activeElement).attr('href').substring(1))
              .attr('aria-hidden', null);

          },
          go_to_hash = function(hash) {
            // This function allows correct behaviour of the browser's back button when deep linking is enabled. Without it
            // the user would get continually redirected to the default hash.
            var is_entry_location = window.location.href === self.entry_location,
                default_hash = settings.scroll_to_content ? self.default_tab_hashes[0] : is_entry_location ? window.location.hash :'fndtn-' + self.default_tab_hashes[0].replace('#', '')

            if (!(is_entry_location && hash === default_hash)) {
              window.location.hash = hash;
            }
          };

      // allow usage of data-tab-content attribute instead of href
      if (anchor.data('tab-content')) {
        target_hash = '#' + anchor.data('tab-content').split('#')[1];
        target = S(target_hash);
      }

      if (settings.deep_linking) {

        if (settings.scroll_to_content) {

          // retain current hash to scroll to content
          go_to_hash(location_hash || target_hash);

          if (location_hash == undefined || location_hash == target_hash) {
            tab.parent()[0].scrollIntoView();
          } else {
            S(target_hash)[0].scrollIntoView();
          }
        } else {
          // prefix the hashes so that the browser doesn't scroll down
          if (location_hash != undefined) {
            go_to_hash('fndtn-' + location_hash.replace('#', ''));
          } else {
            go_to_hash('fndtn-' + target_hash.replace('#', ''));
          }
        }
      }

      // WARNING: The activation and deactivation of the tab content must
      // occur after the deep linking in order to properly refresh the browser
      // window (notably in Chrome).
      // Clean up multiple attr instances to done once
      tab.addClass(settings.active_class).triggerHandler('opened');
      tab_link.attr({'aria-selected' : 'true',  tabindex : 0});
      siblings.removeClass(settings.active_class)
      siblings.find('a').attr({'aria-selected' : 'false',  tabindex : -1});
      target.siblings().removeClass(settings.active_class).attr({'aria-hidden' : 'true',  tabindex : -1});
      target.addClass(settings.active_class).attr('aria-hidden', 'false').removeAttr('tabindex');
      settings.callback(tab);
      target.triggerHandler('toggled', [target]);
      tabs.triggerHandler('toggled', [tab]);

      tab_link.off('keydown').on('keydown', interpret_keyup_action );
    },

    data_attr : function (str) {
      if (this.namespace.length > 0) {
        return this.namespace + '-' + str;
      }

      return str;
    },

    off : function () {},

    reflow : function () {}
  };
}(jQuery, window, window.document));

;(function ($, window, document, undefined) {
  

  Foundation.libs.tooltip = {
    name : 'tooltip',

    version : '5.2.1',

    settings : {
      additional_inheritable_classes : [],
      tooltip_class : '.tooltip',
      append_to: 'body',
      touch_close_text: 'Tap To Close',
      disable_for_touch: false,
      hover_delay: 200,
      tip_template : function (selector, content) {
        return '<span data-selector="' + selector + '" class="'
          + Foundation.libs.tooltip.settings.tooltip_class.substring(1)
          + '">' + content + '<span class="nub"></span></span>';
      }
    },

    cache : {},

    init : function (scope, method, options) {
      Foundation.inherit(this, 'random_str');
      this.bindings(method, options);
    },

    events : function (instance) {
      var self = this,
          S = self.S;

      self.create(this.S(instance));

      $(this.scope)
        .off('.tooltip')
        .on('mouseenter.fndtn.tooltip mouseleave.fndtn.tooltip touchstart.fndtn.tooltip MSPointerDown.fndtn.tooltip',
          '[' + this.attr_name() + ']:not(a)', function (e) {
          var $this = S(this),
              settings = $.extend({}, self.settings, self.data_options($this)),
              is_touch = false;

          if (/mouse/i.test(e.type) && self.ie_touch(e)) return false;

          if ($this.hasClass('open')) {
            if (Modernizr.touch && /touchstart|MSPointerDown/i.test(e.type)) e.preventDefault();
            self.hide($this);
          } else {
            if (settings.disable_for_touch && Modernizr.touch && /touchstart|MSPointerDown/i.test(e.type)) {
              return;
            } else if(!settings.disable_for_touch && Modernizr.touch && /touchstart|MSPointerDown/i.test(e.type)) {
              e.preventDefault();
              S(settings.tooltip_class + '.open').hide();
              is_touch = true;
            }

            if (/enter|over/i.test(e.type)) {
              this.timer = setTimeout(function () {
                var tip = self.showTip($this);
              }.bind(this), self.settings.hover_delay);
            } else if (e.type === 'mouseout' || e.type === 'mouseleave') {
              clearTimeout(this.timer);
              self.hide($this);
            } else {
              self.showTip($this);
            }
          }
        })
        .on('mouseleave.fndtn.tooltip touchstart.fndtn.tooltip MSPointerDown.fndtn.tooltip', '[' + this.attr_name() + '].open', function (e) {
          if (/mouse/i.test(e.type) && self.ie_touch(e)) return false;

          if($(this).data('tooltip-open-event-type') == 'touch' && e.type == 'mouseleave') {
            return;
          }
          else if($(this).data('tooltip-open-event-type') == 'mouse' && /MSPointerDown|touchstart/i.test(e.type)) {
            self.convert_to_touch($(this));
          } else {
            self.hide($(this));
          }
        })
        .on('DOMNodeRemoved DOMAttrModified', '[' + this.attr_name() + ']:not(a)', function (e) {
          self.hide(S(this));
        });
    },

    ie_touch : function (e) {
      // How do I distinguish between IE11 and Windows Phone 8?????
      return false;
    },

    showTip : function ($target) {
      var $tip = this.getTip($target);

        return this.show($target);
    },

    getTip : function ($target) {
      var selector = this.selector($target),
          settings = $.extend({}, this.settings, this.data_options($target)),
          tip = null;

      if (selector) {
        tip = this.S('span[data-selector="' + selector + '"]' + settings.tooltip_class);
      }

      return (typeof tip === 'object') ? tip : false;
    },

    selector : function ($target) {
      var id = $target.attr('id'),
          dataSelector = $target.attr(this.attr_name()) || $target.attr('data-selector');

      if ((id && id.length < 1 || !id) && typeof dataSelector != 'string') {
        dataSelector = this.random_str(6);
        $target.attr('data-selector', dataSelector);
      }

      return (id && id.length > 0) ? id : dataSelector;
    },

    create : function ($target) {
      var self = this,
          settings = $.extend({}, this.settings, this.data_options($target)),
          tip_template = this.settings.tip_template;

      if (typeof settings.tip_template === 'string' && window.hasOwnProperty(settings.tip_template)) {
        tip_template = window[settings.tip_template];
      }

      var $tip = $(tip_template(this.selector($target), $('<div></div>').html($target.attr('title')).html())),
          classes = this.inheritable_classes($target);

      $tip.addClass(classes).appendTo(settings.append_to);

      if (Modernizr.touch) {
        $tip.append('<span class="tap-to-close">'+settings.touch_close_text+'</span>');
        $tip.on('touchstart.fndtn.tooltip MSPointerDown.fndtn.tooltip', function(e) {
          self.hide($target);
        });
      }

      $target.removeAttr('title').attr('title','');
    },

    reposition : function (target, tip, classes) {
      var width, nub, nubHeight, nubWidth, column, objPos;

      tip.css('visibility', 'hidden').show();

      width = target.data('width');
      nub = tip.children('.nub');
      nubHeight = nub.outerHeight();
      nubWidth = nub.outerHeight();

      if (this.small()) {
        tip.css({'width' : '100%' });
      } else {
        tip.css({'width' : (width) ? width : 'auto'});
      }

      objPos = function (obj, top, right, bottom, left, width) {
        return obj.css({
          'top' : (top) ? top : 'auto',
          'bottom' : (bottom) ? bottom : 'auto',
          'left' : (left) ? left : 'auto',
          'right' : (right) ? right : 'auto',
        }).end();
      };

      objPos(tip, (target.offset().top + target.outerHeight() + 10), 'auto', 'auto', target.offset().left);

      if (this.small()) {
        objPos(tip, (target.offset().top + target.outerHeight() + 10), 'auto', 'auto', 12.5, $(this.scope).width());
        tip.addClass('tip-override');
        objPos(nub, -nubHeight, 'auto', 'auto', target.offset().left);
      } else {
        var left = target.offset().left;
        if (Foundation.rtl) {
          nub.addClass('rtl');
          left = target.offset().left + target.outerWidth() - tip.outerWidth();
        }
        objPos(tip, (target.offset().top + target.outerHeight() + 10), 'auto', 'auto', left);
        tip.removeClass('tip-override');
        if (classes && classes.indexOf('tip-top') > -1) {
          if (Foundation.rtl) nub.addClass('rtl');
          objPos(tip, (target.offset().top - tip.outerHeight()), 'auto', 'auto', left)
            .removeClass('tip-override');
        } else if (classes && classes.indexOf('tip-left') > -1) {
          objPos(tip, (target.offset().top + (target.outerHeight() / 2) - (tip.outerHeight() / 2)), 'auto', 'auto', (target.offset().left - tip.outerWidth() - nubHeight))
            .removeClass('tip-override');
          nub.removeClass('rtl');
        } else if (classes && classes.indexOf('tip-right') > -1) {
          objPos(tip, (target.offset().top + (target.outerHeight() / 2) - (tip.outerHeight() / 2)), 'auto', 'auto', (target.offset().left + target.outerWidth() + nubHeight))
            .removeClass('tip-override');
          nub.removeClass('rtl');
        }
      }

      tip.css('visibility', 'visible').hide();
    },

    small : function () {
      return matchMedia(Foundation.media_queries.small).matches;
    },

    inheritable_classes : function ($target) {
      var settings = $.extend({}, this.settings, this.data_options($target)),
          inheritables = ['tip-top', 'tip-left', 'tip-bottom', 'tip-right', 'radius', 'round'].concat(settings.additional_inheritable_classes),
          classes = $target.attr('class'),
          filtered = classes ? $.map(classes.split(' '), function (el, i) {
            if ($.inArray(el, inheritables) !== -1) {
              return el;
            }
          }).join(' ') : '';

      return $.trim(filtered);
    },

    convert_to_touch : function($target) {
      var self = this,
          $tip = self.getTip($target),
          settings = $.extend({}, self.settings, self.data_options($target));

      if ($tip.find('.tap-to-close').length === 0) {
        $tip.append('<span class="tap-to-close">'+settings.touch_close_text+'</span>');
        $tip.on('click.fndtn.tooltip.tapclose touchstart.fndtn.tooltip.tapclose MSPointerDown.fndtn.tooltip.tapclose', function(e) {
          self.hide($target);
        });
      }

      $target.data('tooltip-open-event-type', 'touch');
    },

    show : function ($target) {
      var $tip = this.getTip($target);

      if ($target.data('tooltip-open-event-type') == 'touch') {
        this.convert_to_touch($target);
      }

      this.reposition($target, $tip, $target.attr('class'));
      $target.addClass('open');
      $tip.fadeIn(150);
    },

    hide : function ($target) {
      var $tip = this.getTip($target);

      $tip.fadeOut(150, function() {
        $tip.find('.tap-to-close').remove();
        $tip.off('click.fndtn.tooltip.tapclose touchstart.fndtn.tooltip.tapclose MSPointerDown.fndtn.tapclose');
        $target.removeClass('open');
      });
    },

    off : function () {
      var self = this;
      this.S(this.scope).off('.fndtn.tooltip');
      this.S(this.settings.tooltip_class).each(function (i) {
        $('[' + self.attr_name() + ']').get(i).attr('title', $(this).text());
      }).remove();
    },

    reflow : function () {}
  };
}(jQuery, this, this.document));

;(function ($, window, document, undefined) {
  

  Foundation.libs.topbar = {
    name : 'topbar',

    version: '5.4.7',

    settings : {
      index : 0,
      sticky_class : 'sticky',
      custom_back_text: true,
      back_text: 'Back',
      mobile_show_parent_link: true,
      is_hover: true,
      scrolltop : true, // jump to top when sticky nav menu toggle is clicked
      sticky_on : 'all'
    },

    init : function (section, method, options) {
      Foundation.inherit(this, 'add_custom_rule register_media throttle');
      var self = this;

      self.register_media('topbar', 'foundation-mq-topbar');

      this.bindings(method, options);

      self.S('[' + this.attr_name() + ']', this.scope).each(function () {
        var topbar = $(this),
            settings = topbar.data(self.attr_name(true) + '-init'),
            section = self.S('section, .top-bar-section', this);
        topbar.data('index', 0);
        var topbarContainer = topbar.parent();
        if (topbarContainer.hasClass('fixed') || self.is_sticky(topbar, topbarContainer, settings) ) {
          self.settings.sticky_class = settings.sticky_class;
          self.settings.sticky_topbar = topbar;
          topbar.data('height', topbarContainer.outerHeight());
          topbar.data('stickyoffset', topbarContainer.offset().top);
        } else {
          topbar.data('height', topbar.outerHeight());
        }

        if (!settings.assembled) {
          self.assemble(topbar);
        }

        if (settings.is_hover) {
          self.S('.has-dropdown', topbar).addClass('not-click');
        } else {
          self.S('.has-dropdown', topbar).removeClass('not-click');
        }

        // Pad body when sticky (scrolled) or fixed.
        self.add_custom_rule('.f-topbar-fixed { padding-top: ' + topbar.data('height') + 'px }');

        if (topbarContainer.hasClass('fixed')) {
          self.S('body').addClass('f-topbar-fixed');
        }
      });

    },

    is_sticky: function (topbar, topbarContainer, settings) {
      var sticky = topbarContainer.hasClass(settings.sticky_class);

      if (sticky && settings.sticky_on === 'all') {
        return true;
      } else if (sticky && this.small() && settings.sticky_on === 'small') {
        return (matchMedia(Foundation.media_queries.small).matches && !matchMedia(Foundation.media_queries.medium).matches &&
            !matchMedia(Foundation.media_queries.large).matches);
        //return true;
      } else if (sticky && this.medium() && settings.sticky_on === 'medium') {
        return (matchMedia(Foundation.media_queries.small).matches && matchMedia(Foundation.media_queries.medium).matches &&
            !matchMedia(Foundation.media_queries.large).matches);
        //return true;
      } else if(sticky && this.large() && settings.sticky_on === 'large') {
        return (matchMedia(Foundation.media_queries.small).matches && matchMedia(Foundation.media_queries.medium).matches &&
            matchMedia(Foundation.media_queries.large).matches);
        //return true;
      }

      return false;
    },

    toggle: function (toggleEl) {
      var self = this,
          topbar;

      if (toggleEl) {
        topbar = self.S(toggleEl).closest('[' + this.attr_name() + ']');
      } else {
        topbar = self.S('[' + this.attr_name() + ']');
      }

      var settings = topbar.data(this.attr_name(true) + '-init');

      var section = self.S('section, .top-bar-section', topbar);

      if (self.breakpoint()) {
        if (!self.rtl) {
          section.css({left: '0%'});
          $('>.name', section).css({left: '100%'});
        } else {
          section.css({right: '0%'});
          $('>.name', section).css({right: '100%'});
        }

        self.S('li.moved', section).removeClass('moved');
        topbar.data('index', 0);

        topbar
          .toggleClass('expanded')
          .css('height', '');
      }

      if (settings.scrolltop) {
        if (!topbar.hasClass('expanded')) {
          if (topbar.hasClass('fixed')) {
            topbar.parent().addClass('fixed');
            topbar.removeClass('fixed');
            self.S('body').addClass('f-topbar-fixed');
          }
        } else if (topbar.parent().hasClass('fixed')) {
          if (settings.scrolltop) {
            topbar.parent().removeClass('fixed');
            topbar.addClass('fixed');
            self.S('body').removeClass('f-topbar-fixed');

            window.scrollTo(0,0);
          } else {
            topbar.parent().removeClass('expanded');
          }
        }
      } else {
        if (self.is_sticky(topbar, topbar.parent(), settings)) {
          topbar.parent().addClass('fixed');
        }

        if (topbar.parent().hasClass('fixed')) {
          if (!topbar.hasClass('expanded')) {
            topbar.removeClass('fixed');
            topbar.parent().removeClass('expanded');
            self.update_sticky_positioning();
          } else {
            topbar.addClass('fixed');
            topbar.parent().addClass('expanded');
            self.S('body').addClass('f-topbar-fixed');
          }
        }
      }
    },

    timer : null,

    events : function (bar) {
      var self = this,
          S = this.S;

      S(this.scope)
        .off('.topbar')
        .on('click.fndtn.topbar', '[' + this.attr_name() + '] .toggle-topbar', function (e) {
          e.preventDefault();
          self.toggle(this);
        })
        .on('click.fndtn.topbar','.top-bar .top-bar-section li a[href^="#"],[' + this.attr_name() + '] .top-bar-section li a[href^="#"]',function (e) {
            var li = $(this).closest('li');
            if(self.breakpoint() && !li.hasClass('back') && !li.hasClass('has-dropdown'))
            {
            self.toggle();
            }
        })
        .on('click.fndtn.topbar', '[' + this.attr_name() + '] li.has-dropdown', function (e) {
          var li = S(this),
              target = S(e.target),
              topbar = li.closest('[' + self.attr_name() + ']'),
              settings = topbar.data(self.attr_name(true) + '-init');
          //this is ASA customized code for fixing FireFox doesn't blur issue.Start
          $(target).blur();
          //this is ASA customized code for fixing FireFox doesn't blur issue.End
          if(target.data('revealId')) {
            self.toggle();
            return;
          }

          if (self.breakpoint()) return;
          if (settings.is_hover && !Modernizr.touch) return;

          e.stopImmediatePropagation();

          if (li.hasClass('hover')) {
            li
              .removeClass('hover')
              .find('li')
              .removeClass('hover');

            li.parents('li.hover')
              .removeClass('hover');
          } else {
            li.addClass('hover');

            $(li).siblings().removeClass('hover');

            if (target[0].nodeName === 'A' && target.parent().hasClass('has-dropdown')) {
              e.preventDefault();
            }
          }
        })
        .on('click.fndtn.topbar', '[' + this.attr_name() + '] .has-dropdown>a', function (e) {
          if (self.breakpoint()) {

            e.preventDefault();

            var $this = S(this),
                topbar = $this.closest('[' + self.attr_name() + ']'),
                section = topbar.find('section, .top-bar-section'),
                dropdownHeight = $this.next('.dropdown').outerHeight(),
                $selectedLi = $this.closest('li');

            topbar.data('index', topbar.data('index') + 1);
            $selectedLi.addClass('moved');

            if (!self.rtl) {
              section.css({left: -(100 * topbar.data('index')) + '%'});
              section.find('>.name').css({left: 100 * topbar.data('index') + '%'});
            } else {
              section.css({right: -(100 * topbar.data('index')) + '%'});
              section.find('>.name').css({right: 100 * topbar.data('index') + '%'});
            }

            topbar.css('height', $this.siblings('ul').outerHeight(true) + topbar.data('height'));
          }
        });

      S(window).off(".topbar").on("resize.fndtn.topbar", self.throttle(function() {
          self.resize.call(self);
      }, 50)).trigger("resize").trigger("resize.fndtn.topbar").load(function(){
          // Ensure that the offset is calculated after all of the pages resources have loaded
          S(this).trigger("resize.fndtn.topbar");
      });

      S('body').off('.topbar').on('click.fndtn.topbar', function (e) {
        var parent = S(e.target).closest('li').closest('li.hover');

        if (parent.length > 0) {
          return;
        }

        S('[' + self.attr_name() + '] li.hover').removeClass('hover');
      });

      // Go up a level on Click
      S(this.scope).on('click.fndtn.topbar', '[' + this.attr_name() + '] .has-dropdown .back', function (e) {
        e.preventDefault();

        var $this = S(this),
            topbar = $this.closest('[' + self.attr_name() + ']'),
            section = topbar.find('section, .top-bar-section'),
            settings = topbar.data(self.attr_name(true) + '-init'),
            $movedLi = $this.closest('li.moved'),
            $previousLevelUl = $movedLi.parent();

        topbar.data('index', topbar.data('index') - 1);

        if (!self.rtl) {
          section.css({left: -(100 * topbar.data('index')) + '%'});
          section.find('>.name').css({left: 100 * topbar.data('index') + '%'});
        } else {
          section.css({right: -(100 * topbar.data('index')) + '%'});
          section.find('>.name').css({right: 100 * topbar.data('index') + '%'});
        }

        if (topbar.data('index') === 0) {
          topbar.css('height', '');
        } else {
          topbar.css('height', $previousLevelUl.outerHeight(true) + topbar.data('height'));
        }

        setTimeout(function () {
          $movedLi.removeClass('moved');
        }, 300);
      });

      // Show dropdown menus when their items are focused
      S(this.scope).find('.dropdown a')
        .focus(function() {
          $(this).parents('.has-dropdown').addClass('hover');
        })
        .blur(function() {
          $(this).parents('.has-dropdown').removeClass('hover');
        });
    },

    resize : function () {
      var self = this;
      self.S('[' + this.attr_name() + ']').each(function () {
        var topbar = self.S(this),
            settings = topbar.data(self.attr_name(true) + '-init');

        var stickyContainer = topbar.parent('.' + self.settings.sticky_class);
        var stickyOffset;

        if (!self.breakpoint()) {
          var doToggle = topbar.hasClass('expanded');
          topbar
            .css('height', '')
            .removeClass('expanded')
            .find('li')
            .removeClass('hover');

            if(doToggle) {
              self.toggle(topbar);
            }
        }

        if(self.is_sticky(topbar, stickyContainer, settings)) {
          if(stickyContainer.hasClass('fixed')) {
            // Remove the fixed to allow for correct calculation of the offset.
            stickyContainer.removeClass('fixed');

            stickyOffset = stickyContainer.offset().top;
            if(self.S(document.body).hasClass('f-topbar-fixed')) {
              stickyOffset -= topbar.data('height');
            }

            topbar.data('stickyoffset', stickyOffset);
            stickyContainer.addClass('fixed');
          } else {
            stickyOffset = stickyContainer.offset().top;
            topbar.data('stickyoffset', stickyOffset);
          }
        }

      });
    },

    breakpoint : function () {
      return !matchMedia(Foundation.media_queries['topbar']).matches;
    },

    small : function () {
      return matchMedia(Foundation.media_queries['small']).matches;
    },

    medium : function () {
      return matchMedia(Foundation.media_queries['medium']).matches;
    },

    large : function () {
      return matchMedia(Foundation.media_queries['large']).matches;
    },

    assemble : function (topbar) {
      var self = this,
          settings = topbar.data(this.attr_name(true) + '-init'),
          section = self.S('section, .top-bar-section', topbar);

      // Pull element out of the DOM for manipulation
      section.detach();

      self.S('.has-dropdown>a', section).each(function () {
        var $link = self.S(this),
            $dropdown = $link.siblings('.dropdown'),
            url = $link.attr('href'),
            $titleLi;


        if (!$dropdown.find('.title.back').length) {

          if (settings.mobile_show_parent_link == true && url) {
            $titleLi = $('<li class="title back js-generated"><h5><a href="javascript:void(0)"></a></h5></li><li class="parent-link show-for-small"><a class="parent-link js-generated" href="' + url + '">' + $link.html() +'</a></li>');
          } else {
            $titleLi = $('<li class="title back js-generated"><h5><a href="javascript:void(0)"></a></h5>');
          }

          // Copy link to subnav
          if (settings.custom_back_text == true) {
            $('h5>a', $titleLi).html(settings.back_text);
          } else {
            $('h5>a', $titleLi).html('&laquo; ' + $link.html());
          }
          $dropdown.prepend($titleLi);
        }
      });

      // Put element back in the DOM
      section.appendTo(topbar);

      // check for sticky
      this.sticky();

      this.assembled(topbar);
    },

    assembled : function (topbar) {
      topbar.data(this.attr_name(true), $.extend({}, topbar.data(this.attr_name(true)), {assembled: true}));
    },

    height : function (ul) {
      var total = 0,
          self = this;

      $('> li', ul).each(function () {
        total += self.S(this).outerHeight(true);
      });

      return total;
    },

    sticky : function () {
      var self = this;

      this.S(window).on('scroll', function() {
        self.update_sticky_positioning();
      });
    },

    update_sticky_positioning: function() {
      var klass = '.' + this.settings.sticky_class,
          $window = this.S(window),
          self = this;

      if (self.settings.sticky_topbar && self.is_sticky(this.settings.sticky_topbar,this.settings.sticky_topbar.parent(), this.settings)) {
        var distance = this.settings.sticky_topbar.data('stickyoffset');
        if (!self.S(klass).hasClass('expanded')) {
          if ($window.scrollTop() > (distance)) {
            if (!self.S(klass).hasClass('fixed')) {
              self.S(klass).addClass('fixed');
              self.S('body').addClass('f-topbar-fixed');
            }
          } else if ($window.scrollTop() <= distance) {
            if (self.S(klass).hasClass('fixed')) {
              self.S(klass).removeClass('fixed');
              self.S('body').removeClass('f-topbar-fixed');
            }
          }
        }
      }
    },

    off : function () {
      this.S(this.scope).off('.fndtn.topbar');
      this.S(window).off('.fndtn.topbar');
    },

    reflow : function () {}
  };
}(jQuery, window, window.document));

define("foundation5", ["jquery","foundation.lib"], function(){});

/* global dcsMultiTrack */
require([
    'jquery',
    'salt',
    'salt/models/SiteMember',
    'asa/ASAUtilities',
    'salt/analytics/webtrends-config',
    'underscore',
    'salt/LoginManager',
    'salt/analytics/webtrends',
    'foundation5',
    'jquery.cookie'
], function ($, SALT, SiteMember, Utility, WT, _) {


    function setFocus(element) {
        var elSetFoucus = function () {
            $(element).focus();
        };
        setTimeout(elSetFoucus, 350);
    }

    //Open overlay opens a reveal overlay and Publishes a overlay opened event with page specific data
    //  it takes 3 arguments
    //      -a css selector in string form e.g. '#foo' or '#bar'
    //      -a string with the overlay name to be passed to webtrends
    //      -a string with the url of the current page to be passed to webtrends
    SALT.openOverlay = function (selector, overlayName, currentPage) {
        $(selector).foundation('reveal', 'open');
    };

    SALT.on('open:customOverlay', function (overlay) {
        openWindowShade(overlay);
    });

    SALT.on('close:customOverlay', function (overlay) {
        closeWindowShade(overlay);
    });

    //Open overlay closes a reveal overlay
    //  it takes 1 argument
    //      -a css selector in string form e.g. '#foo' or '#bar'
    SALT.closeOverlay = function (selector) {
        $(selector).foundation('reveal', 'close');
    };
    
    $(document.body).on('click', '.js-reveal-close', function () {
        $(this).closest('.reveal-modal').foundation('reveal', 'close');
    });

    $('body').on('click', '.loginButton, .js-reg-button', function (e) {
        e.preventDefault();
        if (!$(e.currentTarget).is('[data-window-shade]')) {
            if ($(e.currentTarget).hasClass('loginButton')) {
                $(this).attr('data-window-shade', 'loginOverlay');
            } else {
                $(this).attr('data-window-shade', 'registrationOverlay');
            }
            $(this).trigger('click');
        }
    });

    $(document.body).on('click', '[data-window-shade]', function (e) {
        e.preventDefault();
        var windowshadeToOpen = $(this).attr('data-window-shade');
        toggleNoMenuHeader(windowshadeToOpen);
        if (windowshadeToOpen === 'registrationOverlay') {
            if ($.cookie('TooYoung')) {
                return;
            }
            if ($(window).height() < 750) {
                window.location = '/register' + window.location.search;
                return;
            }
        }
        if (windowshadeToOpen === 'loginOverlay') {
            if ($(window).height() < 400 || window.location.pathname === '/register') {
                window.location = '/logon';
                return;
            }
        }
        openWindowShade(windowshadeToOpen);
    });

    $(document.body).on('click keydown', '.window-shade-close', function (e) {
        if (e.type === 'click' || e.keyCode === 13) {
            var $windowShadeToClose = $(this).closest('.custom-overlay'),
                focusOnAfterSel = $(this).attr('data-focus-sel');
            closeWindowShade($windowShadeToClose.attr('id'));
            if (focusOnAfterSel) {
                $(focusOnAfterSel).focus();
            }
        }
    });

    function toggleNoMenuHeader(windowshadeToOpen) {
        if (windowshadeToOpen === 'registrationOverlay') {
            $('.js-registration-link').hide();
            $('.js-login-link').show();
        }
        else if (windowshadeToOpen === 'loginOverlay') {
            $('.js-login-link').hide();
            $('.js-registration-link').show();
        }
    }

    function windowShadeClosedCleanUp($windowShade) {
        $windowShade.find('form').trigger('reset');
        if ($windowShade.attr('id') === 'registrationOverlay') {
            SALT.trigger('resetControlsToDefault');
        }

        $('label[class="error"]').empty();
        $('ul.main-nav li a').removeClass('inactive');

        //Clean up server errors
        $('.js-server-error-text').text('');
        $('#email-reused').addClass('hidden');
    }

    function windowShadeOpenedCleanUp($windowShade) {
        if ($windowShade.attr('id') === 'registrationOverlay') {
            SALT.trigger('prePopulateSchool');
            SALT.trigger('showRadioButtons');
            SALT.publish('Registration:Start');
            Utility.handleRegWallWT('Wall_Signup_Start');
        }
        else if ($windowShade.attr('id') === 'loginOverlay') {
            Utility.handleRegWallWT('Wall_Login_Start');
            //need to reinitialize LoginManger
            Utility.trackLastVisitedURL();
            var returnUrl = Utility.getRerouteURL();
            loginOverlayHandler(returnUrl);
        }
        $windowShade.find('form').find(':input:visible:enabled:first').focus();
        $('ul.main-nav li a').addClass('inactive');
    }

    var loginOverlayHandler = function (returnUrl) {
        if (!returnUrl) {
            returnUrl = location.pathname + location.search;
        }
        SALT.LoginManager.init({
            logonFormName: '#loginForm',
            errorFieldName: '#errorCode',
            returnURLOverride: returnUrl
        });
    };

    $(document).ready(function () {
        if (!$.cookie('IndividualId')) {
            //stop after validation
            Utility.trackLastVisitedURL();
            var returnUrl = Utility.getRerouteURL();
            loginOverlayHandler(returnUrl);
            //third condition below for SWD-6480
            if (returnUrl &&  //there's return url
                window.location.href.indexOf("/register") === -1) {
                openWindowShade('loginOverlay');
            }
        }
    });

    function openWindowShade(windowShade) {
        var $windowShade = $('#' + windowShade);
        if ($windowShade.is(':hidden')) {
            var currentlyOpenWindowShadeID = $('.custom-overlay:visible').attr('id');
            //make sure all other window shades are closed in order that their forms be reset
            //We dont want forms to be reset when the confirm personal info is about to be opened, check for this case before closing
            if (currentlyOpenWindowShadeID && windowShade !== 'confirmPersonalInfo') {
                closeWindowShade(currentlyOpenWindowShadeID);
            } else {
                //ConfirmPersonalInfo case, just slide up the registration form rather than fully closing and restting the form
                $('.custom-overlay').slideUp('slow');
            }
            // open the window shade
            $windowShade.slideDown('slow', function () {
                //clean up after opening the window shade
                windowShadeOpenedCleanUp($windowShade);
            });
        }
    }

    function closeWindowShade(windowShade) {
        var $windowShade = $('#' + windowShade);
        $windowShade.slideUp('slow', function () {
            //clean up after closing the window shade
            windowShadeClosedCleanUp($windowShade);
        });
    }

});

define("modules/overlays", function(){});

/*global getParameterByName */
require([
    'require',
    'underscore',
    'jquery',
    'salt',
    'salt/models/SiteMember',
    'asa/ASAUtilities',
    'modules/overlays',
    'foundation5'
], function (req, _, $, SALT, SiteMember, utility) {

    SiteMember.done(function (siteMember) {
        if (siteMember.IsAuthenticated === 'true') {
            if (siteMember.DashboardEnabled) {
                require(['modules/Dashboard', 'modules/ProgressWidgets']);

                //Set the cookie to take the user to onboarding on their next visit to /home if they dont yet have goal ranking responses foo
                if (!siteMember.goalRankResponses.length) {
                    document.cookie = 'NeedsOnboarding = true; path=/';
                }
                var redraw = function () {
                    if ($(window).width() > 1025) {
                        //f-dropdown adds inline styling to #js-sidebar as it opens - which persists if the browser is resized larger.
                        //By removing the inline style, the div will revert to the proper css class for it's media query.
                        $('#js-sidebar').removeAttr('style');
                    }
                };
                var debouncedRedraw = _.debounce(redraw, 400);
                $(window).on('resize', debouncedRedraw);
            }
            if (siteMember.OnboardingEnabled) {
                require(['modules/Onboarding']);
            }

            $(document.body).click(function () {
                SALT.trigger('sessionTimeOut:reset');
            });
        }
    });
    $(function () {
        //pass all query strings for mobile devices on logon and signup page.
        var getPathNameFromString = function (string) {
            return string.substring(string.indexOf('/'), string.indexOf('?') === -1 ? string.length : string.indexOf('?'));
        };
        $(document.body).on('click', '.registration-link', function (e) {
            var queryString,
                href;
            e.preventDefault();
            queryString = utility.getLocationSearch();
            href = $(this).attr('href');

            if (queryString) {
                window.location.href = getPathNameFromString(href) + queryString;
            } else {
                window.location.href = getPathNameFromString(href);
            }
        });

        var searchOpenedYet = false;
        $(document.body).on('click keydown', '.js-search-magn', function (e) {
            if (e.type === 'click' || e.which === 13) {
                e.preventDefault();

                //If this is the first time opening the search bar on the page fire a SALT event
                if (!searchOpenedYet) {
                    SALT.trigger('search:opened:firsttime');
                    searchOpenedYet = true;
                }

                $('.js-search-form').toggleClass('hide').find(':input:visible:enabled:first').focus();
                $('.js-search').toggleClass('hide');
                $('.js-header').toggleClass('search-magn-active');
            }
        });
        // functionality for x button to clear text and shutoff autocomplete
        $(document.body).on('click keydown', '.js-search-x', function (e) {
            if (e.type === 'click' || e.which === 13) {
                e.preventDefault();
                e.stopPropagation();
                $('#searchCriteria').attr('value', '').focus().autocomplete('close');
                $('.autocomplete-suggestions').hide();
            }
        });
        /*to close the search box when clicked anywhere on the page */
        $(document.body).on('click', function (e) {
            if (!($(e.target).is('#search-btn, .js-search-label, #searchForm .search'))) {
                $('.js-search-form').addClass('hide');
                $('.js-search').addClass('hide');
                $('.js-header').removeClass('search-magn-active');
            }
        });
        //on click of menu-btn and profile-btn
        //if the .js-search-form does not have class hide then add class hide
        $(document.body).on('click', '.salt-header .dd-opener', function (e) {
            if (!$('.js-search-form').hasClass('hide')) {
                $('.js-search-form').addClass('hide');
                $('.js-search').addClass('hide');
            }
        });
        $(document.body).on('click', '.js-scroll-top-button', function (e) {
            e.preventDefault();
            $('html, body').animate({scrollTop: 0}, 300);
        });

        $(document.body).on('click', '.no-accordion a, .dropdown li>a', function (e) {
            Foundation.libs.dropdown.close($('.left-menu'));
        });

        $(document.body).on('keydown', '.glossaryTerm', function (e) {
            if (e.which === 9) {
                $('#glossaryClose').click();
            }
        });

        $(document.body).on('click', '.left-menu li:not(.menu-copyright) a, .left-menu .no-accordion a', function (e) {
            var itemText = e.target.text.toString();
            SALT.publish('dashboard:action:taken', {'activity_name': 'Main Menu', 'activity_type': itemText});
        });
        $(document.body).on('click keydown', '.js-f-skip-link', function (e) {
            if (e.type === 'click' || e.which === 13) {
                $('.js-footer-container').focus();
            }
        });
        $(document.body).on('click keydown', '.js-rr-skip-link', function (e) {
            if (e.type === 'click' || e.which === 13) {
                $('#js-sidebar').focus();
            }
        });
        $(document).on('click', '.liveChatLink, .js-live-chat-now-btn', function (e) {
            e.preventDefault();
            $('#aLHNBTN').click();
            var actName = $(this).next('.js-livechat-WT-act').val();
            //If we didnt find a "js-livechat-WT-act" value, check the data-wt-value field on the input itself
            //Newer links will have this field, as it allows for more flexibility in the structure of the html (the .next call enforces that js-livechat-wt-act be directly after the input, which is bad)
            actName = actName ? actName : $(this).attr('data-wt-value');
            if (actName) {
                SALT.publish('Standard:Action:Start', { 'activity_name': actName});
            }
        });
        //the dropdowns on dashboard do not close when clicked on right rail
        //the data-dropdown-content a foundation element is impacting this. It is required for the right rail to close onsmall screens. So data-dropdown-content is being removed for screen sizes bigger than 640.
        $(window).on('resize', function () {
            if ($(window).width() > 1025) {
                $('#js-sidebar').attr('data-dropdown-content', null);
            } else {
                $('#js-sidebar').attr('data-dropdown-content', '');
            }
        });
        SiteMember.done(function (siteMember) {
            if (siteMember.IsAuthenticated === 'true') {
                if (!$.cookie('IndividualId')) {
                    //In remembered state the links need re authorisation
                    $('.MM101 a').attr('href', '/index.html?ReturnUrl=' + $('.MM101 a').attr('href'));
                    $('.SC a').attr('href', '/index.html?ReturnUrl=' + $('.SC a').attr('href') + '/login.jspa');
                }
            }
        });

        // stop the site search form from submitting if blank, aka 0 characters -khennings
        $('#searchForm').on('submit', function (e) {
            var searchValue = $('#searchCriteria').val().length;
            if (searchValue === 0) {
                e.preventDefault();
            } else {
                return;
            }
        });

        // stop the site search form from submitting if blank, aka 0 characters -khennings
        $('#mobileSearchForm').on('submit', function (e) {
            var searchValue = $('#mobileSubmit').val().length;
            if (searchValue === 0) {
                e.preventDefault();
            } else {
                return;
            }
        });

        req(['salt/analytics/webtrends']);


        $(document.body).on('change', '.js-profileQA-container input:checkbox, #js-tag-selection-container input:checkbox', function () {
            var $this = $(this),
                thisName = $this.attr('name');
            // if the current selection is "None", uncheck all other options
            if ($this.hasClass('js-none') && $this.is(':checked')) {
                $('input[name=' + thisName + ']').removeAttr('checked');
                $this.attr('checked', '');
                // hide the "other" input field and clear it.
                $this.closest('.js-question-container').find('.other-ans').hide().find('.js-other-input').val('');
                //prevent unchecking the only checked checkbox
            } else if (!$('input[name=' + thisName + ']').is(':checked') && !$this.hasClass('js-single-check')) {
                $this.attr('checked', '');
                // user clicked on "other" answer, show/hide the text field
            } else if ($this.hasClass('js-other')) {
                if ($this.is(':checked')) {
                    $this.siblings('.other-ans').show();
                    // Uncheck none answers.
                    $('input[name=' + thisName + '].js-none').removeAttr('checked');
                } else {
                    // hide the "other" input field and clear it.
                    $this.siblings('.other-ans').hide().find('.js-other-input').val('');
                }
                // else if the user selects an aption that is not "none", uncheck "none"
            } else {
                $('input[name=' + thisName + '].js-none').removeAttr('checked');
            }
        });
        // when a user modifies an "other" input field, concat the the value to the value of corresponding input field
        $(document.body).on('change', '.js-other-input', function () {
            var otherFieldValue = document.getElementById(this.id.substr(this.id.indexOf('ans'))),
                customVal = otherFieldValue.value.split('-')[2];
            // if we previousely didn't have a custom value stored
            if (!customVal) {
                otherFieldValue.value += '-' + this.value;
            } else {
                // if we have a previous custom value, delete it
                var customValIndex = otherFieldValue.value.indexOf(customVal);
                otherFieldValue.value = otherFieldValue.value.substring(0, customValIndex - 1);
                // if we have a new custom value to replace the old one, use it, other wise just delete it
                if (this.value) {
                    otherFieldValue.value += '-' + this.value;
                }
            }
        });

        //Open rel="external" links in new window

        $("a[rel*='external']").live('click', function () {
            var externalLinkMessage = "You are leaving the Salt\xAE site and will be directed to a third-party site. Please read the Privacy Policy and Terms and Conditions of that site.";
            if (confirm(externalLinkMessage)) {
                window.open(this.href);
            }
            return false;
        });
    });
});

define("main", function(){});

/*! A fix for the iOS orientationchange zoom bug.
 Script by @scottjehl, rebound by @wilto.
 MIT / GPLv2 License.
*/
(function(w){
	
	// This fix addresses an iOS bug, so return early if the UA claims it's something else.
	var ua = navigator.userAgent;
	if( !( /iPhone|iPad|iPod/.test( navigator.platform ) && /OS [1-5]_[0-9_]* like Mac OS X/i.test(ua) && ua.indexOf( "AppleWebKit" ) > -1 ) ){
		return;
	}

    var doc = w.document;

    if( !doc.querySelector ){ return; }

    var meta = doc.querySelector( "meta[name=viewport]" ),
        initialContent = meta && meta.getAttribute( "content" ),
        disabledZoom = initialContent + ",maximum-scale=1",
        enabledZoom = initialContent + ",maximum-scale=10",
        enabled = true,
		x, y, z, aig;

    if( !meta ){ return; }

    function restoreZoom(){
        meta.setAttribute( "content", enabledZoom );
        enabled = true;
    }

    function disableZoom(){
        meta.setAttribute( "content", disabledZoom );
        enabled = false;
    }
	
    function checkTilt( e ){
		aig = e.accelerationIncludingGravity;
		x = Math.abs( aig.x );
		y = Math.abs( aig.y );
		z = Math.abs( aig.z );
				
		// If portrait orientation and in one of the danger zones
        if( (!w.orientation || w.orientation === 180) && ( x > 7 || ( ( z > 6 && y < 8 || z < 8 && y > 6 ) && x > 5 ) ) ){
			if( enabled ){
				disableZoom();
			}        	
        }
		else if( !enabled ){
			restoreZoom();
        }
    }
	
	w.addEventListener( "orientationchange", restoreZoom, false );
	w.addEventListener( "devicemotion", checkTilt, false );

})( this );
define("libs/ios-orientationchange-fix", function(){});

//This is the format for all entries into this list - "Content Type Name": "Action Word"
//The character limit for "Action Word" is 10
//How to edit: 
//Below is a list of all the contents types and their matching "action word"
//Only edit the "Content Type Name" and the "Action Word".
//If you change the "Content Type Name" remember to update the matching icons in "Assets/images"
//DO NOT Delete this file
//DO NOT Change the structure or format of the text below. That includes indentations and these characters ("", : , =, {, } , ; )
var actionWords = {
	"Article": "read",
	"eBook": "download",	
	"Form": "download",
	"Video": "watch",	
	"Infographic": "view",
	"Comic": "view",
	"Tool": "try",
	"Lesson": "do",
	"Course": "study"
};
//-------------------------------------------------------------------------------------------------------------
//TOUCH NOTHING BELOW THIS LINE IF YOU ARE NOT A DEVELOPER
//-------------------------------------------------------------------------------------------------------------
//The following is a shim to allow this file to be require'd both client and server side
if (typeof exports !== 'undefined') {
	if (typeof module !== 'undefined' && module.exports) {
		//node.js require style using exports
		module.exports = actionWords;
	}
} else {
	//require.js/AMD require style for use in browser
	define('actionWords',actionWords);
};
//
// Dust-helpers - Additional functionality for dustjs-linkedin package v1.1.1
//
// Copyright (c) 2012, LinkedIn
// Released under the MIT License.
//

(function(dust){

// Note: all error conditions are logged to console and failed silently

/* make a safe version of console if it is not available
 * currently supporting:
 *   _console.log
 * */
var _console = (typeof console !== 'undefined')? console: {
  log: function(){
     /* a noop*/
   }
};

function isSelect(context) {
  var value = context.current();
  return typeof value === "object" && value.isSelect === true;
}

// Utility method : toString() equivalent for functions
function jsonFilter(key, value) {
  if (typeof value === "function") {
    return value.toString();
  }
  return value;
}

// Utility method: to invoke the given filter operation such as eq/gt etc
function filter(chunk, context, bodies, params, filterOp) {
  params = params || {};
  var body = bodies.block,
      actualKey,
      expectedValue,
      filterOpType = params.filterOpType || '';
  // when @eq, @lt etc are used as standalone helpers, key is required and hence check for defined
  if ( typeof params.key !== "undefined") {
    actualKey = dust.helpers.tap(params.key, chunk, context);
  }
  else if (isSelect(context)) {
    actualKey = context.current().selectKey;
    //  supports only one of the blocks in the select to be selected
    if (context.current().isResolved) {
      filterOp = function() { return false; };
    }
  }
  else {
    _console.log ("No key specified for filter in:" + filterOpType + " helper ");
    return chunk;
  }
  expectedValue = dust.helpers.tap(params.value, chunk, context);
  // coerce both the actualKey and expectedValue to the same type for equality and non-equality compares
  if (filterOp(coerce(expectedValue, params.type, context), coerce(actualKey, params.type, context))) {
    if (isSelect(context)) {
      context.current().isResolved = true;
    }
    // we want helpers without bodies to fail gracefully so check it first
    if(body) {
     return chunk.render(body, context);
    }
    else {
      _console.log( "Missing body block in the " + filterOpType + " helper ");
      return chunk;
    }
   }
   else if (bodies['else']) {
    return chunk.render(bodies['else'], context);
  }
  return chunk;
}

function coerce (value, type, context) {
  if (value) {
    switch (type || typeof(value)) {
      case 'number': return +value;
      case 'string': return String(value);
      case 'boolean': {
        value = (value === 'false' ? false : value);
        return Boolean(value);
      }
      case 'date': return new Date(value);
      case 'context': return context.get(value);
    }
  }

  return value;
}

var helpers = {

  // Utility helping to resolve dust references in the given chunk
  // uses the Chunk.render method to resolve value
  /*
   Reference resolution rules:
   if value exists in JSON:
    "" or '' will evaluate to false, boolean false, null, or undefined will evaluate to false,
    numeric 0 evaluates to true, so does, string "0", string "null", string "undefined" and string "false". 
    Also note that empty array -> [] is evaluated to false and empty object -> {} and non-empty object are evaluated to true
    The type of the return value is string ( since we concatenate to support interpolated references 

   if value does not exist in JSON and the input is a single reference: {x}
     dust render emits empty string, and we then return false   
     
   if values does not exist in JSON and the input is interpolated references : {x} < {y}
     dust render emits <  and we return the partial output 
     
  */
  "tap": function( input, chunk, context ){
    // return given input if there is no dust reference to resolve
    var output = input;
    // dust compiles a string/reference such as {foo} to function, 
    if( typeof input === "function"){
      // just a plain function (a.k.a anonymous functions) in the context, not a dust `body` function created by the dust compiler
      if( input.isFunction === true ){
        output = input();
      } else {
        output = '';
        chunk.tap(function(data){
           output += data;
           return '';
          }).render(input, context).untap();
        if( output === '' ){
          output = false;
        }
      }
    }
   return output;
  },

  "sep": function(chunk, context, bodies) {
    var body = bodies.block;
    if (context.stack.index === context.stack.of - 1) {
      return chunk;
    }
    if(body) {
     return bodies.block(chunk, context);
    }
    else {
     return chunk;
    }
  },

  "idx": function(chunk, context, bodies) {
    var body = bodies.block;
     if(body) {
       return bodies.block(chunk, context.push(context.stack.index));
     }
     else {
       return chunk;
     }
  },

  /**
   * contextDump helper
   * @param key specifies how much to dump.
   * "current" dumps current context. "full" dumps the full context stack.
   * @param to specifies where to write dump output.
   * Values can be "console" or "output". Default is output.
   */
  "contextDump": function(chunk, context, bodies, params) {
    var p = params || {},
      to = p.to || 'output',
      key = p.key || 'current',
      dump;
    to = dust.helpers.tap(to, chunk, context),
    key = dust.helpers.tap(key, chunk, context);
    if (key === 'full') {
      dump = JSON.stringify(context.stack, jsonFilter, 2);
    }
    else {
      dump = JSON.stringify(context.stack.head, jsonFilter, 2);
    }
    if (to === 'console') {
      _console.log(dump);
      return chunk;
    }
    else {
      return chunk.write(dump);
    }
  },
  /**
   if helper for complex evaluation complex logic expressions.
   Note : #1 if helper fails gracefully when there is no body block nor else block
          #2 Undefined values and false values in the JSON need to be handled specially with .length check
             for e.g @if cond=" '{a}'.length && '{b}'.length" is advised when there are chances of the a and b been
             undefined or false in the context
          #3 Use only when the default ? and ^ dust operators and the select fall short in addressing the given logic,
             since eval executes in the global scope
          #4 All dust references are default escaped as they are resolved, hence eval will block malicious scripts in the context
             Be mindful of evaluating a expression that is passed through the unescape filter -> |s
   @param cond, either a string literal value or a dust reference
                a string literal value, is enclosed in double quotes, e.g. cond="2>3"
                a dust reference is also enclosed in double quotes, e.g. cond="'{val}'' > 3"
    cond argument should evaluate to a valid javascript expression
   **/

  "if": function( chunk, context, bodies, params ){
    var body = bodies.block,
        skip = bodies['else'];
    if( params && params.cond){
      var cond = params.cond;
      cond = dust.helpers.tap(cond, chunk, context);
      // eval expressions with given dust references
      if(eval(cond)){
       if(body) {
        return chunk.render( bodies.block, context );
       }
       else {
         _console.log( "Missing body block in the if helper!" );
         return chunk;
       }
      }
      if(skip){
       return chunk.render( bodies['else'], context );
      }
    }
    // no condition
    else {
      _console.log( "No condition given in the if helper!" );
    }
    return chunk;
  },

  /**
   * math helper
   * @param key is the value to perform math against
   * @param method is the math method,  is a valid string supported by math helper like mod, add, subtract
   * @param operand is the second value needed for operations like mod, add, subtract, etc.
   * @param round is a flag to assure that an integer is returned
   */
  "math": function ( chunk, context, bodies, params ) {
    //key and method are required for further processing
    if( params && typeof params.key !== "undefined" && params.method ){
      var key  = params.key,
          method = params.method,
          // operand can be null for "abs", ceil and floor
          operand = params.operand,
          round = params.round,
          mathOut = null,
          operError = function(){_console.log("operand is required for this math method"); return null;};
      key  = dust.helpers.tap(key, chunk, context);
      operand = dust.helpers.tap(operand, chunk, context);
      //  TODO: handle  and tests for negatives and floats in all math operations
      switch(method) {
        case "mod":
          if(operand === 0 || operand === -0) {
            _console.log("operand for divide operation is 0/-0: expect Nan!");
          }
          mathOut = parseFloat(key) %  parseFloat(operand);
          break;
        case "add":
          mathOut = parseFloat(key) + parseFloat(operand);
          break;
        case "subtract":
          mathOut = parseFloat(key) - parseFloat(operand);
          break;
        case "multiply":
          mathOut = parseFloat(key) * parseFloat(operand);
          break;
        case "divide":
         if(operand === 0 || operand === -0) {
           _console.log("operand for divide operation is 0/-0: expect Nan/Infinity!");
         }
          mathOut = parseFloat(key) / parseFloat(operand);
          break;
        case "ceil":
          mathOut = Math.ceil(parseFloat(key));
          break;
        case "floor":
          mathOut = Math.floor(parseFloat(key));
          break;
        case "round":
          mathOut = Math.round(parseFloat(key));
          break;
        case "abs":
          mathOut = Math.abs(parseFloat(key));
          break;
        default:
          _console.log( "method passed is not supported" );
     }

      if (mathOut !== null){
        if (round) {
          mathOut = Math.round(mathOut);
        }
        if (bodies && bodies.block) {
          // with bodies act like the select helper with mathOut as the key
          // like the select helper bodies['else'] is meaningless and is ignored
          return chunk.render(bodies.block, context.push({ isSelect: true, isResolved: false, selectKey: mathOut }));
        } else {
          // self closing math helper will return the calculated output
          return chunk.write(mathOut);
        }
       } else {
        return chunk;
      }
    }
    // no key parameter and no method
    else {
      _console.log( "Key is a required parameter for math helper along with method/operand!" );
    }
    return chunk;
  },
   /**
   select helperworks with one of the eq/gt/gte/lt/lte/default providing the functionality
   of branching conditions
   @param key,  ( required ) either a string literal value or a dust reference
                a string literal value, is enclosed in double quotes, e.g. key="foo"
                a dust reference may or may not be enclosed in double quotes, e.g. key="{val}" and key=val are both valid
   @param type (optional), supported types are  number, boolean, string, date, context, defaults to string
   **/
  "select": function(chunk, context, bodies, params) {
    var body = bodies.block;
    // key is required for processing, hence check for defined
    if( params && typeof params.key !== "undefined"){
      // returns given input as output, if the input is not a dust reference, else does a context lookup
      var key = dust.helpers.tap(params.key, chunk, context);
      // bodies['else'] is meaningless and is ignored
      if( body ) {
       return chunk.render(bodies.block, context.push({ isSelect: true, isResolved: false, selectKey: key }));
      }
      else {
       _console.log( "Missing body block in the select helper ");
       return chunk;
      }
    }
    // no key
    else {
      _console.log( "No key given in the select helper!" );
    }
    return chunk;
  },

  /**
   eq helper compares the given key is same as the expected value
   It can be used standalone or in conjunction with select for multiple branching
   @param key,  The actual key to be compared ( optional when helper used in conjunction with select)
                either a string literal value or a dust reference
                a string literal value, is enclosed in double quotes, e.g. key="foo"
                a dust reference may or may not be enclosed in double quotes, e.g. key="{val}" and key=val are both valid
   @param value, The expected value to compare to, when helper is used standalone or in conjunction with select
   @param type (optional), supported types are  number, boolean, string, date, context, defaults to string
   Note : use type="number" when comparing numeric
   **/
  "eq": function(chunk, context, bodies, params) {
    if(params) {
      params.filterOpType = "eq";
    }
    return filter(chunk, context, bodies, params, function(expected, actual) { return actual === expected; });
  },

  /**
   ne helper compares the given key is not the same as the expected value
   It can be used standalone or in conjunction with select for multiple branching
   @param key,  The actual key to be compared ( optional when helper used in conjunction with select)
                either a string literal value or a dust reference
                a string literal value, is enclosed in double quotes, e.g. key="foo"
                a dust reference may or may not be enclosed in double quotes, e.g. key="{val}" and key=val are both valid
   @param value, The expected value to compare to, when helper is used standalone or in conjunction with select
   @param type (optional), supported types are  number, boolean, string, date, context, defaults to string
   Note : use type="number" when comparing numeric
   **/
  "ne": function(chunk, context, bodies, params) {
    if(params) {
      params.filterOpType = "ne";
      return filter(chunk, context, bodies, params, function(expected, actual) { return actual !== expected; });
    }
   return chunk;
  },

  /**
   lt helper compares the given key is less than the expected value
   It can be used standalone or in conjunction with select for multiple branching
   @param key,  The actual key to be compared ( optional when helper used in conjunction with select)
                either a string literal value or a dust reference
                a string literal value, is enclosed in double quotes, e.g. key="foo"
                a dust reference may or may not be enclosed in double quotes, e.g. key="{val}" and key=val are both valid
   @param value, The expected value to compare to, when helper is used standalone  or in conjunction with select
   @param type (optional), supported types are  number, boolean, string, date, context, defaults to string
   Note : use type="number" when comparing numeric
   **/
  "lt": function(chunk, context, bodies, params) {
     if(params) {
       params.filterOpType = "lt";
       return filter(chunk, context, bodies, params, function(expected, actual) { return actual < expected; });
     }
  },

  /**
   lte helper compares the given key is less or equal to the expected value
   It can be used standalone or in conjunction with select for multiple branching
   @param key,  The actual key to be compared ( optional when helper used in conjunction with select)
                either a string literal value or a dust reference
                a string literal value, is enclosed in double quotes, e.g. key="foo"
                a dust reference may or may not be enclosed in double quotes, e.g. key="{val}" and key=val are both valid
   @param value, The expected value to compare to, when helper is used standalone or in conjunction with select
   @param type (optional), supported types are  number, boolean, string, date, context, defaults to string
   Note : use type="number" when comparing numeric
  **/
  "lte": function(chunk, context, bodies, params) {
     if(params) {
       params.filterOpType = "lte";
       return filter(chunk, context, bodies, params, function(expected, actual) { return actual <= expected; });
     }
    return chunk;
  },


  /**
   gt helper compares the given key is greater than the expected value
   It can be used standalone or in conjunction with select for multiple branching
   @param key,  The actual key to be compared ( optional when helper used in conjunction with select)
                either a string literal value or a dust reference
                a string literal value, is enclosed in double quotes, e.g. key="foo"
                a dust reference may or may not be enclosed in double quotes, e.g. key="{val}" and key=val are both valid
   @param value, The expected value to compare to, when helper is used standalone  or in conjunction with select
   @param type (optional), supported types are  number, boolean, string, date, context, defaults to string
   Note : use type="number" when comparing numeric
   **/
  "gt": function(chunk, context, bodies, params) {
    // if no params do no go further
    if(params) {
      params.filterOpType = "gt";
      return filter(chunk, context, bodies, params, function(expected, actual) { return actual > expected; });
    }
    return chunk;
  },

 /**
   gte helper, compares the given key is greater than or equal to the expected value
   It can be used standalone or in conjunction with select for multiple branching
   @param key,  The actual key to be compared ( optional when helper used in conjunction with select)
                either a string literal value or a dust reference
                a string literal value, is enclosed in double quotes, e.g. key="foo"
                a dust reference may or may not be enclosed in double quotes, e.g. key="{val}" and key=val are both valid
   @param value, The expected value to compare to, when helper is used standalone or in conjunction with select
   @param type (optional), supported types are  number, boolean, string, date, context, defaults to string
   Note : use type="number" when comparing numeric
  **/
  "gte": function(chunk, context, bodies, params) {
     if(params) {
      params.filterOpType = "gte";
      return filter(chunk, context, bodies, params, function(expected, actual) { return actual >= expected; });
     }
    return chunk; 
  },

  // to be used in conjunction with the select helper
  // TODO: fix the helper to do nothing when used standalone
  "default": function(chunk, context, bodies, params) {
    // does not require any params
     if(params) {
        params.filterOpType = "default";
      }
     return filter(chunk, context, bodies, params, function(expected, actual) { return true; });
  },

  /**
  * size helper prints the size of the given key
  * Note : size helper is self closing and does not support bodies
  * @param key, the element whose size is returned
  */
  "size": function( chunk, context, bodies, params ) {
    var key, value=0, nr, k;
    params = params || {};
    key = params.key;
    if (!key || key === true) { //undefined, null, "", 0
      value = 0;
    }
    else if(dust.isArray(key)) { //array
      value = key.length;
    }
    else if (!isNaN(parseFloat(key)) && isFinite(key)) { //numeric values
      value = key;
    }
    else if (typeof key  === "object") { //object test
      //objects, null and array all have typeof ojbect...
      //null and array are already tested so typeof is sufficient http://jsperf.com/isobject-tests
      nr = 0;
      for(k in key){
        if(Object.hasOwnProperty.call(key,k)){
          nr++;
        }
      }
      value = nr;
    } else {
      value = (key + '').length; //any other value (strings etc.)
    }
    return chunk.write(value);
  }
  
  
};

dust.helpers = helpers;

})(typeof exports !== 'undefined' ? module.exports = require('dustjs-linkedin') : dust);
define("dust-helpers", ["dust"], function(){});

/* jshint maxstatements: 55 */
define('modules/SaltDustHelpers',[
    'dust',
    'asa/ASAUtilities',
    'underscore',
    'configuration',
    'actionWords',
    'dust-helpers'
], function (dust, Utility, _, Configuration, actionWords) {
    dust.filters.refinementState = function (value) {
        return value.substring(value.indexOf('/_/'), value.indexOf('?'));
    };

    dust.filters.shorten = function (value) {
        var lastSpace = value.lastIndexOf(' '),
            firstDash = value.indexOf('-');
            //If there's a dash, and its before the last space, cutoff at dash, otherwise use last space.
            //Sometimes the dash is after the last space eg. "Texarkana TX-AR"
        if (firstDash > -1 && firstDash < lastSpace) {
            return value.substring(0, firstDash);
        }
        return value.substring(0, lastSpace);
    };

    dust.filters.actionType = function (contentType) {
        var actionText = actionWords[contentType[0]];
        if (!actionText) {
            return 'try';
        } else { 
            return actionText;
        }
    };

    dust.filters.contentMainNav = function (value) {
        var contentMap = {
            article: 'N-15',
            video: 'N-16',
            lesson: 'N-18',
            form: 'N-17',
            comic: 'N-4d',
            infographic: 'N-19',
            tool: 'N-1A'
        };
        return contentMap[value.toLowerCase()];
    };

    dust.filters.trim = function (value) {
        if (Array.isArray(value)) {
            return value[0].trim();
        }
        return value.trim();
    };

    dust.filters.contentRefineNav = function (value) {
        var toReturn = '';
        var pathArray = value.split('/');
        if (pathArray) {
            toReturn = pathArray[pathArray.length - 1].split('?')[0];
        }
        return toReturn;
    };

    //potentially leverage helper with params instead to combine these two filters
    dust.filters.returnFirstFlatTag = function (value) {
        var arrayvalue = 0;
        var delim = '|';
        var toReturn = '';
        var parsedArray = value.split(delim);
        if (parsedArray && parsedArray.length > arrayvalue) {
            toReturn = parsedArray[arrayvalue];
            toReturn = toReturn.trim();
        }
        return toReturn;
    };

    dust.filters.returnSecondFlatTag = function (value) {
        var arrayvalue = 1;
        var delim = '|';
        var toReturn = '';
        var parsedArray = value.split(delim);
        if (parsedArray && parsedArray.length > arrayvalue) {
            toReturn = parsedArray[arrayvalue];
            toReturn = toReturn.trim();
        }
        return toReturn;
    };

    //This function is duplicated in the server helpers file at /Content/salt-dust-helpers.js
    //DO NOT MODIFY WITHOUT MODIFYING THAT FUNCTION AS WELL.  BETTER YET, COMBINE THESE FILES PLEASE
    dust.filters.currencyComma = Utility.currencyComma;

    dust.helpers.pathname = function (chunk, ctx, bodies, params) {
        return chunk.write(($.browser.msie) ? Utility.getPathnameHash(location, true) : Utility.getPathnameHash(location, false));
    };

    dust.filters.dc = function (value) {
        return $($.parseHTML(value)).text();
    };

    dust.filters.lc = function (value) {
        return value.toLowerCase();
    };

    dust.filters.us = function (value) {
        return value.replace(/ /g, '_');
    };

    dust.filters.ceil = function (value) {
        if (isNaN(value)) {
            return value;
        }
        return Math.ceil(value);
    };

    dust.filters.abs = function (value) {
        if (isNaN(value)) {
            return value;
        }
        return Math.abs(value);
    };

    //Write out a count of the results, omitting definitions
    dust.filters.countWithoutDefinitions  = function (secondaryContent) {
        var refinements = Utility.checkNested(secondaryContent, 'navigation[0].refinements'),
            //if it's not search result page, we don't need short page spacer in tiles.dust
            count = location.pathname.indexOf('content/search') > -1 ? 0 : '';
        if (refinements && refinements.length) {
            count = _.reduce(refinements, function (runningCount, obj) {
                if (obj.label !== 'Definition') {
                    return runningCount + obj.count;
                } else {
                    return runningCount;
                }
            }, 0);
        }
        return count;
    };

    dust.helpers.glossaryTile = function (chunk, ctx, bodies, params) {
        var searchTerm = this.tap(params.search, chunk, ctx),
            adjustedSearches = params.adjustedSearches,
            records = this.tap(params.records, chunk, ctx),
            termToDisplay = {},
            out = '',
            searchTermArray = '';

        if (searchTerm) {
            searchTerm = searchTerm.toLowerCase();
        } else {
            return chunk.write(out);
        }

        var definitions = _.filter(records, function (rec) {
            return rec.attributes.ContentTypes[0].toLowerCase() === 'definition';
        });

        if (adjustedSearches) {
            searchTerm = getAdjustedTerm(adjustedSearches);
            //SWD-7641 - adjusted terms may come back with double quotes
            searchTerm = searchTerm.replace(/"/g, '');
        }

        //SWD-7640: Strip out all non alpha numeric characters from search term for comparison
        var searchTermNoSpaces = searchTerm.replace(/[\W_]+/g, '');

        //Try to find an exact match for the search term or find the searchTerm within the acronym first
        termToDisplay = _.find(definitions, function (definition) {
            var termNoSpaces = definition.attributes.term[0].trim().toLowerCase().replace(/[\W_]+/g, ''),
                acronymNoSpaces = definition.attributes.acronym[0].trim().toLowerCase().replace(/[\W_]+/g, '');

            return termNoSpaces === searchTermNoSpaces ||
            acronymNoSpaces.indexOf(searchTermNoSpaces) > -1;
        });

        if (!termToDisplay) {
            termToDisplay = definitions[0];
            //if we're defaulting the definition record, check to see if the search term is a part of the defaulted record's
            //"term" attribute (search each token of the search term). This is for SWD-6668 and SWD-7641
            searchTermArray = searchTerm.split(' ');
            _.each(searchTermArray, function (searchTermItem) {
                if (termToDisplay.attributes.term[0].trim().toLowerCase().indexOf(searchTermItem) > -1) {
                    termToDisplay.display = true;
                }
            });
        } else {
            termToDisplay.display = true;
        }

        //SWD 6668 - If search term and acronym are not a part of the definition's term, don't display definition treatment
        if (termToDisplay.display) {
            out = '<div class="definition-body"><h2><a href="/home/glossary.html#' + termToDisplay.attributes.letter[0].substr(1, 1) + '" title="' + termToDisplay.attributes.resource_link_title[0] + '">' + termToDisplay.attributes.term[0].trim() +
                ':</a> (Definition)</h2>' + termToDisplay.attributes.body[0] + "</div>";
        }

        return chunk.write(out);
    };

    //This function is duplicated WITH SOME CHANGES in the server helpers file at /Content/salt-dust-helpers.js
    //DO NOT MODIFY WITHOUT MODIFYING THAT FUNCTION AS WELL.  BETTER YET, COMBINE THESE FILES PLEASE
    /*
    accepts the following parameters:
    - startYear
    - range
    - gradDate (grad date as saved in DB. To be used for graduation year only)
    - savedValue (to be usedfor non-gradDate year dropdowns)
    */
    dust.helpers.yearDropdown = function (chunk, ctx, bodies, params) {
        var manip = this.tap(params.range, chunk, ctx),
            absolute = manip / Math.abs(manip),
            year =  new Date().getFullYear(),
            startYear = parseInt(this.tap(params.startYear, chunk, ctx), 10),
            gradDate = this.tap(params.gradDate, chunk, ctx),
            savedValue = this.tap(params.savedValue, chunk, ctx),
            stringToWrite = '';

        if (startYear) {
            year += startYear;
        }
        if (gradDate) {
            savedValue = gradDate;
        }
        savedValue = savedValue ? parseInt(savedValue, 10) : null;

        for (var i = 0; i < Math.abs(manip) + 1; i++) {
            if ((year + absolute * i) === savedValue) {
                stringToWrite += '<option value="' + (year + absolute * i) + '" selected >' + (year + absolute * i) + '</option>';
            } else {
                stringToWrite += '<option value="' + (year + absolute * i) + '">' + (year + absolute * i) + '</option>';
            }
        }
        return chunk.write(stringToWrite);
    };

    dust.helpers.dateParser = function (chunk, ctx, bodies, params) {
        /* helper to parse date from the  server format to a user friendly format*/
        var myDate = this.tap(params.utcDate, chunk, ctx);
        var parsedDate = Utility.convertSALtoJSdate(myDate);
        return chunk.write(parsedDate.toDateString());
    };

    dust.helpers.dateLinkChooser = function  (chunk, ctx, bodies, params) {
        var enrollStatus = this.tap(params.enrollStatus, chunk, ctx);
        var link = (enrollStatus === 'Enrolled') ? '#WhenWillYouGrad' : '#LastAttended';

        return chunk.write(link);
    };
    dust.helpers.urlParser = function (chunk, ctx, bodies, params) {
        /* helper to parse url from the data and normalize it - written for scholarship search*/
        var url = this.tap(params.url, chunk, ctx);
        if (url.substring(0, 4) === 'http') {
            return chunk.write(url);
        }
        else {
            return chunk.write('http://' + url);
        }
    };

    dust.helpers.contactParser = function (chunk, ctx, bodies, params) {
        //format contact data coming back from Unigo
        var populatedFields = {}, contact = '', address = '', cityStateZip = '';

        for (var key in params) {
        //null and undefined are already null
        //leave out other 'null' data
            if (params[key] && params[key] !== '-' && params[key] !== '- ') {
                populatedFields[key] = params[key];
            }
        }
        if (populatedFields.providerName) {
            contact = populatedFields.providerName + "<br>";
        }
        //if  contactPerson exists, don't use the providerName
        if (populatedFields.contactPerson) {
            contact = "Attn: " + populatedFields.contactPerson + "<br>";
        }
        if (populatedFields.contactTitle) {
            contact += populatedFields.contactTitle  + "<br>";
        }
        if (contact) {
            contact += '<br>';
        }
        if (populatedFields.address1) {
            address = populatedFields.address1;
        }
        //add address2 to address1 without a comma, per the comp
        if (populatedFields.address1 && populatedFields.address2) {
            address += ' ' + populatedFields.address2;
        }
        if (address) {
            address += '<br>';
        }
        if (populatedFields.city) {
            cityStateZip += populatedFields.city;
        }
        if (populatedFields.province) {
            cityStateZip += ', ' + populatedFields.province;
        }
        if (populatedFields.state) {
            cityStateZip += ', ' + populatedFields.state;
        }
        if (populatedFields.zipCode) {
            cityStateZip += ' ' + populatedFields.zipCode;
        }
        if (cityStateZip) {
            cityStateZip += '<br>';
        }
        //if address is null and there is no email or phone we don't need a second <br>, otherwise we do.
        if (address && populatedFields.email1 || populatedFields.phone1) {
            cityStateZip += '<br>';
        }
        return chunk.write(contact + address + cityStateZip);
    };

    dust.helpers.substr = function (chunk, ctx, bodies, params) {
        // Get the values of all the parameters. The tap function takes care of resolving any variable references
        // used in parameters (e.g. param="{name}"
        var str = this.tap(params.str, chunk, ctx),
            begin = dust.helpers.tap(params.begin, chunk, ctx),
            end = dust.helpers.tap(params.end, chunk, ctx),
            len = dust.helpers.tap(params.len, chunk, ctx);
        begin = begin || 0; // Default begin to zero if omitted
        // Use JavaScript substr if len is supplied.
        // Helpers need to return some value using chunk. Here we write the substring into chunk.
        // If you have nothing to output, just return chunk.write("");
        if ((typeof(len) !== 'undefined')) {
            return chunk.write(str.substr(begin, len));
        }
        if ((typeof(end) !== 'undefined')) {
            return chunk.write(str.slice(begin, end));
        }
        if ((typeof(end) === 'undefined') && (typeof(len) === 'undefined')) {
            return chunk.write(str.substr(begin));
        }
        return chunk.write(str);
    };

    dust.helpers.dateUS = function (chunk, ctx, bodies, params) {
        //Converts 2004-08-10 into 08/10/2004
        var dateStr = this.tap(params.dateString, chunk, ctx),
            year =  dateStr.substr(0, 4),
            month = dateStr.substr(5, 2),
            day = dateStr.substr(8, 2);

        return chunk.write(year + '/' + month + '/' + day);
    };

    //This function is duplicated in the server helpers file at /Content/salt-dust-helpers.js
    //DO NOT MODIFY WITHOUT MODIFYING THAT FUNCTION AS WELL.  BETTER YET, COMBINE THESE FILES PLEASE
    dust.helpers.flatTagsSeperator = function (chunk, ctx, bodies, params) {
        //seperates the Flat Tags and convert them into searchable Anchor Tags
        var tags = this.tap(params.str, chunk, ctx),
            tagStr = '';
        if (tags !== ' ' && (typeof(tags) === 'string')) {
            var tagArray = tags.split('|'),
                tagLinks = tagArray[0],
                startTag = '<a class="js-SPA" href="/content/search?ntk=Tags&searchCriteria=';
            tagStr = startTag + encodeURIComponent(tagArray[0].trim()) + '" >' + tagArray[0] + '</a>';
            for (var i = 1; i < tagArray.length; i++) {
                tagLinks += ',' +  tagArray[i];
                if ((tagLinks.trim()).length <= 50 && (ctx.templateName === 'oldTiles' || ctx.templateName === 'partial_todo')) {
                    tagStr += ', ' + startTag + encodeURIComponent(tagArray[i].trim()) + '" >' + tagArray[i] + '</a>';
                } else if (ctx.templateName !== 'oldTiles' && ctx.templateName !== 'partial_todo') {
                    tagStr += ', ' + startTag + encodeURIComponent(tagArray[i].trim()) + '" >' + tagArray[i] + '</a>';
                }
            }
        }
        return chunk.write(tagStr);
    };

    dust.helpers.featuredTaskImage = function (chunk, ctx, bodies, params) {
        var tags = this.tap(params.tagList, chunk, ctx),
            primaryKey = this.tap(params.primaryKey, chunk, ctx),
            lookupkey = '',
            imageName = '',
            imageAlt = '';

        if (primaryKey !== ' ' && (typeof(primaryKey) === 'string')) {
            var toolName = '',
                altName = '';
            switch (primaryKey) {
            case '101-8645':
                toolName = 'repaymentnavigator',
                altName = 'Repayment Navigator';
                break;
            case '101-12042':
                toolName = 'salaryestimator',
                altName = 'Salary Estimator';
                break;
            case '101-7416':
                toolName = 'scholarshipsearch',
                altName = 'Scholarship Search';
                break;
            case '101-4367':
                toolName = 'savingscalculator',
                altName = 'Savings Calculator';
                break;
            case '101-13584':
                toolName = 'kwyo',
                altName = "Debt Organizer";
                break;
            default:
            }
            if (toolName) {
                imageName = 'icon_' + toolName;
                imageAlt = 'Icon for ' + altName + 'tool';
            }
        }

        // if we don't have a specific image for the tool, then
        // pick one by the tags
        if (!imageName && tags !== ' ' && (typeof(tags) === 'string')) {
            var tagArray = tags.split('|'),
                firstTag = tagArray[0].replace(/\s/g, ''),
                altText = '';

            switch (firstTag) {
            case 'credit':
            case 'creditscores':
                firstTag = 'creditdebt',
                altText = 'Credit Debt';
                break;
            case 'checkingaccounts':
                firstTag = 'banking',
                altText = 'Credit Debt';
                break;
            case 'resumes':
            case 'careerskills':
            case 'careerchoice':
                firstTag = 'jobsearch',
                altText = 'Job Search';
                break;
            case 'scholarships':
                firstTag = 'scholarshipsearch',
                altText = 'Scholarship Search';
                break;
            case 'collegesavings':
                firstTag = 'savings',
                altText = 'College Savings';
                break;
            case 'postponements':
            case 'cancellations':
                firstTag = 'studentloanrepayment',
                altText = 'Student Loan Repayment';
                break;


            default:
            }
            imageName = 'icon_' + firstTag,
            imageAlt = 'Icon for ' + altText + 'tool';
        }
        var htmlOut = '<i role="image" class="small-12 salticon featuredIcon large ' + imageName + '" ' + 'aria-label="' + imageAlt + '"></i>';
        return chunk.write(htmlOut);
    };
    //find the tags for the content on the page
    //map the gloat tags to the goals
    // add the goals to the gloat image so that the image can be displayed.
    dust.helpers.gloatImage = function (chunk, ctx, bodies, params) {
        var tags = this.tap(params.tagList, chunk, ctx),
            psTags = Configuration.goalTags.PayForSchool,
            rsdTags = Configuration.goalTags.RepayStudentDebt,
            fjTags = Configuration.goalTags.FindAJob,
            mmTags = Configuration.goalTags.MasterMoney;
           
        // Default if there are no tags
        var gloatHtml = '<img src="/assets/images/sidebar/join-salt1.jpg" title="Join Salt." alt="Join Salt"/>';        
        
        // tags exist    
        if (tags !== ' ' && (typeof(tags) === 'string')) {
            var tagArray = tags.split('|').map(function (nospace) {
                    return nospace.trim();
                }),
                primaryTag = tagArray[0],
                rsdTagsArray = rsdTags.split(','),
                psTagsArray = psTags.split(','),
                fjTagsArray = fjTags.split(','),
                mmTagsArray = mmTags.split(','),
                gloatGoal = '';

            if (rsdTagsArray.indexOf(primaryTag) !== -1) {
                gloatGoal = 'repaystudentdebt-join';
            } else if (psTagsArray.indexOf(primaryTag) !== -1) {
                gloatGoal = 'payforschool-join';
            } else if (fjTagsArray.indexOf(primaryTag) !== -1) {
                gloatGoal = 'findajob-join';
            } else if (mmTagsArray.indexOf(primaryTag) !== -1) {
                gloatGoal = 'managemoney-join';
            } else {
                gloatGoal = 'join-salt1';
            }

            gloatHtml = '<img src="/assets/images/sidebar/' + gloatGoal + '.jpg" title="Join Salt." alt="Join Salt"/>';
        }
        return chunk.write(gloatHtml);
    };

    dust.helpers.personaButtons = function (chunk, ctx, bodies, params) {
        var links = this.tap(params.links, chunk, ctx),
            copy = this.tap(params.copy, chunk, ctx),
            out = '';

        //Strip <p> that CM1 puts in
        copy = copy.substr(copy.indexOf('<p>') + 3, copy.indexOf('</p>') - 4).split('|');

        links.trim().split('|').forEach(function (el, ind, arr) {
            if (el === 'Home') {
                el = '';
            }
            out += '<div class="unauth-home-btn-wrapper small-6 columns">' +
              '<a href="index.html?Type=' + el.replace(/\s/g, '') + '" class="home-link button base-btn main-btn unauth-home-btn js-persona-btn">' +
            copy[ind] + '</a></div>';
        });
        return chunk.write(out);
    };

    dust.helpers.personaNav = function (chunk, ctx, bodies, params) {
        var links = this.tap(params.links, chunk, ctx),
            copy = this.tap(params.copy, chunk, ctx),
            out = '';

        links = links.trim();

        if (copy.indexOf('<p>') > -1) {
            copy = copy.substr(copy.indexOf('<p>') + 3, copy.indexOf('</p>') - 4);
        }
        copy = copy.trim().split('|');
        links.split('|').forEach(function (el, ind, arr) {
            if (el === 'Home') {
                el = '';
            }
            out += '<li><a href="/index.html?Type=' + el + '" class="home-link" title="' + copy[ind] + '">' + copy[ind] + '<i class="fa fa-caret-up"></i></a></li>';
        });
        return chunk.write(out);
    };

    /*this helper is for outputing keys*/
    dust.helpers.iterate = function (chunk, context, bodies, params) {
        params = params || {};
        var obj = params.on || context.current();
        for (var k in obj) {
            chunk = chunk.render(bodies.block, context.push({key: k, value: obj[k]}));
        }
        return chunk;
    };

    /*this helper is for outputing keys*/
    dust.helpers.checkProfileSelect = function (chunk, ctx, bodies, params) {
        params = params || {};
        var responses = params.responsesObj,
            ansID  = this.tap(params.ansVal, chunk, ctx),
            lastAnswer;
        if (responses.length) {
            var lastResponse = responses[responses.length - 1];
            lastAnswer = lastResponse.QuestionExternalId + '-' + lastResponse.AnsExternalId;
            if (ansID && lastAnswer && ansID === lastAnswer) {
                return chunk.write('selected');
            }
        }
        return chunk.write('');
    };

    /*this helper is for outputing keys*/
    dust.helpers.isProfileInputChecked = function (chunk, ctx, bodies, params) {
        params = params || {};
        var responses = params.responsesObj,
            ansID  = this.tap(params.ansVal, chunk, ctx);
        for (var i = 0; i < responses.length; i++) {
            if (responses[i].QuestionExternalId + '-' + responses[i].AnsExternalId === ansID) {
                return chunk.write('checked');
            }
        }
        return chunk.write('');
    };

    /*this helper is for outputing keys*/
    // AnswerName field is set to "Other" or "None" for answers where other/none behavior is needed
    dust.helpers.profileSpecialAnswer = function (chunk, ctx, bodies, params) {
        var ansType  = this.tap(params.ansType, chunk, ctx);
        if (ansType === 'None' || ansType === 'Other') {
            return chunk.write('js-' + ansType.toLowerCase());
        }
        return chunk.write('');
    };

     /*return correct div (for text styling) on /home */
    dust.helpers.dynamicSchoolOutput = function (chunk, ctx, bodies, params) {
        var CurrentSchoolBrand = this.tap(params.CurrentSchoolBrand, chunk, ctx),
            out = '',
            noLogoCase = this.tap(params.noLogoCase, chunk, ctx),
            schoolCase = this.tap(params.schoolCase, chunk, ctx),
            querystring = Utility.getLocationSearch();
        //checks to see if there is an url parameter, no school specified or school has no logo
        //if none then output the standard ASA text style
        if (Utility.getLocationSearch() && Utility.getLocationSearch().indexOf('Type=') !== -1 && Utility.getParameterByNameFromString('Type', querystring) !== 'All' || !CurrentSchoolBrand || CurrentSchoolBrand === 'nologo') {
            out = noLogoCase;
        }
        else {
            //otherwise output the school overlay style
            out = schoolCase;
        }
        return chunk.write(out);
    };


        /* returns '&s=1' to url if there is already '?' in the url */
    /* otherwise returns '?s=1' */
    dust.helpers.fixSpanishQuerystring  = function (chunk, ctx, bodies, params) {
        var url = this.tap(params.url, chunk, ctx), existingQuerystring = url.indexOf('?') > -1;
        if (existingQuerystring) {
            url += '&s=1';
        } else {
            url += '?s=1';
        }
        return chunk.write(url);
    };

    //We want to output the unknown option if the user has a GradYear of 1900
    dust.helpers.checkGradYear = function (chunk, ctx, bodies, params) {
        var gradDate = this.tap(params.gradDate, chunk, ctx),
            gradYear = gradDate ? gradDate : null;

        gradYear = gradYear ? parseInt(gradYear, 10) : null;

        if (gradYear === 1900) {
            return chunk.write('selected');
        }
        return chunk;
    };

    // check if the tag has been selected for tag tray
    dust.helpers.tagSelectedCheck = function (chunk, ctx, bodies, params) {
        var currentTagFromURL = Utility.getParameterByNameFromString('Tag', Utility.getLocationSearch()),
            tag = this.tap(params.tag, chunk, ctx);
        if (currentTagFromURL && currentTagFromURL === tag) {
            return chunk.write('selected');
        }
        return chunk;
    };

    //Live Chat dust helpers.
    dust.helpers.liveChatAccount = function (chunk, ctx, bodies, params) {
        return chunk.write(Configuration.liveChatProperty.AccountID);
    };
    dust.helpers.liveChatWindow = function (chunk, ctx, bodies, params) {
        var adKey = this.tap(params.adKey, chunk, ctx);
        if (adKey) {
            return chunk.write(Configuration.liveChatProperty.WindowAuthID);
        } else {
            return chunk.write(Configuration.liveChatProperty.WindowUnAuthID);
        }
    };
    dust.helpers.liveChatDepartment = function (chunk, ctx, bodies, params) {
        return chunk.write(Configuration.liveChatProperty.DepartmentID);
    };
     /*return correct image on /home */
    dust.helpers.dynamicBanner = function (chunk, ctx, bodies, params) {
        var CurrentSchool = this.tap(params.CurrentSchool, chunk, ctx),
            CurrentSchoolBrand = this.tap(params.CurrentSchoolBrand, chunk, ctx),
            mediumAvailable = this.tap(params.mediumAvailable, chunk, ctx),
            CMImage = this.tap(params.CMImage, chunk, ctx).replace('src=', 'nosrc=').trim(),
            $cmImage = $(CMImage),
            src = $cmImage.is('img') ? $cmImage.attr('nosrc') : $cmImage.find('img').attr('nosrc'),
            out = '',
            querystring = Utility.getLocationSearch();
        //checks to see if the url parameter has type parameter, or no school specified or school has no logo.
        //if a condition is met, use the reponsive image output
        if ((querystring.indexOf('Type=') !== -1 && Utility.getParameterByNameFromString('Type', querystring) !== 'All') || !CurrentSchoolBrand || CurrentSchoolBrand === 'nologo') {
            out = outputResponsiveImage(src, mediumAvailable);
        }
        //otherwise output the school-provided image
        else {
            $.ajax({ url: '/assets/images/backgrounds/' + CurrentSchoolBrand + '-web.jpg', async: false})
                .done(function () {
                    out = '<img class="responsive-image" src="/assets/images/backgrounds/' + CurrentSchoolBrand + '-web.jpg" alt="' + CurrentSchool + '" title="' + CurrentSchool + '" />';
                }).fail(function () {
                    out = outputResponsiveImage(src, mediumAvailable);
                });
        }
        return chunk.write(out);
    };

    dust.helpers.responsiveImage = function (chunk, ctx, bodies, params) {
        // Replacing "src" attributes with "nosrc" to avoid making the http request until the desired HTML is built
        var imageTag = this.tap(params.imageSource, chunk, ctx).replace('src=', 'nosrc=').trim(),
            altVal = this.tap(params.altVal, chunk, ctx),
            mediumAvailable = this.tap(params.mediumAvailable, chunk, ctx),
            sourceName = $(imageTag).is('img') ? $(imageTag).attr('nosrc') : $(imageTag).find('img').attr('nosrc');
        return chunk.write(outputResponsiveImage(sourceName, mediumAvailable, altVal));
    };
    function outputResponsiveImage(sourceName, mediumAvailable, altVal) {
        //var stringToReturn = sourceName;
        if (sourceName) {
            var lastIndexOfSlash = sourceName.lastIndexOf('/'),
                lastIndexOfDot = sourceName.lastIndexOf('.'),
                filePath = sourceName.substring(0, lastIndexOfDot),
                ext = sourceName.substring(lastIndexOfDot),
                // if medium is not available, use the large image
                mediumDefault = mediumAvailable ? '-medium' : '';
            if (altVal) {
                altVal = altVal.replace('<h1>', '').replace('</h1>', '').trim();
                sourceName = '<picture><source media="(min-width: 1026px)" srcset="' + filePath + ext +
                '"><source media="(min-width: 480px)" srcset="' + filePath + mediumDefault + ext +
                '"><img class="responsive-image" src="' + filePath + '-small' + ext +
                '" alt="' + altVal + '" title="' + altVal + '"></picture>';
            } else {
                sourceName = '<picture><source media="(min-width: 1026px)" srcset="' + filePath + ext +
                '"><source media="(min-width: 480px)" srcset="' + filePath + mediumDefault + ext +
                '"><img class="responsive-image" src="' + filePath + '-small' + ext +
                '"></picture>';
            }
        }
        return sourceName;
    }
    dust.helpers.sizeImage = function (chunk, ctx, bodies, params) {
        // Replacing "src" attributes with "nosrc" to avoid making the http request until the desired HTML is built
        var imageTag = this.tap(params.imageSource, chunk, ctx).replace('src=', 'nosrc=').trim(),
            sizeWanted = this.tap(params.size, chunk, ctx),
            sourceName = '',
            $imgTag = $(imageTag), 
            size = sizeWanted === 'small' ? '-small' : '';
        sourceName = $imgTag.is('img') ? $imgTag.attr('nosrc') : $imgTag.find('img').attr('nosrc');

        if (!sourceName) {
            sourceName = '';
        }
        altVal = $imgTag.is('img') ? $imgTag.attr('alt') : $imgTag.find('img').attr('alt');
        var lastIndexOfSlash = sourceName.lastIndexOf('/'),
            lastIndexOfDot = sourceName.lastIndexOf('.'),
            filePath = sourceName.substring(0, lastIndexOfDot),
            ext = sourceName.substring(lastIndexOfDot);
        if (altVal) {
            altVal = altVal.replace('<h1>', '').replace('</h1>', '').trim();
            sourceName = '<img src="' + filePath + size + ext + '" alt="' + altVal + '">';
        } else {
            sourceName = '<img src="' + filePath + size + ext + '">';
        }

        return chunk.write(sourceName);
    };
    dust.helpers.profileOtherInputVisibilty = function (chunk, ctx, bodies, params) {
        var responses = params.responsesObject,
            otherAnswer = _.filter(responses, function (response) {
                return response.AnsName === 'Other';
            });
        if (otherAnswer.length) {
            return chunk.write('');
        }
        return chunk.write('hide');
    };

    //This helper is used both client side and in the nodejs layer, do not modify one without checking the other
    dust.helpers.postedDateFormatter = function (chunk, ctx, bodies, params) {
        var dateObj = new Date(this.tap(params.date, chunk, ctx)),
            lng = this.tap(params.lng, chunk, ctx),
            monthNames = ['January', 'February', 'March', 'April', 'May', 'June', 'July', 'August', 'September', 'October', 'November', 'December'],
            nombres = ['Enero', 'Febrero', 'Marzo', 'Abril', 'Mayo', 'Junio', 'Julio', 'Agosto', 'Septiembre', 'Octubre', 'Noviembre', 'Diciembre'],
            month = monthNames[dateObj.getMonth()],
            //CM1 passes version number as a string thats the first element of an array, e.g. ["1"]
            version = parseInt(this.tap(params.version, chunk, ctx)[0], 10),
            //'Published' shoud be english or spanish depending on paramter
            publishOrUpdated = (lng === 'spanish') ? 'Publicado: ' : 'Published: ';

        //Determine if the piece of content is newly published or has been updated
        //Look at version number, if not 1, its updated, adjust text accordingly
        if (version !== 1) {
            if (lng === 'spanish') {
                publishOrUpdated = 'Actualizado: ';
            } else {
                publishOrUpdated = 'Updated: ';
            }
        }
            //Output date formatted like "June 5, 2014"
            //or Febrero 29, 1996
        if  (lng === 'spanish') {
            return chunk.write(publishOrUpdated + nombres[monthNames.indexOf(month)] + ' ' + dateObj.getDate() + ', ' + dateObj.getFullYear());
        }
        return chunk.write(publishOrUpdated + month + ' ' + dateObj.getDate() + ', ' + dateObj.getFullYear());
    };

    //This helper is used both client side and in the nodejs layer, do not modify one without checking the other
    dust.helpers.getCurrentYear = function (chunk, ctx, bodies, params) {
        return chunk.write(new Date().getFullYear());
    };

    dust.helpers.contentLength = function (chunk, ctx, bodies, params) {
        var phrase = this.tap(params.phrase, chunk, ctx);
        if (phrase.length > 31) {
            return chunk.write('large');
        }
        return chunk.write('');
    };

    dust.helpers.searchResultsHeader = function (chunk, ctx, bodies, params) {
        var originalSearchTerm = this.tap(params.origTerm, chunk, ctx),
            numRecords = this.tap(params.numRecords, chunk, ctx),
            resultsHeading = '';

        if (params.termObject) {
            var termObject = this.tap(params.termObject, chunk, ctx),
                spellCorrected = false,
                autoPhrased = false,
                dymTerm = '';

            _.each(_.keys(termObject), function (term) {
                spellCorrected = termObject[term][0].spellCorrected;
                autoPhrased = termObject[term][0].autoPhrased;
                dymTerm = termObject[term][0].adjustedTerms.replace(/"/g, '');
            });

            if (spellCorrected || (autoPhrased && dymTerm.toLowerCase() !== originalSearchTerm.toLowerCase())) {
                resultsHeading = 'Your search:<br> ' + originalSearchTerm + '</br>';
                resultsHeading += 'Showing results for:<br> <strong>' + dymTerm + '</strong> (' + numRecords + ')';
                return chunk.write(resultsHeading);
            }
        }

        //if the originalSearchTerm was empty, the origTerm param resolves to "false" and we don't want
        //to display that to the user.
        if (!originalSearchTerm) {
            originalSearchTerm = '';
        }

        resultsHeading = 'Your search:<br/> <strong>' + originalSearchTerm + '</strong> (' + numRecords + ')';

        return chunk.write(resultsHeading);
    };

    dust.helpers.SALTTour = function (chunk, ctx, bodies, params) {
        var saltTourPresent = 0,
            mainContentArray = this.tap(params.mainContent, chunk, ctx);

        for (var i = 0, tot = mainContentArray.length; i < tot; i++) {
            if (mainContentArray[i].name === 'PromoTile') {
                for (var x = 0, objectNum = mainContentArray[i].records.length; x < objectNum; x++) {
                    if (mainContentArray[i].records[x].attributes.page_title[0] === 'Salt tour') {
                        saltTourPresent++;
                    }
                }
            }
        }

        //check our counter to see if salttour was one of the records
        if (saltTourPresent === 0) {
            chunk.render(bodies.block, ctx);
        }

        return chunk;
    };

    function getAdjustedTerm(misspelledTerm) {
        var dymTerm = '';

        _.each(_.keys(misspelledTerm), function (term) {
            dymTerm = misspelledTerm[term][0].adjustedTerms;
        });

        return dymTerm;
    }

    dust.helpers.returnAnswersForQuestion = function (chunk, ctx, bodies, params) {
        var questionId = this.tap(params.questionId, chunk, ctx),
            answersArray = this.tap(params.answersArray, chunk, ctx),
            toReturn = '';
        if (answersArray && questionId) {
            _.each(answersArray[questionId - 1], function (element, index, list) {
                toReturn += '<option value="' + element.id + '">' + element.description + '</option>';
            });
        }
        return chunk.write(toReturn);
    };

    dust.helpers.returnOrgLogos = function (chunk, ctx, bodies, params) {

        var member = ctx.get("SiteMember");

        var logoOrgs = [];

        if (member !== undefined) {
            var orgs = ctx.get("SiteMember").Organizations;

            logoOrgs = Utility.returnBrandedOrganizations(orgs);
        } else {
            config = ctx.get("Configuration");
            if (config.CurrentSchoolBrand !== "nologo") {
                var org;
                org.OrgnizationName = config.CurrentSchool;
                org.Brand = config.CurrentSchoolBrand;
                logoOrgs = [org];
            }

        }

        var length = logoOrgs.length;

        _.each(logoOrgs, function (element, index, list) {
            element.orgIndex = index;
            element.orgLength = length;
            chunk.render(bodies.block, ctx.push(element));
        });
        return chunk;
    };
});

/**
*  Ajax Autocomplete for jQuery, version 1.2.9
*  (c) 2013 Tomas Kirda
*
*  Ajax Autocomplete for jQuery is freely distributable under the terms of an MIT-style license.
*  For details, see the web site: https://github.com/devbridge/jQuery-Autocomplete
*
*/

/*jslint  browser: true, white: true, plusplus: true */
/*global define, window, document, jQuery */

/*ASA modifications at line 157 and 219*/

// Expose plugin as an AMD module if AMD loader is present:
(function (factory) {
    
    if (typeof define === 'function' && define.amd) {
        // AMD. Register as an anonymous module.
        define('jquery.autocomplete',['jquery'], factory);
    } else {
        // Browser globals
        factory(jQuery);
    }
}(function ($) {
    

    var
        utils = (function () {
            return {
                escapeRegExChars: function (value) {
                    return value.replace(/[\-\[\]\/\{\}\(\)\*\+\?\.\\\^\$\|]/g, "\\$&");
                },
                createNode: function (containerClass) {
                    var div = document.createElement('div');
                    div.className = containerClass;
                    div.style.position = 'absolute';
                    div.style.display = 'none';
                    return div;
                }
            };
        }()),

        keys = {
            ESC: 27,
            TAB: 9,
            RETURN: 13,
            LEFT: 37,
            UP: 38,
            RIGHT: 39,
            DOWN: 40
        };

    function Autocomplete(el, options) {
        var noop = function () { },
            that = this,
            defaults = {
                autoSelectFirst: false,
                appendTo: document.body,
                serviceUrl: null,
                lookup: null,
                onSelect: null,
                minChars: 1,
                maxHeight: '',
                deferRequestBy: 0,
                params: {},
                formatResult: Autocomplete.formatResult,
                delimiter: null,
                zIndex: 9999,
                type: 'GET',
                noCache: false,
                onSearchStart: noop,
                onSearchComplete: noop,
                onSearchError: noop,
                containerClass: '',
                tabDisabled: false,
                dataType: 'text',
                currentRequest: null,
                triggerSelectOnValidInput: true,
                preventBadQueries: true,
                lookupFilter: function (suggestion, originalQuery, queryLowerCase) {
                    return suggestion.value.toLowerCase().indexOf(queryLowerCase) !== -1;
                },
                paramName: 'query',
                transformResult: function (response) {
                    return typeof response === 'string' ? $.parseJSON(response) : response;
                },
                showNoSuggestionNotice: false,
                noSuggestionNotice: 'No results',
                orientation: 'bottom',
                forceFixPosition: false
            };

        // Shared variables:
        that.element = el;
        that.el = $(el);
        that.suggestions = [];
        that.badQueries = [];
        that.selectedIndex = -1;
        that.currentValue = that.element.value;
        that.intervalId = 0;
        that.cachedResponse = {};
        that.onChangeInterval = null;
        that.onChange = null;
        that.isLocal = false;
        that.suggestionsContainer = null;
        that.noSuggestionsContainer = null;
        that.options = $.extend({}, defaults, options);
        that.classes = {
            selected: 'autocomplete-selected',
            suggestion: 'autocomplete-suggestion'
        };
        that.hint = null;
        that.hintValue = '';
        // Fork: changing the default value from null to 'no-selection-made' to fire onInvalidSelection function even if a selection hasn't been made yet
        that.selection = 'no-selection-made';

        // Initialize and set options:
        that.initialize();
        that.setOptions(options);
    }

    Autocomplete.utils = utils;

    $.Autocomplete = Autocomplete;

    Autocomplete.formatResult = function (suggestion, currentValue) {
        var pattern = '(' + utils.escapeRegExChars(currentValue) + ')';
        var formattedSuggestion = suggestion.value.replace(new RegExp(pattern, 'gi'), '<strong>$1<\/strong>');

        if (suggestion.type && suggestion.type !== 'definition') {
            formattedSuggestion += '<div class="autocomplete-suggestion-type">' + 'in ' + suggestion.type + '</div>';
        }
        return formattedSuggestion;
    };

    Autocomplete.prototype = {

        killerFn: null,

        initialize: function () {
            var that = this,
                suggestionSelector = '.' + that.classes.suggestion,
                selected = that.classes.selected,
                options = that.options,
                container,
                noSuggestionsContainer;

            // Remove autocomplete attribute to prevent native suggestions:
            that.element.setAttribute('autocomplete', 'off');

            that.killerFn = function (e) {
                //the check that it used to have "$(e.target).closest('.' + that.options.containerClass).length === 0" doesn't make any sense.
                //it always returns true. so the function killSuggestions() will be excecuted all the time.
                //replace the previous check with the one below.
                //if the target element is not "that", that's the focusout behavior, we want to excecute killSuggestions() for "that" element in this case and in this case only.
                if ($(that.element).attr('id') !== $(e.target).attr('id')) {
                    that.killSuggestions();
                    that.disableKillerFn();
                }
            };

            // html() deals with many types: htmlString or Element or Array or jQuery
            that.noSuggestionsContainer = $('<div class="autocomplete-no-suggestion"></div>')
                                          .html(this.options.noSuggestionNotice).get(0);

            that.suggestionsContainer = Autocomplete.utils.createNode(options.containerClass);

            container = $(that.suggestionsContainer);

            container.appendTo(options.appendTo);

            // Only set width if it was provided:
            if (options.width !== 'auto') {
                container.width(options.width);
            }

            // Listen for mouse over event on suggestions list:
            container.on('mouseover.autocomplete', suggestionSelector, function () {
                that.activate($(this).data('index'));
            });

            // Deselect active element when mouse leaves suggestions container:
            container.on('mouseout.autocomplete', function () {
                that.selectedIndex = -1;
                container.children('.' + selected).removeClass(selected);
            });

            // Listen for click event on suggestions list:
            container.on('click.autocomplete', suggestionSelector, function () {
                that.select($(this).data('index'));
            });

            that.fixPositionCapture = function () {
                if (that.visible) {
                    that.fixPosition();
                }
            };

            $(window).on('resize.autocomplete', that.fixPositionCapture);

            that.el.on('keydown.autocomplete', function (e) { that.onKeyPress(e); });
            that.el.on('keyup.autocomplete', function (e) { that.onKeyUp(e); });
            that.el.on('blur.autocomplete', function () { that.onBlur(); });
            that.el.on('focus.autocomplete', function () { that.onFocus(); });
            that.el.on('change.autocomplete', function (e) { that.onKeyUp(e); });
        },

        onFocus: function () {
            var that = this;
            that.fixPosition();
            if (that.options.minChars <= that.el.val().length) {
                that.onValueChange();
            }
        },

        onBlur: function () {
            //added check for this.options.selection !== 'refocused-org' to account for user going back to an already selected org via keyboard instead of mouseclicks. For 508
            if (!this.selection || this.selection.value !== this.currentValue && this.lastSelectedIndex === -1 && this.options.selection !== 'refocused-org') {
                this.selection = null;
                (this.options.onInvalidateSelection || $.noop).call(this.element);
            }
            //reset override
            if (this.options.selection === 'refocused-org') {
                this.options.selection = '';
            }
            this.enableKillerFn();
        },

        setOptions: function (suppliedOptions) {
            var that = this,
                options = that.options;

            $.extend(options, suppliedOptions);

            that.isLocal = $.isArray(options.lookup);

            if (that.isLocal) {
                options.lookup = that.verifySuggestionsFormat(options.lookup);
            }

            options.orientation = that.validateOrientation(options.orientation, 'bottom');

            // Adjust height, width and z-index:
            $(that.suggestionsContainer).css({
                'max-height': options.maxHeight + 'px',
                'z-index': options.zIndex
            });
        },


        clearCache: function () {
            this.cachedResponse = {};
            this.badQueries = [];
        },

        clear: function () {
            this.clearCache();
            this.currentValue = '';
            this.suggestions = [];
        },

        disable: function () {
            var that = this;
            that.disabled = true;
            if (that.currentRequest) {
                that.currentRequest.abort();
            }
        },

        enable: function () {
            this.disabled = false;
        },

        fixPosition: function () {
            // Use only when container has already its content

            var that = this,
                $container = $(that.suggestionsContainer),
                containerParent = $container.parent().get(0);
            // Fix position automatically when appended to body.
            // In other cases force parameter must be given.
            if (containerParent !== document.body && !that.options.forceFixPosition)
                return;

            // Choose orientation
            var orientation = that.options.orientation,
                containerHeight = $container.outerHeight(),
                height = that.el.outerHeight(),
                offset = that.el.offset(),
                styles = { 'top': offset.top, 'left': offset.left };

            if (orientation == 'auto') {
                var viewPortHeight = $(window).height(),
                    scrollTop = $(window).scrollTop(),
                    top_overflow = -scrollTop + offset.top - containerHeight,
                    bottom_overflow = scrollTop + viewPortHeight - (offset.top + height + containerHeight);

                if (Math.max(top_overflow, bottom_overflow) === top_overflow)
                    orientation = 'top';
                else
                    orientation = 'bottom';
            }

            if (orientation === 'top')
                styles.top += -containerHeight;
            else
                styles.top += height;

            // If container is not positioned to body,
            // correct its position using offset parent offset
            if(containerParent !== document.body) {
                var opacity = $container.css('opacity'),
                    parentOffsetDiff;
                if (!that.visible)
                    $container.css('opacity', 0).show();

                parentOffsetDiff = $container.offsetParent().offset();
                styles.top -= parentOffsetDiff.top;
                styles.left -= parentOffsetDiff.left;

                if (!that.visible)
                    $container.css('opacity', opacity).hide();
            }

            // -2px to account for suggestions border.
            if (that.options.width === 'auto') {
                styles.width = (that.el.outerWidth() - 2) + 'px';
            }

            $container.css(styles);
        },

        enableKillerFn: function () {
            var that = this;
            $(document).on('click.autocomplete', that.killerFn);
        },

        disableKillerFn: function () {
            var that = this;
            $(document).off('click.autocomplete', that.killerFn);
        },

        killSuggestions: function () {
            var that = this;
            that.stopKillSuggestions();
            that.intervalId = window.setInterval(function () {
                that.hide();
                that.stopKillSuggestions();
            }, 50);
        },

        stopKillSuggestions: function () {
            window.clearInterval(this.intervalId);
        },

        isCursorAtEnd: function () {
            var that = this,
                valLength = that.el.val().length,
                selectionStart = that.element.selectionStart,
                range;

            if (typeof selectionStart === 'number') {
                return selectionStart === valLength;
            }
            if (document.selection) {
                range = document.selection.createRange();
                range.moveStart('character', -valLength);
                return valLength === range.text.length;
            }
            return true;
        },

        onKeyPress: function (e) {
            var that = this;

            // If suggestions are hidden and user presses arrow down, display suggestions:
            if (!that.disabled && !that.visible && e.which === keys.DOWN && that.currentValue) {
                that.suggest();
                return;
            }

            if (that.disabled || !that.visible) {
                return;
            }

            switch (e.which) {
                case keys.ESC:
                    that.el.val(that.currentValue);
                    that.hide();
                    break;
                case keys.RIGHT:
                    if (that.hint && that.options.onHint && that.isCursorAtEnd()) {
                        that.selectHint();
                        break;
                    }
                    return;
                case keys.TAB:
                    if (that.hint && that.options.onHint) {
                        that.selectHint();
                        return;
                    }
                    // Fall through to RETURN
                case keys.RETURN:
                    if (that.selectedIndex === -1) {
                        that.hide();
                        return;
                    }
                    that.select(that.selectedIndex);
                    if (e.which === keys.TAB && that.options.tabDisabled === false) {
                        return;
                    }
                    break;
                case keys.UP:
                    that.moveUp();
                    break;
                case keys.DOWN:
                    that.moveDown();
                    break;
                default:
                    return;
            }

            // Cancel event if function did not return:
            e.stopImmediatePropagation();
            e.preventDefault();
        },

        onKeyUp: function (e) {
            var that = this;
            that.lastSelectedIndex = that.selectedIndex;
            if (that.disabled) {
                return;
            }

            switch (e.which) {
                case keys.UP:
                case keys.DOWN:
                    return;
            }

            clearInterval(that.onChangeInterval);

            if (that.currentValue !== that.el.val()) {
                that.findBestHint();
                if (that.options.deferRequestBy > 0) {
                    // Defer lookup in case when value changes very quickly:
                    that.onChangeInterval = setInterval(function () {
                        that.onValueChange();
                    }, that.options.deferRequestBy);
                } else {
                    that.onValueChange();
                }
            }
        },

        onValueChange: function () {
            var that = this,
                options = that.options,
                value = that.el.val(),
                query = that.getQuery(value),
                index;

            clearInterval(that.onChangeInterval);
            that.currentValue = value;
            that.selectedIndex = -1;

            // Check existing suggestion for the match before proceeding:
            if (options.triggerSelectOnValidInput) {
                index = that.findSuggestionIndex(query);
                if (index !== -1) {
                    that.select(index);
                    return;
                }
            }

            if (query.length < options.minChars) {
                that.hide();
            } else {
                that.getSuggestions(query);
            }
        },

        findSuggestionIndex: function (query) {
            var that = this,
                index = -1,
                queryLowerCase = query.toLowerCase();

            $.each(that.suggestions, function (i, suggestion) {
                if (suggestion.value.toLowerCase() === queryLowerCase) {
                    index = i;
                    return false;
                }
            });

            return index;
        },

        getQuery: function (value) {
            var delimiter = this.options.delimiter,
                parts;

            if (!delimiter) {
                return value;
            }
            parts = value.split(delimiter);
            return $.trim(parts[parts.length - 1]);
        },

        getSuggestionsLocal: function (query) {
            var that = this,
                options = that.options,
                queryLowerCase = query.toLowerCase(),
                filter = options.lookupFilter,
                limit = parseInt(options.lookupLimit, 10),
                data;

            data = {
                suggestions: $.grep(options.lookup, function (suggestion) {
                    return filter(suggestion, query, queryLowerCase);
                })
            };

            if (limit && data.suggestions.length > limit) {
                data.suggestions = data.suggestions.slice(0, limit);
            }

            return data;
        },

        getSuggestions: function (q) {
            var response,
                that = this,
                options = that.options,
                serviceUrl = options.serviceUrl,
                params,
                cacheKey;

            options.params[options.paramName] = q;
            params = options.ignoreParams ? null : options.params;

            if (that.isLocal) {
                response = that.getSuggestionsLocal(q);
            } else {
                if ($.isFunction(serviceUrl)) {
                    serviceUrl = serviceUrl.call(that.element, q);
                }
                cacheKey = serviceUrl + '?' + $.param(params || {});
                response = that.cachedResponse[cacheKey];
            }

            if (response && $.isArray(response.suggestions)) {
                that.suggestions = response.suggestions;
                that.suggest();
            } else if (!that.isBadQuery(q)) {
                if (options.onSearchStart.call(that.element, options.params) === false) {
                    return;
                }
                if (that.currentRequest) {
                    that.currentRequest.abort();
                }
                that.currentRequest = $.ajax({
                    url: serviceUrl,
                    data: params,
                    type: options.type,
                    dataType: options.dataType
                }).done(function (data) {
                    var result;
                    that.currentRequest = null;
                    result = options.transformResult(data);
                    that.processResponse(result, q, cacheKey);
                    options.onSearchComplete.call(that.element, q, result.suggestions);
                }).fail(function (jqXHR, textStatus, errorThrown) {
                    options.onSearchError.call(that.element, q, jqXHR, textStatus, errorThrown);
                });
            }
        },

        isBadQuery: function (q) {
            if (!this.options.preventBadQueries){
                return false;
            }

            var badQueries = this.badQueries,
                i = badQueries.length;

            while (i--) {
                if (q.indexOf(badQueries[i]) === 0) {
                    return true;
                }
            }

            return false;
        },

        hide: function () {
            var that = this;
            that.visible = false;
            that.selectedIndex = -1;
            $(that.suggestionsContainer).hide();
            that.signalHint(null);
        },

        suggest: function () {
            if (this.suggestions.length === 0) {
                this.options.showNoSuggestionNotice ? this.noSuggestions() : this.hide();               
                return;
            }

            var that = this,
                options = that.options,
                formatResult = options.formatResult,
                value = that.getQuery(that.currentValue),
                className = that.classes.suggestion,
                classSelected = that.classes.selected,
                container = $(that.suggestionsContainer),
                noSuggestionsContainer = $(that.noSuggestionsContainer),
                beforeRender = options.beforeRender,
                html = '',
                index,
                width;

            if (options.triggerSelectOnValidInput) {
                index = that.findSuggestionIndex(value);
                if (index !== -1) {
                    that.select(index);
                    return;
                }
            }

            // Build suggestions inner HTML:
            $.each(that.suggestions, function (i, suggestion) {
                if (suggestion.type === "courses") {
                    className += ' js-salt-courses-link" data-coursepath="' + suggestion.recState;
                }

                html += '<div class="' + className + '" data-index="' + i + '">' + formatResult(suggestion, value) + '</div>';
            
                className = that.classes.suggestion;
            });

            this.adjustContainerWidth();      

            noSuggestionsContainer.detach();
            container.html(html);

            // Select first value by default:
            if (options.autoSelectFirst) {
                that.selectedIndex = 0;
                container.children().first().addClass(classSelected);
            }

            if ($.isFunction(beforeRender)) {
                beforeRender.call(that.element, container);
            }

            that.fixPosition();

            container.show();
            that.visible = true;

            that.findBestHint();
        },

        noSuggestions: function() {
             var that = this,
                 container = $(that.suggestionsContainer),
                 noSuggestionsContainer = $(that.noSuggestionsContainer);

            this.adjustContainerWidth();

            // Some explicit steps. Be careful here as it easy to get
            // noSuggestionsContainer removed from DOM if not detached properly.
            noSuggestionsContainer.detach();
            container.empty(); // clean suggestions if any
            container.append(noSuggestionsContainer);

            that.fixPosition();

            container.show();
            that.visible = true;
        },

        adjustContainerWidth: function() {
            var that = this,
                options = that.options,
                width,
                container = $(that.suggestionsContainer);

            // If width is auto, adjust width before displaying suggestions,
            // because if instance was created before input had width, it will be zero.
            // Also it adjusts if input width has changed.
            // -2px to account for suggestions border.
            if (options.width === 'auto') {
                width = that.el.outerWidth() - 2;
                container.width(width > 0 ? width : 300);
            }
        },

        findBestHint: function () {
            var that = this,
                value = that.el.val().toLowerCase(),
                bestMatch = null;

            if (!value) {
                return;
            }

            $.each(that.suggestions, function (i, suggestion) {
                var foundMatch = suggestion.value.toLowerCase().indexOf(value) === 0;
                if (foundMatch) {
                    bestMatch = suggestion;
                }
                return !foundMatch;
            });

            that.signalHint(bestMatch);
        },

        signalHint: function (suggestion) {
            var hintValue = '',
                that = this;
            if (suggestion) {
                hintValue = that.currentValue + suggestion.value.substr(that.currentValue.length);
            }
            if (that.hintValue !== hintValue) {
                that.hintValue = hintValue;
                that.hint = suggestion;
                (this.options.onHint || $.noop)(hintValue);
            }
        },

        verifySuggestionsFormat: function (suggestions) {
            // If suggestions is string array, convert them to supported format:
            if (suggestions.length && typeof suggestions[0] === 'string') {
                return $.map(suggestions, function (value) {
                    return { value: value, data: null };
                });
            }

            return suggestions;
        },

        validateOrientation: function(orientation, fallback) {
            orientation = orientation.trim().toLowerCase();
            if(['auto', 'bottom', 'top'].indexOf(orientation) == '-1')
                orientation = fallback;
            return orientation
        },

        processResponse: function (result, originalQuery, cacheKey) {
            var that = this,
                options = that.options;

            result.suggestions = that.verifySuggestionsFormat(result.suggestions);

            // Cache results if cache is not disabled:
            if (!options.noCache) {
                that.cachedResponse[cacheKey] = result;
                if (options.preventBadQueries && result.suggestions.length === 0) {
                    that.badQueries.push(originalQuery);
                }
            }

            // Return if originalQuery is not matching current query:
            if (originalQuery !== that.getQuery(that.currentValue)) {
                return;
            }

            that.suggestions = result.suggestions;
            that.suggest();
        },

        activate: function (index) {
            var that = this,
                activeItem,
                selected = that.classes.selected,
                container = $(that.suggestionsContainer),
                children = container.children();

            container.children('.' + selected).removeClass(selected);

            that.selectedIndex = index;

            if (that.selectedIndex !== -1 && children.length > that.selectedIndex) {
                activeItem = children.get(that.selectedIndex);
                $(activeItem).addClass(selected);
                return activeItem;
            }

            return null;
        },

        selectHint: function () {
            var that = this,
                i = $.inArray(that.hint, that.suggestions);

            that.select(i);
        },

        select: function (i) {
            var that = this;
            that.hide();
            that.onSelect(i);
        },

        moveUp: function () {
            var that = this;

            if (that.selectedIndex === -1) {
                return;
            }

            if (that.selectedIndex === 0) {
                $(that.suggestionsContainer).children().first().removeClass(that.classes.selected);
                that.selectedIndex = -1;
                that.el.val(that.currentValue);
                that.findBestHint();
                return;
            }

            that.adjustScroll(that.selectedIndex - 1);
        },

        moveDown: function () {
            var that = this;

            if (that.selectedIndex === (that.suggestions.length - 1)) {
                return;
            }

            that.adjustScroll(that.selectedIndex + 1);
        },

        adjustScroll: function (index) {
            var that = this,
                activeItem = that.activate(index),
                offsetTop,
                upperBound,
                lowerBound,
                heightDelta = 25;

            if (!activeItem) {
                return;
            }

            offsetTop = activeItem.offsetTop;
            upperBound = $(that.suggestionsContainer).scrollTop();
            lowerBound = upperBound + that.options.maxHeight - heightDelta;

            if (offsetTop < upperBound) {
                $(that.suggestionsContainer).scrollTop(offsetTop);
            } else if (offsetTop > lowerBound) {
                $(that.suggestionsContainer).scrollTop(offsetTop - that.options.maxHeight + heightDelta);
            }

            that.el.val(that.getValue(that.suggestions[index].value));
            that.signalHint(null);
        },

        onSelect: function (index) {
            var that = this,
                onSelectCallback = that.options.onSelect,
                suggestion = that.suggestions[index];

            that.currentValue = that.getValue(suggestion.value);

            if (that.currentValue !== that.el.val()) {
                that.el.val(that.currentValue);
            }

            that.signalHint(null);
            that.suggestions = [];
            that.selection = suggestion;

            if ($.isFunction(onSelectCallback)) {
                onSelectCallback.call(that.element, suggestion);
            }
        },

        getValue: function (value) {
            var that = this,
                delimiter = that.options.delimiter,
                currentValue,
                parts;

            if (!delimiter) {
                return value;
            }

            currentValue = that.currentValue;
            parts = currentValue.split(delimiter);

            if (parts.length === 1) {
                return value;
            }

            return currentValue.substr(0, currentValue.length - parts[parts.length - 1].length) + value;
        },

        dispose: function () {
            var that = this;
            that.el.off('.autocomplete').removeData('autocomplete');
            that.disableKillerFn();
            $(window).off('resize.autocomplete', that.fixPositionCapture);
            $(that.suggestionsContainer).remove();
        }
    };

    // Create chainable jQuery plugin:
    $.fn.autocomplete = function (options, args) {  
        var dataKey = 'autocomplete';
        // If function invoked without argument return
        // instance of the first matched element:
        if (arguments.length === 0) {
            return this.first().data(dataKey);
        }

        return this.each(function () {
            var inputElement = $(this),
                instance = inputElement.data(dataKey);

            if (typeof options === 'string') {
                if (instance && typeof instance[options] === 'function') {
                    instance[options](args);
                }
            } else {
                // If instance already exists, destroy it:
                if (instance && instance.dispose) {
                    instance.dispose();
                }
                instance = new Autocomplete(this, options);
                inputElement.data(dataKey, instance);
            }
        });
    };
}));

require([
    'jquery',
    'configuration',
    'underscore',
    'asa/ASAUtilities',
    'salt/models/SiteMember',
    'modules/ReportingStatus',
    'salt',
    'jquery.autocomplete'
], function ($, configuration, _, Utility, SiteMember, reportingStatus, SALT) {

    SiteMember.done(function (siteMember) {
        //Only initiate the logic when the search is enabled
        if (!siteMember.SearchDisabled) {
            var url = Utility.setUrlToHideRecords(configuration.apiEndpointBases.GenericEndeca + 'predictiveSearch', siteMember, false),
                showReportIdQuestion = reportingStatus.checkReportIdStatus(siteMember);

            SALT.on('search:opened:firsttime', function () {
                $.getJSON(url, function (response) {
                    var suggestions = [];

                    _.each(response.secondaryContent[0].records, function (record) {
                        if (Utility.checkNested(record, 'attributes.term[0]')) {
                            var term = record.attributes.term[0].trim();
                            if (Utility.checkNested(record, 'attributes.acronym[0]') && record.attributes.acronym[0].trim()) {
                                suggestions.push({value: term + ' ' + record.attributes.acronym[0].trim(), data: term, type: ''});
                            } else {
                                suggestions.push({value: term, type: ''});
                            }
                        }
                    });

                    _.each(response.mainContent[0].records, function (record) {
                        if (Utility.checkNested(record, 'attributes.headline[0]')) {

                            var contentType,
                                detailRecordState = '';
                            if (Utility.checkNested(record, 'attributes.ContentTypes[0]')) {
                                contentType = record.attributes.ContentTypes[0].trim().toLowerCase() + 's';
                                if (contentType === 'tools' || contentType === 'apps') {
                                    if (record.attributes['iframe-url'][0].indexOf('calcxml') > -1) {
                                        record.detailsAction.recordState = '/calculators/' + record.attributes.sys_title[0].trim();
                                    }
                                    contentType = 'tools';
                                }
                                if (contentType === 'courses') {
                                    record.detailsAction.recordState = configuration.mm101.url + record.attributes['external-url'][0];
                                }
                            }

                            if (Utility.checkNested(record, 'detailsAction.recordState')) {
                                detailRecordState = record.detailsAction.recordState;
                            }

                            //Need to wrap headline in a div in case there are values without html elements in the text
                            var headline = '<div>' + record.attributes.headline[0].trim() + '</div>';
                            var headlineNoHtml = $(headline).text();

                            suggestions.push({value: headlineNoHtml, type: contentType, recState: detailRecordState});

                            if (Utility.checkNested(record, 'attributes.FlatTags[0]')) {
                                var tagArray = record.attributes.FlatTags[0].trim();
                                var trimmedTag, tagCase;
                                if (tagArray) {
                                    tagArray.split('|').forEach(function (tag) {
                                        suggestions.push({value: Utility.toTitleCase(tag.trim()), type: ''});
                                    });
                                }
                            }
                        }
                    });

                    var uniqueList = _.uniq(suggestions, JSON.stringify);
                    var sortedList = _.sortBy(uniqueList, 'type');

                    $(function () {
                        $('#searchCriteria, #searchCriteria-mobile').autocomplete({
                            minChars: 3,
                            lookupLimit: 10,
                            lookup: sortedList,
                            triggerSelectOnValidInput: false,
                            maxHeight: 530,
                            containerClass: 'autocomplete-suggestions',
                            onSelect: function (suggestion) {
                                // if suggestion has a record state, go directly to that piece of content
                                if (suggestion.recState) {
                                    if (suggestion.recState.indexOf('/calculators/') > -1) {
                                        $(location).attr('href', suggestion.recState);
                                    } else if (suggestion.type === 'courses') {
                                        if (showReportIdQuestion) {
                                            $(location).attr('href', '#IDFrm');
                                            $(location).attr('target', '');
                                        }
                                    } else if (suggestion.recState.indexOf('/Lesson/my-money101-landing') > -1) {
                                        $(location).attr('href', '/MM101/');
                                    } else if (suggestion.recState.indexOf('/Tool/saveup-tool') > -1) {
                                        $(location).attr('href', '/landing/Saveup');
                                    } else {
                                        $(location).attr('href', '/content/media' + suggestion.recState);
                                    }
                                } else {
                                    // data attribute is only available for terms with acronyms. use it as the search value to return the appropriate definition in the search page.
                                    this.value = suggestion.data || this.value;
                                    $(this).closest('form').submit();
                                }
                            }
                        });
                    });
                });
            });
        }
    });
});

define("modules/SearchAutoComplete", function(){});

/**
 * @preserve FastClick: polyfill to remove click delays on browsers with touch UIs.
 *
 * @version 1.0.2
 * @codingstandard ftlabs-jsv2
 * @copyright The Financial Times Limited [All Rights Reserved]
 * @license MIT License (see LICENSE.txt)
 */
function FastClick(a,b){function c(a,b){return function(){return a.apply(b,arguments)}}var d;if(b=b||{},this.trackingClick=!1,this.trackingClickStart=0,this.targetElement=null,this.touchStartX=0,this.touchStartY=0,this.lastTouchIdentifier=0,this.touchBoundary=b.touchBoundary||10,this.layer=a,this.tapDelay=b.tapDelay||200,!FastClick.notNeeded(a)){for(var e=["onMouse","onClick","onTouchStart","onTouchMove","onTouchEnd","onTouchCancel"],f=this,g=0,h=e.length;h>g;g++)f[e[g]]=c(f[e[g]],f);deviceIsAndroid&&(a.addEventListener("mouseover",this.onMouse,!0),a.addEventListener("mousedown",this.onMouse,!0),a.addEventListener("mouseup",this.onMouse,!0)),a.addEventListener("click",this.onClick,!0),a.addEventListener("touchstart",this.onTouchStart,!1),a.addEventListener("touchmove",this.onTouchMove,!1),a.addEventListener("touchend",this.onTouchEnd,!1),a.addEventListener("touchcancel",this.onTouchCancel,!1),Event.prototype.stopImmediatePropagation||(a.removeEventListener=function(b,c,d){var e=Node.prototype.removeEventListener;"click"===b?e.call(a,b,c.hijacked||c,d):e.call(a,b,c,d)},a.addEventListener=function(b,c,d){var e=Node.prototype.addEventListener;"click"===b?e.call(a,b,c.hijacked||(c.hijacked=function(a){a.propagationStopped||c(a)}),d):e.call(a,b,c,d)}),"function"==typeof a.onclick&&(d=a.onclick,a.addEventListener("click",function(a){d(a)},!1),a.onclick=null)}}var deviceIsAndroid=navigator.userAgent.indexOf("Android")>0,deviceIsIOS=/iP(ad|hone|od)/.test(navigator.userAgent),deviceIsIOS4=deviceIsIOS&&/OS 4_\d(_\d)?/.test(navigator.userAgent),deviceIsIOSWithBadTarget=deviceIsIOS&&/OS ([6-9]|\d{2})_\d/.test(navigator.userAgent);FastClick.prototype.needsClick=function(a){switch(a.nodeName.toLowerCase()){case"button":case"select":case"textarea":if(a.disabled)return!0;break;case"input":if(deviceIsIOS&&"file"===a.type||a.disabled)return!0;break;case"label":case"video":return!0}return/\bneedsclick\b/.test(a.className)},FastClick.prototype.needsFocus=function(a){switch(a.nodeName.toLowerCase()){case"textarea":return!0;case"select":return!deviceIsAndroid;case"input":switch(a.type){case"button":case"checkbox":case"file":case"image":case"radio":case"submit":return!1}return!a.disabled&&!a.readOnly;default:return/\bneedsfocus\b/.test(a.className)}},FastClick.prototype.sendClick=function(a,b){var c,d;document.activeElement&&document.activeElement!==a&&document.activeElement.blur(),d=b.changedTouches[0],c=document.createEvent("MouseEvents"),c.initMouseEvent(this.determineEventType(a),!0,!0,window,1,d.screenX,d.screenY,d.clientX,d.clientY,!1,!1,!1,!1,0,null),c.forwardedTouchEvent=!0,a.dispatchEvent(c)},FastClick.prototype.determineEventType=function(a){return deviceIsAndroid&&"select"===a.tagName.toLowerCase()?"mousedown":"click"},FastClick.prototype.focus=function(a){var b;deviceIsIOS&&a.setSelectionRange&&0!==a.type.indexOf("date")&&"time"!==a.type?(b=a.value.length,a.setSelectionRange(b,b)):a.focus()},FastClick.prototype.updateScrollParent=function(a){var b,c;if(b=a.fastClickScrollParent,!b||!b.contains(a)){c=a;do{if(c.scrollHeight>c.offsetHeight){b=c,a.fastClickScrollParent=c;break}c=c.parentElement}while(c)}b&&(b.fastClickLastScrollTop=b.scrollTop)},FastClick.prototype.getTargetElementFromEventTarget=function(a){return a.nodeType===Node.TEXT_NODE?a.parentNode:a},FastClick.prototype.onTouchStart=function(a){var b,c,d;if(a.targetTouches.length>1)return!0;if(b=this.getTargetElementFromEventTarget(a.target),c=a.targetTouches[0],deviceIsIOS){if(d=window.getSelection(),d.rangeCount&&!d.isCollapsed)return!0;if(!deviceIsIOS4){if(c.identifier===this.lastTouchIdentifier)return a.preventDefault(),!1;this.lastTouchIdentifier=c.identifier,this.updateScrollParent(b)}}return this.trackingClick=!0,this.trackingClickStart=a.timeStamp,this.targetElement=b,this.touchStartX=c.pageX,this.touchStartY=c.pageY,a.timeStamp-this.lastClickTime<this.tapDelay&&a.preventDefault(),!0},FastClick.prototype.touchHasMoved=function(a){var b=a.changedTouches[0],c=this.touchBoundary;return Math.abs(b.pageX-this.touchStartX)>c||Math.abs(b.pageY-this.touchStartY)>c?!0:!1},FastClick.prototype.onTouchMove=function(a){return this.trackingClick?((this.targetElement!==this.getTargetElementFromEventTarget(a.target)||this.touchHasMoved(a))&&(this.trackingClick=!1,this.targetElement=null),!0):!0},FastClick.prototype.findControl=function(a){return void 0!==a.control?a.control:a.htmlFor?document.getElementById(a.htmlFor):a.querySelector("button, input:not([type=hidden]), keygen, meter, output, progress, select, textarea")},FastClick.prototype.onTouchEnd=function(a){var b,c,d,e,f,g=this.targetElement;if(!this.trackingClick)return!0;if(a.timeStamp-this.lastClickTime<this.tapDelay)return this.cancelNextClick=!0,!0;if(this.cancelNextClick=!1,this.lastClickTime=a.timeStamp,c=this.trackingClickStart,this.trackingClick=!1,this.trackingClickStart=0,deviceIsIOSWithBadTarget&&(f=a.changedTouches[0],g=document.elementFromPoint(f.pageX-window.pageXOffset,f.pageY-window.pageYOffset)||g,g.fastClickScrollParent=this.targetElement.fastClickScrollParent),d=g.tagName.toLowerCase(),"label"===d){if(b=this.findControl(g)){if(this.focus(g),deviceIsAndroid)return!1;g=b}}else if(this.needsFocus(g))return a.timeStamp-c>100||deviceIsIOS&&window.top!==window&&"input"===d?(this.targetElement=null,!1):(this.focus(g),this.sendClick(g,a),deviceIsIOS&&"select"===d||(this.targetElement=null,a.preventDefault()),!1);return deviceIsIOS&&!deviceIsIOS4&&(e=g.fastClickScrollParent,e&&e.fastClickLastScrollTop!==e.scrollTop)?!0:(this.needsClick(g)||(a.preventDefault(),this.sendClick(g,a)),!1)},FastClick.prototype.onTouchCancel=function(){this.trackingClick=!1,this.targetElement=null},FastClick.prototype.onMouse=function(a){return this.targetElement?a.forwardedTouchEvent?!0:a.cancelable&&(!this.needsClick(this.targetElement)||this.cancelNextClick)?(a.stopImmediatePropagation?a.stopImmediatePropagation():a.propagationStopped=!0,a.stopPropagation(),a.preventDefault(),!1):!0:!0},FastClick.prototype.onClick=function(a){var b;return this.trackingClick?(this.targetElement=null,this.trackingClick=!1,!0):"submit"===a.target.type&&0===a.detail?!0:(b=this.onMouse(a),b||(this.targetElement=null),b)},FastClick.prototype.destroy=function(){var a=this.layer;deviceIsAndroid&&(a.removeEventListener("mouseover",this.onMouse,!0),a.removeEventListener("mousedown",this.onMouse,!0),a.removeEventListener("mouseup",this.onMouse,!0)),a.removeEventListener("click",this.onClick,!0),a.removeEventListener("touchstart",this.onTouchStart,!1),a.removeEventListener("touchmove",this.onTouchMove,!1),a.removeEventListener("touchend",this.onTouchEnd,!1),a.removeEventListener("touchcancel",this.onTouchCancel,!1)},FastClick.notNeeded=function(a){var b,c;if("undefined"==typeof window.ontouchstart)return!0;if(c=+(/Chrome\/([0-9]+)/.exec(navigator.userAgent)||[,0])[1]){if(!deviceIsAndroid)return!0;if(b=document.querySelector("meta[name=viewport]")){if(-1!==b.content.indexOf("user-scalable=no"))return!0;if(c>31&&document.documentElement.scrollWidth<=window.outerWidth)return!0}}return"none"===a.style.msTouchAction?!0:!1},FastClick.attach=function(a,b){return new FastClick(a,b)},"function"==typeof define&&"object"==typeof define.amd&&define.amd?define('libs/fastclick',[],function(){return FastClick}):"undefined"!=typeof module&&module.exports?(module.exports=FastClick.attach,module.exports.FastClick=FastClick):window.FastClick=FastClick;

/*
 *	jQuery dotdotdot 1.6.14
 *
 *	Copyright (c) Fred Heusschen
 *	www.frebsite.nl
 *
 *	Plugin website:
 *	dotdotdot.frebsite.nl
 *
 *	Dual licensed under the MIT and GPL licenses.
 *	http://en.wikipedia.org/wiki/MIT_License
 *	http://en.wikipedia.org/wiki/GNU_General_Public_License
 */

(function( $, undef )
{
	if ( $.fn.dotdotdot )
	{
		return;
	}

	$.fn.dotdotdot = function( o )
	{
		if ( this.length == 0 )
		{
			$.fn.dotdotdot.debug( 'No element found for "' + this.selector + '".' );
			return this;
		}
		if ( this.length > 1 )
		{
			return this.each(
				function()
				{
					$(this).dotdotdot( o );
				}
			);
		}


		var $dot = this;

		if ( $dot.data( 'dotdotdot' ) )
		{
			$dot.trigger( 'destroy.dot' );
		}

		$dot.data( 'dotdotdot-style', $dot.attr( 'style' ) || '' );
		$dot.css( 'word-wrap', 'break-word' );
		if ($dot.css( 'white-space' ) === 'nowrap')
		{
			$dot.css( 'white-space', 'normal' );
		}

		$dot.bind_events = function()
		{
			$dot.bind(
				'update.dot',
				function( e, c )
				{
					e.preventDefault();
					e.stopPropagation();

					opts.maxHeight = ( typeof opts.height == 'number' )
						? opts.height
						: getTrueInnerHeight( $dot );

					opts.maxHeight += opts.tolerance;

					if ( typeof c != 'undefined' )
					{
						if ( typeof c == 'string' || c instanceof HTMLElement )
						{
					 		c = $('<div />').append( c ).contents();
						}
						if ( c instanceof $ )
						{
							orgContent = c;
						}
					}

					$inr = $dot.wrapInner( '<div class="dotdotdot" />' ).children();
					$inr.contents()
						.detach()
						.end()
						.append( orgContent.clone( true ) )
						.find( 'br' ).replaceWith( '  <br />  ' ).end()
						.css({
							'height'	: 'auto',
							'width'		: 'auto',
							'border'	: 'none',
							'padding'	: 0,
							'margin'	: 0
						});

					var after = false,
						trunc = false;

					if ( conf.afterElement )
					{
						after = conf.afterElement.clone( true );
					    after.show();
						conf.afterElement.detach();
					}

					if ( test( $inr, opts ) )
					{
						if ( opts.wrap == 'children' )
						{
							trunc = children( $inr, opts, after );
						}
						else
						{
							trunc = ellipsis( $inr, $dot, $inr, opts, after );
						}
					}
					$inr.replaceWith( $inr.contents() );
					$inr = null;

					if ( $.isFunction( opts.callback ) )
					{
						opts.callback.call( $dot[ 0 ], trunc, orgContent );
					}

					conf.isTruncated = trunc;
					return trunc;
				}

			).bind(
				'isTruncated.dot',
				function( e, fn )
				{
					e.preventDefault();
					e.stopPropagation();

					if ( typeof fn == 'function' )
					{
						fn.call( $dot[ 0 ], conf.isTruncated );
					}
					return conf.isTruncated;
				}

			).bind(
				'originalContent.dot',
				function( e, fn )
				{
					e.preventDefault();
					e.stopPropagation();

					if ( typeof fn == 'function' )
					{
						fn.call( $dot[ 0 ], orgContent );
					}
					return orgContent;
				}

			).bind(
				'destroy.dot',
				function( e )
				{
					e.preventDefault();
					e.stopPropagation();

					$dot.unwatch()
						.unbind_events()
						.contents()
						.detach()
						.end()
						.append( orgContent )
						.attr( 'style', $dot.data( 'dotdotdot-style' ) || '' )
						.data( 'dotdotdot', false );
				}
			);
			return $dot;
		};	//	/bind_events

		$dot.unbind_events = function()
		{
			$dot.unbind('.dot');
			return $dot;
		};	//	/unbind_events

		$dot.watch = function()
		{
			$dot.unwatch();
			if ( opts.watch == 'window' )
			{
				var $window = $(window),
					_wWidth = $window.width(),
					_wHeight = $window.height();

				$window.bind(
					'resize.dot' + conf.dotId,
					function()
					{
						if ( _wWidth != $window.width() || _wHeight != $window.height() || !opts.windowResizeFix )
						{
							_wWidth = $window.width();
							_wHeight = $window.height();

							if ( watchInt )
							{
								clearInterval( watchInt );
							}
							watchInt = setTimeout(
								function()
								{
									$dot.trigger( 'update.dot' );
								}, 100
							);
						}
					}
				);
			}
			else
			{
				watchOrg = getSizes( $dot );
				watchInt = setInterval(
					function()
					{
						if ( $dot.is( ':visible' ) )
						{
							var watchNew = getSizes( $dot );
							if ( watchOrg.width  != watchNew.width ||
								 watchOrg.height != watchNew.height )
							{
								$dot.trigger( 'update.dot' );
								watchOrg = watchNew;
							}
						}
					}, 500
				);
			}
			return $dot;
		};
		$dot.unwatch = function()
		{
			$(window).unbind( 'resize.dot' + conf.dotId );
			if ( watchInt )
			{
				clearInterval( watchInt );
			}
			return $dot;
		};

		var	orgContent	= $dot.contents(),
			opts 		= $.extend( true, {}, $.fn.dotdotdot.defaults, o ),
			conf		= {},
			watchOrg	= {},
			watchInt	= null,
			$inr		= null;


		if ( !( opts.lastCharacter.remove instanceof Array ) )
		{
			opts.lastCharacter.remove = $.fn.dotdotdot.defaultArrays.lastCharacter.remove;
		}
		if ( !( opts.lastCharacter.noEllipsis instanceof Array ) )
		{
			opts.lastCharacter.noEllipsis = $.fn.dotdotdot.defaultArrays.lastCharacter.noEllipsis;
		}


		conf.afterElement	= getElement( opts.after, $dot );
		conf.isTruncated	= false;
		conf.dotId			= dotId++;


		$dot.data( 'dotdotdot', true )
			.bind_events()
			.trigger( 'update.dot' );

		if ( opts.watch )
		{
			$dot.watch();
		}

		return $dot;
	};


	//	public
	$.fn.dotdotdot.defaults = {
		'ellipsis'			: '... ',
		'wrap'				: 'word',
		'fallbackToLetter'	: true,
		'lastCharacter'		: {},
		'tolerance'			: 0,
		'callback'			: null,
		'after'				: null,
		'height'			: null,
		'watch'				: false,
		'windowResizeFix'	: true
	};
	$.fn.dotdotdot.defaultArrays = {
		'lastCharacter'		: {
			'remove'			: [ ' ', '\u3000', ',', ';', '.', '!', '?' ],
			'noEllipsis'		: []
		}
	};
	$.fn.dotdotdot.debug = function( msg ) {};


	//	private
	var dotId = 1;

	function children( $elem, o, after )
	{
		var $elements 	= $elem.children(),
			isTruncated	= false;

		$elem.empty();

		for ( var a = 0, l = $elements.length; a < l; a++ )
		{
			var $e = $elements.eq( a );
			$elem.append( $e );
			if ( after )
			{
				$elem.append( after );
			}
			if ( test( $elem, o ) )
			{
				$e.remove();
				isTruncated = true;
				break;
			}
			else
			{
				if ( after )
				{
					after.detach();
				}
			}
		}
		return isTruncated;
	}
	function ellipsis( $elem, $d, $i, o, after )
	{
		var isTruncated	= false;

		//	Don't put the ellipsis directly inside these elements
		var notx = 'table, thead, tbody, tfoot, tr, col, colgroup, object, embed, param, ol, ul, dl, blockquote, select, optgroup, option, textarea, script, style';

		//	Don't remove these elements even if they are after the ellipsis
		var noty = 'script';

		$elem
			.contents()
			.detach()
			.each(
				function()
				{

					var e	= this,
						$e	= $(e);

					if ( typeof e == 'undefined' || ( e.nodeType == 3 && $.trim( e.data ).length == 0 ) )
					{
						return true;
					}
					else if ( $e.is( noty ) )
					{
						$elem.append( $e );
					}
					else if ( isTruncated )
					{
						return true;
					}
					else
					{
						$elem.append( $e );
						if ( after )
						{
							$elem[ $elem.is( notx ) ? 'after' : 'append' ]( after );
						}
						if ( test( $i, o ) )
						{
							if ( e.nodeType == 3 ) // node is TEXT
							{
								isTruncated = ellipsisElement( $e, $d, $i, o, after );
							}
							else
							{
								isTruncated = ellipsis( $e, $d, $i, o, after );
							}

							if ( !isTruncated )
							{
								$e.detach();
								isTruncated = true;
							}
						}

						if ( !isTruncated )
						{
							if ( after )
							{
								after.detach();
							}
						}
					}
				}
			);

		return isTruncated;
	}
	function ellipsisElement( $e, $d, $i, o, after )
	{
		var e = $e[ 0 ];

		if ( !e )
		{
			return false;
		}

		var txt			= getTextContent( e ),
			space		= ( txt.indexOf(' ') !== -1 ) ? ' ' : '\u3000',
			separator	= ( o.wrap == 'letter' ) ? '' : space,
			textArr		= txt.split( separator ),
			position 	= -1,
			midPos		= -1,
			startPos	= 0,
			endPos		= textArr.length - 1;


		//	Only one word
		if ( o.fallbackToLetter && startPos == 0 && endPos == 0 )
		{
			separator	= '';
			textArr		= txt.split( separator );
			endPos		= textArr.length - 1;
		}

		while ( startPos <= endPos && !( startPos == 0 && endPos == 0 ) )
		{
			var m = Math.floor( ( startPos + endPos ) / 2 );
			if ( m == midPos )
			{
				break;
			}
			midPos = m;

			setTextContent( e, textArr.slice( 0, midPos + 1 ).join( separator ) + o.ellipsis );

			if ( !test( $i, o ) )
			{
				position = midPos;
				startPos = midPos;
			}
			else
			{
				endPos = midPos;

				//	Fallback to letter
				if (o.fallbackToLetter && startPos == 0 && endPos == 0 )
				{
					separator	= '';
					textArr		= textArr[ 0 ].split( separator );
					position	= -1;
					midPos		= -1;
					startPos	= 0;
					endPos		= textArr.length - 1;
				}
			}
		}

		if ( position != -1 && !( textArr.length == 1 && textArr[ 0 ].length == 0 ) )
		{
			txt = addEllipsis( textArr.slice( 0, position + 1 ).join( separator ), o );
			setTextContent( e, txt );
		}
		else
		{
			var $w = $e.parent();
			$e.detach();

			var afterLength = ( after && after.closest($w).length ) ? after.length : 0;

			if ( $w.contents().length > afterLength )
			{
				e = findLastTextNode( $w.contents().eq( -1 - afterLength ), $d );
			}
			else
			{
				e = findLastTextNode( $w, $d, true );
				if ( !afterLength )
				{
					$w.detach();
				}
			}
			if ( e )
			{
				txt = addEllipsis( getTextContent( e ), o );
				setTextContent( e, txt );
				if ( afterLength && after )
				{
					$(e).parent().append( after );
				}
			}
		}

		return true;
	}
	function test( $i, o )
	{
		return $i.innerHeight() > o.maxHeight;
	}
	function addEllipsis( txt, o )
	{
		while( $.inArray( txt.slice( -1 ), o.lastCharacter.remove ) > -1 )
		{
			txt = txt.slice( 0, -1 );
		}
		if ( $.inArray( txt.slice( -1 ), o.lastCharacter.noEllipsis ) < 0 )
		{
			txt += o.ellipsis;
		}
		return txt;
	}
	function getSizes( $d )
	{
		return {
			'width'	: $d.innerWidth(),
			'height': $d.innerHeight()
		};
	}
	function setTextContent( e, content )
	{
		if ( e.innerText )
		{
			e.innerText = content;
		}
		else if ( e.nodeValue )
		{
			e.nodeValue = content;
		}
		else if (e.textContent)
		{
			e.textContent = content;
		}

	}
	function getTextContent( e )
	{
		if ( e.innerText )
		{
			return e.innerText;
		}
		else if ( e.nodeValue )
		{
			return e.nodeValue;
		}
		else if ( e.textContent )
		{
			return e.textContent;
		}
		else
		{
			return "";
		}
	}
	function getPrevNode( n )
	{
		do
		{
			n = n.previousSibling;
		}
		while ( n && n.nodeType !== 1 && n.nodeType !== 3 );

		return n;
	}
	function findLastTextNode( $el, $top, excludeCurrent )
	{
		var e = $el && $el[ 0 ], p;
		if ( e )
		{
			if ( !excludeCurrent )
			{
				if ( e.nodeType === 3 )
				{
					return e;
				}
				if ( $.trim( $el.text() ) )
				{
					return findLastTextNode( $el.contents().last(), $top );
				}
			}
			p = getPrevNode( e );
			while ( !p )
			{
				$el = $el.parent();
				if ( $el.is( $top ) || !$el.length )
				{
					return false;
				}
				p = getPrevNode( $el[0] );
			}
			if ( p )
			{
				return findLastTextNode( $(p), $top );
			}
		}
		return false;
	}
	function getElement( e, $i )
	{
		if ( !e )
		{
			return false;
		}
		if ( typeof e === 'string' )
		{
			e = $(e, $i);
			return ( e.length )
				? e
				: false;
		}
		return !e.jquery
			? false
			: e;
	}
	function getTrueInnerHeight( $el )
	{
		var h = $el.innerHeight(),
			a = [ 'paddingTop', 'paddingBottom' ];

		for ( var z = 0, l = a.length; z < l; z++ )
		{
			var m = parseInt( $el.css( a[ z ] ), 10 );
			if ( isNaN( m ) )
			{
				m = 0;
			}
			h -= m;
		}
		return h;
	}


	//	override jQuery.html
	var _orgHtml = $.fn.html;
	$.fn.html = function( str )
	{
		if ( str != undef && !$.isFunction( str ) && this.data( 'dotdotdot' ) )
		{
			return this.trigger( 'update', [ str ] );
		}
		return _orgHtml.apply( this, arguments );
	};


	//	override jQuery.text
	var _orgText = $.fn.text;
	$.fn.text = function( str )
	{
		if ( str != undef && !$.isFunction( str ) && this.data( 'dotdotdot' ) )
		{
			str = $( '<div />' ).text( str ).html();
			return this.trigger( 'update', [ str ] );
		}
		return _orgText.apply( this, arguments );
	};


})( jQuery );

define("jquery.dotdotdot", ["jquery"], function(){});

