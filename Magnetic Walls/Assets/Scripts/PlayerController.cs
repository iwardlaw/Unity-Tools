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

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour {


  #region Public Members

  public GameObject scriptMaster;
  public ViveCameraRigController viveCamRigCtrlr;
  public float moveSpeed = 10f, rotSpeed = 2.5f;
  public LayerMask groundLayer, fauxGravityLayer;
  public float groundedDistance = 1f;

  #endregion Public Members


  #region Private Members

  InputMaster inputMaster;
  Vector3 dir, rotVec;
  Rigidbody rb;
  Transform tf;
  Vector3 startPos;
  Quaternion startRot;

  #endregion Private Members


  #region MonoBehaviour Callbacks

  void Awake()
  {
    rb = GetComponent<Rigidbody>();
    tf = GetComponent<Transform>();
    inputMaster = scriptMaster.GetComponent<InputMaster>();
  }

  void Start()
  {
    startPos = new Vector3(tf.position.x, tf.position.y, tf.position.z);
    startRot = new Quaternion(tf.rotation.x, tf.rotation.y, tf.rotation.z, tf.rotation.w);
  }
	
	void Update()
  {
    if (inputMaster.GetInput("PlayerReset") == 1f) {
      rb.velocity = Vector3.zero;
      rb.angularVelocity = Vector3.zero;
      tf.position = startPos;
      tf.rotation = startRot;
    }
    dir = new Vector3(0f, 0f, inputMaster.GetInput("Vertical"));
    if(!inputMaster.usingViveInput)
      rotVec = new Vector3(0f, inputMaster.GetInput("Horizontal") * rotSpeed, 0f);
    else if(GetComponent<FauxGravityBody>().IsOrthogonalToAttractor()) {
      rotVec = viveCamRigCtrlr.GetCameraForward();
      rotVec = new Vector3(rotVec.x, 0f, rotVec.z);
    }
  }

  void FixedUpdate()
  {
    if(IsGroundedOnLayer(groundLayer) || IsGroundedOnLayer(fauxGravityLayer)) {
      rb.MovePosition(rb.position + tf.TransformDirection(dir) * moveSpeed * Time.deltaTime);
      if(!inputMaster.usingViveInput)
        rb.rotation *= Quaternion.Euler(rotVec);
      else if(GetComponent<FauxGravityBody>().IsOrthogonalToAttractor()) {
        tf.rotation = Quaternion.LookRotation(rotVec, tf.up);
      }
    }
  }

  #endregion MonoBehaviour Callbacks


  #region Methods

  public bool IsGroundedOnLayer(LayerMask layer)
  {
    RaycastHit hit;
    return Physics.Raycast(tf.position, -tf.up, out hit, groundedDistance, layer);
  }

  #endregion Methods
}