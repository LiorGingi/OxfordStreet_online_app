using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OxfordStreet_online_app.Models;

namespace OxfordStreet_online_app.ViewModels
{
    public class EmployeeUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public bool IsEmployee { get; set; }
        public string Password { get; set; } //need to add password encrypt and decrypt
        public int MonthlySalary { get; set; }
        public Branch Branch { get; set; }
        public Role Role { get; set; }
        public bool IsManager { get; set; }

    }
}