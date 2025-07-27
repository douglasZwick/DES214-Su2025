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

    /// PICK IT UP HERE:
    /// I dunno maybe I should just use a static public function on Dungeon
    /// instead of the event I'm using here.
    /// 
    /// In any case, next I need to start the staircase transition process,
    /// which I figure will look something like this:
    /// 0. Set up the ins and outs of the tunnel room
    /// 1. Send an event to the Hero to disable its player input and possibly
    ///    do an animation for going down the stairs
    /// 2. Send an event to the IrisTransition to begin closing the iris
    /// 3. When the iris is done, teleport the hero to the tunnel room
    /// 4. Send an event to the Hero to make it animate into the room
    /// 5. Send an event to the IrisTransition to begin opening the iris
    /// 6. When the iris is done, send an event to the Hero to reenable input

    m_CurrentTarget = stairs.m_Target;
    m_Tx.position = m_CurrentTarget.m_LandingPointTransform.position;
  }


  public void OnProxyTriggerExit2D(ProxyCollisionEventData proxyCED)
  {
    var stairs = proxyCED.m_Collider2D.gameObject.GetComponent<Staircase>();
    Debug.Log($"Exit {stairs} | Is curr: {stairs == m_CurrentTarget}");

    if (stairs != m_CurrentTarget) return;

    m_CurrentTarget = null;
  }
}
