using CodeDomDynamicGenerator.Interfaces;
using System;
using System.CodeDom;
using System.Collections.Generic;

namespace CodeDomDynamicGenerator
{
	/// <summary>
	/// A class which generates the code statements required to generate an instance
	/// 
	/// This class is intended to be used with an ICDReflectedInstance.
	/// The properties on the interface will be turned into the required code statements and necessary imports will be added added
	/// </summary>
	internal class CDInstanceGenerator : ICDInstanceGenerator
	{
		private static int instanceCounter = 0;
		private string className;
		private string instanceName;
		private List<CodeStatement> statements { get; set; }
		private List<string> imports { get; set; }

		public IEnumerable<CodeStatement> CodeStatements { get { return statements; } }
		public IEnumerable<string> Imports { get { return imports; } }

		internal CDInstanceGenerator(string className, string instanceName) : this()
		{
			this.className = className;
			this.instanceName = instanceName;
			CreateInstance();
		}

		/// <summary>
		/// Create an instance generator and the necessary code statements for the ReflectedInstance argument
		/// </summary>
		/// <param name="instanceToGenerate">The reflected instance to generate the code statements for</param>
		internal CDInstanceGenerator(ICDReflectedInstance instanceToGenerate) : this()
		{
			className = instanceToGenerate.ClassName;
			// unique instance name
			this.instanceName = className.ToLower() + (instanceCounter++);
			CreateInstance();
			imports.Add(instanceToGenerate.ClassName);

			var properties = instanceToGenerate.PropertyValues;
			foreach ( var prop in properties )
			{
				var propName = prop.Key;
				var propType = prop.Value.PropType;
				var propValue = prop.Value.PropValue;
				dynamic typedPropValue = Convert.ChangeType(propValue, propType);
				_ = CreatePropertyAssignment(prop.Key, typedPropValue);
			}
		}

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
	}
}
