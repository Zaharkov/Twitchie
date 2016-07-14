namespace Twitchiedll.IRC.Events
{
    public class RoomStateEventArgs
    {
        public bool R9K { get; internal set; }
        public bool SubOnly { get; internal set; }
        public bool SlowMode { get; internal set; }
        public string BroadcasterLanguage { get; internal set; }
        public string Channel { get; internal set; }

        public RoomStateEventArgs(string ircMessage)
        {
            if (ircMessage.Split(';').Length > 3)
            {
                if (ircMessage.Split(';')[0].Split('=').Length > 1)
                    BroadcasterLanguage = ircMessage.Split(';')[0].Split('=')[1];

                if (ircMessage.Split(';')[1].Split('=').Length > 1)
                    R9K = ToBoolean(ircMessage.Split(';')[1].Split('=')[1]);

                if (ircMessage.Split(';')[2].Split('=').Length > 1)
                    SlowMode = ToBoolean(ircMessage.Split(';')[2].Split('=')[1]);

                if (ircMessage.Split(';')[3].Split('=').Length > 1)
                    SubOnly = ToBoolean(ircMessage.Split(';')[3].Split('=')[1]);

                Channel = ircMessage.Split('#')[1];
            }
        }

        private static bool ToBoolean(string str)
            => str == "1";
    }
}