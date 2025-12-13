using System.Collections.Generic;

public class SW_Rules
{
    public void AddRule(SW_Rule rule)
    {
        GetRules.Add(rule);
    }
    public List<SW_Rule> GetRules { get; } = new List<SW_Rule>();
}