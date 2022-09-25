namespace TimeMonitorInvisibleAdminConsole
{
    using Microsoft.Win32;
    using System;
    using System.Diagnostics;
    using System.Numerics;
    using System.Text;
    using System.Threading.Tasks;
    using static System.Environment;
    using static System.IO.File;
    using static System.Threading.Tasks.Task;

    internal class Program
    {
        public static Stopwatch swTimer = new Stopwatch();

        private static async Task Main()
        {
            swTimer.Start();

            await DetectPowerChange();

            await GetCurrentSessionTimes();
        }

        static async Task DetectPowerChange()
        {
            SystemEvents.PowerModeChanged += new PowerModeChangedEventHandler(OnPowerChangeAsync);
            await Delay(1000);
        }

        public static String GetElapsedTime(TimeSpan e)
        {
            String elapsed = "";
            if (e.TotalMilliseconds < 1000)
            {
                elapsed = e.Milliseconds + " ms";
                return elapsed;
            }
            if (e.TotalSeconds < 60 && e.TotalMilliseconds >= 1000)
            {
                elapsed = e.Seconds + " s     " + e.Milliseconds + " ms";
                return elapsed;
            }
            if (e.TotalMinutes < 60 && e.TotalSeconds >= 60)
            {
                elapsed = e.Minutes + " m     " + e.Seconds + " s     " + e.Milliseconds + " ms";
                return elapsed;
            }
            if (e.TotalHours < 24 && e.TotalMinutes >= 60)
            {
                elapsed = e.Hours + " h     " + e.Minutes + " m     " + e.Seconds + " s     " + e.Milliseconds + " ms";
                return elapsed;
            }
            if (e.TotalDays < 7 && e.TotalHours >= 24)
            {
                elapsed = e.Days + " d     " + e.Hours + " h     " + e.Minutes + " m     " + e.Seconds + " s     " + e.Milliseconds + " ms";
                return elapsed;
            }
            if (e.TotalDays >= 7)
            {
                elapsed = (e.Days / 7) + " w     " + e.Days + " d     " + e.Hours + " h     " + e.Minutes + " m     " + e.Seconds + " s     " + e.Milliseconds + " ms";
                return elapsed;
            }
            if (e.TotalDays >= 365)
            {
                elapsed = (e.TotalDays / 365) + " y     " + (e.Days / 7) + " w     " + e.Days + " d     " + e.Hours + " h     " + e.Minutes + " m     " + e.Seconds + " s     " + e.Milliseconds + " ms";
                return elapsed;
            }
            return elapsed;
        }

        public static async void OnPowerChangeAsync(Object s, PowerModeChangedEventArgs e)
        {
            StringBuilder sbPower = new StringBuilder();

            if (e.Mode == PowerModes.Resume)
            {
                sbPower.AppendLine($"Suspended for: {GetElapsedTime(swTimer.Elapsed)}");
                sbPower.AppendLine($"At: {DateTime.Now}");
                sbPower.AppendLine();
                sbPower.AppendLine();

                await Delay(1000);

                swTimer.Restart();
            }

            if (e.Mode == PowerModes.Suspend)
            {
                sbPower.AppendLine($"Resumed for: {GetElapsedTime(swTimer.Elapsed)}");
                sbPower.AppendLine($"At: {DateTime.Now}");
                sbPower.AppendLine();
                sbPower.AppendLine();

                await Delay(1000);

                swTimer.Restart();
            }

            AppendAllText($@"{GetLogicalDrives()[0]}UsageTimesLog.txt", sbPower.ToString());

            await Delay(1000);
        }

        public static async Task GetCurrentSessionTimes()
        {
            while (true)
            {
                StringBuilder sbTimer = new StringBuilder();

                BigInteger ns = (BigInteger)TickCount * 1000000;
                BigInteger ohnsu = (BigInteger)TickCount * 10000;
                BigInteger ms = TickCount;
                BigInteger s = (BigInteger)TickCount / 1000;
                BigInteger min = (BigInteger)TickCount / 60000;
                BigInteger h = (BigInteger)TickCount / 3600000;
                BigInteger d = (BigInteger)TickCount / 86400000;
                BigInteger w = (BigInteger)TickCount / 604800000;
                BigInteger f = (BigInteger)TickCount / 1209600000;
                BigInteger mon = (BigInteger)TickCount / 2629800000;
                BigInteger y = (BigInteger)TickCount / 31557600000;

                TimeSpan ts = new TimeSpan((Int64)((BigInteger)TickCount * 10000));

                sbTimer.AppendLine("Current session:");
                sbTimer.AppendLine();
                sbTimer.AppendLine(GetElapsedTime(swTimer.Elapsed));
                sbTimer.AppendLine();
                sbTimer.AppendLine();
                sbTimer.AppendLine("Time since last restart:");
                sbTimer.AppendLine();
                sbTimer.AppendLine(GetElapsedTime(ts));
                sbTimer.AppendLine();
                sbTimer.AppendLine(ns + "     nanoseconds");
                sbTimer.AppendLine(ohnsu + "     100-nanosecond units");
                sbTimer.AppendLine(ms + "     milliseconds");
                sbTimer.AppendLine(s + "     seconds");
                sbTimer.AppendLine(min + "     minutes");
                sbTimer.AppendLine(h + "     hours");
                sbTimer.AppendLine(d + "     days");
                sbTimer.AppendLine(w + "     weeks");
                sbTimer.AppendLine(f + "     fortnights");
                sbTimer.AppendLine(mon + "     months");
                sbTimer.AppendLine(y + "     years");
                sbTimer.AppendLine();
                sbTimer.AppendLine();

                WriteAllText($@"{GetLogicalDrives()[0]}TimerElapsed.txt", sbTimer.ToString());

                sbTimer.Clear();

                await Delay(10000);
            }
        }
    }
}
