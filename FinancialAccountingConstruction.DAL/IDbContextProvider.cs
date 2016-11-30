using System;

namespace FinancialAccountingConstruction.DAL
{
    public interface IDbContextProvider : IDisposable
    {
        FinancialAccountingDbContext GetDbContext();
    }
}
