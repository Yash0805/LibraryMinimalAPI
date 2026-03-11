using System;
using System.Collections.Generic;
using System.Text;

namespace LibraryManagementSystem.Core.Request
{
    public class PatchRenewedBookIssueRequest
    {
        public DateOnly ReturnDate { get; set; }
        public int RenewCount { get; set; }
        public DateOnly RenewDate { get; set; }
        public string Status { get; set; }
    }
}
