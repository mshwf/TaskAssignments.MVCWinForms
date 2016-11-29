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
        public ActionResult Delete(int? taskId)
        {
            var taskToDelete = context.Tasks.Where(t => t.Id == taskId).Single();
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
                ViewBag.AllUsers = context.Users.ToList();
                if (taskToUpdate != null)
                {
                    return View(taskToUpdate);
                }
            }

            return RedirectToAction("Index");
        }
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Title,Description,DueDate,Status")] UserTask task, string[] asndUsers)
        {
            if (ModelState.IsValid)
            {
                //Should be done this way to ensure loading the Users entity,
                //otherwise the list will be empty and EF wouldn't be able to read
                //the data from the db, and will add Users NOT update them
                //(remember you can't use Find() with Eager loading)
                task = context.Tasks.Include(t => t.Users).Single(s => s.Id == task.Id);
                task.Users = new List<ApplicationUser>();
                if (asndUsers != null)
                {
                    foreach (var userId in asndUsers)
                    {
                        var user = context.Users.Find(userId);
                        task.Users.Add(user);
                    }
                }
                context.Entry(task).State = EntityState.Modified;
                context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(task);
        }
    }
}