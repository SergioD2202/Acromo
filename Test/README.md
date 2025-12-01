# Acromo Test Project

This project contains the automated tests for the Acromo application.

## Structure

- **Components**: Contains bUnit tests for Blazor components.
- **Pages**: Contains bUnit tests for full Pages (e.g., `Acromo.razor`).
- **Services**: Contains xUnit tests for services (e.g., Archetype detection).
- **MockData**: Contains text files for storing mock data.

## Running Tests

To run the tests, execute the following command from the root `Acromo` folder:

```bash
dotnet test
```

## Mock Data Format

### `MockData/PokepasteUrls.txt`
Paste Pokepaste URLs here, one per line.
Example:
```
https://pokepast.es/example1
https://pokepast.es/example2
```

### `MockData/Teams.txt`
Paste Pokemon teams in Showdown format here. Separate teams with a clear delimiter (e.g., `=== Team 1 ===`) if you want to store multiple, or just keep one for quick testing.

Example:
```
Pikachu @ Light Ball
Ability: Static
EVs: 252 Atk / 4 SpD / 252 Spe
Jolly Nature
- Volt Tackle
- Iron Tail
- Play Rough
- Fake Out
```
