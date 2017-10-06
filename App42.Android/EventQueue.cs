using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace App42
{
    class EventQueue : IDisposable
    {
        private readonly int _tripId;
        private readonly ConcurrentStack<Event> _events = new ConcurrentStack<Event>();
        private readonly Timer _timer;
        private bool _flushing = false;

        public EventQueue(int tripId)
        {
            _tripId = tripId;
            _timer = new Timer(_ => Flush(), null, 0, Consts.FlushInterval);
        }

        public event Action OnFlush;

        private async void Flush()
        {
            if (_flushing)
            {
                return;
            }

            Logger.I(">EventQueue.Flush");
            _flushing = true;

            var repository = new EventRepository();
            try
            {
                int popped;
                var evts = new Event[10];
                while ((popped = _events.TryPopRange(evts)) > 0)
                {
                    while (true)
                    {
                        try
                        {
                            await repository.PostEvents(_tripId, evts.Take(popped).ToArray());
                            break;
                        }
                        catch
                        {
                            Thread.Sleep(20000);
                        }
                    }
                                      
                }
                OnFlush?.Invoke();
            }
            finally
            {
                _flushing = false;
            }
            Logger.I("<EventQueue.Flush");
        }

        public void Add(Event evt) => _events.Push(evt);

        public void Dispose() => _timer.Dispose();
    }
}