using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Chipsoft.Assignments.EPDConsole.Data;

namespace EPDConsole.Tests
{
    internal static class TestHelpers
    {
        public static (EPDDbContext db, SqliteConnection conn) CreateInMemory()
        {
            var conn = new SqliteConnection("DataSource=:memory:");
            conn.Open();
            var options = new DbContextOptionsBuilder<EPDDbContext>()
                .UseSqlite(conn)
                .Options;
            var db = new EPDDbContext(options);
            db.Database.EnsureCreated();
            return (db, conn);
        }
    }
}
