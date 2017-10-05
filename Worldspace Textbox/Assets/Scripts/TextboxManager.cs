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
using System.Collections;
using UnityEngine.UI;

public class TextboxManager : MonoBehaviour {

  #region Public Members

  public InputMaster inputMaster;
  public GameObject defaultTextboxPanel;
  public Text defaultTextbox;
  public Animator defaultAnimator;
  public AudioSource defaultAudioSource;
  public float audioCutoffFadeDuration = 0.2f;
  public TextAsset textFile;
  public string[][] lines;
  public int currentLine = 0, endAtLine = 0, currentSubline = 0;
  public int currentPose = 0;
  public GameObject player;
  public bool disableTextboxAfterAudio = false;
  public float textboxTimeoutAfterAudio = 0.5f;
  public bool textboxActive = false;
  public bool stopPlayer = true;
  public float textboxInteractCooldown = 0.1f;
  public bool scrollText = true;
  public float writeSpeed = 0.02f;
  public string playerScriptName = "PLAYER";

  #endregion Public Members


  #region Private Members

  GameObject textboxPanel;
  Text textbox;
  Animator animator;
  ActivateText.Pose pose;
  int originalPoseConditionHash = 0, previousPoseConditionHash = 0;
  int defaultAnimationHash = 0;
  AudioSource audioSource;
  int audioClipIndex = 0;
  float originalVolume;
  PlayerController pc;
  float textboxTimerAfterAudio = 0f;
  float textboxInteractTimer = 0f;
  bool isWriting = false;
  bool writingCanceled = false;
  string lineSeparator = "::", sublineSeparator = ",,";
  bool speakerIsPlayer = false;
  ActivateText collocutor;
  ActivateText.TextInfo currText;

  #endregion Private Members


  #region MonoBehaviour Callbacks

  void Awake()
  {
    pc = player.GetComponent<PlayerController>();
    textbox = defaultTextbox;
    textboxPanel = defaultTextboxPanel;
    animator = defaultAnimator;
    audioSource = defaultAudioSource;
  }

  void Start()
  {
    inputMaster = FindObjectOfType<InputMaster>();
    if(audioSource != null)
      originalVolume = audioSource.volume;

    if(textFile != null)
      ParseText(textFile, lineSeparator, sublineSeparator);

    if(textbox != null)
      textbox.text = "";
    else
      textboxActive = false;

    if(textboxActive)
      EnableTextbox();
    else
      DisableTextbox();
  }

  void Update()
  {
    if(textboxActive) {
      if(inputMaster.GetInput("Interact") == 1f && textboxInteractTimer > textboxInteractCooldown) {
        if(!isWriting && !audioSource.isPlaying) {
          if(currentLine <= endAtLine)
            WriteText();
          else if(currentLine > endAtLine)
            DisableTextbox();
        }
        else if(isWriting && !writingCanceled)
          writingCanceled = true;
        textboxInteractTimer = 0f;
      }
      else if(inputMaster.GetInput("Cancel") == 1f && textboxInteractTimer > textboxInteractCooldown) {
        DisableTextbox();
        writingCanceled = true;
        textboxInteractTimer = 0f;
      }

      if(disableTextboxAfterAudio && !audioSource.isPlaying)
        textboxTimerAfterAudio += Time.deltaTime; 
    }

    if(disableTextboxAfterAudio && textboxTimerAfterAudio > textboxTimeoutAfterAudio) {
      DisableTextbox();
      textboxTimerAfterAudio = 0f;
    }

    if(textboxInteractTimer < textboxInteractCooldown /*&& audioSource != null && !audioSource.isPlaying*/)
      textboxInteractTimer += Time.deltaTime;
  }

  #endregion MonoBehaviour Callbacks


  #region Public Methods

  public void EnableTextbox()
  {
    if(textbox != null && textboxPanel != null && textboxInteractTimer > textboxInteractCooldown) {
      textboxPanel.SetActive(true);
      textboxActive = true;
      if(stopPlayer)
        pc.canMove = false;
      if(endAtLine == 0)
        endAtLine = lines.Length - 1;
      textboxInteractTimer = 0f;
      WriteText();
      if(animator != null && pose != null && pose.overrideDefaultAnimation && defaultAnimationHash != Animator.StringToHash("neutral_idle"))
        animator.SetBool(defaultAnimationHash, false);
    }
  }

  public void DisableTextbox()
  {
    if(textbox != null && textboxPanel != null) {
      textbox.text = "";
      currentLine = 0;
      currentSubline = 0;
      textboxPanel.SetActive(false);
      textboxActive = false;
      pc.canMove = true;
      textboxInteractTimer = 0f;
      currentPose = 0;
      if(animator != null && pose != null) {
        if(pose.resetConditionAfterPose /*&& pose.conditionHash != defaultAnimationHash*/) {
          animator.SetBool(pose.conditionHash, false);
          if(defaultAnimationHash != 0)
            animator.SetBool(defaultAnimationHash, true);
        }
        if(pose.returnToOriginalFacing)
          StartCoroutine(RotateCollocutor(collocutor.originalRotation, pose.rotateSpeed));
        pose = null;
      }
      if(audioSource && audioSource.isPlaying)
        StartCoroutine(FadeOutAudio(audioCutoffFadeDuration));
    }
  }

  public void LoadText(ActivateText at)
  {
    if(at != null) {
      collocutor = at;
      currText = at.texts[at.currentText];
      LoadText(currText.text, at.textbox, at.textboxPanel, at.animator, at.defaultAnimationHash, at.audioSource, currText.currentLine, currText.endAtLine, currText.lineSeparator, currText.sublineSeparator);
      currentPose = 0;
    }
  }

  #endregion Public Methods


  #region Private Methods

  void WriteText()
  {
    if(!speakerIsPlayer)
      NpcSpeak();

    if(currentSubline == 0)
      textbox.text = "";
    if(scrollText && !isWriting)
      StartCoroutine(ScrollText(lines[currentLine][currentSubline]));
    else {
      textbox.text += lines[currentLine][currentSubline];
      writingCanceled = false;
    }
    if(OnLastSubline()) {
      ++currentLine;
      currentSubline = 0;
    }
    else
      ++currentSubline;
  }

  void NpcSpeak()
  {
    if(animator != null && pose != null && pose.resetConditionAfterPose) {
      previousPoseConditionHash = pose.conditionHash;
      animator.SetBool(pose.conditionHash, false);
    }

    pose = currText.poses.Length > currentPose ? currText.poses[currentPose] : new ActivateText.Pose();
    Vector3 lookVec = collocutor.transform.position - player.transform.position;
    Quaternion destRot = Quaternion.LookRotation(new Vector3(lookVec.x, 0f, lookVec.z), collocutor.transform.up);
    destRot *= Quaternion.Euler(0f, 180f + pose.angleToPlayer, 0f);
    StartCoroutine(RotateCollocutor(destRot, pose.rotateSpeed));

    if(animator != null && pose != null) {
      animator.SetBool(pose.conditionHash, true);
    }
    if(pose != null && pose.positionChange != Vector3.zero)
      collocutor.transform.Translate(pose.positionChange);
    if(audioSource && !audioSource.isPlaying) {
      audioSource.clip = pose.audioClip;
      audioSource.Play();
    }

    if(currText.poses.Length > 0 && !pose.finalPose) {
      ++currentPose;
      currentPose %= currText.poses.Length;
    }
  }

  IEnumerator ScrollText(string line)
  {
    int chIndex = 0;
    isWriting = true;
    writingCanceled = false;
    string startText = textbox.text;

    while(isWriting && !writingCanceled && chIndex < line.Length - 1) {
      textbox.text += line[chIndex];
      ++chIndex;
      yield return new WaitForSeconds(writeSpeed);
    }
    textbox.text = startText + line;
    isWriting = false;
    writingCanceled = false;
  }

  IEnumerator RotateCollocutor(Quaternion destinationRotation, float rotateSpeed)
  {
    while(collocutor.transform.rotation != destinationRotation) {
      collocutor.transform.rotation = Quaternion.RotateTowards(collocutor.transform.rotation, destinationRotation, rotateSpeed * Time.deltaTime);
      collocutor.SetTextboxPos();
      yield return null;
    }
  }

  IEnumerator FadeOutAudio(float duration)
  {
    while(audioSource.volume > 0f) {
      audioSource.volume -= Time.deltaTime / duration;
      yield return null;
    }
    audioSource.Stop();
    audioSource.volume = originalVolume;
    yield return null;
  }

  void LoadText(TextAsset newText, Text newTextbox, GameObject newPanel, Animator newAnimator, int dfltAnimHash, AudioSource newAudioSource, int currLn, int endLn, string lineSep, string sublineSep)
  {
    //Debug.Log("asdf");
    if(newText != null && !textboxActive) {
      textbox = newTextbox;
      textboxPanel = newPanel;
      animator = newAnimator;
      defaultAnimationHash = dfltAnimHash;
      audioSource = newAudioSource;
      ParseText(newText, lineSep, sublineSep);
      currentLine = currLn;
      endAtLine = endLn;
      lineSeparator = lineSep;
      sublineSeparator = sublineSep;
      originalVolume = newAudioSource.volume;
    }
  }

  void ParseText(TextAsset txt, string lineSep, string sublineSep)
  {
    string[] baseLines = txt.text.Split(new[] { lineSep }, System.StringSplitOptions.RemoveEmptyEntries);
    string[] rawLines = new string[baseLines.Length];
    for(int i = 0; i < baseLines.Length; ++i)
      rawLines[i] = baseLines[i].Trim(System.Environment.NewLine.ToCharArray());
    lines = new string[rawLines.Length][];
    for(int i = 0; i < rawLines.Length; ++i) {
      string[] rawSublines = rawLines[i].Split(new[] { sublineSep }, System.StringSplitOptions.RemoveEmptyEntries);
      lines[i] = new string[rawSublines.Length];
      for(int j = 0; j < rawSublines.Length; ++j)
        lines[i][j] = rawSublines[j];
    }
  }

  bool OnLastSubline()
  {
    return currentSubline == lines[currentLine].Length - 1;
  }

  #endregion Private Methods
}
