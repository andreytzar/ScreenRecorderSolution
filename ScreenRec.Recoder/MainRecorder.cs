using LibVLCSharp.Shared;

namespace ScreenRec.Recoder
{
    public class MainRecorder : IDisposable
    {
        private LibVLC libvlc;
        public List<AreaRecorder> Recorders { get; set; }
        CancellationToken _token;

        public MainRecorder()
        {
            Core.Initialize();
            libvlc = new LibVLC();
            Recorders = new();
           // ClenVideoFolder();
        }
      
        public async Task ClenVideoFolder(CancellationToken token = default)
        {
            var noerrors = true;
            while (noerrors && !token.IsCancellationRequested)
            {
                try
                {
                    var info = new DirectoryInfo(Settings.Sett.Setting.VideoFolder);
                    if (info.Exists)
                    {
                        long fsize = 0;
                        var fi = info.GetFiles();
                        foreach (var f in fi)
                        {
                            fsize=fsize+f.Length;
                        }
                        if (fsize > Settings.Sett.Setting.MaxGigFolderSize * 1024*1024*1024)
                        {
                            var file=fi.OrderBy(x=>x.CreationTime).FirstOrDefault();
                            if (file != null) file.Delete();
                        }
                    }
                }
                catch { }
                await Task.Delay(new TimeSpan(0,1,0));
            } 
        }

        public async Task Start(CancellationToken token =default)
        {
            _token = token;
            var sett = Settings.Sett.Setting;
            await Stop();
            Recorders = new();
            foreach (var monitor in sett.Monitors) 
            {
                if (token.IsCancellationRequested) break;
                int i = 1;
                foreach (var rect in monitor.Rectangles)
                {
                    if (token.IsCancellationRequested) break;
                    var param = new RecorderParam(sett, monitor, rect, i);
                    var rec = new AreaRecorder(libvlc, param);
                    Recorders.Add(rec);
                    rec.RunScheduler(token);
                    i++;
                }
            }
        }

        public async Task Stop()
        {
            foreach (var rec in Recorders)
            {
                rec.Dispose();
            }
            Recorders.Clear();
        }

        public async void Dispose()
        {
            await Stop();
            libvlc?.Dispose();
        }
    }
}