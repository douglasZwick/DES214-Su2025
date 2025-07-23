using UnityEngine;
using System.Collections.Generic;


public class Dungeon : MonoBehaviour
{
  public delegate void DungeonEvent();
  public event DungeonEvent e_BuildRequest;
  public event DungeonEvent e_BuildComplete;

  public GameObject m_DungeonRoot;
  public GameObject m_TunnelRoom;
  // public Transform m_ItemRoom;
  public Dictionary<Vector2Int, RoomData> m_Grid = new();
  public Vector2 m_RoomSize = new(16, 11);
  public GameObject m_Hero;

  static public List<Tunnel> s_Tunnels = new(); 


  void Start()
  {
    e_BuildRequest?.Invoke();
    e_BuildComplete?.Invoke();
  }



}


public class Tunnel
{
  public RoomData m_RoomA;
  public RoomData m_RoomB;
  public int m_StaircaseIndexA;
  public int m_StaircaseIndexB;

  public Vector2Int IndexA { get => m_RoomA.m_Index; }
  public Vector2Int IndexB { get => m_RoomB.m_Index; }
}
