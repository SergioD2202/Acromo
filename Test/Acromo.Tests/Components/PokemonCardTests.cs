using Bunit;
using Xunit;
using Acromo; // Components are in root namespace
using System.Collections.Generic;

namespace Acromo.Tests.Components
{
    public class PokemonCardTests : TestContext
    {
        [Fact]
        public void RendersBasicInfoCorrectly()
        {
            // Arrange
            var pokemon = new PokemonData
            {
                Name = "Pikachu",
                Gender = "(M)",
                SpriteUrl = "https://example.com/pikachu.png"
            };

            // Act
            var cut = Render<PokemonCard>(parameters => parameters
                .Add(p => p.Pokemon, pokemon));

            // Assert
            Assert.Contains("Pikachu", cut.Find(".pokemon-title h3").TextContent);
            Assert.Contains("(M)", cut.Find(".pokemon-title h3").TextContent);
            var img = cut.Find("img.pokemon-sprite");
            Assert.Equal("https://example.com/pikachu.png", img.GetAttribute("src"));
        }

        [Fact]
        public void RendersNickname_WhenPresent()
        {
            // Arrange
            var pokemon = new PokemonData
            {
                Name = "Pikachu",
                Nickname = "Sparky"
            };

            // Act
            var cut = Render<PokemonCard>(parameters => parameters
                .Add(p => p.Pokemon, pokemon));

            // Assert
            var title = cut.Find(".pokemon-title h3").TextContent;
            Assert.Contains("Sparky", title);
            Assert.Contains("(Pikachu)", title);
        }

        [Fact]
        public void RendersTypesAndTeraType()
        {
            // Arrange
            var pokemon = new PokemonData
            {
                Name = "Pikachu",
                Types = new List<string> { "Electric" },
                TeraType = "Flying"
            };

            // Act
            var cut = Render<PokemonCard>(parameters => parameters
                .Add(p => p.Pokemon, pokemon));

            // Assert
            Assert.Contains("Electric", cut.Find(".type-electric").TextContent);
            Assert.Contains("Tera Flying", cut.Find(".pokemon-tera-type").TextContent);
        }

        [Fact]
        public void RendersItem_WhenPresent()
        {
            // Arrange
            var pokemon = new PokemonData
            {
                Name = "Pikachu",
                Item = "Light Ball",
                ItemSpriteUrl = "https://example.com/lightball.png"
            };

            // Act
            var cut = Render<PokemonCard>(parameters => parameters
                .Add(p => p.Pokemon, pokemon));

            // Assert
            Assert.Contains("@ Light Ball", cut.Find(".pokemon-item").TextContent);
            var img = cut.Find("img.item-sprite");
            Assert.Equal("https://example.com/lightball.png", img.GetAttribute("src"));
        }

        [Fact]
        public void RendersStats_WhenPresent()
        {
            // Arrange
            var pokemon = new PokemonData
            {
                Name = "Pikachu",
                Ability = "Static",
                Level = "50",
                EVs = "252 Spe",
                IVs = "0 Atk",
                Nature = "Jolly"
            };

            // Act
            var cut = Render<PokemonCard>(parameters => parameters
                .Add(p => p.Pokemon, pokemon));

            // Assert
            Assert.Contains("Static", cut.Markup);
            Assert.Contains("Level:", cut.Markup);
            Assert.Contains("50", cut.Markup);
            Assert.Contains("EVs:", cut.Markup);
            Assert.Contains("252 Spe", cut.Markup);
            Assert.Contains("IVs:", cut.Markup);
            Assert.Contains("0 Atk", cut.Markup);
            Assert.Contains("Nature:", cut.Markup);
            Assert.Contains("Jolly", cut.Markup);
        }

        [Fact]
        public void RendersMoves()
        {
            // Arrange
            var pokemon = new PokemonData
            {
                Name = "Pikachu",
                Moves = new List<MoveInfo> 
                { 
                    new MoveInfo { Name = "Thunderbolt", Type = "Electric" },
                    new MoveInfo { Name = "Surf", Type = "Water" }
                }
            };

            // Act
            var cut = Render<PokemonCard>(parameters => parameters
                .Add(p => p.Pokemon, pokemon));

            // Assert
            var moves = cut.FindAll(".move-list li");
            Assert.Equal(2, moves.Count);
            Assert.Contains("Thunderbolt", moves[0].TextContent);
            Assert.Contains("Surf", moves[1].TextContent);
            Assert.Contains("type-border-electric", moves[0].ClassName);
            Assert.Contains("type-border-water", moves[1].ClassName);
        }
    }
}
