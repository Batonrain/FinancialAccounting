using System;
using System.Collections.Generic;
using System.Data.Entity;
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

        public IEnumerable<Stage> GetAllStages(int contractorId)
        {
            return _context.Stages.Where(c => c.ContractorId == contractorId);
        }

        public IEnumerable<Stage> GetStages(int contractorId, bool isInCash)
        {
            return _context.Stages.Where(c => c.ContractorId == contractorId && c.IsInCash == isInCash);
        }

        public Stage GetStage(int stageId)
        {
            return _context.Stages.Single(c => c.Id == stageId);
        }

        public void UpdateStagePayment(Stage stage)
        {
            stage.DateOfActualisation = DateTime.Now;
            _context.Stages.Attach(stage);
            var entry = _context.Entry(stage);
            entry.Property(e => e.FinalPayment).IsModified = true;
            entry.Property(e => e.Prepayment).IsModified = true;
            entry.Property(e => e.DateOfActualisation).IsModified = true;
            _context.SaveChanges();
        }

        public void AddStage(Stage stage)
        {
            stage.DateOfActualisation = DateTime.Now;
            _context.Stages.Add(stage);
            _context.SaveChanges();
        }

        public void UpdateStage(Stage stage)
        {
            stage.DateOfActualisation = DateTime.Now;

            var entry = _context.Entry(stage);

            if (entry.State == EntityState.Detached)
            {
                var set = _context.Set<Stage>();
                var attachedEntity = set.Local.SingleOrDefault(e => e.Id == stage.Id);  // You need to have access to key

                if (attachedEntity != null)
                {
                    var attachedEntry = _context.Entry(attachedEntity);
                    attachedEntry.CurrentValues.SetValues(stage);
                }
                else
                {
                    entry.State = EntityState.Modified; // This should attach entity
                }
            }

            //entry.State = EntityState.Modified;

            //entry.Property(e => e.FinalPayment).IsModified = true;
            //entry.Property(e => e.Prepayment).IsModified = true;
            //entry.Property(e => e.TotalPayment).IsModified = true;
            //entry.Property(e => e.DateOfActualisation).IsModified = true;
            //entry.Property(e => e.DateOfEnding).IsModified = true;
            //entry.Property(e => e.DateOfFinalPayment).IsModified = true;
            //entry.Property(e => e.DateOfPrepayment).IsModified = true;
            //entry.Property(e => e.Name).IsModified = true;

            _context.SaveChanges();
        }
    }
}
