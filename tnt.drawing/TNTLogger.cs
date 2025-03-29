using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace TNT.Drawing;

public static class TNTLogger
{
  public static void I(string msg = "", [CallerMemberName] string callingMethod = "") => Debug.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} [{callingMethod}] {msg}");

}
