using trafficlight.TrafficLightControls;

namespace trafficlight
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            ITrafficLightControl control = new Disco(new TrafficLightInterface());
            control.Activate();
        }
    }
}
