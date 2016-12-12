using System.Web.Mvc;
using FinancialAccountingConstruction.DAL.Repository;

namespace FinancialAccounting.Controllers
{
    [Authorize]
    public class PaymentsController : Controller
    {
        private readonly BuildingObjectRepository _buildingObjectRepository;
        private readonly ContractorRepository _contractorRepository;

        public PaymentsController()
        {
            _buildingObjectRepository = new BuildingObjectRepository();
            _contractorRepository = new ContractorRepository();
        }

        public ActionResult Index(int contractorId, bool? isInCash)
        {
            return View();
        }
	}
}