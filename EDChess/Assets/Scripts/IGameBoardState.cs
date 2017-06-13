using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public interface IGameBoardState
{
    ISpaceState GetSpaceState(int level, int row, int col);
    int GetNumLevels();
    int GetNumRows();
    int GetNumCols();
    void Move(IPieceState piece, ISpaceState space);

}
