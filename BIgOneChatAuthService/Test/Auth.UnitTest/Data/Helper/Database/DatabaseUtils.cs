using Data.Persistence;
using Microsoft.EntityFrameworkCore;


namespace Auth.UnitTest.Data.Helper.Database
{
    public static class DatabaseUtils
    {
        public static DatabaseContext CreateDbContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<DatabaseContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;

            var dbContext = new DatabaseContext(options);

            dbContext.Database.EnsureCreated();

            return dbContext;
        }

        public static void ClearDatabase(DatabaseContext dbContext)
        {
            dbContext.RemoveRange(dbContext.Tokens);
            dbContext.SaveChanges();
        }
    }
}
