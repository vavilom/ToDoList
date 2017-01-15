using Microsoft.AspNet.Identity;
using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.Http.Description;
using ToDoList.Models;

namespace ToDoList.Controllers
{
    [Authorize]
    public class TaskItemsController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private string _userId;

        public TaskItemsController() {
            db.Configuration.ProxyCreationEnabled = false;
            _userId = User.Identity.GetUserId();
        }

        // GET: api/TaskItems
        public IQueryable<TaskItem> GetTasks()
        {
            return db.Tasks.Where(t => t.List.ApplicationUserId == _userId);
        }

        // GET: api/TaskItems/5
        [ResponseType(typeof(TaskItem))]
        public IHttpActionResult GetTaskItem(int id)
        {
            TaskItem taskItem = db.Tasks.FirstOrDefault(t => t.List.ApplicationUserId == _userId && t.Id == id);
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

            if (!db.Tasks.Any(t => t.List.ApplicationUserId == _userId && t.Id == id))
            {
                return NotFound();
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

        //PUT: api/ExecuteItem/5
        [HttpPut]
        public IHttpActionResult ExecuteItem(int id, [FromBody]bool change)
        {
            if (!db.Tasks.Any(t => t.Id == id))
            {
                return BadRequest();
            }

            TaskItem task = db.Tasks.FirstOrDefault(t => t.Id == id && t.List.ApplicationUserId == _userId);

            if(task != null)
            {
                if (change)
                {
                    task.Finished = DateTime.Now;
                }
                else
                {
                    task.Finished = DateTime.MinValue;
                }
            }
            else
            {
                return NotFound();
            }

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
            if (!ModelState.IsValid || taskItem.ListId <= 0)
            {
                return BadRequest(ModelState);
            }

            taskItem.Created = DateTime.Now;

            if (!db.Lists.Any(l => l.ApplicationUserId == _userId && l.Id == taskItem.ListId))
            {
                return NotFound();
            }

            db.Tasks.Add(taskItem);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = taskItem.Id }, taskItem);
        }

        // DELETE: api/TaskItems/5
        [ResponseType(typeof(TaskItem))]
        public IHttpActionResult DeleteTaskItem(int id)
        {
            TaskItem taskItem = db.Tasks.FirstOrDefault(t => t.Id == id && t.List.ApplicationUserId == _userId);
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