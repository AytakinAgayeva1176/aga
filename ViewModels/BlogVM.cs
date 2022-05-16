using Microsoft.AspNetCore.Http;

namespace AGA.ViewModels
{
    public class BlogVM
    {
        public string category_id { get; set; }
        public string title { get; set; }
        public string content { get; set; }
        public string image { get; set; }
        public string alt_text { get; set; }
        public string creator_name { get; set; }
        public string creator_photo { get; set; }
        public string creator_smedia { get; set; }
        public string slug { get; set; }
        
    }
}
