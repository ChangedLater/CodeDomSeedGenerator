using System;

namespace CodeDomDynamicGenerator
{
	/// <summary>
	/// A class to represnt a property which has been reflected from an instance
	/// </summary>
	public class CDPropertyValue
	{
		/// <summary>
		/// The type of the property being represented
		/// </summary>
		public Type PropType { get; set; }
		/// <summary>
		/// The value of the property being represented
		/// </summary>
		public object PropValue { get; set; }

		public CDPropertyValue(Type propertyType, object propertyValue)
		{
			PropType = propertyType;
			PropValue = propertyValue;
		}
	}
}
