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
        Task<ResponseModel> getChannelbyId(long id);
        Task<ResponseModel> UpdateChannel(Channel channel);
        Task<ResponseModel> UpdateLink(Link link);
        Task<ResponseModel> CreateChannel(Channel channel);
        Task<ResponseModel> SaveLink(Link link);
        Task<ResponseModel> DeleteChannel(long Id);
        Task<ResponseModel> DeleteUser(long Id);
        Task<ResponseModel> DeleteLink(string link);
        Task<ResponseModel> CreateSuperGroup(Channel channel);
        Task<ResponseModel> SaveUser(User user);
        Task<ResponseModel> ApproveRejectJoinReject(JoinRequest joinRequest);
    }
}
