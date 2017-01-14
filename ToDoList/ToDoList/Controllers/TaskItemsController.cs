using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using ToDoList.Models;

namespace ToDoList.Controllers
{
    public class TaskItemsController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public TaskItemsController() {
            db.Configuration.ProxyCreationEnabled = false;
        }

        // GET: api/TaskItems
        public IQueryable<TaskItem> GetTasks()
        {
            return db.Tasks;
        }

        // GET: api/TaskItems/5
        [ResponseType(typeof(TaskItem))]
        public IHttpActionResult GetTaskItem(int id)
        {
            TaskItem taskItem = db.Tasks.Find(id);
            if (taskItem == null)
            {
                return NotFound();
            }

            return Ok(taskItem);
        }

        // PUT: api/TaskItems/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutTaskItem(int id, TaskItem taskItem)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != taskItem.Id)
            {
                return BadRequest();
            }

            db.Entry(taskItem).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TaskItemExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/TaskItems
        [ResponseType(typeof(TaskItem))]
        public IHttpActionResult PostTaskItem(TaskItem taskItem)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            taskItem.Created = DateTime.Now;

            db.Tasks.Add(taskItem);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = taskItem.Id }, taskItem);
        }

        // DELETE: api/TaskItems/5
        [ResponseType(typeof(TaskItem))]
        public IHttpActionResult DeleteTaskItem(int id)
        {
            TaskItem taskItem = db.Tasks.Find(id);
            if (taskItem == null)
            {
                return NotFound();
            }

            db.Tasks.Remove(taskItem);
            db.SaveChanges();

            return Ok(taskItem);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool TaskItemExists(int id)
        {
            return db.Tasks.Count(e => e.Id == id) > 0;
        }
    }
}