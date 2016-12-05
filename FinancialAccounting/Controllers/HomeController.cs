using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using FinancialAccounting.Models.Biuldings;
using FinancialAccountingConstruction.DAL.Models;
using FinancialAccountingConstruction.DAL.Repository;

namespace FinancialAccounting.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var rr = new BuildingObjectRepository();
            var er = rr.GetAllBuildingObjects();
            return View(ToBuildingList(er));
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
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