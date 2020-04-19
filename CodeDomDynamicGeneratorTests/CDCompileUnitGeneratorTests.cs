using CodeDomDynamicGenerator;
using CodeDomDynamicGenerator.Interfaces;
using Moq;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using Xunit;

namespace CodeDomDynamicGeneratorTests
{
	public class CDCompileUnitGeneratorTests
	{
		private const string nameSpace = "NameSpace";
		private const string className = "TestClass";

		[Fact]
		public void CDCompileUnitGenerator_Constructor_Succeeds()
		{
			var cdCompile = new CDCompileUnitGenerator(nameSpace);
		}

		[Fact]
		public void CDCompileUnitGenerator_WriteToStream_ContainsNamespace()
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
		public void CDCompileUnitGenerator_AddClass_ContainsClass()
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
		public void CDCompileUnitGenerator_AddImports_ContainsImports()
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
		public void CDCompileUnitGenerator_AddEntryPoint_ContainsMain()
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

		[Fact]
		public void CDCompileUnitGenerator_AddInstance_ContainsInstance()
		{
			var cdCompile = new CDCompileUnitGenerator(nameSpace);
			var targetClass = cdCompile.AddClass(className);
			cdCompile.AddEntryPoint(targetClass);

			// instance creation declaration statement that should be created when addind an instance
			var declaration = new CodeVariableDeclarationStatement(
				new CodeTypeReference(className), "testInstance",
				new CodeObjectCreateExpression(new CodeTypeReference(className))
			);

			var mockedInstanceGenerator = new Mock<ICDInstanceGenerator>();
			mockedInstanceGenerator.Setup(instGen => instGen.GetStatements())
				// return the code declaration above
				.Returns( new List<CodeStatement>() { declaration } );

			cdCompile.AddInstance(mockedInstanceGenerator.Object);

			using (StringWriter tw = new StringWriter())
			{
				cdCompile.WriteToStream(tw);
				var actual = tw.ToString();
				var expectedContents = $"new {className}()";
				Assert.Contains(expectedContents, actual);
			}
		}

		[Fact]
		public void CDCompileUnitGenerator_AddInstance_ContainsInstancesImports()
		{
			var cdCompile = new CDCompileUnitGenerator(nameSpace);
			var targetClass = cdCompile.AddClass(className);
			cdCompile.AddEntryPoint(targetClass);

			var mockedInstanceGenerator = new Mock<ICDInstanceGenerator>();
			mockedInstanceGenerator.Setup(instGen => instGen.GetImports())
				// return a string for a namespace
				.Returns(new List<string>() { nameSpace });

			cdCompile.AddInstance(mockedInstanceGenerator.Object);

			using (StringWriter tw = new StringWriter())
			{
				cdCompile.WriteToStream(tw);
				var actual = tw.ToString();
				var expectedContents = $"using {nameSpace};";
				Assert.Contains(expectedContents, actual);
			}
		}
	}
}
