using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public interface ISpaceState
{
    bool IsOccupied();
    IPieceState Occupier();
    int GetLevel();
    int GetRow();
    int GetCol();
    SpaceState CreateSpaceState();
}

 