using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using FinancialAccounting.Models.Buildings;
using FinancialAccounting.Models.Contractors;
using FinancialAccounting.Models.Contracts;
using FinancialAccountingConstruction.DAL.Models.Building;
using FinancialAccountingConstruction.DAL.Models.Contracts;
using FinancialAccountingConstruction.DAL.Models.Payments;
using FinancialAccountingConstruction.DAL.Repository;

namespace FinancialAccounting.Controllers
{
    [Authorize]
    public class BuildingController : Controller
    {
        private readonly BuildingObjectRepository _buildingObjectRepository;
        private readonly ContractRepository _contractsRepository;
        private readonly ContractorRepository _contractorRepository;

        public BuildingController()
        {
            _buildingObjectRepository = new BuildingObjectRepository();
            _contractsRepository = new ContractRepository();
            _contractorRepository = new ContractorRepository();
        }

        public ActionResult Index(int id)
        {
            if (id != 0)
            {
                var model = new ManageBuildingViewModel();
                var buildingObject = _buildingObjectRepository.GetObjectById(id);
                var contracts = _contractsRepository.GetAllContractsForBuilding(id);

                model.BuildingMainInfo = ToBuildingViewModel(buildingObject);
                var list = contracts.Select(s => s.ContractorId).ToList();
                model.Contractors = _contractsRepository.GetAllContractorsRelatedToTheContracts(list);

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

        public ActionResult CreateContractForObject(int buildingId)
        {
            var contractors = _contractorRepository.GetAllContractors().ToList();
            var contractorsList = contractors.Select(contractor => new SelectListItem
            {
                Text = contractor.Name, Value = contractor.Id.ToString()
            }).ToList();

            var viewModel = new CreateContractViewModel()
            {
                BuildingObjectId = buildingId,
                Contractors = contractorsList
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateContractForObject(CreateContractViewModel contract)
        {
            if (ModelState.IsValid)
            {
                var contractorModel = ToContractorModel(contract);
                _contractsRepository.AddContract(contractorModel);

                return RedirectToAction("Index", new { @id = contract.BuildingObjectId });
            }

            return View(contract);
        }

        public ActionResult UpdateContractor(int contractorId)
        {
            //var contractorObject = _buildingObjectRepository.GetContractorById(contractorId);
            //var contractorViewModel = ToUpdateContractorViewModel(contractorObject);

            return View();
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult UpdateContractor(UpdateContractorViewModel contractor)
        //{
        //    var obj = ToUpdateContractorObj(contractor);
        //    _contractorRepository.UpdateContractor(obj);

        //    return RedirectToAction("Index", "Building", new { @id  = contractor.BuildingObjectId});
        //}

        public ActionResult CreatePaymentForContractor(int contractorId)
        {
            var list = new List<SelectListItem>
            {
                new SelectListItem {Text = "08.2016", Selected = true, Value = "1"},
                new SelectListItem {Text = "09.2016", Selected = true, Value = "2"},
                new SelectListItem {Text = "10.2016", Selected = true, Value = "3"},
                new SelectListItem {Text = "11.2016", Selected = true, Value = "4"}
            };

            var contractorObject = _buildingObjectRepository.GetContractorById(contractorId);
            var contractorViewModel = new CreatePaymentViewModel
            {
                ContractorId = contractorObject.Id,
                ContractorName = contractorObject.Name,
                IsPlanned = false,
                Summ = 0
                //,
                //PeriodsOfPayments = list
            };

            return View(contractorViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreatePaymentForContractor(ContractorViewModel contractor)
        {
            return View();
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

        private BuildingObject ToBuildingObjectModel(BuildingViewModel buildingObject)
        {
            return new BuildingObject
            {
                Id = buildingObject.Id,
                Name = buildingObject.Name,
                Notes = buildingObject.Description
            };
        }

        private Contract ToContractorModel(CreateContractViewModel contractsViewModel)
        {
            return new Contract
            {
                Number = contractsViewModel.Number,
                Description = contractsViewModel.Descriptions,
                BuildingObjectId = contractsViewModel.BuildingObjectId,
                ContractorId = contractsViewModel.ContractorId
            };
        }

        //private ContractorViewModel ToContractorViewModel(Contractor contractorObject)
        //{
        //    return new ContractorViewModel
        //    {
        //        Id = contractorObject.Id,
        //        Name = contractorObject.Name,
        //        ContractDescriptions = contractorObject.ContractDescriptions,
        //        BuildingObjectId = contractorObject.BuildingObjectId,
        //        ContractNumbers = contractorObject.ContractNumbers,
        //        TimingOfWorks = contractorObject.TimingOfWorks.ToShortDateString(),
        //        PaymentDay = string.Format("{0} число", contractorObject.PaymentDay),
        //        TotalCosts = contractorObject.TotalCosts,
        //        Notes = contractorObject.Notes
        //    };
        //}

        //private UpdateContractorViewModel ToUpdateContractorViewModel(Contractor contractorObject)
        //{
        //    return new UpdateContractorViewModel
        //    {
        //        Id = contractorObject.Id,
        //        Name = contractorObject.Name,
        //        ContractDescriptions = contractorObject.ContractDescriptions,
        //        BuildingObjectId = contractorObject.BuildingObjectId,
        //        ContractNumbers = contractorObject.ContractNumbers,
        //        TimingOfWorks = contractorObject.TimingOfWorks,
        //        PaymentDay = contractorObject.PaymentDay,
        //        TotalCosts = contractorObject.TotalCosts,
        //        Notes = contractorObject.Notes
        //    };
        //}

        //private Contractor ToUpdateContractorObj(UpdateContractorViewModel contractorViewModel)
        //{
        //    return new Contractor
        //    {
        //        Id = contractorViewModel.Id,
        //        Name = contractorViewModel.Name,
        //        ContractDescriptions = contractorViewModel.ContractDescriptions,
        //        BuildingObjectId = contractorViewModel.BuildingObjectId,
        //        ContractNumbers = contractorViewModel.ContractNumbers,
        //        TimingOfWorks = contractorViewModel.TimingOfWorks,
        //        PaymentDay = contractorViewModel.PaymentDay,
        //        TotalCosts = contractorViewModel.TotalCosts,
        //        Notes = contractorViewModel.Notes
        //    };
        //}

        #endregion

        #region Payments

        private void AddPayments(int date, DateTime timingOfWorks, int contractorId)
        {
            var dayNumber = 1;
            if (date >= 31)
            {
                dayNumber = 30;
            }
            var todayDate = DateTime.Now;
            var months = MonthsBetween(timingOfWorks, DateTime.Now);
            var dates = new List<PlannedPaymentsDate>();

        }

        static IEnumerable<DateTime> MonthsBetween(DateTime d0, DateTime d1)
        {
            return Enumerable.Range(0, (d1.Year - d0.Year) * 12 + (d1.Month - d0.Month + 1))
                             .Select(m => new DateTime(d0.Year, d0.Month, 1).AddMonths(m));
        }

        #endregion
    }
}