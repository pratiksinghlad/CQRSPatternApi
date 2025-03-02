To add migrations:
+ Open Package Manager Console
+ Set default project to: CQRSPattern.Infrastructure.Persistence
+ Execute following command: Add-Migration NameOfMigration -StartupProject CQRSPattern.Application.Api -Context RealDbContext

To remove last migration:
+ Set default project to: CQRSPattern.Infrastructure.Persistence
+ Execute following command:  Remove-Migration -Context RealDbContext -StartupProject CQRSPattern.Application.Api

--After doing a migration remove the "RenameTable" properties
--When dropping a table, the migrator does not mention from which schema it has to drop the table. 
  You have to manually type from which schema you wish to drop the table.
  
eg.      migrationBuilder.DropTable(
                name: "Employee",
                schema: "");

                This is the same for renaming our tables.
                You have to manually say which tables you want to rename for a given schema.