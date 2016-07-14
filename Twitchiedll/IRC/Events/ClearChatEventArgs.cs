namespace Twitchiedll.IRC.Events
{
    public class ClearChatEventArgs
    {
        public bool IsTimeout { get; internal set; }
        public string Channel { get; internal set; }
        public string TimeoutUsername { get; internal set; }

        public ClearChatEventArgs(string ircMessage)
        {
            var splittedMsg = ircMessage.Split(' ');

            Channel = splittedMsg[2].TrimStart('#');

            if (splittedMsg.Length > 3)
            {
                IsTimeout = true;
                TimeoutUsername = splittedMsg[3].Replace(":", "");
            }
            else
            {
                IsTimeout = false;
                TimeoutUsername = "";
            }
        }
    }
}