﻿using System.Collections.Generic;
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

        public Stage GetStage(int stageId)
        {
            return _context.Stages.Single(c => c.Id == stageId);
        }

        public void UpdateStagePayment(Stage stage)
        {
            _context.Stages.Attach(stage);
            var entry = _context.Entry(stage);
            entry.Property(e => e.FinalPayment).IsModified = true;
            entry.Property(e => e.Prepayment).IsModified = true;
            _context.SaveChanges();
        }

        public void AddStage(Stage stage)
        {
            _context.Stages.Add(stage);
            _context.SaveChanges();
        }
    }
}
