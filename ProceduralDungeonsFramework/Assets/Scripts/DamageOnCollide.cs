using UnityEngine;


public class DamageOnCollide : MonoBehaviour
{
  public int m_DamageAmount = 1;
  public bool m_Ablative = false;
  public Team m_Team = Team.Neutral;

  Character m_Source = null;


  public void Setup(int damageAmount, Character source = null)
  {
    m_DamageAmount = damageAmount;

    m_Source = source;

    if (source)
      m_Team = source.m_Team;
  }


  void OnCollisionEnter2D(Collision2D collision)
  {
    var otherHealth = collision.gameObject.GetComponent<Health>();
    if (!otherHealth) return;

    var otherCharacter = collision.gameObject.GetComponent<Character>();
    var otherTeam = otherCharacter ? otherCharacter.m_Team : Team.Neutral;
    if (!Character.CanDamage(m_Team, otherTeam)) return;

    var otherHp = otherHealth.CurrentHp;

    var healthEd = new HealthEventData();
    healthEd.m_DamageAmount = m_DamageAmount;
    healthEd.m_Source = m_Source;
    otherHealth.m_Events.m_DamageRequested.Invoke(healthEd);

    if (m_Ablative)
    {
      m_DamageAmount -= otherHp;

      if (m_DamageAmount <= 0)
      {
        Destroy(gameObject);
      }
    }
  }
}
