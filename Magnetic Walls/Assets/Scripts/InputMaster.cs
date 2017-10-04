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

public class InputMaster : MonoBehaviour {

  #region Enums

  public enum InputType {INPUT_KEYBOARD, INPUT_VIVE};

  #endregion Enums


  #region Members

  public bool usingKeyboardInput = true;
  public bool usingViveInput = true;
  public InputType dominantInputSource;
  public ViveInputReader virLeft, virRight;

  #endregion Members


  #region Public Methods

  public float GetInput(string axis)
  {
    switch(axis) {
      case "Horizontal":
        return GetHorizontal();
      case "Vertical":
        return GetVertical();
      case "Interact":
        return GetInteract();
      case "Cancel":
        return GetCancel();
      case "PlayerReset":
        return GetPlayerReset();
    }
    return 0f;
  }

  #endregion Public Methods


  #region Private Methods

  float GetHorizontal()
  {
    float inputVal = 0f;
    switch(dominantInputSource) {
      case InputType.INPUT_KEYBOARD:
        if(usingKeyboardInput) {
          inputVal = GetKeyboardHorizontal();
          if(inputVal == 0f && usingViveInput)
            inputVal = GetViveHorizontal();
          return inputVal;
        }
        else if(usingViveInput)
          return GetViveHorizontal();
        return inputVal;
      case InputType.INPUT_VIVE:
        if(usingViveInput) {
          inputVal = GetViveHorizontal();
          if(inputVal == 0f && usingKeyboardInput)
            inputVal = GetKeyboardHorizontal();
          return inputVal;
        }
        else if(usingKeyboardInput)
          return GetKeyboardHorizontal();
        return inputVal;
    }
    return 0f;
  }

  float GetVertical()
  {
    float inputVal = 0f;
    switch(dominantInputSource) {
      case InputType.INPUT_KEYBOARD:
        if(usingKeyboardInput) {
          inputVal = GetKeyboardVertical();
          if(inputVal == 0f && usingViveInput)
            inputVal = GetViveVertical();
          return inputVal;
        }
        else if(usingViveInput)
          return GetViveVertical();
        return inputVal;
      case InputType.INPUT_VIVE:
        if(usingViveInput) {
          inputVal = GetViveVertical();
          if(inputVal == 0f && usingKeyboardInput)
            inputVal = GetKeyboardVertical();
          return inputVal;
        }
        else if(usingKeyboardInput)
          return GetKeyboardVertical();
        return inputVal;
    }
    return 0f;
  }

  float GetInteract()
  {
    float inputVal = 0f;
    switch(dominantInputSource) {
      case InputType.INPUT_KEYBOARD:
        if(usingKeyboardInput) {
          inputVal = GetKeyboardInteract();
          if(inputVal == 0f && usingViveInput)
            inputVal = GetViveInteract();
          return inputVal;
        }
        else if(usingViveInput)
          return GetViveInteract();
        return inputVal;
      case InputType.INPUT_VIVE:
        if(usingViveInput) {
          inputVal = GetViveInteract();
          if(inputVal == 0f && usingKeyboardInput)
            inputVal = GetKeyboardInteract();
          return inputVal;
        }
        else if(usingKeyboardInput)
          return GetKeyboardInteract();
        return inputVal;
    }
    return 0f;
  }

  float GetCancel()
  {
    float inputVal = 0f;
    switch(dominantInputSource) {
      case InputType.INPUT_KEYBOARD:
        if(usingKeyboardInput) {
          inputVal = GetKeyboardCancel();
          if(inputVal == 0f && usingViveInput)
            inputVal = GetViveCancel();
          return inputVal;
        }
        else if(usingViveInput)
          return GetViveCancel();
        return inputVal;
      case InputType.INPUT_VIVE:
        if(usingViveInput) {
          inputVal = GetViveCancel();
          if(inputVal == 0f && usingKeyboardInput)
            inputVal = GetKeyboardCancel();
          return inputVal;
        }
        else if(usingKeyboardInput)
          return GetKeyboardCancel();
        return inputVal;
    }
    return 0f;
  }

  float GetPlayerReset()
  {
    float inputVal = 0f;
    switch(dominantInputSource) {
      case InputType.INPUT_KEYBOARD:
        if(usingKeyboardInput) {
          inputVal = GetKeyboardPlayerReset();
          if(inputVal == 0f && usingViveInput)
            inputVal = GetVivePlayerReset();
          return inputVal;
        }
        else if(usingViveInput)
          return GetVivePlayerReset();
        return inputVal;
      case InputType.INPUT_VIVE:
        if(usingViveInput) {
          inputVal = GetVivePlayerReset();
          if(inputVal == 0f && usingKeyboardInput)
            inputVal = GetKeyboardPlayerReset();
          return inputVal;
        }
        else if(usingKeyboardInput)
          return GetKeyboardPlayerReset();
        return inputVal;
    }
    return 0f;
  }

  float GetKeyboardHorizontal() {return Input.GetAxisRaw("Horizontal");}

  float GetViveHorizontal() {return virLeft != null ? virLeft.GetAxis("Horizontal") : 0f;}

  float GetKeyboardVertical() {return Input.GetAxisRaw("Vertical");}

  float GetViveVertical() {return virLeft != null ? virLeft.GetAxis("Vertical") : 0f;}

  float GetKeyboardInteract() {return Input.GetKeyDown(KeyCode.Space) ? 1f : 0f;}

  float GetViveInteract() {return virLeft != null ? virLeft.GetPressDown("Trigger") : 0f;}

  float GetKeyboardCancel() {return Input.GetKeyDown(KeyCode.Escape) ? 1f : 0f;}

  float GetViveCancel() {return virLeft != null ? virLeft.GetPressDown("Grip") : 0f;}

  float GetKeyboardPlayerReset() {return Input.GetKeyDown(KeyCode.R) ? 1f : 0f;}

  float GetVivePlayerReset() {return virRight != null ? virRight.GetPressDown("Grip") : 0f;}

  #endregion Private Methods
}
