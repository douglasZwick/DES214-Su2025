using UnityEngine;
using System;
using System.Collections.Generic;

public class Room : MonoBehaviour
{
  public RoomData m_RoomData = new();
}


[Serializable]
public class RoomData
{
  // TODO: Since this is basically exactly the same set of stuff I want for
  //   stair data, I should consider renaming this class
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


    public void Set(Direction direction)
    {
      switch (direction)
      {
      default:  // N
        m_N = true;
        break;
      case Direction.S:
        m_S = true;
        break;
      case Direction.E:
        m_E = true;
        break;
      case Direction.W:
        m_W = true;
        break;
      }
    }
  }

  [Serializable]
  public class Tags
  {
    public bool m_Origin = false;
    public bool m_Setup = false;
    public bool m_Hook = false;
    public bool m_Dev1 = false;
    public bool m_Dev2 = false;
    public bool m_Dev3 = false;
    public bool m_Turn = false;
    public bool m_Resolution = false;
  }

  public DoorData m_Doors = new();
  public Tags m_Tags = new();

  [HideInInspector]
  public Vector2Int m_Index = new();
  [HideInInspector]
  public RoomData m_Next;
  [HideInInspector]
  public RoomData m_Prev;
  [HideInInspector]
  public List<RoomData> m_TunnelConnections = new();
  [HideInInspector]
  public Frame m_Frame;
  [HideInInspector]
  public ArcPhase m_Phase = ArcPhase.Setup;

  public bool IsStart
  { get { return m_Prev == null; }}
  public bool IsEnd
  { get { return m_Next == null; }}


  public RoomData() { }


  public RoomData(Vector2Int index)
  {
    m_Index = index;
  }


  public RoomData(Connection connection)
  {
    m_Index = connection.m_Source;
    m_Doors.Set(connection.m_Direction);
  }


  public Direction GetDirectionToNext()
  {
    var currIndex = m_Index;
    var nextIndex = m_Next.m_Index;
    var offset = nextIndex - currIndex;

    return PhasedGridDungeonBuilder.ConvertOffsetToDirection(offset);
  }


  public Direction GetDirectionToPrev()
  {
    var currIndex = m_Index;
    var prevIndex = m_Prev.m_Index;
    var offset = prevIndex - currIndex;

    return PhasedGridDungeonBuilder.ConvertOffsetToDirection(offset);
  }
}
