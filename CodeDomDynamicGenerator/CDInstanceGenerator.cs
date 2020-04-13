using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;

namespace CodeDomDynamicGenerator
{
	public class CDInstanceGenerator
	{
		private static int instanceCounter = 0;
		private string className;
		private string instanceName;
		public List<CodeStatement> statements { get; private set; }
		public List<string> imports { get; set; }

		public CDInstanceGenerator(string className, string instanceName) : this()
		{
			this.className = className;
			this.instanceName = instanceName;
			CreateInstance();
		}

		public CDInstanceGenerator(CDReflectedInstance instanceToGenerate) : this()
		{
			className = instanceToGenerate.className;
			// unique instance name
			this.instanceName = className.ToLower() + (instanceCounter++);
			CreateInstance();
			imports.Add(instanceToGenerate.nameSpace);

			foreach( var prop in instanceToGenerate.propertyValues)
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

		public CodeAssignStatement CreatePropertyAssignment(string propertyName, object value)
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
	}
}
