using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Models;

namespace WebApplication1.Profiles
{
    public class ClientProfile : Profile
    {
        public ClientProfile()
        {
            CreateMap<Client, ClientResource>()
                .ForMember(x => x.Id, opt => opt.MapFrom(y => y.Id))
                .ForMember(x => x.Name, opt => opt.MapFrom(y => y.Name))
                .ForMember(x => x.Surname, opt => opt.MapFrom(y => y.Surname));

            CreateMap<ClientResource, Client>()
                .ForMember(x => x.Id, opt => opt.MapFrom(y => y.Id))
                .ForMember(x => x.Name, opt => opt.MapFrom(y => y.Name))
                .ForMember(x => x.Surname, opt => opt.MapFrom(y => y.Surname))
                .ForAllOtherMembers(opt => opt.Ignore());
        }
    }
}
