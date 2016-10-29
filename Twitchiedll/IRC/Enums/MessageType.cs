namespace Twitchiedll.IRC.Enums
{
    public enum MessageType
    {
        Message,
        Action
    }

    public static class TwitchConstName
    {
        public const string Action = "/me";
        public const char Command = '!';
        public const char UserStartName = '@';
    }
}