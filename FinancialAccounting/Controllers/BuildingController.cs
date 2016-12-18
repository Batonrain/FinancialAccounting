using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using FinancialAccounting.Models.Buildings;
using FinancialAccounting.Models.Contractors;
using FinancialAccounting.Models.Contracts;
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

                model.BuildingMainInfo = ToBuildingViewModel(buildingObject);
                model.Contractors = _contractorsRepository.GetAllContractorsForBuilding(id);

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
                        _paymentsRepository.AddPlannedPayment(new PlannedPaymentsDate()
                        {
                            ContractorId = contractorId,
                            Date = DateTime.Parse(date),
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
                reqDates = requieredDates.Aggregate(reqDates, (current, plannedPaymentsDate) => current.Append(string.Format("{0}, ", plannedPaymentsDate)));
                contractorViewModel.RequriedDates = reqDates.ToString();
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
                    var parsedDate = DateTime.ParseExact(date, "MM/dd/yyyy", CultureInfo.InvariantCulture);
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