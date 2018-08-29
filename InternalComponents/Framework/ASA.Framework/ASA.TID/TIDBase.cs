///////////////////////////////////////////////
//  WorkFile Name: TIDBase.cs in ASA.TID
//  Description:        
//      Base TID class class for all versions
//            ASA Proprietary Information
///////////////////////////////////////////////
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;

namespace ASA.TID
{
    [DataContract(Namespace = "http://amsa.com/contract/tidbase/v1.0", Name = "TIDBase")]
    public abstract class TIDBase : ITID
    {
        #region Data Members
        // Data Members
        protected Hashtable mapProps;
        #endregion

        #region Abstract Methods
        // Abstract Methods
        public abstract bool CreateTID(Hashtable mapPropsIn);
        public abstract bool SendTID(ITID tidIn, Hashtable mapPropsIn);
        public abstract bool RecvTID(Hashtable mapPropsIn);
        public abstract bool Set(string fieldNameIn, object valueIn);
        public abstract string GetVersion();
        #endregion

        #region Constructors
        // Constructors
        /// <summary>
        /// The TIDBase() default constructor is used to initialize the fields in a new TIDBase object.
        /// The TIDBase() default constructor will instantiate a new Hashtable used to store the fields.
        /// </summary>
        /// <param name="tidIn"></param>
        /// <returns></returns>
        public TIDBase()
        {
            // Initialize data members
            mapProps = new Hashtable();
        }

        /// <summary>
        /// The TIDBase() copy constructor is used to initialize a new TIDBase object based on fields set in an existing TIDBase object upon object creation.
        /// The TIDBase() copy constructor will create a new Hashtable and copy the Hashtable containing the TID fields from the source object passed.
        /// </summary>
        /// <param name="tidIn"></param>
        /// <returns></returns>
        public TIDBase(TIDBase tidIn)
        {
            // Create a copy of the data members
            mapProps = (tidIn.mapProps != null) 
                ? new Hashtable(tidIn.mapProps) 
                : new Hashtable();
        }
        #endregion

        #region Public Methods
        // Methods
        /// <summary>
        /// Use GetFields() to retrieve all of the fields within the version-specific TID object whose class derives from this one.
        /// The GetFields() method must be implemented in any class that implements the ITID interface.
        /// GetFields() returns a Hashtable containing the field/value mapping of all TID fields in the object.
        /// </summary>
        /// <returns>Hashtable</returns>
        public virtual Hashtable GetFields()
        {
            // Return a copy of mapProps
            return new Hashtable(mapProps);
        }

        /// <summary>
        /// Use SetFields() to set one or more fields within the version-specific TID object whose class derives from this one.
        /// SetFields() will update the appropriate property and Hashtable entries with the Hashtable entries in the passed Hashtable argument.
        /// The SetFields() method must be implemented in any class that implements the ITID interface.
        /// SetFields() returns false if any field in the passed Hashtable fails to get set.
        /// </summary>
        /// <param name="mapPropsIn"></param>
        /// <returns>bool</returns>
        public virtual bool SetFields(Hashtable mapPropsIn)
        {
            bool bRet = true;

            // Attempt to set all fields specified in mapPropsIn to mapProps
            foreach (DictionaryEntry de in mapPropsIn)
            {
                // Set next field from mapPropsIn to mapProps
                if (!Set((string)(de.Key), de.Value))
                {
                    bRet = false;
                }
            }

            // If all fields were set, return true; otherwise return false
            return bRet;
        }

        /// <summary>
        /// Use Get() to retrieve a field from within the version-specific TID object whose class derives from this one.
        /// Get() will retrieve the appropriate value from the Hashtable entry.
        /// The Get() method must be implemented in any class that implements the ITID interface.
        /// Get() returns the value in the form of an object type that must be cast to the appropriate type by the caller
        /// </summary>
        /// <param name="fieldNameIn"></param>
        /// <returns>object</returns>
        public virtual object Get(string fieldNameIn)
        {
            // Retrieve the value stored in mapProps based on specified fieldNameIn key
            return (mapProps.ContainsKey(fieldNameIn.ToUpper())) ? mapProps[fieldNameIn.ToUpper()] : null;
        }

        /// <summary>
        /// Call ToString() to retrieve an XML string representation of the TID structure.
        /// ToString() will deserialize the TID into a MemoryStream using DataContractSerializer/XmlTextWriter methods.
        /// The ToString() method must be implemented in any class that implements the ITID interface.
        /// </summary>
        /// <returns>string</returns>
        public override string ToString()
        {
            string xml = null;
            
            // Create a new DataContractSerializer, MemoryStream, and XmlTextWriter for serializing
            DataContractSerializer dcSerializer = new DataContractSerializer(GetType());
            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (XmlTextWriter xmlTxtWriter = new XmlTextWriter(memoryStream, Encoding.UTF8))
                {

                    // Serialize the current object using the XmlTextWriter through the DataContractSerializer
                    dcSerializer.WriteObject(xmlTxtWriter, this);

                    // Ensure all data has been written to the MemoryStream
                    xmlTxtWriter.Flush();
                    xmlTxtWriter.BaseStream.Flush();

                    // Extract the XML string from the memory stream
                    using (MemoryStream memStream = (MemoryStream)xmlTxtWriter.BaseStream)
                    {
                        UTF8Encoding utf8Encoding = new UTF8Encoding();
                        xml = utf8Encoding.GetString(memStream.ToArray());
                    }
                }
            }

            // Return XML string to caller
            return xml;
        }
        #endregion
    }

}
