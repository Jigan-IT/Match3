using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchInfo : MonoBehaviour {

    public List<BoardItem> match;
    public int matchStartingX;
    public int matchEndingX;
    public int matchStartingY;
    public int matchEndingY;

    public bool ValidMatch
    {
        get { return match != null; }
    }
}
