using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EdgeTemplate {
	class Example : JSClass {
		/// <summary>
		/// Will not be revealed to JS
		/// </summary>
		private string _hidden = "It's a secret";

		/// <summary>
		/// Properties are revealed to JS
		/// </summary>
		public string Name { get; set; }

		public Example(int id) : base(id) { }

		/// <summary>
		/// Called directly after construction
		/// Generally, use this as the constructor
		/// </summary>
		/// <param name="parameters"></param>
		public override void Initialize(object[] parameters) {
			Name = (string)parameters[0];
		}

		/// <summary>
		/// Exposed to JS
		/// Returns a delightful message
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public string SayHello(string name) {
			return "Hello, " + name + "! I'm " + Name;
		}

		/// <summary>
		/// Not exposed to JS
		/// </summary>
		/// <returns></returns>
		private string GetHidden() {
			return _hidden;
		}

	}
}
