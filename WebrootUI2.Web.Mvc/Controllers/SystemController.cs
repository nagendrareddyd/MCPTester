using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using WebrootUI2.Domain.Contracts.Tasks;
using WebrootUI2.Domain.Models;
using WebrootUI2.Infrastructure.Common.Log;
using WebrootUI2.Web.Mvc.Controllers.Queries;
using WebrootUI2.Web.Mvc.Controllers.ViewModels;
using WebrootUI2.Web.Mvc.Filters;
using WebrootUI2.Domain;
using WebrootUI2.Resources;

namespace WebrootUI2.Web.Mvc.Controllers
{
    [Authorize]
    [Permissions(AllowedPermissions = new Common.AdminPermission[]{
    Common.AdminPermission.EditSystemSettings})]
    public class SystemController : BaseController
    {
        private readonly ILogEventTask logEventTask;
        private readonly IUserTask userTask;
        private readonly IRoleTask roleTask;
        private readonly RoleQuery roleQuery;
        private readonly UserQuery userQuery;
        private readonly IAcquirerTask acquireTask;

        public SystemController(ILogEventTask logEventTask, IUserTask userTask, RoleQuery companyRoleQuery,
        UserQuery userQuery,IRoleTask roleTask,IAcquirerTask acquireTask)
        {
            this.userTask = userTask;
            this.logEventTask = logEventTask;
            this.roleQuery = companyRoleQuery;
            this.userQuery = userQuery;
            this.roleTask = roleTask;
            this.acquireTask = acquireTask;
        }


        #region Users list

        /// <summary>
        /// Load the Users list in the Audit view.
        /// </summary>
        public ActionResult Index()
        {
            var auditModel = new AuditModel() { Username = string.Empty, RoleName = string.Empty };

            if (Request.QueryString["isCa"] != null && Request.QueryString["isCa"] == "true" && HttpContext.Cache["auditModel"] != null)
            {
                var cachedAuditModel = (AuditModel)HttpContext.Cache["auditModel"];

                auditModel.Username = cachedAuditModel.Username;
                auditModel.RoleName = cachedAuditModel.RoleName;
                auditModel.TotalRecordsCount = cachedAuditModel.Users.Count;
                auditModel.Users = cachedAuditModel.Users.Take(Setting.Page_Size).ToList<UserModel>();
            }
            else
            {
                var users = new List<UserModel>();

                foreach (var user in userTask.GetAdminUsers())
                {
                    users.Add(new UserModel()
                    {
                        Name = user.UserName,
                        RoleName = user.Role.Name,
                        Id = user.UserId
                    });
                }

                auditModel.TotalRecordsCount = users.Count();
                auditModel.Users = users.Take(Setting.Page_Size).ToList<UserModel>();
                HttpContext.Cache["auditModel"] = new AuditModel() { Users = users };
            }

            return View(auditModel);
        }

        /// <summary>
        /// Laoding Acquire user list
        /// </summary>
        public ActionResult Acquirer()
        {
            var acquiremodel = new Acquire { Enabled = false, LogicalId = 0, name = string.Empty };
            if (HttpContext.Cache["acquireModel"] != null)
            {
                var cachedAcquireModel = (Acquire)HttpContext.Cache["acquireModel"];

                acquiremodel.name = cachedAcquireModel.name;
                acquiremodel.LogicalId = cachedAcquireModel.LogicalId;
                acquiremodel.TotalRecordsCount = cachedAcquireModel.Acquires.Count;
                acquiremodel.Acquires = cachedAcquireModel.Acquires.Take(Setting.Page_Size).ToList<Acquire>();
            }
            else
            {
                acquiremodel.Acquires = acquireTask.GetAllAquires().ToList();
                acquiremodel.TotalRecordsCount = acquiremodel.Acquires.Count;
                HttpContext.Cache["acquireModel"] = new Acquire() { Acquires = acquiremodel.Acquires.ToList() };
                acquiremodel.Acquires = acquiremodel.Acquires.Take(Setting.Page_Size).ToList();
            }
            return View(acquiremodel);
        }
        
        public ActionResult SaveAcquire()
        {
            var acquires = new List<Acquire>();
            var count = 0;
            var name = Request.Form.GetValues(CommonResources.Acquire_name.ToLower())[0];
            var LogicalId = Convert.ToInt32(Request.Form.GetValues(CommonResources.Acquire_LogicalId.ToLower())[0]);
            var Enabled = Convert.ToBoolean(Request.Form.GetValues(CommonResources.Acquire_Enabled.ToLower())[0]);
            var _acquire = new Acquire();
            _acquire.name = name;
            _acquire.Enabled = Enabled;
            _acquire.LogicalId = LogicalId;
            acquireTask.UpdateAcquirer(_acquire);
            acquires = acquireTask.GetAllAquires();
            HttpContext.Cache["acquireModel"] = new Acquire() { name = name, LogicalId = LogicalId, Acquires = acquires };
            count = acquires.Count;
            return RedirectToAction("Acquirer");
        }
        /// <summary>
        /// Search Acquirers
        /// </summary>
        [HttpGet]
        public JsonResult AcquirerSearch(string name,int LogicalId)
        {

            var acquires = new List<Acquire>();
            var count = 0;

            acquires =  acquireTask.Search(name == null ? string.Empty : name, LogicalId);
            HttpContext.Cache["acquireModel"] = new Acquire() { name = name, LogicalId = LogicalId, Acquires = acquires };
            count = acquires.Count;

            return Json(new { status = "success", acquireList = acquires.Take(Setting.Page_Size).ToList<Acquire>(), recordsCount = count }
                , JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// Search Users in the Audit 
        /// </summary>
        [HttpGet]
        public JsonResult UsersSearch(string username, string role)
        {

            var users = new List<UserModel>();
            var count = 0;

            var usersQuery = userTask.Search(username == null ? string.Empty : username, role == null ? string.Empty : role);

            foreach (var user in usersQuery)
            {
                users.Add(new UserModel()
                {
                    Name = user.UserName,
                    RoleName = user.Role.Name,
                    Id = user.UserId
                });
            }

            HttpContext.Cache["auditModel"] = new AuditModel() { Username = username, RoleName = role, Users = users };
            count = users.Count;

            return Json(new { status = "success", userList = users.Take(Setting.Page_Size).ToList<UserModel>(), recordsCount = count }
                , JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Fetch data from the cached users list in the page index change
        /// </summary>
        [HttpGet]
        public JsonResult AcquiresPagingIndexChanged(int index)
        {
            var count = 0;
            var users = new List<Acquire>();
            var cachedAuditModel = (Acquire)HttpContext.Cache["acquireModel"];

            if (cachedAuditModel == null)
                return Json(new { status = "failed" });

            users = cachedAuditModel.Acquires.Skip((index - 1) * Setting.Page_Size).Take(Setting.Page_Size).ToList<Acquire>();
            count = cachedAuditModel.Acquires.Count;

            return Json(new { status = "success", acquireList = users, currentIndex = index, recordsCount = count }, JsonRequestBehavior.AllowGet);

        }

        /// <summary>
        /// Fetch data from the cached users list in the page index change
        /// </summary>
        [HttpGet]
        public JsonResult PagingIndexChanged(int index)
        {
            var count = 0;
            var users = new List<UserModel>();
            var cachedAuditModel = (AuditModel)HttpContext.Cache["auditModel"];

            if (cachedAuditModel == null)
                return Json(new { status = "failed" });

            users = cachedAuditModel.Users.Skip((index - 1) * Setting.Page_Size).Take(Setting.Page_Size).ToList<UserModel>();
            count = cachedAuditModel.Users.Count;

            return Json(new { status = "success", usersList = users, currentIndex = index, recordsCount = count }, JsonRequestBehavior.AllowGet);

        }

        #endregion

        #region Logs

        /// <summary>
        /// Load Audit logs of the selected user.
        /// </summary>
        public ActionResult Audit()
        {
            var userId = Guid.Empty;

            if (Request.QueryString["userId"] == null ||
               !Guid.TryParse(Request.QueryString["userId"].ToString(), out userId))
            {
                TempData["error"] = "Inavlid url";
                return View(new List<LogEvent>());
            }

            Session["userId"] = userId;
            var logs = new List<LogModel>();

            var eventsQuery = logEventTask.GetLogByUserId(userId);

            foreach (var eventLog in eventsQuery)
            {
                logs.Add(new LogModel() { DisplayDate = eventLog.Date.ToString(), Level = eventLog.Level, Message = eventLog.Message });
            }

            HttpContext.Cache["Logs"] = logs;
            ViewBag.recordsCount = logs.Count;

            return View(logs.Take(Setting.Page_Size).ToList<LogModel>());
        }

        /// <summary>
        /// Fetch data from the cached log list in the page index change
        /// </summary>
        [HttpGet]
        public JsonResult AuditPagingIndexChanged(int index)
        {
            var count = 0;
            var logs = new List<LogModel>();

            var cachedLogs = (List<LogModel>)HttpContext.Cache["Logs"];

            if (cachedLogs == null)
                return Json(new { status = "failed" });

            logs = cachedLogs.Skip((index - 1) * Setting.Page_Size).Take(Setting.Page_Size).ToList<LogModel>();
            count = cachedLogs.Count;


            return Json(new { status = "success", logEvents = logs, currentIndex = index, recordsCount = count }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Search audit log events.
        /// </summary>
        [HttpGet]
        public JsonResult AuditSearch(DateTime? from, DateTime? to, string level, string message)
        {
            var userId = Guid.Empty;
            var logs = new List<LogModel>();

            if (!Guid.TryParse(Session["userId"].ToString(), out userId))
                return Json(new { status = "failed" });

            var eventList = logEventTask.Search(from, to, level, message, userId);

            foreach (var _event in eventList)
            {
                logs.Add(new LogModel() { DisplayDate = _event.Date.ToString(), Level = _event.Level, Message = _event.Message });
            }

            HttpContext.Cache["Logs"] = logs;
            return Json(new { status = "success", logEvents = logs.Take(Setting.Page_Size), recordsCount = logs.Count },
                JsonRequestBehavior.AllowGet);
        }

        #endregion
    }
}
