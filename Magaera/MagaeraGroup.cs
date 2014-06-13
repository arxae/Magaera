using System.Collections.Generic;

namespace Magaera
{
	public class MagaeraGroup
	{
		public string Name;
		public List<string> Entries;

		public MagaeraGroup(string name)
		{
			Name = name;
			Entries = new List<string>();
		}
	}
}