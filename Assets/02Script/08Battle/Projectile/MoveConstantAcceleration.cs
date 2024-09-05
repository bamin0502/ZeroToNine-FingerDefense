using UnityEngine;

public class MoveConstantAcceleration : IMovable
{
    private GameObject projectile;
    private float initialSpeed;
    private float acceleration;

    private float currentSpeed;

    public MoveConstantAcceleration(GameObject projectile, float initialSpeed, float acceleration)
    {
        this.projectile = projectile;
        this.initialSpeed = initialSpeed;
        this.acceleration = acceleration;

        currentSpeed = initialSpeed;
    }

    public void Move(Vector3 target)
    {
        if (!projectile)
            return;

        currentSpeed += acceleration * Time.deltaTime;
        
        var direction = (target - projectile.transform.position).normalized;
        projectile.transform.position += direction * currentSpeed * Time.deltaTime;
    }
}
