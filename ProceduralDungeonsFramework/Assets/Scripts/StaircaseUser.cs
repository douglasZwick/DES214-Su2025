using UnityEngine;


public class StaircaseUser : MonoBehaviour
{
  Transform m_Tx;
  Staircase m_CurrentTarget;


  void Awake()
  {
    m_Tx = transform;
  }


  public void OnProxyTriggerEnter2D(ProxyCollisionEventData proxyCED)
  {
    var stairs = proxyCED.m_Collider2D.gameObject.GetComponent<Staircase>();
    Debug.Log($"Enter {stairs} | Is curr: {stairs == m_CurrentTarget}");

    if (stairs == null) return;
    if (stairs == m_CurrentTarget) return;

    UseStairs(stairs);
  }


  void UseStairs(Staircase stairs)
  {
    Debug.Log($"Using stairs {stairs}");
    m_CurrentTarget = stairs.m_Target;
    m_Tx.position = m_CurrentTarget.m_DestinationTransform.position;
  }


  public void OnProxyTriggerExit2D(ProxyCollisionEventData proxyCED)
  {
    var stairs = proxyCED.m_Collider2D.gameObject.GetComponent<Staircase>();
    Debug.Log($"Exit {stairs} | Is curr: {stairs == m_CurrentTarget}");

    if (stairs != m_CurrentTarget) return;

    m_CurrentTarget = null;
  }
}
