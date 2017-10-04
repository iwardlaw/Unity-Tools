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
public class FauxGravityBody : MonoBehaviour {

  #region Public Members

  public GameObject scriptMaster;
  public GameObject ground;
  public float switchButtonTimeout = 0.25f;
  public GameObject player;
  public float raycastCheckDistance = 2.5f;
  [Range(0f, 1f)] public float raycastCheckOrthonogality = 0.9f;
  public float groundedDistance = 1f;
  public LayerMask groundLayer, fauxGravityLayer;
  public bool releaseAnywhere = false;
  public bool releasingAtGround = true;

  #endregion Public Members


  #region Private Members

  InputMaster inputMaster;
  FauxGravityAttractor attractor;
  float switchButtonTimer;
  Rigidbody rb;
  Transform tf;
  FauxGravityAttractor gravSource;
  bool upright;
  Vector3 raycastCheckPos;

  #endregion Private Members


  #region MonoBehaviour Callbacks

  void Awake()
  {
    rb = GetComponent<Rigidbody>();
    tf = GetComponent<Transform>();
    attractor = null;
    inputMaster = scriptMaster.GetComponent<InputMaster>();
  }
  
	void Start()
  {
    rb.constraints = RigidbodyConstraints.FreezeRotation;
    rb.useGravity = true;
    switchButtonTimer = 0f;
    gravSource = null;
    upright = true;
	}
	
	void Update()
  {
    if(switchButtonTimer > switchButtonTimeout) {
      if(inputMaster.GetInput("Interact") == 1f) {
        RaycastHit hit;
        if(Physics.Raycast(tf.position, tf.forward, out hit, raycastCheckDistance, fauxGravityLayer) && Mathf.Abs(Vector3.Dot(tf.forward, hit.collider.transform.up)) >= raycastCheckOrthonogality) {
          attractor = hit.collider.GetComponentInParent<FauxGravityAttractor>();
          rb.useGravity = false;
          gravSource = attractor;
          upright = false;
          releasingAtGround = false;
        }
        else if((gravSource != null && (releaseAnywhere || Physics.Raycast(tf.position, tf.forward, out hit, raycastCheckDistance, groundLayer)))) {
          Release();
          releasingAtGround = true;
        }
      
        switchButtonTimer = 0f;
      }
    }
    else
      switchButtonTimer += Time.deltaTime;

    if(attractor != null && gravSource == attractor) {
      if(switchButtonTimer > switchButtonTimeout && !IsGroundedOnLayer(fauxGravityLayer))
        Release();
      else
        attractor.Attract(tf);
    }
    else if(!upright)
      upright = attractor.Release(tf);
	}

  #endregion MonoBehaviour Callbacks


  #region Collision Callbacks

  void OnCollisionEnter(Collision coln)
  {
    if(!upright && (attractor == null || gravSource != attractor) && !releasingAtGround && coln.collider.tag == "Floor") {
      rb.rotation = Quaternion.Euler(0f, rb.rotation.eulerAngles.y, 0f);
      rb.constraints = RigidbodyConstraints.FreezeRotation;
      if(attractor != null) attractor.RemoveFromReleaseList(tf.GetInstanceID());
      upright = true;
    }
  }

  void OnCollisionStay(Collision coln)
  {
    if(!upright && (attractor == null || gravSource != attractor) && !releasingAtGround && coln.collider.tag == "Floor") {
      rb.rotation = Quaternion.Euler(0f, rb.rotation.eulerAngles.y, 0f);
      rb.constraints = RigidbodyConstraints.FreezeRotation;
      if(attractor != null) attractor.RemoveFromReleaseList(tf.GetInstanceID());
      upright = true;
    }
  }

  #endregion Collision Callbacks


  #region Methods

  public bool IsGroundedOnLayer(LayerMask layer)
  {
    RaycastHit hit;
    return Physics.Raycast(tf.position, -tf.up, out hit, groundedDistance, layer);
  }

  public bool IsOrthogonalToAttractor()
  {
    if(gravSource != null)
      return Vector3.Dot(transform.up, attractor.GetUp()) == 1f;
    return Vector3.Dot(transform.up, ground.transform.up) > 0.9f;
  }

  void Release()
  {
    rb.useGravity = true;
    gravSource = null;
  }

  #endregion Methods
}
