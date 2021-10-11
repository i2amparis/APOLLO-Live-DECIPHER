using AutoMapper;
using Topsis.Application.Contracts.Identity;
using Topsis.Application.Features;
using Topsis.Domain;

namespace Topsis.Application.Contracts
{
    public class Mapping : Profile
    {
        public Mapping()
        {
            this.CreateMap<Moderator, CreateModerator.Command>().ReverseMap();
            this.CreateMap<ApplicationUser, CreateStakeholder.Command>().ReverseMap();
        }
    }
}
