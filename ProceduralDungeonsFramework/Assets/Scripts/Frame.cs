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


  public void BlockSetupForDoorData(RoomData.DoorData doorData)
  {
    m_BlockN.SetActive(!doorData.m_N);
    m_BlockS.SetActive(!doorData.m_S);
    m_BlockE.SetActive(!doorData.m_E);
    m_BlockW.SetActive(!doorData.m_W);
  }


  public void StairSetupForRoomData(RoomData roomData)
  {
    var stairs = roomData.m_Stairs;

    m_StaircaseN.gameObject.SetActive(stairs.m_N);
    m_StaircaseS.gameObject.SetActive(stairs.m_S);
    m_StaircaseE.gameObject.SetActive(stairs.m_E);
    m_StaircaseW.gameObject.SetActive(stairs.m_W);

    // TODO: We need to connect this room's stairs to its prev/next stairs, but
    //   we gotta determine which staircase to use for each of them
  }
}
