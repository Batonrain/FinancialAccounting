using System.Web.Mvc;
using FinancialAccounting.Models.Biuldings;
using FinancialAccountingConstruction.DAL.Models;
using FinancialAccountingConstruction.DAL.Repository;

namespace FinancialAccounting.Controllers
{
    public class BuildingController : Controller
    {
        private readonly BuildingObjectRepository _buildingObjectRepository;

        public BuildingController()
        {
            _buildingObjectRepository = new BuildingObjectRepository();
        }

        public ActionResult Index(int id)
        {
            var model = new ManageBuildingViewModel();
            var buildingObject = _buildingObjectRepository.GetObjectById(id);

            model.BuildingMainInfo = ToBuildingViewModel(buildingObject);

            return View(model);
        }

        [ValidateAntiForgeryToken]
        public ActionResult CreateObject()
        {
            return View();
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
    }
}