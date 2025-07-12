using UnityEngine;


public class Staircase : MonoBehaviour
{
  /// <summary>
  /// The transform to go to when this staircase is the destination. Defaults
  /// to this object's transform.
  /// </summary>
  public Transform m_DestinationTransform;
  [HideInInspector]
  public Staircase m_Target;


  void Awake()
  {
    if (m_DestinationTransform == null)
      m_DestinationTransform = transform;
  }
}
