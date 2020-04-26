using Xunit;
using CodeDomDynamicGenerator;
using System.CodeDom;
using System.Linq;
using Moq;
using CodeDomDynamicGenerator.Interfaces;
using System.Collections.Generic;

namespace CodeDomDynamicGeneratorTests
{
	public class CDInstanceGeneratorTest
	{
		private const string className = "ClassName";
		private const string instanceName = "instance";
		private const string propName = "Value";
		private const double propValue = 2.2;

		[Fact]
		public void CDInstanceGenerator_Constructor_Succeeds()
		{
			new CDInstanceGenerator(className, instanceName);
		}

		[Fact]
		public void CDInstanceGenerator_Constructor_StatementsNotEmpty()
		{
			var instance = new CDInstanceGenerator(className, instanceName);
			var actual = instance.CodeStatements;
			Assert.NotEmpty(actual);
		}

		[Fact]
		public void CDInstanceGenerator_Constructor_StatementsOfTypeCodeVariableDeclarationStatement()
		{
			var instance = new CDInstanceGenerator(className, instanceName);
			var actual = instance.CodeStatements;
			Assert.IsType<CodeVariableDeclarationStatement>(actual.First());
		}

		[Fact]
		public void CDInstanceGenerator_CreatePropertyAssignment_Succeeds()
		{
			var instance = new CDInstanceGenerator(className, instanceName);
			var assignStatement = instance.CreatePropertyAssignment(propName, propValue);
		}

		[Fact]
		public void CDInstanceGenerator_CreatePropertyAssignment_HasPropAndValue()
		{

			var instance = new CDInstanceGenerator(className, instanceName);
			var assignStatement = instance.CreatePropertyAssignment(propName, propValue);
			// test type and value of left operand
			var leftOperand = assignStatement.Left;
			Assert.IsType<CodeFieldReferenceExpression>(leftOperand);
			Assert.Equal(propName, ((CodeFieldReferenceExpression)leftOperand).FieldName);

			// test type and value of right operand
			var rightOperand = assignStatement.Right;
			Assert.IsType<CodePrimitiveExpression>(rightOperand);
			Assert.Equal(propValue, ((CodePrimitiveExpression)rightOperand).Value);
		}

		[Fact]
		public void CDInstanceGenerator_ConstructorWithReflectedInstanceArg_Succeeds()
		{
			var mockedReflectedInstance = new Mock<ICDReflectedInstance>();
			// classname returned
			mockedReflectedInstance.Setup(reflected => reflected.ClassName)
				.Returns(className);
			// return an empty dictionary for this test
			mockedReflectedInstance.Setup(reflected => reflected.PropertyValues)
				.Returns(new Dictionary<string, CDPropertyValue>());
			new CDInstanceGenerator(mockedReflectedInstance.Object);
		}

		[Fact]
		public void CDInstanceGenerator_ConstructorWithReflectedInstanceArg_ContainsCodeVarDeclaration()
		{
			var mockedReflectedInstance = new Mock<ICDReflectedInstance>();
			// classname returned
			mockedReflectedInstance.Setup(reflected => reflected.ClassName)
				.Returns(className);
			// return an empty dictionary for this test
			mockedReflectedInstance.Setup(reflected => reflected.PropertyValues)
				.Returns(new Dictionary<string, CDPropertyValue>());

			var instance = new CDInstanceGenerator(mockedReflectedInstance.Object);

			var actual = instance.CodeStatements;
			Assert.IsType<CodeVariableDeclarationStatement>(actual.First());
		}

		[Fact]
		public void CDInstanceGenerator_ConstructorWithReflectedInstanceArg_ContainsPropAndValue()
		{
			var mockedReflectedInstance = new Mock<ICDReflectedInstance>();
			// classname returned
			mockedReflectedInstance.Setup(reflected => reflected.ClassName)
				.Returns(className);
			// return an empty dictionary for this test
			mockedReflectedInstance.Setup(reflected => reflected.PropertyValues)
				.Returns(new Dictionary<string, CDPropertyValue>()
				{
					{ propName, new CDPropertyValue(propValue.GetType(), propValue) }
				});

			var instance = new CDInstanceGenerator(mockedReflectedInstance.Object);

			var assignStatements = instance.CodeStatements.Where( statement => statement is CodeAssignStatement);
			Assert.NotEmpty(assignStatements);
			var assignStatement = assignStatements.First() as CodeAssignStatement;

			var leftOperand = assignStatement.Left;
			Assert.IsType<CodeFieldReferenceExpression>(leftOperand);
			Assert.Equal(propName, ((CodeFieldReferenceExpression)leftOperand).FieldName);

			// test type and value of right operand
			var rightOperand = assignStatement.Right;
			Assert.IsType<CodePrimitiveExpression>(rightOperand);
			Assert.Equal(propValue, ((CodePrimitiveExpression)rightOperand).Value);
		}

	}
}
