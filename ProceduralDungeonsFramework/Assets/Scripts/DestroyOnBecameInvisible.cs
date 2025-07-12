using UnityEngine;


public class DestroyOnBecameInvisible : MonoBehaviour
{
  public float m_Delay = 0;


  void OnBecameInvisible()
  {
    if (m_Delay <= 0)
    {
      DestroySelf();

      return;
    }

    // TODO: Somehow I mistrust Invoke as a long-term application, but I don't
    //   exactly know why. Look into whether there's an alternative way of doing
    //   what I want to do here
    Invoke("DestroySelf", m_Delay);
  }


  void DestroySelf()
  {
    Destroy(gameObject);
  }
}
