using BLL.Service.Abstract;
using BOL;
using DAL.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Service.Implemention
{
    public class ChannelService : IChannelService
    {
        private readonly GeneralContext db;
        private readonly ResponseModel response;
        int errorflag = 0;
        public ChannelService(GeneralContext db,ResponseModel response)
        {
            this.response=response;
            this.db = db;

        }
        public async Task<ResponseModel> getChannelbyId(long id)
        {
            try
            {
                var channel = db.Channels.FirstOrDefault(x=>x.channelId == id);
                if (channel == null)
                {
                    response.Data = channel;
                    response.Success = false;
                    response.Message = "Record Not Found.";
                    response.StatusCode = 404;
                }
                else
                {
                    response.Data = channel;
                    response.Success = true;
                    response.Message = "Record Found.";
                    response.StatusCode = 200;
                }
            }
            catch (Exception e)
            {
                response.Message = e.Message;
                response.StatusCode = 400;
                response.Success = false;
                response.Data = new { };
            }
            return response;
        }
        public async Task<ResponseModel> UpdateChannel(Channel channel)
        {
            try
            {
                var Exist = await db.Channels.FirstOrDefaultAsync(x => x.channelId == channel.channelId);
                if (Exist != null)
                {
                    Exist.title = channel.title;
                    Exist.about = channel.about;
                }
                db.Entry(Exist).State = EntityState.Modified;
                Save();
                response.Success = true;
                response.Message = "Channel updated Successfully with Id = " + channel.channelId;
                response.Data = channel;
                response.StatusCode = 200;
            }
            catch(Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
                response.Data = new {};
                response.StatusCode = 500;
            }
            return response;
        }
        public async Task<ResponseModel> UpdateLink(Link link)
        {
            try
            {
                db.Entry(link).State = EntityState.Modified;
                Save();
                response.Success = true;
                response.Message = "link updated Successfully with Id = " + link.linkId;
                response.Data = link;
                response.StatusCode = 200;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
                response.Data = new { };
                response.StatusCode = 500;
            }
            return response;
        }
        public async Task<ResponseModel> CreateChannel(Channel channel)
        {
            try
            {
                if (channel.title != "" && channel.about != "")
                {
                    bool isNew = false;
                    var UserNameExist = await db.Channels.FirstOrDefaultAsync(x => x.title == channel.title && x.about == channel.about);
                    if (UserNameExist != null)
                    {
                        response.Success = true;
                        response.Message = "channel already exists.";
                        response.Data = new { channel.title, channel.about };
                        response.StatusCode = 1;

                    }
                    else
                    {
                        isNew = true;
                        Channel channel1 = new Channel();

                        channel1.title = channel.title;
                        channel1.about = channel.about.Trim();
                        channel1.channelId = channel.channelId;
                        channel1.username = channel.username;
                        channel1.broadcast = true;
                        channel1.megagroup = false;
                        db.Channels.Add(channel1);
                        if (isNew)
                            Save();
                        if (errorflag == 0)
                        {
                            channel.channelId = channel1.channelId;
                            response.Success = true;
                            response.Message = "Channel added Successfully with Id = " + channel1.channelId;
                            response.Data = channel1;
                            response.StatusCode = 200;
                        }
                    }
                }
                else
                {
                    response.Success = true;
                    response.Message = "Some fields are missings!!!, Please check.";
                    response.Data = channel;
                    response.StatusCode = 3;
                }
            }
            catch (Exception exception)
            {
                response.Message = exception.Message;
                response.StatusCode = 400;
                response.Success = false;
                response.Data = new { };
            }
            return response;
        }
        public async Task<ResponseModel> DeleteChannel(long id)
        {
            try
            {
                var channel = db.Channels.FirstOrDefault(x=>x.channelId == id);
                db.Channels.Remove(channel);
                Save();
                response.Success = true;
                response.Message = "Channel deleted Successfully with Id = " + channel.channelId;
                response.Data = channel;
                response.StatusCode = 200;
            }
            catch (Exception exception)
            {
                response.Message = exception.Message;
                response.StatusCode = 400;
                response.Success = false;
                response.Data = new { };
            }
            return response;
        }
        public async Task<ResponseModel> CreateSuperGroup(Channel channel)
        {
            try
            {
                if (channel.title != "" && channel.about != "")
                {
                    bool isNew = false;
                    var UserNameExist = await db.Channels.FirstOrDefaultAsync(x => x.title == channel.title && x.about == channel.about);
                    if (UserNameExist != null)
                    {
                        response.Success = true;
                        response.Message = "channel already exists.";
                        response.Data = new { channel.title, channel.about };
                        response.StatusCode = 1;

                    }
                    else
                    {
                        isNew = true;
                        Channel channel1 = new Channel();

                        channel1.title = channel.title;
                        channel1.about = channel.about.Trim();
                        channel1.channelId = channel.channelId;
                        channel1.broadcast = false;
                        channel1.megagroup = true;
                        db.Channels.Add(channel1);
                        if (isNew)
                            Save();
                        if (errorflag == 0)
                        {
                            channel.channelId = channel1.channelId;
                            response.Success = true;
                            response.Message = "Channel added Successfully with Id = " + channel1.channelId;
                            response.Data = channel1;
                            response.StatusCode = 200;
                        }
                    }
                }
                else
                {
                    response.Success = true;
                    response.Message = "Some fields are missings!!!, Please check.";
                    response.Data = channel;
                    response.StatusCode = 3;
                }
            }
            catch (Exception exception)
            {
                response.Message = exception.Message;
                response.StatusCode = 400;
                response.Success = false;
                response.Data = new { };
            }
            return response;
        }
        public async Task<ResponseModel> SaveLink(Link link)
        {
            try
            {
                if (link.link != "")
                {
                    bool isNew = false;
                    var linkExist = await db.Links.FirstOrDefaultAsync(x => x.link == link.link);
                    if (linkExist != null)
                    {
                        response.Success = true;
                        response.Message = "Link already exists.";
                        response.Data = link;
                        response.StatusCode = 1;

                    }
                    else
                    {
                        isNew = true;
                        Link link1 = new Link();

                        link1.link = link.link;
                        link1.title = link.title;
                        link1.channelId = link.channelId;
                        link1.CreatedDate = DateTime.Now;
                        link1.expire_Date = link.expire_Date;
                        link.usage_limit = link.usage_limit;
                        link.legacy_revoke_permanent = false;
                        link.request_needed = false;
                        link.revoked = false;
                        link.deleted = false;
                        db.Links.Add(link1);
                        if (isNew)
                            Save();
                        if (errorflag == 0)
                        {
                            link.Id = link1.Id;
                            response.Success = true;
                            response.Message = "link added Successfully with Id = " + link1.linkId;
                            response.Data = link1;
                            response.StatusCode = 200;
                        }
                    }
                }
                else
                {
                    response.Success = true;
                    response.Message = "Some fields are missings!!!, Please check.";
                    response.Data = link;
                    response.StatusCode = 3;
                }
            }
            catch (Exception exception)
            {
                response.Message = exception.Message;
                response.StatusCode = 400;
                response.Success = false;
                response.Data = new { };
            }
            return response;
        }

        public async Task<ResponseModel> SaveUser(User user)
        {
            try
            {
                bool isNew = false;
                var userExist = await db.Users.FirstOrDefaultAsync(x => x.userId == user.userId && x.channelId == user.channelId);
                if (userExist != null)
                {
                    response.Success = true;
                    response.Message = "User already exists.";
                    response.Data = user;
                    response.StatusCode = 1;

                }
                else
                {
                    isNew = true;
                    User user1 = new User();
                    user1.access_hash = user.access_hash;
                    user1.userId = user.userId;
                    user1.channelId = user.channelId;
                    user1.first_name = user.first_name;
                    user1.last_name = user.last_name;
                    user1.username = user.username;
                    user1.phone = user.phone;
                    user1.status = user.status;
                    user1.bot_info_version = user.bot_info_version;
                    user1.restriction_reason = user.restriction_reason;
                    user1.bot_inline_placeholder = user.bot_inline_placeholder;
                    user1.lang_code = user.lang_code;
                    user1.emoji_status = user.emoji_status;
                    user1.IsActive = true;
                    user1.LastSeenAgo = user.LastSeenAgo;
                    db.Users.Add(user1);
                    if (isNew)
                        Save();
                    if (errorflag == 0)
                    {
                        user.Id = user1.Id;
                        response.Success = true;
                        response.Message = "user added Successfully with Id = " + user1.userId;
                        response.Data = user1;
                        response.StatusCode = 200;
                    }
                }
            }
            catch (Exception exception)
            {
                response.Message = exception.Message;
                response.StatusCode = 400;
                response.Success = false;
                response.Data = new { };
            }
            return response;
        }
        public async Task<ResponseModel> ApproveRejectJoinReject(JoinRequest joinRequest)
        {
            try
            {
                bool isNew = false;
                var requestExist = await db.JoinRequests.FirstOrDefaultAsync(x => x.userId == joinRequest.userId && x.channelId == joinRequest.channelId);
                if (requestExist != null)
                {
                    response.Success = true;
                    response.Message = "Join Request already exists.";
                    response.Data = joinRequest;
                    response.StatusCode = 1;
                }
                else
                {
                    isNew = true;
                    JoinRequest request = new JoinRequest();
                    request.userId = joinRequest.userId;
                    request.channelId = joinRequest.channelId;
                    request.Approved = joinRequest.Approved;
                    request.link = joinRequest.link;
                    db.JoinRequests.Add(request);
                    if (isNew)
                        Save();
                    if (errorflag == 0)
                    {
                        joinRequest.Id = request.Id;
                        response.Success = true;
                        response.Message = "JoinRequest added Successfully with Id = " + request.userId;
                        response.Data = joinRequest;
                        response.StatusCode = 200;
                    }
                }
            }
            catch (Exception exception)
            {
                response.Message = exception.Message;
                response.StatusCode = 400;
                response.Success = false;
                response.Data = new { };
            }
            return response;
        }
        public async Task<ResponseModel> DeleteUser(long id)
        {
            try
            {
                var user = db.Users.FirstOrDefault(x=>x.userId == id);
                user.IsActive = false;
                db.Entry(user).State = EntityState.Modified;
                Save();
                response.Success = true;
                response.Message = "User updated Successfully with Id = " + user.userId;
                response.Data = user;
                response.StatusCode = 200;
            }
            catch (Exception exception)
            {
                response.Message = exception.Message;
                response.StatusCode = 400;
                response.Success = false;
                response.Data = new { };
            }
            return response;
        }
        public async Task<ResponseModel> DeleteLink(string link)
        {
            try
            {
                var link1 = db.Links.FirstOrDefault(x => x.link == link);
                link1.deleted = true;
                db.Entry(link).State = EntityState.Modified;
                Save();
                response.Success = true;
                response.Message = "link deleted Successfully with link = " + link;
                response.Data = link;
                response.StatusCode = 200;
            }
            catch (Exception exception)
            {
                response.Message = exception.Message;
                response.StatusCode = 400;
                response.Success = false;
                response.Data = new { };
            }
            return response;
        }
        public void Save()
        {
            try
            {
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
                errorflag = 1;
            }
        }
    }
}
