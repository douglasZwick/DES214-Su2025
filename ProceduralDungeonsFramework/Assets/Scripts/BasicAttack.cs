using System.Collections.Generic;
using System.Linq;
using UnityEngine;


[RequireComponent(typeof(HeroStats))]
public class BasicAttack : MonoBehaviour
{
  public BulletLogic m_BulletPrefab;
  public float m_BulletSpeed = 8;
  public int m_MaxBullets = 3;
  public List<KeyCode> m_FireKeys = new() { KeyCode.Space, KeyCode.Z, };

  Transform m_Tx;
  HeroStats m_HeroStats;
  List<BulletLogic> m_Bullets = new();

  bool CanAttack { get { return m_Bullets.Count < m_MaxBullets; } }
  bool FireKeyDown
  {
    get { return m_FireKeys.Any(key => Input.GetKeyDown(key)); }
  }


  void Awake()
  {
    m_Tx = transform;
    m_HeroStats = GetComponent<HeroStats>();
  }


  void Update()
  {
    if (FireKeyDown)
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
    bullet.Power = m_HeroStats.Power;

    m_Bullets.Add(bullet);
  }


  void BulletCleanup()
  {
    var newList = new List<BulletLogic>();

    foreach (var bullet in m_Bullets)
      if (bullet != null)
        newList.Add(bullet);

    m_Bullets = newList;
  }
}
