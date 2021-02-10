using Microsoft.Extensions.Logging;
using n2e.MicroService.Core.Actor;
using System;

namespace n2e.MicroService.Core
{
    public class Testclass2 : ActorBase<Testclass2>
    {
        public Testclass2(ILogger<Testclass2> logger) : base(logger)
        {
        }

        public override void TimerInterval(DateTime dateTime)
        {
            Logger.LogInformation($"Interval Testclass2 : {dateTime}");
            base.TimerInterval(dateTime);
        }

        public override void Update(TimeSpan timeSpan)
        {
            Logger.LogInformation($"Update Testclass2 : {timeSpan.TotalMilliseconds}ms");
            base.Update(timeSpan);
        }
    }

}
