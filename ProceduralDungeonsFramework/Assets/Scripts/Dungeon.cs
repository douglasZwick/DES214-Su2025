using UnityEngine;
using System.Collections.Generic;


public class Dungeon : MonoBehaviour
{
  public delegate void BuildRequest();
  public event BuildRequest e_BuildRequest;

  public Dictionary<Vector2Int, RoomData> m_Grid = new();
  public Vector2 m_RoomSize = new(16, 11);


  void Start()
  {
    e_BuildRequest?.Invoke();
  }
}
