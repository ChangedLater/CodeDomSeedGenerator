using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.IO;
using System.Reflection;

namespace CodeDomConsole
{
	/// <summary>
	/// This was a first pass at using code dom based on microsoft docs examples
	/// </summary>
	class CodeDomTarget
	{
		private CodeCompileUnit targetUnit;
		private CodeTypeDeclaration targetClass;

		private const string outputFileName = "SampleClass.cs";

		private const string className = "SampleClass";
		private const string name = "Name";
		private const string description = "Description";
		private const string descriptionBacking = "_description";
		private const string methodName = "ConcatNameAndDescription";


		public CodeDomTarget()
		{
			targetUnit = new CodeCompileUnit();
			CodeNamespace sampleNamespace = new CodeNamespace("space");
			sampleNamespace.Imports.Add(new CodeNamespaceImport("System"));
			targetClass = new CodeTypeDeclaration(className);
			targetClass.IsClass = true;
			targetClass.TypeAttributes = TypeAttributes.Public | TypeAttributes.Sealed;
			sampleNamespace.Types.Add(targetClass);
			targetUnit.Namespaces.Add(sampleNamespace);
			AddFields();
			AddProps();
			AddNameDescMethod();
			AddConstructor();
			AddEntryPoint();
		}

		private void AddFields()
		{
			var nameField = new CodeMemberField();
			nameField.Attributes = MemberAttributes.Private;
			nameField.Name = name;
			nameField.Type = new CodeTypeReference(typeof(String));
			nameField.Comments.Add(new CodeCommentStatement("This is the name."));
			targetClass.Members.Add(nameField);

			var descBackingField = new CodeMemberField();
			descBackingField.Attributes = MemberAttributes.Private;
			descBackingField.Name = descriptionBacking;
			descBackingField.Type = new CodeTypeReference(typeof(String));
			descBackingField.Comments.Add(new CodeCommentStatement("Backing field for description."));
			targetClass.Members.Add(descBackingField);
		}

		private void AddProps()
		{
			var descriptionProp = new CodeMemberProperty();
			descriptionProp.Attributes = MemberAttributes.Public | MemberAttributes.Final;
			descriptionProp.Name = description;
			descriptionProp.Type = new CodeTypeReference(typeof(String));
			descriptionProp.HasGet = true;
			descriptionProp.GetStatements.Add(new CodeMethodReturnStatement(
							new CodeFieldReferenceExpression(
							new CodeThisReferenceExpression(), descriptionBacking)));

			targetClass.Members.Add(descriptionProp);
		}

		private void AddNameDescMethod()
		{
			CodeMemberMethod concatMethod = new CodeMemberMethod();
			concatMethod.Attributes = MemberAttributes.Public | MemberAttributes.Final;
			concatMethod.Name = methodName;
			concatMethod.ReturnType = new CodeTypeReference(typeof(string));

			var nameReference = new CodeFieldReferenceExpression(
					new CodeThisReferenceExpression(), name
				);
			var descriptionReference = new CodeFieldReferenceExpression(
					new CodeThisReferenceExpression(), description
				);
			var returnStatement = new CodeMethodReturnStatement();

			var formattedOutput = "Object is name {0}, with description {1}";

			returnStatement.Expression = new CodeMethodInvokeExpression(
				new CodeTypeReferenceExpression("System.String"),
				"Format",
				new CodePrimitiveExpression(formattedOutput),
				nameReference, descriptionReference
				);
			concatMethod.Statements.Add(returnStatement);
			targetClass.Members.Add(concatMethod);
		}

		public void AddConstructor()
		{
			// Declare the constructor
			CodeConstructor constructor = new CodeConstructor();
			constructor.Attributes =
				MemberAttributes.Public | MemberAttributes.Final;

			var nameReference = new CodeFieldReferenceExpression(
					new CodeThisReferenceExpression(), name
				);
			constructor.Statements.Add(
				new CodeAssignStatement(
					nameReference,
					new CodePrimitiveExpression("default name")
				)
			);

			var descReference = new CodeFieldReferenceExpression(
					new CodeThisReferenceExpression(), descriptionBacking
				);
			constructor.Statements.Add(
				new CodeAssignStatement(
					descReference,
					new CodePrimitiveExpression("default description")
				)
			);
			targetClass.Members.Add(constructor);
		}

		public void AddEntryPoint()
		{
			CodeEntryPointMethod start = new CodeEntryPointMethod();
			CodeObjectCreateExpression objectCreate =
				new CodeObjectCreateExpression(
				new CodeTypeReference(className)
				);

			// Add the statement:
			// "CodeDOMCreatedClass testClass = 
			//     new CodeDOMCreatedClass(5.3, 6.9);"
			start.Statements.Add(new CodeVariableDeclarationStatement(
				new CodeTypeReference(className), "testClass",
				objectCreate));

			var variableReference = new CodeVariableReferenceExpression("testClass");

			// Creat the expression:
			// "testClass.ToString()"
			CodeMethodInvokeExpression toStringInvoke =
				new CodeMethodInvokeExpression(variableReference, methodName);

			// Add a System.Console.WriteLine statement with the previous 
			// expression as a parameter.
			var invokeMethod = new CodeMethodInvokeExpression(
				new CodeTypeReferenceExpression("System.Console"),
				"WriteLine", toStringInvoke);

			start.Statements.Add(invokeMethod);

			//re-assign the name and call method again
			var nameReference = new CodeFieldReferenceExpression(
					variableReference, name
				);

			start.Statements.Add(
				new CodeAssignStatement(
					nameReference,
					new CodePrimitiveExpression("Altered Name")
				)
			);
			start.Statements.Add(invokeMethod);


			targetClass.Members.Add(start);
		}

		public void GenerateCode(string fileName)
		{
			CodeDomProvider provider = CodeDomProvider.CreateProvider("CSharp");
			var options = new CodeGeneratorOptions();
			options.BracingStyle = "C";
			using (var sw = new StreamWriter(fileName))
			{
				provider.GenerateCodeFromCompileUnit(targetUnit, sw, options);
			}
		}
	}
}
