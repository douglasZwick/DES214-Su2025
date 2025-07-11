using System.Collections.Generic;
using UnityEngine;


public class DestroyOnCollide : MonoBehaviour
{
  public HashSet<GameObject> m_ObjectsToIgnore = new();
  public HashSet<Team> m_TeamsToIgnore = new();


  public void IgnoreTeam(Team team)
  {
    m_TeamsToIgnore.Add(team);
  }


  public void IgnoreTeam(Character character)
  {
    m_TeamsToIgnore.Add(character.m_Team);
  }


  public void IgnoreObject(GameObject obj)
  {
    m_ObjectsToIgnore.Add(obj);
  }


  bool IsIgnoring(Character character)
  {
    return character ? m_TeamsToIgnore.Contains(character.m_Team) : false;
  }


  bool IsIgnoring(GameObject obj)
  {
    return m_ObjectsToIgnore.Contains(obj);
  }


  void OnCollisionEnter2D(Collision2D collision)
  {
    CollisionHandler(collision.gameObject);
  }


  void OnTriggerEnter2D(Collider2D collision)
  {
    CollisionHandler(collision.gameObject);
  }


  void CollisionHandler(GameObject other)
  {
    if (IsIgnoring(other)) return;

    var character = other.GetComponent<Character>();

    if (IsIgnoring(character)) return;

    Destroy(gameObject);
  }
}
