using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace AGA.ViewModels
{
    public class StaffVM
    {
        public string job_title { get; set; }
        public string fullname { get; set; }
        public string biography { get; set; }
        public SocialMedia social_media { get; set; }
        public string activity { get; set; }
        public string main_image { get; set; }
        public string slug { get; set; }
        public List<string> images { get; set; }
    }
}
