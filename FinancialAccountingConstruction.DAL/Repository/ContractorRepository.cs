using System.Collections.Generic;
using System.Linq;
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

        public int AddContractor(Contractor contract)
        {
            _context.Contractors.Add(contract);
            _context.SaveChanges();
            return contract.Id;
        }

        public void UpdateContractor(Contractor contract)
        {
            _context.Contractors.Attach(contract);
            var entry = _context.Entry(contract);
            entry.Property(e => e.Name).IsModified = true;
            entry.Property(e => e.Description).IsModified = true;
            entry.Property(e => e.TotalCostsInCash).IsModified = true;
            entry.Property(e => e.TotalCostsCashless).IsModified = true;
            _context.SaveChanges();
        }

        public IEnumerable<Contractor> GetAllContractorsForBuilding(int id)
        {
            return _context.Contractors.Where(c => c.BuildingObjectId == id).ToList();
        }
    }
}
