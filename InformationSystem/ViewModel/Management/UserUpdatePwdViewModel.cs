using ICCModule.Entity.Tables;
using ICCModule.Entity.Tables.Platform;
using ICCModule.EntityService.Service;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace InformationSystem.ViewModel.Management
{
    public class UserUpdatePwdViewModel
    {
        public string OldLoginPass { get; set; }

        public string LoginPass { get; set; }

        public string ConfirmLoginPass { get; set; }

        public bool CheckStrength
        {
            get
            {
                return Regex.IsMatch(LoginPass, @"((?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[!@#$%^&*()_=+{};',:"" <>?\.]).{ 8,40})");
            }
        }
    }
}