using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Enum = System.Enum;
using System.Text;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;


[RequireComponent(typeof(Dungeon))]
public class PhasedGridDungeonBuilder : MonoBehaviour
{
  public enum DebuggingLevel
  {
    Off,
    Basic,
    Detail,
    Verbose,
  }

  public List<Frame> m_FramePrefabs = new();
  public List<Room> m_InteriorPrefabs = new();

  // TODO: Create a custom editor for this class, which will let me edit this
  //   dictionary in the inspector
  public Dictionary<ArcPhase, int> m_RoomCounts = new()
  {
    { ArcPhase.Setup, 4 },
    { ArcPhase.Hook, 1 },
    { ArcPhase.Development1, 8 },
    { ArcPhase.Development2, 12 },
    { ArcPhase.Development3, 10 },
    { ArcPhase.Turn, 3 },
    { ArcPhase.Resolution, 1 },
  };

  public DebuggingLevel m_DebuggingLevel = DebuggingLevel.Off;
  public bool m_EnsureTunnel = false;

  Dictionary<Vector2Int, RoomData> m_Grid;
  Vector2 m_RoomSize;
  List<RoomData> m_GoldenPath = new();
  List<TunnelData> m_Tunnels = new();
  bool m_ContainsTunnel = false;

  ArcPhase CurrentPhase
  {
    get
    {
      var runningTotal = 0;
      var roomCount = m_GoldenPath.Count;

      foreach (var kvp in m_RoomCounts)
      {
        runningTotal += kvp.Value;

        if (roomCount <= runningTotal)
          return kvp.Key;
      }

      Error("Error in determining current phase: reached the end of the " +
        "m_RoomCounts dictionary and we still haven't placed " +
       $"{roomCount} rooms! Returning ArcPhase.Resolution by default.");

      return ArcPhase.Resolution;
    }
  }
  

  int ComputeTargetPathLength()
  {
    return m_RoomCounts.Values.Sum();
  }


  void Awake()
  {
    var dungeon = GetComponent<Dungeon>();
    m_Grid = dungeon.m_Grid;
    m_RoomSize = dungeon.m_RoomSize;

    dungeon.e_BuildRequest += OnBuildRequest;
  }


  void OnBuildRequest()
  {
    CarveGoldenPath();
    // CarveExtras();

    PrintGrid();

    BuildRooms();

    if (m_EnsureTunnel && !m_ContainsTunnel)
      SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
  }


  /// <summary>
  /// "Carving" is the process of placing room SLOTS as opposed to rooms
  /// </summary>
  void CarveGoldenPath()
  {
    Log("Carving golden path...");

    CarveStartingSlots();

    var targetPathLength = ComputeTargetPathLength();

    while (m_GoldenPath.Count < targetPathLength)
      CarveNextSlot();
  }


  /// <summary>
  /// Carves and connects two slots, so that the golden path will be guaranteed
  ///   to have more than one item in it, which is needed for CarveNextSlot to
  ///   iterate properly.
  /// </summary>
  void CarveStartingSlots()
  {
    Log($"  Carving starting slot...", DebuggingLevel.Detail);
    Log($"    Carving at {Vector2Int.zero}", DebuggingLevel.Detail);
    Log($"      Sweeping the floor...", DebuggingLevel.Verbose);

    var newSlot = new RoomData(Vector2Int.zero);
    var startingRoom = Carve(newSlot, Vector2Int.zero);

    var nextDirection = PickRandomDirection();
    var nextOffset = ConvertDirectionToOffset(nextDirection);

    Log($"    Carving second slot at {nextOffset}", DebuggingLevel.Detail);

    ConnectAlongGoldenPath(startingRoom, nextOffset);
  }


  /// <summary>
  /// Adds another slot to the golden path, tunneling as needed
  /// </summary>
  RoomData CarveNextSlot()
  {
    // The "current" slot is the last one in the list
    var currSlot = m_GoldenPath[^1];
    Log($"  Carving slot after {currSlot.m_Index}...", DebuggingLevel.Detail);

    // Here are all the directions in which we could possibly take our next
    //   step. We start the list with everything except the direction that
    //   points toward the previous slot.
    // Note that the previous slot should never be null, because we manually
    //   carve the first TWO slots before we begin calling this function
    //   iteratively
    var availableDirections = new List<Direction>();
    var directionToPrev = currSlot.GetDirectionToPrev();
    Log($"    (Can't go {directionToPrev})", DebuggingLevel.Verbose);
    foreach (var direction in (Direction[])Enum.GetValues(typeof(Direction)))
      if (direction != directionToPrev)
        availableDirections.Add(direction);

    // As long as we haven't exhausted all our options, keep trying to pick a
    //   direction for the next slot.
    // 
    // We need to hold onto the offset from the first pick we make, because if
    //   we get to the end of the loop and we haven't found a direction we can
    //   use, then we'll need a tunnel, and we should put it in the direction
    //   we first chose.
    var firstPickedOffset = Vector2Int.zero;

    do
    {
      var nextDirection = PickRandom(availableDirections);
      var nextOffset = ConvertDirectionToOffset(nextDirection);

      Log($"    Trying {nextDirection}:", DebuggingLevel.Verbose);

      // Since the actual offset will never be zero, we can use Vector2Int.zero
      //   to indicate that we haven't already overwritten it
      if (firstPickedOffset == Vector2Int.zero)
        firstPickedOffset = nextOffset;

      var nextIndex = currSlot.m_Index + nextOffset;

      // If there's already something there,
      if (m_Grid.TryGetValue(nextIndex, out _))
      {
        Log($"      {nextDirection} failed.", DebuggingLevel.Verbose);

        // ...pull that direction out of the list and try again
        availableDirections.Remove(nextDirection);
        continue;
      }

      Log($"      {nextDirection} succeeded!", DebuggingLevel.Verbose);

      // At this point, we've found an index that will work
      return ConnectAlongGoldenPath(currSlot, nextIndex);
    } while (availableDirections.Count > 0);

    Log($"    All directions from {currSlot.m_Index} failed. Tunneling:",
      DebuggingLevel.Detail);

    // If we reached the end of the loop without returning, then it's because
    //   we couldn't find a direction to go in that wouldn't collide with an
    //   existing room. This means it's time to build a tunnel!
    var tunnelDestinationIndex = currSlot.m_Index + firstPickedOffset;

    // The tunnel will go in the direction of our first pick, which we've saved
    //   from above. To find the destination, we'll just walk the grid in that
    //   direction until we find a vacant spot.
    while (m_Grid.TryGetValue(tunnelDestinationIndex, out _))
    {
      Log($"      Can't stop at {tunnelDestinationIndex}",
        DebuggingLevel.Verbose);

      tunnelDestinationIndex += firstPickedOffset;
    }

    Log($"      Found vacant spot at {tunnelDestinationIndex}",
      DebuggingLevel.Verbose);

    var tunnelDirection = ConvertOffsetToDirection(firstPickedOffset);

    // At this point, we've found an index that will work
    return ConnectByTunnel(currSlot, tunnelDestinationIndex, tunnelDirection);
  }


  RoomData ConnectAlongGoldenPath(RoomData from, Vector2Int destIndex,
    bool isTunnel = false)
  {
    var fromIndex = from.m_Index;

    Log($"      Connecting from {fromIndex} to {destIndex}:",
      DebuggingLevel.Verbose);

    var dest = new RoomData(destIndex);
    dest.m_Prev = from;
    from.m_Next = dest;

    var indexDiff = destIndex - fromIndex;
    var direction = ConvertOffsetToDirection(indexDiff);
    
    var (destConnector, fromConnector) = isTunnel ?
      (dest.m_Stairs, from.m_Stairs) : (dest.m_Doors, from.m_Doors);

    switch (direction)
    {
      case Direction.N:
        destConnector.m_S = true;
        fromConnector.m_N = true;
        break;
      case Direction.S:
        destConnector.m_N = true;
        fromConnector.m_S = true;
        break;
      case Direction.E:
        destConnector.m_W = true;
        fromConnector.m_E = true;
        break;
      default:  // W
        destConnector.m_E = true;
        fromConnector.m_W = true;
        break;
    }

    return Carve(dest, destIndex);
  }


  RoomData ConnectByTunnel(RoomData from, Vector2Int destIndex,
    Direction direction)
  {
    m_ContainsTunnel = true;
    var roomNum = m_GoldenPath.FindIndex(room => room == from);
    Log($"TUNNEL BUILT connecting rooms {roomNum} and {roomNum + 1}",
      DebuggingLevel.Detail);
    m_Tunnels.Add(new TunnelData(from, destIndex, direction));

    var dest = ConnectAlongGoldenPath(from, destIndex, true);
    from.m_Tags.m_Tunnel = true;
    dest.m_Tags.m_Tunnel = true;

    return dest;
  }


  RoomData Carve(RoomData newSlot, Vector2Int index)
  {
    Log($"        Carving new slot at {index}...",
      DebuggingLevel.Verbose);

    // TODO: should I validate here? Anywhere??

    m_GoldenPath.Add(newSlot);
    m_Grid.Add(index, newSlot);

    // Carving a new slot in the golden path makes it longer, and the 
    //   CurrentPhase getter uses the length of the path to determine the 
    //   current phase, so it updates automatically
    newSlot.m_Phase = CurrentPhase;

    Log($"          {CurrentPhase} slot carved.",
      DebuggingLevel.Verbose);

    return newSlot;
  }


  static public Direction OppositeDirection(Direction direction)
  {
    return direction switch
    {
      Direction.N => Direction.S,
      Direction.S => Direction.N,
      Direction.E => Direction.W,
      _ => Direction.E,
    };
  }


  static public Vector2Int ConvertDirectionToOffset(Direction direction)
  {
    return direction switch
    {
      Direction.N => Vector2Int.up,
      Direction.S => Vector2Int.down,
      Direction.E => Vector2Int.right,
      _ => Vector2Int.left,
    };
  }


  static public Direction ConvertOffsetToDirection(Vector2Int offset)
  {
    return offset.x == 0 ?
      offset.y > 0 ? Direction.N : Direction.S :
      offset.x > 0 ? Direction.E : Direction.W;
  }


  void CarveExtras()
  {

  }


  void BuildRooms()
  {
    // PROBLEM:
    //   In this loop, we create the current room's frame. In the current
    //   implementation, for staircase linkage, we also need to access the next
    //   room's frame. But, of course, that frame doesn't exist yet, because we
    //   haven't made it yet (it'll be next in the loop).
    //
    // SOLUTION 1:
    //   First create the first two frames outside of a loop, and then loop
    //   through the rest of the rooms, linking the "current" room to its
    //   predecessor, and linking that predecessor to ITS predecessor.
    // SOLUTION 2:
    //   Same as solution 1, except we do the whole thing in a loop, including
    //   the first two frames, and we just do null checks on the predecessors.

    foreach (var kvp in m_Grid)
    {
      var index = kvp.Key;
      var roomData = kvp.Value;
      var roomPosition2d = index * m_RoomSize;
      var roomPosition = new Vector3(roomPosition2d.x, roomPosition2d.y);

      // For now we're just using the one and only frame prefab, but we'll
      //   replace this later
      var frame = Instantiate(m_FramePrefabs[0],
        roomPosition, Quaternion.identity);

      roomData.m_Frame = frame;
      frame.Setup(roomData);

      if (roomData.m_Tags.m_Tunnel)
        frame.StairSetup();
    }

    foreach (var tunnel in m_Tunnels)
    {
      // TODO: Make it so that the function that connects rooms via tunnel also
      //   passes directions into the new Tunnel object it adds to m_Tunnels.
      //   Then, make this loop connect the tunnels based on that direction
      var dir = tunnel.m_Direction;
      var opp = OppositeDirection(dir);
      var fromFrame = m_Grid[tunnel.m_FromIndex].m_Frame;
      var toFrame = m_Grid[tunnel.m_ToIndex].m_Frame;
      var fromStairs = fromFrame.GetStaircaseFromDirection(dir);
      var toStairs = toFrame.GetStaircaseFromDirection(opp);
      Staircase.Connect(fromStairs, toStairs);
    }
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


  void PrintGrid()
  {
    if (!DebuggingAt(DebuggingLevel.Basic)) return;

    var minXIndex = int.MaxValue;
    var maxXIndex = int.MinValue;
    var minYIndex = int.MaxValue;
    var maxYIndex = int.MinValue;

    foreach (var kvp in m_Grid)
    {
      if (kvp.Key.x < minXIndex)
        minXIndex = kvp.Key.x;
      else if (kvp.Key.x > maxXIndex)
        maxXIndex = kvp.Key.x;

      if (kvp.Key.y < minYIndex)
        minYIndex = kvp.Key.y;
      else if (kvp.Key.y > maxYIndex)
        maxYIndex = kvp.Key.y;
    }

    var gridW = maxXIndex - minXIndex + 1;
    var gridH = maxYIndex - minYIndex + 1;
    var xShift = minXIndex < 0 ? -minXIndex : 0;
    var yShift = minYIndex < 0 ? -minYIndex : 0;

    var gridAsArray = new string[gridH][];
    for (var y = 0; y < gridH; ++y)
    {
      var row = new string[gridW];
      System.Array.Fill(row, ". ");
      gridAsArray[y] = row;
    }

    foreach (var kvp in m_Grid)
    {
      var x = kvp.Key.x + xShift;
      var y = kvp.Key.y + yShift;
      var gridString = GetGridStringFromRoomData(kvp.Value);
      gridAsArray[y][x] = gridString;
    }

    var outSb = new StringBuilder();
    foreach (var row in gridAsArray)
      outSb.AppendLine(string.Join("", row));

    var outStr = $"Grid W: {gridW}, H: {gridH}" + System.Environment.NewLine +
      outSb.ToString();
    Debug.Log(outStr);
  }


  string GetGridStringFromRoomData(RoomData roomData)
  {
    var str = roomData.m_Phase switch
    {
      ArcPhase.Setup => "<color=#ffffff>S </color>",
      ArcPhase.Hook => "<color=#80c0ff>H </color>",
      ArcPhase.Development1 => "<color=#80ffc0>D1</color>",
      ArcPhase.Development2 => "<color=#f0e020>D2</color>",
      ArcPhase.Development3 => "<color=#f0a040>D3</color>",
      ArcPhase.Turn => "<color=#f02040>T </color>",
      _ => "<color=#a020e0>R </color>",
    };

    if (roomData.IsStart || roomData.IsEnd)
    {
      str = $"<b>{str}</b>";
    }

    return str;
  }


  void Log(object message, DebuggingLevel level = DebuggingLevel.Basic)
  {
    if (DebuggingAt(level))
      Debug.Log(message);
  }


  void Warn(object message, DebuggingLevel level = DebuggingLevel.Basic)
  {
    if (DebuggingAt(level))
      Debug.LogWarning(message);
  }


  void Error(object message, DebuggingLevel level = DebuggingLevel.Basic)
  {
    if (DebuggingAt(level))
      Debug.LogError(message);
  }


  bool DebuggingAt(DebuggingLevel level)
  {
    return (int)m_DebuggingLevel >= (int)level;
  }
}


public enum Direction { N, S, E, W, }


public enum ArcPhase
{
  Setup,
  Hook,
  Development1,
  Development2,
  Development3,
  Turn,
  Resolution,
}


[System.Serializable]
public class TunnelData
{
  public Vector2Int m_FromIndex;
  public Vector2Int m_ToIndex;
  public Direction m_Direction;

  public TunnelData() { }

  public TunnelData(Vector2Int fromIndex, Vector2Int toIndex,
    Direction direction)
  {
    m_FromIndex = fromIndex;
    m_ToIndex = toIndex;
    m_Direction = direction;
  }

  public TunnelData(Vector2Int fromIndex, RoomData toRoom,
    Direction direction)
  {
    m_FromIndex = fromIndex;
    m_ToIndex = toRoom.m_Index;
    m_Direction = direction;
  }

  public TunnelData(RoomData fromRoom, Vector2Int toIndex,
    Direction direction)
  {
    m_FromIndex = fromRoom.m_Index;
    m_ToIndex = toIndex;
    m_Direction = direction;
  }

  public TunnelData(RoomData fromRoom, RoomData toRoom,
    Direction direction)
  {
    m_FromIndex = fromRoom.m_Index;
    m_ToIndex = toRoom.m_Index;
    m_Direction = direction;
  }
}
