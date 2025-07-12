using UnityEngine;


public class StaircaseUser : MonoBehaviour
{
  Transform m_Tx;
  bool m_Active = true;


  void Awake()
  {
    m_Tx = transform;
  }


  public void OnProxyTriggerEnter2D(ProxyCollisionEventData proxyCED)
  {
    if (!m_Active) return;

    var stairs = proxyCED.m_Collider2D.gameObject.GetComponent<Staircase>();

    if (stairs == null) return;

    UseStairs(stairs);
  }


  void UseStairs(Staircase stairs)
  {
    m_Active = false;

    m_Tx.position = stairs.m_DestinationTransform.position;

    Invoke("Reactivate", 0);
  }


  void Reactivate()
  {
    m_Active = true;
  }
}
