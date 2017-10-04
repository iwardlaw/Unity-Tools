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

[System.Serializable]
public class Vector3Eq : System.IEquatable<Vector3Eq> {

  #region Private Members

  Vector3 _vec;

  #endregion Private Members


  #region Public Accessors

  public Vector3 vector { get { return _vec; } set { _vec = value; } }
  public float x { get { return _vec.x; } set { x = value; } }
  public float y { get { return _vec.y; } set { y = value; } }
  public float z { get { return _vec.z; } set { z = value; } }
  public float magnitude { get { return _vec.magnitude; } }
  public float sqrMagnitude { get { return _vec.sqrMagnitude; } }
  public Vector3Eq normalized { get { return new Vector3Eq(_vec.normalized); } }

  #endregion Public Accessors


  #region Static Members

  public static Vector3Eq right { get { return new Vector3Eq(1, 0, 0); } }
  public static Vector3Eq left { get { return new Vector3Eq(-1, 0, 0); } }
  public static Vector3Eq up { get { return new Vector3Eq(0, 1, 0); } }
  public static Vector3Eq down { get { return new Vector3Eq(0, -1, 0); } }
  public static Vector3Eq forward { get { return new Vector3Eq(0, 0, 1); } }
  public static Vector3Eq back { get { return new Vector3Eq(0, 0, -1); } }
  public static Vector3Eq zero { get { return new Vector3Eq(0, 0, 0); } }
  public static Vector3Eq one { get { return new Vector3Eq(1, 1, 1); } }

  #endregion Static Members


  #region Constructors

  public Vector3Eq(Vector3 baseVector) { _vec.x = baseVector.x; _vec.y = baseVector.y; _vec.z = baseVector.z; }
  public Vector3Eq(float baseX, float baseY, float baseZ) { _vec.x = baseX; _vec.y = baseY; _vec.z = baseZ; }
  public Vector3Eq(Vector3Eq other) { _vec = other._vec; }

  #endregion Constructors


  #region Public Methods

  public void Set(float newX, float newY, float newZ) { _vec.Set(newX, newY, newZ); }
  public void Normalize() { _vec.Normalize(); }
  public override string ToString() { return _vec.ToString(); }
  public string ToString(string format) { return _vec.ToString(format); }
  public bool Equals(Vector3Eq other) { if((object)other == null) return false; return _vec.x == other._vec.x && _vec.y == other._vec.y && _vec.z == other._vec.z; }

  #endregion Public Methods


  #region Static Methods

  public static float Angle(Vector3Eq from, Vector3Eq to) { return Vector3.Angle(from._vec, to._vec); }
  public static Vector3Eq ClampMagnitude(Vector3Eq vector3eq, float maxLength) { return new Vector3Eq(Vector3.ClampMagnitude(vector3eq._vec, maxLength)); }
  public static Vector3Eq Cross(Vector3Eq a, Vector3Eq b) { return new Vector3Eq(Vector3.Cross(a._vec, b._vec)); }
  public static float Distance(Vector3Eq a, Vector3Eq b) { return Vector3.Distance(a._vec, b._vec); }
  public static float Dot(Vector3Eq a, Vector3Eq b) { return Vector3.Dot(a._vec, b._vec); }
  public static Vector3Eq Lerp(Vector3Eq a, Vector3Eq b, float t) { return new Vector3Eq(Vector3.Lerp(a._vec, b._vec, t)); }
  public static Vector3Eq LerpUnclamped(Vector3Eq a, Vector3Eq b, float t) { return new Vector3Eq(Vector3.LerpUnclamped(a._vec, b._vec, t)); }
  public static Vector3Eq Max(Vector3Eq a, Vector3Eq b) { return new Vector3Eq(Vector3.Max(a._vec, b._vec)); }
  public static Vector3Eq Min(Vector3Eq a, Vector3Eq b) { return new Vector3Eq(Vector3.Min(a._vec, b._vec)); }
  public static Vector3Eq MoveTowards(Vector3Eq current, Vector3Eq target, float maxDistanceDelta) { return new Vector3Eq(Vector3.MoveTowards(current._vec, target._vec, maxDistanceDelta)); }
  public static void OrthoNormalize(ref Vector3Eq a, ref Vector3Eq b) { Vector3.OrthoNormalize(ref a._vec, ref b._vec); }
  public static Vector3Eq Project(Vector3Eq vector3eq, Vector3Eq onNormal) { return new Vector3Eq(Vector3.Project(vector3eq._vec, onNormal._vec)); }
  public static Vector3Eq ProjectOnPlane(Vector3Eq vector3eq, Vector3Eq planeNormal) { return new Vector3Eq(Vector3.ProjectOnPlane(vector3eq._vec, planeNormal._vec)); }
  public static Vector3Eq Reflect(Vector3Eq inDirection, Vector3Eq inNormal) { return new Vector3Eq(Vector3.Reflect(inDirection._vec, inNormal._vec)); }
  public static Vector3Eq RotateTowards(Vector3Eq current, Vector3Eq target, float maxRadiansDelta, float maxMagnitudeDelta) { return new Vector3Eq(Vector3.RotateTowards(current._vec, target._vec, maxRadiansDelta, maxMagnitudeDelta)); }
  public static Vector3Eq Scale(Vector3Eq a, Vector3Eq b) { return new Vector3Eq(Vector3.Scale(a._vec, b._vec)); }
  public static Vector3Eq Slerp(Vector3Eq a, Vector3Eq b, float t) { return new Vector3Eq(Vector3.Slerp(a._vec, b._vec, t)); }
  public static Vector3Eq SlerpUnclamped(Vector3Eq a, Vector3Eq b, float t) { return new Vector3Eq(Vector3.SlerpUnclamped(a._vec, b._vec, t)); }
  public static Vector3Eq SmoothDamp(Vector3Eq current, Vector3Eq target, ref Vector3Eq currentVelocity, float smoothTime, float maxSpeed = Mathf.Infinity, float deltaTime = -1f) { return new Vector3Eq(Vector3.SmoothDamp(current._vec, target._vec, ref currentVelocity._vec, smoothTime, maxSpeed, deltaTime < 0 ? Time.deltaTime : deltaTime)); }

  #endregion Static Methods


  #region Operators

  public static bool operator == (Vector3Eq a, Vector3Eq b) { return a._vec.x == b._vec.x && a._vec.y == b._vec.y && a._vec.z == b._vec.z; }
  public static bool operator != (Vector3Eq a, Vector3Eq b) { return !(a == b); }
  public static bool operator < (Vector3Eq a, Vector3Eq b) { return a._vec.x < b._vec.x && a._vec.y < b._vec.y && a._vec.z < b._vec.z; }
  public static bool operator > (Vector3Eq a, Vector3Eq b) { return a._vec.x > b._vec.x && a._vec.y > b._vec.y && a._vec.z > b._vec.z; }
  public static bool operator <= (Vector3Eq a, Vector3Eq b) { return !(a > b); }
  public static bool operator >= (Vector3Eq a, Vector3Eq b) { return !(a < b); }
  public static Vector3Eq operator + (Vector3Eq a, Vector3Eq b) { return new Vector3Eq(a._vec + b._vec); }
  public static Vector3Eq operator - (Vector3Eq a, Vector3Eq b) { return new Vector3Eq(a._vec - b._vec); }
  public static Vector3Eq operator * (Vector3Eq a, float f) { return new Vector3Eq(a._vec * f); }
  public static Vector3Eq operator / (Vector3Eq a, float f) { return new Vector3Eq(a._vec / f); }

  #endregion Operators
}