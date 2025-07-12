using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


[RequireComponent(typeof(Character))]
[RequireComponent(typeof(PlayerInput))]
public class BasicAttack : MonoBehaviour
{
  public GameObject m_BulletPrefab;
  public int m_BulletPower = 16;
  public float m_BulletSpeed = 8;
  public int m_MaxBullets = 3;

  Transform m_Tx;
  Character m_Character;
  InputAction m_Attack;

  List<GameObject> m_Bullets = new();

  bool CanAttack { get { return m_Bullets.Count < m_MaxBullets; } }


  void Awake()
  {
    m_Tx = transform;
    m_Character = GetComponent<Character>();

    var playerInput = GetComponent<PlayerInput>();
    m_Attack = playerInput.actions.FindAction("Attack");
  }


  void Start()
  {
    m_Attack.Enable();
  }


  void Update()
  {
    if (m_Attack.triggered)
      AttemptAttack();
  }


  void AttemptAttack()
  {
    BulletCleanup();

    if (!CanAttack) return;

    Attack();
  }


  void Attack()
  {
    var bullet = Instantiate(m_BulletPrefab, m_Tx.position, m_Tx.rotation);
    bullet.GetComponent<Rigidbody2D>().linearVelocity = m_Tx.up * m_BulletSpeed;
    var damageOnCollide = bullet.GetComponent<DamageOnCollide>();
    damageOnCollide.Setup(m_BulletPower, m_Character);
    var destroyOnCollide = bullet.GetComponent<DestroyOnCollide>();
    destroyOnCollide.IgnoreTeam(m_Character);

    m_Bullets.Add(bullet);
  }


  void BulletCleanup()
  {
    var newList = new List<GameObject>();

    foreach (var bullet in m_Bullets)
      if (bullet != null)
        newList.Add(bullet);

    m_Bullets = newList;
  }


  public void OnDied(HealthEventData healthEd)
  {
    m_Attack.Disable();
  }
}
