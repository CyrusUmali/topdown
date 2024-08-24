using UnityEngine;

public class WeaponDetails : MonoBehaviour
{
    public enum WeaponType
    {
        Sword,
        Bow
    }

    public WeaponType weaponType;
    public int damageAmount;
    public float attackSpeed;
    public float range;
    public float arrowSpeed; // Only relevant for Bow
    public float cooldown;

}
