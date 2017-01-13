using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ToDoList.Models
{
    public enum Repeats { OneTime, EveryDay, EveryWeek, EveryMonth, EveryYear };

    public class List
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ApplicationUserId { get; set; }
        [Column(TypeName = "datetime2")]
        public DateTime Created { get; set; }
        [Column(TypeName = "datetime2")]
        public DateTime Finished { get; set; }

        public virtual ApplicationUser ApplicationUser { get; set; }
        public virtual ICollection<TaskItem> Tasks { get; set; }
    }

    public class TaskItem
    {
        public int Id { get; set; }
        public int ListId { get; set; }
        public string Name { get; set; }
        [Column(TypeName = "datetime2")]
        public DateTime Created { get; set; }
        [Column(TypeName = "datetime2")]
        public DateTime Finished { get; set; }
        public Repeats Repeat { get; set; }

        public virtual List List { get; set; }
    }
}