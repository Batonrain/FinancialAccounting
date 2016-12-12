namespace FinancialAccountingConstruction.DAL.Repository
{
    public class PaymentsRepository
    {
         private readonly FinancialAccountingDbContext _context;

         public PaymentsRepository()
        {
            _context = new FinancialAccountingDbContext();
        }
    }
}
