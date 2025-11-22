using UnityEngine;
[CreateAssetMenu(menuName = "RPG/Element Icon Set")]
public class ElementIconSet : ScriptableObject
{
    public Sprite noneIcon;
    public Sprite fireIcon;
    public Sprite iceIcon;
    public Sprite lightningIcon;
    public Sprite windIcon;
    public Sprite earthIcon;
    public Sprite waterIcon;
    public Sprite lightIcon;
    public Sprite darkIcon;

    public Sprite GetIcon(ElementType element)
    {
        switch (element)
        {
            case ElementType.Fire: return fireIcon;
            case ElementType.Ice: return iceIcon;
            case ElementType.Lightning: return lightningIcon;
            case ElementType.Wind: return windIcon;
            case ElementType.Earth: return earthIcon;
            case ElementType.Water: return waterIcon;
            case ElementType.Light: return lightIcon;
            case ElementType.Dark: return darkIcon;
            case ElementType.None:
            default: return noneIcon;
        }
    }
}