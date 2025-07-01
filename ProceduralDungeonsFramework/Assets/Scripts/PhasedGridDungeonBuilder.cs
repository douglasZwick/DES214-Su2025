using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class PhasedGridDungeonBuilder : MonoBehaviour
{
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

    static public List<Connection> From(RoomData roomData, Vector2Int index)
    {
      var output = new List<Connection>();

      if (roomData.m_Doors.m_N)
        output.Add(new Connection(index, Direction.N));
      if (roomData.m_Doors.m_S)
        output.Add(new Connection(index, Direction.S));
      if (roomData.m_Doors.m_E)
        output.Add(new Connection(index, Direction.E));
      if (roomData.m_Doors.m_W)
        output.Add(new Connection(index, Direction.W));

      return output;
    }
  }

  public List<RoomData> m_RoomPrefabs = new();

  public int m_SetupRoomCount = 3;
  public int m_Dev1RoomCount = 5;
  public int m_Dev2RoomCount = 6;
  public int m_Dev3RoomCount = 7;

  private Dictionary<Vector2Int, RoomData> m_Grid = new();
  private List<Connection> m_ToDo = new();
  private int m_TotalLength = 0;


  void Start()
  {
    AddStartingRoom();
    BuildGoldenPath();
  }


  RoomData PickRandom(IEnumerable<RoomData> collection)
  {
    var list = collection.ToList();
    var index = Random.Range(0, list.Count);
    return list[index];
  }


  Direction PickRandomDirection()
  {
    return (Direction)Random.Range(0, 4);
  }


  Direction PickRandomDirection(Direction exclusion)
  {
    switch (exclusion)
    {
      default:    // N
        return (Direction)Random.Range(1, 4);
      case Direction.S:
      {
        var r = Random.Range(0, 3);
        if (r == 0) return Direction.N;
        return (Direction)(r + 1);
      }
      case Direction.E:
      {
        var r = Random.Range(0, 3);
        if (r == 2) return Direction.W;
        return (Direction)r;
      }
      case Direction.W:
        return (Direction)Random.Range(0, 3);
    }
  }


  void Add(RoomData roomPrefab, Vector2Int index)
  {
    var roomWasAdded = m_Grid.TryAdd(index, roomPrefab);

    if (!roomWasAdded)
    {
      // TODO:
      // Complain if there was already something there
    }

    var newConnections = Connection.From(roomPrefab, index);
    m_ToDo.AddRange(newConnections);
  }


  Connection AddStartingRoom()
  {
    var direction = PickRandomDirection();

    var filteredPrefabs =
      from prefab in m_RoomPrefabs
      where prefab.m_Tags.m_Origin && prefab.m_Doors.Has(direction)
      select prefab;
    var selectedPrefab = PickRandom(filteredPrefabs);

    AddToPath(selectedPrefab, Vector2Int.zero);

    ++m_TotalLength;

    var connection = new Connection(Vector2Int.zero, direction);

    return connection;
  }


  Connection AddRoomToPath(Connection fromConnection)
  {
    var destinationIndex = fromConnection.Destination;
    var newDirection = PickRandomDirection(fromConnection.m_Direction);

    var filteredPrefabs =
      from prefab in m_RoomPrefabs
      where prefab.m_Doors.Has(fromConnection.m_Direction) &&
        prefab.m_Doors.Has(newDirection)
      select prefab;
    var selectedPrefab = PickRandom(filteredPrefabs);

    AddToPath(selectedPrefab, destinationIndex);

    ++m_TotalLength;

    var connection = new Connection(destinationIndex, newDirection);
    return connection;
  }


  void AddToPath(RoomData roomData, Vector2Int index)
  {
    var roomWasAdded = m_Grid.TryAdd(index, roomData);

    if (!roomWasAdded)
    {
      // TODO:
      // Complain if there was already something there
    }
  }


  void BuildGoldenPath()
  {
    BuildSetup();
    BuildDev1();
    BuildDev2();
    BuildDev3();
  }


  void BuildSetup()
  {

  }


  void BuildDev1()
  {

  }


  void BuildDev2()
  {

  }


  void BuildDev3()
  {

  }
}


public enum Direction { N, S, E, W, }
