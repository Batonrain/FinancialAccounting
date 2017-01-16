using System.Collections.Generic;
using System.Linq;
using FinancialAccountingConstruction.DAL.Models.Stages;

namespace FinancialAccountingConstruction.DAL.Repository
{
    public class StagesRepository
    {
        private readonly FinancialAccountingDbContext _context;

        public StagesRepository()
        {
            _context = new FinancialAccountingDbContext();
        }

        public IEnumerable<Stage> GetStages(int contractorId, bool isInCash)
        {
            return _context.Stages.Where(c => c.ContractorId == contractorId && c.IsInCash == isInCash);
        }

        public void AddStage(Stage stage)
        {
            _context.Stages.Add(stage);
            _context.SaveChanges();
        }
    }
}
