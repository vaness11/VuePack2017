using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.Html.Editor.Completion;
using Microsoft.Html.Editor.Completion.Def;
using Microsoft.VisualStudio.Utilities;

namespace VuePack
{
    [HtmlCompletionProvider(CompletionTypes.Children, "*")]
    [ContentType("htmlx")]
    class ElementComponentCompletion : BaseCompletion
    {
        public override string CompletionType
        {
            get { return CompletionTypes.Children; }
        }

        public override IList<HtmlCompletion> GetEntries(HtmlCompletionContext context)
        {
			var session = context.Session;
            var cache = ComponentsCache.GetCache();
			var list = new List<HtmlCompletion>(cache.Count);

            foreach(var data in cache.Values)
				list.Add(CreateItem(data.Name, data.Description, session));

            return list;
        }
    }
}
