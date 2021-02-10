using Microsoft.Extensions.Logging;
using n2e.MicroService.Core.Actor;
using System;

namespace n2e.MicroService.Core
{
    public class Testclass1 : ActorBase<Testclass1> {
        public Testclass1(ILogger<Testclass1> logger) : base(logger)
        {
        }

        public override void Update(TimeSpan timeSpan)
        {
            Logger.LogInformation($"Update Testclass1 : {timeSpan.TotalMilliseconds}ms");
            base.Update(timeSpan);
        }
    }

}
