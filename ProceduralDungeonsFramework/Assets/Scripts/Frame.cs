using UnityEngine;

public class Frame : MonoBehaviour
{
  [Header("Blocks")]
  public GameObject m_BlockN;
  public GameObject m_BlockS;
  public GameObject m_BlockE;
  public GameObject m_BlockW;


  public void BlockSetupForDoorData(RoomData.DoorData doorData)
  {
    m_BlockN.SetActive(!doorData.m_N);
    m_BlockS.SetActive(!doorData.m_S);
    m_BlockE.SetActive(!doorData.m_E);
    m_BlockW.SetActive(!doorData.m_W);
  }
}
