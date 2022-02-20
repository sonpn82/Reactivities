using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain
{
    // class contains our data model
    // use by entity framework to generate the datatable in Persistence
    public class Activity
    {
        // properties initialize
        public Guid Id { get; set; }  // global unique identifier - be primary key in Activities table in DataContext.cs
        public string? Title { get; set; }
        public DateTime Date { get; set; }
        public string? Description { get; set; }
        public string? Category { get; set; }
        public string? City { get; set; }
        public string? Venue { get; set; }
    }
}