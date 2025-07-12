using UnityEngine;
using UnityEngine.Events;


public class CollisionProxy : MonoBehaviour
{
  [System.Serializable]
  public class Events
  {
    public ProxyCollisionEvent m_CollisionEnter;
    public ProxyCollisionEvent m_CollisionStay;
    public ProxyCollisionEvent m_CollisionExit;
    public ProxyCollisionEvent m_TriggerEnter;
    public ProxyCollisionEvent m_TriggerStay;
    public ProxyCollisionEvent m_TriggerExit;
    public ProxyCollisionEvent m_CollisionEnter2D;
    public ProxyCollisionEvent m_CollisionStay2D;
    public ProxyCollisionEvent m_CollisionExit2D;
    public ProxyCollisionEvent m_TriggerEnter2D;
    public ProxyCollisionEvent m_TriggerStay2D;
    public ProxyCollisionEvent m_TriggerExit2D;
  }

  public Events m_Events;


  void OnCollisionEnter(Collision collision)
  {
    var proxyCED = new ProxyCollisionEventData(collision);
    m_Events.m_CollisionEnter.Invoke(proxyCED);
  }


  void OnCollisionStay(Collision collision)
  {
    var proxyCED = new ProxyCollisionEventData(collision);
    m_Events.m_CollisionStay.Invoke(proxyCED);
  }


  void OnCollisionExit(Collision collision)
  {
    var proxyCED = new ProxyCollisionEventData(collision);
    m_Events.m_CollisionExit.Invoke(proxyCED);
  }


  void OnTriggerEnter(Collider other)
  {
    var proxyCED = new ProxyCollisionEventData(other);
    m_Events.m_TriggerEnter.Invoke(proxyCED);
  }


  void OnTriggerStay(Collider other)
  {
    var proxyCED = new ProxyCollisionEventData(other);
    m_Events.m_TriggerStay.Invoke(proxyCED);
  }


  void OnTriggerExit(Collider other)
  {
    var proxyCED = new ProxyCollisionEventData(other);
    m_Events.m_TriggerExit.Invoke(proxyCED);
  }
  

  void OnCollisionEnter2D(Collision2D collision)
  {
    var proxyCED = new ProxyCollisionEventData(collision);
    m_Events.m_CollisionEnter2D.Invoke(proxyCED);
  }


  void OnCollisionStay2D(Collision2D collision)
  {
    var proxyCED = new ProxyCollisionEventData(collision);
    m_Events.m_CollisionStay2D.Invoke(proxyCED);
  }


  void OnCollisionExit2D(Collision2D collision)
  {
    var proxyCED = new ProxyCollisionEventData(collision);
    m_Events.m_CollisionExit2D.Invoke(proxyCED);
  }


  void OnTriggerEnter2D(Collider2D other)
  {
    var proxyCED = new ProxyCollisionEventData(other);
    m_Events.m_TriggerEnter2D.Invoke(proxyCED);
  }


  void OnTriggerStay2D(Collider2D other)
  {
    var proxyCED = new ProxyCollisionEventData(other);
    m_Events.m_TriggerStay2D.Invoke(proxyCED);
  }


  void OnTriggerExit2D(Collider2D other)
  {
    var proxyCED = new ProxyCollisionEventData(other);
    m_Events.m_TriggerExit2D.Invoke(proxyCED);
  }
}


[System.Serializable]
public class ProxyCollisionEvent : UnityEvent<ProxyCollisionEventData> { }

public class ProxyCollisionEventData
{
  public Collision m_Collision;
  public Collider m_Collider;
  public Collision2D m_Collision2D;
  public Collider2D m_Collider2D;


  public ProxyCollisionEventData() { }

  public ProxyCollisionEventData(Collision collision)
  {
    m_Collision = collision;
    m_Collider = collision.collider;
  }

  public ProxyCollisionEventData(Collider other)
  {
    m_Collider = other;
  }

  public ProxyCollisionEventData(Collision2D collision)
  {
    m_Collision2D = collision;
    m_Collider2D = collision.collider;
  }

  public ProxyCollisionEventData(Collider2D other)
  {
    m_Collider2D = other;
  }
}
