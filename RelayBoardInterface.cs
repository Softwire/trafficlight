using System;
using System.IO;
using System.Runtime.InteropServices;

namespace trafficlight
{
  /// <summary>
  /// FTD2XX.DLL can be downloaded from 
  /// http://www.sainsmart.com/zen/documents/20-018-909/4%20channel%20USB%2012V%20relay%20document/USB8RelayManager.rar
  /// It seems to be already present on my Windows 7 machine.
  /// </summary>
  class RelayBoardInterface : IDisposable
  {
    // Based in part on http://stackoverflow.com/a/5957504/8261

    [DllImport("FTD2XX.DLL", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
    private static extern int FT_Open(short intDeviceNumber, ref int lngHandle);

    [DllImport("FTD2XX.DLL", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
    private static extern int FT_Close(int lngHandle);

    [DllImport("FTD2XX.DLL", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
    private static extern int FT_Write(int lngHandle, string lpszBuffer, int lngBufferSize, ref int lngBytesWritten);

    [DllImport("FTD2XX.DLL", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
    private static extern int FT_ResetDevice(int lngHandle);

    [DllImport("FTD2XX.DLL", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
    private static extern int FT_SetBitMode(int lngHandle, byte ucMask, byte ucEnable);

    // FTDI Constants
    private const short FT_OK = 0;
    private const short FT_INVALID_HANDLE = 1;
    private const short FT_DEVICE_NOT_FOUND = 2;
    private const short FT_DEVICE_NOT_OPENED = 3;
    private const short FT_IO_ERROR = 4;

    private const short FT_INSUFFICIENT_RESOURCES = 5;
    // Word Lengths
    private const byte FT_BITS_8 = 8;
    // Stop Bits
    private const byte FT_STOP_BITS_2 = 2;
    // Parity
    private const byte FT_PARITY_NONE = 0;
    // Flow Control
    private const byte FT_FLOW_NONE = 0x0;
    // Purge rx and tx buffers
    private const byte FT_PURGE_RX = 1;

    private const byte FT_PURGE_TX = 2;

    private int handle = 0;

    public RelayBoardInterface()
    {
      if (FT_Open(0, ref handle) != FT_OK)
      {
        throw new IOException("Could not open FTTD device #0");
      }
      if (FT_ResetDevice(handle) != FT_OK)
      {
        throw new IOException("Failed To Reset Device!");
      }
      if (FT_SetBitMode(handle, 0xff, 0xff) != FT_OK)
      {
        throw new IOException("FT_SetBitMode Failed");
      }
    }

    public void Dispose()
    {
      FT_Close(handle);
    }

    public void SetPins(byte pins)
    {
      int writtenCount = 0;
      if (FT_Write(handle, "" + (char)pins, 1, ref writtenCount) != FT_OK)
      {
        throw new IOException("FT_Write Failed");
      }
    }
  }
}
