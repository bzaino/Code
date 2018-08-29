///////////////////////////////////////////////////////////////////////////////
//
//  WorkFile Name:	RowMapperBase.cs
//
//  ASA Proprietary Information
//
///////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Text;
using Spring.Data;
using System.Data;

namespace ASA.DataAccess
{
    /// <summary>
    /// Custom interfaces that extends Spring.NETs IRowMapper interface. 
    /// Helps with mapping resultsets from stored procedures to custom
    /// business entities.
    /// </summary>
[CLSCompliantAttribute(false)]   
    public interface IRowMapperDAO : IRowMapper
    {
        string GetTypeName();
    }

    /// <summary>
    /// Abstract base class for all the row mappers of any type. 
    /// Implements IRowMapperDAO interface
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class RowMapperBase<T> : IRowMapperDAO 
    {
        //Returns the mapper type name as string
        string _typeName = typeof(T).ToString();
        
        /// <summary>
        /// Returns type name as string
        /// </summary>
        /// <returns>type name</returns>
        virtual public string GetTypeName()
        {
            return _typeName;
        }

        /// <summary>
        /// Gets field value from the data reader
        /// </summary>
        /// <param name="reader">data reader</param>
        /// <param name="field">field index</param>
        /// <returns>field value</returns>
        public string GetSafeString(IDataReader reader, int field)
        {
            if (reader.IsDBNull(field))
                return string.Empty;
            else
                return reader.GetString(field);
        }

        /// <summary>
        /// Abstract method to be implemented by the sub objects. This method
        /// is called by Spring.NET at run time for every row mapper
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="rowNum"></param>
        /// <returns></returns>
        public abstract object MapRow(IDataReader reader, int rowNum);
    }
}
///////////////////////////////////////////////////////////////////////////////