namespace Twitchiedll.IRC.Events
{
    public class NoticeEventArgs
    {
        public string Channel { get; internal set; }
        public string Message { get; internal set; }
        public Noticetype NoticeType { get; internal set; }

        public NoticeEventArgs(string ircMessage)
        {
            var splittedMessage = ircMessage.Split(' ');

            switch (splittedMessage[0].Split('=')[1])
            {
                case "subs_on":
                    NoticeType = Noticetype.SubsOn;
                    break;

                case "subs_off":
                    NoticeType = Noticetype.SubsOff;
                    break;

                case "slow_on":
                    NoticeType = Noticetype.SlowOn;
                    break;

                case "slow_off":
                    NoticeType = Noticetype.SlowOff;
                    break;

                case "r9k_on":
                    NoticeType = Noticetype.R9KOn;
                    break;

                case "r9k_off":
                    NoticeType = Noticetype.R9KOff;
                    break;

                case "host_on":
                    NoticeType = Noticetype.HostOn;
                    break;

                case "host_off":
                    NoticeType = Noticetype.HostOff;
                    break;
            }

            Channel = splittedMessage[3].TrimStart('#');
            Message = ircMessage.Split(':')[2];
        }
    }
}