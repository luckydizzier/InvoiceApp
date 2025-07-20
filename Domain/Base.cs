using System;
namespace InvoiceApp.Domain
{
    public class Base
    {
        public int Id { get; set; }
        public bool Active { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateUpdated { get; set; }
    }
}
