using System.Collections.Generic;
using System.Linq;
using TNT.Drawing.Objects;

namespace TNT.Drawing.Extensions;

/// <summary>
/// Extension methods
/// </summary>
public static class ListExt
{
  /// <summary>
  /// Determines if <paramref name="element"/> is the first item in <paramref name="list"/>.
  /// </summary>
  /// <typeparam name="T">Type of the elements in the list</typeparam>
  /// <param name="list">The list to check</param>
  /// <param name="element">The element to check</param>
  /// <returns>True if the element is the first item in the list; otherwise, false</returns>
  public static bool IsFirst<T>(this List<T> list, T element)
  {
    if (list.Count == 0)
      return false;
    return list.IndexOf(element) == 0;
  }

  /// <summary>
  /// Determines if <paramref name="element"/> is the last item in <paramref name="list"/>.
  /// </summary>
  /// <typeparam name="T">Type of the elements in the list</typeparam>
  /// <param name="list">The list to check</param>
  /// <param name="element">The element to check</param>
  /// <returns>True if the element is the last item in the list; otherwise, false</returns>
  public static bool IsLast<T>(this List<T> list, T element)
  {
    if (list.Count == 0)
      return false;
    return list.IndexOf(element) == list.Count - 1;
  }

  /// <summary>
  /// Finds the elements in <paramref name="list"/> adjacent to <paramref name="element"/>
  /// </summary>
  /// <returns>The elements in <paramref name="list"/> adjacent to <paramref name="element"/></returns>
  public static List<T> AdjacentTo<T>(this List<T> list, T element)
  {
    var elementIndex = list.IndexOf(element);
    return list.Where((_, index) => index == elementIndex - 1 || index == elementIndex + 1).ToList();
  }

  /// <summary>
  /// Adds <paramref name="value"/> to <paramref name="list"/> if <paramref name="value"/> is not null.
  /// This method ensures null values are not added to the list, maintaining its integrity.
  /// </summary>
  public static void AddNotNull<T>(this List<T> list, T? value)
  {
    if (value != null) list.Add(value);
  }

  /// <summary>
  /// Determines if <paramref name="element"/> is either the first or last item in <paramref name="list"/>.
  /// </summary>
  /// <typeparam name="T">Type of the elements in the list</typeparam>
  /// <param name="list">The list to check</param>
  /// <param name="element">The element to check</param>
  /// <returns>True if the element is the first or last item in the list; otherwise, false</returns>
  public static bool IsFirstOrLast<T>(this List<T> list, T element)
  {
    if (list.Count == 0)
      return false;

    var elementIndex = list.IndexOf(element);
    if (elementIndex == -1)
      return false;

    return elementIndex == 0 || elementIndex == list.Count - 1;
  }

  /// <summary>
  /// Finds the coincident <see cref="Vertex"/> at the opposite end of the list (first or last),
  /// but only if the provided <paramref name="point"/> is itself the first or last in the list.
  /// Returns <c>null</c> if the input vertex is not first or last, or if no coincidence is found.
  /// Coincidence is determined using <see cref="Vertex.IsNear(Vertex, int)"/> with the specified threshold.
  /// </summary>
  /// <param name="points">The list of vertices to search.</param>
  /// <param name="point">The vertex to compare against.</param>
  /// <param name="threshold">The distance threshold for coincidence (defaults to 8).</param>
  /// <returns>The coincident <see cref="Vertex"/> at the opposite end, or <c>null</c> if not found or not first/last.</returns>
  public static CanvasPoint? FindCoincident(this List<CanvasPoint> points, CanvasPoint point, int threshold = 8)
  {
    if (points.Count < 3) return null;

    if (points.IsFirst(point))
    {
      //var last = points.LastOrDefault();
      var last = points.Last();
      if (last.IsNear(point, threshold)) return last;
    }
    else if (points.IsLast(point))
    {
      //var first = points.FirstOrDefault();
      var first = points.First();
      if (first.IsNear(point, threshold)) return first;
    }
    return null;
  }
}