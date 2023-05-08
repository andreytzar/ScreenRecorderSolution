using ScreenRec.Settings;
using System.Drawing;


namespace ScreenRec.Recoder
{
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
