using AetherSenseRedux.Pattern;
using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AetherSenseRedux.Trigger
{
    internal class ChatTrigger
    {
        public bool Enabled { get; set; }
        private string Name;
        private string Regex;
        private string Pattern;
        private Dictionary<string, object> PatternSettings;

        private List<ChatMessage> _messages;
        private List<Device> _devices;

        public ChatTrigger(string name, string regex, string pattern, ref List<Device> devices, Dictionary<string,object> patternSettings)
        {
            Enabled = true;
            Name = name;
            Regex = regex;
            Pattern = pattern;
            PatternSettings = patternSettings;
            _messages = new List<ChatMessage>();
            this._devices = devices;
        }


        public void Queue(ChatMessage message)
        {
            _messages.Add(message);
        }

        private void OnMatch()
        {
            foreach (Device device in _devices)
            {
                device.Patterns.Add(PatternFactory.GetPatternFromString(Pattern,PatternSettings));
            }
        }

        public async Task Run()
        {
            while (Enabled)
            {
                if (_messages.Count > 0)
                {
                    foreach (ChatMessage message in _messages)
                    {
                        //TODO: search for match and call OnMatch() if matched
                        _messages.Remove(message);
                        await Task.Delay(1);
                    }
                }
                await Task.Delay(10);
            }
        }
    }

    struct ChatMessage
    {

        public ChatMessage(XivChatType chatType, uint senderId, ref SeString sender, ref SeString message, ref bool isHandled)
        {
            ChatType = chatType;
            SenderId = senderId;
            Sender = sender;
            Message = message;
            IsHandled = isHandled;
        }

        public XivChatType ChatType;
        public uint SenderId;
        public SeString Sender;
        public SeString Message;
        public bool IsHandled;

        public override string ToString()
        {
            return string.Format("<{0}> {1}", Sender.TextValue, Message.TextValue);
        }
    }
}
