using UnityEngine;

public class MoveConstantSpeed : IMovable
{
    private GameObject projectile;
    private float speed;

    public MoveConstantSpeed(GameObject projectile, float speed)
    {
        this.projectile = projectile;
        this.speed = speed;
    }

    public void Move(Vector3 target)
    {
        if (!projectile)
            return;

        var direction = (target - projectile.transform.position).normalized;
        projectile.transform.position += direction * speed * Time.deltaTime;
    }
}
