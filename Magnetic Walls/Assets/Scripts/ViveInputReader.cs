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

public class ViveInputReader : MonoBehaviour {

  #region Members

  SteamVR_TrackedObject trackObj;
  SteamVR_Controller.Device controller { get {return SteamVR_Controller.Input((int)trackObj.index);} }
  ulong touchpad = SteamVR_Controller.ButtonMask.Touchpad;
  ulong grip = SteamVR_Controller.ButtonMask.Grip;
  ulong trigger = SteamVR_Controller.ButtonMask.Trigger;
  public float horizontalInputThreshold = 0.3f, verticalInputThreshold = 0.2f;

  #endregion Members


  #region MonoBehaviour Callbacks

  void Awake()
  {
    trackObj = GetComponent<SteamVR_TrackedObject>();
  }

  #endregion MonoBehvaiour Callbacks


  #region Public Methods

  public float GetAxis(string axis)
  {
    switch(axis) {
      case "Horizontal":
        return GetTouchpadHorizontal();
      case "Vertical":
        return GetTouchpadVertical();
    }
    return 0f;
  }

  public float GetPressDown(string axis)
  {
    switch(axis) {
      case "Touchpad":
        return GetTouchpadPressDown();
      case "Trigger":
        return GetTriggerPressDown();
      case "Grip":
        return GetGripPressDown();
    }
    return 0f;
  }

  #endregion Public Methods


  #region Private Methods

  float GetTouchpadHorizontal()
  {
    if(trackObj != null && controller != null) {
      float input = controller.GetAxis().x;
      if(Mathf.Abs(input) >= horizontalInputThreshold) {
        Debug.Log("Vive Horizontal Axis: " + input);
        return input;
      }
    }
    return 0f;
  }

  float GetTouchpadVertical()
  {
    if(trackObj != null && controller != null) {
      float input = controller.GetAxis().y;
      if(Mathf.Abs(input) >= verticalInputThreshold) {
        Debug.Log("Vive Vertical Axis:  " + input);
        return input;
      }
    }
    return 0f;
  }

  float GetTouchpadPressDown()
  {
    if(trackObj != null && controller != null) {
      float input = controller.GetPressDown(touchpad) ? 1f : 0f;
      if(input != 0f) {
        Debug.Log("Vive Touchpad Press: " + input);
        return input;
      }
    }
    return 0f;
  }

  float GetTriggerPressDown()
  {
    if(trackObj != null && controller != null) {
      float input = controller.GetPressDown(trigger) ? 1f : 0f;
      if(input != 0f) {
        Debug.Log("Vive Trigger Press: " + input);
        controller.TriggerHapticPulse(1000);
        return input;
      }
    }
    return 0f;
  }

  float GetGripPressDown()
  {
    if(trackObj != null && controller != null) {
      float input = controller.GetPressDown(grip) ? 1f : 0f;
      if(input != 0f) {
        Debug.Log("Vive Grip Press: " + input);
        controller.TriggerHapticPulse(500);
        return input;
      }
    }
    return 0f;
  }

  #endregion Private Methods
}
