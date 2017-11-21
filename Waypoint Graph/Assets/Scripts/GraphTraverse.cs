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
using EndAction = WaypointGraph.EndAction;

public class GraphTraverse : MonoBehaviour {

  #region Constants

  // Use this instead of 'Mathf.Epsilon' or a similar epsilon value:
  const float EPSILON = 0.0009f;

  #endregion Constants


  #region Public Members

  public WaypointGraph graph;


  [Header("Path Settings")]

  public bool overrideGraphPath;
  public Waypoint startPoint, endPoint;
  public Waypoint[] setPath;

  [Tooltip("The number of full traversals to make on this path. If no End Point is set above, if it is the same " +
    "as the last point in Set Path, or if it not set to a point in Set Path (provided Set Path has at least one" +
    "point), then the path will end at the last Set Path point as soon as this number of traversals is completed. " +
    "If the End Point is the same any point in Set Path except the last one, then the path will end at that point " +
    "after this number of traversals is completed.\n\nIf less than 0, then the object traverses the path indefinitely.")]
  public int setPathTraversals = 0;

  [Tooltip("Action to take when this object reaches either 'End Point' or the last point on its Set Path. " +
    "\"Release\" releases the object from the graph; \"Hold\" holds the object at that point (the waypoint remains occupied); " +
    "\"Destroy\" destroys the object.")]
  public EndAction endAction;

  [Tooltip("If End Action = Destroy, the number of seconds to wait before destroying the object.")]
  public float destroyDelay;


  [Header("Movement Settings")]

  public bool usesOccupiedLinks = false;
  public float moveSpeed = 2f, rotateSpeed = 1f;
  public float waitTimeout = 3f;


  [Header("AV Settings")]

  public string idleAnimationName;
  public string linkAnimationName;
  public AudioClip idleAudioClip, linkAudioClip;
  public bool loopIdleAudioClip = true, loopLinkAudioClip = true;

  #endregion Public Members


  #region Private Members

  // Path Variables
  bool pathEnding = false;
  int setPathIndex = 0;
  int currentTraversals = 0;

  // Movement Variables
  int currentIndex;
  Waypoint currentPoint;
  WaypointLink currentLink;
  Waypoint currentDestination;
  float idleTimer = 0f, currentIdleTimeout;
  bool moving = false, rotating = false, waiting = false, stopped = false;
  float _waitTimer;
  Vector3 rotateDirection = Vector3.zero;
  Quaternion toRotation;
  float moveStartTime;

  // AV Variables
  Animation anim;
  AudioSource audioSource;
  bool idleAudioClipPlayed = false, linkAudioClipPlayed = false;

  #endregion Private Memebers


  #region MonoBehaviour Callbacks

  void Awake()
  {
    anim = GetComponent<Animation>();
    audioSource = GetComponent<AudioSource>();
  }

  void Start()
  {
    // TODO: Close application if 'graph' is null.

    if(!overrideGraphPath) {
      startPoint = graph.startPoint ?? startPoint;
      if(graph.setPath.Length != 0) {
        setPath = new Waypoint[graph.setPath.Length];
        for(int i = 0; i < graph.setPath.Length; ++i)
          setPath[i] = graph.setPath[i];
      }
      if(setPath.Length != 0)
        endPoint = graph.endPoint ?? setPath[setPath.Length - 1];
      else
        endPoint = graph.endPoint ?? endPoint;
      setPathTraversals = graph.setPathTraversals;
      endAction = graph.endAction;
      destroyDelay = graph.destroyDelay;
    }

    // TODO: Close application if points are null.

    currentPoint = startPoint;
    transform.position = startPoint.transform.position;
    currentIndex = graph.GetIndex(startPoint);
    currentIdleTimeout = Random.Range(startPoint.minIdleTime, startPoint.maxIdleTime);
    graph.pointsInUse.Add(startPoint);

    //if(graph != null) {
    //  startPoint = startPoint ?? graph.startPoint;
    //  //endPoint = endPoint ?? graph.endPoint;
    //}
    //if(setPath.Length == 0) {
    //  currentPoint = startPoint;
    //  transform.position = startPoint.transform.position;
    //  currentIndex = graph.GetIndex(startPoint);
    //  endPoint = endPoint ?? graph.endPoint;
    //}
    //else {
    //  currentPoint = setPath[0];
    //  transform.position = setPath[0].transform.position;
    //  currentIndex = graph.GetIndex(setPath[0]);
    //  endPoint = endPoint ?? setPath[setPath.Length - 1];
    //}
    //currentIdleTimeout = Random.Range(currentPoint.minIdleTime, currentPoint.maxIdleTime);
    //graph.pointsInUse.Add(currentPoint);
  }

  void Update()
  {
    if(graph != null) {
      if(!moving && !rotating && !waiting) {
        if(anim != null && idleAnimationName != "")
          anim.CrossFade(idleAnimationName);
        if(audioSource != null && idleAudioClip != null && ((loopIdleAudioClip && !audioSource.isPlaying) || !idleAudioClipPlayed)) {
          audioSource.clip = idleAudioClip;
          audioSource.Play();
          idleAudioClipPlayed = true;
          linkAudioClipPlayed = false;
        }
        if(idleTimer < currentIdleTimeout)
          idleTimer += Time.deltaTime;
        else if(!stopped) {
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
        //Debug.Log("Ortho: " + Vector3.Dot(rotateDirection, transform.forward));
        if(anim != null && linkAnimationName != "")
          anim.CrossFade(linkAnimationName);
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
        if(anim != null && idleAnimationName != "")
          anim.CrossFade(idleAnimationName);
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
          if(anim != null && linkAnimationName != "")
            anim.CrossFade(linkAnimationName);
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
        //Debug.Log(currentLink.length + "  " + currentLink.moveSpeedMultiplier + "  " + currentLink.movementFactor);
        if(anim != null && linkAnimationName != "")
          anim.CrossFade(linkAnimationName);
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
            if(setPathIndex == setPath.Length - 1)
              ++currentTraversals;
            else if(setPathIndex == setPath.Length)
              setPathIndex %= setPath.Length;

            if(currentTraversals == setPathTraversals && currentPoint == endPoint)
              pathEnding = true;
          }
          else if(currentPoint == endPoint)
            pathEnding = true;

          currentIndex = graph.GetIndex(currentPoint);
          currentIdleTimeout = Random.Range(currentPoint.minIdleTime, currentPoint.maxIdleTime);
          moving = false;

          if(pathEnding)
            switch(endAction) {
              case EndAction.Release:
                RemoveFromGraph();
                break;
              case EndAction.Hold:
                stopped = true;
                //graph.pointsInUse.Remove(currentPoint == currentLink.fromPoint ? currentLink.toPoint : currentLink.fromPoint);
                currentLink.RemoveOccupant();
                //graph.pointsInUse.Remove(currentPoint);
                break;
              case EndAction.Destroy:
                RemoveFromGraph();
                Destroy(gameObject, destroyDelay);
                return;
            }
        }
      }
      else {
        if(anim != null && idleAnimationName != "")
          anim.CrossFade(idleAnimationName);
        if(audioSource != null && idleAudioClip != null && ((loopIdleAudioClip && !audioSource.isPlaying) || !idleAudioClipPlayed)) {
          audioSource.clip = idleAudioClip;
          audioSource.Play();
          idleAudioClipPlayed = true;
          linkAudioClipPlayed = false;
        }
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

  void RemoveFromGraph(WaypointGraph newGraph = null)
  {
    currentLink.RemoveOccupant();
    if(currentPoint != null)
      graph.pointsInUse.Remove(currentPoint);
    if(currentDestination != null)
      graph.pointsInUse.Remove(currentDestination);
    anim.CrossFade(idleAnimationName);
    //usingGraph = false;
    graph = newGraph;
  }

  #endregion Private Methods


  #region Public Methods

  public void Halt() { stopped = true; }
  public void PermitToMove() { stopped = false; }

  #endregion Public Methods
}
