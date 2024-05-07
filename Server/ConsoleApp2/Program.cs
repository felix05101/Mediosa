using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace HttpListenerDemo
{
    class Program
    {
        [DllImport("user32.dll")]
        public static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, UIntPtr dwExtraInfo);

        const int VK_MEDIA_PLAY_PAUSE = 0xB3;
        const int VK_MEDIA_PREV_TRACK = 0xB1;
        const int VK_MEDIA_NEXT_TRACK = 0xB0;
        const int VK_VOLUME_UP = 0xAF;
        const int VK_VOLUME_DOWN = 0xAE;

        static async Task Main(string[] args)
        {
            HttpListener listener = new HttpListener();
            listener.Prefixes.Add("http://+:54783/");
            listener.Start();
            Console.WriteLine("Listening for requests...");

            while (true)
            {
                HttpListenerContext context = await listener.GetContextAsync();
                HandleRequest(context);
            }
        }

        static void HandleRequest(HttpListenerContext context)
        {
            string path = context.Request.Url.AbsolutePath.ToLower();

            switch (path)
            {
                case "/function1":
                    SendKeyPress(VK_MEDIA_PLAY_PAUSE);
                    break;
                case "/function2":
                    SendKeyPress(VK_MEDIA_PREV_TRACK);
                    break;
                case "/function3":
                    SendKeyPress(VK_MEDIA_NEXT_TRACK);
                    break;
                case "/function4":
                    SendKeyPress(VK_VOLUME_UP);
                    break;
                case "/function5":
                    SendKeyPress(VK_VOLUME_DOWN);
                    break;
                default:
                    context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                    break;
            }

            context.Response.Close();
        }

        static void SendKeyPress(int virtualKeyCode)
        {
            const int KEYEVENTF_EXTENDEDKEY = 0x0001;
            const int KEYEVENTF_KEYUP = 0x0002;

            keybd_event((byte)virtualKeyCode, 0, KEYEVENTF_EXTENDEDKEY, UIntPtr.Zero);
            keybd_event((byte)virtualKeyCode, 0, KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP, UIntPtr.Zero);
        }
    }
}
