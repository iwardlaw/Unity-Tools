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
using System.Collections.Generic;

[RequireComponent(typeof(GraphGizmo))]
public class WaypointGraph : MonoBehaviour {

  #region Members

  RAList<Pair<Waypoint, RAList<WaypointLink>>> _pointMap;
  public RAList<Pair<Waypoint, RAList<WaypointLink>>> pointMap { get { return _pointMap; } }
  public Waypoint startPoint, endPoint;
  public int startIndex { get { return GetIndex(startPoint); } }
  public int endIndex { get { return GetIndex(endPoint); } }
  public List<Waypoint> pointsInUse;

  #endregion Members


  #region MonoBehaviour Callbacks and Methods

  void Awake()
  {
    RAList<Waypoint> waypoints = new RAList<Waypoint>(GetComponentsInChildren<Waypoint>());
    RAList<WaypointLink> links = new RAList<WaypointLink>(GetComponentsInChildren<WaypointLink>());
    Debug.Log(waypoints.size + ", " + links.size);
    _pointMap = new RAList<Pair<Waypoint, RAList<WaypointLink>>>(waypoints.capacity);
    for(int i = 0; i < waypoints.size; ++i) {
      RAList<WaypointLink> temp = new RAList<WaypointLink>();
      for(int j = 0; j < links.size; ++j)
        if(links[j].fromPoint == waypoints[i] || (!links[j].directed && links[j].toPoint == waypoints[i]))
          temp.PushBack(links[j]);
      _pointMap.PushBack(new Pair<Waypoint, RAList<WaypointLink>>(waypoints[i], temp));
    }
    // DEBUG PRINTOUT //
    for(int i = 0; i < _pointMap.size; ++i)
      Debug.Log(_pointMap[i].first + " -> " + _pointMap[i].second);
  }

  public int GetIndex(Waypoint point)
  {
    for(int i = 0; i < _pointMap.size; ++i)
      if(_pointMap[i].first == point)
        return i;
    return 0;
  }

  #endregion MonoBehaviour Callbacks and Methods
}