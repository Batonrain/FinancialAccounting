using System.Collections.Generic;
using System.Linq;
using FinancialAccountingConstruction.DAL.Models.Building;
using FinancialAccountingConstruction.DAL.Models.Contractors;

namespace FinancialAccountingConstruction.DAL.Repository
{
    public class ContractorRepository
    {
        private readonly FinancialAccountingDbContext _context;

        public ContractorRepository()
        {
            _context = new FinancialAccountingDbContext();
        }

        public Contractor GetContractorById(int id)
        {
            return _context.Contractors.Single(contr => contr.Id == id);
        }

        public void AddContractor(Contractor contractor)
        {
            _context.Contractors.Add(contractor);
            _context.SaveChanges();
        }

        public IEnumerable<Contractor> GetBuildingContractors(int buildingId)
        {
            return _context.Contractors.Where(contr => contr.BuildingObjectId == buildingId);
        }

        public void UpdateContractor(Contractor contractor)
        {
            _context.Contractors.Attach(contractor);
            var entry = _context.Entry(contractor);
            entry.Property(e => e.Name).IsModified = true;
            entry.Property(e => e.Notes).IsModified = true;
            entry.Property(e => e.PaymentDay).IsModified = true;
            entry.Property(e => e.ContractDescriptions).IsModified = true;
            entry.Property(e => e.ContractNumbers).IsModified = true;
            entry.Property(e => e.TimingOfWorks).IsModified = true;
            entry.Property(e => e.TotalCosts).IsModified = true;
            _context.SaveChanges();
        }
    }
}
