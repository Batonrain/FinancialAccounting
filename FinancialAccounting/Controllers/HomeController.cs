using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using FinancialAccounting.Models.Biuldings;
using FinancialAccountingConstruction.DAL.Models.Building;
using FinancialAccountingConstruction.DAL.Repository;

namespace FinancialAccounting.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly BuildingObjectRepository _buildingObjectRepository;

        public HomeController()
        {
            _buildingObjectRepository = new BuildingObjectRepository();
        }

        public ActionResult Index()
        {
            var objects = _buildingObjectRepository.GetAllBuildingObjects();
            return View(ToBuildingList(objects));
        }

        private List<BuildingListItem> ToBuildingList(IEnumerable<BuildingObject> objects)
        {
            return objects.Select(o => new BuildingListItem()
            {
                Id = o.Id,
                Name = o.Name,
                Notes = o.Notes
            }).ToList();
        }
    }
}