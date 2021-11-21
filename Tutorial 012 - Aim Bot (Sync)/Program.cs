using System;
using System.Net;
using RCi.Tutorials.Csgo.Cheat.External.Data;
using RCi.Tutorials.Csgo.Cheat.External.Features;
using RCi.Tutorials.Csgo.Cheat.External.Gfx;

namespace RCi.Tutorials.Csgo.Cheat.External
{
    /// <summary>
    /// Main program.
    /// </summary>
    public class Program :
        System.Windows.Application,
        IDisposable
    {
        #region // entry point

        /// <summary />
        [STAThread]
        public static void Main() => new Program().Run();

        #endregion

        #region // storage

        /// <inheritdoc cref="GameProcess"/>
        private GameProcess GameProcess { get; set; }

        /// <inheritdoc cref="GameConsole"/>
        private GameConsole GameConsole { get; set; }

        /// <inheritdoc cref="GameData"/>
        private GameData GameData { get; set; }

        /// <inheritdoc cref="WindowOverlay"/>
        private WindowOverlay WindowOverlay { get; set; }

        /// <inheritdoc cref="Graphics"/>
        private Graphics Graphics { get; set; }

        /// <inheritdoc cref="TriggerBot"/>
        private TriggerBot TriggerBot { get; set; }

        /// <inheritdoc cref="AimBot"/>
        private AimBot AimBot { get; set; }

        /// <inheritdoc cref="Sonar"/>
        private Sonar Sonar { get; set; }

        private HTTPServer HTTPServer { get; set; }

        private commandline cmd { get; set; }
        #endregion

        #region // ctor

        /// <summary />
        public Program()
        {
            Startup += (sender, args) => Ctor();
            Exit += (sender, args) => Dispose();
        }
        public void SimpleListener(string[] prefixes)
        {
            if (!HttpListener.IsSupported)
            {
                Console.WriteLine("Windows XP SP2 or Server 2003 is required to use the HttpListener class.");
                return;
            }
            // URI prefixes are required,
            // for example "http://contoso.com:8080/index/".
            if (prefixes == null || prefixes.Length == 0)
                throw new ArgumentException("prefixes");

            // Create a listener.
            HttpListener listener = new HttpListener();
            // Add the prefixes.
            foreach (string s in prefixes)
            {
                listener.Prefixes.Add(s);
            }
            listener.Start();
            Console.WriteLine("Listening...");
            // Note: The GetContext method blocks while waiting for a request.
            HttpListenerContext context = listener.GetContext();
            HttpListenerRequest request = context.Request;
            // Obtain a response object.
            HttpListenerResponse response = context.Response;
            // Construct a response.
            string responseString = "<HTML><BODY> Hello world!</BODY></HTML>";
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
            // Get a response stream and write the response to it.
            response.ContentLength64 = buffer.Length;
            System.IO.Stream output = response.OutputStream;
            output.Write(buffer, 0, buffer.Length);
            // You must close the output stream.
            output.Close();
            listener.Stop();
        }

        /// <summary />
        public void Ctor()
        {
            GameProcess = new GameProcess();
            GameConsole = new GameConsole(GameProcess);
            GameData = new GameData(GameProcess);
            WindowOverlay = new WindowOverlay(GameProcess);
            Graphics = new Graphics(WindowOverlay, GameProcess, GameData);
            TriggerBot = new TriggerBot(GameProcess, GameData);
            AimBot = new AimBot(GameProcess, GameData, false); // Friendly fire
            Sonar = new Sonar(GameProcess, GameData, GameConsole);
            cmd = new commandline(GameProcess, GameData, GameConsole);
            HTTPServer = new HTTPServer(GameProcess, GameData, GameConsole);

            GameProcess.Start();
            GameData.Start();
            WindowOverlay.Start();
            Graphics.Start();
            TriggerBot.Start();
            AimBot.Start();
            // Sonar.Start();
            cmd.Start();
            HTTPServer.Start();
        }

        /// <inheritdoc />
        public void Dispose()
        {
            AimBot.Dispose();
            AimBot = default;

            TriggerBot.Dispose();
            TriggerBot = default;

            Graphics.Dispose();
            Graphics = default;

            WindowOverlay.Dispose();
            WindowOverlay = default;

            GameData.Dispose();
            GameData = default;

            GameProcess.Dispose();
            GameProcess = default;

            Sonar.Dispose();
            Sonar = default;

            cmd.Dispose();
            cmd = default;
        }

        #endregion
    }
}
