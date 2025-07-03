using UnityEngine;
using System.Collections.Generic;

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
