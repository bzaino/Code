///////////////////////////////////////////////////////////////////////////////
//
//  WorkFile Name:	BaseDAO.cs
//
//  ASA Proprietary Information
//
///////////////////////////////////////////////////////////////////////////////

using System;
using NHibernate;
using NHibernate.Criterion;
using log4net;
using System.Collections;
using System.Collections.Generic;
using System.Transactions;
using Spring.Data.NHibernate;
using NHibernate.Collection.Generic;
using ASA.Log.ServiceLogger;
using ASA.Common;

namespace ASA.DataAccess
{
    /// <summary>
    ///This is the base class for all DAO objects (except the stored procedure DAO objects)
    /// Transaction management and session management handled by Spring.NET
    /// </summary>
    /// <remarks>Using IBaseDAO interface to abstract NHibernate's session factory functionality from BaseDAO</remarks>
    [CLSCompliant(false)]
    public class BaseDAO : ASA.DataAccess.IBaseDAO
    {
        //session factory variable
        private ISessionFactory sessionFactory;

        //Log4Net
        public IASALog Log = ASALogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public BaseDAO()
        {
        }

        /// <summary>
        /// This property is set at run time using Spring's property setter
        /// dependency injection
        /// </summary>
        public ISessionFactory SessionFactory
        {

            set
            {
                sessionFactory = value;
            }
            get
            {
                return sessionFactory;
            }
        }

        /// <summary>
        /// Creates new criteria object and applies the transaction timeout
        /// configured as part of the transaction attributes
        /// example: <tx:method name="Get*" read-only="true" timeout="900"/>
        /// </summary>
        /// <typeparam name="T">Type requesting the criteria</typeparam>
        /// <param name="session">NHibernate session</param>
        /// <returns></returns>
        private ICriteria GetCriteria<T>(ISession session)
        {
            ICriteria Criteria = session.CreateCriteria(typeof(T));
            SessionFactoryUtils.ApplyTransactionTimeout(Criteria, sessionFactory);

            NHibernate.Engine.ISessionFactoryImplementor sessionFactoryImpl = sessionFactory as NHibernate.Engine.ISessionFactoryImplementor;
            if (sessionFactoryImpl != null && sessionFactoryImpl.Settings.IsQueryCacheEnabled)
            {
                Criteria.SetCacheable(true);
            }
            return (Criteria);
        }

        /// <summary>
        /// Inserts row into the table associated with the 
        /// data object that is requested to be updated
        /// </summary>
        /// <param name="obj">Business entity object</param>
        /// <returns>Boolean indicating success or failure</returns>
        public void Add(Object obj)
        {
            ISession session = null;
            session = GetSession();
            Log.InfoFormat("Adding data object of type {0}:", obj.ToString());
            session.Save(obj);
            LogTransactionInfo();
            Log.Info("Save completed");
        }

        /// <summary>
        /// Inserts row into the table associated with the 
        /// data object that is requested to be added, returns
        /// the identfier for the inserted record
        /// </summary>
        /// <param name="obj">Business entity object</param>
        /// <param name="objID">object identity</param>
        /// <returns>Boolean indicating success or failure</returns>
        public void Add(Object obj, out long objID)
        {
            ISession session = null;
            session = GetSession();
            Log.InfoFormat("Adding data object of type {0}:", obj.ToString());
            session.Save(obj);
            objID = (long)session.GetIdentifier(obj);
            LogTransactionInfo();
            Log.Info("Save completed");
        }

        /// <summary>
        /// Updates the table associated with the data object 
        /// that is requested to be updated. 
        /// </summary>
        /// <param name="obj">Business entity object</param>
        /// <returns>Boolean indicating success or failure</returns>
        public void Update(Object obj)
        {
            ISession session = null;
            session = GetSession();
            Log.InfoFormat("Updating data object of type {0}:", obj.ToString());
            session.Update(obj);
            LogTransactionInfo();
            Log.Info("Update completed");
        }

        /// <summary>
        /// Deletes the specified data object
        /// </summary>
        /// <param name="obj">object to be deleted</param>
        /// <returns>Boolean indicating success or failure</returns>
        public void Delete(object obj)
        {
            ISession session = null;
            Log.Info("Entering BaseDAO::Delete");
            session = GetSession();
            LogTransactionInfo();
            session.Delete(obj);
            LogTransactionInfo();
            Log.Info("Leaving BaseDAO::Delete");
        }

        /// <summary>
        /// Deletes the specified sub-object from the parent data object
        /// </summary>
        /// <param name="subObjectList">list of sub-objects</param>
        /// <param name="index">index of sub-object  to be deleted</param>
        /// <returns></returns>
        public void DeleteSubObject<T>(IList<T> subObjectList, int index)
        {
            this.Delete(subObjectList[index]);
            subObjectList.RemoveAt(index);
        }

        /// <summary>
        /// Updates/Adds/Delete set of objects to support atomic transaction
        /// </summary>
        /// <param name="transArray">list of TransItem objects</param>
        /// <returns>Boolean indicating success or failure</returns>
        public void ExecTransaction(List<TransItem> transArray)
        {
            Log.Info("Executing Transaction...");
            ISession session = null;
            LogTransactionInfo();
            session = GetSession();
            foreach (TransItem item in transArray)
            {
                ProcessTransItem(item, session);
            }
            LogTransactionInfo();
            Log.Info("Transaction completed");
        }

        /// <summary>
        /// Updates/Adds/Delete set of objects to support atomic transaction
        /// </summary>
        /// <param name="transArray">list of TransItem objects</param>
        /// <param name="objIDList">identities for the objects</param>
        /// <returns>Boolean indicating success or failure</returns>
        public void ExecTransaction(List<TransItem> transArray, out List<long> objIDList)
        {
            Log.Info("Executing Transaction...");
            ISession session = null;
            objIDList = new List<long>();
            session = GetSession();
            foreach (TransItem item in transArray)
            {
                ProcessTransItem(item, session);
                objIDList.Add((long)session.GetIdentifier(item.ObjectValue));
            }
            //session.Flush();
            LogTransactionInfo();
            Log.Info("Transaction completed");
        }

        /// <summary>
        /// Process each TransItem and calls appropriate methods on
        /// the NHibernate session object
        /// </summary>
        /// <param name="item">TransItem object</param>
        /// <param name="session">NHibernate session</param>
        private void ProcessTransItem(TransItem item, ISession session)
        {

            switch (item.TransType)
            {
                case transType.Add:
                    session.Save(item.ObjectValue);
                    break;
                case transType.Update:
                    session.Update(item.ObjectValue);
                    break;
                case transType.Delete:
                    session.Delete(item.ObjectValue);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Gets data object of any type based on the identity
        /// </summary>
        /// <typeparam name="T">Business entity object type</typeparam>
        /// <typeparam name="objectID">object identity</typeparam>
        /// <returns>Business entity object</returns>
        public T Get<T>(object objectID) where T : new()
        {
            T retObject = new T();
            ISession session = GetSession();
            ICriteria criteria = GetCriteria<T>(session);
            criteria.Add(Expression.IdEq(objectID));
            retObject = criteria.UniqueResult<T>();
            return retObject;
        }

        /// <summary>
        /// Determine if the SubObjectTypeField is populated.  If it is, return true.  This is 
        /// part of QC 2186 
        /// </summary>
        /// <typeparam name="crit">Criterion search object type</typeparam>
        /// <returns>Boolean</returns>
        private bool buildSubObjectList(Criterion crit)
        {
            return crit.SubObjectTypeField != null;
        }


        /// <summary>
        /// 
        /// </summary>
        private bool buildSubObjectList(SortCriterion sortCriterion)
        {
            return sortCriterion.SubObjectTypeField != null;
        }

        /// <summary>
        /// Gets a list of records based on criteriaList passed in.  Supports OR searches as well.
        /// If a sub-object search is done, HQL will be built.
        /// </summary>
        /// <typeparam name="T">Business entity object type</typeparam>
        /// <typeparam name="objectID">object identity</typeparam>
        /// <returns>Business entity object</returns>
        public List<T> GetTypeSafeList<T>(CriteriaList criteriaList) where T : new()
        {

            bool doHQLSearch = false;
            foreach (Criteria criteria in criteriaList)
            {
                Predicate<Criterion> filterBySubObject = new Predicate<Criterion>(buildSubObjectList);
                Predicate<SortCriterion> sortBySubObject= new Predicate<SortCriterion>(buildSubObjectList);
                List<Criterion> subObjectCriterionList = criteria.FindAll(filterBySubObject);
                
                List<Criterion> logicalOperatorCriterionList = criteria.FindAll(delegate(Criterion lci) { return lci.LogicalOperator.Equals(LogicalOperatorType.OR); });

                if (criteriaList.SortCriteria != null && criteriaList.SortCriteria.Count > 0)
                {
                    List<SortCriterion> subObjectSortCriterionList = criteriaList.SortCriteria.FindAll(sortBySubObject);

                    if (subObjectSortCriterionList.Count > 0)
                    {
                        doHQLSearch = true;
                        break;
                    }
                }

                if (subObjectCriterionList.Count > 0 || logicalOperatorCriterionList.Count > 0)
                {
                    doHQLSearch = true;
                    break;
                }

                
            }

          

            //No sub object searches.  Use ExecCriteria method
            if (doHQLSearch == false)
            {
                List<CriteriaItem> critList = new List<CriteriaItem>();
                foreach (Criteria criteria in criteriaList)
                {
                    foreach (Criterion criterion in criteria)
                    {
                        switch (criterion.RelationalOperator)
                        {
                            case RelationalOperatorType.EQUALS:
                            case RelationalOperatorType.GREATERTHAN:
                            case RelationalOperatorType.LESSTHAN:
                            case RelationalOperatorType.GREATERTHAN_EQUAL:
                            case RelationalOperatorType.LESSTHAN_EQUAL: 
                                critList.Add(new CriteriaItem(criterion.FieldName, criterion.FieldValue, (criteriaType)criterion.RelationalOperator));
                                break;

                            case RelationalOperatorType.STARTSWITH:
                                critList.Add(new CriteriaItem(criterion.FieldName, criterion.FieldValue, criteriaType.Like, ASA.Common.MatchMode.Start));
                                break;

                            case RelationalOperatorType.ENDSWITH:
                                critList.Add(new CriteriaItem(criterion.FieldName, criterion.FieldValue, criteriaType.Like, ASA.Common.MatchMode.End));
                                break;

                            case RelationalOperatorType.CONTAINS:
                                critList.Add(new CriteriaItem(criterion.FieldName, criterion.FieldValue, criteriaType.Like, ASA.Common.MatchMode.Anywhere));
                                break;

                            case RelationalOperatorType.IN:
                                List<object> inList = new List<object>();
                                string separator = ",";
                                string[] stringList = criterion.FieldValue.ToString().Split(separator.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

                                for (int x = 0; x < stringList.Length; x++)
                                {
                                    inList.Add((object)stringList[x]);
                                }

                                critList.Add(new CriteriaItem(criterion.FieldName, inList));
                                break;
                        }
                    }
                }

                IList<SortCriterion> sortList = null;
                if (criteriaList.SortCriteria != null && criteriaList.SortCriteria.Count > 0)
                {
                    sortList = (IList<SortCriterion>)criteriaList.SortCriteria;
                }

                //set rows to return to -1 so all rows matching criteria are returned.
                // If MaxEntities is 0 or negative number return all rows matching the criteria 
                if (criteriaList.MaxEntities < 1)
                    return ExecCriteria<T>(sortList, critList, -1);
                else
                    return ExecCriteria<T>(sortList, critList, criteriaList.MaxEntities);
            }
            else
            {
                return ExecuteHQLSearch<T>(criteriaList);
            }

        }

        /// <summary>
        /// Gets a list of records based on criteriaList passed in, using HQL.
        /// Supports OR searches as well.
        /// </summary>
        /// <typeparam name="T">Business entity object type</typeparam>
        /// <typeparam name="criteriaList">list of TypeSafe Criteria objects</typeparam>
        /// <returns>list of Business entity objects</returns>
        private List<T> ExecuteHQLSearch<T>(CriteriaList criteriaList) where T : new()
        {
            List<T> businessEntitiesList;
            T retObject = new T();

            List<HQLParameterItem> masterHQLParameterItemList = new List<HQLParameterItem>();

            Dictionary<string, object> paramDictionary = new Dictionary<string, object>();

            int i = 0;

            string whereClause = " WHERE ";

            //used when building out where clause.  Need to know index of all entities in the search
            //since parameters are named param0, param1, etc.  Parent entity is always first (index=0) 
            List<string> entityNameList = new List<string>();

            //add parent entity type to entityNameList
            entityNameList.Add(retObject.GetType().Name);

            foreach (Criteria criteria in criteriaList)
            {
                criteriaType TargetOperationalOperator;
                List<HQLParameterItem> HQLParameterItemList = new List<HQLParameterItem>();

                string criteriaHQL = "";

                //if this is not the first criteria in the criteria list, supply the () and correct operator
                if (criteriaList.IndexOf(criteria) > 0)
                {
                    criteriaHQL += " " + criteria.LogicalOperator.ToString() + " (";
                }

                foreach (Criterion criterion in criteria)
                {
                    switch (criterion.RelationalOperator)
                    {
                        case RelationalOperatorType.EQUALS: TargetOperationalOperator = criteriaType.Equal;
                            break;
                        case RelationalOperatorType.STARTSWITH: TargetOperationalOperator = criteriaType.LikeStartsWith;
                            break;
                        case RelationalOperatorType.ENDSWITH: TargetOperationalOperator = criteriaType.LikeEndsWith;
                            break;
                        case RelationalOperatorType.CONTAINS: TargetOperationalOperator = criteriaType.Like;
                            break;
                        case RelationalOperatorType.IN: TargetOperationalOperator = criteriaType.In;
                            break;
                        case RelationalOperatorType.GREATERTHAN: TargetOperationalOperator = criteriaType.GreaterThan;
                            break;
                        case RelationalOperatorType.GREATERTHAN_EQUAL: TargetOperationalOperator = criteriaType.GreaterThanEqual;
                            break;
                        case RelationalOperatorType.LESSTHAN: TargetOperationalOperator = criteriaType.LessThan;
                            break;
                        case RelationalOperatorType.LESSTHAN_EQUAL: TargetOperationalOperator = criteriaType.LessThanEqual;
                            break;

                        default: throw new Exception(String.Format("% is not supported as relation operator", criterion.RelationalOperator));

                    }

                    if (criterion.SubObjectTypeField != null && entityNameList.Contains(criterion.SubObjectTypeField) == false)
                    {
                        entityNameList.Add(criterion.SubObjectTypeField);
                    }

                    HQLParameterItemList.Add(new HQLParameterItem(criterion.FieldName, criterion.FieldValue, TargetOperationalOperator, criterion.LogicalOperator.ToString(), criterion.SubObjectTypeField));

                }
             

                // build out where clause
                foreach (HQLParameterItem hqlItem in HQLParameterItemList)
                {
                    paramDictionary.Add("param" + i, hqlItem.FieldValue);

                    criteriaHQL = CreateHQLWhereClause(criteriaHQL, i, hqlItem, HQLParameterItemList, entityNameList);
                    i++;
                }

                //if this is not the first criteria in the criteria list, supply the ) to end grouping
                if (criteriaList.IndexOf(criteria) > 0)
                {
                    criteriaHQL += ")";
                }

                whereClause += criteriaHQL;
                masterHQLParameterItemList.AddRange(HQLParameterItemList);

            }


            // HMF Beging Sort Criteria Support

            List<string> sortCriterionSubObjectNameList = new List<String>();

            if (criteriaList.SortCriteria != null && criteriaList.SortCriteria.Count > 0)
            {
                foreach (SortCriterion sortCriterion in  criteriaList.SortCriteria )
                {
                    if (sortCriterion.SubObjectTypeField != null && entityNameList.Contains(sortCriterion.SubObjectTypeField) == false)
                    {
                        entityNameList.Add(sortCriterion.SubObjectTypeField);
                        sortCriterionSubObjectNameList.Add(sortCriterion.SubObjectTypeField);
                       
                    }
                }

                string orderByClause = " ORDER BY ";
                int x = 0;

                foreach (SortCriterion sortCriterion in criteriaList.SortCriteria)
                {


                    //determine if sort goes with parent or sub-object
                    if (sortCriterion.SubObjectTypeField == null)
                    {
                        orderByClause += " parent.";
                    }
                    else
                    {
                        orderByClause += " sub" + entityNameList.IndexOf(sortCriterion.SubObjectTypeField) + ".";
                    }

                    if (x > 0)
                    {
                        orderByClause += ",";
                    }
                    if (sortCriterion.SortDirection == SortDirection.Ascending)
                    {

                        orderByClause += sortCriterion.FieldName + " ASC";
                    }
                    else
                    {
                        orderByClause += sortCriterion.FieldName + " DESC";
                    }
                    x++;
                }
                whereClause += orderByClause;
            }

            // HMF End Sort Criteria Support

            if (criteriaList.MaxEntities > 0)
                businessEntitiesList = GetSubObjectFilteredList<T>(masterHQLParameterItemList, paramDictionary, whereClause, criteriaList.MaxEntities,sortCriterionSubObjectNameList);
            else
                businessEntitiesList = GetSubObjectFilteredList<T>(masterHQLParameterItemList, paramDictionary, whereClause, -1,sortCriterionSubObjectNameList);
            return businessEntitiesList;
        }

        /// <summary>
        /// Gets data object of any type based criteria of a sub object collection
        /// </summary>
        /// <typeparam name="T">Business entity object type</typeparam>
        /// <param name="hqlSearchCrit">List of HQL parameter items that contain the search criteria
        /// of the parent object (not required) and sub object</param>
        /// <typeparam name="results"># of records to return. -1 returns all records</typeparam>
        /// <returns>Business entity object</returns>
        private List<T> GetSubObjectFilteredList<T>(List<HQLParameterItem> hqlSearchCrit, Dictionary<string, object> paramDictionary, string whereClause, int results, List<string> sortCriterionSubObjecNametList) where T : new()
        {
            T retObject = new T();

            List<T> list = null;

            string parentObjectName = retObject.GetType().Name;

            String hql = "";
            int i = 0;

            List<string> objectNameList = new List<string>();

            foreach (HQLParameterItem param in hqlSearchCrit)
            {
                if (param.SubObjectType == null && objectNameList.Contains(parentObjectName) == false)
                {
                    hql += "FROM " + parentObjectName + " parent";
                    objectNameList.Add(parentObjectName);
                    i++;
                }
                else if (param.SubObjectType != null && objectNameList.Contains(param.SubObjectType) == false)
                {
                    if (i == 0)
                    {
                        //the first table is always the parent.  In this case, the only
                        //filter is on the sub-object
                        i = 1;
                    }

                    hql += " inner join fetch parent." + param.SubObjectType + " sub" + i;
                    objectNameList.Add(param.SubObjectType);
                    i++;
                }
            }

            // Accounts for criterion sub objects in the From class/table names 
            foreach (string sortCriterionSubObject in  sortCriterionSubObjecNametList)
            {
                if (objectNameList.Contains(sortCriterionSubObject) == false)
                {
                    hql += " inner join fetch parent." + sortCriterionSubObject + " sub" + i;
                     objectNameList.Add(sortCriterionSubObject);
                    i++;
                }
            }
            

            //if no parent search params were requested, add parent to HQL
            if (objectNameList.Contains(parentObjectName) == false)
            {
                hql = "FROM " + parentObjectName + " parent" + hql;
            }

            hql += whereClause;

            IQuery query = GetSession().CreateQuery(hql);

            //set results for query
            query.SetMaxResults(results);

            i = 0;

            foreach (HQLParameterItem param in hqlSearchCrit)
            {
                if (param.SearchType == criteriaType.In)
                {
                    string separator = ",";
                    string[] stringList = param.FieldValue.ToString().Split(separator.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                    object[] array = new object[stringList.Length];

                    for (int x = 0; x < stringList.Length; x++)
                    {
                        array[x] = Convert.ChangeType(stringList[x], typeof(object));
                    }
                    query.SetParameterList("param" + i, array);
                }
                else
                {
                    query.SetParameter("param" + i, param.FieldValue);
                }
                i++;
            }

            query.SetResultTransformer(new NHibernate.Transform.DistinctRootEntityResultTransformer());

            list = query.List<T>() as List<T>;

            return list;

        }

        ///// <summary>
        ///// CreateHQLWhereClause
        ///// </summary>
        ///// <param name="whereClause">the where clause that has been created up to this point</param>
        ///// <param name="i">a counter that identifies the item's place in the order of the parameter list</param>
        ///// <param name="param">the HQL param item we need to extract data from</param>
        ///// <returns>an updated where clause</returns>
        public static string CreateHQLWhereClause(string whereClause, int i, HQLParameterItem param, List<HQLParameterItem> paramList, List<string> objectNameList)
        {
            //if not first time thru loop add AND to clause
            if (paramList.IndexOf(param) > 0)
            {
                whereClause += " " + param.LogicalOperator;
            }

            //determine if search param goes with parent or sub-object
            if (param.SubObjectType == null)
            {
                whereClause += " parent.";
            }
            else
            {
                whereClause += " sub" + objectNameList.IndexOf(param.SubObjectType) + ".";
            }

            string searchOperator = Enums.GetStringValue(param.SearchType);

            switch (param.SearchType)
            {
                case criteriaType.In:
                    whereClause += param.FieldName + " in (:param" + i + ")";
                    break;

                case criteriaType.Like:
                case criteriaType.LikeStartsWith:
                case criteriaType.LikeEndsWith:
                    whereClause += param.FieldName + " like :param" + i;
                    break;

                case criteriaType.GreaterThan:
                    whereClause += param.FieldName + " > :param" + i;
                    break;

                case criteriaType.GreaterThanEqual:
                    whereClause += param.FieldName + " >= :param" + i;
                    break;

                case criteriaType.LessThan:
                    whereClause += param.FieldName + " < :param" + i;
                    break;

                case criteriaType.LessThanEqual:
                    whereClause += param.FieldName + " <= :param" + i;
                    break;

                case criteriaType.Not:
                    whereClause += param.FieldName + " != :param" + i;
                    break;

                default:
                    whereClause += param.FieldName + searchOperator + ":param" + i;
                    break;
            }

            return whereClause;
        }

        /* Test method related to sub object testing - ignore for review*/
        /// <summary>
        /// Filter collection based on criteria
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <param name="filterCriteria"></param>
        /// <returns></returns>
        public IList<T> FilterCollection<T>(object collection, string filterCriteria)
        {
            ICollection<T> orderedCollection = GetSession().CreateFilter(collection, filterCriteria).List() as
                ICollection<T>;
            return orderedCollection as IList<T>;
        }

        /// <summary>
        /// Gets all object of Type T
        /// </summary>
        /// <typeparam name="T">Business entity type</typeparam>
        /// <returns>List of business entity types</returns>
        public IList<T> GetAll<T>()
        {
            return (GetAll<T>(null, null, -1));
        }

        /// <summary>
        /// Executes the criteria query
        /// </summary>
        /// <typeparam name="T">Business object type</typeparam>
        /// <param name="sortCriteria"> sorting order</param>
        /// <param name="critList">list of parameters to be set
        /// in the where class</param>
        /// <param name="maxResults">Maximum no of results</param>
        /// <returns>List of business entities that matches the criteria 
        /// and sorted as per the sort criteria</returns>
        public List<T> ExecCriteria<T>(IList<SortCriterion> sortCriteria,
            List<CriteriaItem> critList, int maxResults)
        {
            List<ICriterion> queryCriterion = new List<ICriterion>();
            AddCriteria(ref queryCriterion, critList);
            return GetAll<T>(sortCriteria, queryCriterion, maxResults) as List<T>;
        }


        /// <summary>
        /// Executes the criteria query when doing the following query types: "OR", "AND"
        /// </summary>
        /// <typeparam name="T">Business object type</typeparam>
        /// <param name="sortCriteria"> sorting order</param>
        /// <param name="critListLHS">list of parameters to be set
        /// on the LEFT hand side of the AND/OR clause within the WHERE clause </param>
        /// <param name="critListRHS">list of parameters to be set
        /// on the RIGHT hand side of the AND/OR clause within the WHERE clause </param>
        /// <param name="maxResults">Maximum no of results</param>
        /// <returns>List of business entities that matches the criteria 
        /// and sorted as per the sort criteria</returns>
        //public List<T> ExecCriteria<T>(IList<SortCriteria> sortCriteria, criteriaType queryType,
        //    List<CriteriaItem> critListLHS, List<CriteriaItem> critListRHS, int maxResults)
        //{
        ///////////////////////////////////////////////////
        /// !!!DO NOT USE.  THIS IS A WORK IN PROGRESS!!!!
        ///////////////////////////////////////////////////

        ////Master query Criterion
        //List<ICriterion> queryCriterion = new List<ICriterion>();
        ////Query Criterion for left hand side of AND/OR statement
        //List<ICriterion> criterionLHS = new List<ICriterion>();
        ////Query Criterion for right hand side of AND/OR statement
        //List<ICriterion> criterionRHS = new List<ICriterion>();

        //AddCriteria(ref criterionLHS, critListLHS);
        //if (critListRHS.Count > 0)
        //{
        //    AddCriteria(ref criterionRHS, critListRHS);
        //}

        //switch (queryType)
        //{
        //    case criteriaType.And:
        //        queryCriterion.Add(Expression.And(criterionLHS[0], criterionRHS[0]));
        //        break;
        //    case criteriaType.Or:
        //        queryCriterion.Add(Expression.Or(criterionLHS[0], criterionRHS[0]));
        //        break;
        //    case criteriaType.Not:
        //        queryCriterion.Add(Expression.Not(criterionLHS[0]));
        //        break;

        //    default:
        //        break;
        //}

        //return GetAll<T>(sortCriteria, queryCriterion, maxResults) as List<T>;
        //}        

        /// <summary>
        /// Adds criteria items to NHibernate's criteria interface
        /// </summary>
        /// <param name="queryCriterion">NHibernate criteria interface list</param>
        /// <param name="critList">Criteria items to be added to the NHibernate criteria</param>
        private void AddCriteria(ref List<ICriterion> queryCriterion, List<CriteriaItem> critList)
        {
            foreach (CriteriaItem crit in critList)
            {
                switch (crit.CriteriaType)
                {
                    case criteriaType.Between:
                        queryCriterion.Add(Expression.Between(crit.FieldName, crit.LowValue, crit.HighValue));
                        break;
                    case criteriaType.Equal:
                        queryCriterion.Add(Expression.Eq(crit.FieldName, crit.FieldValue));
                        break;
                    case criteriaType.GreaterThan:
                        queryCriterion.Add(Expression.Gt(crit.FieldName, crit.FieldValue));
                        break;
                    case criteriaType.GreaterThanEqual:
                        queryCriterion.Add(Expression.Ge(crit.FieldName, crit.FieldValue));
                        break;
                    case criteriaType.In:
                        queryCriterion.Add(Expression.In(crit.FieldName, crit.InList as List<Object>));
                        break;
                    case criteriaType.LessThan:
                        queryCriterion.Add(Expression.Lt(crit.FieldName, crit.FieldValue));
                        break;
                    case criteriaType.LessThanEqual:
                        queryCriterion.Add(Expression.Le(crit.FieldName, crit.FieldValue));
                        break;
                    case criteriaType.Like:
                        queryCriterion.Add(Expression.Like(crit.FieldName, crit.FieldValue as string, crit.MatchMode));
                        break;
                    case criteriaType.LikeNoCase:
                        queryCriterion.Add(Expression.InsensitiveLike(crit.FieldName, crit.FieldValue as string, crit.MatchMode));
                        break;
                    case criteriaType.IsEmpty:
                        queryCriterion.Add(Expression.IsEmpty(crit.FieldName));
                        break;
                    case criteriaType.IsNotEmpty:
                        queryCriterion.Add(Expression.IsNotEmpty(crit.FieldName));
                        break;
                    case criteriaType.IsNull:
                        queryCriterion.Add(Expression.IsNull(crit.FieldName));
                        break;
                    case criteriaType.IsNotNull:
                        queryCriterion.Add(Expression.IsNotNull(crit.FieldName));
                        break;

                    default:
                        break;
                }
            }
        }
        /// <summary>
        /// Gets all object of Type T sorted by passed sort criteria and 
        /// where criteria
        /// </summary>
        /// <typeparam name="T">Business entity type</typeparam>
        /// <typeparam name="sortCriteria">List of SortCriteria objects</typeparam>
        /// <typeparam bane="criterion">List of ICriterion obejcts to add to query</typeparam>
        /// <returns>List of business entity types</returns>
        private IList<T> GetAll<T>(IList<SortCriterion> sortCriteria, List<ICriterion> criterion, int MaxResults)
        {
            IList<T> list = null;
            //Get Criteria for Query
            ISession session = GetSession();
            ICriteria QueryCriteria = GetCriteria<T>(session);

            if (sortCriteria != null)
            {
                //Add sort criteria if specified
                AddSortCriteria(QueryCriteria, sortCriteria);
            }

            //Add expressions
            if (criterion != null)
            {
                foreach (ICriterion CriteriaItem in criterion)
                {
                    QueryCriteria.Add(CriteriaItem);
                }
            }

            //Set max results
            if (MaxResults > 0)
            {
                QueryCriteria.SetMaxResults(MaxResults);
            }

            //Execute query
            list = ListInternal<T>(QueryCriteria);

            //CloseSession(session);

            return list;
        }

        /// <summary>
        /// Gets all objects of type T based on the passed ICriteria object
        /// </summary>
        /// <typeparam name="T">Business entity type</typeparam>
        /// <param name="criteria">Query Criteria</param>
        /// <returns>List of Business Entity Objects</returns>
        private IList<T> ListInternal<T>(ICriteria criteria)
        {
            IList<T> RetObjects = null;
            RetObjects = (IList<T>)criteria.List<T>();
            return RetObjects;
        }

        /// <summary>
        /// Add the list of sort criterias to the query criteria
        /// </summary>
        /// <param name="queryCriteria">Criteria object to be added to query</param>
        /// <param name="sortCriteria">sorting order to be added to the query criteria</param>
        private void AddSortCriteria(ICriteria queryCriteria, IList<SortCriterion> sortCriteria)
        {
            //Add sort criteria


            foreach (SortCriterion sortCriteriaItem in sortCriteria)
            {
                if (sortCriteriaItem.SortDirection == SortDirection.Ascending)
                {
                    //Ascending
                    queryCriteria.AddOrder(Order.Asc(sortCriteriaItem.FieldName));
                }
                else
                {
                    queryCriteria.AddOrder(Order.Desc(sortCriteriaItem.FieldName));
                    //Descending
                }
            }

            
        }

        /// <summary>
        /// Logs the transaction identifiers, both local and distributed
        /// </summary>
        private void LogTransactionInfo()
        {
            if (Transaction.Current != null)
            {
                TransactionInformation transInfo = Transaction.Current.TransactionInformation;
                Log.InfoFormat("Local Transaction Identifier: {0}", transInfo.LocalIdentifier.ToString());
                Log.InfoFormat("Distributed Transaction Identifier: {0}", transInfo.DistributedIdentifier.ToString());
            }
        }

        /// <summary>
        /// Gets the NHibernate session through Spring.NET - required
        /// for session management. Non-transactional data access are given
        /// new sessions automatically if one doesn't exists or open
        /// </summary>
        /// <returns>NHibernate session</returns>
        private ISession GetSession()
        {
            //return SessionFactoryUtils.DoGetSession(sessionFactory, true) -do not uncomment;
            return sessionFactory.GetCurrentSession();
        }

        private void CloseSession(ISession session)
        {
            SessionFactoryUtils.CloseSession(session);
        }

    }
}
///////////////////////////////////////////////////////////////////////////////
