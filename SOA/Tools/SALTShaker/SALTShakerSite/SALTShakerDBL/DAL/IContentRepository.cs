using System;
using System.Collections.Generic;

using SALTShaker.DAL.DataContracts;


namespace SALTShaker.DAL
{
    public interface IContentRepository : IDisposable
    {
        List<ContentModel> GetContentItems();
    }
}
