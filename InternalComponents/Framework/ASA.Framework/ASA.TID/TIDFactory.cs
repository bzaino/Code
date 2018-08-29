///////////////////////////////////////////////
//  WorkFile Name: TIDFactory.cs in ASA.TID
//  Description:        
//      TID factory class used for generating a TID
//      object of the correct version
//            ASA Proprietary Information
///////////////////////////////////////////////
using System;
using System.Collections;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;

namespace ASA.TID
{
    public static class TIDFactory
    {
        #region GetTIDFromMessage
        /// <summary>
        /// The GetTIDFromMessage() static function is used to retrieve an ITID interface
        ///  to a TID object based on the MessageHeaders object within a Message.
        /// If the TID version is not known, the latest TID version will be used to
        ///  deserialize the TID.
        /// GetTIDFromMessage() returns null if the TID header cannot be found.
        /// </summary>
        /// <param name="messageHeadersIn"></param>
        /// <returns>ITID</returns>
        public static ITID GetTIDFromMessage(MessageHeaders messageHeadersIn)
        {
            // Get the name of the TID namespace from the message header
            string tidNamespace = "";
            foreach (MessageHeaderInfo mhi in messageHeadersIn)
            {
                if (mhi.Name != null && mhi.Name.ToUpper().StartsWith("TID"))
                {
                    tidNamespace = mhi.Name;
                    break;
                }
            }

            // Deserialize the version from message header into a TIDVersion object
            TIDVersion tidVersion = (tidNamespace.Length > 0)
                ? messageHeadersIn.GetHeader<TIDVersion>("TID", tidNamespace)
                : null;

            // Deserialize the TID header into the version-specific object based on the version returned
            TIDBase tid = (tidVersion == null)  // Add supported versions below
                        ? null
                        : (tidVersion.TidVersion == "01.00.000")
                        ? new TID(messageHeadersIn.GetHeader<TID>("TID", tidNamespace))
//                      : (tidVersion.TidVersion == "99.99.999")
//                      ? messageHeadersIn.GetHeader<TID_99_99_999>("TID", tidNamespace)
                        // keep updated to latest TID version below
                        : new TID(messageHeadersIn.GetHeader<TID>("TID", tidNamespace)); 

            // Return interface to new object
            return tid;
        }
        #endregion

        #region GetCreateTID
        /// <summary>
        /// The GetCreateTID() static function is used to create a new TID based on the most recent
        ///  known TID version and return an ITID interface to the TID object for the new TID.
        /// GetCreateTID() should be called by external logic when a new TID needs to be created for a logical message transaction.
        /// GetCreateTID() calls CreateTID() on the newly created TID object to set the TID fields.
        /// Update/override any TID fields by specifying those field/values within the passed Hashtable argument.
        /// GetCreateTID() sets the SUCCESS key in the passed Hashtable to the return value of CreateTID (true or false).
        /// </summary>
        /// <param name="mapPropsIn"></param>
        /// <returns>ITID</returns>
        public static ITID GetCreateTID(Hashtable mapPropsIn)
        {
            // Create a new TID
            TIDBase tid = new TID();

            // Call CreateTID() to set fields for creation and store success status in map for caller
            mapPropsIn[TIDField.SUCCESS] = tid.CreateTID(mapPropsIn);

            // Return interface to new TID object
            return tid;
        }
        #endregion

        #region GetRecvTID
        /// <summary>
        /// The GetRecvTID() static function is used to create a new TID object based on the
        ///  TID version found in the MessageHeaders argument passed to the function.
        /// GetRecvTID() calls GetTIDFromMessage() to create the new TID object based on the MessageHeaders argument.
        /// GetRecvTID() calls RecvTID() on the newly created TID object to set the TID fields.
        /// Update/override any TID fields by specifying those field/values within the passed Hashtable argument.
        /// GetRecvTID() sets the SUCCESS key in the passed Hashtable to the return value of CreateTID (true or false)
        ///  or to false if GetTIDFromMessage() returns null.
        /// </summary>
        /// <param name="messageHeadersIn"></param>
        /// <param name="mapPropsIn"></param>
        /// <returns>ITID</returns>
        public static ITID GetRecvTID(MessageHeaders messageHeadersIn, Hashtable mapPropsIn)
        {
            // Initialize success flag
            mapPropsIn[TIDField.SUCCESS] = true;

            // Deserialize the message and retrieve the interface as a TIDBase object
            TIDBase tid = (TIDBase)GetTIDFromMessage(messageHeadersIn);

            // Set the fields for TID receiving and store error status in map for caller on failure
            if (tid == null || !tid.RecvTID(mapPropsIn))
            {
                // If version is not known, create the latest known version
                mapPropsIn[TIDField.SUCCESS] = false;
            }

            // Return interface to new object
            return tid;
        }
        #endregion

        #region GetSendTID
        /// <summary>
        /// The GetSendTID() static function is used to create a new TID object based on the
        ///  TID version found in the MessageHeaders argument passed to the function.
        /// GetRecvTID() calls GetTIDFromMessage() to create the new TID object based on the MessageHeaders argument.
        /// GetRecvTID() calls RecvTID() on the newly created TID object to set the TID fields.
        /// Update/override any TID fields by specifying those field/values within the passed Hashtable argument.
        /// GetRecvTID() sets the SUCCESS key in the passed Hashtable to the return value of CreateTID (true or false)
        ///  or to false if GetTIDFromMessage() returns null.
        /// </summary>
        /// <param name="messageHeadersIn"></param>
        /// <param name="mapPropsIn"></param>
        /// <returns>ITID</returns>
        public static ITID GetSendTID(ITID tidIn, Hashtable mapPropsIn)
        {
            // Initialize success flag
            mapPropsIn[TIDField.SUCCESS] = true;

            // Create a new TID if not already created
            if (tidIn == null)
            {
                // Set fields for TID creation
                tidIn = GetCreateTID(mapPropsIn);

                // Save success status of GetCreateTID()
                mapPropsIn[TIDField.SUCCESS] = (bool)mapPropsIn[TIDField.SUCCESS];
            }

            // Create the new TID object based on version passed in request rcvd TID (tidIn)
            TIDBase tid = (tidIn.GetVersion() == "01.00.000")   // Add supported versions below
                        ? new TID()
//                      : (tidVersion.TidVersion == "99.99.999")
//                      ? new TID_99_99_999()
                        // keep updated to latest TID version below
                        : new TID();  

            // Call SendTID()
            if (!tid.SendTID(tidIn, mapPropsIn))
            {
                // If either GetCreateTID() or SendTID() failed, update success status to false
                mapPropsIn[TIDField.SUCCESS] = false;
            }

            // Return interface to new object
            return tid;
        }
        #endregion
    }
}
