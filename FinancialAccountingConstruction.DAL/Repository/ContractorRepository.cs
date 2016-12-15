using System.Collections.Generic;
using System.Linq;
using FinancialAccountingConstruction.DAL.Models.Contracts;

namespace FinancialAccountingConstruction.DAL.Repository
{
    public class ContractorRepository
    {
        private readonly FinancialAccountingDbContext _context;

        public ContractorRepository()
        {
            _context = new FinancialAccountingDbContext();
        }

        public IEnumerable<Contractor> GetAllContractors()
        {
            return _context.Contractors.ToList();
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

        public void UpdateContract(Contractor contract)
        {
            _context.Contractors.Attach(contract);
            var entry = _context.Entry(contract);
            entry.Property(e => e.Name).IsModified = true;
            entry.Property(e => e.Notes).IsModified = true;
            _context.SaveChanges();
        }

        public void RemoveContract(int id)
        {
            var toDelete = _context.Contractors.Single(c => c.Id == id);
            _context.Contractors.Remove(toDelete);
            _context.SaveChanges();
        }
    }
}
