using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace LibraryManagementSystem.Core.Request
{
    public class CreateMemberRequest
    {
        public required string MemberName { get; set; }
        public required string MemberType { get; set; }
    }
}
