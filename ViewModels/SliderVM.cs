using Microsoft.AspNetCore.Http;

namespace AGA.ViewModels
{
    public class SliderVM
    {

        public string file { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public ButtonVM button { get; set; }
        public int priority { get; set; }
        public string f_index_id { get; set; }
        public string media_path { get; set; }
    }
}
