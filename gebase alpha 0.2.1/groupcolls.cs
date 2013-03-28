using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson;

namespace gebase_alpha_0._2._1
{
    public class groupcolls
    {
        public ObjectId _id { get; set; }
        public string number { get; set; }
        public string teacher { get; set; }
        public string level { get; set; }
        public string days { get; set; }
        public int hours { get; set; }
        public string kind { get; set; }
        public string status { get; set; }
        public string start { get; set; }
        public string time { get; set; }
        public int stdcount { get; set; }
        public string suntime { get; set; }
        public string montime { get; set; }
        public string tuetime { get; set; }
        public string wedtime { get; set; }
        public string thutime { get; set; }
        public string fritime { get; set; }
        public string sattime { get; set; }
    }
}
