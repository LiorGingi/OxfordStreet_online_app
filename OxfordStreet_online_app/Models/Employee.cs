using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Web;

namespace OxfordStreet_online_app.Models
{
    public class Employee
    {
        public int EmployeeId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Range(5300, int.MaxValue, ErrorMessage = "Monthly salary should be greater than or equal to 5300NIS (minimum wage).")]
        [Required]
        public int MonthlySalary { get; set; }

        [Required]
        public int BranchId { get; set; }

        public string Role { get; set; } //need to pre-define role types
        public bool IsManager { get; set; }

        //RELATIONS
        public User User { get; set; }//foreign key
        public Branch Branch { get; set; }//foreign key
    }
}