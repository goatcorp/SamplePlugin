using Dalamud.Game.Command;
using Dalamud.IoC;
using Dalamud.Plugin;
using System.IO;
using System.Reflection;
using Dalamud.Interface.Windowing;
using Dancingway.Windows;
using XivCommon.Functions;
using XivCommon;
using System.Dynamic;

namespace Dancingway
{
    public sealed class Plugin : IDalamudPlugin
    {
        public string Name => "Dancingway";

        private DalamudPluginInterface PluginInterface { get; init; }
        private CommandManager CommandManager { get; init; }

        private XivCommonBase XivCommon { get; }

        public Configuration Configuration { get; init; }
        public WindowSystem WindowSystem = new("Dancingway");

        public Dancingway.EmoteList emoteLister = new EmoteList();

        public Plugin(
            [RequiredVersion("1.0")] DalamudPluginInterface pluginInterface,
            [RequiredVersion("1.0")] CommandManager commandManager)
        {
            this.PluginInterface = pluginInterface;
            this.CommandManager = commandManager;
            XivCommon = new XivCommonBase();

            this.Configuration = this.PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
            this.Configuration.Initialize(this.PluginInterface);

            // you might normally want to embed resources and load them from the manifest stream
            var imagePath = Path.Combine(PluginInterface.AssemblyLocation.Directory?.FullName!, "goat.png");
            var goatImage = this.PluginInterface.UiBuilder.LoadImage(imagePath);

            WindowSystem.AddWindow(new ConfigWindow(this));
            WindowSystem.AddWindow(new MainWindow(this, goatImage));

            //this.CommandManager.AddHandler(CommandName, new CommandInfo(OnDancingway)
            this.CommandManager.AddHandler("/dancingway", new CommandInfo(OnDancingway)
            {
                HelpMessage = "Open Dancingway settings window. Use '/dancingway'."
            });

            //this.CommandManager.AddHandler(CommandName, new CommandInfo(OnDDR)
            this.CommandManager.AddHandler("/ddr", new CommandInfo(OnDDR)
            {
                HelpMessage = "Executes a random dance from the chosen list. Use '/ddr'."
            });

            this.PluginInterface.UiBuilder.Draw += DrawUI;
            this.PluginInterface.UiBuilder.OpenConfigUi += DrawConfigUI;
        }

        public void Dispose()
        {
            this.WindowSystem.RemoveAllWindows();
            this.CommandManager.RemoveHandler("/dancingway");
            this.CommandManager.RemoveHandler("/ddr");
        }

        private void OnDancingway(string command, string args)
        {
            // in response to the slash command, just display our main ui
            WindowSystem.GetWindow("Dancingway Settings Window").IsOpen = true;
            // initialise the list of emotes
            emoteLister.InitialiseEmoteList();

            // TEST TEST
            emoteLister.TestBuild();
        }

        private void OnDDR(string command, string args)
        {
            /* in response to the slash command, dance, baby, dance!
            XivCommon.Functions.Chat.SendMessage("/dance");
            old code */

            // new test code
            XivCommon.Functions.Chat.SendMessage(emoteLister.getRandomDance());
        }

        private void DrawUI()
        {
            this.WindowSystem.Draw();
        }

        public void DrawConfigUI()
        {
            WindowSystem.GetWindow("A Wonderful Configuration Window").IsOpen = true;
        }
    }
}
