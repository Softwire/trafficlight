using System;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using System.Xml.Linq;

namespace trafficlight.TrafficLightControls
{
    /// <summary>
    /// Poll Jenkins build states
    /// </summary>
    internal class JenkinsPoller : ITrafficLightControl
    {
        /// <summary>
        /// See https://github.com/jenkinsci/jenkins/blob/master/core/src/main/java/hudson/model/BallColor.java
        /// </summary>
        private enum BuildState
        {
            red,
            red_anime,
            yellow,
            yellow_anime,
            blue, // Jenkins refers to green as "blue" http://jenkins-ci.org/content/why-does-jenkins-have-blue-balls
            blue_anime,
            grey,
            disabled
        }

        private const int PollIntervalInMilliseconds = 5000;
        private readonly WebClient _webClient = new WebClient();
        private readonly string _listViewUrl = ConfigurationManager.AppSettings["jenkinsUrl"];
        private readonly TrafficLightInterface _trafficLight;

        public JenkinsPoller(TrafficLightInterface trafficLight)
        {
            _trafficLight = trafficLight;
        }

        /// <summary>
        /// Fetches the colour of the selected builds and displays it on the lights.
        /// 
        /// If the supplied URL encompasses more than one build, the 'worst' value will be used, which is the
        /// lowest value in the BuildState enum.
        /// </summary>
        public void Activate()
        {
            var consecutiveErrorCount = 0;

            while (true)
            {
                try
                {
                    var sw = Stopwatch.StartNew();
                    var apiData = _webClient.DownloadString(_listViewUrl);
                    Console.WriteLine("Fetched {0} ({1} bytes in {2} ms)", _listViewUrl, apiData.Length, sw.ElapsedMilliseconds);

                    var apiDoc = XDocument.Parse(apiData);
                    var colours = apiDoc.Descendants("color").Select(c => (BuildState)Enum.Parse(typeof(BuildState), c.Value)).ToList();
                    var minColour = colours.Min();
                    Console.WriteLine("Monitoring {0} builds", colours.Count());
                    Console.WriteLine("Colour is {0}", minColour);

                    OnBuildStateFetched(minColour);
                    
                    Thread.Sleep(PollIntervalInMilliseconds);
                    
                    consecutiveErrorCount = 0;
                }
                catch (Exception e)
                {
                    if (++consecutiveErrorCount > 10)
                    {
                        throw new Exception("Too many consecutive errors: " + consecutiveErrorCount, e);
                    }
                }
            }
        }

        private void OnBuildStateFetched(BuildState state)
        {
            switch (state)
            {
                case BuildState.red:
                    _trafficLight.SetRed();
                    break;
                case BuildState.red_anime:
                    _trafficLight.SetRedAndYellow();
                    break;
                case BuildState.yellow:
                    _trafficLight.SetYellow();
                    break;
                case BuildState.yellow_anime:
                    _trafficLight.SetRedAndYellow();
                    break;
                case BuildState.blue:
                    _trafficLight.SetGreen();
                    break;
                case BuildState.blue_anime:
                    _trafficLight.SetYellowAndGreen();
                    break;
                case BuildState.grey:
                case BuildState.disabled:
                    _trafficLight.SetOff();
                    break;
                default:
                    throw new Exception("Unrecognised state: " + state);
            }
        }
    }
}