using System.Collections.Generic;
using UnityEngine;


public class GridDungeonBuilder : MonoBehaviour
{
  public enum Direction { N, S, E, W, }


  public class Connection
  {
    public Vector2Int m_Source = new();
    public Direction m_Direction = Direction.N;

    public Connection(Vector2Int source, Direction direction)
    {
      m_Source = source;
      m_Direction = direction;
    }
    
    public Vector2Int Offset
    {
      get
      {
        return m_Direction switch
        {
          Direction.N => Vector2Int.up,
          Direction.S => Vector2Int.down,
          Direction.E => Vector2Int.right,
          _ => Vector2Int.left,
          // Default is West
        };
      }
    }

    public Vector2Int Destination
    {
      get { return m_Source + Offset; }
    }
  }


  public Vector2 m_CellSize = new(10, 7);
  public List<RoomData> m_RoomPrefabs = new();
  public RoomData m_OriginPrefab;

  private Dictionary<Vector2Int, RoomData> m_Grid = new();
  private List<Connection> m_ToDo = new();


  void Start()
  {

  }


  void Initialize()
  {

  }


  void Add(RoomData room, Vector2Int index)
  {
    var added = m_Grid.TryAdd(index, room);

    if (!added)
    {
      // TODO:
      // Complain if there was already something there
    }
  }


  bool Validate(RoomData room, Vector2Int index)
  {
    // var roomWasFound = m_Grid.TryGetValue(index, out var foundRoom);
    return false;
  }


  RoomData PlaceRoom(RoomData roomPrefab, Vector2Int index)
  {
    var roomPosition = GetPositionFromIndex(index);
    var room = Instantiate(roomPrefab, roomPosition, Quaternion.identity);
    Add(room, index);

    return room;
  }


  Vector3 GetPositionFromIndex(Vector2Int index)
  {
    return index * m_CellSize;
  }
}
