'use strict';

var edge = require('edge');

/**
 * Creates a wrapper for an Edge library
 * @param {string} dllPath  Path to the class library
 * @constructor
 */
var EdgeWrapper = function(dllPath) {
	this._METHODS = {
		create: edge.func({
			assemblyFile: dllPath,
			methodName: 'New'
		}),
		runMethod: edge.func({
			assemblyFile: dllPath,
			methodName: 'RunMethod'
		}),
		getProperty: edge.func({
			assemblyFile: dllPath,
			methodName: 'GetProperty'
		}),
		setProperty: edge.func({
			assemblyFile: dllPath,
			methodName: 'SetProperty'
		}),
		removeReference: edge.func({
			assemblyFile: dllPath,
			methodName: 'RemoveReference'
		})
	};
};

/**
 * Generates a new instance of an object derived from JSClass
 * @param {string} type Name of the object type
 * @returns {object}
 */
EdgeWrapper.prototype.create = function(type) {
	var args = Array.prototype.slice.call(arguments, 1);
	return this._METHODS.create({
		type: type,
		args: [args]
	}, true);
};

/**
 * Runs an instance method
 * @param {number} id           JSClass._JSID: Instance ID
 * @param {string} methodName   Name of the method to invoke
 * @param {...*}   args         Additional arguments required for method
 * @returns {*}
 */
EdgeWrapper.prototype.runMethod = function(id, methodName, args) {
	args = Array.prototype.slice.call(arguments, 2);
	return this._METHODS.runMethod({
		id: id,
		methodName: methodName,
		args: args
	}, true);
};

/**
 * Runs an asynchronous instance method
 * @param {number} id           JSClass._JSID: Instance ID
 * @param {string} methodName   Name of the method to invoke
 * @param {...*}   args         Additional arguments required for method
 * @returns {*}
 */
EdgeWrapper.prototype.runAsyncMethod = function(id, methodName, args) {
	args = Array.prototype.slice.call(arguments, 2);
	return this._METHODS.runMethod({
		id: id,
		methodName: methodName,
		args: args
	});
};

/**
 * Gets the value of a property
 * @param {number} id           JSClass._JSID: Instance ID
 * @param {string} propertyName Name of the property
 * @returns {*}
 */
EdgeWrapper.prototype.getProperty = function(id, propertyName) {
	return this._METHODS.getProperty({
		id: id,
		propertyName: propertyName
	}, true);
};

/**
 * Sets the value of a property
 * @param {number} id           JSClass._JSID: Instance ID
 * @param {string} propertyName Name of the property
 * @param {*}      value        New value
 */
EdgeWrapper.prototype.setProperty = function(id, propertyName, value) {
	this._METHODS.setProperty({
		id: id,
		propertyName: propertyName,
		value: value
	}, true);
};

module.exports = EdgeWrapper;