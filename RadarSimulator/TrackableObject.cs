using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadarSimulator
{
    public class TrackableObject
    {
        private Coordinates _location;
        public int Id { get; }
        public ObjectTypeEnum Type { get; }
        public Coordinates Location => _location;

        private Random _random;

        public EventHandler<Coordinates> LocationUpdated;

        private float velocity = 0;

        private float A, B, C, D, E, F;

        private bool Xmode = true;

        private bool xpositive = true;
        private bool ypositive = true;

        public TrackableObject()
        {
            Type = ObjectTypeEnum.Generic;
            Id = IdGenerator.Instance.GetId();
            _random = new Random(Id);

            A = _random.Next(-5, 6);
            B = _random.Next(-5, 6);
            C = _random.Next(-5, 6);
            D = _random.Next(-5, 6);
            E = _random.Next(-5, 6);
            F = _random.Next(-5, 6);


            float x = (float)(_random.NextDouble() * (_random.Next(-2, 1) + 1));

            var points = IntersectConicAndLine(A, B, C, D, E, F, new PointF(x, 0), new PointF(x, 1));


            var velArray = new List<float> { -1, 1 };
            velocity = (float)(velArray[(_random.Next(0, 2))] * 0.02);
            

            xpositive = velocity > 0;
            ypositive = velocity > 0;
            
            //if(Id==78)
            if (points.Count > 0 && !float.IsNaN(points[0].X) && !float.IsNaN(points[0].Y) && !float.IsInfinity(points[0].X) && !float.IsInfinity(points[0].Y))
            {

                _location = new Coordinates(points[0].X, points[0].Y);
                Console.WriteLine($"Id :{Id} , x: {_location.X}, y: {_location.Y}, A :{A}, B:{B}, C:{C}, D:{D}, E:{E}, F:{F}, velocity : {velocity}");
                Task.Run(async () => await UpdateLocation());
            }

        }

        public async Task UpdateLocation()
        {
            /*
             Each Track able object needs to move in a conic curve. Using the general equation of a curve.
            Ax^2 + Bxy + Cy^2 + Dx + Ey + F = 0

            we need to move x ever so slightly to get new y values.
            To get Y values, we would use Intercept calculations.
            http://csharphelper.com/blog/2014/11/see-where-a-line-intersects-a-conic-section-in-c/?unapproved=465406&moderation-hash=14cbfb5f55c46e05e221a0a4a96c4609#comment-465406
           */

            while (true)
            {
                await Task.Delay(100);
                try
                {
                    if (Xmode)
                    {
                        var points = IntersectConicAndLine(A, B, C, D, E, F, new PointF(_location.X + (float)(velocity), 0), new PointF(_location.X + (float)(velocity), 1));
                        
                        if (points.Count > 0 )
                        {
                            if (float.IsNaN(points[0].X) || float.IsNaN(points[0].Y) || float.IsInfinity(points[0].X) || float.IsInfinity(points[0].Y))
                            {
                                FireClearingEvent();
                                break;
                            }

                            Console.WriteLine($"Id :{Id} , x: {_location.X}, y: {_location.Y}, X mode");
                            var point = GetPointInRightDirection(points, Xmode);
                            xpositive = point.X > _location.X;
                            ypositive = point.Y > _location.Y;
                            _location = new Coordinates(point.X, point.Y);
                            if (Math.Abs(point.X) > 5 || Math.Abs(point.Y) > 5)
                            {
                                FireClearingEvent();
                                break;
                            }
                            LocationUpdated?.Invoke(this, Location);
                        }
                        else
                        {
                            Xmode = false;
                            Console.WriteLine($"Id :{Id} , x: {_location.X}, y: {_location.Y}, starting Y mode");
                            velocity = (float)((ypositive ? 1 : -1) * 0.1);
                        }
                    }
                    else
                    {
                        var points = IntersectConicAndLine(A, B, C, D, E, F, new PointF(0, _location.Y + (float)(velocity)), new PointF(1, _location.Y + (float)(velocity)));

                        if (points.Count > 0 )
                        {
                            if (float.IsNaN(points[0].X) || float.IsNaN(points[0].Y) || float.IsInfinity(points[0].X) || float.IsInfinity(points[0].Y))
                            {
                                FireClearingEvent();
                                break;
                            }
                            Console.WriteLine($"Id :{Id} , x: {_location.X}, y: {_location.Y}, Y mode");
                            var point = GetPointInRightDirection(points, Xmode);
                            xpositive = point.X > _location.X;
                            ypositive = point.Y > _location.Y;
                            _location = new Coordinates(point.X, point.Y);
                            if (Math.Abs(point.X) > 5 || Math.Abs(point.Y) > 5)
                            {
                                FireClearingEvent();
                                break;
                            }
                            LocationUpdated?.Invoke(this, Location);
                        }
                        else
                        {
                            Xmode = true;
                            Console.WriteLine($"Id :{Id} , x: {_location.X}, y: {_location.Y}, starting X mode");
                            velocity = (float)((xpositive ? 1 : -1) * 0.1);
                        }
                    }
                }
                catch(Exception ex)
                {
                    Console.WriteLine($"Error : Id :{Id} , x: {_location.X}, y: {_location.Y},",ex);

                }
            }
        }



        private void FireClearingEvent()
        {
            _location = new Coordinates(float.NaN, float.NaN);
            LocationUpdated?.Invoke(this, Location);
           
        }

        private PointF GetPointInRightDirection(List<PointF> points, bool xmode)
        {

            return points.First(p => Math.Abs(p.Y - _location.Y) + Math.Abs(p.X - _location.X) == points.Min(o => Math.Abs(o.Y - _location.Y) + Math.Abs(o.X - _location.X)));

        }

        // Find the points of intersection between
        // a conic section and a line.
        private List<PointF> IntersectConicAndLine(
            float A, float B, float C, float D, float E, float F,
            PointF pt1, PointF pt2)
        {
            // Get dx and dy;
            float x1 = pt1.X;
            float y1 = pt1.Y;
            float x2 = pt2.X;
            float y2 = pt2.Y;
            float dx = x2 - x1;
            float dy = y2 - y1;

            // Calculate the coefficients for the quadratic formula.
            float a = A * dx * dx + B * dx * dy + C * dy * dy;
            float b = A * 2 * x1 * dx + B * x1 * dy + B * y1 * dx +
                C * 2 * y1 * dy + D * dx + E * dy;
            float c = A * x1 * x1 + B * x1 * y1 + C * y1 * y1 +
                D * x1 + E * y1 + F;

            // Check the determinant to see how many solutions there are.
            List<PointF> solutions = new List<PointF>();
            float det = b * b - 4 * a * c;
            if (det == 0)
            {
                float t = -b / (2 * a);
                solutions.Add(new PointF(x1 + t * dx, y1 + t * dy));
            }
            else if (det > 0)
            {
                float root = (float)Math.Sqrt(b * b - 4 * a * c);
                float t1 = (-b + root) / (2 * a);
                solutions.Add(new PointF(x1 + t1 * dx, y1 + t1 * dy));
                float t2 = (-b - root) / (2 * a);
                solutions.Add(new PointF(x1 + t2 * dx, y1 + t2 * dy));
            }

            return solutions;
        }

    }
}
