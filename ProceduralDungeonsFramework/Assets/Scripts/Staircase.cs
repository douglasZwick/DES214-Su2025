using UnityEngine;


public class Staircase : MonoBehaviour
{
  /// <summary>
  /// The transform to go to when this staircase is the destination. Defaults
  /// to this object's transform.
  /// </summary>
  public Transform m_LandingPointTransform;
  [HideInInspector]
  public Staircase m_Target;
  [HideInInspector]
  public int m_StairIndex = -1;

  /// <summary>
  /// The room containing this staircase.
  /// </summary>
  RoomData m_FromRoom;
  /// <summary>
  /// The room containing the target staircase.
  /// </summary>
  RoomData m_ToRoom;

  public RoomData FromRoom { get => m_FromRoom; }
  public RoomData ToRoom { get => m_ToRoom; }
  public Frame FromFrame { get => m_FromRoom.m_Frame; }
  public Frame ToFrame { get => m_ToRoom.m_Frame; }
  public Vector2Int FromIndex { get => m_FromRoom.m_Index; }
  public Vector2Int ToIndex { get => m_ToRoom.m_Index; }


  void Awake()
  {
    if (m_LandingPointTransform == null)
      m_LandingPointTransform = transform;
  }


  public void Setup(RoomData fromRoom, int stairIndex)
  {
    m_FromRoom = fromRoom;
    m_StairIndex = stairIndex;
  }


  static public void Connect(Staircase a, Staircase b)
  {
    a.m_Target = b;
    b.m_Target = a;
  }
}
