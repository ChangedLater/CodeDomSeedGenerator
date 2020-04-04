using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace CodeDomDynamicGenerator
{
	public class CDCompileUnitGenerator
	{
		private CodeCompileUnit targetUnit;
		private CodeNamespace codeNamespace;
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

		public void AddImports(string[] imports)
		{
			foreach( var import in imports)
			{
				codeNamespace.Imports.Add(new CodeNamespaceImport(import));
			}
		}

		public void AddEntryPoint(CodeTypeDeclaration targetClass, List<CodeStatement> statements = null)
		{
			CodeEntryPointMethod start = new CodeEntryPointMethod();
			if( statements != null)
			{
				start.Statements.AddRange(statements.ToArray());
			}
			targetClass.Members.Add(start);
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
