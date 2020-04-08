using CodeDomDynamicGenerator;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace CodeDomDynamicGeneratorTests
{
	public class CDReflectedInstanceTests
	{
		[Fact]
		public void CDReflectedInstance_ConstructorSucceeds()
		{
			var o = new Object();
			var reflectedInstance = new CDReflectedInstance(o);
		}

		[Fact]
		public void CDReflectedInstance_ReflectInstance_ReflectsClassAndNamespace()
		{
			var toBeReflected = new TestClass()
			{
				DoubleProp = 2.2,
				IntProp = 5,
				StringProp = "A String"
			};

			var reflectedInstance = new CDReflectedInstance(toBeReflected);

			Assert.Equal(nameof(toBeReflected), reflectedInstance.className);
			Assert.Equal("CodeDomDynamicGenerator", reflectedInstance.nameSpace);

		}

		[Fact]
		public void CDReflectedInstance_ReflectInstance_ContainsAllProperties()
		{
			var toBeReflected = new TestClass()
			{
				DoubleProp = 2.2,
				IntProp = 5,
				StringProp = "A String"
			};

			var reflectedInstance = new CDReflectedInstance(toBeReflected);

			var propertyList = reflectedInstance.propertyValues;
			Assert.True(propertyList.ContainsKey("DoubleProp"));
			Assert.True(propertyList.ContainsKey("IntProp"));
			Assert.True(propertyList.ContainsKey("StringProp"));

		}

		[Fact]
		public void CDReflectedInstance_ReflectInstance_PropertyValueMatches()
		{
			var expectedName = "DoubleProp";
			var expectedType = typeof(double);
			var expectedValue = 2.2;

			var toBeReflected = new TestClass()
			{
				DoubleProp = expectedValue,
			};

			var reflectedInstance = new CDReflectedInstance(toBeReflected);

			var propertyList = reflectedInstance.propertyValues;
			Assert.True(propertyList.ContainsKey(expectedName));
			var valueTuple = propertyList[expectedName];
			Assert.Equal(expectedType, valueTuple.Item1);
			Assert.Equal(expectedValue, valueTuple.Item2);
		}
	}
}
