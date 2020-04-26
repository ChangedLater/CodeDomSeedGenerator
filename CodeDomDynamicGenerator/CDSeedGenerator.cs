using CodeDomDynamicGenerator.Interfaces;
using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace CodeDomDynamicGenerator
{
	/// <summary>
	/// The main class for generating a string representing the code required to seed data.
	/// The generated string should be a valid compiling C# file which return a list off all the data seeded.
	/// 
	/// In general this class should be used as follows
	///		var cdCompile = new CDSeedGenerator(nameSpace, className);
	///		cdCompile.AddSeedData(testData);
	///		cdCompileUnit.WriteToStream(targetStream);
	/// </summary>
	public class CDSeedGenerator
	{
		private CodeCompileUnit targetUnit;
		private CodeNamespace codeNamespace;
		private CodeEntryPointMethod mainMethod;
		private Dictionary<Type, SeedMethodContainer> seedMethodsForType;
		private CodeTypeDeclaration targetClass;

		/// <summary>
		/// Create a seed generator object
		/// </summary>
		/// <param name="nameSpace">the namespace to be used in the resultant cs file</param>
		/// <param name="className">the ClassName of the class containing seed method in the generated file</param>
		public CDSeedGenerator(string nameSpace, string className)
		{
			seedMethodsForType = new Dictionary<Type, SeedMethodContainer>();
			targetUnit = new CodeCompileUnit();
			codeNamespace = new CodeNamespace(nameSpace);
			targetUnit.Namespaces.Add(codeNamespace);
			targetClass = AddClass(className);
		}

		/// <summary>
		/// Adds a method to seed all of the items in the given list
		/// 
		/// If this method is called multiple times with the same generic type all data will be added to the single method
		/// </summary>
		/// <typeparam name="T">The type of data to seed</typeparam>
		/// <param name="seedList">the list of data to seed</param>
		public void AddSeedData<T>(IEnumerable<T> seedList)
			where T : class
		{
			var targetType = typeof(T);
			// if we havent added a seed method for this type yet do so now
			if (!seedMethodsForType.ContainsKey(targetType))
			{
				CreateSeedMethod<T>();
				var nameSpace = targetType.Namespace;
				AddImport(nameSpace);
			}
			//Now add the data seed to that method
			var targetMethodContainer = seedMethodsForType[targetType];
			//targetMethodContainer
			foreach (var itemToSeed in seedList)
			{
				var InstanceGenerator = CreateGeneratorForObject(itemToSeed);
				AddStatementsToMethod(targetMethodContainer.method, InstanceGenerator.CodeStatements);
				//now add this instance to the methods internal seed list
				var addToListInvocation = new CodeMethodInvokeExpression(
					targetMethodContainer.listVariable,
					"Add",
					InstanceGenerator.ReflectedReference
					);
				targetMethodContainer.method.Statements.Add(addToListInvocation);
			}
		}

		/// <summary>
		/// Write all of the added seed data and a containing class to a stream
		/// </summary>
		/// <param name="writer">The target writer to write all of the C# code for seed to</param>
		public void WriteToStream(TextWriter writer)
		{
			// this should be moved into a close out method as it should not be called twice
			AddReturnStatementsToSeedMethods();

			CodeDomProvider provider = CodeDomProvider.CreateProvider("CSharp");
			var options = new CodeGeneratorOptions();
			options.BracingStyle = "C";
			provider.GenerateCodeFromCompileUnit(targetUnit, writer, options);
		}

		private CodeTypeDeclaration AddClass(
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

		private void AddImports(IEnumerable<string> imports)
		{
			foreach( var import in imports)
			{
				AddImport(import);
			}
		}

		private void AddImport(string import)
		{
			codeNamespace.Imports.Add(new CodeNamespaceImport(import));
		}

		private void AddStatementsToMain(IEnumerable<CodeStatement> statements)
		{
			AddStatementsToMethod(mainMethod, statements);
		}

		private void AddStatementsToMethod(CodeMemberMethod method, IEnumerable<CodeStatement> statements)
		{
			if (statements != null)
			{
				method.Statements.AddRange(statements.ToArray());
			}
		}

		private void CreateSeedMethod<T>()
			where T : class
		{
			CodeMemberMethod seedMethod = new CodeMemberMethod();
			seedMethod.Attributes = MemberAttributes.Public | MemberAttributes.Static;
			seedMethod.Name = "GenerateSeedData";
			seedMethod.ReturnType = new CodeTypeReference(typeof(IEnumerable<T>));

			var listType = typeof(List<T>);
			CodeObjectCreateExpression objectCreate =
				new CodeObjectCreateExpression(
				new CodeTypeReference(listType)
				);
			var seedListVariableName = "seedData";
			var declaration = new CodeVariableDeclarationStatement(
				new CodeTypeReference(listType), seedListVariableName,
				objectCreate);

			seedMethod.Statements.Add(declaration);

			var seedMethodContainer = new SeedMethodContainer()
			{
				method = seedMethod,
				listVariable = new CodeVariableReferenceExpression(seedListVariableName)
			};

			seedMethodsForType.Add(typeof(T), seedMethodContainer);

			targetClass.Members.Add(seedMethod);

		}

		private void AddReturnStatementsToSeedMethods()
		{
			foreach( var seedType in seedMethodsForType.Keys)
			{
				var seedMethod = seedMethodsForType[seedType].method;
				var seededList = seedMethodsForType[seedType].listVariable;
				var returnStatement = new CodeMethodReturnStatement(seededList);
				seedMethod.Statements.Add(returnStatement);
			}
		}

		private ICDInstanceGenerator CreateGeneratorForObject<T>(T targetObject)
			where T: class
		{
			var reflectedInstance = new CDReflectedInstance(targetObject);
			var generator = new CDInstanceGenerator(reflectedInstance);
			return generator;
		}

		private void AddInstance(ICDInstanceGenerator instanceGen)
		{
			AddStatementsToMain(instanceGen.CodeStatements);
			AddImports(instanceGen.Imports);
		}

		class SeedMethodContainer
		{
			internal CodeMemberMethod method;
			internal CodeVariableReferenceExpression listVariable;
		}
	}
}
