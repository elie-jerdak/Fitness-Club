Scaffold command example (open CMD inside the FitnessClub.Api project location first):

dotnet ef dbcontext scaffold "Name=DefaultConnection" Microsoft.EntityFrameworkCore.SqlServer --output-dir ..\FitnessClub.Core\Models --context FitnessClubContext --context-dir ..\FitnessClub.Core\ -f