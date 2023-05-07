using LibVLCSharp.Shared;
using ScreenRec.Settings;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScreenRec.Recoder
{
    public class AreaRecorder : IDisposable
    {
        public bool RunTask { get; set; } = true;
        public RecorderParam Param { get; set; }
        private MediaPlayer mediaPlayer;
        private Media media;
        private LibVLC libvlc;
        public DateTime TimeStart { get; private set; }
        public DateTime TimeEnd { get; private set; }
        public TimeSpan TimePassed { get { return DateTime.Now.Subtract(TimeStart); } }
        public TimeSpan TimeLeft { get { return TimeEnd.Subtract(DateTime.Now); } }
        public string FileName { get { return _file; } }
        private string _file;

        public AreaRecorder(LibVLC _libvlc, RecorderParam param)
        {
            libvlc = _libvlc;
            Param = param;
        }
        public async Task RunScheduler(CancellationToken token = default)
        {
            RunTask = true;
            await Start();
            do
            {
                await Task.Delay(500);
                if (TimeLeft.TotalMinutes < 0)
                {
                    await Start();
                }
            } while (RunTask && !token.IsCancellationRequested);
        }

        public async Task Start()
        {
            try
            {
                Stop();
                TimeStart = DateTime.Now;
                TimeEnd = TimeStart.AddMinutes(Param.RecordDurationMin);
                GetFileName();
                if (mediaPlayer == null) mediaPlayer = new MediaPlayer(libvlc);
                media = new Media(libvlc, "screen://", FromType.FromLocation);
                media.AddOption($":screen-fps={Param.Fps}");
                if (!Param.WriteSound) media.AddOption(":no-audio");
                string par =
                ($":sout=#transcode{{{Param.VideoString}" +
                    $"{(Param.WriteSound ? Param.AudioString : "")}}}:file{{dst={_file}}}");
                media.AddOption(par);
               
                media.AddOption($":screen-caching=1000");
                int w = Param.Rectangle.Width / 2;
                w = w * 2;
                media.AddOption($":screen-width={w}");
                int h = Param.Rectangle.Height / 2;
                h = h * 2;
                media.AddOption($":screen-height={h}");
                int y =( Param.Location.Y+Param.Rectangle.Y )/ 2;
                y= y * 2;
                int x = (Param.Location.X + Param.Rectangle.X) / 2;
                x=x* 2;
                if (y>0) media.AddOption($":screen-top={y}");
                if (x>0) media.AddOption($":screen-left={x}");
                media.AddOption(":sout-keep");
                mediaPlayer.Play(media);
            }
            catch
            { RunTask = false; media?.Dispose(); media = null; }
        }

        private string GetFileName()
        {
            _file = $"{Param.VideoFolder}\\Mon_{Param.Monitor}_Zone_{Param.Zone}_{TimeStart.Year}-{TimeStart.Month}-" +
                $"{TimeStart.Day}-{TimeStart.Hour}-{TimeStart.Minute}-{TimeStart.Second}.mp4";
            return _file;
        }

        public async Task Stop()
        {
            mediaPlayer?.Stop();
            media?.Dispose();
            media = null;
        }

        public async void Dispose()
        {
            RunTask = false;
            await Stop();
            media?.Dispose();
            mediaPlayer?.Dispose();
            await Task.Delay(200);
        }

        public override string ToString()
        {
            return $"Mon {Param.Monitor} Area {Param.Zone} {(RunTask?"Record":"Stopped")} {TimePassed.Hours}:{TimePassed.Minutes}:{TimePassed.Seconds} {Path.GetFileName(_file)}";
        }
    }

    public class RecorderParam
    {
        public int Monitor { get; set; }
        public int Zone { get; set; }
        public int RecordDurationMin { get; set; } = 30;
        public int Fps { get; set; } = 20;
        public string VideoFolder { get; set; }
        public bool WriteSound { get; set; } = false;
        public string VideoString { get; set; } = "vcodec=h264";
        public string AudioString { get; set; } = "acodec = mp4a,ab=128,channels=1,samplerate=44100";
        public Point Location { get; set; } = new Point();
        public Rectangle Rectangle { get; set; } = new();

        public RecorderParam(int monitor, int zone, int recordDurationMin, int fps, string videoFolder, bool writeSound, string videoString, string audioString, Point location, Rectangle rectangle)
        {
            Monitor = monitor;
            Zone = zone;
            RecordDurationMin = recordDurationMin;
            Fps = fps;
            VideoFolder = videoFolder;
            WriteSound = writeSound;
            VideoString = videoString;
            AudioString = audioString;
            Location = location;
            Rectangle = rectangle;
        }

        public RecorderParam(Settings.Settings settings, MonitorNum monitor, Rectangle rectangle, int zone)
        {
            Monitor = monitor.Index + 1;
            Zone = zone;
            RecordDurationMin = settings.RecordDurationMin;
            Fps = settings.Fps;
            VideoFolder = settings.VideoFolder;
            WriteSound = settings.WriteSound;
            VideoString = settings.VideoString;
            AudioString = settings.AudioString;
            Location = monitor.Location;
            Rectangle = rectangle;
        }
    }
}
