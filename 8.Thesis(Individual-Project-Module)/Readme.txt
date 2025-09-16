The current Source Code folder contains 4 folders:

Backend: This is the ASP.NET Core backend project.
Deployment Configurations: These are the YAML configuration files for Kubernetes.
Frontend: This is the React app for the platform.
Matching-Algorithm: This is the matching algorithm, which has been integrated into the backend but contains additional tests in this folder.
--------------------------
--------------------------
The platform is hosted on Kubernetes and may not be operational at the time of your review. 
If this is the case, kindly email me at ali.momenzadeh-kholenjani@city.ac.uk to request activation for your evaluation. 
(Please note that the platform has been redeployed due to a delay in the initial submission, which is why the platform's URL differs from the one shown in the Platform Walkthrough video.)


Platform URL: http://85.210.154.169/


Startup Founder Account:
Username: founder_test@example.com
Password: TestP@ss123

Skilled Individual Account: 
Username: individual_test@example.com
Password: TestP@ss123
--------------------------
--------------------------
Running the Platform Locally


Running the Frontend Locally:
1. Navigate to the folder \frontend\startupteam.
2. Open a terminal or console window in that directory.
3. Run the following command: npm start
This will fire up the frontend app on your localhost.

Please note: The **node_modules** folder has been deleted to reduce the size of the submission. To install the necessary dependencies for the frontend, navigate to the **\frontend\startupteam** folder and run `npm install` before starting the application.



Running the Backend Locally:
1. Navigate to the folder \backend\StartupTeam.
2. Open the solution file StartupTeam.sln in Visual Studio.
3. Click the Run button to start the backend.

By default, the backend will connect to the Azure SQL database hosted in the cloud. If you'd like to create and connect to a local SQL database, follow these steps:

Connecting to Local SQL Server Database
1. Navigate to \backend\StartupTeam\StartupTeam.Api and open the appsettings.json file.
2. Change the value of "ConnectionStrings:DefaultConnection" to your local SQL Server connection string.

Applying Entity Framework Core Migrations
Once your local database is set up, you need to apply the database migrations for each module:
1. Open the Command Prompt and navigate to the \backend\StartupTeam folder.
2. Run the following commands to apply the Entity Framework Core migrations for each module:

dotnet ef database update --project ./Modules/StartupTeam.Module.UserManagement --startup-project ./StartupTeam.Api --context UserManagementDbContext
dotnet ef database update --project ./Modules/StartupTeam.Module.PortfolioManagement --startup-project ./StartupTeam.Api --context PortfolioManagementDbContext
dotnet ef database update --project ./Modules/StartupTeam.Module.JobManagement --startup-project ./StartupTeam.Api --context JobManagementDbContext
dotnet ef database update --project ./Modules/StartupTeam.Module.TeamManagement --startup-project ./StartupTeam.Api --context TeamManagementDbContext
--------------------------
--------------------------
Running Tests

Running Backend Tests:
1. Navigate to the folder \backend\StartupTeam.
2. Open the solution file StartupTeam.sln in Visual Studio.
3. In the Solution Explorer, ensure that the project StartupTeam.Tests is selected.
3. From the Test menu, select Run All Tests.
4. All tests should pass, and you can view the results in the Test Explorer. You can find the Test Explorer under the View menu if it's not already open.


Running Frontend Tests:
1. Navigate to the folder \frontend.
2. Open the folder startupteam in Visual Studio Code.
3. From the Testing menu, select the option to run all tests. All tests should pass successfully.
--------------------------
--------------------------
Running Matching Algorithm (Standalone)

1. Navigate to the folder \matching-algorithm.
2. Open the Solution1.sln in Visual Studio.
3. Click the Run button, and the console application should display the results.
--------------------------
--------------------------
Usability Survey Link:
https://cityunilondon.eu.qualtrics.com/jfe/form/SV_4SCLEl5Jdc0EF7g