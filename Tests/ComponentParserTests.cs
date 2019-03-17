using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VuePack.Helpers;
using FluentAssertions;

namespace Tests
{
	[TestClass]
	public class ComponentParserTests
	{
		[TestMethod]
		public void ShouldReturnComponentData_WithValidFile()
		{
			var path = Path.Combine(Environment.CurrentDirectory, "TestData/ComponentData/vdialog.vdef.json");
			var content = File.ReadAllText(path);
			var parsed = ComponentDataParser.Parse(content);

			parsed
				.Should().NotBeNull()
				.And.HaveCount(2);

			var data2 = parsed[1];
			data2.Name.Should().Be("other-component");
			data2.Props.Should().BeEmpty();
			data2.Events.Should().BeEmpty();

			var data1 = parsed[0];
			data1.Name.Should().Be("v-dialog");
			data1.Description.Should().StartWith("The `v-dialog` component inform users about a specific");
			data1.Props
				.Should().HaveCount(14)
				.And.ContainKeys(
					"dark", 
					"fullscreen", 
					"no-click-animation"
				)
				.And.ContainValues(
					"Mixins.Themeable.props.dark",
					"Hides the display of the overlay",
					"Mixins.Transitionable.props.origin"
				);
			data1.Events
				.Should().HaveCount(7)
				.And.ContainKeys(
					"change", 
					"click:append-outer", 
					"click:prepend-inner" 
				)
				.And.ContainValues(
					"Components.Inputs.events.change",
					"Emitted when clearable icon clicked",
					"Mixins.Validatable.events['update:error']"
				);
		}
	}
}
