using ICCModule.Entity.Tables;
using ICCModule.Entity.Tables.Platform;
using ICCModule.EntityService.Service;
using ICCModule.Models;
using InformationSystem.Helper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;

namespace InformationSystem.ViewModel.PlantDoctor
{
    public class PlantDoctorWord
    {              
        /// <summary>
        /// 
        /// </summary>
        public int PicNum { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string PicUrl { get; set; }
    }
}