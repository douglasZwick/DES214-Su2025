using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Enum = System.Enum;


public class PhasedGridDungeonBuilder : MonoBehaviour
{
  public List<RoomData> m_RoomPrefabs = new();

  public int m_SetupRoomCount = 3;
  public int m_Dev1RoomCount = 5;
  public int m_Dev2RoomCount = 6;
  public int m_Dev3RoomCount = 7;
  public int m_ResolutionCount = 1;

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

    
  }


  /// <summary>
  /// "Carving" is the process of placing room SLOTS as opposed to rooms
  /// </summary>
  void CarveGoldenPath()
  {
    CarveStartingSlot();

    while (m_GoldenPath.Count < TargetPathLength)
      CarveNextSlot();
  }


  RoomData CarveStartingSlot()
  {
    var newSlot = new RoomData(Vector2Int.zero);
    return Carve(newSlot, Vector2Int.zero);
  }


  /// <summary>
  /// Adds another slot to the golden path, tunneling as needed
  /// </summary>
  RoomData CarveNextSlot()
  {
    // The "current" slot is the last one in the list
    var currSlot = m_GoldenPath[^1];

    // Here are all the directions in which we could possibly take our next
    //   step. We start the list with everything except the direction that
    //   points toward the previous slot.
    // Note that the previous slot should never be null, because we manually
    //   carve the first TWO slots before we begin calling this function
    //   iteratively
    var availableDirections = new List<Direction>();
    var directionToPrev = currSlot.GetDirectionToPrev();
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

      // Since the actual offset will never be zero, we can use Vector2Int.zero
      //   to indicate that we haven't already overwritten it
      if (firstPickedOffset == Vector2Int.zero)
        firstPickedOffset = nextOffset;

      var nextIndex = currSlot.m_Index + nextOffset;

      // If there's already something there,
      if (m_Grid[nextIndex] != null)
      {
        // ...pull that direction out of the list and try again
        availableDirections.Remove(nextDirection);
        continue;
      }

      // At this point, we've found an index that will work
      return ConnectAlongGoldenPath(currSlot, nextIndex);
    } while (availableDirections.Count > 0);

    // If we reached the end of the loop without returning, then it's because
    //   we couldn't find a direction to go in that wouldn't collide with an
    //   existing room. This means it's time to build a tunnel!
    var tunnelDestinationIndex = currSlot.m_Index + firstPickedOffset;

    // The tunnel will go in the direction of our first pick, which we've saved
    //   from above. To find the destination, we'll just walk the grid in that
    //   direction until we find a vacant spot.
    while (m_Grid[tunnelDestinationIndex] != null)
      tunnelDestinationIndex += firstPickedOffset;

    // At this point, we've found an index that will work
    return ConnectByTunnel(currSlot, tunnelDestinationIndex);
  }


  RoomData ConnectAlongGoldenPath(RoomData from, Vector2Int destIndex)
  {
    var dest = new RoomData(destIndex);
    dest.m_Prev = from;
    from.m_Next = dest;

    return Carve(dest, destIndex);
  }


  RoomData ConnectByTunnel(RoomData from, Vector2Int destIndex)
  {
    var dest = ConnectAlongGoldenPath(from, destIndex);
    dest.m_Tags.m_Tunnel = true;

    return dest;
  }


  RoomData Carve(RoomData newSlot, Vector2Int index)
  {
    // TODO: should I validate here? Anywhere??

    m_GoldenPath.Add(newSlot);
    m_Grid.Add(index, newSlot);

    // Carving a new slot in the golden path makes it longer, and the 
    //   CurrentPhase getter uses the length of the path to determine the 
    //   current phase, so it updates automatically
    newSlot.m_Phase = CurrentPhase;

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
