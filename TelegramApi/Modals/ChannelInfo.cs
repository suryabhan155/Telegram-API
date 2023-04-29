﻿namespace TelegramApi.Modals
{
    public class ChannelInfo
    {
        public long Id { get; set; }
        public bool isActive { get; set; }
        public string title { get; set; } 
        public TL.ChatPhoto photo { get; set; }  
        public string type { get; set; }  
        public int noofuser { get; set; }
        public TL.InputChannel Channel { get; set; }
        public bool IsRequestJoin { get; set; }
        public int RequestCount { get; set; }
    }
}
