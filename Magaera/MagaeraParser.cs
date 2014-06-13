using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Magaera
{
	public class MagaeraParser
	{
		public List<string> Labels = new List<string>();
		public Dictionary<string, MagaeraGroup> Groups = new Dictionary<string, MagaeraGroup>();

		private readonly List<string> __keyWords = "LABEL GROUP ENTRY GROUPEND".Split(' ').ToList();
		private Random _random = new Random((int)DateTime.Now.Ticks);
		public string GenerateSentenc()
		{
			string baseSentence = Labels[_random.Next(0, Labels.Count)];

			var fullSentence = new List<string>();

			foreach (string word in baseSentence.Split(' '))
			{
				if (word.StartsWith("#{"))
				{
					fullSentence.Add(_replaceEntry(word));
					continue;
				}

				if(word.StartsWith("#d("))
				{
					fullSentence.Add(_diceRoll(word));
					continue;
				}

				fullSentence.Add(word);
			}

			return string.Join(" ", fullSentence);
		}

		private string _replaceEntry(string word)
		{
			// clean groupname -> groupname
			var groupName = word.Substring(word.IndexOf("#{") + 2);
			groupName = groupName.Substring(0, groupName.IndexOf('}'));

			// groupname full -> #{groupname}
			var groupNameFull = word.Substring(word.IndexOf("#{"));
			groupNameFull = groupNameFull.Substring(0, groupNameFull.IndexOf('}') + 1);


			var final = word.Replace(groupNameFull, Groups[groupName].Entries[_random.Next(0, Groups[groupName].Entries.Count)]);

			if (final.Contains("#{"))
			{
				final = _replaceEntry(final);
			}

			if (final.StartsWith("#d("))
			{
				final = _diceRoll(final);
			}

			return final;
		}

		private string _diceRoll(string word)
		{								//#d(D6)
			var inString = word.Substring(2);
			inString = inString.Replace("(", "");
			inString = inString.Replace(")", "");

			string[] vals = inString.Split('D');

			int dices = 1;
			int max = 0;

			if(!int.TryParse(vals[0], out dices))
			{
				dices = 1;
			}

			if(!int.TryParse(vals[1], out max))
			{
				max = 6;
			}

			int rolls = 0;
			for (int i = 0; i < dices; i++)
			{
				rolls += _random.Next(1, max);
			}

			return rolls.ToString();
		}

		public void ParseFile(string path)
		{
			string[] lines = File.ReadAllLines(path);

			string _lastGroup = string.Empty;
			foreach (var line in lines)
			{
				string _line = line.Trim();

				if (_line.StartsWith("LABEL"))
				{
					Labels.Add(ExtractContent(_line));
				}

				if (_line.StartsWith("GROUP"))
				{
					string _groupName = ExtractContent(_line);

					if (Groups.ContainsKey(_groupName) == false)
					{
						Groups.Add(_groupName, new MagaeraGroup(_groupName));
					}

					_lastGroup = _groupName;
				}

				if (_line.StartsWith("ENTRY"))
				{
					string _entry = ExtractContent(line);
					Groups[_lastGroup].Entries.Add(_entry);
				}

				if (_line.StartsWith("ENDGROUP"))
				{
					_lastGroup = string.Empty;
				}
			}
		}

		private string ExtractContent(string line)
		{
			string val = System.Text.RegularExpressions.Regex.Replace(line, @"\t|\n|\r", "");

			List<string> words = val.Split(' ').ToList();
			if( __keyWords.Contains(words.First()))
			{
				words.Remove(words.First());
			}

			val = string.Join(" ", words).Trim();

			return val;
		}
	}
}
