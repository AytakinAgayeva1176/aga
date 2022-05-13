using AGA.Models;
using System;

namespace AGA.ViewModels
{
    public class VacancyVM
    {
        
        public string sector_uniq_id { get; set; }
        public string brand_uniq_id { get; set; }
        public string job_title { get; set; }
        public string location { get; set; }
        public DateTime deadline { get; set; }
        public string schedule_type { get; set; }
        public string schedule { get; set; }
        public string description { get; set; }
        public string salary { get; set; }
        public string job_duties { get; set; }
        public JobRequirement job_requirements { get; set; }
        public string other_information { get; set; }
        public string slug { get; set; }
    }
}
