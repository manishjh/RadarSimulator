﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace RadarSimulator
{
    public class FixedSizedQueue<T>
    {
        public ConcurrentQueue<T> q = new ConcurrentQueue<T>();

        private object lockObject = new object();

        public int Limit { get; set; }

        public void Enqueue(T obj)
        {          
            q.Enqueue(obj);
            lock (lockObject)
            {
                T overflow;
                while (q.Count > Limit && q.TryDequeue(out overflow)) ;
            }
        }
    }
}