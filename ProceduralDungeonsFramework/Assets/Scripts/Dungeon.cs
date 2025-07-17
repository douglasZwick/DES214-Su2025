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


  void Start()
  {
    e_BuildRequest?.Invoke();
    e_BuildComplete?.Invoke();
  }
}
