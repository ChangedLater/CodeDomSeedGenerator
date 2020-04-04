using CodeDomDynamicGenerator;
using System;
using System.IO;
using Xunit;

namespace CodeDomDynamicGeneratorTests
{
	public class CDCompileUnitGeneratorTests
	{
		private const string nameSpace = "NameSpace";
		private const string className = "TestClass";

		[Fact]
		public void CDCompileUnitGenerator_ConstructorSucceeds()
		{
			var cdCompile = new CDCompileUnitGenerator(nameSpace);
		}

		[Fact]
		public void CDCompileUnitGenerator_ConstructorToStreamContainsNamespace()
		{
			var cdCompile = new CDCompileUnitGenerator(nameSpace);
			using(StringWriter tw = new StringWriter())
			{
				cdCompile.WriteToStream(tw);
				var actual = tw.ToString();
				var expectedContents = $"namespace {nameSpace}";
				Assert.Contains(expectedContents, actual);
			}
		}

		[Fact]
		public void CDCompileUnitGenerator_AddClassContainsClass()
		{
			var cdCompile = new CDCompileUnitGenerator(nameSpace);
			cdCompile.AddClass(className);
			using (StringWriter tw = new StringWriter())
			{
				cdCompile.WriteToStream(tw);
				var actual = tw.ToString();
				var expectedContents = $"public sealed class {className}";
				Assert.Contains(expectedContents, actual);
			}
		}

		[Fact]
		public void CDCompileUnitGenerator_AddImportsContainsImports()
		{
			var cdCompile = new CDCompileUnitGenerator(nameSpace);
			var imports = new string[] { "System", "System.IO" };
			cdCompile.AddImports(imports);
			using (StringWriter tw = new StringWriter())
			{
				cdCompile.WriteToStream(tw);
				var actual = tw.ToString();
				foreach( var import in imports)
				{
					var expectedContents = $"using {import};";
					Assert.Contains(expectedContents, actual);
				}
			}
		}

		[Fact]
		public void CDCompileUnitGenerator_AddEntryContainsMain()
		{
			var cdCompile = new CDCompileUnitGenerator(nameSpace);
			var targetClass = cdCompile.AddClass(className);
			cdCompile.AddEntryPoint(targetClass);
			using (StringWriter tw = new StringWriter())
			{
				cdCompile.WriteToStream(tw);
				var actual = tw.ToString();
				var expectedContents = "public static void Main()";
				Assert.Contains(expectedContents, actual);
			}
		}
	}
}
