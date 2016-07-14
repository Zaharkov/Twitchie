namespace Twitchiedll.IRC
{
    public enum IrcState
    {
        Closed,
        Connecting,
        Connected,
        Registering,
        Registered,
        Closing,
        Reconnecting,
        Error
    }
}
