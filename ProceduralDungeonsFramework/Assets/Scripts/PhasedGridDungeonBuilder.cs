using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Enum = System.Enum;
using System.Text;
using System;
using Random = UnityEngine.Random;


public class PhasedGridDungeonBuilder : MonoBehaviour
{
  public enum DebuggingLevel
  {
    Off,
    Basic,
    Detail,
    Verbose,
  }

  public List<RoomData> m_RoomPrefabs = new();

  public int m_SetupRoomCount = 3;
  public int m_Dev1RoomCount = 5;
  public int m_Dev2RoomCount = 6;
  public int m_Dev3RoomCount = 7;
  public int m_ResolutionCount = 1;

  public DebuggingLevel m_DebuggingLevel = DebuggingLevel.Off;

  private Dictionary<Vector2Int, RoomData> m_Grid = new();
  private List<RoomData> m_GoldenPath = new();
  private ArcPhase CurrentPhase
  {
    get
    {
      var phaseLength = m_SetupRoomCount;

      if (m_GoldenPath.Count <= phaseLength)
        return ArcPhase.Setup;

      ++phaseLength;

      if (m_GoldenPath.Count <= phaseLength)
        return ArcPhase.Hook;

      phaseLength += m_Dev1RoomCount;

      if (m_GoldenPath.Count <= phaseLength)
        return ArcPhase.Development1;

      phaseLength += m_Dev2RoomCount;

      if (m_GoldenPath.Count <= phaseLength)
        return ArcPhase.Development2;

      phaseLength += m_Dev3RoomCount;

      if (m_GoldenPath.Count <= phaseLength)
        return ArcPhase.Development3;

      ++phaseLength;

      if (m_GoldenPath.Count <= phaseLength)
        return ArcPhase.Turn;

      return ArcPhase.Resolution;
    }
  }

  private int TargetPathLength
  {
    get
    {
      return m_SetupRoomCount + // For the setup
             1 +                // For the hook
             m_Dev1RoomCount +  // For development 1
             m_Dev2RoomCount +  // For development 2
             m_Dev3RoomCount +  // For development 3
             1 +                // For the turn
             m_ResolutionCount; // For the resolution
    }
  }


  void Start()
  {
    CarveGoldenPath();
    CarveExtras();

    PrintGrid();
  }


  /// <summary>
  /// "Carving" is the process of placing room SLOTS as opposed to rooms
  /// </summary>
  void CarveGoldenPath()
  {
    Log("Carving golden path...");

    CarveStartingSlots();

    while (m_GoldenPath.Count < TargetPathLength)
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

    // At this point, we've found an index that will work
    return ConnectByTunnel(currSlot, tunnelDestinationIndex);
  }


  RoomData ConnectAlongGoldenPath(RoomData from, Vector2Int destIndex)
  {
    Log($"      Connecting from {from.m_Index} to {destIndex}:",
      DebuggingLevel.Verbose);

    var dest = new RoomData(destIndex);
    dest.m_Prev = from;
    from.m_Next = dest;

    return Carve(dest, destIndex);
  }


  RoomData ConnectByTunnel(RoomData from, Vector2Int destIndex)
  {
    Log($"      Connecting from {from.m_Index} to {destIndex}:",
      DebuggingLevel.Verbose);

    var dest = ConnectAlongGoldenPath(from, destIndex);
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

    var gridW = maxXIndex - minXIndex;
    var gridH = maxYIndex - minYIndex;
    var xShift = minXIndex < 0 ? -minXIndex : 0;
    var yShift = minYIndex < 0 ? -minYIndex : 0;

    var outputSb = new StringBuilder();

    for (var y = 0; y < gridH; ++y)
      for (var x = 0; x < gridW; ++x)
        outputSb.Append("▪️");

    foreach (var kvp in m_Grid)
    {
      var x = kvp.Key.x + xShift;
      var y = kvp.Key.y + yShift;
      var offset = x + y * (gridW + 1);
      var roomChar = (kvp.Value.m_Phase switch
      {
        ArcPhase.Setup        => "⬜",
        ArcPhase.Hook         => "🟦",
        ArcPhase.Development1 => "🟩",
        ArcPhase.Development2 => "🟨",
        ArcPhase.Development3 => "🟧",
        ArcPhase.Turn         => "🟥",
        _                     => "🟪",
      })[0];
      outputSb[offset] = roomChar;
    }

    for (var y = gridH - 1; y > 0; --y)
    {
      var index = gridW * y;
      outputSb.Insert(index, Environment.NewLine);
    }

    outputSb.Append(Environment.NewLine);

    var outputStr = "Grid string:" + Environment.NewLine + outputSb.ToString();
    Debug.Log(outputStr);
    Debug.Log($"gridW: {gridW} | gridH: {gridH}");
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
