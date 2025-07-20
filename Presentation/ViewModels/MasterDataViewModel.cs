using System;

namespace InvoiceApp.Presentation.ViewModels
{
    public abstract class MasterDataViewModel<T> : EntityCollectionViewModel<T> where T : class
    {
        protected MasterDataViewModel(bool trackChanges = false)
            : base(trackChanges)
        {
        }
    }
}
