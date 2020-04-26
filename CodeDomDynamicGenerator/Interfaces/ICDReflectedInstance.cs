using System.Collections.Generic;

namespace CodeDomDynamicGenerator.Interfaces
{
	public interface ICDReflectedInstance
	{
		/// <summary>
		/// The Class of the instance being reflected 
		/// </summary>
		string ClassName { get; }
		/// <summary>
		/// The namespace containing the class of the instance being reflected 
		/// </summary>
		string NameSpace { get; }
		/// <summary>
		/// Dictionary mapping propery Name to its type and Value.
		/// </summary>
		Dictionary<string, CDPropertyValue> PropertyValues { get; }
	}
}
