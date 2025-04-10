using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;

namespace TNT.Drawing;

public static class TNTLogger
{
  public static void Info(string msg = "", [CallerMemberName] string callingMethod = "", [CallerFilePath] string filePath = "")
  {
    var fileName= Path.GetFileNameWithoutExtension(filePath);
    Debug.WriteLine($"{DateTime.Now:HH:mm:ss.fff} [{fileName}:{callingMethod}] {msg}");
  }
}
