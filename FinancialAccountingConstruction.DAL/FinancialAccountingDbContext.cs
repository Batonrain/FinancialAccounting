﻿using System.Data.Entity;
using FinancialAccountingConstruction.DAL.Models;
using FinancialAccountingConstruction.DAL.Models.Building;
using FinancialAccountingConstruction.DAL.Models.Contractors;
using FinancialAccountingConstruction.DAL.Models.Payments;

namespace FinancialAccountingConstruction.DAL
{
    public class FinancialAccountingDbContext : DbContext
    {
        public FinancialAccountingDbContext()
            : base("name=DefaultConnection")
        { }

        public DbSet<BuildingObject> BuildingObjects { get; set; }
        public DbSet<Contractor> Contractors { get; set; }
        public DbSet<PlannedPaymentsDate> PlannedPaymentsDates { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            Database.SetInitializer<FinancialAccountingDbContext>(null);
            base.OnModelCreating(modelBuilder);
        }
    }
}
