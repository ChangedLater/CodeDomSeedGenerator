using CodeDomDynamicGenerator.Interfaces;
using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;

namespace CodeDomDynamicGenerator
{
	internal class CDInstanceGenerator : ICDInstanceGenerator
	{
		private static int instanceCounter = 0;
		private string className;
		private string instanceName;
		private List<CodeStatement> statements { get; set; }
		private List<string> imports { get; set; }

		internal CDInstanceGenerator(string className, string instanceName) : this()
		{
			this.className = className;
			this.instanceName = instanceName;
			CreateInstance();
		}

		internal CDInstanceGenerator(ICDReflectedInstance instanceToGenerate) : this()
		{
			className = instanceToGenerate.GetClassName();
			// unique instance name
			this.instanceName = className.ToLower() + (instanceCounter++);
			CreateInstance();
			imports.Add(instanceToGenerate.GetClassName());

			foreach( var prop in instanceToGenerate.GetPropertyValues())
			{
				var propName = prop.Key;
				var propValue = prop.Value.Item2;
				var propType = prop.Value.Item1;
				dynamic typedPropValue = Convert.ChangeType(propValue, propType);
				_ = CreatePropertyAssignment(prop.Key, typedPropValue);
			}
		}

		//private T ConvertType

		private CDInstanceGenerator()
		{
			statements = new List<CodeStatement>();
			imports = new List<string>();
		}

		private void CreateInstance()
		{
			CodeObjectCreateExpression objectCreate =
				new CodeObjectCreateExpression(
				new CodeTypeReference(className)
				);

			var declaration =  new CodeVariableDeclarationStatement(
				new CodeTypeReference(className), instanceName,
				objectCreate);
			statements.Add(declaration);
		}

		internal CodeAssignStatement CreatePropertyAssignment(string propertyName, object value)
		{
			var variableReference = new CodeVariableReferenceExpression(instanceName);
	
			var propReference = new CodeFieldReferenceExpression(
					variableReference, propertyName
				);
			// assigning flag enums to be handled as integers
			var valueType = value.GetType();
			CodeExpression assignStatementValue = null;
			// if its a non flag enum add the assignment statement using the enum type
			if ( valueType.IsEnum && !valueType.IsDefined(typeof(FlagsAttribute), false) )
			{
				assignStatementValue =  new CodeFieldReferenceExpression(
					new CodeTypeReferenceExpression(valueType),
					value.ToString()
				);
			}
			// otherwise use primitive assignment
			else 
			{
				if(valueType.IsEnum && !valueType.IsDefined(typeof(FlagsAttribute), false))
				{
					value = Convert.ToInt32(value);
				}
				assignStatementValue = new CodePrimitiveExpression(value);
			}

			var assignmentStatement = new CodeAssignStatement(
				propReference,
				assignStatementValue
			);
			statements.Add(assignmentStatement);
			return assignmentStatement;
		}

		public IEnumerable<CodeStatement> GetStatements()
		{
			return statements;
		}

		public IEnumerable<string> GetImports()
		{
			return imports;
		}
	}
}
