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

public class ViveCameraRigController : MonoBehaviour {

  #region Members

  public GameObject player;
  public GameObject scriptMaster;
  InputMaster inputMaster;
  Vector3 transformOffset;
  public GameObject cameraHead;
  public GameObject hmdTracker;
  SteamVR_TrackedObject trackObj;
  SteamVR_Controller.Device hmd { get {return SteamVR_Controller.Input((int)trackObj.index);} }

  #endregion Members


  #region MonoBehaviour Callbacks

  void Awake()
  {
    inputMaster = scriptMaster.GetComponent<InputMaster>();
    trackObj = cameraHead.GetComponent<SteamVR_TrackedObject>();
  }

  void Start()
  {
    transformOffset = transform.position - player.transform.position;
  }

	void FixedUpdate()
  {
    if(inputMaster.usingViveInput && trackObj != null && hmd != null) {
      transform.position = player.transform.position;
      if(player.GetComponent<FauxGravityBody>().IsOrthogonalToAttractor())
        transform.rotation = Quaternion.LookRotation(Vector3.forward, player.transform.up);
      else
        transform.rotation = Quaternion.LookRotation(player.transform.forward, player.transform.up);
    }
    else {
      transform.rotation = player.transform.rotation;
      transform.position = player.transform.position + transform.TransformDirection(transformOffset);
    }
	}

  #endregion MonoBehaviour Callbacks


  #region Methods

  public Quaternion GetHMDRotation()
  {
    if(inputMaster.usingViveInput && trackObj != null && hmd != null) {
      Debug.Log(hmd.transform.rot.eulerAngles);
      return hmd.transform.rot;
    }
    else
      return new Quaternion(0f, 0f, 0f, 0f);
  }

  public Vector3 GetCameraForward()
  {
    if(inputMaster.usingViveInput && trackObj != null && hmd != null)
      return cameraHead.GetComponent<Camera>().transform.forward;
    return Vector3.zero;
  }

  public Vector3 GetCameraUp()
  {
    if(inputMaster.usingViveInput && trackObj != null && hmd != null)
      return cameraHead.GetComponent<Camera>().transform.up;
    return Vector3.zero;
  }

  public void RotateToPlayer()
  {
    cameraHead.GetComponent<Camera>().transform.rotation = Quaternion.LookRotation(transform.forward, player.transform.up);
  }

  public void SetTracking(bool val)
  {
    cameraHead.GetComponent<SteamVR_TrackedObject>().enabled = val;
  }

  public void RotateCamera(float yRot)
  {
    cameraHead.GetComponent<Camera>().transform.rotation *= Quaternion.Euler(0f, yRot, 0f);
  }

  #endregion Methods
}
