using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;
using UsersAndRolesMVC.ViewModels;
using SharedModels;

namespace UsersAndRolesMVC.Controllers
{
    public class TasksController : Controller
    {
        ApplicationDbContext context;
        public TasksController()
        {
            context = new ApplicationDbContext();
        }
        [Authorize(Roles = "Admin")]
        // GET: Roles
        public ActionResult Index(int? taskId)
        {
            var viewModel = new TasksUsers();
            viewModel.Tasks = context.Tasks.Include(x => x.Users);
            if (taskId != null)
            {
                viewModel.Users = viewModel.Tasks.Where(t => t.Id == taskId).Single().Users;
                ViewBag.Row = taskId;
            }
            return View(viewModel);
        }
        [Authorize]
        public ActionResult MyTasks()
        {
            string id = User.Identity.GetUserId();
            var userTasks = context.Users.Include(t => t.Tasks).Where(u => u.Id == id).Single().Tasks;
            return View(userTasks);
        }
    }
}