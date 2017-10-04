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

public class Waypoint : MonoBehaviour, System.IEquatable<Waypoint> {

  #region Subclasses

  [System.Serializable]
  public class Action {
    public Animation animation;
    public AudioClip audioClip;
    public Action() { animation = null; audioClip = null; }
    public Action(Action other) { animation = other.animation; audioClip = other.audioClip; }
    public void Play() { }
  }

  #endregion Subclasses


  #region Members

  public bool active = true;
  public float toleranceRadius = 0f;
  public float minIdleTime = 0f;
  public float maxIdleTime = 10f;
  public bool wanderWhileIdle = false;
  public float rotationSpeed = 5f;
  public Action actionOnArrival;
  public Animation[] idleAnimations;

  #endregion Members


  #region Constructor, Methods, and Operators

  public Waypoint(Waypoint other) {
    active = other.active;
    toleranceRadius = other.toleranceRadius;
    minIdleTime = other.minIdleTime;
    maxIdleTime = other.maxIdleTime;
    wanderWhileIdle = other.wanderWhileIdle;
    rotationSpeed = other.rotationSpeed;
    actionOnArrival = other.actionOnArrival;
    idleAnimations = other.idleAnimations;
  }

  public bool Equals(Waypoint other) { if((object)other == null) return false;  return GetInstanceID() == other.GetInstanceID(); }
  public static bool operator == (Waypoint a, Waypoint b) { return a.Equals(b); }
  public static bool operator != (Waypoint a, Waypoint b) { return !(a == b); }

  #endregion Constructor, Methods, and Operators
}
