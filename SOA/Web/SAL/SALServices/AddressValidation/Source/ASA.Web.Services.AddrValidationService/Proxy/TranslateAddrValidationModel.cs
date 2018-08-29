using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ASA.Web.Services.AddrValidationService.QasProWeb;
using ASA.Web.Services.AddrValidationService.DataContracts;
using Common.Logging;

namespace ASA.Web.Services.AddrValidationService.Proxy
{
    public class TranslateAddrValidationModel
    {
        private const string CLASSNAME = "ASA.Web.Services.AddrValidationService.Proxy.TranslateAddrValidationModel";
        static readonly ILog _log = LogManager.GetLogger(CLASSNAME);

        public static QasProWeb.QASearch MapAddressToQasSearch(DataContracts.AddressModel addr)
        {
            _log.Debug("START MapAddressToQasSearch");
            EngineType engineType = new EngineType();
            engineType.Intensity = EngineIntensityType.Close;
            engineType.PromptSet = PromptSetType.Default;
            engineType.Threshold = Config.QasThreshold.ToString();
            engineType.Timeout = Config.QasTimeout.ToString();
            engineType.Flatten = true;
            engineType.Value = EngineEnumType.Verification;
            
            // qas adds
            engineType.IntensitySpecified = true;
            engineType.PromptSetSpecified = true;
            engineType.FlattenSpecified = true;
            // end qas adds


            QASearch search = new QASearch();
            search.Country = "USA";
            search.Layout = Config.QasLayout.ToString(); 
            search.Engine = engineType;
            search.Search = GetFormatedAddressString(addr);

            _log.Debug("END MapAddressToQasSearch");

            return search;
        }

        public static DataContracts.AddrValidationResponseModel MapGetResponseToModel(QasProWeb.QASearchResult result)
        {
            _log.Debug("START MapGetResponseToModel");
            AddrValidationResponseModel avrModel = new AddrValidationResponseModel();
            if (result != null)
            {
                //note: VerifyLevel is not checkd for null here because VerifyLevel type is non-nullable. it would never happen.
                avrModel.VerifyLevel = result.VerifyLevel.ToString();
                _log.Debug("VerifyLevel = " + result.VerifyLevel.ToString());

                if (result.QAAddress != null && result.QAAddress.AddressLine != null && result.QAAddress.AddressLine.Length > 5)
                {
                    _log.Debug("VerifiedAddress exists in the results from QAS");
                    avrModel.VerifiedAddress = new AddressModel(); // ASA.Web.Services.AddrValidationService.DataContracts.
                    avrModel.VerifiedAddress.AddressLine1 = result.QAAddress.AddressLine[0].Line;
                    if (result.QAAddress.AddressLine[1].Line != null)
                    avrModel.VerifiedAddress.AddressLine2 = result.QAAddress.AddressLine[1].Line;
                    if (result.QAAddress.AddressLine[2].Line != null)
                    avrModel.VerifiedAddress.AddressLine2 += result.QAAddress.AddressLine[2].Line;
                    avrModel.VerifiedAddress.City = result.QAAddress.AddressLine[3].Line;
                    avrModel.VerifiedAddress.StateID = result.QAAddress.AddressLine[4].Line;
                    string strFullPostalZip = result.QAAddress.AddressLine[5].Line; // long zip is returned -- may need to be truncated for UI
                    avrModel.VerifiedAddress.Zip = strFullPostalZip.Substring(0, 5);
                    avrModel.VerifiedAddress.CountryID = "US";
                }

                avrModel.SuggestedAddresses = ConvertPickListToAddressListModel(result.QAPicklist);

            }
            _log.Debug("END MapGetResponseToModel");

            return avrModel;
        }

        /// Creates a formated string to be sent to QAS containing values from the Contact Address object
        /// </summary>
        /// <param name="address">ContactAddress object containing a domestic address</param>
        /// <returns> a formated string which will be sent to the QAS ProWeb service</returns>
        private static string GetFormatedAddressString(AddressModel address)
        {
            _log.Debug("START GetFormatedAddressString");
            StringBuilder addressString = new StringBuilder();

            if (!string.IsNullOrEmpty(address.AddressLine1))
            {
                if (addressString.Length > 0)
                {
                    addressString.Append(", ");
                }
                addressString.Append(address.AddressLine1);
            }

            if (!string.IsNullOrEmpty(address.AddressLine2))
            {
                if (addressString.Length > 0)
                {
                    addressString.Append(", ");
                }
                addressString.Append(address.AddressLine2);
            }

            if (!string.IsNullOrEmpty(address.City))
            {
                if (addressString.Length > 0)
                {
                    addressString.Append(", ");
                }
                addressString.Append(address.City);
            }

            if (!string.IsNullOrEmpty(address.StateID))
            {
                if (addressString.Length > 0)
                {
                    addressString.Append(", ");
                }
                addressString.Append(address.StateID);
            }

            if (!string.IsNullOrEmpty(address.Zip))
            {
                if (addressString.Length > 0)
                {
                    addressString.Append(", ");
                }
                addressString.Append(address.Zip);
            }

            _log.Debug("Address: " + addressString.ToString());
            _log.Debug("END GetFormatedAddressString");
            return addressString.ToString();
        }

        private static AddressListModel ConvertPickListToAddressListModel(QAPicklistType qasPickList)
        {
            _log.Debug("START ConvertPickListToAddressListModel");
            AddressListModel addressList = new AddressListModel();
            if (qasPickList != null && qasPickList.PicklistEntry !=null && qasPickList.PicklistEntry.Length > 0)
            {
                _log.Debug("PickListEntries were found. Count = " + qasPickList.PicklistEntry.Length.ToString());
                int numPicklistTotal = qasPickList.PicklistEntry.Length;
                int numOfEntries = numPicklistTotal < Config.QasPickListLength ? numPicklistTotal : Config.QasPickListLength;  // ASA.Web.Services.AddrValidationService
                AddressModel PriorAddr = new AddressModel();

                for (int i = 0; i < numOfEntries; i++)
                {
                    if (qasPickList.PicklistEntry[i] != null &&
                        qasPickList.PicklistEntry[i].Picklist != null && 
                        !string.IsNullOrEmpty(qasPickList.PicklistEntry[i].Picklist))
                    {
                        _log.Debug("Creating AddressModel for PickListEntry[" + i + "]");
                        
                        AddressModel addr = new AddressModel();
                        if (qasPickList.Total != "0")
                        {
                            //_log.Debug("PickListEntry[" + i + "].Picklist = " + qasPickList.PicklistEntry[i].Picklist + " Score: " + qasPickList.PicklistEntry[i].Score);

                            string[] pickListParts = null;
                            pickListParts = qasPickList.PicklistEntry[i].Picklist.Split(',');

                            addr.CountryID = "US";
                            //if (qasPickList.PicklistEntry[i].PostcodeRecoded)

                            if (qasPickList.PicklistEntry[i].Postcode != null && qasPickList.PicklistEntry[i].Postcode.Length >= 5)
                            {
                                string strFullPostalZip = null;
                                strFullPostalZip = qasPickList.PicklistEntry[i].Postcode;
                                if (strFullPostalZip != null && strFullPostalZip.Length >= 5)
                                {
                                    strFullPostalZip = strFullPostalZip.Trim();
                                    addr.Zip = strFullPostalZip.Substring(0, 5);
                                }
                            }

                            if (pickListParts != null && pickListParts.Length >= 2)
                            {
                                string strCityState = null;
                                strCityState = pickListParts[pickListParts.Length - 1];
                                // remove [odd] or [even] trailing strings
                                if (strCityState.Contains("["))
                                    strCityState = strCityState.Substring(0, strCityState.LastIndexOf("["));
                                if (strCityState != null && strCityState.Length > 2)
                                {
                                    // clean up whitespace
                                    strCityState = strCityState.Trim();
                                    addr.City = strCityState.Substring(0, strCityState.Length - 3);
                                    addr.StateID = strCityState.Substring(strCityState.Length - 2, 2);
                                }
                                addr.AddressLine1 = pickListParts[0];
                            }
                            if (pickListParts != null && pickListParts.Length >= 3)
                            {
                                addr.AddressLine1 = pickListParts[0];
                                addr.AddressLine2 = pickListParts[1];
                            }
                        }
                        if ((PriorAddr.AddressLine1 + PriorAddr.AddressLine2 + PriorAddr.City + PriorAddr.StateID + PriorAddr.Zip) != (addr.AddressLine1 + addr.AddressLine2 + addr.City + addr.StateID + addr.Zip))
                        {
                            addressList.Addresses.Add(addr);
                            PriorAddr = addr;
                        }
                        else
                        {
                            if (numPicklistTotal > numOfEntries)
                                numOfEntries++;
                        }
                    }

                }
            }

            _log.Debug("END ConvertPickListToAddressListModel");
            return addressList;
        }
    }
}
