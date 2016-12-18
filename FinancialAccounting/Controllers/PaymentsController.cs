using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using FinancialAccounting.Models.Contractors;
using FinancialAccounting.Models.Payments;
using FinancialAccountingConstruction.DAL.Models.Contractors;
using FinancialAccountingConstruction.DAL.Models.Payments;
using FinancialAccountingConstruction.DAL.Repository;

namespace FinancialAccounting.Controllers
{
    [Authorize]
    public class PaymentsController : Controller
    {
        private readonly BuildingObjectRepository _buildingObjectRepository;
        private readonly PaymentsRepository _paymentsRepository;

        public PaymentsController()
        {
            _buildingObjectRepository = new BuildingObjectRepository();
            _paymentsRepository = new PaymentsRepository();
        }

        public ActionResult Index(int contractorId)
        {
            return View();
        }

        public ActionResult ContractorPayments(int contractorId)
        {
            var contractorObject = _buildingObjectRepository.GetContractorById(contractorId);

            var contractorViewModel = ToContractorPaymentsViewModel(contractorObject);

            var allPayments = _paymentsRepository.GetPaymentsForContractor(contractorId).ToList();

            var needToPayByContract = contractorObject.TotalCostsCashless + contractorObject.TotalCostsInCash -
                                          allPayments.Where(p => p.ContractorId == contractorId).Sum(p => p.Summ);

            contractorViewModel.PaymentsSummary = new PaymentSummaryViewModel()
            {
                NeedToPayByContract = needToPayByContract,
                PayedByContract = allPayments.Where(p => p.ContractorId == contractorId).Sum(p => p.Summ),
                SummByContract = contractorObject.TotalCostsCashless + contractorObject.TotalCostsInCash
            };

            contractorViewModel.Payments = new List<PaymentViewModel>();

            if (allPayments.Any())
            {
                var inCashNeedToPayByContract = contractorObject.TotalCostsInCash -
                                                allPayments.Where(p => p.IsInCash && p.ContractorId == contractorId)
                                                    .Sum(p => p.Summ);

                var inCashlessNeedToPayByContract = contractorObject.TotalCostsCashless -
                                                allPayments.Where(p => !p.IsInCash && p.ContractorId == contractorId)
                                                    .Sum(p => p.Summ);

                contractorViewModel.PaymentsSummary.InCashNeedToPayByContract = inCashNeedToPayByContract;
                contractorViewModel.PaymentsSummary.InCashPayedByContract =
                    allPayments.Where(p => p.IsInCash && p.ContractorId == contractorId).Sum(p => p.Summ);
                contractorViewModel.PaymentsSummary.InCashSummByContract = contractorObject.TotalCostsInCash;

                contractorViewModel.PaymentsSummary.InCashlessNeedToPayByContract = inCashlessNeedToPayByContract;
                contractorViewModel.PaymentsSummary.InCashlessPayedByContract =
                    allPayments.Where(p => !p.IsInCash && p.ContractorId == contractorId).Sum(p => p.Summ);
                contractorViewModel.PaymentsSummary.InCashlessSummByContract = contractorObject.TotalCostsCashless;

                foreach (var payment in allPayments)
                {
                    contractorViewModel.Payments.Add(new PaymentViewModel
                    {
                        Date = payment.Date,
                        Type = payment.IsInCash ? "Наличный" : "Безналичный",
                        Summ = payment.Summ,
                        Name = payment.Name,
                        Executor = "Alex",
                        Id = payment.Id
                    });
                }

                var lastDate = allPayments.OrderByDescending(t => t.Date).FirstOrDefault().Date;

                ViewBag.LastUpdate = string.Format("Последнее обновление: {0}", lastDate);
            }
            else
            {
                ViewBag.LastUpdate = string.Empty;
            }

            ViewBag.Title = string.Format("Платежи подрядчика '{0}'", contractorViewModel.Name);

            return View(contractorViewModel);
        }

        public ActionResult CreatePayment(int contractorId)
        {
            var model = new CreatePaymentViewModel
            {
                ContractorId = contractorId
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreatePayment(CreatePaymentViewModel paymentViewModel)
        {
            var newPayment = ToPaymentObject(paymentViewModel);
            _paymentsRepository.AddPayment(newPayment);

            return RedirectToAction("ContractorPayments", new { @contractorId = paymentViewModel.ContractorId });
        }

        private ContractorPaymentsViewModel ToContractorPaymentsViewModel(Contractor contractor)
        {
            return new ContractorPaymentsViewModel()
            {
                Id = contractor.Id,
                BuildingObjectId = contractor.BuildingObjectId,
                Name = contractor.Name,
                Description = contractor.Description,
                TotalCostsCashless = contractor.TotalCostsCashless,
                TotalCostsInCash = contractor.TotalCostsInCash
            };
        }

        private Payment ToPaymentObject(CreatePaymentViewModel paymentViewModel)
        {
            return new Payment
            {
                ContractorId = paymentViewModel.ContractorId,
                IsInCash = true,
                Name = paymentViewModel.Name,
                Summ = paymentViewModel.Summ,
                Date = DateTime.Now,
                ExecutorId = 1
            };
        }
    }
}