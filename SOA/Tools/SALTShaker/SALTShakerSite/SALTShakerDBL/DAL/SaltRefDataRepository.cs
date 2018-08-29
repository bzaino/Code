using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SALTShaker.DAL;
using SALTShaker.DAL.DataContracts;
using SALTShaker.Proxies;
using SALTShaker.Proxies.SALTService;
using log4net;

namespace SALTShaker.BLL
{
    public class SaltRefDataRepository : IDisposable, ISaltRefDataRepository
    {
        #region SALTService calls

        private static readonly ILog logger = LogManager.GetLogger(typeof(SaltRefDataRepository));
        SaltServiceAgent saltAgent = new SaltServiceAgent();        

        public SaltRefDataRepository()
        {
            
        }

        public IEnumerable<RefRegistrationSourceModel> GetAllRefRegistrationSources()
        {
            IEnumerable<RefRegistrationSourceModel> regSources = saltAgent.GetAllRefRegistrationSources().ToDomainObject();

            return regSources;
        }

        public IEnumerable<RefRegistrationSourceTypeModel> GetAllRefRegistrationSourceTypes()
        {
            IEnumerable<RefRegistrationSourceTypeModel> regSourceTypes = saltAgent.GetAllRefRegistrationSourceTypes().ToDomainObject();

            return regSourceTypes;
        }

        public IEnumerable<RefCampaignModel> GetAllRefCampaigns()
        {
            IEnumerable<RefCampaignModel> refCampaigns = saltAgent.GetAllRefCampaigns().ToDomainObject();

            return refCampaigns;
        }

        public IEnumerable<RefChannelModel> GetAllRefChannels()
        {
            IEnumerable<RefChannelModel> refChannels = saltAgent.GetAllRefChannels().ToDomainObject();

            return refChannels;
        }

        public IEnumerable<vMemberRoleModel> GetAllRefUserRoles()
        {
            IEnumerable<vMemberRoleModel> refMemberRoles = saltAgent.GetAllRefUserRoles().ToDomainObject();

            return refMemberRoles;
        }

        public bool AddRefRegistrationSource(string name, string description, int typeId, string createdBy, int campaignId, int channelId)
        {
            RefRegistrationSourceContract refRegistrationSourceContract = new RefRegistrationSourceContract();
            refRegistrationSourceContract.RegistrationSourceName = name;
            refRegistrationSourceContract.RegistrationDetail = description;
            refRegistrationSourceContract.RefRegistrationSourceTypeId = typeId;
            refRegistrationSourceContract.RefCampaignId = campaignId;
            refRegistrationSourceContract.RefChannelId = channelId;
            refRegistrationSourceContract.CreatedBy = createdBy;

            try
            {
                return saltAgent.AddRefRegistrationSource(refRegistrationSourceContract);
            }
            catch
            {
                return false;
            }
        }

        public bool UpdateRefRegistrationSource(int id, string name, string description, int typeId, string createdBy, string modifiedBy, int campaignId, int channelId)
        {
            RefRegistrationSourceContract refRegistrationSourceContract = new RefRegistrationSourceContract();
            refRegistrationSourceContract.RefRegistrationSourceId = id;
            refRegistrationSourceContract.RegistrationSourceName = name;
            refRegistrationSourceContract.RegistrationDetail = description;
            refRegistrationSourceContract.RefRegistrationSourceTypeId = typeId;
            refRegistrationSourceContract.RefCampaignId = campaignId;
            refRegistrationSourceContract.RefChannelId = channelId;
            refRegistrationSourceContract.CreatedBy = createdBy;
            refRegistrationSourceContract.ModifiedBy = modifiedBy;

            try
            {
                return saltAgent.UpdateRefRegistrationSource(refRegistrationSourceContract);
            }
            catch
            {
                return false;
            }
        }

        public bool AddRefCampaign(string campaignName, string createdBy)
        {
            RefCampaignContract refCampaignContract = new RefCampaignContract();
            refCampaignContract.CampaignName = campaignName;
            refCampaignContract.CreatedBy = createdBy;

            try
            {
                return saltAgent.AddRefCampaign(refCampaignContract);
            }
            catch
            {
                return false;
            }
        }

        public bool UpdateRefCampaign(int campaignId, string campaignName, string createdBy, string modifiedBy)
        {
            RefCampaignContract refCampaignContract = new RefCampaignContract();
            refCampaignContract.RefCampaignID = campaignId;
            refCampaignContract.CampaignName = campaignName;
            refCampaignContract.CreatedBy = createdBy;
            refCampaignContract.ModifiedBy = modifiedBy;

            try
            {
                return saltAgent.UpdateRefCampaign(refCampaignContract);
            }
            catch
            {
                return false;
            }
        }
        #endregion

        //For IDisposable interface
        #region IDisposable interface
        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposedValue)
            {
                if (disposing)
                {
                    //context.Dispose();
                }
            }
            this.disposedValue = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
