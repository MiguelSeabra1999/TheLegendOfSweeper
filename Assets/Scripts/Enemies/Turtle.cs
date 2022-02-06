using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turtle : Enemy
{
   [SerializeField] private GameObject RockPrefab;

    protected override void Start()
    {
       
        base.Start();

    }
    protected override void OnEnable()
    {
        base.OnEnable();
        TryGoForward();
       StartCoroutine(ShootRoutine());
    }
    protected override void Update()
    {
        base.Update();

          

    }
    protected override void FinishedMoving()
    {
        base.FinishedMoving();
        TryGoForward();
    }

    private void Shoot()
    {
        _audioManager.PlaySFX(3);
        GameObject rock = Instantiate(RockPrefab,transform.position + DirectionToVec(_movementDir)*0.5f, Quaternion.identity);
        Hitbox rockHitbox = rock.GetComponent<Hitbox>();
        Projectile rockProjectile = rock.GetComponent<Projectile>();
        rockProjectile.Direction = DirectionToVec(_movementDir);
        rockHitbox._direction = _movementDir;
    }
    private IEnumerator ShootRoutine()
    {
        while(true)
        {
            yield return new WaitForSeconds(Random.Range(1f,3f));
            Shoot();
        }
    }
}

