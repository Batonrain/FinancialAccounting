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

            var stages = _stagesRepository.GetStages(contractorId, isInCash);

            var viewModel = new ContractorViewModel
            {
                BuildingObjectId = contractorInfo.BuildingObjectId,
                Description = contractorInfo.Description,
                Id = contractorInfo.Id,
                Name = contractorInfo.Name,
                IsInCahs = isInCash,
                TypeText = isInCash ? "Наличная оплата" : "Безналичная оплата",
                Stages = stages.Select(StagesToStageViewModel).OrderBy(s=> s.Name).ToList(),
                PaymentsSummary = GetSummaryPayments(stages)  
            };

            if (stages.Count != 0)
            {
                viewModel.ActualisationDate = stages.Max(s => s.DateOfActualisation);
                viewModel.ActualisationPerson = stages.OrderByDescending(s => s.DateOfActualisation).FirstOrDefault().ActualizedBy;
            }

            return View(viewModel);
        }

        public ActionResult CreateStage(int contractorId, bool isInCash)
        {
            var types = new List<KeyValuePair<bool, string>>
            {
                new KeyValuePair<bool, string>(false, "Безналичная оплата"),
                new KeyValuePair<bool, string>(true, "Наличная оплата")
            };

            var contractors = _buildingObjectRepository.GetContractors();

            var contractorsSelect = contractors.Select(contractor => new KeyValuePair<int, string>(contractor.Id, contractor.Name)).ToList();

            var model = new CreateStageViewModel
            {
                ContractorId = contractorId,
                IsInCash = isInCash,
                Types = types,
                ContractorsSelect = contractorsSelect
            };

            return View(model);
        }

        public ActionResult UpdateStage(int stageId)
        {
            var currentStage = _stagesRepository.GetStage(stageId);

            var model = StagelToCreateStageViewMode(currentStage);

            var types = new List<KeyValuePair<bool, string>>
            {
                new KeyValuePair<bool, string>(false, "Безналичная оплата"),
                new KeyValuePair<bool, string>(true, "Наличная оплата")
            };

            model.Types = types;

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateStage(CreateStageViewModel updateStageViewModel)
        {
            if (ModelState.IsValid)
            {
                var stageToAdd = CreateStageViewModelToStageUpdate(updateStageViewModel);

                _stagesRepository.UpdateStage(stageToAdd);

                return RedirectToAction("ContractorStages", new { @contractorId = updateStageViewModel.ContractorId, @isInCash = updateStageViewModel.IsInCash });
            }

            return RedirectToAction("UpdateStage", new { stageId = updateStageViewModel.Id });
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
                    currentStage.FinalPaymentPayed = currentStage.FinalPaymentPayed + Convert.ToDecimal(model.PaymentSum);
                }
                if (model.PaymentType == 2)
                {
                    currentStage.PrepaymentPayed = currentStage.PrepaymentPayed + Convert.ToDecimal(model.PaymentSum);
                }

                _stagesRepository.UpdateStagePayment(currentStage);

                return Json("Success");
            }

            return Json("An Error Has occoured");
        }

        public ActionResult RemoveStage(int stageId)
        {
            var currentStage = _stagesRepository.GetStage(stageId);
            _stagesRepository.RemoveStage(currentStage);

            return RedirectToAction("ContractorStages", new { @contractorId = currentStage.ContractorId, @isInCash = currentStage.IsInCash });
        }

        private Status GetCurrentStatus(Stage stage)
        {
            var isPrepaymentPayed = (stage.Prepayment - stage.PrepaymentPayed) <= 0;
            var isFinalPaymentPayedPayed = (stage.FinalPayment - stage.FinalPaymentPayed) <= 0;

            var prepaymentTotalDaysGreen = stage.DateOfPrepayment != null && ((stage.DateOfPrepayment.Value - DateTime.Now).TotalDays > 10 && stage.Prepayment > 0) && !isPrepaymentPayed;
            var prepaymentTotalDaysYellow = stage.DateOfPrepayment != null && ((stage.DateOfPrepayment.Value - DateTime.Now).TotalDays < 10 && (stage.DateOfPrepayment.Value - DateTime.Now).TotalDays >= 4 && stage.Prepayment > 0) && !isPrepaymentPayed;
            var prepaymentTotalDaysRed = stage.DateOfPrepayment != null && ((stage.DateOfPrepayment.Value - DateTime.Now).TotalDays < 3 && stage.Prepayment > 0) && !isPrepaymentPayed;

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

            var finalPaymentTotalDaysGreen = stage.DateOfFinalPayment != null && ((stage.DateOfFinalPayment.Value - DateTime.Now).TotalDays > 10 && stage.FinalPayment > 0) && !isFinalPaymentPayedPayed;
            var finalPaymentTotalDaysYellow = stage.DateOfFinalPayment != null && ((stage.DateOfFinalPayment.Value - DateTime.Now).TotalDays < 10 && (stage.DateOfFinalPayment.Value - DateTime.Now).TotalDays >= 4 && stage.FinalPayment > 0) && !isFinalPaymentPayedPayed;
            var finalPaymentTotalDaysRed = stage.DateOfFinalPayment != null && ((stage.DateOfFinalPayment.Value - DateTime.Now).TotalDays < 3 && stage.FinalPayment > 0) && !isFinalPaymentPayedPayed;

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

                Prepayment = stage.Prepayment.ToString("C2"),
                FinalPayment = stage.FinalPayment.ToString("C2"),
                TotalPayment = stage.TotalPayment.ToString("C2"),

                PrepaymentPayed = stage.PrepaymentPayed.ToString("C2"),
                FinalPaymentPayed = stage.FinalPaymentPayed.ToString("C2"),
                TotalPayed = (stage.PrepaymentPayed + stage.FinalPaymentPayed).ToString("C2"),

                IsPrepaymentFullyPayed = stage.PrepaymentPayed >= stage.Prepayment,
                IsFinalPaymentFullyPayed = stage.FinalPaymentPayed >= stage.FinalPayment,

                SummOfPayment = (stage.Prepayment + stage.FinalPayment).ToString("C2"),

                DateOfEnding = stage.DateOfEnding.HasValue ? stage.DateOfEnding.Value.ToString("d") : string.Empty,
                DateOfFinalPayment = stage.DateOfFinalPayment.HasValue ? stage.DateOfFinalPayment.Value.ToString("d") : string.Empty,
                DateOfPrepayment = stage.DateOfPrepayment.HasValue ? stage.DateOfPrepayment.Value.ToString("d") : string.Empty,

                Status = GetCurrentStatus(stage),

                ActualizedBy = string.IsNullOrEmpty(stage.ActualizedBy) ? "Superuser" : stage.ActualizedBy
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
                TotalPayed = 0,
                DateOfEnding = stageViewModel.DateOfEnding == null ? (DateTime?)null : DateTime.Parse(stageViewModel.DateOfEnding),
                DateOfFinalPayment = stageViewModel.DateOfFinalPayment == null ? (DateTime?)null : DateTime.Parse(stageViewModel.DateOfFinalPayment),
                DateOfPrepayment = stageViewModel.DateOfPrepayment == null ? (DateTime?)null : DateTime.Parse(stageViewModel.DateOfPrepayment),
                ActualizedBy = User.Identity.Name
            };
        }

        private Stage CreateStageViewModelToStageUpdate(CreateStageViewModel stageViewModel)
        {
            var currentStageData = _stagesRepository.GetStage(stageViewModel.Id);
            var totalPayment = currentStageData.TotalPayment;
            if (currentStageData.Prepayment > stageViewModel.Prepayment)
            {
                totalPayment = totalPayment - (currentStageData.Prepayment - stageViewModel.Prepayment);
            }
            if (currentStageData.Prepayment < stageViewModel.Prepayment)
            {
                totalPayment = totalPayment + (stageViewModel.Prepayment - currentStageData.Prepayment);
            }

            if (currentStageData.FinalPayment > stageViewModel.FinalPayment)
            {
                totalPayment = totalPayment - (currentStageData.FinalPayment - stageViewModel.FinalPayment);
            }
            if (currentStageData.FinalPayment < stageViewModel.FinalPayment)
            {
                totalPayment = totalPayment + (stageViewModel.FinalPayment - currentStageData.FinalPayment);
            }

            return new Stage
            {
                Id = stageViewModel.Id,
                Name = stageViewModel.Name,
                ContractorId = stageViewModel.ContractorId,
                IsInCash = stageViewModel.IsInCash,
                Prepayment = stageViewModel.Prepayment,
                PrepaymentPayed = stageViewModel.PrepaymentPayed,
                FinalPayment = stageViewModel.FinalPayment,
                FinalPaymentPayed = stageViewModel.FinalPaymentPayed,
                TotalPayment = totalPayment,
                DateOfEnding = stageViewModel.DateOfEnding == null ? (DateTime?)null : DateTime.Parse(stageViewModel.DateOfEnding),
                DateOfFinalPayment = stageViewModel.DateOfFinalPayment == null ? (DateTime?)null : DateTime.Parse(stageViewModel.DateOfFinalPayment),
                DateOfPrepayment = stageViewModel.DateOfPrepayment == null ? (DateTime?)null : DateTime.Parse(stageViewModel.DateOfPrepayment)
            };
        }

        private CreateStageViewModel StagelToCreateStageViewMode(Stage stage)
        {
            return new CreateStageViewModel
            {
                Id = stage.Id,
                Name = stage.Name,
                ContractorId = stage.ContractorId,
                IsInCash = stage.IsInCash,
                Prepayment = stage.Prepayment,
                PrepaymentPayed = stage.PrepaymentPayed,
                FinalPayment = stage.FinalPayment,
                FinalPaymentPayed = stage.FinalPaymentPayed,
                DateOfEnding = stage.DateOfEnding.HasValue ? stage.DateOfEnding.Value.ToShortDateString() : string.Empty,
                DateOfFinalPayment = stage.DateOfFinalPayment.HasValue ? stage.DateOfFinalPayment.Value.ToShortDateString() : string.Empty,
                DateOfPrepayment = stage.DateOfPrepayment.HasValue ? stage.DateOfPrepayment.Value.ToShortDateString() : string.Empty
            };
        }

        private PaymentSummaryViewModel GetSummaryPayments(IList<Stage> stages)
        {
            var summByContract = stages.Sum(s => s.TotalPayment);
            var payedByContract = stages.Sum(s => s.PrepaymentPayed + s.FinalPaymentPayed);

            return new PaymentSummaryViewModel
            {
                SummByContract = summByContract.ToString("C2"),
                PayedByContract = payedByContract.ToString("C2"),
                NeedToPayByContract = (summByContract - payedByContract).ToString("C2")
            };
        }
    }
}