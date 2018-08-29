using System;
using System.Collections.Generic;
using ASA.Common;
namespace ASA.DataAccess
{
    [CLSCompliant(false)]
    public interface IBaseDAO
    {
        void Add(object obj);
        void Delete(object obj);
        void DeleteSubObject<T>(IList<T> subObjectList, int index);
        List<T> ExecCriteria<T>(IList<SortCriterion> sortCriteria, List<CriteriaItem> critList, int maxResults);
        void ExecTransaction(List<TransItem> transArray);
        System.Collections.Generic.IList<T> FilterCollection<T>(object collection, string filterCriteria);
        IList<T> GetAll<T>();
        List<T> GetTypeSafeList<T>(CriteriaList criteriaList) where T : new();
        T Get<T>(object objectID) where T : new();
        void Update(object obj);
    }
}
