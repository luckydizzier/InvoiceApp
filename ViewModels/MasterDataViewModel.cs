using System;

namespace InvoiceApp.ViewModels
{
    public abstract class MasterDataViewModel<T> : EntityCollectionViewModel<T> where T : class
    {
        protected MasterDataViewModel(bool trackChanges = false)
            : base(trackChanges)
        {
        }
    }
}
