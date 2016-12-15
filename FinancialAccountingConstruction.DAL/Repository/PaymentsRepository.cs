using System.Collections.Generic;
using FinancialAccountingConstruction.DAL.Models.Payments;

namespace FinancialAccountingConstruction.DAL.Repository
{
    public class PaymentsRepository
    {
        private readonly FinancialAccountingDbContext _context;

        public PaymentsRepository()
        {
            _context = new FinancialAccountingDbContext();
        }

        public void AddPlannedPayments(IEnumerable<PlannedPaymentsDate> dates)
        {
            _context.PlannedPaymentsDates.AddRange(dates);
            _context.SaveChanges();
        }
    }
}
