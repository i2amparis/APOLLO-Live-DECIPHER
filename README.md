# Topsis Tool
- Moderators can create/configure workspaces with a questionnaire.
- Guest accounts can vote/answer these questionnaires.
- System can calculate results based on topsis algorithm
- Moderators can finalize a workspace/questionnaire and publish the results

## Developer
### Architecture/Design
- We use the hexagonal-like architecture
- We use the mediator pattern
- We use dependency injection
- We use .net 8

### Database
- You can use the following database engines (mysql, mariadb, postgresql), you can configure that option in appsettings.json under DatabaseSettings/Engine
- System uses EF core as a data access mapper
  - Some reports needs special treatment in specific databases (like double quotting for postgresql)
- We use 2 connection strings:
  - For the runtime user.
  - For the system user that does the migrations when the process starts.
- These connection strings can be set either in appsettings.json or as environment variables like:
  - DatabaseSettings__Runtime__Password:<mypassword> (double underscore needed in order settings to work in win or linux)

### Bootstrap
- You can define your admin's user email in appsettings.json or as an env variable "AdminSettings__Email"
- You can define your admin's user password in appsettings.json or as an env variable "AdminSettings__InitialPassword" (you can later change it through email and the forgot password procedure)

### Encryption
- We currently encrypt passwords with the help of the database, where we hold the key rings there
- Other ways are already implemented (google provider, file-based)

### Testing
- Some basic tests are added to ensure algorithm correctness and behavior logic

### Evolution
- You can write code and add data tables and/or columns (Code-First approach)
	- WorkspaceDbContext class is responsible of handing the code/data mapping
- You can add a new column with these steps:
	- Add "Education" property/column to ApplicationUser class
		- Handle the creation/update behavior
	- Open a terminal to the root folder of Topsis.Adapters.Database proj
	- Execute > dotnet ef migrations add add_user_education (this needs to have dotnet ef core tools installed, with default version 3.1 
		- https://learn.microsoft.com/en-us/ef/core/cli/dotnet)
		- This cmd will use the connection string inside DatabaseFactory class in order to check the current state of the database and
		  it will create a .cs file under "Migrations" folder
	- (Optional) If you changed your mind you can remove the added .cs file by executing cmd > dotnet ef migrations remove
	- (Optional) You can test this into a new or existing database with the connection string hardcoded or using the DeployConnectionString env var, 
		executing the cmd > dotnet ef database update
	- Test it (Debug-F5, this will run the migration process in MigrationHostedService)

