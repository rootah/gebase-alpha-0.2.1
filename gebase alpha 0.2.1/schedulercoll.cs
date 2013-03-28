using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson;

namespace gebase_alpha_0._2._1
{
    public class schedulercoll
    {
        public ObjectId _id { get; set; }
        public string Name { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Subject { get; set; }
        public int Status { get; set; }
        public string Description { get; set; }
        public string Label { get; set; }
        public string Location { get; set; }
        public bool AllDay { get; set; }
        public int EventType { get; set; }
        public string RecurrenceInfo { get; set; }
        public string ReminderInfo { get; set; }
    }
}
