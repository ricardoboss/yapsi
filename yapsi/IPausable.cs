using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace yapsi
{
    public interface IPausable
    {
        public delegate void PausedEventHandler(IPausable sender);
        public delegate void ResumedEventHandler(IPausable sender);

        event PausedEventHandler? Paused;
        event ResumedEventHandler? Resumed;

        bool IsPaused { get; }

        void Pause();

        void Resume();
    }
}
