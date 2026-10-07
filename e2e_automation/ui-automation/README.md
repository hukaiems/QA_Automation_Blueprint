# E2E Automation Project

Tech Stack: Selenium + Extent Reports + NUnit (Test Framework)

## Prerequisites

- [.NET 10.0 SDK](https://dotnet.microsoft.com/download) or later.
- Web browsers (Chrome, Firefox, Edge) installed on the execution machine

## Configuration

Application and test settings are managed in `appsettings.json`. You can configure:

- **AppSettings**: `BaseUrl`, `ImplicitWait`, `ExplicitWait`
- **Browser**: `Type` (e.g., Chrome, Firefox), `Headless` (true/false), and remote execution settings (`RunRemote`, `RemoteUrl`).

## Project Structure

- `Base/`: Base classes and setups for tests.
- `Tests/`: Contains all NUnit test classes (e.g., `SampleTest.cs`).
- `Utils/`: Utility helpers, logging, and reporting logic (includes NPOI for Excel processing).
- `Utils/Route.cs`: Defined Url for POM
- `appsettings.json`: Main configuration file.

## How to Run

1. **CLI:**

   ```bash
   dotnet restore
   dotnet test
   # Run tests in Edge                                              
   dotnet test --TestRunParameters.Parameter(name="Browser",value="Edge")  
   Admin__Username="..." Admin__Password="...."  dotnet test Team2QA.Automation.Tests/Team2QA.Automation.Tests.csproj --filter "Category=Regression" --logger "junit;LogFileName=regression-${BROWSER_NAME}.xml" --results-directory ./TestResults 
   ```
2. *Linting,Formating Code*

   ```bash
   dotnet format
   ```

## 🛠️ Local Setup

After cloning this repository, the .NET will auto run the following command to enable our automated branch and commit linter:

```bash
git config core.hooksPath .githooks
```
