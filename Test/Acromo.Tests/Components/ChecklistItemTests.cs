using Bunit;
using Xunit;
using Acromo; // Components are in root namespace
using Acromo; // Components are in root namespace

namespace Acromo.Tests.Components
{
    public class ChecklistItemTests : TestContext
    {
        [Fact]
        public void RendersCorrectly_WhenStatusIsPass()
        {
            // Arrange
            var check = new CheckResult
            {
                Name = "Stealth Rock",
                Description = "Team has Stealth Rock",
                Status = CheckStatus.Pass,
                Details = "Torkoal has Stealth Rock"
            };

            // Act
            var cut = Render<ChecklistItem>(parameters => parameters
                .Add(p => p.Check, check));

            // Assert
            cut.MarkupMatches(@"
                <div class=""check-item check-pass"">
                    <div class=""check-header"">
                        <span class=""check-icon"">✅</span>
                        <span class=""check-name"">Stealth Rock</span>
                        <span class=""check-status-badge"">Pass</span>
                    </div>
                    <p class=""check-description"">Team has Stealth Rock</p>
                    <p class=""check-details"">Torkoal has Stealth Rock</p>
                </div>");
        }

        [Fact]
        public void RendersCorrectly_WhenStatusIsFail()
        {
            // Arrange
            var check = new CheckResult
            {
                Name = "Hazard Removal",
                Description = "Team needs hazard removal",
                Status = CheckStatus.Fail
            };

            // Act
            var cut = Render<ChecklistItem>(parameters => parameters
                .Add(p => p.Check, check));

            // Assert
            // Verify class is check-fail
            Assert.Contains("check-fail", cut.Find(".check-item").ClassName);
            // Verify icon is X
            Assert.Contains("❌", cut.Find(".check-icon").TextContent);
            // Verify details are not rendered
            Assert.Empty(cut.FindAll(".check-details"));
        }

        [Fact]
        public void RendersCorrectly_WhenStatusIsWarning()
        {
            // Arrange
            var check = new CheckResult
            {
                Name = "Speed Control",
                Description = "Team lacks speed control",
                Status = CheckStatus.Warning
            };

            // Act
            var cut = Render<ChecklistItem>(parameters => parameters
                .Add(p => p.Check, check));

            // Assert
            Assert.Contains("check-warning", cut.Find(".check-item").ClassName);
            Assert.Contains("⚠️", cut.Find(".check-icon").TextContent);
        }
    }
}
