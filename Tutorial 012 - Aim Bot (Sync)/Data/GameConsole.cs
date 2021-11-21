using RCi.Tutorials.Csgo.Cheat.External.Sys;
using RCi.Tutorials.Csgo.Cheat.External.Sys.Data;
using RCi.Tutorials.Csgo.Cheat.External.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace RCi.Tutorials.Csgo.Cheat.External.Data
{
    public class GameConsole
    {
        private System.Timers.Timer writeTimer;

        private string CFG_PATH;

        private FileInfo CFG_INFO;

        private bool isWriting = false;

        private List<string> CommandQueue = new List<string>();

        public static bool cfgIsReady = true;

        private GameProcess GameProcess;

        public GameConsole(GameProcess gp)
        {
            GameProcess = gp;

            // Lets add required keybinding to users cs go config on first setup
            if (!checkIfCfgIsReady())
            {
                setupUserConfigs();
            }

            // Create cfg file used to send commands to console using exec
            CFG_PATH = Helper.getPathToCSGO() + @"\cfg\cheater.cfg";
            System.Diagnostics.Debug.WriteLine(CFG_PATH);

            if (!File.Exists(CFG_PATH))
            {
                File.Create(CFG_PATH);
            }

            CFG_INFO = new FileInfo(CFG_PATH);

            // Start timer that writes our commands to our cheater.cfg
            writeTimer = new System.Timers.Timer(100);
            writeTimer.Elapsed += fileWriter;
            writeTimer.AutoReset = true;
            writeTimer.Enabled = true;
        }

        private void setupUserConfigs()
        {

            string[] users = Directory.GetDirectories(Helper.getPathToSteam() + @"\userdata");
            foreach (string user in users)
            {
                // string userConfig = user + @"\730\local\cfg\config.cfg";
                string userConfig = user + @"\730\local\cfg\config.cfg";

                // Add keybinding to execute our console commands to the player default config
                if (File.Exists(userConfig))
                {

                    // Add our keybinding to player cfg
                    using (StreamWriter sw = File.AppendText(userConfig))
                    {
                        // Bind F9 in player config to exec our cheater.cfg file
                        sw.WriteLine("\rbind \"F9\" \"exec cheater.cfg\"");
                    }
                }
            }
        }

        public bool checkIfCfgIsReady()
        {
            bool isReady = true;

            string[] users = Directory.GetDirectories(Helper.getPathToSteam() + @"\userdata");
            foreach (string user in users)
            {
                string userConfig = user + @"\730\local\cfg\config.cfg";

                // Add keybinding to execute our console commands to the player default config
                if (File.Exists(userConfig))
                {

                    // Check if we already tempered with the config
                    using (StreamReader sr = new StreamReader(userConfig))
                    {
                        string contents = sr.ReadToEnd();
                        if (!contents.Contains("cheater.cfg"))
                        {
                            isReady = false;
                        }
                    }
                }
            }

            cfgIsReady = isReady;

            return isReady;
        }

        public void SendCommand(string Command)
        {
            CommandQueue.Add(Command);
        }

        private void fileWriter(Object source, ElapsedEventArgs e)
        {
            // Check if we have any commands queued and file is not being written to already
            if (Helper.IsFileLocked(CFG_INFO) || isWriting || CommandQueue.Count < 1)
            {
                return;
            }

            // Write our commands to cheater.cfg
            writeToCFG();
        }

        private void writeToCFG()
        {
            if (!GameProcess.IsValid) return;

            isWriting = true;

            string combinedCommands = "";
            foreach (string Command in CommandQueue.ToList())
            {
                if (!Command.EndsWith(";"))
                {
                    combinedCommands += Command + ";";
                }
                else
                {
                    combinedCommands += Command;
                }
                CommandQueue.Remove(Command);
            }

            try
            {
                // Write commands to our cheater.cfg file
                using (var sw = new StreamWriter(CFG_PATH, false))
                {
                    sw.WriteLine(combinedCommands);
                    sw.Close();
                }
                //User32.SetForegroundWindow(MemoryReader.GameWindow);

                // Trigger console exec by simulating keypress F9
                // User32.SendInput.KeyPress(KeyCode.F9);
                SendInput.KeyPress(KeyCode.F9);
            }
            catch (IOException e)
            {
                Console.WriteLine(e.Message);
            }

            isWriting = false;

        }
    }

}
