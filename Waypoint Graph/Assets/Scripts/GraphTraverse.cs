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

public class GraphTraverse : MonoBehaviour {

  #region Constants

  // Use this instead of 'Mathf.Epsilon' or a similar epsilon value:
  const float EPSILON = 0.0009f;

  #endregion Constants


  #region Members

  public WaypointGraph graph;
  public Waypoint startPoint, endPoint;
  Animation animation;
  public string idleAnimationName, linkAnimationName;
  AudioSource audioSource;
  public AudioClip idleAudioClip, linkAudioClip;
  public bool loopIdleAudioClip = true, loopLinkAudioClip = true;
  bool idleAudioClipPlayed = false, linkAudioClipPlayed = false;
  public bool usesOccupiedLinks = false;
  public int currentIndex;
  public Waypoint currentPoint;
  public WaypointLink currentLink;
  public Waypoint currentDestination;
  public float idleTimer = 0f, currentIdleTimeout;
  public bool moving = false, rotating = false, waiting = false, stopped = false;
  public float moveSpeed = 2f, rotateSpeed = 1f;
  float _waitTimer;
  public float waitTimeout = 3f;
  public Vector3 rotateDirection = Vector3.zero;
  public Quaternion toRotation;
  public float moveStartTime;
  public int setPathIndex = 0;
  public Waypoint[] setPath;

  #endregion Members


  #region MonoBehaviour Callbacks

  void Awake()
  {
    animation = GetComponent<Animation>();
    audioSource = GetComponent<AudioSource>();
  }

  void Start()
  {
    if(graph != null) {
      startPoint = startPoint ?? graph.startPoint;
      endPoint = endPoint ?? graph.endPoint;
    }
    if(setPath.Length == 0) {
      currentPoint = startPoint;
      transform.position = startPoint.transform.position;
      currentIndex = graph.GetIndex(startPoint);
    }
    else {
      currentPoint = setPath[0];
      transform.position = setPath[0].transform.position;
      currentIndex = graph.GetIndex(setPath[0]);
    }
    currentIdleTimeout = Random.Range(currentPoint.minIdleTime, currentPoint.maxIdleTime);
    graph.pointsInUse.Add(currentPoint);
  }

  void Update()
  {
    if(!moving && !rotating && !waiting) {
      if(animation != null && idleAnimationName != "")
        animation.CrossFade(idleAnimationName);
      if(audioSource != null && idleAudioClip != null && ((loopIdleAudioClip && !audioSource.isPlaying) || !idleAudioClipPlayed)) {
        audioSource.clip = idleAudioClip;
        audioSource.Play();
        idleAudioClipPlayed = true;
        linkAudioClipPlayed = false;
      }
      if(idleTimer < currentIdleTimeout)
        idleTimer += Time.deltaTime;
      else {
        currentLink = SelectLink(currentIndex);
        if(currentLink != null) {
          currentDestination = currentPoint == currentLink.fromPoint ? currentLink.toPoint : currentLink.fromPoint;
          rotateDirection = (currentDestination.transform.position - currentPoint.transform.position).normalized;
          toRotation = Quaternion.LookRotation(currentDestination.transform.position - transform.position);
          rotating = true;
        }
      }
    }
    else if(rotating) {
      Debug.Log("Ortho: " + Vector3.Dot(rotateDirection, transform.forward));
      if(animation != null && linkAnimationName != "")
        animation.CrossFade(linkAnimationName);
      if(audioSource != null && linkAudioClip != null && ((loopIdleAudioClip && !audioSource.isPlaying) || !linkAudioClipPlayed)) {
        audioSource.clip = linkAudioClip;
        audioSource.Play();
        linkAudioClipPlayed = true;
        idleAudioClipPlayed = false;
      }

      if(Vector3.Dot(rotateDirection, transform.forward) <= 1 - EPSILON)
        transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, Mathf.Max(rotateSpeed * Time.deltaTime, 0.35f));
      else {
        rotating = false;
        waiting = true;
      }
    }
    else if(waiting) {
      if(animation != null && idleAnimationName != "")
        animation.CrossFade(idleAnimationName);
      if(audioSource != null && idleAudioClip != null && ((loopIdleAudioClip && !audioSource.isPlaying) || !idleAudioClipPlayed)) {
        audioSource.clip = idleAudioClip;
        audioSource.Play();
        idleAudioClipPlayed = true;
        linkAudioClipPlayed = false;
      }

      if(_waitTimer >= waitTimeout) {
        waiting = false;
        _waitTimer = 0f;
      }
      else if((!currentLink.Occupied() && !graph.pointsInUse.Contains(currentDestination)) || usesOccupiedLinks) {
        waiting = false;
        moving = true;
        graph.pointsInUse.Remove(currentPoint);
        currentLink.AddOccupant();
        graph.pointsInUse.Add(currentDestination);
        if(animation != null && linkAnimationName != "")
          animation.CrossFade(linkAnimationName);
        if(audioSource != null && linkAudioClip != null && ((loopIdleAudioClip && !audioSource.isPlaying) || !linkAudioClipPlayed)) {
          audioSource.clip = linkAudioClip;
          audioSource.Play();
          linkAudioClipPlayed = true;
          idleAudioClipPlayed = false;
        }
      }
      else if(!usesOccupiedLinks)
        _waitTimer += Time.deltaTime;
    }
    else if(!stopped) {
      Debug.Log(currentLink.length + "  " + currentLink.moveSpeedMultiplier + "  " + currentLink.movementFactor);
      if(animation != null && linkAnimationName != "")
        animation.CrossFade(linkAnimationName);
      if(audioSource != null && linkAudioClip != null && ((loopIdleAudioClip && !audioSource.isPlaying) || !linkAudioClipPlayed)) {
        audioSource.clip = linkAudioClip;
        audioSource.Play();
        linkAudioClipPlayed = true;
        idleAudioClipPlayed = false;
      }

      if(Vector3.Distance(transform.position, currentDestination.transform.position) > EPSILON) {
        transform.position = Vector3.Lerp(currentPoint.transform.position, currentDestination.transform.position, (transform.position - currentPoint.transform.position).magnitude / currentLink.length + moveSpeed * currentLink.movementFactor * Time.deltaTime);
      }
      else {
        idleTimer = 0f;
        currentPoint = currentDestination;
        currentLink.RemoveOccupant();
        if(setPath.Length != 0) {
          ++setPathIndex;
          setPathIndex %= setPath.Length;
        }
        currentIndex = graph.GetIndex(currentPoint);
        currentIdleTimeout = Random.Range(currentPoint.minIdleTime, currentPoint.maxIdleTime);
        moving = false;
      }
    }
    else {
      if(animation != null && idleAnimationName != "")
        animation.CrossFade(idleAnimationName);
      if(audioSource != null && idleAudioClip != null && ((loopIdleAudioClip && !audioSource.isPlaying) || !idleAudioClipPlayed)) {
        audioSource.clip = idleAudioClip;
        audioSource.Play();
        idleAudioClipPlayed = true;
        linkAudioClipPlayed = false;
      }
    }
  }

  #endregion MonoBehaviour Callbacks


  #region Private Methods

  WaypointLink SelectLink(int index)
  {
    if(index >= 0 && index < graph.pointMap.size && !graph.pointMap[index].second.Empty()) {
      if(setPath.Length == 0) {
        float[] thresholds = new float[graph.pointMap[index].second.size];
        float sumOfWeights = 0f;
        for(int i = 0; i < graph.pointMap[index].second.size; ++i) {
          if((!graph.pointMap[index].second[i].Occupied() &&
                  !graph.pointsInUse.Contains(graph.pointMap[index].second[i].PointOpposite(currentPoint))) ||
                  usesOccupiedLinks) {
            thresholds[i] = sumOfWeights + graph.pointMap[index].second[i].weight;
            sumOfWeights += graph.pointMap[index].second[i].weight;
          }
          else
            thresholds[i] = -10f;
        }
        for(int i = 0; i < thresholds.Length; ++i)
          thresholds[i] /= sumOfWeights;
        float f = Random.value;
        for(int i = 0; i < thresholds.Length; ++i)
          if(f <= thresholds[i])
            return graph.pointMap[index].second[i];
        return graph.pointMap[index].second.front;
      }
      else {
        RAList<WaypointLink> links = graph.pointMap[index].second;
        int spi = (setPathIndex + 1) % setPath.Length;
        for(int i = 0; i < links.size; ++i)
          if((currentPoint == links[i].fromPoint && setPath[spi] == links[i].toPoint) ||
                (currentPoint == links[i].toPoint && setPath[spi] == links[i].fromPoint))
            return links[i];
      }
    }
    Debug.LogWarning("GraphTraverse.SelectLink() returning no waypoint.");
    return null;
  }

  #endregion Private Methods


  #region Public Methods

  public void Halt() { stopped = true; }
  public void PermitToMove() { stopped = false; }

  #endregion Public Methods
}
