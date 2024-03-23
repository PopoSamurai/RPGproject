using UnityEngine;
public enum EquipmentType
{
    Weampon,
    Weampon2Hand,
    Armor,
    Boots,
    Helmet,
    Cloat,
    Ring,
    Belt,
    Shield,
    Neackle,
    Potion,
    Material
}
[CreateAssetMenu]
public class EquippableItem : Item
{
    public int STR; //strength si³a atk
    public int VIT; //vitality ¿ywotnoœæ/hp
    public int INT; //intelligence ilosc many
    public int AGL; //agility ilosc staminy
    public int DEF; //defend unik/obrona
    [Space] //buff %
    public float STRbonus;
    public float VITbonus;
    public float INTbonus;
    public float AGLbonus;
    public float DEFbonus;
    public EquipmentType type;
}
