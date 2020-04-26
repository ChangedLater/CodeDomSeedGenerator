using CodeDomDynamicGenerator;
using OtherNameSpace;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace CodeDomConsole
{
	class Program
	{

		static void Main(string[] args)
		{
			GenerateFromList();
		}

		static void GenerateFromList()
		{
			var sample = new ClassWithProps()
			{
				EnumProperty = EnumValues.Value2,
				IntProperty = 4
			};
			var sample2 = new ClassWithProps()
			{
				EnumProperty = EnumValues.Value2,
				IntProperty = 4
			};

			var list = new List<ClassWithProps>()
			{
				sample, sample2
			};

			var nameSpace = "SampleNameSpace";
			var className = "TestClass";
			var compileUnit = new CDSeedGenerator(nameSpace, className);

			compileUnit.AddSeedData(list);

			string fileName = GetPathForClass(className);
			using (var sw = new StreamWriter(fileName))
			{
				compileUnit.WriteToStream(sw);
			}
		}

		static void RunCodeDomSample()
		{
			var codeGen = new CodeDomTarget();
			codeGen.GenerateCode(GetPathForClass("sample"));
		}

		static string GetPathForClass(string className)
		{
			var basePath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
			return basePath + Path.DirectorySeparatorChar + className + ".cs";
		}
	}

	class ClassWithProps
	{
		public int IntProperty { get; set; }
		public EnumValues EnumProperty { get; set; }
	}

}
namespace OtherNameSpace
{
	enum EnumValues
	{
		Value1,
		Value2
	}

}