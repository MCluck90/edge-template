// Disable 'await' warnings
#pragma warning disable 1998

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace EdgeTemplate {

	public class Startup {
		private static Dictionary<int, JSClass> _jsInstances = new Dictionary<int, JSClass>();
		private static Dictionary<string, Type> _nameToType = new Dictionary<string, Type>();
		private static int _instanceID = 0;

		/// <summary>
		/// Creates a new JS class
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
		public async Task<object> New(dynamic input) {
			string instanceType = (string)input.type;
			object[] args = (object[])input.args;
			int id = _instanceID++;
			JSClass instance;
			Type type;

			if (_nameToType.ContainsKey(instanceType)) {
				type = _nameToType[instanceType];
				instance = (JSClass)Activator.CreateInstance(type, id);
				type.GetMethod("Initialize").Invoke(instance, args);
				_jsInstances.Add(id, instance);
				return instance;
			}

			type = FindJSClassByName(instanceType);
			_nameToType.Add(instanceType, type);
			instance = (JSClass)Activator.CreateInstance(type, id);
			type.GetMethod("Initialize").Invoke(instance, args);
			_jsInstances.Add(id, instance);
			return instance;
		}

		/// <summary>
		/// Finds
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		private Type FindJSClassByName(string name) {
			Type baseType = typeof(JSClass);
			return Assembly.GetAssembly(baseType)
				.GetTypes()
				.Where(t =>
					t != baseType &&
					baseType.IsAssignableFrom(t) &&
					t.Name == name).First();
		}

		/// <summary>
		/// Runs a method on a JSClass
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
		public async Task<object> RunMethod(dynamic input) {
			int id = (int)input.id;
			string methodName = (string)input.methodName;
			object[] args = (object[])input.args;

			return _jsInstances[id].CallMethod(methodName, args);
		}

		/// <summary>
		/// Gets a property of a JSClass
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
		public async Task<object> GetProperty(dynamic input) {
			int id = (int)input.id;
			string propertyName = (string)input.propertyName;
			return _jsInstances[id].GetProperty(propertyName);
		}

		/// <summary>
		/// Sets a property on a JSClass
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
		public async Task<object> SetProperty(dynamic input) {
			int id = (int)input.id;
			string propertyName = (string)input.propertyName;
			object value = (object)input.value;
			_jsInstances[id].SetProperty(propertyName, value);
			return null;
		}

		/// <summary>
		/// Removes a reference to a JSClass
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
		public async Task<object> RemoveReference(dynamic input) {
			int id = (int)input;
			_jsInstances.Remove(id);
			return null;
		}
	}
}
