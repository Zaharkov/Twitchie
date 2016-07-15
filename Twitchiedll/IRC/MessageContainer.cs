using System;

namespace Twitchiedll.IRC
{
    public class MessageContainer
    {
        public string Message { get; }
        public DateTime Time { get; set; }

        public MessageContainer(string message)
        {
            Message = message;
            Time = DateTime.Now;
        }
    }
}
