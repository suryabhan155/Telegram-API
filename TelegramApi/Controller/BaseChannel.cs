using TL;

namespace TelegramApi.Controller
{
    public class BaseChannel : InputChannelBase
    {
        public BaseChannel() { }

        public override long ChannelId => base.ChannelId;

    }
}
