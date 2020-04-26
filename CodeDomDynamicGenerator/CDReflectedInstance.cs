using CodeDomDynamicGenerator.Interfaces;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("CodeDomDynamicGeneratorTests")]
[assembly: InternalsVisibleTo("CodeDomConsole")]
namespace CodeDomDynamicGenerator
{
	/// <summary>
	/// This class is intended to extract the required information from an object to 
	/// enable creation of that object via CodeDom
	/// </summary>

	internal class CDReflectedInstance : ICDReflectedInstance
	{
		public string ClassName { get; private set; }
		public string NameSpace { get; private set; }
		/// <summary>
		/// Dictionary mapping propery Name to its type and Value.
		/// </summary>
		public Dictionary<string, CDPropertyValue> PropertyValues { get; private set; }
		internal CDReflectedInstance(object objectToReflect)
		{
			PropertyValues = new Dictionary<string, CDPropertyValue>();
			ReflectInstance(objectToReflect);
		}

		private void ReflectInstance(object objectToReflect)
		{
			var typeOfObject = objectToReflect.GetType();
			ClassName = typeOfObject.Name;
			NameSpace = typeOfObject.Namespace;
			foreach( var prop in typeOfObject.GetProperties())
			{
				// dont write this property if its not primitive(ish)
				if(!PropertyShouldBeWritten(prop)) continue;
				var value = prop.GetValue(objectToReflect, null);
				var propertyType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
				// TODO - this could have significant performance impacts as its using refelction for every property...
				// dont write this property if the value is the default for that type
				var method = typeof(CDReflectedInstance).GetMethod("ValueIsDefault", BindingFlags.NonPublic | BindingFlags.Instance);
				var generic = method.MakeGenericMethod(propertyType);
				bool valueIsDefault = (bool)generic.Invoke(this, new object[] { value });
				if (valueIsDefault) continue;
				AddProperty(prop.Name, propertyType, value);
			}
		}

		private bool ValueIsDefaultForType(Type propertyType, object value)
		{
			// see if the value is null or if a value type the default
			return value == null
				|| propertyType.IsValueType 
				&& value == Activator.CreateInstance(propertyType);
		}

		private bool ValueIsDefault<T>(object value)
		{
			T defaultValue = default;
			T castedValue = (T)value;
			// see if the value is null or if a value type the default
			return value == null
				|| defaultValue != null && defaultValue.Equals(castedValue);
		}

		/// <summary>
		/// Only write properties that are non struct value types with public getters and setters
		/// </summary>
		/// <param name="prop">The property to inspect</param>
		/// <returns>boolean indicating if it should be added to the reflected property set</returns>
		private bool PropertyShouldBeWritten(PropertyInfo prop)
		{
			var propType = prop.PropertyType;
			// check that its primitive(ish)
			var propTypeIsWritable = propType.IsPrimitive
				|| propType.IsEnum
				|| propType == typeof(string)
				|| propType == typeof(decimal)
				|| propType == typeof(DateTime)
				|| propType == typeof(DateTimeOffset)
				|| propType == typeof(TimeSpan)
				|| propType == typeof(Guid);
			//check that it has public getter and setter
			var hasPublicAccessors = prop.GetGetMethod()?.IsPublic == true 
				&& prop.GetSetMethod()?.IsPublic == true;
			return propTypeIsWritable
				&& hasPublicAccessors;
		}

		private void AddProperty(string propertyName, Type propertyType, object propertyValue)
		{
			PropertyValues.Add(propertyName, new CDPropertyValue( propertyType, propertyValue));
		}

		public Dictionary<string, CDPropertyValue> GetPropertyValues()
		{
			return PropertyValues;
		}
	}
}
