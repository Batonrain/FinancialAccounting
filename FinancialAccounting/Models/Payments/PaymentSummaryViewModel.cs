using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FinancialAccounting.Models.Payments
{
    public class PaymentSummaryViewModel
    {
        public decimal SummByContract { get; set; }
        public decimal PayedByContract { get; set; }
        public decimal NeedToPayByContract { get; set; }

        public decimal InCashSummByContract { get; set; }
        public decimal InCashPayedByContract { get; set; }
        public decimal InCashNeedToPayByContract { get; set; }

        public decimal InCashlessSummByContract { get; set; }
        public decimal InCashlessPayedByContract { get; set; }
        public decimal InCashlessNeedToPayByContract { get; set; }
    }
}