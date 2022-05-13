using AGA.Models;
using AGA.ViewModels;
using AutoMapper;

namespace AGA.Automapper
{
    public class MapperProfile:Profile
    {
        public MapperProfile()
        {
            CreateMap<Scholarship, ScholarshipVM>();
            CreateMap<ScholarshipVM, Scholarship>();
            CreateMap<Brand, BrandVM>();
            CreateMap<BrandVM, Brand>();
        }
    }
}
