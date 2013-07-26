using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebrootUI2.Domain.Models;

namespace WebrootUI2.Web.Mvc.Controllers.ViewModels
{
    public class AuditModel
    {
        public AuditModel()
        {
            Users = new List<UserModel>();
        }

       public string Username { get; set; }
       public string RoleName { get; set; }
       public int TotalRecordsCount { get; set; }
       public List<UserModel> Users { get; set; }

    }
    public class AcquirerModel
    {
        public AcquirerModel()
        {
            Acquires = new List<Acquire>();
        }

        public string name { get; set; }
        public int LogicalId { get; set; }
        public bool Enabled { get; set; }
        public int Id { get; set; }
        public int TotalRecordsCount { get; set; }
        public List<Acquire> Acquires { get; set; }

    }
    public class LogModel
    {
        public string DisplayDate { get; set; }
        public string Level { get; set; }
        public string Message { get; set; }
    }
}