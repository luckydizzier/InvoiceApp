using System;

namespace InvoiceApp.Domain
{
    public class Unit : Base
    {
        public string? Code { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
