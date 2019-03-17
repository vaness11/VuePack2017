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
			var data = ComponentDataParser.Parse(content);

			data.Name.Should().Be("v-dialog");
			data.Description.Should().StartWith("The `v-dialog` component inform users about a specific");
			data.Props
				.Should().HaveCount(14*2) // x2 for :-prefixed
				.And.ContainKeys("dark", "fullscreen", "no-click-animation")
				.And.ContainValues(
					"Mixins.Themeable.props.dark",
					"Hides the display of the overlay",
					"Mixins.Transitionable.props.origin"
				);
		}
	}
}
