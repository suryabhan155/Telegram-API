using Azure.Core;
using BLL.Service.Abstract;
using BOL;
using DAL.Entity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Numerics;
using System.Threading.Channels;
using TelegramApi.Modals;
using TL;
using WTelegram;

namespace TelegramApi.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChannelController : ControllerBase
    {
        private readonly IChannelService channelService;
        private readonly TelegramService WT;
        private readonly static ConcurrentDictionary<long, ChatBase> chatlist = new ConcurrentDictionary<long, ChatBase>();
        private readonly static ConcurrentDictionary<long, UserBase> userlist = new ConcurrentDictionary<long, UserBase>();
        private ResponseModel ResponseModel;
        public ChannelController(IChannelService channelService, TelegramService wT,ResponseModel response)
        {
            this.channelService = channelService;
            WT = wT;
            ResponseModel = response;
        }
        [HttpGet("status")]
        public async Task<object> Status()
        {
            switch (WT.ConfigNeeded)
            {
                case "connecting": return Ok("WTelegram is connecting...");
                case null: return Ok($@"Connected as {WT.User}");
                default: return Ok($@"Enter {WT.ConfigNeeded}: ");
            }
        }

        [HttpPost("config")]
        public async Task<ActionResult> Config([FromBody]string value)
        {
            await WT.DoLogin(value);
            return Redirect("status");
        }
        [HttpGet("username")]
        public async Task<ActionResult> username()
        {
            return Ok(WT.User);
        }

        [HttpGet("chatcount")]
        public async Task<object> Chats()
        {
            if (WT.User == null) throw new Exception("Complete the login first");
            await WT.Client.LoginUserIfNeeded();
            //var dialogs = await WT.Client.Messages_GetAllDialogs();
            //var p = new Messages_Chats { chats = dialogs.chats };
            var chats = await WT.Client.Messages_GetAllChats();
            ResponseModel.Message = "";
            ResponseModel.Data = chats.chats.Count;
            ResponseModel.Success = true;
            ResponseModel.StatusCode = 200;
            return ResponseModel;
        }
        [HttpGet("logout")]
        public async Task<object> Logout()
        {
            if (WT.User == null) throw new Exception("Complete the login first");
            await WT.Client.Auth_LogOut();
               
            ResponseModel.Message = "";
            ResponseModel.Data = new { };
            ResponseModel.Success = true;
            ResponseModel.StatusCode = 200;
            return ResponseModel;
        }
        [HttpPost("createchannel")]
        public async Task<object> CreateChannel(DAL.Entity.Channel channel)
        {
            if (WT.User == null) throw new Exception("Complete the login first");
            await WT.Client.LoginUserIfNeeded();
            var p =await WT.Client.Channels_CreateChannel(channel.title,channel.about);
            foreach (var (id, chat) in p.Chats)
            {
                chatlist.TryAdd(id, chat);
                channel.channelId = id;
                ResponseModel = await channelService.CreateChannel(channel);
            }
                
            return Ok(ResponseModel);
        }

        [HttpPost("createpublicchannel")]
        public async Task<object> CreatePublicChannel(DAL.Entity.Channel channel)
        {
            if (WT.User == null) throw new Exception("Complete the login first");
            TL.Channel? channel1 = new TL.Channel();
            await WT.Client.LoginUserIfNeeded();
            var p = await WT.Client.Channels_CreateChannel(channel.title, channel.about);
            foreach(var(id,chat) in p.Chats)
            {
                chatlist.TryAdd(id, chat);
                channel1 = p.Chats[id] as TL.Channel;
                var check = await WT.Client.Channels_CheckUsername(channel1, channel.address);
                var publicchannel = await WT.Client.Channels_UpdateUsername(channel1, channel.address);
                channel.channelId = id;
                ResponseModel = await channelService.CreateChannel(channel);
            }
            return Ok(ResponseModel);
        }
        [HttpPost("searchuserbyphone")]
        public async Task<object> searchUserbyPhone([FromBody] string phone)
        {
            if (WT.User == null) throw new Exception("Complete the login first");
                await WT.Client.LoginUserIfNeeded();
                UserModel users = new UserModel();
                var contact = await WT.Client.Contacts_ImportContacts(new[] { new InputPhoneContact { phone = phone } });
                foreach (var (id, user) in contact.users)
                {
                        var userinfo = new UserModel
                        {
                            Id = id,
                            username = user.username,
                            first_name = user.first_name,
                            last_name = user.last_name,
                            LastSeenAgo = user.LastSeenAgo,
                            phone = user.phone,
                            photo = user.photo,
                            IsActive = user.IsActive,
                            emoji_status = user.emoji_status,
                            restriction_reason = user.restriction_reason,
                            access_hash = user.access_hash,
                            bot_info_version = user.bot_info_version,
                            bot_inline_placeholder = user.bot_inline_placeholder,
                            lang_code = user.lang_code
                        };
                        users = userinfo;
                    
                }
                ResponseModel.Message = "Record searched Successfully!!!";
                ResponseModel.Data = contact.users;
                ResponseModel.Success = true;
                ResponseModel.StatusCode = 200;
                return Ok(ResponseModel);
        }
        [HttpPost("searchuserbyname")]
        public async Task<object> searchUserbyUsername([FromBody] string name)
        {
            if (WT.User == null) throw new Exception("Complete the login first");
                await WT.Client.LoginUserIfNeeded();
                UserModel users = new UserModel();
                //var contact1 = await WT.Client.Contacts_ResolveUsername("@Rama5201");
                var contact = await WT.Client.Contacts_Search(name);
                foreach (var (id, user) in contact.users)
                {
                    
                        var userinfo = new UserModel
                        {
                            Id = id,
                            username = user.username,
                            first_name = user.first_name,
                            last_name = user.last_name,
                            LastSeenAgo = user.LastSeenAgo,
                            phone = user.phone,
                            photo = user.photo,
                            IsActive = user.IsActive,
                            emoji_status = user.emoji_status,
                            restriction_reason = user.restriction_reason,
                            access_hash = user.access_hash,
                            bot_info_version = user.bot_info_version,
                            bot_inline_placeholder = user.bot_inline_placeholder,
                            lang_code = user.lang_code
                        };
                        users=userinfo;
                    
                }
                ResponseModel.Message = "Record searched Successfully!!!";
                ResponseModel.Data = users;
                ResponseModel.Success = true;
                ResponseModel.StatusCode = 200;
                return ResponseModel;
            

        }
        [HttpGet("getchannel")]
        public async Task<object> getChannel()
        {
            if (WT.User == null) throw new Exception("Complete the login first");
           
            await WT.Client.LoginUserIfNeeded();
            List<ChannelInfo> channellist = new List<ChannelInfo>();
            var chats = await WT.Client.Messages_GetAllChats();
            //var chats1 = await WT.Client.Channels_GetChannels();
                
            foreach (var (id, chat) in chats.chats)
            {
                chatlist.TryAdd(id, chat);
                var channel = chats.chats[id] as TL.Channel;
                //var channel1 = chat as TL.Channel;
                if(channel != null && channel.admin_rights != null)
                {
                    var chats2 = await WT.Client.Channels_GetFullChannel(channel);
                    InputPeer peer = chat;
                    Messages_ChatInviteImporters request = await WT.Client.Messages_GetChatInviteImporters(peer, requested: true);

                    if (request != null)
                    {
                        var channelinfo = new ChannelInfo
                        {
                            Id = id,
                            title = channel.title,
                            photo = channel.photo,
                            isActive = channel.IsActive,
                            type = channel.IsChannel == true ? "Channel" : "Group",
                            noofuser = chats2.full_chat.ParticipantsCount,
                            IsRequestJoin = request.count > 0 ? true : false,
                            RequestCount = request.count
                        };
                        channellist.Add(channelinfo);
                    }
                }
            }
            ResponseModel.Message = "Record Successful";
            ResponseModel.Data = channellist;
            ResponseModel.Success = true;
            ResponseModel.StatusCode = 200;
            return ResponseModel;
        }
        [HttpGet("getchannelfull")]
        public async Task<object> getChannelfull(long id)
        {
            if (WT.User == null) throw new Exception("Complete the login first");
            
            await WT.Client.LoginUserIfNeeded();
            //var chats = await WT.Client.Messages_GetAllChats();
            //var channel = (TL.Channel)chats.chats[id]; // the channel we want
            chatlist.TryGetValue(id, out var chat);
            var channel = chat as TL.Channel;
            var p = await WT.Client.Channels_GetFullChannel(channel);
            ResponseModel.Message = "Record Successfully!!!";
            ResponseModel.Data = p;
            ResponseModel.Success = true;
            ResponseModel.StatusCode = 200;
            return ResponseModel;
        }
        [HttpPost("getchannelinfo")]
        public async Task<object> getChannelInfo([FromBody]long id)
        {
            if (WT.User == null) throw new Exception("Complete the login first");
            
            await WT.Client.LoginUserIfNeeded();
            //var chats = await WT.Client.Messages_GetAllChats();
            //var channel = (TL.Channel)chats.chats[id]; // the channel we want
            chatlist.TryGetValue(id, out var chat);
            var channel = chat as TL.Channel;
            ResponseModel.Message = "Record Successfully!!!";
            ResponseModel.Data = channel;
            ResponseModel.Success = true;
            ResponseModel.StatusCode = 200;
            return ResponseModel;
        }
        [HttpPost("getchannelbyid")]
        public async Task<object> getChannelbyId([FromBody]long id)
        {
            if (WT.User == null) throw new Exception("Complete the login first");
           
            await WT.Client.LoginUserIfNeeded();
            chatlist.TryGetValue(id, out var chat);
            var channel = chat as TL.Channel;
            //var p = await WT.Client.Channels_GetChannels(channel);
            ResponseModel = await channelService.getChannelbyId(id);
            return Ok(ResponseModel);
        }
        [HttpPost("deletechannel")]
        public async Task<object> deleteChannel([FromBody]long id)
        {
            if (WT.User == null) throw new Exception("Complete the login first");
            
            await WT.Client.LoginUserIfNeeded();
            chatlist.TryGetValue(id, out var chat);
            //var chats = await WT.Client.Messages_GetAllChats();
            //var channel = (TL.Channel)chats.chats[id]; // the channel we want
            var channel = chat as TL.Channel;
            var p = await WT.Client.Channels_DeleteChannel(channel);
            chatlist.TryRemove(id, out var chat1);
            var res = await channelService.DeleteChannel(id);
            return Ok(res);
        }
        [HttpPost("editchanneltitle")]
        public async Task<object> editChannelTitle(DAL.Entity.Channel channel)
        {
            if (WT.User == null) throw new Exception("Complete the login first");
            
            await WT.Client.LoginUserIfNeeded();
            chatlist.TryGetValue(channel.channelId, out var chat);
            var channel1 = chat as TL.Channel;
            var p = await WT.Client.Channels_EditTitle(channel1, channel.title);
            ResponseModel = await channelService.UpdateChannel(channel);
            return ResponseModel;
        }
        //[HttpPost("editchannelphoto")]
        //public async Task<object> editChannelPhoto(InputChatPhotoBase photo)
        //{
        //    if (WT.User == null) throw new Exception("Complete the login first");
        //    try
        //    {
        //        BaseChannel input = new BaseChannel();
        //        var p = await WT.Client.Channels_EditPhoto(input, photo);
        //        return Ok(p);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }

        //}
        [HttpPost("createsupergroup")]
        public async Task<object> CreateSuperGroup(DAL.Entity.Channel channel)
        {
            if (WT.User == null) throw new Exception("Complete the login first");
            
            await WT.Client.LoginUserIfNeeded();
            var p =await WT.Client.Channels_CreateChannel(channel.title, channel.about,null,null,null, false, true, false,false);
            foreach (var (id, chat) in p.Chats)
            {
                chatlist.TryAdd(id, chat);
                channel.channelId = id;
                ResponseModel = await channelService.CreateSuperGroup(channel);
            }

            return Ok(ResponseModel);
        }
        [HttpPost("creategigagroup")]
        public async Task<object> CreateGigaGroup([FromBody] long id)
        {
            if (WT.User == null) throw new Exception("Complete the login first");
            
            await WT.Client.LoginUserIfNeeded();
            chatlist.TryGetValue(id, out var chat);
            var channel = chat as TL.Channel;
            var p = await WT.Client.Channels_ConvertToGigagroup(channel);
            ResponseModel.Message = "Record Successfully!!!";
            ResponseModel.Data = p;
            ResponseModel.Success = true;
            ResponseModel.StatusCode = 200;
            return ResponseModel;
        }
        [HttpPost("AddUserToChannel")]
        public async Task<object> AddUserToChannel(AddUser addRemoveUser)
        {
            if (WT.User == null)
                throw new Exception("Complete the login first");
            
                await WT.Client.LoginUserIfNeeded();
                var usr = new TL.User();
                //var chats = await WT.Client.Messages_GetAllChats();
                //var channel = (TL.Channel)chats.chats[addRemoveUser.id]; // the channel we want
                //var chat = chats.chats[addRemoveUser.id]; // the target chat
                chatlist.TryGetValue(addRemoveUser.id, out var chat);
                if (addRemoveUser.rdoPhoneNumber != "")
                {
                    var contact = await WT.Client.Contacts_ImportContacts(new[] { new InputPhoneContact { phone = addRemoveUser.username } });
                    foreach (var (id, user) in contact.users)
                    {
                        // • Directly add the user to a Chat/Channel/group:
                        await WT.Client.AddChatUser(chat, user);
                        usr = user;
                    }
                }
                else
                {
                    var contact = await WT.Client.Contacts_Search(addRemoveUser.username);
                    foreach (var (id, user) in contact.users)
                    {
                        // • Directly add the user to a Chat/Channel/group:
                        await WT.Client.AddChatUser(chat, user);
                        usr = user;
                    }
                }
                //var add = WT.Client.Channels_CheckUsername(channel,addRemoveUser.user.username);
                //if (!add.Result)
                //{
                //    await WT.Client.Channels_JoinChannel(channel);
                //    return Ok();
                //}  
                //else
                //    return NoContent();
                var usserlist = new DAL.Entity.User
                {
                    first_name = usr.first_name,
                    last_name = usr.last_name,
                    channelId = addRemoveUser.id,
                    lang_code = usr.lang_code,
                    access_hash = usr.access_hash,
                    bot_info_version = usr.bot_info_version,
                    bot_inline_placeholder = usr.bot_inline_placeholder,
                    //emoji_status = usr.emoji_status,
                    IsActive = usr.IsActive,
                    LastSeenAgo = usr.LastSeenAgo.ToString(),
                    phone = usr.phone,
                    restriction_reason = usr.restriction_reason.ToString(),
                    status = usr.status.ToString(),
                    username = usr.username,
                    userId = usr.ID
                };
                ResponseModel = await channelService.SaveUser(usserlist);
                return ResponseModel;
           
        }
        [HttpPost("LeftUserToChannel")]
        public async Task<object> LeftUserToChannel(RemoveUser RemoveUser)
        {
            if (WT.User == null)
                throw new Exception("Complete the login first");
           
                await WT.Client.LoginUserIfNeeded();
                chatlist.TryGetValue(RemoveUser.id, out var chat);
                //var add = WT.Client.Channels_CheckUsername(channel, WT.User.username);
                var p = new InputUser(RemoveUser.userid, RemoveUser.access_hash);
                    // • Remove the user from a Chat/Channel/Group:
                    await WT.Client.DeleteChatUser(chat, p);


                //if (!add.Result)
                //{
                //    await WT.Client.Channels_LeaveChannel(channel);// leave the channel
                ResponseModel.Message = "Record Successfully!!!";
                ResponseModel.Data = new { };
                ResponseModel.Success = true;
                ResponseModel.StatusCode = 200;
                return ResponseModel;
                //}

                //else
                //    return NoContent();
           
        }
        [HttpPost("getLinkAndSend")]
        public async Task<object> GetLinkAndSend([FromBody] long id)
        {
            if (WT.User == null)
                throw new Exception("Complete the login first");
           
                await WT.Client.LoginUserIfNeeded();
                chatlist.TryGetValue(id, out var chat);
                //InputPeer peer = chats.chats[1234567890]; // the chat we want
                // • Obtain the main invite link for the chat, and send it to the user:
                var mcf = await WT.Client.GetFullChat(chat);
                var invite = mcf.full_chat.ExportedInvite as ChatInviteExported;
                //userlist.TryGetValue(invite.admin_id, out var usr);
                var participants = await WT.Client.Channels_GetAllParticipants(chat as TL.Channel);
                var usr = participants.users[invite.admin_id];
                //var invite1 = await WT.Client.Messages_GetAdminsWithInvites(chat);
                //var invite2 =await WT.Client.Messages_GetChatInviteImporters(chat);
                //var invite3 =await WT.Client.Messages_ExportChatInvite(chat);
                //var invite4 =await WT.Client.Messages_GetExportedChatInvite(chat,invite.link);
                var invite5 = await WT.Client.Messages_GetExportedChatInvites(chat, usr);//revoked:true
                //var checklink = await WT.Client.Messages_CheckChatInvite(mcf.full_chat.ExportedInvite);
                List<LinkModel> linkModels = new List<LinkModel>();
                foreach(ChatInviteExported lk in invite5.invites)
                {
                    var link = new LinkModel
                    {
                        link = lk.link,
                        usage_limit = lk.usage_limit,
                        expire_Date = lk.expire_date,
                        CreatedDate = lk.date,
                        title = lk.title,
                        channelId = id
                    };
                    linkModels.Add(link);
                }
                
                //await WT.Client.SendMessageAsync(chat, "Join our group with this link: " + invite.link);
                ResponseModel.Message = "Record Successfully!!!";
                ResponseModel.Data = linkModels;
                ResponseModel.Success = true;
                ResponseModel.StatusCode = 200;
                return ResponseModel;
           
        }
        [HttpPost("createLink")]
        public async Task<object> createLink(LinkModel link)
        {
            if (WT.User == null)
                throw new Exception("Complete the login first");
            
                await WT.Client.LoginUserIfNeeded();
                chatlist.TryGetValue(link.channelId, out var chat);
                TL.Channel cl = chat as TL.Channel;
                //if(link.usage_limit )
                var invite = await WT.Client.Messages_ExportChatInvite(chat,link.expire_Date,link.usage_limit,link.title, false, true) as ChatInviteExported;
                
                var p = new Link
                {
                    link = invite.link,
                    usage_limit = invite.usage_limit,
                    expire_Date = invite.expire_date,
                    title = link.title,
                    channelId = link.channelId,
                    CreatedDate = link.CreatedDate,
                    request_needed = link.request_needed,
                    legacy_revoke_permanent = link.legacy_revoke_permanent
                };
                //var togglechannel = await WT.Client.Channels_ToggleJoinRequest(cl, true);
                ResponseModel = await channelService.SaveLink(p);
                //var link1 = await WT.Client.SendMessageAsync(chat, "Join our group with this link: " + invite.link);
                // • Revoke then delete that invite link (when you no longer need it)
                //await WT.Client.Messages_EditExportedChatInvite(chat, invite.link, revoked: true,usage_limit:1);
                //await WT.Client.Messages_DeleteExportedChatInvite(chat, invite.link);
                
                return ResponseModel;
            
        }
        [HttpPost("deleteLink")]
        public async Task<object> deleteLink([FromBody] long id)
        {
            if (WT.User == null)
                throw new Exception("Complete the login first");
            
                await WT.Client.LoginUserIfNeeded();
                chatlist.TryGetValue(id, out var chat);
                // • Obtain the main invite link for the chat, and send it to the user:
                var mcf = await WT.Client.GetFullChat(chat);
                var invite = mcf.full_chat.ExportedInvite as ChatInviteExported;
                //await WT.Client.Messages_EditExportedChatInvite(chat, invite.link, revoked: true, usage_limit: 1);
                await WT.Client.Messages_DeleteExportedChatInvite(chat, invite.link);
                ResponseModel = await channelService.DeleteLink(invite.link);
                return ResponseModel;
            
        }
        [HttpPost("editLink")]
        public async Task<object> editLink(LinkModel link)
        {
            if (WT.User == null)
                throw new Exception("Complete the login first");
           
                await WT.Client.LoginUserIfNeeded();
                chatlist.TryGetValue(link.channelId, out var chat);
                await WT.Client.Messages_EditExportedChatInvite(chat, link.link, expire_date:link.expire_Date, usage_limit: link.usage_limit,link.request_needed,revoked: false);
                var p = new Link
                {
                    link = link.link,
                    usage_limit = link.usage_limit,
                    expire_Date = link.expire_Date,
                    title = link.title,
                    channelId = link.channelId,
                    CreatedDate = link.CreatedDate,
                    request_needed = link.request_needed,
                    legacy_revoke_permanent = link.legacy_revoke_permanent
                };
                ResponseModel = await channelService.UpdateLink(p);
                return ResponseModel;
            
        }
        [HttpPost("JoinPublicGroupWithLink")]
        public async Task<object> JoinPublicGroupWithLink([FromBody]string link)
        {
            if (WT.User == null)
                throw new Exception("Complete the login first");
            
                await WT.Client.LoginUserIfNeeded();
                string channelName = link.Split('/').Last();
                var resolved = await WT.Client.Contacts_ResolveUsername(channelName); // without the @
                if (resolved.Chat is TL.Channel channel)
                    await WT.Client.Channels_JoinChannel(channel);
                ResponseModel.Message = "Record Successfully!!!";
                ResponseModel.Data = new {};
                ResponseModel.Success = true;
                ResponseModel.StatusCode = 200;
                return ResponseModel;
            
        }
        [HttpPost("JoinPrivateGroupWithLink")]
        public async Task<object> JoinPrivateGroupWithLink([FromBody] string link)
        {
            if (WT.User == null)
                throw new Exception("Complete the login first");
           
            await WT.Client.LoginUserIfNeeded();
            string channelName = link.Split('/').Last();
            var chatInvite = await WT.Client.Messages_CheckChatInvite(channelName); // optional: get information before joining  
            await WT.Client.Messages_ImportChatInvite(channelName); // join the channel/group  
            ResponseModel.Message = "Record Successfully!!!";
            ResponseModel.Data = new {};
            ResponseModel.Success = true;
            ResponseModel.StatusCode = 200;
            return ResponseModel;
           
        }
        [HttpPost("getAllParticipants")]
        public async Task<object> GetAllParticipants([FromBody]long id)
        {
            if (WT.User == null)
                throw new Exception("Complete the login first");
            
            foreach (var usr in userlist)
            {
                // removed by another thread.
                userlist.TryRemove(usr.Key, out var ignored);
            }
            List<UserModel> users = new List<UserModel>();
            await WT.Client.LoginUserIfNeeded();
            //TL.User user = (await WT.Client.Users_GetUsers(InputUser.Self))[0] as TL.User;
            chatlist.TryGetValue(id, out var chat);
            var channel = chat as TL.Channel;
            var participants = await WT.Client.Channels_GetAllParticipants(channel);
            //var participants = await WT.Client.Channels_GetAllParticipants(channel, includeKickBan:true);
            foreach (var (Id, user) in participants.users)
            {
                    
                userlist.TryAdd(Id, user);
                var userinfo = new UserModel
                {
                    Id = Id,
                    username = user.username,
                    first_name = user.first_name,
                    last_name = user.last_name,
                    LastSeenAgo = user.LastSeenAgo,
                    phone = user.phone,
                    photo = user.photo,
                    IsActive = user.IsActive,
                    emoji_status = user.emoji_status,
                    restriction_reason = user.restriction_reason,
                    access_hash = user.access_hash,
                    bot_info_version = user.bot_info_version,
                    bot_inline_placeholder = user.bot_inline_placeholder,
                    lang_code = user.lang_code
                };
                var usserlist = new DAL.Entity.User
                {
                    first_name = user.first_name,
                    last_name = user.last_name,
                    channelId = id,
                    lang_code = user.lang_code,
                    access_hash = user.access_hash,
                    bot_info_version =  user.bot_info_version,
                    bot_inline_placeholder = user.bot_inline_placeholder,
                    //emoji_status = usr1.emoji_status,
                    IsActive = user.IsActive,
                    LastSeenAgo = user.LastSeenAgo.ToString(),
                    phone = user.phone,
                    restriction_reason = null,
                    status = user.status.ToString(),
                    username = user.username,
                    userId = user.ID
                };
                var result = await channelService.SaveUser(usserlist);
                users.Add(userinfo);
            }
            ResponseModel.Message = "Record Successfully!!!";
            ResponseModel.Data =users;
            ResponseModel.Success = true;
            ResponseModel.StatusCode = 200;
            return ResponseModel;
        }

        [HttpPost("getwaitinglistuser")]
        public async Task<object> GetWaitingListUser([FromBody] long id)
        {
            if (WT.User == null)
                throw new Exception("Complete the login first");

            List<UserModel> users = new List<UserModel>();
            await WT.Client.LoginUserIfNeeded();
            chatlist.TryGetValue(id, out var chat);
            InputPeer peer = chat;
            var request = await WT.Client.Messages_GetChatInviteImporters(peer, requested: true);
            //for(int i = 0;i < request.count; i++)
            //{
            //    requestModel.user_id = request.importers[i].user_id;
            //    requestModel.first_name = request.users[requestModel.user_id].first_name;
            //    requestModel.last_name = request.users[requestModel.user_id].last_name;
            //    requestModel.message = "requested to join " + request.importers[i].date;
            //    requestlist.Add(requestModel);
            //}
            foreach(var(Id,user) in request.users)
            {
                userlist.TryAdd(Id, user);
                var userinfo = new UserModel
                {
                    Id = Id,
                    username = user.username,
                    first_name = user.first_name,
                    last_name = user.last_name,
                    LastSeenAgo = user.LastSeenAgo,
                    phone = user.phone,
                    photo = user.photo,
                    IsActive = user.IsActive,
                    emoji_status = user.emoji_status,
                    restriction_reason = user.restriction_reason,
                    access_hash = user.access_hash,
                    bot_info_version = user.bot_info_version,
                    bot_inline_placeholder = user.bot_inline_placeholder,
                    lang_code = user.lang_code
                };
                users.Add(userinfo);
            }
                
            ResponseModel.Message = "Record Successfully!!!";
            ResponseModel.Data = users;
            ResponseModel.Success = true;
            ResponseModel.StatusCode = 200;
            return ResponseModel;
           
        }
        [HttpPost("approveRejectChatJoinRequest")]
        public async Task<object> ApproveRejectChatJoinRequest(ApproveRejectModel approveRejectModel)
        {
            if (WT.User == null)
                throw new Exception("Complete the login first");
           
                await WT.Client.LoginUserIfNeeded();
                chatlist.TryGetValue(approveRejectModel.channelId, out var chat);
                InputPeer peer = chat;
                var mcf = await WT.Client.GetFullChat(chat);
                var invite = mcf.full_chat.ExportedInvite as ChatInviteExported;
                var participants = await WT.Client.Channels_GetAllParticipants(chat as TL.Channel);
                var usr = participants.users[invite.admin_id];
                var invite5 = await WT.Client.Messages_GetExportedChatInvites(chat,usr);//revoked:true
                foreach (ChatInviteExported lk in invite5.invites)
                {
                    var user1 = await WT.Client.Messages_GetChatInviteImporters(peer, requested: true, link: lk.link);
                    if(user1.count > 0)
                    {
                        foreach (var (Id, usr1) in user1.users)
                        {
                            userlist.TryAdd(Id, usr1);
                            var usserlist = new DAL.Entity.User
                            {
                                first_name = usr1.first_name,
                                last_name = usr1.last_name,
                                channelId = approveRejectModel.channelId,
                                lang_code = usr1.lang_code,
                                access_hash = usr1.access_hash,
                                bot_info_version = usr1.bot_info_version,
                                bot_inline_placeholder = usr1.bot_inline_placeholder,
                                //emoji_status = usr1.emoji_status,
                                IsActive = usr1.IsActive,
                                LastSeenAgo = usr1.LastSeenAgo.ToString(),
                                phone = usr1.phone,
                                restriction_reason = null,
                                status = usr1.status.ToString(),
                                username = usr1.username,
                                userId = usr1.ID
                            };
                            var result = await channelService.SaveUser(usserlist);
                            if (result.Success)
                            {
                                userlist.TryGetValue(approveRejectModel.userId, out var user);
                                if (approveRejectModel.approved)
                                {
                                    await WT.Client.Messages_HideChatJoinRequest(peer, user, approveRejectModel.approved);
                                }
                                else//reject
                                {
                                    await WT.Client.Messages_HideChatJoinRequest(peer, user, approveRejectModel.approved);
                                }
                                var p = new JoinRequest
                                {
                                    userId = approveRejectModel.userId,
                                    channelId = approveRejectModel.channelId,
                                    Approved = approveRejectModel.approved,
                                    link = lk.link,
                                };
                                ResponseModel = await channelService.ApproveRejectJoinReject(p);
                            }
                        }
                    }
                }
                return ResponseModel;
           
        }
    }
}
