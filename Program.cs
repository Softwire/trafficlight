using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace trafficlight
{
  internal class Program
  {
    public static void Main(string[] args)
    {
      var ifc = new RelayBoardInterface();
      
      for (int i = 0; i < 1000; i++)
      {
        ifc.SetPins((byte)i);
        Thread.Sleep(300);
      }
    }
  }
}
