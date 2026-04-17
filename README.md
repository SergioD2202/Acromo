# 🔬 Acromo - Teambuilding Helper

Acromo is a modern Pokemon team analysis tool built with **Blazor WebAssembly**. It helps competitive players analyze their teams by providing comprehensive insights into type coverage, team archetypes, and competitive viability based on SV OU (Scarlet/Violet Overused) standards.

The application features a nostalgic **Pokemon Generation 5 (Black & White)** inspired UI theme.

## ✨ Features

- **📝 Team Parsing**: Paste your team in standard Showdown format.
- **🖼️ Visual Display**: Automatically fetches Pokemon and item sprites (including Gen 8+ items like Heavy-Duty Boots).
- **📊 Type Coverage Analysis**:
  - Identifies neutral coverage gaps.
  - Highlights super-effective coverage against the team.
  - Lists immunity notes based on abilities and items.
- **🧠 Archetype Detection**: Automatically classifies your team as Stall, Semi-Stall, Hyper Offense, or Balanced.
- **✅ Competitive Rating**: Evaluates your team against 15+ competitive criteria (hazards, speed control, utility, etc.) and assigns a grade.
- **🎨 Gen 5 UI**: A fully custom CSS theme inspired by the Pokemon Black & White interface.

## 🛠️ Tech Stack

- **Framework**: [Blazor WebAssembly](https://dotnet.microsoft.com/apps/aspnet/web-apps/blazor) (.NET 8.0)
- **Languages**: C#, Razor, CSS
- **APIs**:
  - [PokeAPI](https://pokeapi.co/) for Pokemon data and sprites.
  - [PokéSprite](https://github.com/msikma/pokesprite) for item sprites.

## 📋 Prerequisites

Before you begin, ensure you have the following installed:

- **.NET 8.0 SDK** or later
  - [Download .NET 8.0](https://dotnet.microsoft.com/download/dotnet/8.0)
  - Verify installation: `dotnet --version`

## 🚀 Getting Started

Follow these steps to set up and run the project locally:

### 1. Clone the Repository

```bash
git clone git@github.com:SergioD2202/Acromo.git
cd Acromo
```

### 2. Restore Dependencies

```bash
dotnet restore
```

### 3. Run the Application

```bash
dotnet run
```

Or use watch mode for hot reloading during development:

```bash
dotnet watch
```

### 4. Open in Browser

Once the application starts, open your browser and navigate to the URL shown in your terminal (usually `http://localhost:5000` or `https://localhost:5001`).

## 📁 Project Structure

```
Acromo/
├── Pages/
│   └── Acromo.razor           # Main application logic and UI
├── Shared/
│   └── MainLayout.razor       # Global layout configuration
├── wwwroot/
│   ├── css/
│   │   ├── app.css            # Base application styles
│   │   └── gen5-theme.css     # Pokemon Gen 5 specific theming
│   └── index.html             # Entry HTML file
├── App.razor                  # Root component
├── Program.cs                 # Application entry point
├── _Imports.razor             # Global using statements
├── Acromo.csproj              # Project configuration
└── README.md                  # Project documentation
```

## 📖 How to Use

1.  **Export your team** from Pokemon Showdown (Teambuilder > Import/Export).
2.  **Paste** the text into the input field on the Acromo home page.
3.  Click **Parse Team**.
4.  Review the generated analysis, including your team's archetype, coverage gaps, and competitive rating.

## ⚖️ Disclaimer & Fair Use

This project is created for educational purposes only. It uses third-party assets, including Pokémon names, images, and other related media, which are the intellectual property of The Pokémon Company, Nintendo, Game Freak, and Creatures Inc.

These assets are used under the principles of **Fair Use** for educational and informational purposes. This project is not affiliated with, endorsed by, or sponsored by The Pokémon Company or any of its partners. No copyright infringement is intended.
