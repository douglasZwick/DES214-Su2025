using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class TopDownMover : MonoBehaviour
{
  public float m_Speed = 5;

  private Transform m_Tx;
  private Rigidbody2D m_RB;


  void Awake()
  {
    m_Tx = transform;
    m_RB = GetComponent<Rigidbody2D>();
  }


  void Update()
  {
    Move();
  }


  void Move()
  {
    var direction = Vector2.right * Input.GetAxisRaw("Horizontal") +
                    Vector2.up * Input.GetAxisRaw("Vertical");
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
