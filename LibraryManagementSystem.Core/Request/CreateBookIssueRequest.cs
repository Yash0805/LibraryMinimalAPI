using System;
using System.Collections.Generic;
using System.Text;

namespace LibraryManagementSystem.Core.Request
{
    public class CreateBookIssueRequest
    {
        public int MemberId { get; set; }
        public int BookId { get; set; }
        public required DateOnly IssueDate { get; set; }
        public required DateOnly ReturnDate { get; set; }
        public int RenewCount { get; set; }
        public DateOnly? RenewDate { get; set; }
        public string Status { get; set; } 
    }
}
