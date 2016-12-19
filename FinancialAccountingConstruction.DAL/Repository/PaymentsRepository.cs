﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

        public void AddPlannedPayment(PlannedPaymentsDate date)
        {
            _context.PlannedPaymentsDates.Add(date);
            _context.SaveChanges();
        }
        public void AddPayment(Payment payment)
        {
            _context.Payments.Add(payment);
            _context.SaveChanges();
        }

        public List<PlannedPaymentsDate> GetPlannedPaymentsDatesByContractorId(int id)
        {
            return _context.PlannedPaymentsDates.Where(p => p.ContractorId == id && !p.IsPayed).ToList();
        }

        public IEnumerable<Payment> GetPaymentsForContractor(int id)
        {
            return _context.Payments.Where(p => p.ContractorId == id);
        }

        public void Remove(PlannedPaymentsDate date)
        {
            _context.PlannedPaymentsDates.Remove(date);
            _context.SaveChanges();
        }

        public void RemoveAllForContractor(int id)
        {
            var dates = _context.PlannedPaymentsDates.Where(p => p.ContractorId == id && !p.IsPayed);

            _context.PlannedPaymentsDates.RemoveRange(dates);
            _context.SaveChanges();
        }
    }
}
