using System;
using System.Collections.Generic;
using System.Text;

namespace CodeDomDynamicGenerator
{
	/// <summary>
	/// This class is intended to extract the required information from an object to 
	/// enable creation of that object via CodeDom
	/// </summary>
	public class CDReflectedInstance
	{
		public string className { get; set; }
		public string nameSpace { get; set; }
		public Dictionary<string, Tuple<Type, object>> propertyValues;
		public CDReflectedInstance(object objectToReflect)
		{
			propertyValues = new Dictionary<string, Tuple<Type, object>>();
			ReflectInstance(objectToReflect);
		}

		private void ReflectInstance(object objectToReflect)
		{
			throw new NotImplementedException();
		}

		private void AddProperty(string propertyName, Type t, object value)
		{
			propertyValues.Add(propertyName, new Tuple<Type, object>( t, value));
		}
	}
}
