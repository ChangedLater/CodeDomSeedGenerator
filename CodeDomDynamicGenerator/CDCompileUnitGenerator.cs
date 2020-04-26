using CodeDomDynamicGenerator.Interfaces;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace CodeDomDynamicGenerator
{
	/// <summary>
	/// The main class for generating a string representing the code required to seed data
	/// 
	/// In genral this class should be created and methods should be call in the following order
	///		AddClass
	///		AddMain
	///		AddInstance
	///		
	///		WriteToStream
	/// </summary>
	public class CDCompileUnitGenerator
	{
		private CodeCompileUnit targetUnit;
		private CodeNamespace codeNamespace;
		private CodeEntryPointMethod mainMethod;

		public CDCompileUnitGenerator(string nameSpace)
		{
			targetUnit = new CodeCompileUnit();
			codeNamespace = new CodeNamespace(nameSpace);
			targetUnit.Namespaces.Add(codeNamespace);
		}

		public CodeTypeDeclaration AddClass(
			string className, 
			TypeAttributes attributes = TypeAttributes.Public | TypeAttributes.Sealed
			)
		{
			var targetClass = new CodeTypeDeclaration(className);
			targetClass.IsClass = true;
			targetClass.TypeAttributes = attributes;
			codeNamespace.Types.Add(targetClass);
			return targetClass;
		}

		public void AddImports(IEnumerable<string> imports)
		{
			foreach( var import in imports)
			{
				codeNamespace.Imports.Add(new CodeNamespaceImport(import));
			}
		}

		private void AddStatementsToMain(IEnumerable<CodeStatement> statements)
		{
			if (statements != null)
			{
				mainMethod.Statements.AddRange(statements.ToArray());
			}
		}

		public void AddInstance(ICDInstanceGenerator instanceGen)
		{
			AddStatementsToMain(instanceGen.CodeStatements);
			AddImports(instanceGen.Imports);
		}

		public void AddEntryPoint(CodeTypeDeclaration targetClass, IEnumerable<CodeStatement> statements = null)
		{
			mainMethod = new CodeEntryPointMethod();
			AddStatementsToMain(statements);
			targetClass.Members.Add(mainMethod);
		}

		public void WriteToStream(TextWriter writer)
		{
			CodeDomProvider provider = CodeDomProvider.CreateProvider("CSharp");
			var options = new CodeGeneratorOptions();
			options.BracingStyle = "C";
			provider.GenerateCodeFromCompileUnit(targetUnit, writer, options);
		}
	}
}
