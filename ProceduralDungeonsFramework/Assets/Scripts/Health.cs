using UnityEngine;
using UnityEngine.Events;


public class Health : MonoBehaviour
{
  [System.Serializable]
  public class Events
  {
    public HealthEvent m_DamageRequested;
    public HealthEvent m_TookDamage;
    public HealthEvent m_DealtDamage;
  }

  public Events m_Events;

  [SerializeField]
  int m_MaxHp = 48;
  int m_Hp;
  [SerializeField]
  float m_MercyDuration = 1;
  float m_MercyTimer = 0;

  bool MercyActive
  {
    get
    {
      return m_MercyTimer <= 0;
    }
  }

  bool CanBeDamaged
  {
    get
    {
      return MercyActive;
    }
  }


  void Awake()
  {
    m_Hp = m_MaxHp;
  }


  void Update()
  {
    if (MercyActive)
      MercyUpdate();
  }


  public void OnDamageRequested(HealthEventData healthEd)
  {
    AttemptDamage(healthEd);
  }


  /// <summary>
  /// Semi-public (like, I might expose this function itself, but even if I
  /// don't, it's still the thing that I'll hook up to the event) interface for
  /// receiving damage.
  /// </summary>
  /// <param name="healthEd">Relevant info about the incoming damage.</param>
  void AttemptDamage(HealthEventData healthEd)
  {
    if (!(CanBeDamaged || healthEd.m_IgnoreMercy)) return;

    Damage(healthEd);
  }


  void Damage(HealthEventData healthEd)
  {
    // TODO: Consider a zero check, and maybe something unique happens if you
    //   take zero damage

    m_Hp -= healthEd.m_DamageAmount;

    // TODO: Invoke an event to say this character was just damaged

    if (m_Hp <= 0)
    {
      Die(healthEd);
    }
    else
    {
      BeginMercy();
    }
  }


  void Die(HealthEventData healthEd)
  {
    // Something bad I guess
  }


  void BeginMercy()
  {
    m_MercyTimer = m_MercyDuration;
  }


  void MercyUpdate()
  {
    m_MercyTimer -= Time.deltaTime;

    if (m_MercyTimer <= 0)
      EndMercy();
  }


  void EndMercy()
  {
    // Nothing happens here, for now anyway
  }
}


[System.Serializable]
public class HealthEvent : UnityEvent<HealthEventData> { }

public class HealthEventData
{
  public int m_DamageAmount;
  public bool m_IgnoreMercy;
}
