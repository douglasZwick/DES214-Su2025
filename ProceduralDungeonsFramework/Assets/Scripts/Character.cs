using UnityEngine;


public class Character : MonoBehaviour
{
  public Team m_Team;
}


public enum Team
{
  Neutral,
  Enemy,
  Player,
}
