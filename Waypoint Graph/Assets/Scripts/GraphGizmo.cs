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

[RequireComponent(typeof(Waypoint))]
public class GraphGizmo : AbstractGizmo {

  #region Enums

  public enum ColorOverrideType { None, IsFromPoint, IsToPoint, UndirLinkSelected };

  #endregion Enums
  

  #region Members

  public Color waypointFromColor = Color.red, waypointToColor = Color.green;
  public float gizmoSize = 0.2f;
  ColorOverrideType _cot = ColorOverrideType.None;
  public ColorOverrideType colorOverrideType { get { return _cot; } set { _cot = value; } }
  List<ColorOverrideType> _cotQueue = new List<ColorOverrideType>();

  #endregion Members


  #region Editor Callbacks

  protected override void OnDrawGizmos()
  {
    Color c = GetComponent<Waypoint>().active ? gizmoColor : inactiveColor;
    _cot = _cotQueue.Count != 0 ? _cotQueue[0] : ColorOverrideType.None;
    switch(_cot) {
      case ColorOverrideType.IsFromPoint:
        c = waypointFromColor;
        break;
      case ColorOverrideType.IsToPoint:
        c = waypointToColor;
        break;
      case ColorOverrideType.UndirLinkSelected:
        c = selectedColor;
        break;
    }
    Gizmos.color = c;
    Gizmos.DrawSphere(transform.position, gizmoSize);
  }

  protected override void OnDrawGizmosSelected()
  {
    Gizmos.color = selectedColor;
    Gizmos.DrawSphere(transform.position, gizmoSize);
  }

  #endregion Editor Callbacks


  #region Public Methods

  public void lockColorOverride(ColorOverrideType cot)
  {
    _cotQueue.Add(cot);
  }

  public void unlockColorOverride()
  {
    if(_cotQueue.Count != 0)
      _cotQueue.RemoveAt(0);
  }

  #endregion Public Methods
}
