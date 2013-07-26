using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpArch.Domain.DomainModel;
using System.Web.Script.Serialization;

namespace WebrootUI2.Domain.Models
{
    public class Acquire : Entity
    {
        public Acquire()
        {

        }
        public virtual int Id { get; set; }
        public virtual bool Enabled { get; set; }
        public virtual int LogicalId { get; set; }
        public virtual string name { get; set; }
           
    }
}