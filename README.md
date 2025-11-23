# 🎨 Coloress - Blazor WebAssembly Boilerplate

A beautiful, modern Blazor WebAssembly application with stunning UI examples and comprehensive component demonstrations.

## 📋 Prerequisites

Before you begin, ensure you have the following installed:

- **.NET 8.0 SDK** or later
  - Download from: https://dotnet.microsoft.com/download
  - Verify installation: `dotnet --version`

## 🚀 Getting Started

Follow these steps to run the application:

### 1. Navigate to the Project Directory

```powershell
cd d:\Experiments\Coloress
```

### 2. Restore Dependencies

```powershell
dotnet restore
```

This command downloads all required NuGet packages specified in `Coloress.csproj`.

### 3. Build the Project

```powershell
dotnet build
```

This compiles the application and checks for any errors.

### 4. Run the Application

```powershell
dotnet run
```

Or use the watch mode for automatic reloading during development:

```powershell
dotnet watch run
```

### 5. Open in Browser

Once the application starts, you'll see output similar to:

```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5000
```

Open your browser and navigate to:
- **http://localhost:5000** (or the URL shown in your terminal)

You should see the Coloress home page with a beautiful gradient header and feature cards! 🎉

## 📁 Project Structure

```
Coloress/
├── Pages/                      # Razor pages/components
│   ├── Index.razor            # Home page with features
│   ├── Counter.razor          # Interactive counter example
│   ├── FetchData.razor        # Async data loading example
│   └── Components.razor       # Component showcase
├── Shared/                     # Shared layout components
│   ├── MainLayout.razor       # Main application layout
│   └── NavMenu.razor          # Navigation sidebar
├── wwwroot/                    # Static files
│   ├── css/
│   │   └── app.css            # Application styles
│   └── index.html             # HTML template
├── App.razor                   # Root component with routing
├── Program.cs                  # Application entry point
├── _Imports.razor             # Global using statements
├── Coloress.csproj            # Project file
├── .gitignore                 # Git ignore rules
└── README.md                  # This file
```

## 🎯 What's Included

### Pages & Examples

1. **Home Page (`/`)**
   - Beautiful hero section with gradient text
   - Feature cards showcasing Blazor benefits
   - Getting started guide
   - Code examples

2. **Counter (`/counter`)**
   - Interactive state management
   - Event handling (increment, decrement, reset)
   - Conditional rendering
   - Dynamic UI updates

3. **Fetch Data (`/fetchdata`)**
   - Async data loading simulation
   - Table rendering
   - LINQ operations
   - Statistics calculations
   - Loading states

4. **Components Showcase (`/components`)**
   - Two-way data binding
   - Form inputs
   - Conditional rendering
   - List operations (add, remove, clear)
   - Color picker
   - Timer with lifecycle management
   - Event handling

### UI Features

- 🌙 **Modern Dark Theme** - Beautiful dark mode with carefully chosen colors
- 🎨 **Gradient Accents** - Eye-catching gradients throughout
- ✨ **Smooth Animations** - Micro-interactions and transitions
- 📱 **Responsive Design** - Works on all screen sizes
- 🎯 **Component-Based** - Reusable and maintainable code
- ⚡ **Fast Performance** - Client-side rendering with WebAssembly

## 🧩 Key Blazor Concepts Demonstrated

### 1. Component Basics
```razor
@page "/example"

<h1>Hello, @name!</h1>

@code {
    private string name = "World";
}
```

### 2. Data Binding
```razor
<input @bind="userName" />
<p>Hello, @userName!</p>
```

### 3. Event Handling
```razor
<button @onclick="IncrementCount">Click me</button>

@code {
    private int count = 0;
    private void IncrementCount() => count++;
}
```

### 4. Conditional Rendering
```razor
@if (isVisible)
{
    <p>This is conditionally rendered!</p>
}
```

### 5. List Rendering
```razor
@foreach (var item in items)
{
    <li>@item</li>
}
```

### 6. Async Operations
```razor
@code {
    protected override async Task OnInitializedAsync()
    {
        data = await LoadDataAsync();
    }
}
```

## 🛠️ Development Commands

| Command | Description |
|---------|-------------|
| `dotnet restore` | Restore NuGet packages |
| `dotnet build` | Build the project |
| `dotnet run` | Run the application |
| `dotnet watch run` | Run with hot reload |
| `dotnet clean` | Clean build artifacts |
| `dotnet publish -c Release` | Create production build |

## 🎨 Customization

### Changing Colors

Edit `wwwroot/css/app.css` and modify the CSS variables in the `:root` selector:

```css
:root {
    --primary-color: #6366f1;      /* Main brand color */
    --secondary-color: #8b5cf6;    /* Secondary accent */
    --success-color: #10b981;      /* Success states */
    /* ... more colors ... */
}
```

### Adding New Pages

1. Create a new `.razor` file in the `Pages/` directory
2. Add the `@page` directive with your route
3. Add a link in `Shared/NavMenu.razor`

Example:
```razor
@page "/mypage"

<PageTitle>My Page</PageTitle>

<h1>My New Page</h1>

@code {
    // Your C# code here
}
```

## 📚 Learn More

- **Blazor Documentation**: https://docs.microsoft.com/aspnet/core/blazor/
- **Blazor Tutorial**: https://dotnet.microsoft.com/learn/aspnet/blazor-tutorial/intro
- **C# Documentation**: https://docs.microsoft.com/dotnet/csharp/
- **.NET Documentation**: https://docs.microsoft.com/dotnet/

## 🐛 Troubleshooting

### Port Already in Use
If you see an error about the port being in use, you can specify a different port:
```powershell
dotnet run --urls "http://localhost:5001"
```

### Build Errors
If you encounter build errors:
1. Clean the project: `dotnet clean`
2. Restore packages: `dotnet restore`
3. Rebuild: `dotnet build`

### Browser Not Opening
Manually navigate to the URL shown in the terminal output (usually `http://localhost:5000`).

## 📝 License

This is a boilerplate project - feel free to use it however you like!

## 🎉 Next Steps

Now that you have the boilerplate running:

1. Explore each example page to understand different Blazor concepts
2. Modify the existing components to see how changes affect the UI
3. Create your own components and pages
4. Customize the styling to match your brand
5. Add your own business logic and features

Happy coding! 🚀
