using System;
using System.Collections.Generic;
using Microsoft.Html.Editor.Completion;
using Microsoft.Html.Editor.Completion.Def;
using Microsoft.VisualStudio.Utilities;

namespace VuePack
{
	[HtmlCompletionProvider(CompletionTypes.Attributes, "*")]
	[ContentType("htmlx")]
	class AttributeCompletion : BaseCompletion
	{
		Dictionary<string, string> _coreDirectives = new Dictionary<string, string>
		{
			{
				"v-text",
				"Expects: string." + Environment.NewLine +
					"Usage:" + Environment.NewLine +
					"Updates the element’s textContent. If you need to update the part of textContent, you should use {{ Mustache }} interpolations."
			},
			{
				"v-html",
				"Expects: string." + Environment.NewLine +
					"Usage:" + Environment.NewLine +
					"Updates the element’s innerHTML. Note that the contents are inserted as plain HTML - they will not be compiled as Vue templates. " + Environment.NewLine +
					"Warning: If you find yourself trying to compose templates using v-html, try to rethink the solution by using components instead. Dynamically rendering arbitrary HTML on your website can be very dangerous because it can easily lead to XSS attacks. Only use v-html on trusted content and never on user-provided content.!"
			},
			{
				"v-show",
				"Expects: any." + Environment.NewLine +
					"Usage:" + Environment.NewLine +
					"Toggles the element’s display CSS property based on the truthy-ness of the expression value. This directive triggers transitions when its condition changes."
			},
			{
				"v-if",
				"Expects: any." + Environment.NewLine +
					"Usage:" + Environment.NewLine +
					"Conditionally render the element based on the truthy-ness of the expression value. The element and its contained directives / components are destroyed and re-constructed during toggles. If the element is a <template> element, its content will be extracted as the conditional block. This directive triggers transitions when its condition changes."
			},
			{
				"v-else",
				"Does not expect expression." + Environment.NewLine +
					"Restriction: previous sibling element must have v-if or v-else-if." + Environment.NewLine +
					"Denote the “else block” for v-if or a v-if/v-else-if chain."
			},
			{
				"v-else-if",
				"Expects: any." + Environment.NewLine +
					"Usage:" + Environment.NewLine +
					"Restriction: previous sibling element must have v-if or v-else-if." + Environment.NewLine +
					"Denote the “else if block” for v-if. Can be chained."
			},
			{
				"v-for",
				"Expects: Array | Object | number | string | Iterable (since 2.6)" + Environment.NewLine +
					"Usage:" + Environment.NewLine +
					"Render the element or template block multiple times based on the source data. Alternatively, you can also specify an alias for the index (or the key if used on an Object)." + Environment.NewLine +
					"Warning: When used together with v-if, v-for has a higher priority than v-if."
			},
			{
				"v-on",
				"Shorthand: @" + Environment.NewLine +
					"Expects: Function | Inline Statement | Object" + Environment.NewLine +
					"Argument: event" + Environment.NewLine +
					"Modifiers: " + Environment.NewLine +
					"  .stop - call event.stopPropagation()." + Environment.NewLine +
					"  .prevent - call event.preventDefault()." + Environment.NewLine +
					"  .capture - add event listener in capture mode." + Environment.NewLine +
					"  .self - only trigger handler if event was dispatched from this element." + Environment.NewLine +
					"  .{keyCode | keyAlias} - only trigger handler on certain keys." + Environment.NewLine +
					"  .native - listen for a native event on the root element of component." + Environment.NewLine +
					"  .once - trigger handler at most once." + Environment.NewLine +
					"  .left - (2.2.0+) only trigger handler for left button mouse events." + Environment.NewLine +
					"  .right - (2.2.0+) only trigger handler for right button mouse events." + Environment.NewLine +
					"  .middle - (2.2.0+) only trigger handler for middle button mouse events." + Environment.NewLine +
					"  .passive - (2.3.0+) attaches a DOM event with { passive: true }." + Environment.NewLine +
					"Usage:" + Environment.NewLine +
					"Attaches an event listener to the element. The event type is denoted by the argument. The expression can be a method name, an inline statement, or omitted if there are modifiers present." + Environment.NewLine +
					"When used on a normal element, it listens to native DOM events only. When used on a custom element component, it listens to custom events emitted on that child component." + Environment.NewLine +
					"When listening to native DOM events, the method receives the native event as the only argument. If using inline statement, the statement has access to the special $event property: v-on:click=\"handle('ok', $event)\"." + Environment.NewLine +
					"Starting in 2.4.0+, v-on also supports binding to an object of event/listener pairs without an argument. Note when using the object syntax, it does not support any modifiers."
			},
			{
				"v-bind",
				"Shorthand: :" + Environment.NewLine +
					"Expects: any (with argument) | Object (without argument)" + Environment.NewLine +
					"Argument: attrOrProp (optional)" + Environment.NewLine +
					"Modifiers:" + Environment.NewLine +
					"  .prop - Bind as a DOM property instead of an attribute (what’s the difference?). If the tag is a component then .prop will set the property on the component’s $el." + Environment.NewLine +
					"  .camel - (2.1.0+) transform the kebab-case attribute name into camelCase." + Environment.NewLine +
					"  .sync - (2.3.0+) a syntax sugar that expands into a v-on handler for updating the bound value." + Environment.NewLine +
					"Usage:" + Environment.NewLine +
					"Dynamically bind one or more attributes, or a component prop to an expression." + Environment.NewLine +
					"When used to bind the class or style attribute, it supports additional value types such as Array or Objects.See linked guide section below for more details." + Environment.NewLine +
					"When used for prop binding, the prop must be properly declared in the child component." + Environment.NewLine +
					"When used without an argument, can be used to bind an object containing attribute name-value pairs. Note in this mode class and style does not support Array or Objects."
			},
			{
				"v-model",
				"Expects: varies based on value of form inputs element or output of components" + Environment.NewLine +
					"Limited to:" + Environment.NewLine +
					"  <input>" + Environment.NewLine +
					"  <select>" + Environment.NewLine +
					"  <textarea>" + Environment.NewLine +
					"  components" + Environment.NewLine +
					"Modifiers:" + Environment.NewLine +
					"  .lazy - listen to change events instead of input" + Environment.NewLine +
					"  .number - cast valid input string to numbers" + Environment.NewLine +
					"  .trim - trim input" + Environment.NewLine +
					"Usage:" + Environment.NewLine +
					"Create a two-way binding on a form input element or a component. For detailed usage and other notes, see the Guide section linked below."
			},
			{
				"v-slot",
				"Shorthand: #" + Environment.NewLine +
					"Expects: JavaScript expression that is valid in a function argument position (supports destructuring in supported environments). Optional - only needed if expecting props to be passed to the slot." + Environment.NewLine +
					"Argument: slot name (optional, defaults to default)" + Environment.NewLine +
					"Limited to:" + Environment.NewLine +
					"  <template>" + Environment.NewLine +
					"  components (for a lone default slot with props)" + Environment.NewLine +
					"Usage:" + Environment.NewLine +
					"Denote named slots or slots that expect to receive props."
			},
			{
				"v-pre",
				"Does not expect expression" + Environment.NewLine +
					"Usage:" + Environment.NewLine +
					"Skip compilation for this element and all its children. You can use this for displaying raw mustache tags. Skipping large numbers of nodes with no directives on them can also speed up compilation."
			},
			{
				"v-cloak",
				"Does not expect expression" + Environment.NewLine +
					"Usage:" + Environment.NewLine +
					"This directive will remain on the element until the associated Vue instance finishes compilation. Combined with CSS rules such as [v-cloak] { display: none }, this directive can be used to hide un-compiled mustache bindings until the Vue instance is ready."
			},
			{
				"v-once",
				"Does not expect expression" + Environment.NewLine +
					"Details:" + Environment.NewLine +
					"Render the element and component once only. On subsequent re-renders, the element/component and all its children will be treated as static content and skipped.This can be used to optimize update performance."
			},
		};

		Dictionary<string, string> _coreAttrs = new Dictionary<string, string>
		{ 
			{
				"key",
				"Expects: number | string" + Environment.NewLine +
					"The key special attribute is primarily used as a hint for Vue’s virtual DOM algorithm to identify VNodes when diffing the new list of nodes against the old list.Without keys, Vue uses an algorithm that minimizes element movement and tries to patch/reuse elements of the same type in-place as much as possible.With keys, it will reorder elements based on the order change of keys, and elements with keys that are no longer present will always be removed/destroyed." + Environment.NewLine +
					"Children of the same common parent must have unique keys.Duplicate keys will cause render errors." + Environment.NewLine +
					"It can also be used to force replacement of an element/component instead of reusing it. This can be useful when you want to:" + Environment.NewLine +
					"  Properly trigger lifecycle hooks of a component" + Environment.NewLine +
					"  Trigger transitions"
			},
			{
				"ref",
				"Expects: string" + Environment.NewLine +
					"ref is used to register a reference to an element or a child component.The reference will be registered under the parent component’s $refs object. If used on a plain DOM element, the reference will be that element; if used on a child component, the reference will be component instance" + Environment.NewLine +
					"When used on elements/components with v-for, the registered reference will be an Array containing DOM nodes or component instances." + Environment.NewLine +
					"An important note about the ref registration timing: because the refs themselves are created as a result of the render function, you cannot access them on the initial render - they don’t exist yet! $refs is also non-reactive, therefore you should not attempt to use it in templates for data-binding."
			},
			{
				"is",
				"Expects: string | Object (component’s options object)" + Environment.NewLine +
					"Used for dynamic components and to work around limitations of in-DOM templates."
			}
		};

		public override string CompletionType
		{
			get { return CompletionTypes.Attributes; }
		}

		public override IList<HtmlCompletion> GetEntries(HtmlCompletionContext context)
		{
			var list = new List<HtmlCompletion>();

			foreach (var name in _coreDirectives.Keys)
			{
				list.Add(CreateItem(name, _coreDirectives[name], context.Session));
			}

			foreach (var name in _coreAttrs.Keys)
			{
				list.Add(CreateItem(name, _coreAttrs[name], context.Session));
				list.Add(CreateItem(":" + name, _coreAttrs[name], context.Session));
			}

			return list;
		}
	}
}