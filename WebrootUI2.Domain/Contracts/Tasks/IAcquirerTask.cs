using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebrootUI2.Domain.Models;

namespace WebrootUI2.Domain.Contracts.Tasks
{
    public interface IAcquirerTask
    {
        List<Acquire> Search(string name, int LogicalId);
        List<Acquire> GetAllAquires();
        Acquire GetAcquirerById(int AcquirerId);
        bool UpdateAcquirer(Acquire acquirer);
        bool Delete(int acquirerId);
    }
}
