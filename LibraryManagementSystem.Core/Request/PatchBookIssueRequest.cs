using System;
using System.Collections.Generic;
using System.Text;

namespace LibraryManagementSystem.Core.Request
{
    public class PatchBookIssueRequest
    {
        public required string Status {  get; set; }
    }
}
