using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(PlayerInput))]
public class TopDownMover : MonoBehaviour
{
  public float m_Speed = 5;

  Transform m_Tx;
  Rigidbody2D m_RB;

  InputAction m_Move;


  void Awake()
  {
    m_Tx = transform;
    m_RB = GetComponent<Rigidbody2D>();

    var playerInput = GetComponent<PlayerInput>();
    m_Move = playerInput.actions.FindAction("Move");
  }


  void Start()
  {
    m_Move.Enable();
  }


  void Update()
  {
    Move();
  }


  void Move()
  {
    var direction = m_Move.ReadValue<Vector2>();
    var normalized = direction.normalized;

    m_RB.linearVelocity = normalized * m_Speed;

    if (direction.sqrMagnitude > 0)
      Face(normalized);
  }


  void Face(Vector2 direction)
  {
    var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
    angle = 360 * Mathf.Round(8 * angle / 360) / 8 - 90;

    m_Tx.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
  }
}
