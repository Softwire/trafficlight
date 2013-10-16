using System;
using System.Threading;

namespace trafficlight.TrafficLightControls
{
    /// <summary>
    /// Make those lights dance!
    /// </summary>
    internal class Disco : ITrafficLightControl
    {
        private readonly TrafficLightInterface _trafficLight;

        public Disco(TrafficLightInterface trafficLight)
        {
            _trafficLight = trafficLight;
        }

        public void Activate()
        {
            var random = new Random();

            while (true)
            {
                _trafficLight.SetPins((byte)random.Next());
                Thread.Sleep(200);
            }
        }
    }
}