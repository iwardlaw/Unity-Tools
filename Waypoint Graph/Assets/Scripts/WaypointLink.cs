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

using UnityEngine;

public class WaypointLink : MonoBehaviour, System.IEquatable<WaypointLink> {

  #region Members

  public Waypoint fromPoint, toPoint;
  public float weight = 1f;
  public bool active = true;
  public bool directed = false;
  [SerializeField] float _moveSpeedMultiplier = 1f;
  public float moveSpeedMultiplier { get { return _moveSpeedMultiplier; } set { _moveSpeedMultiplier = value; _movementFactor = value / _length; } }
  // Not yet implemented, though it may be at a later time:
  //public float toleranceWidth = 0f;
  int _occupantCount = 0;
  public int occupantCount { get { return _occupantCount; } }
  float _length;
  public float length { get { return _length; } }
  float _movementFactor;
  public float movementFactor { get { return _movementFactor; } }

  #endregion Members


  #region Constructor and MonoBehaviour Callbacks

  public WaypointLink(Waypoint from, Waypoint to) { fromPoint = from; toPoint = to; }
  void Awake() {
    _length = Vector3.Distance(fromPoint.transform.position, toPoint.transform.position);
    _movementFactor = moveSpeedMultiplier / _length;
  }

  #endregion Constructor and MonoBehaviour Callbacks


  #region Public Methods

  public bool Equals(WaypointLink other) {
    if((object)other == null) return false;
    return (fromPoint.GetHashCode() == other.fromPoint.GetHashCode() && toPoint.GetHashCode() == other.toPoint.GetHashCode()) ||
      directed ? false : (fromPoint.GetHashCode() == other.toPoint.GetHashCode() && toPoint.GetHashCode() == other.fromPoint.GetHashCode());
  }
  public void AddOccupant() { ++_occupantCount; }
  public void RemoveOccupant() { --_occupantCount; if(_occupantCount < 0) _occupantCount = 0; }
  public bool Occupied() { return _occupantCount > 0; }
  public float RecalculateLength() { _length = Vector3.Distance(fromPoint.transform.position, toPoint.transform.position); return _length; }
  public Waypoint PointOpposite(Waypoint a) { if(a == toPoint) return fromPoint; if(a == fromPoint) return toPoint; return null; }

  #endregion Public Methods


  #region Operators

  public static bool operator == (WaypointLink a, WaypointLink b) { return a.Equals(b); }
  public static bool operator != (WaypointLink a, WaypointLink b) { return !(a == b); }

  #endregion Operators
}