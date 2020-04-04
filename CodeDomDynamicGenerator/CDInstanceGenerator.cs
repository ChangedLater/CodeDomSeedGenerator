using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;

namespace CodeDomDynamicGenerator
{
	public class CDInstanceGenerator
	{
		private string className;
		private string instanceName;
		public List<CodeStatement> statements { get; private set; }
		public CDInstanceGenerator(string className, string instanceName)
		{
			this.className = className;
			this.instanceName = instanceName;
			statements = new List<CodeStatement>();
			CreateInstance();
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

		public CodeAssignStatement CreatePropertyAssignment<T>(string propertyName, T value)
			where T: struct
		{
			var variableReference = new CodeVariableReferenceExpression(instanceName);
	
			var propReference = new CodeFieldReferenceExpression(
					variableReference, propertyName
				);
			var assignmentStatement = new CodeAssignStatement(
				propReference,
				new CodePrimitiveExpression(value)
			);
			statements.Add(assignmentStatement);
			return assignmentStatement;
		}
	}
}
