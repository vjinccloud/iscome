using ICCModule.Entity.Tables;
using ICCModule.Entity.Tables.Platform;
using ICCModule.EntityService.Service;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace InformationSystem.ViewModel.Management
{
    public class SelectRoleViewModel
    {
        public SelectRoleViewModel(string RoleCode)
        {
            Permissions = Service_Permission.GetBackList();
            HasPermissions = Service_Role_Has_Permission.GetList(RoleCode);
        }

        public List<Permission> Permissions { get; set; }

        public List<RoleHasPermission> HasPermissions { get; set; }
    }
}