using System.Data.Entity;
using FinancialAccountingConstruction.DAL.Models.Building;
using FinancialAccountingConstruction.DAL.Models.Contractors;
using FinancialAccountingConstruction.DAL.Models.Stages;

namespace FinancialAccountingConstruction.DAL
{
    public class FinancialAccountingDbContext : DbContext
    {
        public FinancialAccountingDbContext()
            : base("name=DefaultConnection")
        { }

        public DbSet<BuildingObject> BuildingObjects { get; set; }
        public DbSet<Contractor> Contractors { get; set; }
        public DbSet<Stage> Stages { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            Database.SetInitializer<FinancialAccountingDbContext>(null);
            base.OnModelCreating(modelBuilder);
        }
    }
}
