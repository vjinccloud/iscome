using System;
using System.Collections.Generic;
using System.Web.Mvc;
using ICCModule.Entity.Tables.Platform;
using ICCModule.EntityService.Service;
using InformationSystem.Helper;

namespace InformationSystem.ViewModel
{
    public class ServiceAreaStatisticModel
    {
        const int YEAR_RANGE = 5;

        /// <summary>
        /// 年度選單
        /// </summary>
        public List<SelectListItem> YearDdl { get; set; } = new List<SelectListItem>();

        public SelectList DistDdl { get; set; }

        public SelectList CropTypeDdl { get; set; }

        public void Load()
        {
            int currentYear = DateTime.Now.Year;
            for (int i = 0; i < YEAR_RANGE; i++)
            {
                var y = currentYear - i;
                YearDdl.Add(new SelectListItem() { Text = $"{y - 1911}年", Value = y.ToString(), Selected = y == currentYear });
            }


            DistDdl = new SelectList(DistCenterHelper.GetAllDist(), nameof(DistCenterPoint.Name), nameof(DistCenterPoint.Name));

            var cropTypes = Service_cropType.GetList(x => x.Enable);
            CropTypeDdl = new SelectList(cropTypes, nameof(cropType.Name), nameof(cropType.Name));
        }

    }
}