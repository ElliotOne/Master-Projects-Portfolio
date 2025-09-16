# Thesis (Individual-Project-Module)

## Project Structure

The current source code folder contains the following directories:

- **Backend**: ASP.NET Core backend project.  
- **Deployment Configurations**: YAML configuration files for Kubernetes.  
- **Frontend**: React app for the platform.  
- **Matching-Algorithm**: Matching algorithm integrated into the backend, with additional tests in this folder.  

## Test Accounts

**Startup Founder Account**  
- Username: founder_test@example.com  
- Password: TestP@ss123  

**Skilled Individual Account**  
- Username: individual_test@example.com  
- Password: TestP@ss123  

## Running the Platform Locally

### Frontend

1. Navigate to the folder `\frontend\startupteam`.  
2. Open a terminal or console in that directory.  
3. Install dependencies:  
   ```bash
   npm instal
4. Start the frontend application:
   ```bash
   npm start
The app will run on your localhost.

> **Note:** The `node_modules` folder has been removed to reduce submission size. Ensure you run `npm install` before starting the frontend.

### Backend

1. Navigate to the folder `\backend\StartupTeam`.
2. Open the solution file `StartupTeam.sln` in Visual Studio.
3. Click the Run button to start the backend.
By default, the backend connects to the Azure SQL database hosted in the cloud.

#### Connecting to a Local SQL Server Database
1. Navigate to `\backend\StartupTeam\StartupTeam.Api` and open `appsettings.json`.
2. Change the value of `"ConnectionStrings:DefaultConnection"` to your local SQL Server connection string.

#### Applying Entity Framework Core Migrations
1. Open Command Prompt and navigate to `\backend\StartupTeam`.
2. Run the following commands for each module:
   ```bash
   dotnet ef database update --project ./Modules/StartupTeam.Module.UserManagement --startup-project ./StartupTeam.Api --context UserManagementDbContext
   
   dotnet ef database update --project ./Modules/StartupTeam.Module.PortfolioManagement --startup-project ./StartupTeam.Api --context PortfolioManagementDbContext
   
   dotnet ef database update --project ./Modules/StartupTeam.Module.JobManagement --startup-project ./StartupTeam.Api --context JobManagementDbContext
   
   dotnet ef database update --project ./Modules/StartupTeam.Module.TeamManagement --startup-project ./StartupTeam.Api --context TeamManagementDbContext

### Running Tests

#### Backend Tests
1. Navigate to `\backend\StartupTeam`.
2. Open `StartupTeam.sln` in Visual Studio.
3. Select the `StartupTeam.Tests` project in Solution Explorer.
4. From the Test menu, select Run All Tests.
5. All tests should pass. Results can be viewed in the Test Explorer.

#### Frontend Tests
1. Navigate to the folder `\frontend\startupteam` in Visual Studio Code.
2. From the Testing menu, select Run All Tests.
3. All tests should pass successfully.

#### Matching Algorithm (Standalone)
1. Navigate to the folder `\matching-algorithm`.
2. Open the solution file `Solution1.sln` in Visual Studio.
3. Click the Run button. The console application will display the results.
