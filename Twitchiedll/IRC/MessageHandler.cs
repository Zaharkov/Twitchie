using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Twitchiedll.IRC.Enums;
using Twitchiedll.IRC.Limits;

namespace Twitchiedll.IRC
{
    public class MessageHandler : IDisposable
    {
        /*
        It's a global limit and the 100 threshold only applies 
        if you are only sending messages or commands in channels you are a mod in.
        For example: You send 50 messages to a channel where you are modded 
        and then send 1 message to a channel where you are not modded within 30 seconds. 
        Since 50 + 1 = 51 and 51 > 20, you will have exceeded the limit for non-modded channels
        and be locked out globally (by IP) for 2 hours.

        Whispers don't impact your channel send limit, but have own limits (don't know whats limits..gotted by tests)
        whisper_limit_per_sec = 3
        whisper_limit_per_min = 100
        */
        public static MessageLimit GlobalMessageLimit = MessageLimit.Viewer;

        public readonly CancellationTokenSource TokenSource = new CancellationTokenSource();

        private static readonly List<MessageContainer> MessageContainers = new List<MessageContainer>();
        private static readonly List<MessageContainer> WhisperContainers = new List<MessageContainer>();
        private static readonly object LockForCheckMessage = new object();
        private static readonly object LockForCheckWhisper = new object();
        private static readonly object LockForSend = new object();
        private readonly TextWriter _textWriter;

        public MessageHandler(TextWriter writer)
        {
            _textWriter = writer;
        }

        public void WriteRawMessage(string rawMessage, bool whisper = false, bool needWait = false)
        {
            var container = new MessageContainer(rawMessage);

            if (whisper)
            {
                Task.Run(() =>
                {
                    var added = false;
                    while (!IsCanSendWhisper(container, TokenSource.Token, ref added))
                        Thread.Sleep(1000);

                    TokenSource.Token.ThrowIfCancellationRequested();

                    WriteMessage(container);
                }, TokenSource.Token);

                return;
            }

            Action action = () =>
            {
                var added = false;
                while (!IsCanSendMessage(container, TokenSource.Token, needWait, ref added))
                    Thread.Sleep(1000);

                if (!needWait)
                    TokenSource.Token.ThrowIfCancellationRequested();

                WriteMessage(container);
            };

            if (needWait)
            {
                var task = Task.Run(action);
                task.Wait();
            }
            else
            {
                Task.Run(action, TokenSource.Token);
            }
        }

        public void SendMessage(MessageType messageType, string channel, string message)
        {
            var whisper = channel.Equals("jtv");

            switch (messageType)
            {
                case MessageType.Action:
                    WriteRawMessage($"PRIVMSG #{channel} :{TwitchConstName.Action} {message}", whisper);
                    break;

                case MessageType.Message:
                    WriteRawMessage($"PRIVMSG #{channel} :{message}", whisper);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(messageType), messageType, null);
            }
        }

        private void WriteMessage(MessageContainer messageContainer)
        {
            lock (LockForSend)
            {
                messageContainer.Time = DateTime.Now;
                _textWriter.WriteLine(messageContainer.Message);
                _textWriter.Flush();
            }
        }

        private static bool IsCanSendMessage(MessageContainer messageContainer, CancellationToken token, bool needWait, ref bool added)
        {
            lock (LockForCheckMessage)
            {
                if (!added)
                {
                    MessageContainers.Add(messageContainer);
                    added = true;
                }

                if (token.IsCancellationRequested && !needWait)
                {
                    MessageContainers.Remove(messageContainer);
                    token.ThrowIfCancellationRequested();
                }

                var timeCheck = DateTime.Now.AddSeconds(-30);
                var count = 0;
                var copy = MessageContainers.ToList();

                foreach (var message in copy)
                {
                    if (message.Time > timeCheck)
                        count++;
                    else
                        MessageContainers.Remove(message);
                }

                return count < (int)GlobalMessageLimit;
            }
        }

        private static bool IsCanSendWhisper(MessageContainer messageContainer, CancellationToken token, ref bool added)
        {
            lock (LockForCheckWhisper)
            {
                if (!added)
                {
                    WhisperContainers.Add(messageContainer);
                    added = true;
                }

                if (token.IsCancellationRequested)
                {
                    WhisperContainers.Remove(messageContainer);
                    token.ThrowIfCancellationRequested();
                }

                var timeCheckSecond = DateTime.Now.AddSeconds(-1);
                var timeCheckMinute = DateTime.Now.AddSeconds(-60);
                var countSecond = 0;
                var countMinute = 0;
                var copy = WhisperContainers.ToList();

                foreach (var message in copy)
                {
                    if (message.Time > timeCheckSecond)
                        countSecond++;
                }

                foreach (var message in copy)
                {
                    if (message.Time > timeCheckMinute)
                        countMinute++;
                    else
                        WhisperContainers.Remove(message);
                }
                return countSecond < (int)WhisperLimit.PerSecond && countMinute < (int)WhisperLimit.PerMinute;
            }
        }

        public void Dispose()
        {
            if(!TokenSource.IsCancellationRequested)
                TokenSource.Cancel();

            _textWriter.Dispose();
        }
    }
}