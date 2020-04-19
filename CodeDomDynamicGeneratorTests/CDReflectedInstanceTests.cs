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
			var expectedType = toBeReflected.GetType();

			var reflectedInstance = new CDReflectedInstance(toBeReflected);

			Assert.Equal(expectedType.Name, reflectedInstance.GetClassName());
			Assert.Equal(expectedType.Namespace, reflectedInstance.GetNameSpace());
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

			var propertyList = reflectedInstance.GetPropertyValues();
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

			var propertyList = reflectedInstance.GetPropertyValues();
			Assert.True(propertyList.ContainsKey(expectedName));
			var valueTuple = propertyList[expectedName];
			Assert.Equal(expectedType, valueTuple.Item1);
			Assert.Equal(expectedValue, valueTuple.Item2);
		}

		[Fact]
		public void CDReflectedInstance_ReflectInstance_EnumReflected()
		{
			var expectedName = "EnumProp";
			var expectedType = typeof(TestEnum);
			var expectedValue = TestEnum.Value1;

			var toBeReflected = new TestClass()
			{
				EnumProp = expectedValue,
			};

			var reflectedInstance = new CDReflectedInstance(toBeReflected);

			var propertyList = reflectedInstance.GetPropertyValues();
			Assert.True(propertyList.ContainsKey(expectedName));
			var valueTuple = propertyList[expectedName];
			Assert.Equal(expectedType, valueTuple.Item1);
			Assert.Equal(expectedValue, valueTuple.Item2);
		}

		[Fact]
		public void CDReflectedInstance_ReflectInstance_ExcludesNonPublicProps()
		{
			var toBeReflected = new TestClass()
			{
				PrivateGetterProp = 2
			};

			var reflectedInstance = new CDReflectedInstance(toBeReflected);

			var propertyList = reflectedInstance.GetPropertyValues();
			Assert.False(propertyList.ContainsKey("PrivateSetterProp"));
			Assert.False(propertyList.ContainsKey("PrivateGetterProp"));
		}

		[Fact]
		public void CDReflectedInstance_ReflectInstance_ExcludesNonValueTypes()
		{
			var toBeReflected = new TestClass()
			{
				ListProp = new List<int>()
			};

			var reflectedInstance = new CDReflectedInstance(toBeReflected);

			var propertyList = reflectedInstance.GetPropertyValues();
			Assert.False(propertyList.ContainsKey("ListProp"));

		}

		[Fact]
		public void CDReflectedInstance_ReflectInstance_ExcludesUnassignedValues()
		{
			var toBeReflected = new TestClass();

			var reflectedInstance = new CDReflectedInstance(toBeReflected);

			var propertyList = reflectedInstance.GetPropertyValues();
			Assert.False(propertyList.ContainsKey("DoubleProp"));

		}

	}
}
