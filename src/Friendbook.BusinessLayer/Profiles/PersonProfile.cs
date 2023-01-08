using AutoMapper;
using Friendbook.Shared.Models;
using Friendbook.Shared.Models.Requests;
using Entities = Friendbook.DataAccessLayer.Entities;

namespace Friendbook.BusinessLayer.Profiles;

public class PersonProfile : Profile
{
    public PersonProfile()
    {
        CreateMap<Entities.Person, Person>();
        CreateMap<SavePersonRequest, Entities.Person>();
    }
}
