using System.Diagnostics;
using Twitchiedll.IRC.Events;

namespace Twitchiedll.IRC
{
    public partial class Twitchie
    {
        public event RawMessageHandler OnRawMessage;
        public event PrivMessageHandler OnMessage;
        public event PingHandler OnPing;
        public event RoomStateHandler OnRoomState;
        public event ModeHandler OnMode;
        public event NamesHandler OnNames;
        public event JoinHandler OnJoin;
        public event PartHandler OnPart;
        public event NoticeHandler OnNotice;
        public event SubscriberHandler OnSubscribe;
        public event HostTargetHandler OnHostTarget;
        public event ClearChatHandler OnClearChat;
        public event UserStateHandler OnUserState;
        public event WhisperHandler OnWhisper;

        private void HandleEvents()
        {
            OnRawMessage?.Invoke(_buffer);

            if (_buffer.StartsWith("PING"))
            {
                OnPing?.Invoke(_buffer);
                return;
            }

            var commandIndex = _buffer.StartsWith("@") ? 2 : 1;
            var command =  _buffer.Split(' ')[commandIndex];

            switch (command)
            {
                case "PRIVMSG":
                {
                    OnMessage?.Invoke(new MessageEventArgs(_buffer));

                    if (_buffer.StartsWith(":twitchnotify!twitchnotify@twitchnotify.tmi.twitch.tv"))
                        OnSubscribe?.Invoke(new SubscriberEventArgs(_buffer));

                    break;
                }

                case "ROOMSTATE":
                    OnRoomState?.Invoke(new RoomStateEventArgs(_buffer));
                    break;

                case "MODE":
                    OnMode?.Invoke(new ModeEventArgs(_buffer));
                    break;

                case "353":
                    _namesEventArgs.GetNames(_buffer);
                    break;

                case "366":
                    OnNames?.Invoke(_namesEventArgs);
                    break;

                case "JOIN":
                    OnJoin?.Invoke(new JoinEventArgs(_buffer));
                    break;

                case "PART":
                    OnPart?.Invoke(new PartEventArgs(_buffer));
                    break;

                case "NOTICE":
                    OnNotice?.Invoke(new NoticeEventArgs(_buffer));
                    break;

                case "HOSTTARGET":
                    OnHostTarget?.Invoke(new HostTargetEventArgs(_buffer));
                    break;

                case "CLEARCHAT":
                    OnClearChat?.Invoke(new ClearChatEventArgs(_buffer));
                    break;

                case "WHISPER":
                    OnWhisper?.Invoke(new MessageEventArgs(_buffer));
                    break;

                case "USERSTATE":
                    OnUserState?.Invoke(new UserStateEventArgs(_buffer));
                    break;

                default:
                    Debug.WriteLine(_buffer);
                    break;
            }
        }
    }
}