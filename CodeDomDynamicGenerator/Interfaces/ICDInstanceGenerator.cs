using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Text;

namespace CodeDomDynamicGenerator.Interfaces
{
	public interface ICDInstanceGenerator
	{
		IEnumerable<CodeStatement> GetStatements();
		IEnumerable<string> GetImports();
	}
}
