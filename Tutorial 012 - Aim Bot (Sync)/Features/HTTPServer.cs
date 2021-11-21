using Newtonsoft.Json;
using RCi.Tutorials.Csgo.Cheat.External.Data;
using RCi.Tutorials.Csgo.Cheat.External.Utils;
using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace RCi.Tutorials.Csgo.Cheat.External.Features
{
    class PlayerInfo {
        public int team;
        public float x;
        public float y;
        public float z;
        public bool isPlayer;
        public bool isAlive;
        public bool isDormant;
        public bool isSpotted;
        public int health;
    }

    public class HTTPServer :
       ThreadedComponent
    {
        #region // storage

        /// <inheritdoc />
        protected override string ThreadName => nameof(commandline);

        /// <inheritdoc cref="GameProcess"/>
        private GameProcess GameProcess { get; set; }

        /// <inheritdoc cref="GameData"/>
        private GameData GameData { get; set; }

        /// <inheritdoc cref="GameConsole"/>
        private GameConsole GameConsole { get; set; }

        private bool toggle = true;
        #endregion

        #region // ctor

        /// <summary />
        public HTTPServer(GameProcess gameProcess, GameData gameData, GameConsole gc)
        {
            GameProcess = gameProcess;
            GameData = gameData;
            GameConsole = gc;
        }

        /// <inheritdoc />
        public override void Dispose()
        {
            base.Dispose();

            GameData = default;
            GameProcess = default;
        }

        #endregion

        #region // routines

        protected string locations() {
            var playerInfos = new List<PlayerInfo>();
            foreach (var entity in GameData.Entities)
            {
                playerInfos.Add(new PlayerInfo
                {
                    team = (int)entity.Team,
                    x = entity.BonesPos[0].X,
                    y = entity.BonesPos[0].Y,
                    z = entity.BonesPos[0].Z,
                    isPlayer = entity.AddressBase == GameData.Player.AddressBase,
                    health = entity.Health,
                    isAlive = entity.IsAlive(),
                    isDormant = entity.Dormant,
                    isSpotted = entity.Spotted,
                });
            }
            return JsonConvert.SerializeObject(playerInfos.ToArray());
        }
        /// <inheritdoc />
        protected override void FrameAction()
        {
            System.IO.File.WriteAllText(@"C:\Users\heyuh\Desktop\csgo_map\locations.json", locations());
            /*
            using (HttpListener listener = new HttpListener())
            {
                listener.AuthenticationSchemes = AuthenticationSchemes.Negotiate;
                listener.Prefixes.Add("http://localhost:8000/");
                listener.Start();

                while (true)
                {
                    HttpListenerContext ctx = listener.GetContext();
                    ctx.Response.StatusCode = 200;
                    ctx.Response.Headers.Add("Access-Control-Allow-Origin", "*");
                    StreamWriter writer = new StreamWriter(ctx.Response.OutputStream);
                    writer.WriteLine(locations());
                    writer.Close();
                    ctx.Response.Close();
                }
                listener.Stop();
            }
            */
        }

        #endregion
    }
}
