using System;

namespace AGA.ViewModels
{
    public class ScholarshipVM
    {
        public DateTime birthdate { get; set; }
        public string fullname { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
        public string address { get; set; }
        public string image { get; set; }
        public bool is_az_citizen { get; set; }
        public string id_card { get; set; }
        public string s_media { get; set; }
        public string university { get; set; }
        public int? uni_entrance_point { get; set; }
        public string profession { get; set; }
        public string grade { get; set; }
        public decimal? gpa { get; set; }
        public string transcript { get; set; }
        public string student_card { get; set; }
        public string ref_letter { get; set; }
        public string motiv_letter { get; set; }
        public string acceptance_letter { get; set; }
        public string confirming_prev_edu { get; set; }
        public string student_visa { get; set; }
        public string lang_certificate { get; set; }
        public string hobbies { get; set; }
        public string type { get; set; }
    }
}
