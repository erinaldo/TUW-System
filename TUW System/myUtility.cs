using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TUW_System
{
    class LogIn
    {
        public string UserName { get; set; }
        public string EmployeeCode { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Position { get; set; }
        public List<LogIn_Form> Forms { get; set; }
    }
    class LogIn_Form
    {
        public string FormName { get; set; }
        public bool CanSave { get; set; }
        public bool CanPrint { get; set; }
    }
}
