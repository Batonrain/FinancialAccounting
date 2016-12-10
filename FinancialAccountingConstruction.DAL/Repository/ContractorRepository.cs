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

        public void AddContractor(Contractor contractor)
        {
            _context.Contractors.Add(contractor);
            _context.SaveChanges();
        }

        public IEnumerable<Contractor> GetBuildingContractors(int buildingId)
        {
            return _context.Contractors.Where(contr => contr.BuildingObjectId == buildingId);
        }
    }
}
