using UnityEngine;
[CreateAssetMenu(fileName = "Character", menuName = "new character")]
public class Character : ScriptableObject
{
    public int maxHp;
    public int hpValue;
    public Sprite portrait;
    public string charName;
}