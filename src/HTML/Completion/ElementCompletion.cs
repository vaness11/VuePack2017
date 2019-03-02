using System;
using System.Collections.Generic;
using Microsoft.Html.Editor.Completion;
using Microsoft.Html.Editor.Completion.Def;
using Microsoft.VisualStudio.Utilities;

namespace VuePack
{
	[HtmlCompletionProvider(CompletionTypes.Children, "*")]
	[ContentType("htmlx")]
	class ElementCompletion : BaseCompletion
	{
		public override string CompletionType
		{
			get { return CompletionTypes.Children; }
		}

		public override IList<HtmlCompletion> GetEntries(HtmlCompletionContext context)
		{
			var session = context.Session;

			var list = new List<HtmlCompletion>
			{
				CreateItem(
					"component",
					"Props:" + Environment.NewLine +
						"  is - string | ComponentDefinition | ComponentConstructor" + Environment.NewLine +
						"  inline-template - boolean" + Environment.NewLine +
						"Usage:" + Environment.NewLine +
						"A “meta component” for rendering dynamic components.The actual component to render is determined by the is prop", 
					session
				),
				// TODO: extend description
				CreateItem(
					"transition",
					"<transition> serve as transition effects for single element/component. The <transition> only applies the transition behavior to the wrapped content inside; it doesn’t render an extra DOM element, or show up in the inspected component hierarchy.",
					session
				),
				// TODO: extend description
				CreateItem(
					"transition-group",
					"<transition-group> serve as transition effects for multiple elements/components. The <transition-group> renders a real DOM element. By default it renders a <span>, and you can configure what element it should render via the tag attribute." + Environment.NewLine +
						"Note every child in a <transition-group> must be uniquely keyed for the animations to work properly.",
					session
				),
				// TODO: extend description
				CreateItem(
					"keep-alive",
					"When wrapped around a dynamic component, <keep-alive> caches the inactive component instances without destroying them. Similar to <transition>, <keep-alive> is an abstract component: it doesn’t render a DOM element itself, and doesn’t show up in the component parent chain." + Environment.NewLine +
						"When a component is toggled inside <keep-alive>, its activated and deactivated lifecycle hooks will be invoked accordingly.",
					session
				),
				CreateItem(
					"slot",
					"Props:" + Environment.NewLine +
						"name - string, Used for named slot." + Environment.NewLine +
						"Usage:" + Environment.NewLine +
						"<slot> serve as content distribution outlets in component templates. <slot> itself will be replaced.",
					session
				),
			};

			return list;
		}
	}
}
