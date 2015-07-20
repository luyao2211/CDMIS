using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CDMIS.Models;

namespace CDMIS.ViewModels
{
    public class HealthEducationList
    {
        public string selectedModuleId { get; set; }
        public List<SelectListItem> ModuleList()
        {
            return CommonVariables.GetModuleList();
        }

        public List<HealthEducation> HEList { get; set; }

        public HealthEducationList()
        {
            selectedModuleId = "M1";
            HEList = new List<HealthEducation>();
        }
    }

    public class NewHealthEducationFile
    {
        public string selectedModuleId { get; set; }
        public List<SelectListItem> ModuleList()
        {
            return CommonVariables.GetModuleList();
        }

        public HealthEducation news { get; set; }
        public NewHealthEducationFile()
        {
            selectedModuleId = "M1";
            news = new HealthEducation();
        }
    }
}