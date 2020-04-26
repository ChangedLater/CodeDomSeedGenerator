using System.Collections.Generic;

namespace CodeDomDynamicGeneratorTests
{
	class TestClass
	{
		public int IntProp { get; set; }
		public string StringProp { get; set; }
		public double DoubleProp { get; set; }
		public List<int> ListProp { get; set; }
		public TestEnum EnumProp { get; set; }
		public int PrivateGetterProp { private get; set; }
		// setter is private so give it a default for testing
		public int PrivateSetterProp { get; private set; } = 3;
	}

	enum TestEnum
	{
		None,
		Value1,
		Value2
	}
}
