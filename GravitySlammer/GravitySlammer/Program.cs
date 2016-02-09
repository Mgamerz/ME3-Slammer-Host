using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace GravitySlammer
{
    class Program
    {
        [DllImport("User32.dll")]
        static extern int SetForegroundWindow(IntPtr point);
        static int timeUpMs = 200;
        static int upGravity = 500; //newtons
        static int timeZeroMs = 2000;
        static int zeroGravity = 3; //upnewtons
        static int timeSlamMs = 1500;
        static int slamGravity = -3000; //newtons
        static int timeNormalGravity = 6500;
        static int normalGravityMin = 700;
        static int normalGravityMax = 1100;
        static int loopsTillSurprise = 4;

        static string[] guns = { "shuriken", "hornet", "ivory", "blackwidow", "widow", "revenant", "tempest", "minigun" };
        private static string execFile;

        static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.Error.WriteLine("This program takes 1 argument: Directory of Mass Effect 3.");
                endProgram(1);
            }
            string me3dir = args[0];
            if (!me3dir.EndsWith(@"\"))
            {
                me3dir += @"\";
            }

            string me3exe = me3dir + "binaries\\win32\\MassEffect3.exe";
            execFile = me3dir + "binaries\\slamming.txt";

            if (!File.Exists(me3exe))
            {
                Console.Error.WriteLine("This program takes 1 argument: Directory of Mass Effect 3.");
                endProgram(1);
            }

            Console.WriteLine("Type this into the console of Mass Effect 3:");
            Console.WriteLine("setbind O \"exec slamming.txt\"");
            Console.WriteLine();
            Console.WriteLine("Press any key to begin gravity loop");
            Console.ReadKey();

            int i = 5;
            while (i > 0)
            {
                Console.WriteLine("Slamming Gravity Loop starts in "+i+"s...");
                i--;
                Thread.Sleep(1000);
            }
            startGravityLoop();
        }

        private static void endProgram(int v)
        {
            debugPause();
            Environment.Exit(v);
        }

        [Conditional("DEBUG")]
        private static void debugPause()
        {
            Console.WriteLine("Press any key to continue");
            Console.ReadKey();
        }

        static void startGravityLoop()
        {
            var rnd = new Random();
            Process p;
            while (true)
            {
                loopsTillSurprise--;
                if (loopsTillSurprise <= 0)
                {
                    loopsTillSurprise = 4;
                    //Fling up
                    Console.WriteLine("How about a surprise?");
                    File.WriteAllText(execFile, "GiveItem " + guns[rnd.Next(guns.Count())] + " target");
                    SendKeys.SendWait("O");
                    Thread.Sleep(1000);
                }

                //Gravity loop starts with "up" and ends when the normal gravity period stops.
                //Every X of these loops will trigger a "gun surprise" where a target may receive a weapon.

                p = Process.GetProcessesByName("MassEffect3").FirstOrDefault();
                if (p != null)
                {
                    IntPtr h = p.MainWindowHandle;
                    SetForegroundWindow(h);
                }

                //Fling up
                Console.Write("...Gravity malfunction!");
                File.WriteAllText(execFile, "SetGravity " + upGravity);
                SendKeys.SendWait("O");
                Thread.Sleep(timeUpMs);

                //Zero G suspension
                Console.Write("...Zero G");

                File.WriteAllText(execFile, "SetGravity " + zeroGravity);
                SendKeys.SendWait("O");
                Thread.Sleep(timeZeroMs);

                p = Process.GetProcessesByName("MassEffect3").FirstOrDefault();
                if (p != null)
                {
                    IntPtr h = p.MainWindowHandle;
                    SetForegroundWindow(h);
                }
                //Slam
                //Zero G suspension
                Console.Write("...Slam!");
                File.WriteAllText(execFile, "SetGravity " + slamGravity);
                SendKeys.SendWait("O");
                Thread.Sleep(timeSlamMs);

                //Wait for a while
                Console.WriteLine("... and normalize.");
                File.WriteAllText(execFile, "SetGravity " + rnd.Next(normalGravityMin,normalGravityMax));
                SendKeys.SendWait("O");
                Thread.Sleep(timeNormalGravity);
            }
        }
    }
}
