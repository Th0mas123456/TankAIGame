using System.Collections.Generic;

public class SW_RulesBT
{
    //creates list of all the rules added
    public void AddRule(SW_RuleBT rule)
    {
        GetRules.Add(rule);
    }
    public List<SW_RuleBT> GetRules { get; } = new List<SW_RuleBT>();
}
