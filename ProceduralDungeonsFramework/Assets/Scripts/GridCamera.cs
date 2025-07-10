using UnityEngine;

public class GridCamera : MonoBehaviour
{
  public Transform m_FollowTarget;
  public Dungeon m_Dungeon;
  public float m_Interpolant = 0.15f;

  Transform m_Tx;
  Vector2Int m_LatestIndex = Vector2Int.zero;

  Vector3 Destination
  {
    get
    {
      var positionOfTarget = m_LatestIndex * m_Dungeon.m_RoomSize;
      return (Vector3)positionOfTarget + Vector3.forward * m_Tx.position.z;
    }
  }


  void Awake()
  {
    m_Tx = transform;
    m_Dungeon = FindAnyObjectByType<Dungeon>();
  }


  void Update()
  {
    var currentIndex = GetIndexFromPosition(m_FollowTarget.position);

    if (!currentIndex.Equals(m_LatestIndex))
      SetNewIndex(currentIndex);
  }


  void FixedUpdate()
  {
    if (m_FollowTarget == null) return;

    m_Tx.position = Vector3.Lerp(m_Tx.position, Destination, m_Interpolant);
  }


  void SetNewIndex(Vector2Int index)
  {
    m_LatestIndex = index;
  }


  Vector2Int GetIndexFromPosition(Vector3 position)
  {
    var pos = position / m_Dungeon.m_RoomSize;
    return new Vector2Int(Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.y));
  }
}
