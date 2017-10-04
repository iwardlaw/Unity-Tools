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

abstract public class AbstractGizmo : MonoBehaviour {

  #region Members

  Color _defaultGizmoColor = Color.white, _defaultSelectedColor = Color.blue, _defaultInactiveColor = Color.grey;
  public Color gizmoColor = Color.white, selectedColor = Color.blue, inactiveColor = Color.grey;
  protected bool _overrideDefaultColor = false;

  #endregion Members


  #region Public Accessors

  public Color defaultGizmoColor { get { return _defaultGizmoColor; } }
  public Color defaultSelectedColor { get { return _defaultSelectedColor; } }
  public Color defaultInactiveColor { get { return _defaultInactiveColor; } }
  public bool overrideDefaultColor { get { return _overrideDefaultColor; } set { _overrideDefaultColor = value; } }

  #endregion Public Accessors


  #region Abstract Callbacks

  abstract protected void OnDrawGizmos();
  abstract protected void OnDrawGizmosSelected();

  #endregion Abstract Callbacks
}
