using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using App42.Common;

namespace App42
{
    class EventQueue : IDisposable
    {
        private readonly int _tripId;
        private readonly Func<bool> _connectionStatusProvider;
        private readonly ConcurrentStack<Event> _events = new ConcurrentStack<Event>();
        private readonly Timer _timer;
        private bool _flushing = false;
        private readonly EventRepository _repository;

        public EventQueue(int tripId, Func<bool> connectionStatusProvider)
        {
            _tripId = tripId;
            _connectionStatusProvider = connectionStatusProvider;
            _repository = new EventRepository();
            _timer = new Timer(_ => Flush(), null, 0, Consts.FlushInterval);
        }

        public event Action OnBeforeFlush;
        public event Action OnFlush;

        private async void Flush()
        {
            if (_flushing)
            {
                return;
            }

            Logger.I(">EventQueue.Flush");

            try
            {
                OnBeforeFlush?.Invoke();
                if (!_connectionStatusProvider())
                {
                    Logger.I("EventQueue.Flush no connection");
                    return;
                }
                
                int popped;
                var evts = new Event[10];
                while ((popped = _events.TryPopRange(evts)) > 0)
                {
                    try
                    {
                        await _repository.PostEvents(_tripId, evts.Take(popped).ToArray());                        
                    }
                    catch
                    {
                        _events.PushRange(evts, 0, popped);
                        throw;
                    }
                    OnFlush?.Invoke();
                }                
            }
            catch(Exception e)
            {
                Logger.E(e);
            }
            finally 
            {
                _flushing = false;
                Logger.I("<EventQueue.Flush");
            }           
        }

        public void Add(Event evt) => _events.Push(evt);

        public void Dispose()
        {
            _timer.Dispose();
            Flush();
        }
    }
}