using System.Web.Mvc;
using FinancialAccountingConstruction.DAL.Repository;

namespace FinancialAccounting.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var rr = new BuildingObjectRepository();
            var er = rr.GetAllBuildingObjects();
            return View();
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
    }
}