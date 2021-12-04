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
        // ITrigger properties
        public bool Enabled { get; set; }
        public string Name;
        private List<Device> _devices;
        private List<string> _enabledDevices;
        private string _pattern;
        private Dictionary<string, object> _patternSettings;

        // ChatTrigger properties
        private List<ChatMessage> _messages;
        private string _regex;
        private long _retriggerDelay;
        private DateTime _retriggerTime;

        public ChatTrigger(string name, ref List<Device> devices, List<string> enabledDevices, string pattern, Dictionary<string,object> patternSettings, string regex, long retriggerDelay)
        {
            // ITrigger properties
            Enabled = true;
            Name = name;
            _devices = devices;
            _enabledDevices = enabledDevices;
            _pattern = pattern;
            _patternSettings = patternSettings;

            // ChatTrigger properties
            _messages = new List<ChatMessage>();
            _regex = regex;
            _retriggerDelay = retriggerDelay;
            _retriggerTime = DateTime.MinValue;

        }


        public void Queue(ChatMessage message)
        {
            if (Enabled)
            {
                _messages.Add(message);
            }
        }

        private void OnTrigger()
        {
            if ((_retriggerDelay > 0) && (DateTime.UtcNow > _retriggerTime))
            {
                _retriggerTime = DateTime.UtcNow + TimeSpan.FromMilliseconds(_retriggerDelay);
            }
            foreach (Device device in _devices)
            {
                if (_enabledDevices.Contains(device.Name))
                {
                    device.Patterns.Add(PatternFactory.GetPatternFromString(_pattern, _patternSettings));
                }

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
                        //TODO: search for match and call OnTrigger() if matched
                        _messages.Remove(message);
                        await Task.Yield();
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
