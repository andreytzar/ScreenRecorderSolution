using System.Drawing;
using System.Xml.Serialization;

namespace ScreenRec.Settings
{
    public static class Sett
    {
        private static string file=$"{Environment.CurrentDirectory}/sett.xml";
        public static Settings Setting {get;set;}
        public static async Task Load()
        {
            try
            {
                XmlSerializer xml=new XmlSerializer(typeof(Settings));
                using var stream=new FileStream(file, FileMode.OpenOrCreate, FileAccess.Read);
                Setting = xml.Deserialize(stream) as Settings;
                if (Setting == null) Setting = new ();
                CheckMonitorsNum();
            }
            catch { Setting = new();
                try
                {
                    CheckMonitorsNum();
                } catch { }
            }
        }
        public static async Task Save()
        {
            try
            {
                XmlSerializer xml = new XmlSerializer(typeof(Settings));
                using var stream = new FileStream(file, FileMode.Truncate);
                xml.Serialize(stream, Setting);
            }
            catch { }
        }

        public static void CheckMonitorsNum()
        {
            var monitors = Screen.AllScreens;
            for (int i = 0; monitors.Length > i; i++)
            {
                if (i >= Setting.Monitors.Count)
                {
                    Setting.Monitors.Add(new MonitorNum { Index = i });
                }
                Setting.Monitors[i].Location = monitors[i].Bounds.Location;
            }
        }
    }

    public record Settings
    {
        public int RecordDurationMin { get; set; } = 30;
        public int Fps { get; set; } = 20;
        public int MaxGigFolderSize { get; set; } = 20;
        public string VideoFolder { get { return _videofolder; } set 
            {
                _videofolder = value; 
                CheckFolder(_videofolder);
            } }
        string _videofolder = $"{Environment.CurrentDirectory}\\Video";
        public bool WriteSound { get; set; }=false;
        public List<MonitorNum> Monitors { get; set; } = new();
        public string VideoString { get; set; } = "vcodec=h264";
        public string AudioString { get; set; } = ",acodec=mp4a,ab=128,channels=1,samplerate=44100";
        public void CheckFolder(string folder)
        {
            try
            {
                if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);   
            }
            catch { }
        }
       
    }

    public record MonitorNum
    {
        public int Index { get; set; } = 0;
        public Point Location { get; set; } = new Point();
        public List<Rectangle> Rectangles { get; set; } = new();
        public string GetString {
            get { return $"Monitor #{Index + 1} Зон записи {Rectangles.Count}"; } }
        public override string ToString()
        {
            return $"Monitor #{Index+1} Зон записи {Rectangles.Count}";
        }
    }
}