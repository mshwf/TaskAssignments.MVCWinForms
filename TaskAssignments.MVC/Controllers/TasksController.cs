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
using System.Net;
using System.Data.Entity.Infrastructure;

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
        public ActionResult Index(int? taskId)
        {
            var viewModel = new TasksUsers();
            viewModel.Tasks = context.Tasks.Include(x => x.Users);
            if (taskId != null)
            {
                viewModel.Users = viewModel.Tasks.Single(t => t.Id == taskId).Users;
                ViewBag.Row = taskId;
            }
            return View(viewModel);
        }
        [Authorize]
        public ActionResult MyTasks()
        {
            string id = User.Identity.GetUserId();
            var userTasks = context.Users.Include(t => t.Tasks).Single(u => u.Id == id).Tasks;
            return View(userTasks);
        }
        public ActionResult Delete(int? taskId)
        {
            var taskToDelete = context.Tasks.Single(t => t.Id == taskId);
            if (taskToDelete != null)
            {
                context.Tasks.Remove(taskToDelete);
                context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View("Index");
        }
        public ActionResult Edit(int? taskId)
        {
            if (taskId != null)
            {
                var taskToUpdate = context.Tasks.Include("Users").SingleOrDefault(t => t.Id == taskId);
                PopulateUsers();
                if (taskToUpdate != null)
                {
                    return View(taskToUpdate);
                }
            }

            return RedirectToAction("Index");
        }
        public void PopulateUsers()
        {
            ViewBag.AllUsers = context.Users.ToList();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Edit(int? id, string[] asndUsers)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var task = context.Tasks.Include(t => t.Users).SingleOrDefault(s => s.Id == id);
            if (task == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }
            task.Users = new List<ApplicationUser>();
            if (asndUsers != null)
            {
                foreach (var userId in asndUsers)
                {
                    var user = context.Users.Single(u => u.Id == userId);
                    task.Users.Add(user);
                }
            }
            if (TryUpdateModel(task, "", new string[] { "Title", "Description", "DueDate", "Status" }))
            {
                context.SaveChanges();
                return RedirectToAction("Index");
            }
            PopulateUsers();
            return View(task);
        }
    }
}