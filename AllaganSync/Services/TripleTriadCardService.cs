using System.Collections.Generic;
using System.Linq;
using Dalamud.Plugin.Services;
using Lumina.Excel.Sheets;

namespace AllaganSync.Services;

public class TripleTriadCardService
{
    private readonly IDataManager dataManager;
    private readonly IUnlockState unlockState;

    public TripleTriadCardService(IDataManager dataManager, IUnlockState unlockState)
    {
        this.dataManager = dataManager;
        this.unlockState = unlockState;
    }

    private static bool IsValid(TripleTriadCard card)
    {
        return !card.Name.IsEmpty;
    }

    public int GetTotalCount()
    {
        var sheet = dataManager.GetExcelSheet<TripleTriadCard>();
        return sheet?.Count(IsValid) ?? 0;
    }

    public List<uint> GetUnlockedIds()
    {
        var unlockedIds = new List<uint>();
        var cardSheet = dataManager.GetExcelSheet<TripleTriadCard>();

        if (cardSheet == null)
            return unlockedIds;

        foreach (var row in cardSheet)
        {
            if (!IsValid(row))
                continue;

            if (unlockState.IsTripleTriadCardUnlocked(row))
            {
                unlockedIds.Add(row.RowId);
            }
        }

        return unlockedIds;
    }
}
