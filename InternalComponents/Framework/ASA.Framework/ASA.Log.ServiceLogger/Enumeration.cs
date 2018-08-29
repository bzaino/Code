///////////////////////////////////////////////////////////////////////////////
//
//  WorkFile Name:	Enumeration.cs
//
//  ASA Proprietary Information
//
///////////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.Text;

namespace ASA.Log.ServiceLogger
{
    /// <summary>
    /// Logging event type.
    /// </summary>
    enum EventType
    {
        /// <summary>
        /// Start of service processing.
        /// </summary>
        ServiceEntry = 1,

        /// <summary>
        /// End of service processing.
        /// </summary>
        ServiceExit = 2,

        /// <summary>
        /// Request for service.
        /// </summary>
        ServiceCall = 3,

        /// <summary>
        /// Response received from service.
        /// </summary>
        ServiceReturn = 4,

        /// <summary>
        /// Entering method.
        /// </summary>
        MethodEntry = 5,

        /// <summary>
        /// Leaving method.
        /// </summary>
        MethodExit = 6,

        /// <summary>
        /// Exception occured.
        /// </summary>
        Exception = 7,

        /// <summary>
        /// TID audit record.
        /// </summary>
        TID = 8
    }

    /// <summary>
    /// Ending status of business activity.
    /// </summary>
    public enum EndingStatus
    {
        /// <summary>
        /// Processing was successful.
        /// </summary>
        Succeeded = 1,

        /// <summary>
        /// Processing failed.
        /// </summary>
        Failed = 2,

        /// <summary>
        /// processing was aborted.
        /// </summary>
        Aborted = 3
    }
}
