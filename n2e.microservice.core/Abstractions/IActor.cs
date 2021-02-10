using System;

namespace n2e.MicroService.Core.Abstractions
{
    public interface IActor
    {
        bool IsActive { get; }
        void Start();
        void Stop();
        void TimerInterval(DateTime dateTime);
        void Update(TimeSpan timeSpan);
    }

}
