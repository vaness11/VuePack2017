using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.Html.Editor.Completion;
using Microsoft.Html.Editor.Completion.Def;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Utilities;
using VuePack.Helpers;

namespace VuePack
{
	[HtmlCompletionProvider(CompletionTypes.Attributes, "*")]
	[ContentType("htmlx")]
	class AttributeComponentCompletion : BaseCompletion
	{
		/// Add ":" prefix to prop names
		/// </summary>
		/// TODO: Move to settings
		const bool _addPrefixedProps = true;
		/// <summary>
		/// Add "@" prefix to event names
		/// </summary>
		/// TODO: Move to settings
		const bool _addPrefixedEvents = true;

		const int _highPriority = 9999;

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

			var prefixStart = context.Position > 10 ? context.Position - 10 : 0;
			// using text before cursor for different suggestions lists for better UX
			var cursorPrefix = context.Document.TextBuffer.CurrentSnapshot
				.GetText(prefixStart, context.Position - prefixStart);

			//ProjectHelpers.Log("cursorPrefix = " + cursorPrefix);

			var list = new List<HtmlCompletion>();
			if (cursorPrefix.EndsWith("v-on:")) // events only and no prefix
				AddEventsToList(list, session, component, false);
			else if (cursorPrefix.EndsWith("v-bind:")) // props only and no prefix
				AddPropsToList(list, session, component, false);
			else
			{
				AddEventsToList(list, session, component, _addPrefixedEvents);
				AddPropsToList(list, session, component, _addPrefixedProps);
			}

			return list;
		}

		private void AddPropsToList(List<HtmlCompletion> list, ICompletionSession session, 
			ComponentData component, bool addPrefix)
		{
			var newline = Environment.NewLine;
			var suffix = newline + newline + "Component property";
			foreach (var prop in component.Props)
			{
				var item = CreateItem(prop.Key, prop.Value + suffix, session);
				item.SortingPriority = _highPriority;
				list.Add(item);

				if (addPrefix)
				{
					item = CreateItem(":" + prop.Key, prop.Value + suffix, session);
					item.SortingPriority = _highPriority;
					list.Add(item);
				}
			}
		}

		private void AddEventsToList(List<HtmlCompletion> list, ICompletionSession session, 
			ComponentData component, bool addPrefix)
		{
			var newline = Environment.NewLine;
			var suffix = newline + newline + "Component event";
			foreach (var evnt in component.Events)
			{
				// since we cannot use unprefixed values without v-on, 
				// so we don't need prefixed and non-prefixed suggestions of event name at the same time
				var item = CreateItem((addPrefix ? "@" : "") + evnt.Key, evnt.Value + suffix, session);
				item.SortingPriority = _highPriority;
				list.Add(item);
			}
		}
	}
}
