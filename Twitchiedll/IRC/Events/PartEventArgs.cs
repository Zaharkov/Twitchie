namespace Twitchiedll.IRC.Events
{
    public class PartEventArgs
    {
        public string Username { get; internal set; }
        public string Channel { get; internal set; }

        public PartEventArgs(string ircMessage)
        {
            Username = ircMessage.Split(' ')[0].Split(':')[1].Split('!')[0];
            Channel = ircMessage.Split(' ')[2].TrimStart('#');
        }
    }
}