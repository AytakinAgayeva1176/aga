using System;

namespace AGA.Models
{
    public class ContactUs
    {
        public string id { get; set; }
        public DateTime created_at { get; set; }
        public string uniq_id { get; set; }
        public string fullname { get; set; }
        public string phone { get; set; }
        public string email { get; set; }
        public string message { get; set; }
        public string status { get; set; }
    }
}
