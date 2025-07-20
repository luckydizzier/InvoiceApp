using System;

namespace InvoiceApp.Application.DTOs
{
    public class UnitDto
    {
        public int Id { get; set; }
        public string? Code { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
