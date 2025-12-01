using Bunit;
using Xunit;
using Acromo; // Components are in root namespace
using Acromo; // Components are in root namespace
using System.Collections.Generic;

namespace Acromo.Tests.Components
{
    public class SuperEffectiveCoverageRowTests : TestContext
    {
        [Fact]
        public void RendersCorrectly()
        {
            // Arrange
            var coverage = new SuperEffectiveCoverage
            {
                Type = "Ice",
                Count = 3,
                AffectedPokemon = new List<string> { "Dragonite", "Garchomp", "Landorus" }
            };

            // Act
            var cut = Render<SuperEffectiveCoverageRow>(parameters => parameters
                .Add(p => p.Coverage, coverage));

            // Assert
            // Verify Type Badge
            Assert.Contains("ICE", cut.Find(".type-badge").TextContent);
            Assert.Contains("type-ice", cut.Find(".type-badge").ClassName);

            // Verify Count
            Assert.Contains("Hits 3 Pokemon", cut.Find(".hit-count-gen5").TextContent);

            // Verify Affected Pokemon Chips
            var chips = cut.FindAll(".pokemon-chip-gen5");
            Assert.Equal(3, chips.Count);
            Assert.Contains("Dragonite", chips[0].TextContent);
            Assert.Contains("Garchomp", chips[1].TextContent);
            Assert.Contains("Landorus", chips[2].TextContent);
        }
    }
}
