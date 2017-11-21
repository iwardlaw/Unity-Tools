using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class Utilities {

  // Based on the Microsoft .NET method:
  // <http://referencesource.microsoft.com/#mscorlib/system/string.cs,55e241b6143365ef>
  public static bool StrIsNullOrWhiteSpace(string s)
  {
    if(s == null) return true;

    for(int i = 0; i < s.Length; ++i)
      if(!char.IsWhiteSpace(s[i])) return false;

    return true;
  }

  public static void QuitApplication(string message = "", bool sayQuitting = true)
  {
    if(sayQuitting)
      message = "Halting application! " + message;
    if(!StrIsNullOrWhiteSpace(message))
      Debug.LogError(message);
#if UNITY_EDITOR
    EditorApplication.isPlaying = false;
#else
    Application.Quit();
#endif
  }
}
