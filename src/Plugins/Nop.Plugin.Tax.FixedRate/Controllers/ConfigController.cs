﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using Nop.Plugin.Tax.FixedRate.Models;
using Nop.Core.Infrastructure;
using Nop.Services.Configuration;
using Nop.Services.Logging;
using Nop.Services.Tax;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Telerik.Web.Mvc;

namespace Nop.Plugin.Tax.FixedRate.Controllers
{
    [AdminAuthorize]
    public class ConfigController : Controller
    {
        private readonly ITaxCategoryService _taxCategoryService;
        private readonly ISettingService _settingService;

        public ConfigController(ITaxCategoryService taxCategoryService, ISettingService settingService)
        {
            this._taxCategoryService = taxCategoryService;
            this._settingService = settingService;
        }

        public ActionResult Configure()
        {
            var tmp = new List<FixedTaxRateModel>();
            foreach (var taxCategory in _taxCategoryService.GetAllTaxCategories())
                tmp.Add(new FixedTaxRateModel()
                {
                    TaxCategoryId = taxCategory.Id,
                    TaxCategoryName = taxCategory.Name,
                    Rate = GetTaxRate(taxCategory.Id)
                });

            var gridModel = new GridModel<FixedTaxRateModel>
            {
                Data = tmp,
                Total = tmp.Count
            };

            return View("Nop.Plugin.Tax.FixedRate.Views.Config.Configure", gridModel);
        }

        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult Configure(GridCommand command)
        {
            var tmp = new List<FixedTaxRateModel>();
            foreach (var taxCategory in _taxCategoryService.GetAllTaxCategories())
                tmp.Add(new FixedTaxRateModel()
                {
                    TaxCategoryId = taxCategory.Id,
                    TaxCategoryName = taxCategory.Name,
                    Rate = GetTaxRate(taxCategory.Id)
                });

            var tmp2 = tmp.ForCommand(command);
            var gridModel = new GridModel<FixedTaxRateModel>
            {
                Data = tmp2,
                Total = tmp2.Count()
            };
            return new JsonResult
            {
                Data = gridModel
            };
        }
        [GridAction(EnableCustomBinding = true)]
        public ActionResult TaxRateUpdate(FixedTaxRateModel model, GridCommand command)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Configure");
            }

            int taxCategoryId = model.TaxCategoryId;
            decimal rate = model.Rate;

            _settingService.SetSetting(string.Format("Tax.TaxProvider.FixedRate.TaxCategoryId{0}", taxCategoryId), rate);

            var tmp = new List<FixedTaxRateModel>();
            foreach (var taxCategory in _taxCategoryService.GetAllTaxCategories())
                tmp.Add(new FixedTaxRateModel()
                {
                    TaxCategoryId = taxCategory.Id,
                    TaxCategoryName = taxCategory.Name,
                    Rate = GetTaxRate(taxCategory.Id)
                });

            var tmp2 = tmp.ForCommand(command);
            var gridModel = new GridModel<FixedTaxRateModel>
            {
                Data = tmp2,
                Total = tmp2.Count()
            };
            return new JsonResult
            {
                Data = gridModel
            };
        }

        [NonAction]
        protected decimal GetTaxRate(int taxCategoryId)
        {
            decimal rate = this._settingService.GetSettingByKey<decimal>(string.Format("Tax.TaxProvider.FixedRate.TaxCategoryId{0}", taxCategoryId));
            return rate;
        }
    }
}
