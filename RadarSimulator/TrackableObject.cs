using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RadarSimulator
{
   public class TrackableObject
    {
        private Coordinates _location;
        public int Id { get;}
        public ObjectTypeEnum Type { get;}
        public Coordinates Location => _location;

        private Random _random;

        public EventHandler<Coordinates> LocationUpdated;

        private double velocity = 0;

        

        public TrackableObject()
        {
            Id = IdGenerator.Instance.GetId();

            Type = ObjectTypeEnum.Generic;

            _random = new Random(Id);

            _location = new Coordinates(_random.NextDouble(), _random.NextDouble());

            Task.Run(async () => await UpdateLocation());
        }

        public async Task UpdateLocation()
        {
            while(true)
            {
                await Task.Delay(10);

                velocity = 0.03;

                var x = _location.X + _random.Next(-1, 2) * (velocity);
                var y = _location.Y + _random.Next(-1, 2) * (velocity);

                _location = new Coordinates(x, y);

                LocationUpdated?.Invoke(this, Location);
            }
        }

    }
}
