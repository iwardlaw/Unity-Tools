using UnityEditor;

public class GraphTraverseEditor : Editor {

  GraphTraverse tgtScript;

  void OnEnable()
  {
    tgtScript = (GraphTraverse)target;
  }

  public override void OnInspectorGUI()
  {
    // TODO: Display 'startPoint', 'endPoint', 'setPath', and related members only when
    // 'overrideGraphPath' is selected.
    DrawDefaultInspector();
  }
}
