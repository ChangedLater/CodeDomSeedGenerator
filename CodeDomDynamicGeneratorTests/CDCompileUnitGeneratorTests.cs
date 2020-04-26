using CodeDomDynamicGenerator;
using CodeDomDynamicGenerator.Interfaces;
using Moq;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
			var expectedContents = $"namespace {nameSpace}";

			var cdCompile = new CDCompileUnitGenerator(nameSpace);

			var actual = WriteCompileUnitContents(cdCompile);
			Assert.Contains(expectedContents, actual);
		}

		[Fact]
		public void CDCompileUnitGenerator_AddClass_ContainsClass()
		{
			var expectedContents = $"public sealed class {className}";

			var cdCompile = new CDCompileUnitGenerator(nameSpace);
			cdCompile.AddClass(className);

			var actual = WriteCompileUnitContents(cdCompile);
			Assert.Contains(expectedContents, actual);
		}

		[Fact]
		public void CDCompileUnitGenerator_AddImports_ContainsImports()
		{
			var imports = new string[] { "System", "System.IO" };
			var expectedImports = imports.Select( import => $"using {import};").ToList();

			var cdCompile = new CDCompileUnitGenerator(nameSpace);
			cdCompile.AddImports(imports);

			var actual = WriteCompileUnitContents(cdCompile);
			foreach ( var expected in expectedImports )
			{
				Assert.Contains(expected, actual);
			}
		}

		[Fact]
		public void CDCompileUnitGenerator_AddEntryPoint_ContainsMain()
		{
			var expectedContents = "public static void Main()";

			var cdCompile = new CDCompileUnitGenerator(nameSpace);
			var targetClass = cdCompile.AddClass(className);
			cdCompile.AddEntryPoint(targetClass);

			var actual = WriteCompileUnitContents(cdCompile);
			Assert.Contains(expectedContents, actual);
		}

		[Fact]
		public void CDCompileUnitGenerator_AddInstance_ContainsInstance()
		{
			var expectedContents = $"new {className}()";

			var cdCompile = new CDCompileUnitGenerator(nameSpace);
			var targetClass = cdCompile.AddClass(className);
			cdCompile.AddEntryPoint(targetClass);

			// instance creation declaration statement that should be created when addind an instance
			var declaration = new CodeVariableDeclarationStatement(
				new CodeTypeReference(className), "testInstance",
				new CodeObjectCreateExpression(new CodeTypeReference(className))
			);

			var mockedInstanceGenerator = new Mock<ICDInstanceGenerator>();
			mockedInstanceGenerator.Setup(instGen => instGen.CodeStatements)
				// return the code declaration above
				.Returns( new List<CodeStatement>() { declaration } );

			cdCompile.AddInstance(mockedInstanceGenerator.Object);

			var actual = WriteCompileUnitContents(cdCompile);
			Assert.Contains(expectedContents, actual);
		}

		[Fact]
		public void CDCompileUnitGenerator_AddInstance_ContainsInstancesImports()
		{
			var expectedContents = $"using {nameSpace};";

			var cdCompile = new CDCompileUnitGenerator(nameSpace);
			var targetClass = cdCompile.AddClass(className);
			cdCompile.AddEntryPoint(targetClass);

			var mockedInstanceGenerator = new Mock<ICDInstanceGenerator>();
			mockedInstanceGenerator.Setup(instGen => instGen.Imports)
				// return a string for a namespace
				.Returns(new List<string>() { nameSpace });

			cdCompile.AddInstance(mockedInstanceGenerator.Object);

			var actual = WriteCompileUnitContents(cdCompile);
			Assert.Contains(expectedContents, actual);
		}

		private string WriteCompileUnitContents(CDCompileUnitGenerator cdCompileUnit)
		{
			using (StringWriter tw = new StringWriter())
			{
				cdCompileUnit.WriteToStream(tw);
				return tw.ToString();
			}
		}
	}
}
