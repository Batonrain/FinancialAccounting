using System.Data.Entity;
using FinancialAccountingConstruction.DAL.Models;

namespace FinancialAccountingConstruction.DAL
{
    public class FinancialAccountingDbContext : DbContext
    {
        public FinancialAccountingDbContext()
            : base("name=DefaultConnection")
        { }

        public DbSet<BuildingObject> BuildingObjects { get; set; }
    }
}
