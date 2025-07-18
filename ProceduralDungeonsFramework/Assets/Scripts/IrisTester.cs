using UnityEngine;
using UnityEngine.InputSystem;


[RequireComponent(typeof(PlayerInput))]
public class IrisTester : MonoBehaviour
{
  InputAction m_Iris;

  bool m_Open = true;


  void Awake()
  {
    var playerInput = GetComponent<PlayerInput>();
    m_Iris = playerInput.actions.FindAction("Iris");
  }


  void Start()
  {
    m_Iris.Enable();
  }


  void Update()
  {
    if (m_Iris.triggered)
      ToggleIris();
  }


  void ToggleIris()
  {
    if (m_Open)
      CloseRequest();
    else
      OpenRequest();
  }


  void CloseRequest()
  {
    m_Open = false;

    var irisED = new IrisEventData();
    irisED.m_WorldPoint = transform.position;
    IrisTransition.s_CloseRequest.Invoke(irisED);
  }


  void OpenRequest()
  {
    m_Open = true;

    var irisED = new IrisEventData();
    irisED.m_WorldPoint = transform.position;
    IrisTransition.s_OpenRequest.Invoke(irisED);
  }
}
