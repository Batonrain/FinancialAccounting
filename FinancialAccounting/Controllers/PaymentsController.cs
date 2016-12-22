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

        public ActionResult ContractorPayments(int contractorId, bool type)
        {
            var contractorObject = _buildingObjectRepository.GetContractorById(contractorId);

            var contractorViewModel = ToContractorPaymentsViewModel(contractorObject);
            contractorViewModel.TypeText = type ? "Безналичная оплата" : "Наличная оплата";
            contractorViewModel.Type = type;

            contractorViewModel.PaymentsSummary = new PaymentSummaryViewModel();
            contractorViewModel.Payments = new List<PaymentViewModel>();

            if (_paymentsRepository.PaymentsAreExist(contractorId))
            {
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
                                                        allPayments.Where(
                                                            p => !p.IsInCash && p.ContractorId == contractorId)
                                                            .Sum(p => p.Summ);

                    contractorViewModel.PaymentsSummary.InCashNeedToPayByContract = inCashNeedToPayByContract;
                    contractorViewModel.PaymentsSummary.InCashPayedByContract =
                        allPayments.Where(p => p.IsInCash && p.ContractorId == contractorId).Sum(p => p.Summ);
                    contractorViewModel.PaymentsSummary.InCashSummByContract = contractorObject.TotalCostsInCash;

                    contractorViewModel.PaymentsSummary.InCashlessNeedToPayByContract = inCashlessNeedToPayByContract;
                    contractorViewModel.PaymentsSummary.InCashlessPayedByContract =
                        allPayments.Where(p => !p.IsInCash && p.ContractorId == contractorId).Sum(p => p.Summ);
                    contractorViewModel.PaymentsSummary.InCashlessSummByContract = contractorObject.TotalCostsCashless;

                    foreach (var payment in allPayments.Where(p => p.IsInCash == type))
                    {
                        contractorViewModel.Payments.Add(new PaymentViewModel
                        {
                            Date = payment.Date,
                            Summ = payment.Summ,
                            Name = payment.Name,
                            Executor = User.Identity.Name,
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
            }

            ViewBag.ShowPlannedPaymentsButton =
                    _paymentsRepository.GetPlannedPaymentsDatesByContractorId(contractorId).Any();

            return View(contractorViewModel);
        }

        public ActionResult CreatePayment(int contractorId)
        {
            var model = new CreatePaymentViewModel
            {
                ContractorId = contractorId,
                TypesOfPayments = CreatePaymentDictionary()
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

        public ActionResult CreatePlannedPayment(int contractorId)
        {
            var pd = _paymentsRepository.GetPlannedPaymentsDatesByContractorId(contractorId);

            var paymentDates = pd.ToDictionary(plannedPaymentsDate => plannedPaymentsDate.Date.ToShortDateString(), plannedPaymentsDate => plannedPaymentsDate.Id);

            var model = new CreatePlannedPaymentViewModel
            {
                ContractorId = contractorId,
                TypesOfPayments = CreatePaymentDictionary(),
                DatesOfPayments = paymentDates
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreatePlannedPayment(CreatePlannedPaymentViewModel paymentViewModel)
        {
            var newPayment = ToPlannedPaymentObject(paymentViewModel);
            _paymentsRepository.AddPayment(newPayment);
            _paymentsRepository.UpdatePlannedPaymentToPayed(paymentViewModel.PlannedPaymentId);

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

        private Payment ToPlannedPaymentObject(CreatePlannedPaymentViewModel paymentViewModel)
        {
            return new Payment
            {
                ContractorId = paymentViewModel.ContractorId,
                IsInCash = paymentViewModel.IsInCash,
                Name = paymentViewModel.Name,
                Summ = paymentViewModel.Summ,
                Date = DateTime.Now,
                ExecutorId = 1
            };
        }

        private Dictionary<string, bool> CreatePaymentDictionary()
        {
            var result = new Dictionary<string, bool>();

            if (User.IsInRole("Admin"))
            {
                result.Add("Наличные", true);
                result.Add("Безнал", false);
            }

            if (User.IsInRole("CashAccounting"))
            {
                result.Add("Наличные", true);
            }

            if (User.IsInRole("NonCashAccounting"))
            {
                result.Add("Безнал", true);
            }

            return result;
        }
    }
}