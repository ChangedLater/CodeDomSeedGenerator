using System.CodeDom;
using System.Collections.Generic;

namespace CodeDomDynamicGenerator.Interfaces
{
	/// <summary>
	/// An interface for accessing the code statements required to generate an instance
	/// </summary>
	public interface ICDInstanceGenerator
	{
		/// <summary>
		/// The complete set of code statements required to generate an instance
		/// </summary>
		IEnumerable<CodeStatement> CodeStatements { get; }
		/// <summary>
		/// The complete set of code statements required to generate an instance
		/// </summary>
		CodeVariableReferenceExpression ReflectedReference { get; }
		/// <summary>
		/// The imports required for the generated code statements to successfully compile
		/// </summary>
		IEnumerable<string> Imports { get; }
	}
}
