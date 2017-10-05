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
using UnityEngine.UI;

public class ActivateText : MonoBehaviour {

  #region Subclasses

  [System.Serializable]
  public class Pose {
    public AudioClip audioClip;
    public Vector3 positionChange;
    public float angleToPlayer = 0f;
    public float rotateSpeed = 150f;
    public bool returnToOriginalFacing = true;
    public bool finalPose = false;
    public bool overrideDefaultAnimation = true;
    public string conditionName;
    int _conditionHash;
    public int conditionHash {get {return _conditionHash;} }
    public bool resetConditionAfterPose = true;
    public void SetConditionHash() {_conditionHash = Animator.StringToHash(conditionName);}
  }

  [System.Serializable]
  public class TextInfo {
    public bool isActive = true;
    public TextAsset text;
    public int currentLine = 0, endAtLine = 0;
    public bool oneTimeUse = false;
    public string lineSeparator = "::", sublineSeparator = ",,";
    public Pose[] poses;
  }

  #endregion Subclasses


  #region Members

  public GameObject player;
  public TextboxManager textboxManager;
  public Text textbox;
  public GameObject textboxPanel;
  public Canvas canvas;
  public Animator animator;
  public AudioSource audioSource;
  public string defaultAnimationName;
  int _defaultAnimationHash = 0;
  public int defaultAnimationHash {get {return _defaultAnimationHash;} }
  [System.NonSerialized] public int currentText = 0;
  float textboxMinDistance = 1f;
  Quaternion _originalRotation;
  public Quaternion originalRotation {get {return _originalRotation;} }
  public TextInfo[] texts;

  #endregion Members


  #region MonoBehaviour Callbacks

  void Start()
  {
    textboxManager = FindObjectOfType<TextboxManager>();
    if(!textbox) textbox = GetComponentInChildren<Text>();
    if(!audioSource) audioSource = GetComponent<AudioSource>();
    if(!animator) animator = GetComponent<Animator>();
    if(defaultAnimationName != "") {
      _defaultAnimationHash = Animator.StringToHash(defaultAnimationName);
      if(animator)
        animator.SetBool(_defaultAnimationHash, true);
    }
    textboxPanel.SetActive(false);
    foreach(TextInfo t in texts)
      foreach(Pose p in t.poses)
        p.SetConditionHash();
    _originalRotation = transform.rotation;
  }

  #endregion MonoBehaviour Callbacks


  #region Public Methods

  public void Activate(bool deactivate = false)
  {
    if(texts.Length > 0 && !audioSource.isPlaying) {
      int i = 0;
      while(!texts[currentText].isActive && i < texts.Length) {
        ++currentText;
        currentText %= texts.Length;
        ++i;
      }

      if(i < texts.Length) {
        textboxManager.LoadText(this);
        textboxManager.EnableTextbox();

        if(texts[currentText].oneTimeUse || deactivate)
          texts[currentText].isActive = false;
      }
      SetTextboxPos();

      ++currentText;
      currentText %= texts.Length;
    }
  }

  public void SetTextboxPos()
  {
    Vector3 diffVec = transform.position - player.transform.position;
    diffVec = diffVec.normalized * Mathf.Pow(diffVec.magnitude * 0.59f, 1.1f);
    if(diffVec.magnitude < textboxMinDistance)
      diffVec = diffVec.normalized * textboxMinDistance;
    canvas.transform.position = new Vector3(transform.position.x - diffVec.x, canvas.transform.position.y, transform.position.z - diffVec.z);
    canvas.transform.rotation = Quaternion.LookRotation(diffVec, player.transform.up);
  }

  public void SetDefaultAnimationName(string newName)
  {
    defaultAnimationName = newName;
    _defaultAnimationHash = Animator.StringToHash(newName);
  }

  #endregion Public Methods
}
