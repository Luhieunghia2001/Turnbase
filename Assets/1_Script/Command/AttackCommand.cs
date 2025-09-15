using System.Collections;
using UnityEngine;

public class AttackCommand : SkillCommand
{
    private float moveSpeed = 5f;

    public AttackCommand(Character user, Character target, Skill skill)
        : base(user, target, skill) { }

    public override IEnumerator Execute()
    {
        Vector3 initialPosition = user.initialPosition;
        float attackDistance = 1.5f;
        float direction = Mathf.Sign(target.transform.position.x - user.transform.position.x);
        Vector3 destination = target.transform.position - new Vector3(direction * attackDistance, 0, 0);

        // Chạy đến target
        user.animator.Play("Run");
        user.animator.SetBool("IsRunning", true);
        while (Vector3.Distance(user.transform.position, destination) > 0.1f)
        {
            user.transform.position = Vector3.MoveTowards(user.transform.position, destination, moveSpeed * Time.deltaTime);
            yield return null;
        }

        // Attack
        user.animator.SetBool("IsRunning", false);
        user.animator.SetTrigger("Attack");
        yield return new WaitForSeconds(1f);

        // Gây damage
        target.TakeDamage(skill.damage);
        yield return new WaitForSeconds(1.5f);

        // Quay về
        user.animator.SetBool("IsRunning", true);
        while (Vector3.Distance(user.transform.position, initialPosition) > 0.1f)
        {
            user.transform.position = Vector3.MoveTowards(user.transform.position, initialPosition, moveSpeed * Time.deltaTime);
            yield return null;
        }
        user.animator.SetBool("IsRunning", false);
    }
}
