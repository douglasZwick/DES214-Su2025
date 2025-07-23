using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class InputHold
{
  static public HoldEvent e_HoldKeyAdded = new();
  static public HoldEvent e_HoldKeyRemoved = new();
  static public HoldEvent e_HoldStarted = new();
  static public HoldEvent e_HoldReleased = new();

  static HashSet<object> s_Keys = new();

  static public HashSet<object> KeysCopy
  {
    get { return new HashSet<object>(s_Keys); }
  }

  static public bool HasHolds
  {
    get { return s_Keys.Count > 0; }
  }


  static public void Add(object key)
  {
    var keyCount = s_Keys.Count;

    s_Keys.Add(key);

    var holdED = new HoldEventData();
    holdED.m_Key = key;
    e_HoldKeyAdded.Invoke(holdED);

    if (keyCount <= 0)
    {
      e_HoldStarted.Invoke(holdED);
    }
  }


  static public void Release(object key)
  {
    if (!s_Keys.Contains(key))
    {
      var m = $"Tried to release hold for {key}, but it was already free.";
      Debug.LogError(m);

      return;
    }

    var holdED = new HoldEventData();
    holdED.m_Key = key;
    e_HoldKeyRemoved.Invoke(holdED);

    s_Keys.Remove(key);

    if (s_Keys.Count <= 0)
    {
      e_HoldReleased.Invoke(holdED);
    }
  }


  static public void ReleaseAll()
  {
    if (s_Keys.Count > 0)
    {
      s_Keys.Clear();

      var holdED = new HoldEventData();
      e_HoldReleased.Invoke(holdED);
    }
  }
}


[System.Serializable]
public class HoldEvent : UnityEvent<HoldEventData> { }

public class HoldEventData
{
  public object m_Key;
}
