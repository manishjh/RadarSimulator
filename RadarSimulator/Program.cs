using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace RadarSimulator
{
    public sealed class RadarSimulator
    {
        private static readonly Lazy<RadarSimulator> lazy = new Lazy<RadarSimulator>(() => new RadarSimulator());
        private RadarSimulator() { }
        public static RadarSimulator Instance => lazy.Value;

        private ConcurrentDictionary<int, TrackableObject> _objects = new System.Collections.Concurrent.ConcurrentDictionary<int, TrackableObject>();

        public EventHandler<TrackableObject> RadarEvent;

        public void Start()
        {

            //lets start with cartesian. 

            var rand = new Random(50);
            int i = 0;
            while (i<100)
            {
                var randomNumber = rand.NextDouble();

                if (randomNumber > 0.9)
                {
                   
                    var obj = new TrackableObject();
                    obj.LocationUpdated += ObjectLocationUpdated;
                    _objects.TryAdd(obj.Id, obj);
                    i++;
                    Thread.Sleep(100);
                }
            }
        }

        private void ObjectLocationUpdated(object sender, Coordinates e)
        {
            if (sender is TrackableObject obj)
            {
                RadarEvent?.Invoke(null, obj);
            }
        }
    }
}
