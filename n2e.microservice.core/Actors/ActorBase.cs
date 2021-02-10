using Microsoft.Extensions.Logging;
using n2e.MicroService.Core.Abstractions;
using System;

namespace n2e.MicroService.Core.Actor
{
    public abstract class ActorBase<T> : IActor
    {
        public bool IsActive { get; private set; }
        protected ILogger<T> Logger { get; }
        protected ActorBase(ILogger<T> logger)
        {
            Logger = logger;
        }

        public virtual void Start()
        {
            IsActive = true;
        }

        public virtual void Stop()
        {
            IsActive = false;
        }

        public virtual void TimerInterval(DateTime dateTime)
        {
            // Intentionally left empty
        }

        public virtual void Update(TimeSpan timeSpan)
        {
            // Intentionally left empty
        }
    }

}
