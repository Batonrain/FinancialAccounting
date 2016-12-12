using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using FinancialAccounting.Models.Biuldings;
using FinancialAccounting.Models.Contractors;
using FinancialAccountingConstruction.DAL.Models.Building;
using FinancialAccountingConstruction.DAL.Models.Contractors;
using FinancialAccountingConstruction.DAL.Repository;

namespace FinancialAccounting.Controllers
{
    [Authorize]
    public class BuildingController : Controller
    {
        private readonly BuildingObjectRepository _buildingObjectRepository;
        private readonly ContractorRepository _contractorRepository;

        public BuildingController()
        {
            _buildingObjectRepository = new BuildingObjectRepository();
            _contractorRepository = new ContractorRepository();
        }

        public ActionResult Index(int id)
        {
            if (id != 0)
            {
                var model = new ManageBuildingViewModel();
                var buildingObject = _buildingObjectRepository.GetObjectById(id);
                var contractors = _contractorRepository.GetBuildingContractors(id).Select(contr => new ShortContractorInfo
                {
                    Id = contr.Id,
                    Name = contr.Name
                });

                model.BuildingMainInfo = ToBuildingViewModel(buildingObject);
                model.Contractors = contractors;

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
            var viewModel = new CreateContractorViewModel
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
                _contractorRepository.AddContractor(contractorModel);

                return RedirectToAction("Index", new { @id = contractor.BuildingObjectId });
            }

            return View(contractor);
        }

        public ActionResult ContractorPayments(int contractorId, bool? paymentsType)
        {
            var contractorObject = _buildingObjectRepository.GetContractorById(contractorId);
            var contractorViewModel = ToContractorViewModel(contractorObject);

            return View(contractorViewModel);
        }

        public ActionResult UpdateContractor(int contractorId)
        {
            var contractorObject = _buildingObjectRepository.GetContractorById(contractorId);
            var contractorViewModel = ToUpdateContractorViewModel(contractorObject);

            return View(contractorViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateContractor(UpdateContractorViewModel contractor)
        {
            var obj = ToUpdateContractorObj(contractor);
            _contractorRepository.UpdateContractor(obj);

            return RedirectToAction("Index", "Building", new { @id  = contractor.BuildingObjectId});
        }

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
                Summ = 0,
                PeriodsOfPayments = list
            };

            return View(contractorViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreatePaymentForContractor(ContractorViewModel contractor)
        {
            return View();
        }

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

        private Contractor ToContractorModel(CreateContractorViewModel contractorViewModel)
        {
            return new Contractor
            {
                Name = contractorViewModel.Name,
                ContractDescriptions = contractorViewModel.ContractDescriptions,
                BuildingObjectId = contractorViewModel.BuildingObjectId,
                ContractNumbers = contractorViewModel.ContractNumbers,
                TimingOfWorks = contractorViewModel.TimingOfWorks,
                PaymentDay = contractorViewModel.PaymentDay,
                TotalCosts = contractorViewModel.TotalCosts,
                Notes = contractorViewModel.Notes
            };
        }

        private ContractorViewModel ToContractorViewModel(Contractor contractorObject)
        {
            return new ContractorViewModel
            {
                Id = contractorObject.Id,
                Name = contractorObject.Name,
                ContractDescriptions = contractorObject.ContractDescriptions,
                BuildingObjectId = contractorObject.BuildingObjectId,
                ContractNumbers = contractorObject.ContractNumbers,
                TimingOfWorks = contractorObject.TimingOfWorks.ToShortDateString(),
                PaymentDay = string.Format("{0} число", contractorObject.PaymentDay),
                TotalCosts = contractorObject.TotalCosts,
                Notes = contractorObject.Notes
            };
        }

        private UpdateContractorViewModel ToUpdateContractorViewModel(Contractor contractorObject)
        {
            return new UpdateContractorViewModel
            {
                Id = contractorObject.Id,
                Name = contractorObject.Name,
                ContractDescriptions = contractorObject.ContractDescriptions,
                BuildingObjectId = contractorObject.BuildingObjectId,
                ContractNumbers = contractorObject.ContractNumbers,
                TimingOfWorks = contractorObject.TimingOfWorks,
                PaymentDay = contractorObject.PaymentDay,
                TotalCosts = contractorObject.TotalCosts,
                Notes = contractorObject.Notes
            };
        }

        private Contractor ToUpdateContractorObj(UpdateContractorViewModel contractorViewModel)
        {
            return new Contractor
            {
                Id = contractorViewModel.Id,
                Name = contractorViewModel.Name,
                ContractDescriptions = contractorViewModel.ContractDescriptions,
                BuildingObjectId = contractorViewModel.BuildingObjectId,
                ContractNumbers = contractorViewModel.ContractNumbers,
                TimingOfWorks = contractorViewModel.TimingOfWorks,
                PaymentDay = contractorViewModel.PaymentDay,
                TotalCosts = contractorViewModel.TotalCosts,
                Notes = contractorViewModel.Notes
            };
        }
    }
}