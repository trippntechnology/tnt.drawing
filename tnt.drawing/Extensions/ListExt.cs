using System.Collections.Generic;
using System.Linq;

namespace TNT.Drawing.Extensions;

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
}
