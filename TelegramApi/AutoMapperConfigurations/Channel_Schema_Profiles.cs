using AutoMapper.Internal;
using AutoMapper;
using Microsoft.Win32;
using DAL.Entity;
using TelegramApi.Modals;

namespace TelegramApi.AutoMapperConfigurations
{
    public class Channel_Schema_Profiles : Profile
    {
        public Channel_Schema_Profiles()
        {
            CreateMap<Channel,ChannelViewModel>().ReverseMap();
            CreateMap<Link, LinkModel>().ReverseMap();
        }
    }
}
