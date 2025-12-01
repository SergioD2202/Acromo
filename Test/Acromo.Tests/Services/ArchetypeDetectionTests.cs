using Xunit;
using Moq;
using Acromo.Services;
using Acromo.Services.Analysis;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Acromo.Tests.Services
{
    public class ArchetypeDetectionTests
    {
        [Fact]
        public void DetectArchetype_ShouldIdentifySunAndStallTeams()
        {
            // Arrange
            var mockDataPath = Path.Combine(Directory.GetCurrentDirectory(), "MockData", "Teams.txt");
            Assert.True(File.Exists(mockDataPath), $"Mock data file not found at {mockDataPath}");

            var fileContent = File.ReadAllText(mockDataPath);
            
            // Split the teams based on the delimiters we added
            var parts = fileContent.Split(new[] { "=== Team 1 (Sun) ===", "=== Team 2 (Stall) ===" }, StringSplitOptions.RemoveEmptyEntries);
            
            Assert.True(parts.Length >= 2, "Expected at least 2 teams in the mock data file");

            var sunTeamText = parts[0].Trim();
            var stallTeamText = parts[1].Trim();
            
            var parser = new TeamParserService();

            // Act & Assert - Sun Team
            var sunTeamData = parser.ParseTeam(sunTeamText);
            var (sunArchetype, _) = AnalysisUtils.DetectArchetype(sunTeamData);
            Assert.Equal("Sun Team", sunArchetype);

            // Act & Assert - Stall Team
            var stallTeamData = parser.ParseTeam(stallTeamText);
            var (stallArchetype, _) = AnalysisUtils.DetectArchetype(stallTeamData);
            Assert.Equal("Stall", stallArchetype);
        }
    }
}
