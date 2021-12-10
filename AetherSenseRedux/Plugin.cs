using Dalamud.Game.Command;
using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Logging;
using Dalamud.Game.Gui;
using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

using Buttplug;
using AetherSenseRedux.Trigger;
using System.Collections.Concurrent;
using AetherSenseRedux.Pattern;

namespace AetherSenseRedux
{
    public sealed class Plugin : IDalamudPlugin
    {
        public string Name => "AetherSense Redux";
        public bool Running = false;

        private const string commandName = "/asr";

        private DalamudPluginInterface PluginInterface { get; init; }
        private CommandManager CommandManager { get; init; }
        private Configuration Configuration { get; set; }
        [PluginService] private ChatGui ChatGui { get; init; } = null!;
        private PluginUI PluginUi { get; init; }


        private ButtplugClient Buttplug;

        private List<Device> DevicePool;

        private readonly List<ChatTrigger> ChatTriggerPool;

        public Plugin(
            [RequiredVersion("1.0")] DalamudPluginInterface pluginInterface,
            [RequiredVersion("1.0")] CommandManager commandManager)
        {
            PluginInterface = pluginInterface;
            CommandManager = commandManager;

            PluginInterface.Inject(this);

            Buttplug = new ButtplugClient("AetherSense Redux");
            Buttplug.DeviceAdded += OnDeviceAdded;
            Buttplug.DeviceRemoved += OnDeviceRemoved;
            Buttplug.ScanningFinished += OnScanComplete;

            this.DevicePool = new List<Device>();
            this.ChatTriggerPool = new List<ChatTrigger>();

            Configuration = PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
            Configuration.Initialize(PluginInterface);
            Configuration.FixDeserialization();
            if (!Configuration.Initialized)
            {
                Configuration.LoadDefaults();
            }

            // you might normally want to embed resources and load them from the manifest stream
            var assemblyLocation = Assembly.GetExecutingAssembly().Location;
            PluginUi = new PluginUI(Configuration, this);

            CommandManager.AddHandler(commandName, new CommandInfo(OnShowUI)
            {
                HelpMessage = "Opens the Aether Sense Redux configuration window"
            });

            PluginInterface.UiBuilder.Draw += DrawUI;
            PluginInterface.UiBuilder.OpenConfigUi += DrawConfigUI;
        }

        public void Dispose()
        {
            Stop();
            PluginUi.Dispose();
            CommandManager.RemoveHandler(commandName);
        }

        private void OnDeviceAdded(object? sender, DeviceAddedEventArgs e)
        {

            PluginLog.Information("Device {0} added", e.Device.Name);
            Device newDevice = new Device(e.Device);
            this.DevicePool.Add(newDevice);
            if (!Configuration.SeenDevices.Contains(newDevice.Name)){
                Configuration.SeenDevices.Add(newDevice.Name);
            }
            Task.Run(newDevice.Run).ConfigureAwait(false);

        }

        private void OnDeviceRemoved(object? sender, DeviceRemovedEventArgs e)
        {
            PluginLog.Information("Device {0} removed", e.Device.Name);
            var toRemove = new List<Device>();
            lock (this.DevicePool)
            {
                foreach (Device device in this.DevicePool)
                {
                    if (device.ClientDevice == e.Device)
                    {
                        try
                        {
                            device.Stop();
                        }
                        catch (Exception ex)
                        {
                            PluginLog.Error(ex, "Could not stop device {0}, device disconnected?", device.Name);
                        }
                        toRemove.Add(device);
                    }
                }
            }
                foreach (Device device in toRemove)
                {
                    lock (this.DevicePool)
                    {
                        this.DevicePool.Remove(device);
                    }
                    
                }
        }

        // Instead of constant async scanning, it may make sense to simply scan when a command is issued
        private void OnScanComplete(object? sender, EventArgs e)
        {
            Task.Run(DoScan).ConfigureAwait(false);
        }

        public void DoPatternTest(dynamic patternConfig)
        {
            if (!Buttplug.Connected)
            {
                return;
            }
            lock (DevicePool) {
                foreach (var device in this.DevicePool)
                {
                    lock (device.Patterns)
                    {
                        device.Patterns.Add(PatternFactory.GetPatternFromString(patternConfig.Type,patternConfig));
                    }
            }
            }
        }

        private void OnChatReceived(XivChatType type, uint senderId, ref SeString sender, ref SeString message, ref bool isHandled)
        {
            ChatMessage chatMessage = new ChatMessage(type, senderId, ref sender, ref message, ref isHandled);
            foreach (ChatTrigger t in ChatTriggerPool)
            {
                t.Queue(chatMessage);
            }
            if (Configuration.LogChat)
            {
                PluginLog.Debug(chatMessage.ToString());
            }
        }
        private void InitButtplug()
        {
            //TODO: connect to buttplug and start scanning for devices
            if (!Buttplug.Connected)
            {
                try
                {
                    ButtplugWebsocketConnectorOptions wsOptions = new ButtplugWebsocketConnectorOptions(new Uri(Configuration.Address));
                    var t = Buttplug.ConnectAsync(wsOptions);
                    t.Wait();
                }
                catch (Exception ex)
                {
                    PluginLog.Error(ex, "Buttplug failed to connect");
                    Stop();
                }
            }
            Task.Run(DoScan).ConfigureAwait(false);
            PluginLog.Debug("Buttplug created.");
        }

        private void DestroyButtplug()
        {
            lock (DevicePool)
            {
                foreach (Device device in DevicePool)
                {
                    PluginLog.Debug("Stopping device {0}", device.Name);
                    device.Stop();
                }
                DevicePool.Clear();
            }
            try {
                var t = Buttplug.DisconnectAsync();
                t.Wait(); 
            }
            catch (Exception ex)
            {
                PluginLog.Error(ex, "Buttplug failed to connect");
            }
            PluginLog.Debug("Buttplug destroyed.");
        }
        private void InitTriggers()
        {
            foreach (var d in Configuration.Triggers)
            {
                var Trigger = TriggerFactory.GetTriggerFromConfig(d, ref DevicePool);
                if (Trigger.Type == "ChatTrigger")
                {
                    ChatTriggerPool.Add((ChatTrigger)Trigger);
                } else
                {
                    PluginLog.Error("Invalid trigger type {0} created.", Trigger.Type);
                }
            }

            foreach (ChatTrigger t in ChatTriggerPool)
            {
                PluginLog.Debug("Starting chat trigger {0}",t.Name);
                t.Start();
            }

            ChatGui.ChatMessage += OnChatReceived;
            PluginLog.Debug("Triggers created");
        }
        private void DestroyTriggers()
        {
            foreach (ChatTrigger t in ChatTriggerPool)
            {
                PluginLog.Debug("Stopping chat trigger {0}",t.Name);
                t.Stop();
            }
            ChatGui.ChatMessage -= OnChatReceived;
            ChatTriggerPool.Clear();
            PluginLog.Debug("Triggers destroyed.");
        }
        public void Start()
        {
            Running = true;            
            InitTriggers();
            InitButtplug();
        }

        public void Restart()
        {
            DestroyTriggers();

            // while this works, a cleaner restart that doesn't drop the intiface connection may be in order
            //Stop();
            //Start();

            InitTriggers();
        }

        public void Stop()
        {
            DestroyTriggers();
            DestroyButtplug();
            Running = false;


        }

        private async Task DoScan()
        {
            await Task.Delay(1000);
            try
            {
                await Buttplug.StartScanningAsync();
            }
            catch (Exception ex)
            {
                PluginLog.Error(ex, "Asynchronous scanning failed.");
            }
        }

        private void OnShowUI(string command, string args)
        {
            // in response to the slash command, just display our main ui
            PluginUi.SettingsVisible = true;
        }

        private void DrawUI()
        {
            PluginUi.Draw();
        }

        private void DrawConfigUI()
        {
            PluginUi.SettingsVisible = true;
        }
    }
}
