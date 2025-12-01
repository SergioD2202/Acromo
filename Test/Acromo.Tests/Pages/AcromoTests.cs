using Xunit;
using Bunit;
using Moq;
using Microsoft.Extensions.DependencyInjection;
using Acromo;
using Acromo.Services;
using System.Net.Http;
using Microsoft.JSInterop;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Acromo.Tests.Pages
{
    public class AcromoTests : TestContext
    {
        private TeamParserService _teamParserService;
        private TeamAnalysisService _teamAnalysisService;

        public AcromoTests()
        {
            // Use real service instances
            var httpClient = new HttpClient();
            _teamParserService = new TeamParserService();
            _teamAnalysisService = new TeamAnalysisService();
            var pokeAPIService = new PokeAPIService(httpClient);
            var pokeSpriteService = new PokeSpriteService(httpClient);
            var pokepasteService = new PokepasteService(httpClient);
            var jsRuntime = new Mock<IJSRuntime>().Object;

            // Register services
            Services.AddSingleton(httpClient);
            Services.AddSingleton(jsRuntime);
            Services.AddSingleton(pokeAPIService);
            Services.AddSingleton(pokeSpriteService);
            Services.AddSingleton(pokepasteService);
            Services.AddSingleton(_teamParserService);
            Services.AddSingleton(_teamAnalysisService);
        }

        [Fact]
        public void AcromoPage_RendersCorrectly()
        {
            // Act
            var cut = Render<Acromo.Pages.Acromo>();

            // Assert
            Assert.Contains("Acromo Teambuilding Helper", cut.Markup);
            Assert.Contains("🔗 Import from Pokepaste", cut.Markup);
            Assert.Contains("Or Paste Manually", cut.Markup);
        }

        [Fact]
        public async Task FetchButton_WithEmptyUrl_ShowsError()
        {
            // Arrange
            var cut = Render<Acromo.Pages.Acromo>();

            // Act - Click fetch without entering a URL
            var fetchButton = cut.Find("button.btn-primary");
            await cut.InvokeAsync(async () => await fetchButton.ClickAsync(new Microsoft.AspNetCore.Components.Web.MouseEventArgs()));

            // Assert
            Assert.Contains("Please enter a Pokepaste URL first!", cut.Markup);
        }

        [Fact]
        public async Task ParseTeamButton_ParsesAndDisplaysTeam()
        {
            // Arrange
            var teamText = "Pikachu @ Light Ball\nAbility: Static\n- Thunderbolt";

            var cut = Render<Acromo.Pages.Acromo>();

            // Act
            var textarea = cut.Find("textarea.team-input");
            await cut.InvokeAsync(() => textarea.Change(teamText));
            
            var parseButton = cut.FindAll("button.btn-primary")[1]; // Second primary button is "Parse Team"
            await cut.InvokeAsync(async () => await parseButton.ClickAsync(new Microsoft.AspNetCore.Components.Web.MouseEventArgs()));

            // Assert
            Assert.Contains("Pikachu", cut.Markup);
        }

        [Fact]
        public async Task ClearButton_ClearsTeam()
        {
            // Arrange
            var teamText = "Pikachu @ Light Ball\nAbility: Static\n- Thunderbolt";

            var cut = Render<Acromo.Pages.Acromo>();

            // Add a team first
            var textarea = cut.Find("textarea.team-input");
            await cut.InvokeAsync(() => textarea.Change(teamText));
            var parseButton = cut.FindAll("button.btn-primary")[1];
            await cut.InvokeAsync(async () => await parseButton.ClickAsync(new Microsoft.AspNetCore.Components.Web.MouseEventArgs()));

            // Act
            var clearButton = cut.Find("button.btn-secondary");
            await cut.InvokeAsync(async () => await clearButton.ClickAsync(new Microsoft.AspNetCore.Components.Web.MouseEventArgs()));

            // Assert - team should be cleared
            Assert.DoesNotContain("Pikachu", cut.Markup);
        }
    }
}
