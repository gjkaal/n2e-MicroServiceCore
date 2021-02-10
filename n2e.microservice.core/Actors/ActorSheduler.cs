using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using n2e.MicroService.Core.Abstractions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace n2e.MicroService.Core
{
    public class ActorSheduler : IActorSheduler, IDisposable
    {
        private static int _actorId = 0;
        private readonly static object _actorIdLock = new object();
        private readonly ConcurrentDictionary<int, IActor> _actors = new ConcurrentDictionary<int, IActor>();
        private readonly List<int> _activeActors = new List<int>();
        private readonly static object _activeActorsLock = new object();
        private readonly IHostApplicationLifetime _applicationLifetime;
        private readonly ILogger<ActorSheduler> _logger;
        private readonly Timer _timer;
        private bool disposedValue;
        private Task _updateLoop;
        private bool _started;
        private bool _stopping;

        public ActorSheduler(ILogger<ActorSheduler> logger, IHostApplicationLifetime applicationLifetime)
        {
            _applicationLifetime = applicationLifetime;
            _logger = logger;
            _timer = new Timer(TimerElapsed);
        }

        private void TimerElapsed (object state)
        {
            _logger.LogInformation("Timer elapsed");
            var actors = _actors.GetEnumerator();
            while (actors.MoveNext())
            {
                var loopTime = DateTime.UtcNow;
                var actor = actors.Current.Value;
                if (actor != null && actor.IsActive)
                {
                    actor.TimerInterval(loopTime);
                }
            }
        }

        private static int NextId()
        {
            lock (_actorIdLock)
            {
                _actorId++;
                return _actorId;
            }
        }

        public Task Start(int timerInterval, CancellationToken cancellationToken)
        {
            _applicationLifetime.ApplicationStarted.Register(OnStarted);
            _applicationLifetime.ApplicationStopping.Register(OnStopping);
            _applicationLifetime.ApplicationStopped.Register(OnStopped);

            // Start timer with interval of 1s (1000ms)
            _ = _timer.Change(0, timerInterval);
            _updateLoop = UpdateLoop(cancellationToken);

            return Task.CompletedTask;
        }

        private Task UpdateLoop(CancellationToken cancellationToken)
        {            
            return Task.Run(() =>
            {
                _logger.LogInformation("UpdateLoop running");
                var time = DateTime.UtcNow;
                while (!cancellationToken.IsCancellationRequested && !_stopping)
                {
                    var updateTime = DateTime.UtcNow;
                    var loopTime = updateTime - time;
                    var actors = _actors.GetEnumerator();
                    while (actors.MoveNext())
                    {
                        var actorId = actors.Current.Key;
                        var actor = actors.Current.Value;
                        if (actor != null)
                        {
                            if (actor.IsActive)
                            {
                                actor.Update(loopTime);
                            }
                            else {
                                lock (_activeActorsLock) {
                                    if (_started && !_activeActors.Contains(actorId))
                                    {                                        
                                        _activeActors.Add(actorId);
                                        actor.Start();                                      
                                    }
                                }
                            }
                        }
                    }
                    time = updateTime;
                }
                _logger.LogInformation("UpdateLoop stopped");
            });
        }

        public async Task Stop(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stop stopped");
            _timer.Change(int.MaxValue, int.MaxValue);
            Parallel.For(0, _actors.Count, (x) =>
             {
                 var actor = _actors[x];
                 actor?.Stop();
             });            
        }

        private void OnStarted()
        {
            _logger.LogInformation("OnStarted has been called.");
            _started = true;

            // Perform post-startup activities here
        }

        private void OnStopping()
        {
            _logger.LogInformation("OnStopping has been called.");
            _stopping = true;
            // Perform on-stopping activities here
            _updateLoop.Wait();
        }

        private void OnStopped()
        {
            _logger.LogInformation("OnStopped has been called.");

            // Perform post-stopped activities here
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // Dispose managed objects here
                    _timer?.Dispose();
                    _updateLoop?.Dispose();
                    _actors.Clear();
                }
                // Dispose unmanaged objects here.

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        public int Register(IActor ts)
        {
            var id = NextId();
            return (_actors.TryAdd(id, ts)) ? id : -1;
        }
    }

}
