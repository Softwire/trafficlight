using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace trafficlight
{
  /// <summary>
  /// A class to poll JIRA build states
  /// </summary>
  class JiraPoller
  {
    /// <summary>
    /// See https://github.com/jenkinsci/jenkins/blob/master/core/src/main/java/hudson/model/BallColor.java
    /// </summary>
    public enum BuildState
    {
      red,
      red_anime,
      yellow,
      yellow_anime,
      blue, // (Jenkins refers to green as "blue" http://jenkins-ci.org/content/why-does-jenkins-have-blue-balls )
      blue_anime,
      grey,
      disabled
    }

    private const int PollIntervalMillis = 5000;
    private readonly WebClient _webClient = new WebClient();
    private readonly string _listViewUrl;
    private readonly Action<BuildState> _listener;

    /// <summary>
    /// Fetches the color of the selected builds.
    /// 
    /// The url should be a Jenkins XML API url like http://jenkins.zoo.lan/view/CustomerName/api/xml
    /// or http://jenkins.zoo.lan/job/JobName/api/xml
    /// 
    /// If the url encompasses more than one build, the 'worst' value will be used, which is the
    /// lowest value in the BuildState enum.
    /// </summary>
    public JiraPoller(string listViewUrl, Action<BuildState> listener)
    {
      _listViewUrl = listViewUrl;
      _listener = listener;
    }

    public void Poll()
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
          var colors = apiDoc.Descendants("color").Select(colorElt => (BuildState)Enum.Parse(typeof(BuildState), colorElt.Value));
          var minColor = colors.Min();
          Console.WriteLine("Color is {0}", minColor);
          _listener(minColor);
          Thread.Sleep(PollIntervalMillis);
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
  }
}
