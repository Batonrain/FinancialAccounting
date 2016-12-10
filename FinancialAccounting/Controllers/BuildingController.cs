using System.Linq;
using System.Web.Mvc;
using FinancialAccounting.Models.Biuldings;
using FinancialAccounting.Models.Contractors;
using FinancialAccountingConstruction.DAL.Models.Building;
using FinancialAccountingConstruction.DAL.Models.Contractors;
using FinancialAccountingConstruction.DAL.Repository;

namespace FinancialAccounting.Controllers
{
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
                _contractorRepository.AddContractor(ToContractorModel(contractor));

                return RedirectToAction("Index", new { @id = contractor.BuildingObjectId });
            }

            return View(contractor);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateObject(BuildingViewModel buildingModel)
        {
            var buildingObject = new BuildingObject()
            {
                Name = buildingModel.Name,
                Notes = buildingModel.Description
            };

            _buildingObjectRepository.AddBuildingObject(buildingObject);

            return RedirectToAction("Index");
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

        private BuildingObject ToBuildingViewModel(BuildingViewModel buildingObject)
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
    }
}