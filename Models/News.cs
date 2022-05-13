using System;

namespace AGA.Models
{
    public class News
    {
        public string id { get; set; }
        public DateTime created_at { get; set; }
        public string uniq_id { get; set; }
        public string images { get; set; }
        public string title { get; set; }
        public string content { get; set; }
        public string hyperlink { get; set; }
        public string icon { get; set; }
        public string slug { get; set; }
        public string status { get; set; }
    }
}
