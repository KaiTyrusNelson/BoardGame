using System.Collections;
using System.Collections.Generic;
/// <Ability> the ability class are special abilities which the pllayer can choose to activate
public abstract class Ability
{
    public abstract void AbilityCast(List<Character> c);
}