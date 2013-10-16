using System.Threading;

namespace trafficlight.TrafficLightControls
{
    /// <summary>
    /// Standard British traffic lights sequence
    /// </summary>
    internal class RoadSequence : ITrafficLightControl
    {
        private readonly TrafficLightInterface _trafficLight;

        public RoadSequence(TrafficLightInterface trafficLight)
        {
            _trafficLight = trafficLight;
        }

        public void Activate()
        {
            var stage = 0;

            while (true)
            {
                switch (stage)
                {
                    case 0:
                        _trafficLight.SetRed();
                        LongWait();
                        break;
                    case 1:
                        _trafficLight.SetRedAndYellow();
                        ShortWait();
                        break;
                    case 2:
                        _trafficLight.SetGreen();
                        LongWait();
                        break;
                    case 3:
                        _trafficLight.SetYellow();
                        stage = -1;
                        ShortWait();
                        break;
                }

                stage++;
            }
        }

        private void LongWait()
        {
            Thread.Sleep(10 * 1000);
        }

        private void ShortWait()
        {
            Thread.Sleep(3 * 1000);
        }
    }
}