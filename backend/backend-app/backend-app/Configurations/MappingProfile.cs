using AutoMapper;
using backend_app.DTOs;
using backend_app.Models;
using backend_app.TypeConverters;

namespace backend_app.Configurations
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<string, UserDTO>().ConvertUsing<JsonToObjectConverter<UserDTO>>();
            CreateMap<string, UserToken>().ConvertUsing<JsonToObjectConverter<UserToken>>();
            CreateMap<User, UserToken>();
            CreateMap<CreateMessageDTO, Message>();
            CreateMap<Document, DocumentResponse>();    
        }
    }
}
