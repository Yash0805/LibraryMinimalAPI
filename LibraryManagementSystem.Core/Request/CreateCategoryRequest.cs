using System;
using System.Collections.Generic;
using System.Text;

namespace LibraryManagementSystem.Core.Request
{
    public class CreateCategoryRequest
    {
        public required string CategoryName { get; set; }
    }
}
