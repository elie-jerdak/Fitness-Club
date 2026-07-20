Scaffold command example (open CMD inside the FitnessClub.Api project location first):

dotnet ef dbcontext scaffold "Name=DefaultConnection" Microsoft.EntityFrameworkCore.SqlServer --output-dir ..\FitnessClub.Core\Models --context FitnessClubContext --context-dir ..\FitnessClub.Core\ -f

Since the Database in Render's free tier has a lifetime what to do:
1. delete the old database in render
2. create a new database in render
3. create a new connection to that database in pgAdmin
4. restore using the local backup in the backup folder
5. options needed:
	Clean before restore: No
	Create database: No
	No owner: Yes
	Do NOT use "Only data"

