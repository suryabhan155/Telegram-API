using BOL;
using DAL.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Service.Abstract
{
    public interface IChannelService
    {
        Task<ResponseModel> CreateChannel(Channel channel);
        Task<ResponseModel> SaveLink(Link link);
    }
}
