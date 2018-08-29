///////////////////////////////////////////////////////////////////////////////
//
//  WorkFile Name:	CustomExceptionHandler.cs
//
//  ASA Proprietary Information
//
///////////////////////////////////////////////////////////////////////////////
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;


namespace ASA.DataAccess
{
    /// <summary>
    /// Custom Exception Handler that derives from Spring.NETs DataAccessException class
    /// </summary>
[CLSCompliantAttribute(false)]
    public class ASADataAccessException
    {

        string _errorMessage;
        string _source;

    /// <summary>
    /// Constructor that can be called via an exception advice
    /// </summary>
    /// <param name="ex"></param>
        public ASADataAccessException(Exception ex)            
        {
            _errorMessage = ex.GetBaseException().GetBaseException().Message;
            _source = "Data Access";
        }

    /// <summary>
    /// Error message property
    /// </summary>
        public string ErrorMessage
        {
            get
            {
                return _errorMessage;
            }
   
        }

    /// <summary>
    /// source property
    /// </summary>
        public string ExSource
        {
            get
            {
                return _source;                
            }

        }


    }

}
///////////////////////////////////////////////////////////////////////////////
