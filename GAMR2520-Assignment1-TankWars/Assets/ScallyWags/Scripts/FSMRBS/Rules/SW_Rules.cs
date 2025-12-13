using System.Collections.Generic;

public class SW_Rules
{
    //creates list of all the rules added
    public void AddRule(SW_Rule rule)
    {
        GetRules.Add(rule);
    }
    public List<SW_Rule> GetRules { get; } = new List<SW_Rule>();
}