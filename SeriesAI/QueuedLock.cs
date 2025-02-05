﻿namespace SeriesAI
{
    using System.Threading;

    public sealed class QueuedLock
    {
        private readonly object innerLock = new object();
        private volatile int ticketsCount = 0;
        private volatile int ticketToRide = 1;

        public void Enter()
        {
            int myTicket = Interlocked.Increment(ref ticketsCount);
            Monitor.Enter(innerLock);
            while (true)
            {
                if (myTicket == ticketToRide)
                {
                    return;
                }
                else
                {
                    Monitor.Wait(innerLock);
                }
            }
        }

        public void Exit()
        {
            Interlocked.Increment(ref ticketToRide);
            Monitor.PulseAll(innerLock);
            Monitor.Exit(innerLock);
        }
    }
}
