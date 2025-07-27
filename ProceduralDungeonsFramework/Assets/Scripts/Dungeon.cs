using UnityEngine;
using System.Collections.Generic;


public class Dungeon : MonoBehaviour
{
  public delegate void DungeonEvent(DungeonEventData dungeonED);
  public event DungeonEvent e_BuildRequest;
  public event DungeonEvent e_BuildComplete;
  public event DungeonEvent e_StaircaseTriggered;

  public GameObject m_DungeonRoot;
  public GameObject m_TunnelRoom;
  // public Transform m_ItemRoom;
  public Dictionary<Vector2Int, RoomData> m_Grid = new();
  public Vector2 m_RoomSize = new(16, 11);
  public GameObject m_Hero;

  static public List<Tunnel> s_Tunnels = new();


  void Awake()
  {
    e_StaircaseTriggered += OnStaircaseTriggered;
  }


  void Start()
  {
    var dungeonED = new DungeonEventData();

    e_BuildRequest?.Invoke(dungeonED);
    e_BuildComplete?.Invoke(dungeonED);
  }


  static public void RegisterTunnel(Staircase a, Staircase b)
  {
    var tunnel = new Tunnel(a, b);
    s_Tunnels.Add(tunnel);

    Staircase.Connect(a, b);
  }


  void OnStaircaseTriggered(DungeonEventData dungeonED)
  {

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


public class DungeonEventData
{

}
