using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class Character : MonoBehaviour
{
  static public Dictionary<Team, Dictionary<Team, bool>> s_DamageTable = new()
  {
    {
      Team.Neutral, new()
      {
        { Team.Neutral, false },
        { Team.Player, false },
        { Team.Enemy, true },
      }
    },
    {
      Team.Player, new()
      {
        { Team.Neutral, false },
        { Team.Player, false },
        { Team.Enemy, true },
      }
    },
    {
      Team.Enemy, new()
      {
        { Team.Neutral, true },
        { Team.Player, true },
        { Team.Enemy, false },
      }
    },
  };

  static public Dictionary<Team, Dictionary<Team, bool>> s_InteractTable = new()
  {
    {
      Team.Neutral, new()
      {
        { Team.Neutral, false },
        { Team.Player, false },
        { Team.Enemy, false },
      }
    },
    {
      Team.Player, new()
      {
        { Team.Neutral, true },
        { Team.Player, true },
        { Team.Enemy, true },
      }
    },
    {
      Team.Enemy, new()
      {
        { Team.Neutral, false },
        { Team.Player, false },
        { Team.Enemy, false },
      }
    },
  };

  [System.Serializable]
  public class Events
  {
    public HealthEvent m_ScoredKill;
    public HealthEvent m_DealtDamage;
  }

  public Events m_Events;

  public Team m_Team;


  static public bool CanDamage(Team lhs, Team rhs)
  {
    var rhsTable = s_DamageTable.GetValueOrDefault(lhs, null);

    if (rhsTable == null) return false;

    return rhsTable.GetValueOrDefault(rhs, false);
  }


  static public bool CanInteract(Team lhs, Team rhs)
  {
    var rhsTable = s_InteractTable.GetValueOrDefault(lhs, null);

    if (rhsTable == null) return false;

    return rhsTable.GetValueOrDefault(rhs, false);
  }
}


public enum Team
{
  Neutral,
  Player,
  Enemy,
}
