using UnityEngine;
using UnityEngine.Events;


public class Health : MonoBehaviour
{
  [System.Serializable]
  public class Events
  {
    public HealthEvent m_DamageRequested;
    public HealthEvent m_TookDamage;
    public HealthEvent m_Died;
  }

  public Events m_Events;

  [SerializeField]
  int m_MaxHp = 48;
  int m_Hp;
  [SerializeField]
  float m_MercyDuration = 1;
  float m_MercyTimer = 0;

  public int CurrentHp { get { return m_Hp; }}

  // TODO: Probably add some way to check whether you're already dead, because I
  //   foresee trouble without that

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
    healthEd.m_Target = this;

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

    healthEd.m_PreviousHp = m_Hp;
    m_Hp -= healthEd.m_DamageAmount;
    healthEd.m_NewHp = m_Hp;

    m_Events.m_TookDamage.Invoke(healthEd);
    healthEd.m_Source?.m_Events.m_DealtDamage.Invoke(healthEd);

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

    m_Events.m_Died.Invoke(healthEd);
    healthEd.m_Source?.m_Events.m_ScoredKill.Invoke(healthEd);
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
  public int m_PreviousHp;
  public int m_NewHp;
  public bool m_IgnoreMercy;
  public Health m_Target;
  public Character m_Source;
  // public GameObject m_Instrument;
}
