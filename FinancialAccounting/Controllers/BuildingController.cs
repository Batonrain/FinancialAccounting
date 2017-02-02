using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using FinancialAccounting.Models.Buildings;
using FinancialAccounting.Models.Contractors;
using FinancialAccounting.Models.Payments;
using FinancialAccountingConstruction.DAL.Models.Building;
using FinancialAccountingConstruction.DAL.Models.Contractors;
using FinancialAccountingConstruction.DAL.Repository;

namespace FinancialAccounting.Controllers
{
    [Authorize]
    public class BuildingController : Controller
    {
        private readonly BuildingObjectRepository _buildingObjectRepository;
        private readonly ContractorRepository _contractorsRepository;
        private readonly StagesRepository _stagesRepository;

        public BuildingController()
        {
            _buildingObjectRepository = new BuildingObjectRepository();
            _contractorsRepository = new ContractorRepository();
            _stagesRepository = new StagesRepository();
        }

        public ActionResult Index(int id)
        {
            if (id != 0)
            {
                var model = new ManageBuildingViewModel();
                var buildingObject = _buildingObjectRepository.GetObjectById(id);

                var contractorsId = _contractorsRepository.GetAllContractorsForBuilding(id).Select(s => s.Id);

                model.BuildingMainInfo = ToBuildingViewModel(buildingObject);
                model.Contractors = new List<ContractorViewModel>();

                foreach (var contractorId in contractorsId)
                {
                    model.Contractors.Add(GetContractorData(contractorId));
                }

                if (model.Contractors.Count != 0)
                {
                    model.ActualizationDate = model.Contractors.Max(c => c.ActualisationDate).ToString("d");
                    model.TotalPayment = GetTotalSummByContract(contractorsId);
                }
                else
                {
                    model.TotalPayment = new TotalPaymentViewModel();
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

        public ActionResult RemoveContractor(int contractorId)
        {
            _buildingObjectRepository.RemoveContractorById(contractorId);

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

                _contractorsRepository.AddContractor(contractorModel);

                return RedirectToAction("Index", new { @id = contractor.BuildingObjectId });
            }

            return View(contractor);
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
            _contractorsRepository.UpdateContractor(obj);

            return RedirectToAction("Index", "Building", new { id = contractor.BuildingObjectId });
        }

        public ContractorViewModel GetContractorData(int contractorId)
        {
            var contractorObject = _buildingObjectRepository.GetContractorById(contractorId);

            var contractorViewModel = ToContractorPaymentsViewModel(contractorObject);
            var allStages = _stagesRepository.GetAllStages(contractorId).ToList();

            var summByContract = allStages.Sum(s => s.TotalPayment);
            var payedByContract = allStages.Sum(s => s.PrepaymentPayed + s.FinalPaymentPayed);

            contractorViewModel.PaymentsSummary = new PaymentSummaryViewModel
            {
                SummByContract = summByContract.ToString("C2"),
                PayedByContract = payedByContract.ToString("C2"),
                NeedToPayByContract = (summByContract - payedByContract).ToString("C2")
            };

            if (allStages.Count == 0)
                return contractorViewModel;

            contractorViewModel.ActualisationDate = allStages.Max(s => s.DateOfActualisation);

            return contractorViewModel;
        }

        public TotalPaymentViewModel GetTotalSummByContract(IEnumerable<int> contractorIds)
        {
            decimal summByContract = 0;
            decimal payedByContract = 0;
            decimal needToPayByContract = 0;

            foreach (var allStages in contractorIds.Select(contractorId => _stagesRepository.GetAllStages(contractorId).ToList()))
            {
                summByContract = summByContract + allStages.Sum(s => s.TotalPayment);

                payedByContract = payedByContract + allStages.Sum(s => s.PrepaymentPayed + s.FinalPaymentPayed);
            }

            needToPayByContract = needToPayByContract + (summByContract - payedByContract);

            return new TotalPaymentViewModel
            {
                SummByContract = summByContract.ToString("C2"),
                PayedByContract = payedByContract.ToString("C2"),
                NeedToPayByContract = needToPayByContract.ToString("C2")
            };
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

        private ContractorViewModel ToContractorPaymentsViewModel(Contractor contractor)
        {
            return new ContractorViewModel()
            {
                Id = contractor.Id,
                BuildingObjectId = contractor.BuildingObjectId,
                Name = contractor.Name,
                Description = contractor.Description,
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
            };
        }

        private Contractor ToUpdateContractorObj(UpdateContractorViewModel contractorViewModel)
        {
            return new Contractor
            {
                Id = contractorViewModel.Id,
                Name = contractorViewModel.Name,
                Description = contractorViewModel.Descriptions,
                BuildingObjectId = contractorViewModel.BuildingObjectId
            };
        }

        #endregion
    }
}