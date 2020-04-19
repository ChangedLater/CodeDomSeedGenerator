using System;
using System.IO;
using Xunit;
using CodeDomDynamicGenerator;
using System.CodeDom;
using System.Linq;

namespace CodeDomDynamicGeneratorTests
{
	public class CDInstanceGeneratorTest
	{
		private const string className = "ClassName";
		private const string instanceName = "instance";

		[Fact]
		public void CDInstanceGenerator_ConstructorSucceeds()
		{
			new CDInstanceGenerator(className, instanceName);
		}

		[Fact]
		public void CDInstanceGenerator_Constructor_StatementsNotEmpty()
		{
			var instance = new CDInstanceGenerator(className, instanceName);
			var actual = instance.GetStatements();
			Assert.NotEmpty(actual);
		}

		[Fact]
		public void CDInstanceGenerator_Constructor_StatementsOfTypeCodeVariableDeclarationStatement()
		{
			var instance = new CDInstanceGenerator(className, instanceName);
			var actual = instance.GetStatements();
			Assert.IsType<CodeVariableDeclarationStatement>(actual.First());
		}

		[Fact]
		public void CDInstanceGenerator_CreatePropertyAssignment_Succeeds()
		{
			const string propName = "Value";
			const double value = 2.0;

			var instance = new CDInstanceGenerator(className, instanceName);
			var assignStatement = instance.CreatePropertyAssignment(propName, value);
		}

		[Fact]
		public void CDInstanceGenerator_CreatePropertyAssignment_HasPropAndValue()
		{
			const string propName = "Value";
			const double value = 2.0;

			var instance = new CDInstanceGenerator(className, instanceName);
			var assignStatement = instance.CreatePropertyAssignment(propName, value);
			// test type and value of left operand
			var leftOperand = assignStatement.Left;
			Assert.IsType<CodeFieldReferenceExpression>(leftOperand);
			Assert.Equal(propName, ((CodeFieldReferenceExpression)leftOperand).FieldName);

			// test type and value of right operand
			var rightOperand = assignStatement.Right;
			Assert.IsType<CodePrimitiveExpression>(rightOperand);
			Assert.Equal(value, ((CodePrimitiveExpression)rightOperand).Value);
		}
	}
}
