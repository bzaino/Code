using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ASA.Web.Services.Common;
using System.Collections.ObjectModel;
using Endeca.Data;
using Endeca.Data.Provider;

namespace ASA.Web.Services.SearchService.DataContracts
{
    public class SearchResultsModel : BaseWebModel
    {
        public SearchResultsModel()
        {
            this.Records = new List<SearchRecordModel>();
        }

        public List<SearchRecordModel> Records { get; set; }
        public int TotalRecords { get; set; }
        public int Offset { get; set; }

        private string _resultsMessage = "";
        public string ResultsMessage
        {
            get
            {
                if (_resultsMessage == string.Empty)
                {
                    if (this.Records.Count > 0)
                        return "Records were found";
                    else
                        return "No Records were found";

                }
                else return _resultsMessage;
            }
            set { _resultsMessage = value; }
        }
    }

    public class SearchRecordModel //: BaseWebModel
    {
        public Dictionary<string, List<string>> _fields = null;
        public SearchRecordModel()
        {
            _fields = new Dictionary<string, List<string>>();
        }

        public Dictionary<string, List<string>> Fields { get { return _fields; } set { _fields = value; } }
    }

}
