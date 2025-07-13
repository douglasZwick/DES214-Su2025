using UnityEngine;

public class Frame : MonoBehaviour
{
  [Header("Blocks")]
  public GameObject m_BlockN;
  public GameObject m_BlockS;
  public GameObject m_BlockE;
  public GameObject m_BlockW;

  [Header("Stairs")]
  public Staircase m_StaircaseN;
  public Staircase m_StaircaseS;
  public Staircase m_StaircaseE;
  public Staircase m_StaircaseW;

  [HideInInspector]
  public RoomData m_RoomData;


  public void Setup(RoomData roomData)
  {
    m_RoomData = roomData;

    BlockSetup();
    StairSetup();
  }


  void BlockSetup()
  {
    m_BlockN.SetActive(!m_RoomData.m_Doors.m_N);
    m_BlockS.SetActive(!m_RoomData.m_Doors.m_S);
    m_BlockE.SetActive(!m_RoomData.m_Doors.m_E);
    m_BlockW.SetActive(!m_RoomData.m_Doors.m_W);
  }


  void StairSetup()
  {
    var stairs = m_RoomData.m_Stairs;

    if (!(stairs.m_N || stairs.m_S || stairs.m_E || stairs.m_W)) return;

    m_StaircaseN.gameObject.SetActive(stairs.m_N);
    m_StaircaseS.gameObject.SetActive(stairs.m_S);
    m_StaircaseE.gameObject.SetActive(stairs.m_E);
    m_StaircaseW.gameObject.SetActive(stairs.m_W);

    var next = m_RoomData.m_Next;
    if (next != null)
      ConnectStairs(next);

    var prev = m_RoomData.m_Prev;
    if (prev != null)
      ConnectStairs(prev);
  }


  void ConnectStairs(RoomData dest)
  {
    var currIndex = m_RoomData.m_Index;
    var destIndex = dest.m_Index;
    var indexDiff = destIndex - currIndex;

    // N-S
    if (indexDiff.x == 0)
    {
      // Going north -- N on this, S on next
      if (indexDiff.y > 1)
      {
        var a = m_StaircaseN;
        var b = dest.m_Frame.m_StaircaseS;
        Staircase.Connect(a, b);
      }
      else if (indexDiff.x < -1)
      {
        var a = m_StaircaseS;
        var b = dest.m_Frame.m_StaircaseN;
        Staircase.Connect(a, b);
      }
    }
    else  // E-W
    {
      // Going east -- E on this, W on next
      if (indexDiff.x > 1)
      {
        var a = m_StaircaseE;
        var b = dest.m_Frame.m_StaircaseW;
        Staircase.Connect(a, b);
      }
      else if (indexDiff.x < -1)
      {
        var a = m_StaircaseW;
        var b = dest.m_Frame.m_StaircaseE;
        Staircase.Connect(a, b);
      }
    }
  }
}
