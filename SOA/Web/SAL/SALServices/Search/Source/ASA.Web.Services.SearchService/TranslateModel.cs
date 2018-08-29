using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ASA.Web.Services.SearchService.DataContracts;
using ASA.Web.Services.Proxies.SALTService;
using Common.Logging;
using ASA.Web.Collections;
using ASA.Web.Services.Proxies;

namespace ASA.Web.Services.SearchService
{
    public static class MappingExtensions
    {
        /// <summary>
        /// Map SALT Service RefOrganizationProductContract to SAL OrganizationProductModel
        /// </summary>
        /// <param name="orgProduct"></param>
        /// <returns></returns>
        public static OrganizationProductModel ToOrganizationProductModel(this RefOrganizationProductContract orgProduct)
        {
            return new OrganizationProductModel
            {
                OrgID = orgProduct.RefOrganizationID,
                ProductID = orgProduct.RefProductID,
                IsOrgProductActive = orgProduct.IsRefOrganizationProductActive
            };
        }

        /// <summary> 
        /// To the domain object for organization products.
        /// </summary> 
        /// <param name="orgProductContracts">organization product contracts.</param> 
        /// <returns></returns> 
        public static List<OrganizationProductModel> ToDomainObject(this RefOrganizationProductContract[] orgProductContracts)
        {
            var toReturn = new List<OrganizationProductModel>() { };
            var model = new OrganizationProductModel() { };
            foreach (var item in orgProductContracts)
            {
                model.OrgID = item.RefOrganizationID;
                model.ProductID = item.RefProductID;
                model.IsOrgProductActive = item.IsRefOrganizationProductActive;
                toReturn.Add(model);
            }
            return toReturn;
        }

        //TODO:Cleanup the below line - post multi-org release
        /// <summary>
        /// Map SALT Service SchoolProductContract to SAL SchoolProductModel
        /// </summary>
        /// <param name="schoolProduct"></param>
        /// <returns></returns>
        //public static SchoolProductModel ToSchoolProductModel(this RefSchoolProductContract schoolProduct)
        //{
        //    return new SchoolProductModel
        //    {
        //        RefSchoolID = schoolProduct.RefSchoolID,
        //        RefProductID = schoolProduct.RefProductID,
        //        IsRefSchoolProductActive = schoolProduct.IsRefSchoolProductActive
        //    };
        //}

        //TODO:Cleanup the below line - post multi-org release
        /// <summary> 
        /// To the domain object for school products.
        /// </summary> 
        /// <param name="schoolProdContracts">school product contracts.</param> 
        /// <returns></returns> 
        //public static List<SchoolProductModel> ToDomainObject(this RefSchoolProductContract[] schoolProdContracts)
        //{
        //    var toReturn = new List<SchoolProductModel>() { };
        //    var model = new SchoolProductModel() { };
        //    foreach (var item in schoolProdContracts)
        //    {
        //        model.RefSchoolID = item.RefSchoolID;
        //        model.RefProductID = item.RefProductID;
        //        model.IsRefSchoolProductActive = item.IsRefSchoolProductActive;
        //        toReturn.Add(model);
        //    }
        //    return toReturn;
        //}
    }
}
