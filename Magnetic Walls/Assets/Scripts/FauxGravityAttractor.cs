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
using System.Collections.Generic;

public class FauxGravityAttractor : MonoBehaviour {

  #region Subclasses

  public class ListVec<T> where T : System.IEquatable<T> {
    T[] items;
    int size;
    int capacity;
    public ListVec() {clear();}
    public ListVec(int cap) {size = 0; capacity = cap; items = new T[capacity];}
    void expand() {capacity = (capacity == 0 ? 10 : capacity * 3 + 1); updateItemsToCapacity();}
    void shrinkToFit() {capacity = size; updateItemsToCapacity();}
    void updateItemsToCapacity() {
      T[] temp = items;
      items = new T[capacity];
      for(int i = 0; i < temp.Length; ++i)
        items[i] = temp[i];
    }
    public void append(T item) {if(size == capacity) expand(); items[size] = item; ++size;}
    public bool removeAt(int index) {
      if(index >= 0 && index < size) {
        T[] temp1 = new T[index];
        T[] temp2 = new T[size - 1 - index];
        for(int i = 0; i < index; ++i)
          temp1[i] = items[i];
        for(int i = 0; i < size - 1 - index; ++i)
          temp2[i] = items[i + index + 1];
        items = new T[capacity];
        for(int i = 0; i < temp1.Length; ++i)
          items[i] = temp1[i];
        for(int i = 0; i < temp2.Length; ++i)
          items[temp1.Length + i] = temp2[i];
        --size;
        return true;
      }
      return false;
    }
    public bool remove(T item) {if(!contains(item)) return false; removeAt(findFirst(item)); return true;}
    public T at(int index) {if(index >= 0 && index < size) return items[index]; return default(T);}
    public T first() {if(!empty()) return items[0]; return default(T);}
    public T last() {if(!empty()) return items[size - 1]; return default(T);}
    public int findFirst(T item) {for(int i = 0; i < size; ++i) if(EqualityComparer<T>.Default.Equals(items[i], item)) return i; return -1;}
    public int findLast(T item) {for(int i = size - 1; i >= 0; --i) if(EqualityComparer<T>.Default.Equals(items[i], item)) return i; return -1;}
    public bool contains(T item) {return findFirst(item) != -1;}
    public bool empty() {return size == 0;}
    public int getSize() {return size;}
    public int getCapacity() {return capacity;}
    public void clear() {size = 0; capacity = 10; items = new T[capacity];}
  }

  public class Vector3Eq : System.IEquatable<Vector3Eq> {
    public Vector3 vec;
    public Vector3Eq(Vector3 baseVector) {vec = baseVector;}
    public bool Equals(Vector3Eq other) {return vec.x == other.vec.x && vec.y == other.vec.y && vec.z == other.vec.z;}
    public static bool operator == (Vector3Eq lhs, Vector3Eq rhs) {return lhs.vec.x == rhs.vec.x && lhs.vec.y == rhs.vec.y && lhs.vec.z == rhs.vec.z;}
    public static bool operator != (Vector3Eq lhs, Vector3Eq rhs) {return lhs.vec.x != rhs.vec.x || lhs.vec.y != rhs.vec.y || lhs.vec.z != rhs.vec.z;}
  }

  #endregion Subclasses


  #region Enums

  public enum AxisDirection {X_POSITIVE, X_NEGATIVE, Y_POSITIVE, Y_NEGATIVE, Z_POSITIVE, Z_NEGATIVE};

  #endregion Enums


  #region Members

  public AxisDirection up = AxisDirection.Y_POSITIVE;
  public float fgrav = -10f;
  public float rotSpeed = 10f;
  public GameObject floor;
  ListVec<int> releasingBodyIDs;
  ListVec<Vector3Eq> initialForwards;

  #endregion Members


  #region MonoBehaviour Callbacks

  void Awake()
  {
    releasingBodyIDs = new ListVec<int>();
    initialForwards = new ListVec<Vector3Eq>();
  }

  void Start()
  {
    gameObject.layer = LayerMask.NameToLayer("FauxGravity");
  }

  #endregion MonoBehaviour Callbacks


  #region Public Methods

  public void Attract(Transform body)
  {
    Vector3 gravUp = GetUp();
    Vector3 bodyUp = body.up;

    body.GetComponent<Rigidbody>().AddForce(gravUp * fgrav);

    Quaternion tgtRot = Quaternion.FromToRotation(bodyUp, gravUp) * body.rotation;
    body.rotation = Quaternion.Slerp(body.rotation, tgtRot, rotSpeed * Time.deltaTime);
  }

  public bool Release(Transform body)
  {
    int bodyID = body.GetInstanceID();
    if(!releasingBodyIDs.contains(bodyID)) {
      releasingBodyIDs.append(bodyID);
      initialForwards.append(new Vector3Eq(body.forward));
      body.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
    }
    
    Vector3 gravUp = floor.transform.up;
    Vector3 bodyUp = body.up;
    float uprightThreshold = 0.1f;
    
    Vector3 initFwd = initialForwards.at(releasingBodyIDs.findFirst(bodyID)).vec;

    body.GetComponent<Rigidbody>().AddForce(gravUp * fgrav);
    
    Quaternion tgtRot = Quaternion.identity;
    if(body.GetComponent<FauxGravityBody>().releasingAtGround) {
      tgtRot = Quaternion.FromToRotation(bodyUp, gravUp) * body.rotation;
      uprightThreshold = .9999f;
    }
    else {
      tgtRot = Quaternion.LookRotation(new Vector3(initFwd.x, 0f, initFwd.z).normalized, gravUp);
      uprightThreshold = .99f;
    }
    body.rotation = Quaternion.Slerp(body.rotation, tgtRot, rotSpeed * Time.deltaTime);

    Debug.Log(Vector3.Dot(floor.transform.up, bodyUp));
    if(Vector3.Dot(floor.transform.up, bodyUp) > uprightThreshold) {
      body.rotation = Quaternion.Euler(0f, body.rotation.eulerAngles.y, 0f);
      body.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;
      Debug.Log(Vector3.Dot(floor.transform.up, bodyUp));
      RemoveFromReleaseList(bodyID);
      body.GetComponent<FauxGravityBody>().releasingAtGround = false;
      return true;
    }
    return false;
  }

  public void RemoveFromReleaseList(int id)
  {
    initialForwards.removeAt(releasingBodyIDs.findFirst(id));
    releasingBodyIDs.remove(id);
  }

  public Vector3 GetUp()
  {
    Vector3 upDir = Vector3.zero;

    switch (up) {
      case AxisDirection.X_POSITIVE:
        upDir = transform.right;
        break;
      case AxisDirection.X_NEGATIVE:
        upDir = -transform.right;
        break;
      case AxisDirection.Y_POSITIVE:
        upDir = transform.up;
        break;
      case AxisDirection.Y_NEGATIVE:
        upDir = -transform.up;
        break;
      case AxisDirection.Z_POSITIVE:
        upDir = transform.forward;
        break;
      case AxisDirection.Z_NEGATIVE:
        upDir = -transform.forward;
        break;
    }

    return upDir;
  }

  public void DebugPrint()
  {
    Debug.Log("Message from FauxGravityAttractor \"" + name + "\".");
  }

  #endregion Public Members
}
