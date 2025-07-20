using System;

namespace InvoiceApp.DTOs
{
    public class ChangeLogDto
    {
        public int Id { get; set; }
        public string Entity { get; set; } = string.Empty;
        public string Operation { get; set; } = string.Empty;
        public string Data { get; set; } = string.Empty;
        public DateTime DateCreated { get; set; }
    }
}
