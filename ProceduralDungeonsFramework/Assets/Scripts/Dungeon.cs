using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;


public class Dungeon : MonoBehaviour
{
  [System.Serializable]
  public class Events
  {
    public DungeonEvent e_BuildRequest;
    public DungeonEvent e_BuildComplete;
  }

  public Events m_Events;

  static public DungeonEvent s_StaircaseTriggered = new();
  static public DungeonEvent s_TunnelEntered = new();
  static public DungeonEvent s_TunnelExited = new();

  public Transform m_DungeonRoot;
  public GameObject m_TunnelRoom;
  public Transform m_TunnelRoomExitTargetA;
  public Transform m_TunnelRoomExitTargetB;
  // public Transform m_ItemRoom;
  public Dictionary<Vector2Int, RoomData> m_Grid = new();
  public Vector2 m_RoomSize = new(16, 11);
  public GameObject m_Hero;

  static public List<Tunnel> s_Tunnels = new();
  static public Dictionary<Staircase, Staircase> s_StairsAtoB = new();
  static public Dictionary<Staircase, Staircase> s_StairsBtoA = new();


  void Start()
  {
    var dungeonED = new DungeonEventData();
    dungeonED.m_DungeonRoot = m_DungeonRoot;
    dungeonED.m_Grid = m_Grid;
    dungeonED.m_RoomSize = m_RoomSize;

    m_Events.e_BuildRequest.Invoke(dungeonED);
    m_Events.e_BuildComplete.Invoke(dungeonED);
  }


  static public void RegisterTunnel(Staircase a, Staircase b)
  {
    var tunnel = new Tunnel(a, b);
    s_Tunnels.Add(tunnel);

    s_StairsAtoB.Add(a, b);
    s_StairsBtoA.Add(b, a);

    Staircase.Connect(a, b);
  }


  static public void UseStairs(StaircaseUser user, Staircase stairs)
  {
    var dungeonED = new DungeonEventData();
    dungeonED.m_StaircaseUser = user;
    dungeonED.m_StairsBeingUsed = stairs;
    s_StaircaseTriggered.Invoke(dungeonED);
  }
}


public class Tunnel
{
  public RoomData m_RoomA;
  public RoomData m_RoomB;
  public int m_StairIndexA;
  public int m_StairIndexB;

  public Vector2Int IndexA { get => m_RoomA.m_Index; }
  public Vector2Int IndexB { get => m_RoomB.m_Index; }

  public Tunnel() { }

  public Tunnel(Staircase a, Staircase b)
  {
    m_RoomA = a.FromRoom;
    m_RoomB = b.FromRoom;
    m_StairIndexA = a.m_StairIndex;
    m_StairIndexB = b.m_StairIndex;
  }
}


[System.Serializable]
public class DungeonEvent : UnityEvent<DungeonEventData> { }

public class DungeonEventData
{
  public Transform m_DungeonRoot;
  public Dictionary<Vector2Int, RoomData> m_Grid;
  public Vector2 m_RoomSize;
  public StaircaseUser m_StaircaseUser;
  public Staircase m_StairsBeingUsed;
  public Vector2Int m_DestinationIndex;
}
