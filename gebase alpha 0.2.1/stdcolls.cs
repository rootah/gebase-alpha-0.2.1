using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson;

namespace gebase_alpha_0._2._1
{
    public class stdcolls
    {
        public ObjectId _id { get; set; }
        public string fname { get; set; }
        public string lname { get; set; }
        public string address { get; set; }
        public DateTime bdate { get; set; }
        public int age { get; set; }
        public string program { get; set; }
        public string source { get; set; }
        public string daysposs { get; set; }
        public string timeposs { get; set; }
        public int cost { get; set; }
        public DateTime accepted { get; set; }
        public Boolean isgroup { get; set; }
        public Boolean isindividual { get; set; }
        public Boolean isintensive { get; set; }
        public string level { get; set; }
        public string status { get; set; }

        public string group { get; set; }
        public string fullname { get; set; }
        public string email { get; set; }
        public string mphone { get; set; }
        public string hphone { get; set; }
        public string addphone { get; set; }

        public int topay { get; set; }
        public int payed { get; set; }
        public DateTime periodstart { get; set; }
        public DateTime periodend { get; set; }
    }
}
