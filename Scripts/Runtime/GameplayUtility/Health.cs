//---------------------------------------------------------------------------//
// Name        - Health.cs
// Author      - Daniel Onstott
// Project     - Lemongrass
// Description - A generic pool of health that can be attached to objects that
//  need to track a status (that is probably health).
//---------------------------------------------------------------------------//
using UnityEngine;

namespace Lemongrass
{
  /**
   * @brief A generic pool of health that can be attached to objects that
   *        need to track a status (that is probably health).
   */
  public class Health : MonoBehaviour
  {
    /**
     * @brief Represents the team that a source of damage is coming from.
     *        can be used to enable/disable friendly fire, etc.
     */
    public enum Team
    {
      Player,
      Enemy,
      Aggressive, // friends with nobody  :'(
      Passive, // friends with everybody  :')
      Unknown // this team will be determined at runtime 
    }

    /**
     * @brief An event that encapsulates an instance of damage occurring. Should
     *        be extended to match the needs of the project
     */
    public struct DamageEvent
    {
      ////////////////////////////////////Variables//////////////////////////////////
      //-public--------------------------------------------------------------------//

      /**
       * @brief The amount of health points lost in this event. Negative would be healing
       */
      public float Amount;

      /**
       * @brief The team that this event originally spawned from.
       */
      public Team OriginatingTeam;

      ////////////////////////////////////Functions//////////////////////////////////
      //-public--------------------------------------------------------------------//

      /**
       * @brief Construct a damage event with the relevant details
       * @param amount The amount of points dealt.
       * @param originTeam The OriginatingTeam for this damage event
       */
      public DamageEvent(float amount, Team originTeam)
      {
        Amount = amount;
        OriginatingTeam = originTeam;
      }
    }

    public delegate void VoidHealthValue(Health health, float amount);

    ////////////////////////////////////Variables//////////////////////////////////
    //-public--------------------------------------------------------------------//
    /**
     * @brief triggers when the health has been reduced to 0
     */
    public event VoidHealthValue Event_HealthDepleted;
    /**
     * @brief triggers when the health is reduced by any amount
     */
    public event VoidHealthValue Event_DamageTaken;
    /**
     * @brief triggers when the health is increased by any amount
     */
    public event VoidHealthValue Event_HealthGained;

    /**
     * @brief Get the characters maximum possible health
     */
    public float MaxHP { get { return maxHP; } }

    /**
    * @brief Get the characters current health
    */
    public float CurrentHP { get { return currentHP; } }

    //-private-------------------------------------------------------------------//
    [SerializeField] private Team team = Team.Passive;
    [SerializeField] private bool invincible = false;

    [SerializeField] private float maxHP = 1.0f;
    private float currentHP = 1.0f;

    ////////////////////////////////////Functions//////////////////////////////////
    //-public--------------------------------------------------------------------//

    /**
     * @brief Returns the team of this health
     * @return The team assigned to this character
     */
    public Team GetTeam()
    {
      return team;
    }

    /**
     * @brief Determine if the given team is friendly
     * @param otherTeam the Team to check against
     * @return True when the team is friendly to this one
     */
    public bool IsFriendly(Team otherTeam)
    {
      if (otherTeam == Team.Aggressive || team == Team.Aggressive) return false;
      if (otherTeam == Team.Passive) return true;
      return otherTeam == team;
    }

    /**
     * @brief Determine if the given health is on a friendly team
     * @param otherHealth the health to check against the team
     * @return True when the health is on a team that is friendly to this one
     */
    public bool IsFriendly(Health otherHealth)
    {
      return IsFriendly(otherHealth.team);
    }

    /**
     * @brief Deals damage to this health if possible given the inputs.
     * @param amount The amount of damage to deal.
     * @param originTeam The team that is dealing the damage.
     * @return True if the damage went through to this health
     */
    public bool Damage(float amount, Team originTeam)
    { 
      return Damage(new DamageEvent(amount, originTeam));
    }

    public bool Heal(float amount, Team originTeam)
    {
      return Heal(new DamageEvent(-amount, originTeam));
    }

    //-private-------------------------------------------------------------------//

    private void Awake()
    {
      ResetHealth();
    }

    private bool Damage(DamageEvent damageEvent)
    {
      if (CurrentHP == 0) return false;
      if (IsFriendly(damageEvent.OriginatingTeam)) return false;

      // we still have to update the value through the server
      UpdateHealth(CurrentHP - damageEvent.Amount);
      return true;
    }

    private bool Heal(DamageEvent healEvent)
    {
      if (CurrentHP == 0) return false;
      if (!IsFriendly(healEvent.OriginatingTeam)) return false;

      UpdateHealth(CurrentHP + healEvent.Amount);
      return true;
    }

    private void UpdateHealth(float newCurrentHP)
    {
      float healthChange = -(currentHP - newCurrentHP);
      if (!invincible) currentHP = Mathf.Max(newCurrentHP, 0);
      BroadcastHealthChanged(healthChange, newCurrentHP);
    }

    private void ResetHealth()
    {
      currentHP = MaxHP;
    }

    private void BroadcastHealthChanged(float amount, float newCurrentHP)
    {
      if (amount < 0) Event_DamageTaken?.Invoke(this, -amount);
      else if (amount > 0) Event_HealthGained?.Invoke(this, amount);

      if (newCurrentHP <= 0) Event_HealthDepleted?.Invoke(this, amount);
    }
  }
}
