using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Frame : MonoBehaviour
{
  [Header("Blocks")]
  public GameObject m_BlockN;
  public GameObject m_BlockS;
  public GameObject m_BlockE;
  public GameObject m_BlockW;

  [Header("Stairs")]
  public List<Staircase> m_Stairs = new();

  [HideInInspector]
  public RoomData m_RoomData;


  public void Setup(RoomData roomData)
  {
    m_RoomData = roomData;

    for (var i = 0; i < m_Stairs.Count; ++i)
      m_Stairs[i].Setup(m_RoomData, i);

    m_BlockN.SetActive(!m_RoomData.m_Doors.m_N);
    m_BlockS.SetActive(!m_RoomData.m_Doors.m_S);
    m_BlockE.SetActive(!m_RoomData.m_Doors.m_E);
    m_BlockW.SetActive(!m_RoomData.m_Doors.m_W);
  }


  public Staircase SelectAndActivateStaircase()
  {
    var candidates = m_Stairs.Where(stair =>
      !stair.gameObject.activeSelf).ToList();

    if (candidates.Count <= 0)
    {
      Debug.LogError($"Frame {name} tried to select a staircase, " +
        "but there were no unused staircases left!");
    }

    var index = Random.Range(0, candidates.Count);
    var staircase = candidates[index];
    staircase.gameObject.SetActive(true);

    return staircase;
  }
}
