using System;

namespace CodeDomDynamicGenerator
{
	public class CDPropertyValue
	{
		public Type PropType { get; set; }
		public object PropValue { get; set; }

		public CDPropertyValue(Type propertyType, object propertyValue)
		{
			PropType = propertyType;
			PropValue = propertyValue;
		}
	}
}
