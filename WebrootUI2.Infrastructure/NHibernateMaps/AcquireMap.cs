using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Mapping;
using WebrootUI2.Domain;
using WebrootUI2.Domain.Models;

namespace WebrootUI2.Infrastructure.NHibernateMaps
{
    class AcquireMap : ClassMap<Acquire>
    {
        public AcquireMap()
        {
            this.Table("MCP_Acquirer");
            this.Id(u => u.Id, "Id");
            this.Map(u => u.name, "name");
            this.Map(u => u.LogicalId, "LogicalId");
            this.Map(u => u.Enabled, "Enabled");

            //this.Component(u => u.Audit);
            //this.Component(u => u.UserCategory);

            //this.References(u => u.Role).Column("RoleId");
            //HasMany(x => x.LogEvents).Table("[s_EventLog]");
        }
    }
}
