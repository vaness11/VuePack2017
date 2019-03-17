using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace VuePack.Helpers
{
	public class ComponentData
	{
		public string Name { get; }
		public string Description { get; }
		public Dictionary<string, string> Props { get; }

		/// <summary>
		/// Make all prop names lowercase ("fullWidth" -> "full-width")
		/// </summary>
		/// TODO: Move to settings
		const bool lowercaseProps = true;
		/// <summary>
		/// Add : as prefix to prop names
		/// </summary>
		/// TODO: Move to settings
		const bool addPrefixedProps = true;

		[JsonConstructor]
		public ComponentData(string name, string description, Dictionary<string, string> props)
		{
			if (string.IsNullOrWhiteSpace(name))
				throw new ArgumentNullException(nameof(name));

			Name = name;
			Description = description ?? "";
			
			if (props == null || props.Count == 0)
				Props = new Dictionary<string, string>(0);
			else
			{
				Props = new Dictionary<string, string>(props.Count);
				foreach(var prop in props)
				{
					var key = ToLowerCase(prop.Key);
					Props[key] = prop.Value;
					if (addPrefixedProps)
						Props[":" + key] = prop.Value;
				}
			}
		}

		string ToLowerCase(string input)
		{
			if (!lowercaseProps || string.IsNullOrWhiteSpace(input))
				return input;
			string replaced = Regex.Replace(input, @"(?<!-)([A-Z])", "-$1").ToLower();
			if (replaced[0] == '-')
				replaced = replaced.Substring(1);
			return replaced;
		}
	}
}
