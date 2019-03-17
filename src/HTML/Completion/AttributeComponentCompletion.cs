using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.Html.Editor.Completion;
using Microsoft.Html.Editor.Completion.Def;
using Microsoft.VisualStudio.Utilities;
using VuePack.Helpers;

namespace VuePack
{
	[HtmlCompletionProvider(CompletionTypes.Attributes, "*")]
	[ContentType("htmlx")]
	class AttributeComponentCompletion : BaseCompletion
	{
		public override string CompletionType
		{
			get { return CompletionTypes.Attributes; }
		}

		public override IList<HtmlCompletion> GetEntries(HtmlCompletionContext context)
		{
			var session = context.Session;
			var elementName = context.Element.Name;
			var cache = ComponentsCache.GetCache();
			ComponentData component = null;
			cache.TryGetValue(elementName, out component);
			if (component == null)
				return new List<HtmlCompletion>(0);

			var list = new List<HtmlCompletion>(component.Props.Count);

			foreach (var prop in component.Props)
				list.Add(CreateItem(prop.Key, prop.Value, session));

			return list;
		}
	}
}
