using System;
namespace InvoiceApp.Domain
{
    public class ChangeLog : Base
    {
        public string Entity { get; set; } = string.Empty;
        public string Operation { get; set; } = string.Empty;
        public string Data { get; set; } = string.Empty;
    }
}
