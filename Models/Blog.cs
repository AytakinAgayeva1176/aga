using System;

namespace AGA.Models
{
    public class Blog
    {
        public string id { get; set; }
        public string uniq_id { get; set; }
        public string category_id { get; set; }
        public DateTime created_at { get; set; }
        public string title { get; set; }
        public string content { get; set; }
        public string image { get; set; }
        public string alt_text { get; set; }
        public string creator_name { get; set; }
        public string creator_photo { get; set; }
        public string creator_smedia { get; set; }
        public string slug { get; set; }
        public string status { get; set; }
    }
}
