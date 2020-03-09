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
        //public sealed class IdGenerator
        //{
        //    private static readonly Lazy<IdGenerator> lazy = new Lazy<IdGenerator>(() => new IdGenerator());

        //    private int _id = 0;
        //    private IdGenerator() { }
        //    public static IdGenerator Instance => lazy.Value;
        public void Start()
        {

            //lets start with cartesian. 

            // lets assume the grid as 200/200, with our radar at center of it. 
            // we will generate coords within this range. 
            // new coordinates will be moving slowly marking 
            // the movement in objects they are depicting. 
            // movement needs to be in random direction.
            // need differing velocities for different objects.
            //( for now, will smoothen it with inertia in a particular direction later.)

           var rand = new Random(50);
            while (true)
            {
               var randomNumber = rand.NextDouble();

               if (randomNumber > 0.9)
                {
                    var obj = new TrackableObject();
                    obj.LocationUpdated += ObjectLocationUpdated;
                   // Console.WriteLine($"Object created with type {obj.Type.ToString()}, and Id = {obj.Id}");
                    _objects.TryAdd(obj.Id, obj);
                    Thread.Sleep(1000);
                }
            }

            //Console.ReadLine();
        }

        private void ObjectLocationUpdated(object sender, Coordinates e)
        {
            if (sender is TrackableObject obj)
            {
               // Console.WriteLine($"Object with Id: {obj.Id} got new Location : {e.X}, {e.Y}");
                RadarEvent?.Invoke(null, obj);
            }
        }
    }
}
