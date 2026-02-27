using System;
using System.Collections.Generic;
using System.Text;

namespace LibraryManagementSystem.Core.Request
{
    public class CreateBooksRequest
    {
        public required string BookName { get; set; }
        public required string Publisher { get; set; }
        public required string Author { get; set; }
        public required decimal Price { get; set; }
        public int CategoryId { get; set; }
    }
}
