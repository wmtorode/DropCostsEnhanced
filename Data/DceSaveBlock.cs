using System.Collections.Generic;

namespace DropCostsEnhanced.Data;

public class DceSaveBlock
{
    public int version = 1;
    public int fixedDifficulty = 0;
    public bool initialized = false;
    public List<int> previousDropCosts = new List<int>();
    public int currentIndex = 0;

    public void ClearForNewGame()
    {
        fixedDifficulty = 0;
        previousDropCosts.Clear();
        initialized = false;
        currentIndex = 0;
    }
    
}