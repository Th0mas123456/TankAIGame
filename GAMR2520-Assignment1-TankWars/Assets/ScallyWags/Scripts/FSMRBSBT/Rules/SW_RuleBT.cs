using System;
using System.Collections.Generic;

public class SW_RuleBT
{
    public string antecedentA; //condition1
    public string antecedentB; //condition2
    public Type consequentState;//what state it will turn into
    public Predicate compare; //weather its an And, Or, nAnd 
    public enum Predicate
    { And, Or, nAnd }

    //constructor for the rule
    public SW_RuleBT(string antecedentA, string antecedentB, Type consequentState, Predicate compare)
    {
        this.antecedentA = antecedentA;
        this.antecedentB = antecedentB;
        this.consequentState = consequentState;
        this.compare = compare;
    }

    //performs the check on the rule to see if its true and will return the state or false and return nothing
    public Type CheckRule(Dictionary<string, bool> stats)
    {
        bool antecedentABool = stats[antecedentA];
        bool antecedentBBool = stats[antecedentB];

        switch (compare)
        {
            case Predicate.And:
                if (antecedentABool && antecedentBBool)
                {
                    return consequentState;
                }
                else
                {
                    return null;
                }

            case Predicate.Or:
                if (antecedentABool || antecedentBBool)
                {
                    return consequentState;
                }
                else
                {
                    return null;
                }

            case Predicate.nAnd:
                if (!antecedentABool && !antecedentBBool)
                {
                    return consequentState;
                }
                else
                {
                    return null;
                }
            default:
                return null;
        }
    }
}