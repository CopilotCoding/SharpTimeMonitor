namespace TimeMonitorSharp
{
    using System;
    using System.Text;
    using System.Diagnostics;
    using System.Threading.Tasks;
    using static System.Environment;
    using static System.IO.File;
    using static System.Threading.Tasks.Task;
    using Microsoft.Win32;

    class Program
    {
        static Boolean running = true;

        static Stopwatch swRunTime;
        static Stopwatch swTimer;

        static String drive;

        static async Task Main()
        {
            await StartUp();
            await Run();
        }

        static async Task StartUp()
        {
            await StartTimers();
            await GetMainDrive();
            await GetOSSuspendStatus();
        }

        static async Task StartTimers()
        {
            swRunTime = new Stopwatch();
            swTimer = new Stopwatch();

            swRunTime.Start();
            swTimer.Start();

            await Delay(100);
        }

        static async Task GetMainDrive()
        {
            drive = GetLogicalDrives()[0];
            await Delay(100);
        }

        static async Task GetOSSuspendStatus()
        {
            SystemEvents.PowerModeChanged += new PowerModeChangedEventHandler(OnPowerChangeAsync);

            await Delay(100);
        }

        static async void OnPowerChangeAsync(Object s, PowerModeChangedEventArgs e)
        {
            StringBuilder sbPower = new StringBuilder();

            if (e.Mode == PowerModes.Resume)
            {
                /// If you wanted you could create some bat files for viewing the text files this program creates.
                // Process.Start("ViewTheTextFile.bat");

                sbPower.AppendLine($"OS time suspended: {GetElapsedTime(swTimer.Elapsed)}");
                sbPower.AppendLine();
                sbPower.AppendLine($"OS resumed date: {DateTime.Now}");
                sbPower.AppendLine();
                sbPower.AppendLine();
                sbPower.AppendLine();

                swTimer.Restart();
            }

            if (e.Mode == PowerModes.Suspend)
            {
                sbPower.AppendLine($"OS session time: {GetElapsedTime(swTimer.Elapsed)}");
                sbPower.AppendLine();
                sbPower.AppendLine($"OS suspended date: {DateTime.Now}");
                sbPower.AppendLine();
                sbPower.AppendLine();
                sbPower.AppendLine();

                swTimer.Restart();
            }

            String usageTextFile = $@"{drive}UsageTimesLog.txt";

            AppendAllText(usageTextFile, sbPower.ToString());

            await Delay(1000);
        }

        static async Task Run()
        {
            while (running)
            {
                await BuildTimeString();
            }
        }

        static async Task BuildTimeString()
        {
            StringBuilder sbTimer = new StringBuilder();

            sbTimer.AppendLine($"Current session: {GetElapsedTime(swTimer.Elapsed)}");
            sbTimer.AppendLine();
            sbTimer.AppendLine($"TimeMonitor run time: {GetElapsedTime(swRunTime.Elapsed)}");
            sbTimer.AppendLine();

            Int32 seconds = TickCount / 1000;
            Int32 minutes = seconds / 60;
            Int32 hours = minutes / 60;
            Int32 days = hours / 24;
            Int32 weeks = days / 7;

            sbTimer.AppendLine($"OS time since last restart: in weeks: {weeks}, in days: {days}, in hours: {hours}, in minutes {minutes}.");
            sbTimer.AppendLine();

            String timerElapsedFile = $@"{drive}TimerElapsed.txt";

            WriteAllText(timerElapsedFile, sbTimer.ToString());

            sbTimer.Clear();

            await Delay(10000);
        }

        static String GetElapsedTime(TimeSpan e)
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

            if (e.TotalDays < 73 && e.TotalHours >= 24)
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
    }
}
