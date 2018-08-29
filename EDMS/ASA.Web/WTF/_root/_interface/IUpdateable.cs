using System;

namespace ASA.Web.WTF
{
    /// <summary>
    /// IUpdatable
    /// Objects marked as IUpdatable promise to provide features for saving,
    /// updating and managing the data contained in the object. 
    /// 
    /// An object marked as IUpdatable must implment all methods in order to be
    /// used as part of the Web Transaction Framework. 
    /// 
    /// </summary>
    public interface IUpdateable
    {
        /// <summary>
        /// Save() - In an implmentation should save all underlying saveable
        /// data and do housekeeping to esure the restore data is updated
        /// </summary>
        /// <returns>true = success; false = failure</returns>
        Boolean Save();

        /// <summary>
        /// SetValues(IContextDataObject) - Method for accepting a complete data object
        /// and using its contents to update the internal state of the IUpdateable object
        /// Update should NOT automatically save changes to the repository. That should only
        /// happen then Save() is called.
        /// 
        /// Expected behavior for passing null is to clear out all object values to null or defaults while
        /// maintaining key identifiers. 
        /// </summary>
        /// <param name="data">Data Object to populate internal data with. Implmented type must 
        /// match the type of data object being used by IUpdateable.</param>
        /// <returns>true = success; false = failure</returns>
        Boolean SetValues<TModel>(TModel data) where TModel : IContextDataObject;

        Boolean IsDirty { get; }

    }
}