using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;


public class PhasedGridDungeonBuilder : MonoBehaviour
{
  enum Phase
  {
    Setup,
    Hook,
    Development1,
    Development2,
    Development3,
    Turn,
    Resolution,
  }

  public List<RoomData> m_RoomPrefabs = new();

  public int m_SetupRoomCount = 3;
  public int m_Dev1RoomCount = 5;
  public int m_Dev2RoomCount = 6;
  public int m_Dev3RoomCount = 7;

  private Dictionary<Vector2Int, RoomData> m_Grid = new();
  private List<RoomData> m_GoldenPath = new();
  private Phase m_CurrentPhase = Phase.Setup;


  void Start()
  {
    BuildGoldenPath();
    BuildExtras();
  }


  void BuildGoldenPath()
  {
    var origin = Vector2Int.zero;
    var direction = PickRandomDirection();
    var startingRoomData = new RoomData(origin);
  }


  void AddStartingRoomToGoldenPath()
  {
    var newRoom = new RoomData(Vector2Int.zero);
    m_Grid.Add(Vector2Int.zero, newRoom);
    m_GoldenPath.Add(newRoom);
  }


  /// <summary>
  /// Adds another room to the golden path, tunneling as needed
  /// </summary>
  void AddNextRoomToGoldenPath()
  {
    // The "current" room is the last one in the list
    var currRoom = m_GoldenPath[^1];
    // The "previous" room is one before the last one. This should never be
    //   null, because we'll manually add the first TWO rooms before we
    //   begin calling this function iteratively.
    var prevRoom = currRoom.m_Prev;

    // Here are all the directions in which we could possibly take our next
    //   step. We start the list with everything except the direction that
    //   points toward the previous room.
    var availableDirections = new List<Direction>();
    var directionToPrev = currRoom.GetDirectionToPrev();
    foreach (var direction in (Direction[])Enum.GetValues(Direction))
      if (direction != directionToPrev)
        availableDirections.Add(direction);

    // As long as we haven't exhausted all our options, keep trying to pick a
    //   direction for the next room.
    while (availableDirections.Count > 0)
    {
      var nextDirection = PickRandom(availableDirections);
      var nextOffset = ConvertDirectionToOffset(nextDirection);

      // If there's already something there,
      if (m_Grid[nextOffset] != null)
      {
        // ...pull that direction out of the list and try again
        availableDirections.Remove(nextDirection);
        continue;
      }

      // At this point, we've found an offset that will work.
    }

    // If we reached the end of the loop without breaking out of it, then it's
    //   because we couldn't find a direction to go in that wouldn't collide
    //   with an existing room. This means it's time to build a tunnel!
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


  static public  Vector2Int ConvertDirectionToOffset(Direction direction)
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


  void BuildExtras()
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
