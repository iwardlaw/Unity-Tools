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

  #region Subclasses

  [System.Serializable]
  public class SpeechClip {
    public AudioClip clip;
    [Range(0f, 1f)]
    public float volume = 1f;
    [Range(0f, 1f)]
    public float probabilityWeight = 1f;
  }

  #endregion Subclasses


  #region Members

  Transform tf;
  Rigidbody rb;
  AudioSource audsrc;
  public InputMaster inputMaster;
  public float moveSpeed = 15f, rotateSpeed = 2.5f;
  public bool canMove = true;
  public float interactDistance = 3f;
  public float interactCooldown = 0.1f;
  float interactTimer = 0f;
  bool interacting = false;
  public GameObject hmdCamera;
  public bool isMute = false;
  public SpeechClip[] speechClips;

  #endregion Members


  #region MonoBehaviour Callbacks

  void Awake()
  {
    tf = GetComponent<Transform>();
    rb = GetComponent<Rigidbody>();
    audsrc = GetComponent<AudioSource>();
  }

  void Start()
  {
    rb.constraints = RigidbodyConstraints.FreezeRotation;
    inputMaster = FindObjectOfType<InputMaster>();
  }
  
  void Update()
  {
    if(canMove) {
      tf.rotation *= Quaternion.Euler(0f, inputMaster.GetInput("Horizontal") * rotateSpeed, 0f);
      rb.MovePosition(rb.position + tf.TransformDirection(new Vector3(0f, 0f, inputMaster.GetInput("Vertical") * moveSpeed * Time.deltaTime)));
      if(!interacting) {
        if(interactTimer > interactCooldown && inputMaster.GetInput("Interact") == 1f) {
          RaycastHit hit;
          if(Physics.Raycast(tf.position, tf.forward, out hit, interactDistance)) {
            if(hit.collider.tag == "HasText") {
              hit.collider.GetComponent<ActivateText>().Activate();
              interactTimer = 0f;
            }
          }
        }
        interactTimer += Time.deltaTime;
      }
    }
	}

  #endregion MonoBehaviour Callbacks


  #region Methods

  public void Speak()
  {
    if(!isMute && speechClips.Length > 0 && !audsrc.isPlaying) {
      bool probsValid = false;
      foreach(SpeechClip sc in speechClips)
        if(sc.clip != null && sc.probabilityWeight > 0f) {
          probsValid = true;
          break;
        }

      if(probsValid) {
        int index = 0;
        while(true) {
          index = Random.Range(0, speechClips.Length);
          if(Random.value <= speechClips[index].probabilityWeight)
            break;
        }
        audsrc.clip = speechClips[index].clip;
        audsrc.volume = speechClips[index].volume;
        audsrc.Play();
      }
    }
  }

  #endregion Methods
}
