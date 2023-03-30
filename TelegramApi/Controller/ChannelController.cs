using Azure.Core;
using BLL.Service.Abstract;
using DAL.Entity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
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
        public ChannelController(IChannelService channelService, TelegramService wT)
        {
            this.channelService = channelService;
            WT = wT;
        }
        [HttpGet("status")]
        public async Task<object> Status()
        {
            try
            {
                switch (WT.ConfigNeeded)
                {
                    case "connecting": return Ok("WTelegram is connecting...");
                    case null: return Ok($@"Connected as {WT.User}");
                    default: return Ok($@"Enter {WT.ConfigNeeded}: ");
                }
            }
            catch(Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet("config")]
        public async Task<ActionResult> Config(string value)
        {
            await WT.DoLogin(value);
            return Redirect("status");
        }

        [HttpGet("chatcount")]
        public async Task<object> Chats()
        {
            if (WT.User == null) throw new Exception("Complete the login first");
            await WT.Client.LoginUserIfNeeded();
            var chats = await WT.Client.Messages_GetAllChats(null);
            return chats.chats.Count;
        }
        [HttpPost("createchannel")]
        public async Task<object> CreateChannel(DAL.Entity.Channel channel)
        {
            if (WT.User == null) throw new Exception("Complete the login first");
            try
            {
                await WT.Client.LoginUserIfNeeded();
                var p =await WT.Client.Channels_CreateChannel(channel.title,channel.about);
                //var res=await channelService.CreateChannel(channel);
                return Ok(p);
            }
            catch (Exception ex)
            {
                throw;
            }
            
        }
        [HttpPost("createpublicchannel")]
        public async Task<object> CreatePublicChannel(DAL.Entity.Channel channel)
        {
            if (WT.User == null) throw new Exception("Complete the login first");
            try
            {
                await WT.Client.LoginUserIfNeeded();
                var p = await WT.Client.Channels_CreateChannel(channel.title, channel.about);
                //var publicchannel = await WT.Client.Channels_UpdateUsername(p,channel.title);
                //var res=await channelService.CreateChannel(channel);
                return Ok(p);
            }
            catch (Exception ex)
            {
                throw;
            }

        }
        [HttpPost("searchuserbyphone")]
        public async Task<object> searchUserbyPhone([FromBody] string phone)
        {
            if (WT.User == null) throw new Exception("Complete the login first");
            try
            {
                await WT.Client.LoginUserIfNeeded();
                UserModel users = new UserModel();
                var contact = await WT.Client.Contacts_ImportContacts(new[] { new InputPhoneContact { phone = phone } });
                foreach (var (id, user) in contact.users)
                {
                    try
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
                    catch (Exception ex)
                    {

                    }
                }
                return Ok(contact.users);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
        [HttpPost("searchuserbyname")]
        public async Task<object> searchUserbyUsername([FromBody] string name)
        {
            if (WT.User == null) throw new Exception("Complete the login first");
            try
            {
                await WT.Client.LoginUserIfNeeded();
                UserModel users = new UserModel();
                //var contact1 = await WT.Client.Contacts_ResolveUsername("@Rama5201");
                var contact = await WT.Client.Contacts_Search(name);
                foreach (var (id, user) in contact.users)
                {
                    try
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
                    catch (Exception ex)
                    {

                    }
                }
                return users;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
        [HttpGet("getchannel")]
        public async Task<object> getChannel()
        {
            if (WT.User == null) throw new Exception("Complete the login first");
            try
            {
                await WT.Client.LoginUserIfNeeded();
                List<ChannelInfo> channellist = new List<ChannelInfo>();
                var chats = await WT.Client.Messages_GetAllChats();
                //var chats1 = await WT.Client.Channels_GetChannels();
                
                foreach (var (id, chat) in chats.chats)
                {
                    try
                    {
                        var channel = (TL.Channel)chats.chats[id];
                        var chats2 = await WT.Client.Channels_GetFullChannel(channel);
                        var channelinfo = new ChannelInfo
                        {
                            Id = id,
                            title = channel.title,
                            photo = channel.photo,
                            isActive = channel.IsActive,
                            type = channel.IsChannel == true ? "Channel" : "Group",
                            noofuser = chats2.full_chat.ParticipantsCount
                        };
                        channellist.Add(channelinfo);
                    }
                    catch(Exception ex)
                    {

                    }
                }
                return channellist;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
        [HttpGet("getchannelfull")]
        public async Task<object> getChannelfull(long id)
        {
            if (WT.User == null) throw new Exception("Complete the login first");
            try
            {
                await WT.Client.LoginUserIfNeeded();
                var chats = await WT.Client.Messages_GetAllChats();
                var channel = (TL.Channel)chats.chats[id]; // the channel we want
                var p = await WT.Client.Channels_GetFullChannel(channel);
                return p;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
        [HttpPost("getchannelinfo")]
        public async Task<object> getChannelInfo([FromBody]long id)
        {
            if (WT.User == null) throw new Exception("Complete the login first");
            try
            {
                await WT.Client.LoginUserIfNeeded();
                var chats = await WT.Client.Messages_GetAllChats();
                var channel = (TL.Channel)chats.chats[id]; // the channel we want
                return channel;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
        [HttpGet("getchannelbyid")]
        public async Task<object> getChannelbyId(long id)
        {
            if (WT.User == null) throw new Exception("Complete the login first");
            try
            {
                await WT.Client.LoginUserIfNeeded();
                var chats = await WT.Client.Messages_GetAllChats();
                var channel = (TL.Channel)chats.chats[id]; // the channel we want
                var p = await WT.Client.Channels_GetChannels(channel);
                return Ok(p);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
        [HttpPost("deletechannel")]
        public async Task<object> deleteChannel([FromBody]long id)
        {
            if (WT.User == null) throw new Exception("Complete the login first");
            try
            {
                await WT.Client.LoginUserIfNeeded();
                var chats = await WT.Client.Messages_GetAllChats();
                var channel = (TL.Channel)chats.chats[id]; // the channel we want
                var p = await WT.Client.Channels_DeleteChannel(channel);
                return Ok(p);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
        [HttpPost("editchanneltitle")]
        public async Task<object> editChannelTitle(DAL.Entity.Channel channel)
        {
            if (WT.User == null) throw new Exception("Complete the login first");
            try
            {
                await WT.Client.LoginUserIfNeeded();
                BaseChannel input = new BaseChannel();
                var p = await WT.Client.Channels_EditTitle(input, channel.title);
                return Ok(p);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

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
            try
            {
                await WT.Client.LoginUserIfNeeded();
                var p =await WT.Client.Channels_CreateChannel(channel.title, channel.about, false, true, false, null, null);
                return Ok(p);
            }
            catch(Exception ex)
            {
                return BadRequest(ex);
            }
        }
        [HttpPost("creategigagroup")]
        public async Task<object> CreateGigaGroup([FromBody] long id)
        {
            if (WT.User == null) throw new Exception("Complete the login first");
            try
            {
                await WT.Client.LoginUserIfNeeded();
                var chats = await WT.Client.Messages_GetAllChats();
                var channel = (TL.Channel)chats.chats[id];
                var p = await WT.Client.Channels_ConvertToGigagroup(channel);
                return Ok(p);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        [HttpPost("AddUserToChannel")]
        public async Task<object> AddUserToChannel(AddUser addRemoveUser)
        {
            if (WT.User == null)
                throw new Exception("Complete the login first");
            try
            {
                await WT.Client.LoginUserIfNeeded();
                var chats = await WT.Client.Messages_GetAllChats();
                var channel = (TL.Channel)chats.chats[addRemoveUser.id]; // the channel we want
                var chat = chats.chats[addRemoveUser.id]; // the target chat
                if(addRemoveUser.rdoPhoneNumber != "")
                {
                    var contact = await WT.Client.Contacts_ImportContacts(new[] { new InputPhoneContact { phone = addRemoveUser.username } });
                    foreach (var (id, user) in contact.users)
                    {
                        // • Directly add the user to a Chat/Channel/group:
                        await WT.Client.AddChatUser(chat, user);
                    }
                }
                else
                {
                    var contact = await WT.Client.Contacts_Search(addRemoveUser.username);
                    foreach (var (id, user) in contact.users)
                    {
                        // • Directly add the user to a Chat/Channel/group:
                        await WT.Client.AddChatUser(chat, user);
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
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
        [HttpPost("LeftUserToChannel")]
        public async Task<object> LeftUserToChannel(RemoveUser RemoveUser)
        {
            if (WT.User == null)
                throw new Exception("Complete the login first");
            try
            {
                await WT.Client.LoginUserIfNeeded();
                var chats = await WT.Client.Messages_GetAllChats();
                //var channel = (TL.Channel)chats.chats[RemoveUser.id]; // the channel we want
                var chat = chats.chats[RemoveUser.id];
                //var add = WT.Client.Channels_CheckUsername(channel, WT.User.username);
                var p = new InputUser(RemoveUser.userid, RemoveUser.access_hash);
                    // • Remove the user from a Chat/Channel/Group:
                    await WT.Client.DeleteChatUser(chat, p);
                
                
                //if (!add.Result)
                //{
                //    await WT.Client.Channels_LeaveChannel(channel);// leave the channel
                    return Ok();
                //}
                    
                //else
                //    return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
        [HttpPost("getLinkAndSend")]
        public async Task<object> GetLinkAndSend([FromBody] long id)
        {
            if (WT.User == null)
                throw new Exception("Complete the login first");
            try
            {
                await WT.Client.LoginUserIfNeeded();
                var chats = await WT.Client.Messages_GetAllChats();
                var chat = chats.chats[id]; // the target chat
                //InputPeer peer = chats.chats[1234567890]; // the chat we want
                // • Obtain the main invite link for the chat, and send it to the user:
                var mcf = await WT.Client.GetFullChat(chat);
                var invite = (ChatInviteExported)mcf.full_chat.ExportedInvite;
                await WT.Client.SendMessageAsync(chat, "Join our group with this link: " + invite.link);
                return Ok(invite);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        [HttpPost("createLink")]
        public async Task<object> createLink(LinkModel link)
        {
            if (WT.User == null)
                throw new Exception("Complete the login first");
            try
            {
                await WT.Client.LoginUserIfNeeded();
                var chats = await WT.Client.Messages_GetAllChats();
                var chat = chats.chats[link.channelId]; // the target chat
                //if(link.usage_limit )
                var invite = (ChatInviteExported)await WT.Client.Messages_ExportChatInvite(chat,false,false,link.expire_Date,link.usage_limit,link.title);
                var link1 = await WT.Client.SendMessageAsync(chat, "Join our group with this link: " + invite.link);
                // • Revoke then delete that invite link (when you no longer need it)
                //await WT.Client.Messages_EditExportedChatInvite(chat, invite.link, revoked: true,usage_limit:1);
                //await WT.Client.Messages_DeleteExportedChatInvite(chat, invite.link);
                return Ok(invite.link);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        [HttpGet("deleteLink")]
        public async Task<object> deleteLink([FromBody] long id)
        {
            if (WT.User == null)
                throw new Exception("Complete the login first");
            try
            {
                await WT.Client.LoginUserIfNeeded();
                var chats = await WT.Client.Messages_GetAllChats();
                var chat = chats.chats[id]; // the target chat
                // • Obtain the main invite link for the chat, and send it to the user:
                var mcf = await WT.Client.GetFullChat(chat);
                var invite = (ChatInviteExported)mcf.full_chat.ExportedInvite;
                await WT.Client.Messages_EditExportedChatInvite(chat, invite.link, revoked: true, usage_limit: 1);
                await WT.Client.Messages_DeleteExportedChatInvite(chat, invite.link);
                return Ok();
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        [HttpPost("JoinPublicGroupWithLink")]
        public async Task<object> JoinPublicGroupWithLink([FromBody]string link)
        {
            if (WT.User == null)
                throw new Exception("Complete the login first");
            try
            {
                await WT.Client.LoginUserIfNeeded();
                string channelName = link.Split('/').Last();
                var resolved = await WT.Client.Contacts_ResolveUsername(channelName); // without the @
                if (resolved.Chat is TL.Channel channel)
                    await WT.Client.Channels_JoinChannel(channel);
                return Ok();
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        [HttpPost("JoinPrivateGroupWithLink")]
        public async Task<object> JoinPrivateGroupWithLink([FromBody]string link)
        {
            if (WT.User == null)
                throw new Exception("Complete the login first");
            try
            {
                await WT.Client.LoginUserIfNeeded();
                string channelName = link.Split('/').Last();
                var chatInvite = await WT.Client.Messages_CheckChatInvite(channelName); // optional: get information before joining  
                await WT.Client.Messages_ImportChatInvite(channelName); // join the channel/group  
                return Ok();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        [HttpPost("getAllParticipants")]
        public async Task<object> GetAllParticipants([FromBody]long id)
        {
            if (WT.User == null)
                throw new Exception("Complete the login first");
            try
            {
                List<UserModel> users = new List<UserModel>();
                await WT.Client.LoginUserIfNeeded();
                //TL.User user = (await WT.Client.Users_GetUsers(InputUser.Self))[0] as TL.User;
                var chats = await WT.Client.Messages_GetAllChats();
                var channel = (TL.Channel)chats.chats[id]; // the channel we want
                var participants = await WT.Client.Channels_GetAllParticipants(channel);
                //var participants = await WT.Client.Channels_GetAllParticipants(channel, includeKickBan:true);
                foreach (var (Id, user) in participants.users)
                {
                    try
                    {
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
                    catch (Exception ex)
                    {

                    }
                }
                return Ok(users);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        [HttpPost("approveRejectChatJoinRequest")]
        public async Task<object> ApproveRejectChatJoinRequest([FromBody]long id)
        {
            if (WT.User == null)
                throw new Exception("Complete the login first");
            try
            {
                await WT.Client.LoginUserIfNeeded();
                var chats = await WT.Client.Messages_GetAllChats();
                InputPeer peer = chats.chats[id];
                var user = await WT.Client.Messages_GetChatInviteImporters(peer,requested:true);
                var participants = await WT.Client.Messages_HideChatJoinRequest(peer,WT.User);
                return Ok(participants);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}
