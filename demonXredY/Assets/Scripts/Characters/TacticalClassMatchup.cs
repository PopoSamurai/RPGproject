// Class triangle: Warrior > Archer > Mage > Warrior.
public static class TacticalClassMatchup
{
    public static float Multiplier(TacticalClassData attacker, TacticalClassData defender)
    {
        if (attacker == null || defender == null) return 1f;
        TacticalBaseClass a = attacker.baseClass;
        TacticalBaseClass d = defender.baseClass;
        if (a == d) return 1f;
        bool strong = (a == TacticalBaseClass.Warrior && d == TacticalBaseClass.Archer)
                   || (a == TacticalBaseClass.Archer && d == TacticalBaseClass.Mage)
                   || (a == TacticalBaseClass.Mage && d == TacticalBaseClass.Warrior);
        return strong ? 2f : 0.5f;
    }
}
