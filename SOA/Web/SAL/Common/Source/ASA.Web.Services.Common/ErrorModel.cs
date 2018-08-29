using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ASA.Web.Services.Common
{
    public class ErrorModel
    {
        [DisplayName("Detail Message")]
        [StringLength(5000)]
        public string DetailMessage { get; set; }

        [DisplayName("Business Message")]
        [StringLength(5000)]
        public string BusinessMessage { get; set; }

        [DisplayName("Service Name")]
        [StringLength(250)]
        public string ServiceName { get; set; }

        [DisplayName("Code")]
        [StringLength(250)]
        public string Code { get; set; }

        public ErrorModel() 
        {
            setDefaults();
        }

        public ErrorModel(string detailMsg)
        {
            DetailMessage = detailMsg;
            BusinessMessage = detailMsg;
            setDefaults();
        }

        public ErrorModel(string detailMsg, string serviceName)
        {
            DetailMessage = detailMsg;
            BusinessMessage = detailMsg;
            ServiceName = serviceName;
            setDefaults();
        }

        public ErrorModel(string detailMsg, string serviceName, string code)
        {
            DetailMessage = detailMsg;
            BusinessMessage = detailMsg;
            ServiceName = serviceName;
            Code = code;
            setDefaults();
        }

        public ErrorModel(string detailMsg, string serviceName, string code, string businessMsg)
        {
            DetailMessage = detailMsg;
            BusinessMessage = businessMsg;
            ServiceName = serviceName;
            Code = code;
            setDefaults();
        }
        
        private void setDefaults()
        {
            if (DetailMessage == null)
                DetailMessage = string.Empty;

            if (BusinessMessage == null)
                BusinessMessage = string.Empty;

            if (ServiceName == null)
                ServiceName = string.Empty;

            if (Code == null)
                Code = string.Empty;
        }
    }
}
