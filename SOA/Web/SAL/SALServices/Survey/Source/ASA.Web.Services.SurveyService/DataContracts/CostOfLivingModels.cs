using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using ASA.Web.Common.Validation;
using ASA.Web.Services.Common;

namespace ASA.Web.Services.SurveyService.DataContracts
{
    public class COLRequestModel : BaseWebModel
    {
        private const string CLASSNAME = "ASA.Web.Services.SurveyService.DataContracts";
        static ASA.Log.ServiceLogger.IASALog _log = ASA.Log.ServiceLogger.ASALogManager.GetLogger(CLASSNAME);

        //COLRequestModel provides inputs when needed.
        [Required]
        public Nullable<int> StateId { get; set; }
        
        [Required]
        public Nullable<int> CityA { get; set; }
        
        [Required]
        public Nullable<int> CityB { get; set; }
        
        [Required]
        public Nullable<decimal> Salary { get; set; }
    }

    public class COLResponseModel : BaseWebModel
    {
        private const string CLASSNAME = "ASA.Web.Services.SurveyService.DataContracts";
        static ASA.Log.ServiceLogger.IASALog _log = ASA.Log.ServiceLogger.ASALogManager.GetLogger(CLASSNAME);

        public decimal ComparableSalary { get; set; }
        public decimal PercentageIncomeToMaintain { get; set; }
        public decimal PercentageIncomeToSustain { get; set; }

        private List<COLStateModel> _states = new List<COLStateModel>();
        private List<COLUrbanAreaModel> _urbanAreas = new List<COLUrbanAreaModel>();
        private List<COLCostBreakDownModel> _costBreakDowns = new List<COLCostBreakDownModel>();

        [DisplayName(@"State")]
        public List<COLStateModel> State
        {
            get
            {
                return _states;
            }
            set
            {
                _states = value;
            }
        }

        [DisplayName(@"UrbanArea")]
        public List<COLUrbanAreaModel> UrbanArea
        {
            get
            {
                return _urbanAreas;
            }
            set
            {
                _urbanAreas = value;
            }
        }

        [DisplayName(@"CostBreakDown")]
        public List<COLCostBreakDownModel> CostBreakDown
        {
            get
            {
                return _costBreakDowns;
            }
            set
            {
                _costBreakDowns = value;
            }
        }
    }

    public class COLStateModel
    {
        public int StateId { get; set; }
        public string StateCode { get; set; }
        public string State { get; set; }
    }

    public class COLUrbanAreaModel
    {
        public int RefGeographicalIndexID { get; set; }
        public string UrbanAreaName { get; set; }
    }

    public class COLCostBreakDownModel
    {
        public string Category { get; set; }
        public decimal PercentageChange { get; set; }
        public string PercentageChangeIndicator { get; set; }
    }
}