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

  private enum RoomType
  {
    Starting,
    Standard,
    Hook,
    Turn,
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
    var startingConnection = AddStartingRoomDataToPath();
    BuildGoldenPath(startingConnection);
  }


  T PickRandom<T>(IEnumerable<T> collection)
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


  Connection AddStartingRoomDataToPath()
  {
    var direction = PickRandomDirection();
    var startingConnection = new Connection(Vector2Int.zero, direction);

    AddConnectionToPath(startingConnection);
    ++m_TotalLength;

    return startingConnection;
  }


  Connection AddHookRoomDataToPath(Connection fromConnection)
  {
    var destinationIndex = fromConnection.Destination;
    var newDirection = PickRandomDirection(fromConnection.m_Direction);

    // TODO:
    //   validate

    var newConnection = new Connection(destinationIndex, newDirection);
    AddHookConnectionToPath(newConnection);

    return newConnection;
  }


  Connection AddTurnRoomDataToPath(Connection fromConnection)
  {
    var destinationIndex = fromConnection.Destination;
    var newDirection = PickRandomDirection(fromConnection.m_Direction);

    // TODO:
    //   validate

    var newConnection = new Connection(destinationIndex, newDirection);
    AddHookConnectionToPath(newConnection);

    return newConnection;
  }


  Connection AddRoomDataToPath(RoomType type, Connection fromConnection = null)
  {
    if (type == RoomType.Starting)
    {
      var direction = PickRandomDirection();
      var startingConnection = new Connection(Vector2Int.zero, direction);

      AddConnectionToPath(startingConnection);
      ++m_TotalLength;
    }
    var destinationIndex = fromConnection.Destination;
    var newDirection = PickRandomDirection(fromConnection.m_Direction);

    // TODO:
    //   validate

    var newConnection = new Connection(destinationIndex, newDirection);
    AddConnectionToPath(newConnection);
    ++m_TotalLength;

    return newConnection;
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


  void AddConnectionToPath(Connection connection)
  {
    // var destinationIndex = connection.Destination;
    // TODO:
    //   validate the destination

    var newRoomData = new RoomData(connection);
    m_Grid.Add(connection.m_Source, newRoomData);
  }


  void AddHookConnectionToPath(Connection connection)
  {
    // var destinationIndex = connection.Destination;
    // TODO:
    //   validate the destination

    var newRoomData = new RoomData(connection);
    newRoomData.m_Tags.m_Hook = true;
    m_Grid.Add(connection.m_Source, newRoomData);
  }


  void AddTurnConnectionToPath(Connection connection)
  {
    // var destinationIndex = connection.Destination;
    // TODO:
    //   validate the destination

    var newRoomData = new RoomData(connection);
    newRoomData.m_Tags.m_Turn = true;
    m_Grid.Add(connection.m_Source, newRoomData);
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


  void BuildGoldenPath(Connection nextConnection)
  {
  }
}


public enum Direction { N, S, E, W, }
