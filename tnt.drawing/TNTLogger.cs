using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace TNT.Drawing;

/// <summary>
/// Provides simple logging utilities for debugging and performance measurement within the TNT.Drawing library.
/// </summary>
public static class TNTLogger
{
  /// <summary>
  /// Logs an informational message to the debug output, including the calling method and file name.
  /// </summary>
  /// <param name="msg">The message to log. Optional.</param>
  /// <param name="callingMethod">The name of the calling method. Automatically supplied by the compiler.</param>
  /// <param name="filePath">The file path of the calling code. Automatically supplied by the compiler.</param>
  public static void Info(string msg = "", [CallerMemberName] string callingMethod = "", [CallerFilePath] string filePath = "")
  {
    var fileName = Path.GetFileNameWithoutExtension(filePath);
    Debug.WriteLine($"{DateTime.Now:HH:mm:ss.fff} [{fileName}:{callingMethod}] {msg}");
  }

  /// <summary>
  /// Measures the execution time of an action in milliseconds and logs the result to the debug output.
  /// </summary>
  /// <param name="action">The action to measure.</param>
  /// <param name="msg">An optional message to include in the log output.</param>
  /// <param name="callingMethod">The name of the calling method. Automatically supplied by the compiler.</param>
  /// <param name="filePath">The file path of the calling code. Automatically supplied by the compiler.</param>
  public static void MeasureTimeMillis(Action action, string msg = "", [CallerMemberName] string callingMethod = "", [CallerFilePath] string filePath = "")
  {
    var fileName = Path.GetFileNameWithoutExtension(filePath);
    var sw = Stopwatch.StartNew();
    action();
    sw.Stop();

    var sb = new StringBuilder($"[{sw.ElapsedMilliseconds} ms]");
    if (!String.IsNullOrEmpty(msg)) sb.Append($" {msg}");

    Debug.WriteLine($"{DateTime.Now:HH:mm:ss.fff} [{fileName}:{callingMethod}] {sb}");
  }
}
