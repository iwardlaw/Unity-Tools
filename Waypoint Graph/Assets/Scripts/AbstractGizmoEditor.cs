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

using UnityEditor;

public class AbstractGizmoEditor : Editor {

  AbstractGizmo tgtScript;

  void OnEnable()
  {
    tgtScript = (AbstractGizmo)target;
  }

  public override void OnInspectorGUI()
  {
    DrawDefaultInspector();
  }
}
