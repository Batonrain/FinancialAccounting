using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using FinancialAccounting.Models.Buildings;
using FinancialAccounting.Models.Contractors;
using FinancialAccounting.Models.Payments;
using FinancialAccountingConstruction.DAL.Models.Stages;
using FinancialAccountingConstruction.DAL.Repository;

namespace FinancialAccounting.Controllers
{
    [Authorize]
    public class StagesController : Controller
    {
        private readonly BuildingObjectRepository _buildingObjectRepository;
        private readonly StagesRepository _stagesRepository;

        public StagesController()
        {
            _buildingObjectRepository = new BuildingObjectRepository();
            _stagesRepository = new StagesRepository();
        }

        public ActionResult ContractorStages(int contractorId, bool isInCash)
        {
            var contractorInfo = _buildingObjectRepository.GetContractorById(contractorId);
            var stages = _stagesRepository.GetStages(contractorId, isInCash).ToList();
            var viewModel = new ContractorViewModel
            {
                BuildingObjectId = contractorInfo.BuildingObjectId,
                Description = contractorInfo.Description,
                Id = contractorInfo.Id,
                Name = contractorInfo.Name,
                IsInCahs = isInCash,
                TypeText = isInCash ? "Наличная оплата" : "Безналичная оплата",
                Stages = stages.Select(StagesToStageViewModel).ToList(),
                PaymentsSummary = GetSummaryPayments(stages)
            };

            if (stages.Count != 0)
            {
                viewModel.ActualisationDate = stages.Max(s => s.DateOfActualisation);
            }

            return View(viewModel);
        }

        public ActionResult CreateStage(int contractorId, bool isInCash)
        {
            var model = new CreateStageViewModel
            {
                ContractorId = contractorId,
                IsInCash = isInCash
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateStage(CreateStageViewModel createStageViewModel)
        {
            if (ModelState.IsValid)
            {
                var stageToAdd = CreateStageViewModelToStage(createStageViewModel);

                _stagesRepository.AddStage(stageToAdd);

                return RedirectToAction("ContractorStages", new { @contractorId = createStageViewModel.ContractorId, @isInCash = createStageViewModel.IsInCash });
            }

            return RedirectToAction("CreateStage", new { @contractorId = createStageViewModel.ContractorId, @isInCash = createStageViewModel.IsInCash });
        }

        [HttpPost]
        public ActionResult AddPayment(PaymentViewModel model)
        {
            if (model != null)
            {
                var currentStage = _stagesRepository.GetStage(model.StageId);

                if (model.PaymentType == 1)
                {
                    currentStage.FinalPayment = currentStage.FinalPayment - Convert.ToDecimal(model.PaymentSum);
                    _stagesRepository.UpdateStagePayment(currentStage);
                }
                if (model.PaymentType == 2)
                {
                    currentStage.Prepayment = currentStage.Prepayment - Convert.ToDecimal(model.PaymentSum);
                    _stagesRepository.UpdateStagePayment(currentStage);
                }

                return Json("Success");
            }

            return Json("An Error Has occoured");
        }

        private Status GetCurrentStatus(Stage stage)
        {
            var prepaymentTotalDaysGreen = (stage.DateOfPrepayment - DateTime.Now).TotalDays > 10 && stage.Prepayment > 0;
            var prepaymentTotalDaysYellow = (stage.DateOfPrepayment - DateTime.Now).TotalDays < 10 && (stage.DateOfPrepayment - DateTime.Now).TotalDays >= 4 && stage.Prepayment > 0;
            var prepaymentTotalDaysRed = (stage.DateOfPrepayment - DateTime.Now).TotalDays < 3 && stage.Prepayment > 0;

            if (prepaymentTotalDaysGreen)
            {
                return Status.Green;
            }

            if (prepaymentTotalDaysYellow)
            {
                return Status.Yellow;
            }

            if (prepaymentTotalDaysRed)
            {
                return Status.Red;
            }

            var finalPaymentTotalDaysGreen = (stage.DateOfFinalPayment - DateTime.Now).TotalDays > 10 && stage.FinalPayment > 0;
            var finalPaymentTotalDaysYellow = (stage.DateOfFinalPayment - DateTime.Now).TotalDays < 10 && (stage.DateOfFinalPayment - DateTime.Now).TotalDays >= 4 && stage.FinalPayment > 0;
            var finalPaymentTotalDaysRed = (stage.DateOfFinalPayment - DateTime.Now).TotalDays < 3 && stage.FinalPayment > 0;

            if (finalPaymentTotalDaysGreen)
            {
                return Status.Green;
            }

            if (finalPaymentTotalDaysYellow)
            {
                return Status.Yellow;
            }

            if (finalPaymentTotalDaysRed)
            {
                return Status.Red;
            }

            return Status.White;
        }

        private StageViewModel StagesToStageViewModel(Stage stage)
        {
            return new StageViewModel
            {
                Id = stage.Id,
                ContractorId = stage.ContractorId,
                Name = stage.Name,
                IsInCash = stage.IsInCash,
                Prepayment = stage.Prepayment.ToString("C0"),
                FinalPayment = stage.FinalPayment.ToString("C0"),
                TotalPayment = stage.TotalPayment.ToString("C0"),
                SummOfPayment = (stage.Prepayment + stage.FinalPayment).ToString("C0"),
                DateOfEnding = stage.DateOfEnding.ToString("d"),
                DateOfFinalPayment = stage.DateOfFinalPayment.ToString("d"),
                DateOfPrepayment = stage.DateOfPrepayment.ToString("d"),
                Status = GetCurrentStatus(stage)
            };
        }

        private Stage CreateStageViewModelToStage(CreateStageViewModel stageViewModel)
        {
            return new Stage
            {
                Id = stageViewModel.Id,
                Name = stageViewModel.Name,
                ContractorId = stageViewModel.ContractorId,
                IsInCash = stageViewModel.IsInCash,
                Prepayment = stageViewModel.Prepayment,
                FinalPayment = stageViewModel.FinalPayment,
                TotalPayment = stageViewModel.Prepayment + stageViewModel.FinalPayment,
                DateOfEnding = DateTime.Parse(stageViewModel.DateOfEnding),
                DateOfFinalPayment = DateTime.Parse(stageViewModel.DateOfFinalPayment),
                DateOfPrepayment = DateTime.Parse(stageViewModel.DateOfPrepayment)
            };
        }

        private PaymentSummaryViewModel GetSummaryPayments(IEnumerable<Stage> stages)
        {
            return new PaymentSummaryViewModel
            {
                SummByContract = stages.Sum(s => s.TotalPayment).ToString("C0"),
                PayedByContract = (stages.Sum(s => s.TotalPayment) - stages.Sum(s => s.Prepayment + s.FinalPayment)).ToString("C0"),
                NeedToPayByContract = stages.Sum(s => s.Prepayment + s.FinalPayment).ToString("C0")
            };
        }

        //private Dictionary<string, bool> CreatePaymentDictionary()
        //{
        //    var result = new Dictionary<string, bool>();

        //    if (User.IsInRole("Admin"))
        //    {
        //        result.Add("Наличные", true);
        //        result.Add("Безнал", false);
        //    }

        //    if (User.IsInRole("CashAccounting"))
        //    {
        //        result.Add("Наличные", true);
        //    }

        //    if (User.IsInRole("NonCashAccounting"))
        //    {
        //        result.Add("Безнал", true);
        //    }

        //    return result;
        //}
    }
}