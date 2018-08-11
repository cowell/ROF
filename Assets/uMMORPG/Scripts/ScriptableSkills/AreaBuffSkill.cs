// Area heal that heals all entities of same type in cast range
// => player heals players in cast range
// => monster heals monsters in cast range
//
// Based on BuffSkillTemplate so it can be added to Buffs list.
using UnityEngine;
using UnityEngine.Networking;

[CreateAssetMenu(menuName="uMMORPG Skill/Area Buff", order=999)]
public class AreaBuffSkill : BuffSkill
{
    public override bool CheckTarget(Entity caster)
    {
        // no target necessary, but still set to self so that LookAt(target)
        // doesn't cause the player to look at a target that doesn't even matter
        caster.target = caster;
        return true;
    }

    public override bool CheckDistance(Entity caster, int skillLevel, out Vector2 destination)
    {
        // can cast anywhere
        destination = caster.transform.position;
        return true;
    }

    public override void Apply(Entity caster, int skillLevel)
    {
        // find all entities of same type in castRange around the caster
        Collider2D[] colliders = Physics2D.OverlapCircleAll(caster.transform.position, castRange.Get(skillLevel));
        foreach (Collider2D co in colliders)
        {
            Entity candidate = co.GetComponentInParent<Entity>();
            if (candidate != null && candidate.GetType() == caster.GetType())
            {
                // can't buff dead people
                if (candidate.health > 0)
                {
                    // add buff or replace if already in there
                    candidate.AddOrRefreshBuff(new Buff(this, skillLevel));

                    // show effect on target
                    SpawnEffect(caster, candidate);
                }
            }
        }
    }
}
