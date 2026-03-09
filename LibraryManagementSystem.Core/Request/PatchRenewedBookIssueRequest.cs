using System;
using System.Collections.Generic;
using System.Text;

namespace LibraryManagementSystem.Core.Request
{
    public class PatchRenewedBookIssueRequest
    {
        public required DateOnly ReturnDate { get; set; }
        public required int RenewCount { get; set; }
        public required DateOnly RenewDate { get; set; }
        public required string Status { get; set; }
    }
}
