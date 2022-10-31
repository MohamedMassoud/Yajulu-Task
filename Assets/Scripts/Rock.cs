using UnityEngine;

[CreateAssetMenu(fileName = "New Rock", menuName = "ScriptableObjects/Rock", order = 1)]
public class Rock : ScriptableObject
{
    public enum RockType
    {
        Normal,
        Fire,
        Frost
    }
    public GameObject rockPrefab;
    public GameObject shatteredRockPrefab;
    public RockType rockType;
    public float rockSpeed;
    public float rockDmg;
    public float rockTemp;
    public float rockRotationSpeed;
    
}