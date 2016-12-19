using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using FinancialAccounting.Models.Buildings;
using FinancialAccounting.Models.Contractors;
using FinancialAccounting.Models.Contracts;
using FinancialAccounting.Models.Payments;
using FinancialAccountingConstruction.DAL.Models.Building;
using FinancialAccountingConstruction.DAL.Models.Contractors;
using FinancialAccountingConstruction.DAL.Models.Payments;
using FinancialAccountingConstruction.DAL.Repository;

namespace FinancialAccounting.Controllers
{
    [Authorize]
    public class BuildingController : Controller
    {
        private readonly BuildingObjectRepository _buildingObjectRepository;
        private readonly ContractorRepository _contractorsRepository;
        private readonly PaymentsRepository _paymentsRepository;

        public BuildingController()
        {
            _buildingObjectRepository = new BuildingObjectRepository();
            _contractorsRepository = new ContractorRepository();
            _paymentsRepository = new PaymentsRepository();
        }

        public ActionResult Index(int id)
        {
            if (id != 0)
            {
                var model = new ManageBuildingViewModel();
                var buildingObject = _buildingObjectRepository.GetObjectById(id);

                var contractorsId = _contractorsRepository.GetAllContractorsForBuilding(id).Select(s => s.Id);

                model.BuildingMainInfo = ToBuildingViewModel(buildingObject);
                model.Contractors = new List<ContractorPaymentsViewModel>();

                foreach (var contractorId in contractorsId)
                {
                    model.Contractors.Add(GetContractorData(contractorId));
                }

                return View(model);
            }

            return RedirectToAction("Index", "Home");
        }

        public ActionResult CreateObject()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateObject(BuildingViewModel buildingModel)
        {
            var buildingObject = new BuildingObject
            {
                Name = buildingModel.Name,
                Notes = buildingModel.Description
            };

            _buildingObjectRepository.AddBuildingObject(buildingObject);

            return RedirectToAction("Index", "Home");
        }

        public ActionResult UpdateObject(int buildingId)
        {
            var obj = _buildingObjectRepository.GetObjectById(buildingId);

            var viewModel = ToBuildingViewModel(obj);

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateObject(BuildingViewModel buildingModel)
        {
            var obj = ToBuildingObjectModel(buildingModel);

            _buildingObjectRepository.UpdateBuildingObject(obj);

            return RedirectToAction("Index", "Home");
        }

        public ActionResult RemoveObject(int buildingId)
        {
            _buildingObjectRepository.RemoveBuildingObjectById(buildingId);

            return RedirectToAction("Index", "Home");
        }

        public ActionResult CreateContractorForObject(int buildingId)
        {
            var viewModel = new CreateContractorViewModel()
            {
                BuildingObjectId = buildingId
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateContractorForObject(CreateContractorViewModel contractor)
        {
            if (ModelState.IsValid)
            {
                var contractorModel = ToContractorModel(contractor);
                var contractorId = _contractorsRepository.AddContractor(contractorModel);

                if (!string.IsNullOrEmpty(contractor.RequriedDates))
                {
                    var listOfDates = contractor.RequriedDates.Split(',');
                    foreach (var date in listOfDates)
                    {
                        var parsedDate = DateTime.ParseExact(date, "MM/dd/yyyy", CultureInfo.InvariantCulture);
                        _paymentsRepository.AddPlannedPayment(new PlannedPaymentsDate()
                        {
                            ContractorId = contractorId,
                            Date = parsedDate,
                            IsPayed = false
                        });
                    }
                }

                return RedirectToAction("Index", new { @id = contractor.BuildingObjectId });
            }

            return View(contractor);
        }

        public ActionResult UpdateContractor(int contractorId)
        {
            var contractorObject = _buildingObjectRepository.GetContractorById(contractorId);
            var requieredDates = _paymentsRepository.GetPlannedPaymentsDatesByContractorId(contractorId).ToList();
            var contractorViewModel = ToUpdateContractorViewModel(contractorObject);

            var reqDates = new StringBuilder();
            if (requieredDates.Any())
            {
                foreach (var date in requieredDates)
                {
                    var dateToFormat = String.Format("{0:MM/dd/yyyy}", date.Date).Replace('.', '/');
                    reqDates = reqDates.AppendFormat("{0}, ", dateToFormat);
                }
                var final = reqDates.Remove(reqDates.ToString().LastIndexOf(','), 2).ToString();
                contractorViewModel.RequriedDates = final;
            }

            return View(contractorViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateContractor(UpdateContractorViewModel contractor)
        {
            var obj = ToUpdateContractorObj(contractor);
            _contractorsRepository.UpdateContractor(obj);

            if (!string.IsNullOrEmpty(contractor.RequriedDates))
            {
                _paymentsRepository.RemoveAllForContractor(obj.Id);
                var listOfDates = contractor.RequriedDates.Split(',');
                foreach (var date in listOfDates)
                {
                    var parsedDate = DateTime.ParseExact(date.Trim(), "MM/dd/yyyy", CultureInfo.InvariantCulture);
                    _paymentsRepository.AddPlannedPayment(new PlannedPaymentsDate()
                    {
                        ContractorId = obj.Id,
                        Date = parsedDate,
                        IsPayed = false
                    });
                }
            }

            return RedirectToAction("ContractorPayments", "Payments", new { @contractorId = contractor.Id });
        }

        public ContractorPaymentsViewModel GetContractorData(int contractorId)
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

                var plannedPaymentsForContractor =
                    _paymentsRepository.GetPlannedPaymentsDatesByContractorId(contractorId);

                if (plannedPaymentsForContractor.Any())
                {
                    contractorViewModel.PlannedDate = plannedPaymentsForContractor.OrderBy(pd => pd.Date).FirstOrDefault().Date;
                    var daysLeft = (contractorViewModel.PlannedDate - DateTime.Now).TotalDays;

                    if (daysLeft > 7)
                    {
                        contractorViewModel.Status = Status.Green;
                    }

                    if (daysLeft > 3 && daysLeft < 7)
                    {
                        contractorViewModel.Status = Status.Yellow;
                    }

                    if (daysLeft < 3)
                    {
                        contractorViewModel.Status = Status.Yellow;
                    }
                }
                else
                {
                    contractorViewModel.Status = Status.White;
                }

            }
            return contractorViewModel;
        }

        #region DataTransformers

        private BuildingViewModel ToBuildingViewModel(BuildingObject buildingObject)
        {
            return new BuildingViewModel
            {
                Id = buildingObject.Id,
                Name = buildingObject.Name,
                Description = buildingObject.Notes
            };
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

        private BuildingObject ToBuildingObjectModel(BuildingViewModel buildingObject)
        {
            return new BuildingObject
            {
                Id = buildingObject.Id,
                Name = buildingObject.Name,
                Notes = buildingObject.Description
            };
        }

        private Contractor ToContractorModel(CreateContractorViewModel contractorViewModel)
        {
            return new Contractor
            {
                Name = contractorViewModel.Name,
                Description = contractorViewModel.Descriptions,
                BuildingObjectId = contractorViewModel.BuildingObjectId,
                TotalCostsCashless = Convert.ToDecimal(contractorViewModel.TotalCostsCashless),
                TotalCostsInCash = Convert.ToDecimal(contractorViewModel.TotalCostsInCash)
            };
        }

        private UpdateContractorViewModel ToUpdateContractorViewModel(Contractor contractorObject)
        {
            return new UpdateContractorViewModel
            {
                Id = contractorObject.Id,
                Name = contractorObject.Name,
                Descriptions = contractorObject.Description,
                BuildingObjectId = contractorObject.BuildingObjectId,
                TotalCostsCashless = contractorObject.TotalCostsCashless,
                TotalCostsInCash = contractorObject.TotalCostsInCash
            };
        }

        private Contractor ToUpdateContractorObj(UpdateContractorViewModel contractorViewModel)
        {
            return new Contractor
            {
                Id = contractorViewModel.Id,
                Name = contractorViewModel.Name,
                Description = contractorViewModel.Descriptions,
                BuildingObjectId = contractorViewModel.BuildingObjectId,
                TotalCostsCashless = Convert.ToDecimal(contractorViewModel.TotalCostsCashless),
                TotalCostsInCash = Convert.ToDecimal(contractorViewModel.TotalCostsInCash)
            };
        }

        #endregion
    }
}