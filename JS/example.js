'use strict';

var path = require('path'),
	EdgeWrapper = require('./edge-wrapper.js'),
	wrapper = new EdgeWrapper(path.join(__dirname, '../bin/Debug/EdgeTemplate.dll')),
	firstTime = true;

var Example = function(name) {
	var instance = wrapper.create('Example', name);
	this._id = instance._JSID;
	if (!firstTime) {
		return;
	}

	Object.keys(instance).forEach(function(key) {
		Object.defineProperty(Example.prototype, key, {
			get: function() {
				return wrapper.getProperty(this._id, key);
			},
			set: function(val) {
				wrapper.setProperty(this._id, key, val);
			}
		});
	});

	firstTime = false;
};

Example.prototype.sayHello = function(name) {
	return wrapper.runMethod(this._id, 'SayHello', name);
};

Example.prototype.getHidden = function() {
	return wrapper.runMethod(this._id, 'GetHidden');
};

module.exports = Example;