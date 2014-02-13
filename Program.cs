using System;
using trafficlight.TrafficLightControls;

namespace trafficlight
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            DateTime? timeOfLastException = null;

            while (true)
            {
                try
                {
                    ITrafficLightControl control = new Disco(new TrafficLightInterface());
                    control.Activate();
                }
                catch (Exception e)
                {
                    /* 
                     * Sometimes the connection to the traffic light interface is interrupted, e.g. by power cycling. 
                     * By catching the exception here, we can try to recreate the TrafficLightInterface and continue,
                     * rather than having to terminate the program. However, if there's another issue which will cause
                     * exceptions to be continually thrown, then we should give up and terminate. 
                     */
                    var currentDateTime = DateTime.UtcNow;

                    if (timeOfLastException.HasValue && (currentDateTime - timeOfLastException.Value).TotalMinutes < 1)
                    {
                        throw e;
                    }

                    timeOfLastException = currentDateTime;
                }
            }
        }
    }
}
