using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
/// <summary>
/// A bunch of usefull methods
///</summary>
public class H
{
  public static string Print2DArray(int[][] array2D)
  {
    string str = "";
    int nRows = array2D.Length;
    int nCols = array2D[0].Length;

    for (int r = 0; r < nRows; r++)
    {
      for (int c = 0; c < nCols; c++)
        str += array2D[r][c].ToString();
      str += "\n";
    }
    return str;
  }
  public static int[][] MirrorAlongYAxis(int[][] array2D)
  {
    int nRows = array2D.Length;

    array2D = RotateRight(array2D);
    for (int r = 0; r < nRows; r++)
      Array.Reverse(array2D[r]);
    array2D = RotateLeft(array2D);

    return array2D;
  }
  public static int[][] MirrorAlongXAxis(int[][] array2D)
  {
    int nRows = array2D.Length;
    for (int r = 0; r < nRows; r++)
      Array.Reverse(array2D[r]);

    return array2D;
  }
  public static int[][] RotateRight(int[][] array2D)
  {

    array2D = Transpose(array2D);

    int nRows = array2D.Length;
    for (int r = 0; r < nRows; r++)
      Array.Reverse(array2D[r]);

    return array2D;
  }
  public static int[][] RotateLeft(int[][] array2D)
  {
    int nRows = array2D.Length;
    for (int r = 0; r < nRows; r++)
      Array.Reverse(array2D[r]);

    array2D = Transpose(array2D);

    return array2D;
  }
  public static int[][] Transpose(int[][] array2D)
  {
    int nRows = array2D.Length;
    int nCols = array2D[0].Length;

    int[][] transposedArray2D = new int[nRows][];
    for (int c = 0; c < nCols; c++)
      transposedArray2D[c] = new int[nCols];

    for (int r = 0; r < nRows; r++)
      for (int c = 0; c < nCols; c++)
        transposedArray2D[r][c] = array2D[c][r];

    return transposedArray2D;
  }
  public static IEnumerator MoveTo(Transform objectToMove, Vector3 destination, float speed)
  {
    Vector3 start = objectToMove.localPosition;
    float step = (speed / (start - destination).magnitude) * Time.fixedDeltaTime;
    float t = 0;
    while (t <= 1.0f)
    {
      t += step; // Goes from 0 to 1, incrementing by step each time
      objectToMove.localPosition = Vector3.Lerp(start, destination, t); // Move objectToMove closer to b
      yield return new WaitForFixedUpdate();         // Leave the routine and return here in the next frame
    }
    objectToMove.localPosition = destination;
  }


  public static void SetOpacity(GameObject go, float opacity)
  {
    SpriteRenderer sr = go.GetComponent<SpriteRenderer>();
    Color tmp = sr.color;
    tmp.a = opacity;
    sr.color = tmp;
  }
  public static void ClearArray<T>(T[] array) { Array.Clear(array, 0, array.Length); }
  public static IEnumerator SkipRestOfFrame() { yield return new WaitForEndOfFrame(); }
  public static bool HasNestedList(object objs)
  {
    if (objs is IList)
      foreach (object obj in ((IList)objs))
        return obj is IList;
    return false;
  }
  /// <summary>
  /// Checks how deep a possible list is nested. &#10;
  /// Example:
  /// If inception is 1, then the list's type is "List &lt; T &gt;"
  /// If inception is 3, then the list's type is "List &lt; List &lt; List &lt; T &gt; &gt; &gt;" 
  /// </summary>
  public static int NumberOfNestedLists(object objs, int nNestedLists = 0)
  {
    if (objs is IList)
    {
      foreach (object obj in (IList)objs)
      {
        nNestedLists++;
        nNestedLists = NumberOfNestedLists(obj, nNestedLists);
        return nNestedLists;
      }
    }
    return nNestedLists;
  }
  public static Func<T, bool> Not<T>(Func<T, bool> predicate) { return x => !predicate(x); }
  public static Func<T, bool> Or<T>(params Func<T, bool>[] predicates)
  {
    return delegate (T item)
    {
      foreach (Func<T, bool> predicate in predicates)
        if (predicate(item))
          return true;
      return false;
    };
  }
  public static Func<T, bool> And<T>(params Func<T, bool>[] predicates)
  {
    return delegate (T item)
    {
      foreach (Func<T, bool> predicate in predicates)
        if (!predicate(item))
          return false;
      return true;
    };
  }
}
