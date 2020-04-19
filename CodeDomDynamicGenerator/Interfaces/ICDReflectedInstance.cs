using System;
using System.Collections.Generic;
using System.Text;

namespace CodeDomDynamicGenerator.Interfaces
{
	public interface ICDReflectedInstance
	{
		/// <summary>
		/// The Class of the instance being reflected 
		/// </summary>
		string GetClassName();
		/// <summary>
		/// The namespace containing the class of the instance being reflected 
		/// </summary>
		string GetNameSpace();
		/// <summary>
		/// Dictionary mapping propery Name to its type and Value.
		/// </summary>
		Dictionary<string, Tuple<Type, object>> GetPropertyValues();
	}
}
