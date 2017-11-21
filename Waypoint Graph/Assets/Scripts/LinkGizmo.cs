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

[RequireComponent(typeof(WaypointLink))]
public class LinkGizmo : AbstractGizmo {

  #region Public Members

  public Color directedColor = Color.yellow, inactiveDirectedColor = Color.black;
  bool previouslySelected = false;

  #endregion Public Members


  #region Editor Callbacks

  protected override void OnDrawGizmos()
  {
    WaypointLink wl = GetComponent<WaypointLink>();
    if(previouslySelected) {
      wl.fromPoint.GetComponent<GraphGizmo>().dequeueColorOverride();
      wl.toPoint.GetComponent<GraphGizmo>().dequeueColorOverride();
      previouslySelected = false;
    }

    Gizmos.color = wl.active ? (wl.directed ? directedColor : gizmoColor) : (wl.directed ? inactiveDirectedColor : inactiveColor);
    Gizmos.DrawLine(wl.fromPoint.transform.position, wl.toPoint.transform.position);
  }

  protected override void OnDrawGizmosSelected()
  {
    WaypointLink wl = GetComponent<WaypointLink>();
    if(wl.directed) {
      wl.fromPoint.GetComponent<GraphGizmo>().enqueueColorOverride(ColorOverrideType.IsFromPoint);
      wl.toPoint.GetComponent<GraphGizmo>().enqueueColorOverride(ColorOverrideType.IsToPoint);
    }
    else {
      wl.fromPoint.GetComponent<GraphGizmo>().enqueueColorOverride(ColorOverrideType.UndirLinkSelected);
      wl.toPoint.GetComponent<GraphGizmo>().enqueueColorOverride(ColorOverrideType.UndirLinkSelected);
    }

    Gizmos.color = selectedColor;
    Gizmos.DrawLine(wl.fromPoint.transform.position, wl.toPoint.transform.position);

    previouslySelected = true;
  }

  #endregion Editor Callbacks
}
