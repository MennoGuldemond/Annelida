using System.Collections.Generic;

public class HeroModel
{
    public string Name { get; set; }
    public float HitPoints { get; set; }
    public int ActionPoints { get; set; }
    public float Speed { get; set; }
    public float AimPower { get; set; }

    // TODO: add max and min angle range?
    // public List<Ability> Abilities { get; set; }
}