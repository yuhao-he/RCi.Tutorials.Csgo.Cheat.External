using RCi.Tutorials.Csgo.Cheat.External.Data;
using RCi.Tutorials.Csgo.Cheat.External.Utils;
using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RCi.Tutorials.Csgo.Cheat.External.Features
{
    public class commandline :
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
        public commandline(GameProcess gameProcess, GameData gameData, GameConsole gc)
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

        protected HashSet<IntPtr> candidates = new HashSet<IntPtr>();

        /// <inheritdoc />
        protected override void FrameAction()
        {
            Console.WriteLine("Health lookup >>");
            string command = Console.ReadLine();
            int currhealth = Int32.Parse(command);

            Console.WriteLine("Current Health: " + currhealth);
            if (candidates.Count == 0)
            {
                int start = 0x28B2BC; // 0xD8B2BC
                int end = 0xF8B2BC;
                for (int i = start; i < end; i += 4) {
                    int val = GameProcess.Process.Read<int>((IntPtr)start + i);
                    if (val == currhealth) {
                        candidates.Add((IntPtr)start + i);
                    }
                }
                Console.WriteLine("Matched found: " + candidates.Count);
            }
            else {
                foreach (IntPtr ptr in candidates)
                {
                    int val = GameProcess.Process.Read<int>(ptr);
                    if (val != currhealth) {
                        candidates.Remove(ptr);
                    }
                    Console.WriteLine("Left: " + candidates.Count);
                }
            }


            /*
            switch (vals[0])
            {
                case "sonar":
                    GameData.UseSonar = (vals[1] == "1");
                    break;
                case "legit":
                    if (vals.Length != 3)
                    {
                        Console.WriteLine("Invalid Command " + command);
                        break;
                    }
                    float smoothing = float.Parse(vals[1], CultureInfo.InvariantCulture);
                    float fov = float.Parse(vals[2], CultureInfo.InvariantCulture);
                    Console.WriteLine(smoothing + " " + fov + "; " + command);
                    GameData.Smoothing = smoothing;
                    GameData.FOV = fov;
                    break;
                case "trigger":
                    GameData.UseTriggerBot = (vals[1] == "1");
                    break;
                case "overlay":
                    GameData.ShowGraphics = (vals[1] == "1");
                    break;
                default:
                    Console.WriteLine("Unknown Command " + command);
                    break;
            }
            */
        }

        #endregion
    }
}
