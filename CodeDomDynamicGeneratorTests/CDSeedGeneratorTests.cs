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
	public class CDSeedGeneratorTests
	{
		private const string nameSpace = "NameSpace";
		private const string className = "TestClass";

		private List<TestClass> testData = new List<TestClass>()
		{
			new TestClass() {
				DoubleProp = 2.8,
				StringProp = "A String"
			},
			new TestClass()
			{
				DoubleProp = 2.2,
				IntProp = 5,
			}
		};

		[Fact]
		public void CDSeedGenerator_Constructor_Succeeds()
		{
			var cdCompile = new CDSeedGenerator(nameSpace, className);
		}

		[Fact]
		public void CDSeedGenerator_WriteToStream_ContainsNamespace()
		{
			var expectedContents = $"namespace {nameSpace}";

			var cdCompile = new CDSeedGenerator(nameSpace, className);

			var actual = WriteCompileUnitContents(cdCompile);
			Assert.Contains(expectedContents, actual);
		}

		[Fact]
		public void CDSeedGenerator_Constructor_ContainsClass()
		{
			var expectedContents = $"public sealed class {className}";

			var cdCompile = new CDSeedGenerator(nameSpace, className);

			var actual = WriteCompileUnitContents(cdCompile);
			Assert.Contains(expectedContents, actual);
		}

		[Fact]
		public void CDSeedGenerator_AddSeedData_CreatesMethod()
		{
			var expectedContents = "public static System.Collections.Generic.IEnumerable<CodeDomDynamicGeneratorTests.TestClass> GenerateSeedData()";

			var cdCompile = new CDSeedGenerator(nameSpace, className);
			cdCompile.AddSeedData(testData);

			var actual = WriteCompileUnitContents(cdCompile);
			Assert.Contains(expectedContents, actual);
		}

		[Fact]
		public void CDSeedGenerator_AddSeedData_ImportsNamespaceOfSeedType()
		{
			var expectedContents = $"using {testData[1].GetType().Namespace};";

			var cdCompile = new CDSeedGenerator(nameSpace, className);
			cdCompile.AddSeedData(testData);

			var actual = WriteCompileUnitContents(cdCompile);
			Assert.Contains(expectedContents, actual);
		}

		[Fact]
		public void CDSeedGenerator_AddSeedData_CreatesListVariable()
		{
			var expectedContents = "= new System.Collections.Generic.List<CodeDomDynamicGeneratorTests.TestClass>()";

			var cdCompile = new CDSeedGenerator(nameSpace, className);
			cdCompile.AddSeedData(testData);

			var actual = WriteCompileUnitContents(cdCompile);
			Assert.Contains(expectedContents, actual);
		}

		[Fact]
		public void CDSeedGenerator_AddSeedData_AddInputItemsToList()
		{
			var expectedOccureneces = testData.Count;

			var cdCompile = new CDSeedGenerator(nameSpace, className);
			cdCompile.AddSeedData(testData);

			var output = WriteCompileUnitContents(cdCompile);

			// Using split as a quick way to test the count is correct
			var addOccurences = output.Split(".Add(").Count() - 1;

			Assert.Equal(expectedOccureneces, addOccurences);
		}

		[Fact]
		public void CDSeedGenerator_AddSeedData_ContainsReturn()
		{
			var expectedContents = $"return seedData";

			var cdCompile = new CDSeedGenerator(nameSpace, className);
			cdCompile.AddSeedData(testData);

			var actual = WriteCompileUnitContents(cdCompile);
			Assert.Contains(expectedContents, actual);
		}



		private string WriteCompileUnitContents(CDSeedGenerator cdCompileUnit)
		{
			using (StringWriter tw = new StringWriter())
			{
				cdCompileUnit.WriteToStream(tw);
				return tw.ToString();
			}
		}
	}
}
