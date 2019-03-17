using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VuePack.Helpers
{
	public static class ComponentDataParser
	{
		public static ComponentData[] Parse(string content)
		{
			if (string.IsNullOrWhiteSpace(content))
				throw new ArgumentNullException(nameof(content));
			return JsonConvert.DeserializeObject<ComponentData[]>(content);
		}
	}
}
