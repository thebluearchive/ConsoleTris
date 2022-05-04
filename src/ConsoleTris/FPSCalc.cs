using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleTris
{
    internal static class FPSCalc
    {
        private const long MAXSAMPLES = 100;
        private static long tickindex = 0;
        private static long ticksum = 0;
        private static long[] ticklist = new long[MAXSAMPLES];

        /* need to zero out the ticklist array before starting */
        /* average will ramp up until the buffer is full */
        /* returns average ticks per frame over the MAXSAMPLES last frames */
        internal static double CalcAverageTick(long newtick)
        {
            ticksum -= ticklist[tickindex];  /* subtract value falling off */
            ticksum += newtick;              /* add new value */
            ticklist[tickindex] = newtick;   /* save new value so it can be subtracted later */
            if (++tickindex == MAXSAMPLES)    /* inc buffer index */
                tickindex = 0;

            /* return average */
            return (double)ticksum / MAXSAMPLES;
        }
    }
}
