namespace AGA.Models
{
    public class ResidentialApplication
    {
        public string id { get; set; }
        public string uniq_id { get; set; }
        public string sector_uniq_id { get; set; }
        public string fullname { get; set; }
        public string company_name { get; set; }
        public decimal? area_size { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
        public string location { get; set; }
        public string status { get; set; }
    }
}
