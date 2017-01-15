using Microsoft.AspNet.Identity;
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
    [Authorize]
    public class ListsController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private string _userId;

        public ListsController()
        {
            db.Configuration.ProxyCreationEnabled = false;
            _userId = User.Identity.GetUserId();
        }

        // GET: api/Lists
        public IQueryable<List> GetLists()
        {
            return db.Lists.Where(l => l.ApplicationUserId == _userId);
        }

        // GET: api/Lists/5
        [ResponseType(typeof(List))]
        public IHttpActionResult GetList(int id)
        {
            List list = db.Lists.FirstOrDefault(l => l.ApplicationUserId == _userId && l.Id == id);
            if (list == null)
            {
                return NotFound();
            }

            return Ok(list);
        }

        // PUT: api/Lists/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutList(int id, List list)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != list.Id)
            {
                return BadRequest();
            }

            if(!db.Lists.Any(l => l.ApplicationUserId == _userId && l.Id == id))
            {
                return NotFound();
            }

            db.Entry(list).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ListExists(id))
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

        // POST: api/Lists
        [ResponseType(typeof(List))]
        public IHttpActionResult PostList(List list)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            list.ApplicationUserId = _userId;
            db.Lists.Add(list);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = list.Id }, list);
        }

        // DELETE: api/Lists/5
        [ResponseType(typeof(List))]
        public IHttpActionResult DeleteList(int id)
        {
            List list = db.Lists.FirstOrDefault(l => l.Id == id && l.ApplicationUserId == _userId);
            if (list == null)
            {
                return NotFound();
            }

            db.Lists.Remove(list);
            db.SaveChanges();

            return Ok(list);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ListExists(int id)
        {
            return db.Lists.Count(e => e.Id == id) > 0;
        }
    }
}