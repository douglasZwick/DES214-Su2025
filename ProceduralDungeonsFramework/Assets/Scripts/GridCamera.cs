using UnityEngine;

public class GridCamera : MonoBehaviour
{
  public Transform m_FollowTarget;
  public Dungeon m_Dungeon;
  public float m_Interpolant = 0.15f;
  public Transform m_TunnelRoomRoot;
  // public Transform m_ItemRoomRoot;

  Transform m_Tx;
  Vector2Int m_LatestIndex = Vector2Int.zero;
  bool m_InTunnel = false;

  Vector3 Destination
  {
    get
    {
      var positionOfTarget = m_LatestIndex * m_Dungeon.m_RoomSize;
      return (Vector3)positionOfTarget + Vector3.forward * m_Tx.position.z;
    }
  }

  Vector3 TunnelRoomPosition
  {
    get
    {
      return m_TunnelRoomRoot.position + Vector3.forward * m_Tx.position.z;
    }
  }


  void Awake()
  {
    m_Tx = transform;
    m_Dungeon = FindAnyObjectByType<Dungeon>();

    Dungeon.s_TunnelEntered.AddListener(OnTunnelEntered);
    Dungeon.s_TunnelExited.AddListener(OnTunnelExited);
  }


  void Update()
  {
    if (m_InTunnel) return;

    var currentIndex = GetIndexFromPosition(m_FollowTarget.position);

    if (!currentIndex.Equals(m_LatestIndex))
      SetNewIndex(currentIndex);
  }


  void FixedUpdate()
  {
    if (m_FollowTarget == null) return;
    if (m_InTunnel) return;

    // TODO: Replace this with a more robust action system
    LerpToPosition(Destination, m_Interpolant);
  }


  void OnTunnelEntered(DungeonEventData dungeonED)
  {
    m_InTunnel = true;

    SnapToPosition(TunnelRoomPosition);
  }


  void OnTunnelExited(DungeonEventData dungeonED)
  {
    m_InTunnel = false;

    SnapToIndex(dungeonED.m_DestinationIndex);
  }


  void SetNewIndex(Vector2Int index)
  {
    m_LatestIndex = index;
  }


  void LerpToPosition(Vector3 destination, float interpolant)
  {
    m_Tx.position = Vector3.Lerp(m_Tx.position, destination, interpolant);
  }


  void SnapToPosition(Vector3 destination)
  {
    m_Tx.position = destination;
    var index = GetIndexFromPosition(destination);
    SetNewIndex(index);
  }


  void SnapToIndex(Vector2Int index)
  {
    SetNewIndex(index);
    m_Tx.position = Destination;
  }


  Vector2Int GetIndexFromPosition(Vector3 position)
  {
    var pos = position / m_Dungeon.m_RoomSize;
    return new Vector2Int(Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.y));
  }
}
