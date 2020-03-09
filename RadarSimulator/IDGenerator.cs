using System;
using System.Threading;

namespace RadarSimulator
{
    public sealed class IdGenerator
    {
        private static readonly Lazy<IdGenerator> lazy = new Lazy<IdGenerator>(()=> new IdGenerator());

        private int _id = 0;
        private IdGenerator() { }
        public static IdGenerator Instance => lazy.Value;

        public int GetId()
        {
          _id = Interlocked.Increment(ref _id);
           return _id;
        }
    }
}
