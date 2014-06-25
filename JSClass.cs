using System;
using System.Collections.Generic;
using System.Reflection;

namespace EdgeTemplate {
	/// <summary>
	/// Keeps track of different reflection items for each JS enabled class
	/// </summary>
	struct JSClassInfo {
		public Type Type;
		public Dictionary<string, MethodInfo> Methods;
		public Dictionary<string, PropertyInfo> Properties;
		public JSClassInfo(Type type) {
			Type = type;
			Methods = new Dictionary<string, MethodInfo>();
			Properties = new Dictionary<string, PropertyInfo>();
		}
	}

	/// <summary>
	/// Every class that wants to interface with JS should inherit from this
	/// </summary>
	abstract class JSClass {
		public int _JSID { get; private set; }

		protected static Dictionary<Type, JSClassInfo> _jsAccessorCache = new Dictionary<Type, JSClassInfo>();

		/// <summary>
		/// Required for Activator.CreateInstance
		/// </summary>
		public JSClass() { }

		/// <summary>
		/// Create a new JSClass
		/// </summary>
		/// <param name="id">Unique ID</param>
		public JSClass(int id) {
			_JSID = id;
			Type type = this.GetType();
			if (!_jsAccessorCache.ContainsKey(type)) {
				_jsAccessorCache.Add(type, new JSClassInfo(type));
			}
		}

		/// <summary>
		/// Treated as a constructor for all derived classes
		/// </summary>
		/// <param name="parameters"></param>
		public virtual void Initialize(object[] parameters) {

		}

		/// <summary>
		/// Calls a method on this object
		/// </summary>
		/// <param name="methodName">Name of the method</param>
		/// <param name="values">Parameters to pass in</param>
		/// <returns></returns>
		public object CallMethod(string methodName, object[] values) {
			Type type = this.GetType();
			Dictionary<string, MethodInfo> methods = _jsAccessorCache[type].Methods;
			MethodInfo method;
			if (!methods.ContainsKey(methodName)) {
				method = type.GetMethod(methodName);
				methods.Add(methodName, method);
			}
			method = methods[methodName];
			if (method.IsPublic) {
				return method.Invoke(this, values);
			}
			else {
				return null;
			}
		}

		/// <summary>
		/// Returns the value of a property
		/// </summary>
		/// <param name="propertyName">Name of the property</param>
		/// <returns></returns>
		public virtual object GetProperty(string propertyName) {
			Type type = this.GetType();
			Dictionary<string, PropertyInfo> properties = _jsAccessorCache[type].Properties;
			if (!properties.ContainsKey(propertyName)) {
				PropertyInfo propertyInfo = type.GetProperty(propertyName);
				properties.Add(propertyName, propertyInfo);
			}
			return properties[propertyName].GetValue(this);
		}

		/// <summary>
		/// Sets a value of a property
		/// </summary>
		/// <param name="propertyName">Name of the property</param>
		/// <param name="value">New value</param>
		public void SetProperty(string propertyName, object value) {
			Type type = this.GetType();
			Dictionary<string, PropertyInfo> properties = _jsAccessorCache[type].Properties;
			PropertyInfo info;
			if (!properties.ContainsKey(propertyName)) {
				type = this.GetType();
				info = type.GetProperty(propertyName);
				properties.Add(propertyName, info);
			}
			info = properties[propertyName];
			if (info.GetSetMethod().IsPrivate) {
				return;
			}
			info.SetValue(this, Convert.ChangeType(value, info.PropertyType));
		}
	}
}
