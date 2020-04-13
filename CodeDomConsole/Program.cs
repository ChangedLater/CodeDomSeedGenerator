using CodeDomDynamicGenerator;
using OtherNameSpace;
using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.IO;
using System.Reflection;

namespace CodeDomConsole
{
	class Program
	{

		static void Main(string[] args)
		{
			var sample = new ClassWithProps()
			{
				EnumProperty = EnumValues.Value2,
				IntProperty = 4
			};
			var reflectedInstance = new CDReflectedInstance(sample);

			var instanceGen = new CDInstanceGenerator(reflectedInstance);

			var compileUnit = new CDCompileUnitGenerator("SampleNameSpace");
			compileUnit.AddImports(new string[] { "System" });

			var className = "TestClass";
			var targetDeclaration = compileUnit.AddClass(className);

			compileUnit.AddEntryPoint(targetDeclaration);
			compileUnit.AddInstance(instanceGen);

			string fileName = GetPathForClass(className);
			using (var sw = new StreamWriter(fileName))
			{
				compileUnit.WriteToStream(sw);
			}
		}

		static void GenerateInstanceCode()
		{
			var className = "TestClass";
			var compileUnit = new CDCompileUnitGenerator("SampleNamespace");
			compileUnit.AddImports(new string[] { "System" });
			var target = compileUnit.AddClass(className);

			var instanceGen = new CDInstanceGenerator("TestClass", "test");
			instanceGen.CreatePropertyAssignment("Value", 2.0);

			compileUnit.AddEntryPoint(target, instanceGen.statements);

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