using Twitchiedll.IRC.Events;

namespace Twitchiedll.IRC
{
    public delegate void RawMessageHandler(string rawMessage);

    public delegate void PrivMessageHandler(MessageEventArgs e);

    public delegate void PingHandler(string rawMessage);

    public delegate void RoomStateHandler(RoomStateEventArgs e);

    public delegate void ModeHandler(ModeEventArgs e);

    public delegate void NamesHandler(NamesEventArgs e);

    public delegate void JoinHandler(JoinEventArgs e);

    public delegate void PartHandler(PartEventArgs e);

    public delegate void NoticeHandler(NoticeEventArgs e);

    public delegate void SubscriberHandler(SubscriberEventArgs e);

    public delegate void HostTargetHandler(HostTargetEventArgs e);

    public delegate void ClearChatHandler(ClearChatEventArgs e);

    public delegate void UserStateHandler(UserStateEventArgs e);

    public delegate void WhisperHandler(MessageEventArgs e);

    public delegate void UserNoticeHandler(UserNoticeEventArgs e);
}