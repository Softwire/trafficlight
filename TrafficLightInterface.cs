using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace trafficlight
{
  class TrafficLightInterface : RelayBoardInterface
  {
    public void SetRYG(bool red, bool yellow, bool green)
    {
      // Due to the way the relays are wired, a "1" bit is off
      byte pins = 7;
      if (red)
      {
        pins ^= 4;
      }
      if (yellow)
      {
        pins ^= 2;
      }
      if (green)
      {
        pins ^= 1;
      }
      SetPins(pins);
    }
  }
}
