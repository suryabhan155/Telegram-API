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
                        response.success = true;
                        response.message = "channel already exists.";
                        response.data = new { channel.title, channel.about };
                        response.Status_Code = 1;

                    }
                    else
                    {
                        isNew = true;
                        Channel channel1 = new Channel();

                        channel1.title = channel.title;
                        channel1.about = channel.about.Trim();

                        channel1.broadcast = true;
                        channel1.megagroup = false;
                        db.Channels.Add(channel1);
                        if (isNew)
                            Save();
                        if (errorflag == 0)
                        {
                            channel.channelId = channel1.channelId;
                            response.success = true;
                            response.message = "Channel added successfully with Id = " + channel1.channelId;
                            response.data = channel1;
                            response.Status_Code = 200;
                        }
                    }
                }
                else
                {
                    response.success = true;
                    response.message = "Some fields are missings!!!, Please check.";
                    response.data = channel;
                    response.Status_Code = 3;
                }
            }
            catch (Exception exception)
            {
                response.message = exception.Message;
                response.Status_Code = 400;
                response.success = false;
                response.data = new { };
            }
            return response;
        }
        public async Task<ResponseModel> CreateSuperGroup(Channel channel)
        {
            try
            {
                if (channel.title != "" && channel.about != "" && channel.megagroup==true)
                {
                    bool isNew = false;
                    var UserNameExist = await db.Channels.FirstOrDefaultAsync(x => x.title == channel.title && x.about == channel.about);
                    if (UserNameExist != null)
                    {
                        response.success = true;
                        response.message = "channel already exists.";
                        response.data = new { channel.title, channel.about };
                        response.Status_Code = 1;

                    }
                    else
                    {
                        isNew = true;
                        Channel channel1 = new Channel();

                        channel1.title = channel.title;
                        channel1.about = channel.about.Trim();

                        channel1.broadcast = true;
                        channel1.megagroup = channel.megagroup;
                        db.Channels.Add(channel1);
                        if (isNew)
                            Save();
                        if (errorflag == 0)
                        {
                            channel.channelId = channel1.channelId;
                            response.success = true;
                            response.message = "Channel added successfully with Id = " + channel1.channelId;
                            response.data = channel1;
                            response.Status_Code = 200;
                        }
                    }
                }
                else
                {
                    response.success = true;
                    response.message = "Some fields are missings!!!, Please check.";
                    response.data = channel;
                    response.Status_Code = 3;
                }
            }
            catch (Exception exception)
            {
                response.message = exception.Message;
                response.Status_Code = 400;
                response.success = false;
                response.data = new { };
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
                        response.success = true;
                        response.message = "channel already exists.";
                        response.data = link;
                        response.Status_Code = 1;

                    }
                    else
                    {
                        isNew = true;
                        Link link1 = new Link();

                        link1.link = link.link;
                        link1.IsOnceClicked = true;
                        link1.channelId = link.channelId;
                        link1.CreatedDate = DateTime.Now;
                        link1.ExpiredDate = DateTime.Now.AddDays(1);
                        db.Links.Add(link1);
                        if (isNew)
                            Save();
                        if (errorflag == 0)
                        {
                            link.Id = link1.Id;
                            response.success = true;
                            response.message = "Channel added successfully with Id = " + link1.Id;
                            response.data = link1;
                            response.Status_Code = 200;
                        }
                    }
                }
                else
                {
                    response.success = true;
                    response.message = "Some fields are missings!!!, Please check.";
                    response.data = link;
                    response.Status_Code = 3;
                }
            }
            catch (Exception exception)
            {
                response.message = exception.Message;
                response.Status_Code = 400;
                response.success = false;
                response.data = new { };
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
                response.message = ex.Message;
                errorflag = 1;
            }
        }
    }
}
