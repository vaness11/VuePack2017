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
		public Dictionary<string, string> Props { get; private set; }
		public Dictionary<string, string> Events { get; private set; }

		/// <summary>
		/// Make all prop names lowercase ("fullWidth" -> "full-width")
		/// </summary>
		/// TODO: Move to settings
		const bool lowercaseProps = true;
		///// <summary>
		///// Add ":" prefix to prop names
		///// </summary>
		///// TODO: Move to settings
		//const bool addPrefixedProps = true;
		///// <summary>
		///// Add "@" prefix to event names
		///// </summary>
		///// TODO: Move to settings
		//const bool addPrefixedEvents = true;

		[JsonConstructor]
		public ComponentData(string name, string description, 
			Dictionary<string, string> props, Dictionary<string, string> events)
		{
			if (string.IsNullOrWhiteSpace(name))
				throw new ArgumentNullException(nameof(name));

			Name = name;
			Description = description ?? "";

			ProcessProps(props);
			ProcessEvents(events);
		}

		private void ProcessEvents(Dictionary<string, string> events)
		{
			if (events == null || events.Count == 0)
				Events = new Dictionary<string, string>(0);
			else
			{
				Events = new Dictionary<string, string>(events.Count);
				foreach (var evnt in events)
				{
					Events[evnt.Key] = evnt.Value;
					//if (addPrefixedEvents)
					//	Events["@" + evnt.Key] = evnt.Value;
				}
			}
		}

		private void ProcessProps(Dictionary<string, string> props)
		{
			if (props == null || props.Count == 0)
				Props = new Dictionary<string, string>(0);
			else
			{
				Props = new Dictionary<string, string>(props.Count);
				foreach (var prop in props)
				{
					var key = ToLowerCase(prop.Key);
					Props[key] = prop.Value;
					//if (addPrefixedProps)
					//	Props[":" + key] = prop.Value;
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
