using System;
using UnityEngine;


public class RoomData : MonoBehaviour
{
  [Serializable]
  public class DoorData
  {
    public bool m_N = false;
    public bool m_S = false;
    public bool m_E = false;
    public bool m_W = false;


    public bool Has(Direction direction)
    {
      return direction == Direction.N && m_N ||
             direction == Direction.S && m_S ||
             direction == Direction.E && m_E ||
             direction == Direction.W && m_W;
    }
  }

  [Serializable]
  public class Tags
  {
    public bool m_Origin = false;
  }


  public DoorData m_Doors = new();
  public Tags m_Tags = new();
}
