using System.CodeDom;
using System.Collections.Generic;

namespace CodeDomDynamicGenerator.Interfaces
{
	public interface ICDInstanceGenerator
	{
		IEnumerable<CodeStatement> CodeStatements { get; }
		IEnumerable<string> Imports { get; }
	}
}
