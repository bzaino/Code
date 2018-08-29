///////////////////////////////////////////////////////////////////////////////
//
//  WorkFile Name:	StoredProcBaseDAO.cs
//
//  ASA Proprietary Information
//
///////////////////////////////////////////////////////////////////////////////
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Spring.Data;
using Spring.Data.Common;
using Spring.Data.Objects;

namespace ASA.DataAccess
{
    /// <summary>
    /// Base class for all stored procedure data access objects. 
    /// Extends from 
    /// </summary>
[CLSCompliantAttribute(false)]    
    public class StoredProcBaseDAO : StoredProcedure
    {
        protected string _storedProcedureName;
        private bool initialized = false;

    /// <summary>
    /// Constructor that takes the DBProvider (to be configured in the xml files)
    /// </summary>
    /// <param name="provider">data base provider that has connection string information</param>
    /// <param name="storedProcedureName">name of the stored procedure to be executed</param>
    public StoredProcBaseDAO(IDbProvider provider, string storedProcedureName) : base(provider,storedProcedureName)
    {
        _storedProcedureName = storedProcedureName;
    }

    /// <summary>
    /// Adds the row mappers to be mapped after SP execution
    /// </summary>
    /// <param name="rowMappers">Row mappers to be added</param>
    /// <param name="getReturnValue">Boolean indicating if return value is required</param>
    public void Initialize (IList<KeyValuePair<string, IRowMapper>> rowMappers, bool getReturnValue)
    {
        if(!initialized)
        {
            DeriveParameters(getReturnValue);

            // WS-353. set stored procedure timeout period by setting "query_timeout" in ASASetting.config
            CommandTimeout = ASA.Common.Parameters.Instance.QueryTimeout;

            //Add all the row mappers here
            if (rowMappers != null)
            {
                foreach (KeyValuePair<string, IRowMapper> mapper in rowMappers)
                {
                    AddRowMapper(mapper.Key, mapper.Value);
                }
            }
            initialized = true;
            Compile();
        }
    }

    /// <summary>
    /// Name of the stored procedure
    /// </summary>
    public string StoredProcedureName
    {
        get
        {
            return _storedProcedureName;
        }

        /*set
        {
            _storedProcedureName = value;
        }*/
    }

    /// <summary>
    /// Exectures the stored procedure by called base class methods
    /// </summary>
    /// <param name="namedParameters">input parameters to teh SP</param>
    /// <param name="rowMappers">list of row mappers</param>
    /// <param name="getReturnValue">Boolean indicating if return value is required</param>
    /// <returns>Custom business entities list that are mapped to stored procedure resultset</returns>
    protected IDictionary ExecStoredProcedure(IDictionary namedParameters, IList<KeyValuePair<string, IRowMapper>> rowMappers, bool getReturnValue)
    {
        Initialize(rowMappers,getReturnValue);
        if (namedParameters != null)
            return QueryByNamedParam(namedParameters);
        else
            return QueryByNamedParam(new Dictionary<string, Object>());

    }

        public override void Compile()
        {
            if (initialized)
                base.Compile();
        }

    }
}
///////////////////////////////////////////////////////////////////////////////