using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newQuotesData", menuName = "QuotesData", order = 4)]
public class QuotesScriptableObject : ScriptableObject
{
    public List<string> QuotesList;
}
