using CodeDomDynamicGenerator;
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
			var className = "TestClass";
			var compileUnit = new CDCompileUnitGenerator("SampleNamespace");
			compileUnit.AddImports(new string[] { "System" });
			var target = compileUnit.AddClass(className);

			var instanceGen = new CDInstanceGenerator("TestClass", "test");
			instanceGen.CreatePropertyAssignment("Value", 2.0);

			compileUnit.AddEntryPoint(target, instanceGen.statements);

			string fileName = GetPathForClass(className);
			using ( var sw = new StreamWriter(fileName))
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

}
