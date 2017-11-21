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

using System.Collections.Generic;

public class RAList<T> : System.IEquatable<RAList<T>> where T : System.IEquatable<T> {

  #region Constants

  const int DEFAULT_CAPACITY = 3;
  const int EXPANSION_MULTIPLIER = 2;

  #endregion Cosntants

  
  #region Private Members

  T[] _items;
  int _size;
  int _capacity;

  #endregion Private Members


  #region Public Accessors

  public int size { get { return _size; } }
  public int length { get { return _size; } }
  public int capacity { get { return _capacity; } }
  public T front {
    get
    {
      if(_size > 0)
        return _items[0];
      throw new System.InvalidOperationException();
    }
  }
  public T back { get { return _size > 0 ? _items[_size - 1] : default(T); } }
  public T this[int index] { get { return _items[index]; } set { _items[index] = value; } }

  #endregion Public Accessors


  #region Constructors

  public RAList() { Clear(); }

  public RAList(int capacity)
  {
    _size = 0;
    _capacity = capacity;
    _items = new T[_capacity];
  }

  public RAList(RAList<T> other)
  {
    _size = other._size;
    _capacity = other._capacity;
    _items = new T[_capacity];
    for(int i = 0; i < _size; ++i)
      _items[i] = other._items[i];
  }

  public RAList(T[] baseArray)
  {
    _size = baseArray.Length;
    _capacity = baseArray.Length + 10;
    _items = new T[_capacity];
    for(int i = 0; i < baseArray.Length; ++i)
      _items[i] = baseArray[i];
  }

  #endregion Constructors


  #region Private Methods

  void expand()
  {
    _capacity = _capacity == 0 ? DEFAULT_CAPACITY : _capacity * EXPANSION_MULTIPLIER + 1;
    updateItemsToCapacity();
  }

  void shrinkToFit()
  {
    _capacity = _size;
    updateItemsToCapacity();
  }

  void updateItemsToCapacity()
  {
    T[] temp = _items;
    _items = new T[_capacity];
    for(int i = 0; i < temp.Length; ++i)
      _items[i] = temp[i];
  }

  int findFirstFromIndex(int index, T item)
  {
    for(int i = index; i < _size; ++i)
      if(EqualityComparer<T>.Default.Equals(_items[i], item))
        return i;
    return -1;
  }

  int findLastFromIndex(int index, T item)
  {
    for(int i = index; i >= 0; --i)
      if(EqualityComparer<T>.Default.Equals(_items[i], item))
        return i;
    return -1;
  }

  #endregion Private Methods


  #region Public Methods

  public void PushBack(T item)
  {
    if(_size == _capacity) expand();
    _items[_size] = item;
    ++_size;
  }

  public void PopBack() { _size = _size == 0 ? 0 : _size - 1; }

  public void PushFront(T item) { InsertAt(0, item); }

  public void PopFront() { RemoveAt(0); }

  public bool InsertAt(int index, T item)
  {
    if(index == _size - 1) {
      PushBack(item);
      return true;
    }
    if(index < _size) {
      if(_size == _capacity) expand();
      for(int i = index; i < _size; ++i)
        _items[i + 1] = _items[i];
      _items[index] = item;
      ++_size;
      return true;
    }
    return false;
  }

  public bool RemoveAt(int index)
  {
    if(index == _size - 1) {
      PopBack();
      return true;
    }
    if(index < _size) {
      for(int i = index; i < _size - 1; ++i)
        _items[i] = _items[i + 1];
      --_size;
      return true;
    }
    return false;
  }

  public bool RemoveFirst(T item)
  {
    if(!Contains(item))
      return false;
    RemoveAt(FindFirst(item));
    return true;
  }

  public bool RemoveLast(T item)
  {
    if(!Contains(item))
      return false;
    RemoveAt(FindLast(item));
    return true;
  }

  public T At(int index)
  {
    if(index >= 0 && index < _size)
      return _items[index];
    return default(T);
  }

  public int FindFirst(T item) { return findFirstFromIndex(0, item); }

  public int FindLast(T item) { return findLastFromIndex(_size - 1, item); }

  public bool Contains(T item) { return FindFirst(item) != -1; }

  public bool Empty() { return _size == 0; }

  public void Clear()
  {
    _size = 0;
    _capacity = DEFAULT_CAPACITY;
    _items = new T[_capacity];
  }

  public override string ToString()
  {
    string s = "[ ";
    for(int i = 0; i < _size - 1; ++i)
      s += _items[i].ToString() + ", ";
    s += _items[_size - 1].ToString() + " ]";
    return s;
  }

  public bool Equals(RAList<T> other)
  {
    if((object)other == null ||_size != other._size || _capacity != other._capacity)
      return false;
    bool equal = true;
    for(int i = 0; i < _size; ++i)
      if(!_items[i].Equals(other._items[i])) {
        equal = false;
        break;
      }
    return equal;
  }

  #endregion Public Methods


  #region Operators

  public static bool operator == (RAList<T> a, RAList<T> b) { return a.Equals(b); }
  public static bool operator != (RAList<T> a, RAList<T> b) { return !(a == b); }

  #endregion Operators
}
