using System;
using System.Collections.Generic;
using System.Text;

namespace CodeDomDynamicGeneratorTests
{
	class TestClass
	{
		public int IntProp { get; set; }
		public string StringProp { get; set; }
		public double DoubleProp { get; set; }
		public List<int> ListProp { get; set; }
		public int PrivateGetterProp { private get; set; }
		// setter is private so give it a default for testing
		public int PrivateSetterProp { get; private set; } = 3;
	}
}
