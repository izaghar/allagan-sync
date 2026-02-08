using System.Collections.Generic;
using System.Linq;
using Dalamud.Plugin.Services;
using Lumina.Excel.Sheets;

namespace AllaganSync.Services;

public class MinionService
{
    private readonly IDataManager dataManager;
    private readonly IUnlockState unlockState;

    public MinionService(IDataManager dataManager, IUnlockState unlockState)
    {
        this.dataManager = dataManager;
        this.unlockState = unlockState;
    }

    private static bool IsValid(Companion minion)
    {
        return !minion.Singular.IsEmpty;
    }

    public int GetTotalCount()
    {
        var sheet = dataManager.GetExcelSheet<Companion>();
        return sheet?.Count(IsValid) ?? 0;
    }

    public List<uint> GetUnlockedIds()
    {
        var unlockedIds = new List<uint>();
        var companionSheet = dataManager.GetExcelSheet<Companion>();

        if (companionSheet == null)
            return unlockedIds;

        foreach (var row in companionSheet)
        {
            if (!IsValid(row))
                continue;

            if (unlockState.IsCompanionUnlocked(row))
            {
                unlockedIds.Add(row.RowId);
            }
        }

        return unlockedIds;
    }
}
