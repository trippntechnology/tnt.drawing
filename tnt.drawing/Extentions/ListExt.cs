using System.Collections.Generic;
using System.Linq;

namespace TNT.Drawing.Extentions;

/// <summary>
/// Extension methods
/// </summary>
public static class ListExt
{
  /// <summary>
  /// Finds the elements in <paramref name="list"/> adjacent to <paramref name="element"/>
  /// </summary>
  /// <returns>The elements in <paramref name="list"/> adjacent to <paramref name="element"/></returns>
  public static List<T> AdjacentTo<T>(this List<T> list, T element)
  {
    var elementIndex = list.IndexOf(element);
    return list.Where((_, index) => index == elementIndex - 1 || index == elementIndex + 1).ToList();
  }
}
