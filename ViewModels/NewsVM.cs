using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace AGA.ViewModels
{
    public class NewsVM
    {
        public List<string> images { get; set; }
        public string title { get; set; }
        public string content { get; set; }
        public string hyperlink { get; set; }
        public string slug { get; set; }
        public string icon { get; set; }
    }
}
