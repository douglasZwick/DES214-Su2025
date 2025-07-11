using UnityEngine;


public class DestroyOnDied : MonoBehaviour
{
  public void OnDied(HealthEventData healthEd)
  {
    Destroy(gameObject);
  }
}
