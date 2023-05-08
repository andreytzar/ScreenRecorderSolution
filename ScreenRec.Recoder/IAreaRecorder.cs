using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScreenRec.Recoder
{
    public interface IAreaRecorder:IDisposable
    {
        RecorderParam Param { get; set; }
        async Task RunScheduler(CancellationToken token = default) { }
        async Task Start() { }
        async Task Stop() { }
    }
}
