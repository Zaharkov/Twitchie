using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using Twitchiedll.IRC.Events;
using Twitchiedll.IRC.Interfaces;

namespace Twitchiedll.IRC
{
    public partial class Twitchie : IDisposable
    {
        private string _buffer;
        private readonly List<string> _channels = new List<string>();
        private readonly NamesEventArgs _namesEventArgs = new NamesEventArgs();

        private TextReader _textReader;
        private MessageHandler _messageHandler;
        private TcpClient _clientSocket;

        protected ILogger Logger;

        public Twitchie()
        {
            OnPing += Pong;
        }

        private void LogException(string message, Exception e)
        {
            Logger?.LogException(message, e);
        }

        public void Connect(string server, int port)
        {
            _clientSocket = new TcpClient();
            _clientSocket.Connect(server, port);

            if (!_clientSocket.Connected)
                throw new Exception("Connection failed");

            var stream = _clientSocket.GetStream();

            _textReader = new StreamReader(stream);
            var writer = new StreamWriter(stream);
            _messageHandler = new MessageHandler(writer); 
        }

        public virtual void Login(string nick, string password)
        {
            _messageHandler.WriteRawMessage($"USER {nick}", false, true);
            _messageHandler.WriteRawMessage($"PASS {password}", false, true);
            _messageHandler.WriteRawMessage($"NICK {nick}", false, true);

            _buffer = _textReader.ReadLine();

            if(_buffer == null)
                throw new Exception("Login failed");

            if (_buffer.Split(' ')[1] != "001")
                throw new Exception("Registration Failed. Welcome message expected");

            _messageHandler.WriteRawMessage("CAP REQ :twitch.tv/membership", false, true);
            _messageHandler.WriteRawMessage("CAP REQ :twitch.tv/commands", false, true);
            _messageHandler.WriteRawMessage("CAP REQ :twitch.tv/tags", false, true);
        }

        public void Listen()
        {
            while ((_buffer = _textReader.ReadLine()) != null)
            {
                if (_buffer != null)
                {
                    try
                    {
                        HandleEvents();
                    }
                    catch (Exception ex)
                    {
                        LogException("Failed handle events", ex);
                        throw;
                    }
                }
            }
        }

        public void Pong(string ping)
        {
            _messageHandler.WriteRawMessage(ping.Replace("PING", "PONG"));
        }

        public void Join(string channel)
        {
            _channels.Add(channel);
            _messageHandler.WriteRawMessage($"JOIN #{channel}");
        }

        public void Part(string channel)
        {
            _messageHandler.WriteRawMessage($"PART #{channel}");
        }

        public void PartFromAll()
        {
            foreach (var channel in _channels)
                Part(channel);
        }

        public void Whisper(string user, string message)
        {
            Message("jtv", $"/w {user} {message}");
        }

        public void Message(string channel, string message)
        {
            _messageHandler.SendMessage(MessageType.Message, channel, message);
        }

        public void Action(string channel, string message)
        {
            _messageHandler.SendMessage(MessageType.Action, channel, message);
        }

        public void Timeout(string channel, string user, int timeout)
        {
            _messageHandler.SendMessage(MessageType.Message, channel, $"/timeout {user} {timeout}");
        }

        public void Ban(string channel, string user)
        {
            _messageHandler.SendMessage(MessageType.Message, channel, $"/ban {user}");
        }

        public virtual void Quit()
        {
            _messageHandler.TokenSource.Cancel();
            _messageHandler.WriteRawMessage("QUIT", false, true);
        }

        public void Dispose()
        {
            _textReader.Dispose();
            _messageHandler.Dispose();
            _clientSocket.Dispose();
        }
    }
}