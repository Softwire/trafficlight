using System;
using System.Threading;

namespace trafficlight
{
  internal class Program
  {
    private TrafficLightInterface _trafficLight;

    public static void Main(string[] args)
    {
      new Program().Disco();
    }

    public Program()
    {
      _trafficLight = new TrafficLightInterface();
    }

    public void Disco()
    {
      var random = new Random();
      while (true)
      {
        _trafficLight.SetPins((byte)random.Next());
        Thread.Sleep(200);
      }
    }

    public void PollJira()
    {
      var poller = new JiraPoller("http://jenkins.zoo.lan/view/CustomerX/api/xml", OnBuildStateFetched);
      poller.Poll();
    }

    private void OnBuildStateFetched(JiraPoller.BuildState state)
    {
      switch (state)
      {
        case JiraPoller.BuildState.red:
          _trafficLight.SetRYG(true, false, false);
          break;
        case JiraPoller.BuildState.red_anime:
          _trafficLight.SetRYG(true, true, false);
          break;
        case JiraPoller.BuildState.yellow:
          _trafficLight.SetRYG(false, true, false);
          break;
        case JiraPoller.BuildState.yellow_anime:
          _trafficLight.SetRYG(true, true, false);
          break;
        case JiraPoller.BuildState.blue:
          _trafficLight.SetRYG(false, false, true);
          break;
        case JiraPoller.BuildState.blue_anime:
          _trafficLight.SetRYG(false, true, true);
          break;
        case JiraPoller.BuildState.grey:
          _trafficLight.SetRYG(false, false, false);
          break;
        default:
          throw new Exception("Unrecognised state: " + state);
      }
    }
  }
}
