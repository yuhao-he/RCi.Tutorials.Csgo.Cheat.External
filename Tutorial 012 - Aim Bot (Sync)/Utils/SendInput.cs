using RCi.Tutorials.Csgo.Cheat.External.Sys;
using RCi.Tutorials.Csgo.Cheat.External.Sys.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace RCi.Tutorials.Csgo.Cheat.External.Utils
{
    public static class SendInput
    {
        public static void KeyDown(KeyCode key)
        {
            ushort scanCode = User32.MapVirtualKey(key, 0);

            var keyboardInput = new Input
            {
                type = SendInputEventType.InputKeyboard,
                ki = { wScan = scanCode, dwFlags = (uint)RCi.Tutorials.Csgo.Cheat.External.Sys.Data.KeyboardEventFlags.KEYEVENTF_UNICODE }
            };

            User32.SendInput(1, ref keyboardInput, Marshal.SizeOf<Input>());
        }

        public static void KeyUp(KeyCode key)
        {
            ushort scanCode = User32.MapVirtualKey(key, 0);

            var keyboardInput = new Input
            {
                type = SendInputEventType.InputKeyboard,
                ki = { wScan = scanCode, dwFlags = (uint)KeyboardEventFlags.KEYEVENTF_UNICODE | (uint)KeyboardEventFlags.KEYEVENTF_KEYUP }
            };
            User32.SendInput(1, ref keyboardInput, Marshal.SizeOf<Input>());
        }

        public static void KeyPress(KeyCode key)
        {
            KeyDown(key);
            KeyUp(key);
        }

        public static void MouseLeftDown()
        {
            var mouseMoveInput = new Input
            {
                type = SendInputEventType.InputMouse,
                mi = { dwFlags = MouseEventFlags.MOUSEEVENTF_LEFTDOWN }
            };
            User32.SendInput(1, ref mouseMoveInput, Marshal.SizeOf<Input>());
        }

        public static void MouseLeftUp()
        {
            var mouseMoveInput = new Input
            {
                type = SendInputEventType.InputMouse,
                mi = { dwFlags = MouseEventFlags.MOUSEEVENTF_LEFTUP }
            };
            User32.SendInput(1, ref mouseMoveInput, Marshal.SizeOf<Input>());
        }

        public static void MouseRightDown()
        {
            var mouseMoveInput = new Input
            {
                type = SendInputEventType.InputMouse,
                mi = { dwFlags = MouseEventFlags.MOUSEEVENTF_RIGHTDOWN }
            };
            User32.SendInput(1, ref mouseMoveInput, Marshal.SizeOf<Input>());
        }

        public static void MouseRightUp()
        {
            var mouseMoveInput = new Input
            {
                type = SendInputEventType.InputMouse,
                mi = { dwFlags = MouseEventFlags.MOUSEEVENTF_RIGHTUP }
            };
            User32.SendInput(1, ref mouseMoveInput, Marshal.SizeOf<Input>());
        }

        /*
        public static void MouseMove(int x, int y)
        {
            User32.mouse_event(Helper.MOUSEEVENTF_MOVE, x, y, 0, 0);
        }
        */

    }
}
