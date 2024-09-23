using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QeenBoss : MonoBehaviour
{
    [SerializeField] private GameObject _Weapon;
    [SerializeField] private Cooldown cooldown;
    [SerializeField]private GunController[] gunControllers ;

    private void Start()
    {
        gunControllers = _Weapon.GetComponentsInChildren<GunController>();
    }

    void Update()
    {
        _Weapon.transform.rotation = Quaternion.Euler(0, _Weapon.transform.eulerAngles.y+ Time.deltaTime * 300, 0);
        if (!cooldown.IsCoolingDown)
        {
            foreach (var gun in gunControllers)
            {
                gun.Attack(1);
            }
            cooldown.StartCooldown();
        }
    }
}
