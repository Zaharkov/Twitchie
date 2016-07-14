namespace Twitchiedll.IRC.Events
{
    public class ModeEventArgs
    {
        public bool AddingMode { get; internal set; }
        public string Username { get; internal set; }
        public string Channel { get; internal set; }

        public ModeEventArgs(string ircMessage)
        {
            var splittedMessage = ircMessage.Split(' ');

            if (splittedMessage[2].StartsWith("#"))
                Channel = splittedMessage[2].TrimStart('#');

            AddingMode = splittedMessage[3].Equals("+o");
            Username = splittedMessage[4];
        }
    }
}