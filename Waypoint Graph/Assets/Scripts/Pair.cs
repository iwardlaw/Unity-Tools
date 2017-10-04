/*
   Isaac Wardlaw
   ---------------
   Dr. Carolina Cruz-Neira
   IFSC 5399-01
   Virtual Reality Fundamentals
   University of Arkansas at Little Rock
   Fall 2016
   =======================================
*/

public class Pair<T, Y>: System.IEquatable<Pair<T, Y>> where T : System.IEquatable<T> where Y : System.IEquatable<Y> {
  T _first;
  Y _second;
  public T first { get { return _first; } set { _first = value; } }
  public Y second { get { return _second; } set { _second = value; } }
  public Pair(T firstItem, Y secondItem) { _first = firstItem; _second = secondItem; }
  public bool Equals(Pair<T, Y> other) { if((object)other == null) return false; return _first.Equals(other._first) && _second.Equals(other._second); }
  public static bool operator == (Pair<T, Y> a, Pair<T, Y> b) { return a.Equals(b); }
  public static bool operator != (Pair<T, Y> a, Pair<T, Y> b) { return !(a == b);}
}
