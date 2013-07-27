using NHibernate;
using NHibernate.Criterion.Lambda;
using SharpArch.Domain.PersistenceSupport;
using SharpArch.NHibernate;
using SharpArch.NHibernate.Contracts.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Web.Security;
using WebrootUI2.Domain;
using WebrootUI2.Domain.Contracts.Tasks;
using WebrootUI2.Domain.Models;
using WebrootUI2.Infrastructure.Common.Log;


namespace WebrootUI2.Tasks
{
    public class AcquirerTask : IAcquirerTask
    {
        private readonly IRepository<Acquire> acquirerRepo;

        public AcquirerTask(IRepository<Acquire> acquirerRepo)
        {
            this.acquirerRepo = acquirerRepo;
        }

        #region Administrator

        public List<Acquire> Search(string name,int? LogicalId)
        {
            var users = new List<Acquire>();

            try
            {
                var allUsersQuery = GetAllAquires();

                users = (from u in allUsersQuery
                         where (
                             (name == string.Empty || u.name.ToLower().Contains(name.Trim().ToLower())) &&
                             (LogicalId == 0 || u.LogicalId == LogicalId))
                         select u).ToList<Acquire>();

                return users;
            }
            catch (Exception ex)
            {
                LogManager.LogException(ex);
                return new List<Acquire>();
            }
        }

        public List<Acquire> GetAllAquires()
        {
            var users = new List<Acquire>();

            if (Setting.AdministratorId == null)
                return new List<Acquire>();

            try
            {
                users = acquirerRepo.GetAll()
                    .ToList<Acquire>();

                return users;
            }
            catch (Exception ex)
            {
                LogManager.LogException(ex);
                return new List<Acquire>();
            }
        }

    
        #endregion

        public Acquire GetAcquirerById(int acquireId)
        {
            var acquirer = new Acquire();

            try
            {
                acquirerRepo.DbContext.BeginTransaction();
                acquirer = acquirerRepo.GetAll().Single(u => u.Id == acquireId);
                acquirerRepo.DbContext.CommitTransaction();

                return acquirer;
            }
            catch (Exception ex)
            {
                LogManager.LogException(ex);
                return new Acquire();
            }
        }

        public bool Delete(int acquirerId)
        {
            var acquirer = new Acquire();

            try
            {
                acquirerRepo.DbContext.BeginTransaction();

                acquirer = acquirerRepo.GetAll().Single(u => u.Id == acquirerId);
                acquirerRepo.Delete(acquirer);

                acquirerRepo.DbContext.CommitTransaction();

                return true;
            }
            catch (Exception ex)
            {
                LogManager.LogException(ex);
                return false;
            }
        }

       
        public bool UpdateAcquirer(Acquire acquire)
        {
            try
            {
                acquirerRepo.DbContext.BeginTransaction();
                acquirerRepo.SaveOrUpdate(acquire);
                acquirerRepo.DbContext.CommitTransaction();

                return true;
            }
            catch (Exception ex)
            {
                LogManager.LogException(ex);
                return false;
            }
        }
    }
}
