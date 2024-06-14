using AutoMapper;
using Minimal_Api1.Models;
using Minimal_Api1.ViewModel;

namespace Minimal_Api1.Helpers
{
    public class AutoMapper : Profile 
    {
        public AutoMapper()
        {
            CreateMap<Champions, ChampionViewModel>().ReverseMap();
        }
    }
}
