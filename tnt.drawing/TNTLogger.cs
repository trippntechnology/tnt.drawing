using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace TNT.Drawing;

public static class TNTLogger
{
  public static void Log(string msg = "", [CallerMemberName] string callingMethod = "") => Debug.WriteLine($"{DateTime.Now} [{callingMethod}] {msg}");
}
