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
  public Staircase m_StaircaseN;
  public Staircase m_StaircaseS;
  public Staircase m_StaircaseE;
  public Staircase m_StaircaseW;

  public List<Staircase> m_Stairs = new();

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
    var tunnelConnections = m_RoomData.m_TunnelConnections;
    var staircaseExceptions = new List<Staircase>();

    for (var i = 0; i < tunnelConnections.Count; ++i)
    {
      var otherRoom = tunnelConnections[i];
      var staircase = SelectStaircase(staircaseExceptions);
      staircaseExceptions.Add(staircase);

      // TODO: Pick it up here I guess
    }
  }


  public Staircase GetStaircaseFromDirection(Direction direction)
  {
    return direction switch
    {
      Direction.N => m_StaircaseN,
      Direction.S => m_StaircaseS,
      Direction.E => m_StaircaseE,
      _ => m_StaircaseW,
    };
  }


  Staircase SelectStaircase(List<Staircase> exceptions)
  {
    var candidates = m_Stairs.Except(exceptions).ToList();
    var index = Random.Range(0, candidates.Count);

    return candidates[index];
  }
}
