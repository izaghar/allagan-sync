using System.Collections.Generic;
using System.Linq;
using Dalamud.Plugin.Services;
using Lumina.Excel.Sheets;

namespace AllaganSync.Services;

public class BardingService
{
    private readonly IDataManager dataManager;
    private readonly IUnlockState unlockState;

    public BardingService(IDataManager dataManager, IUnlockState unlockState)
    {
        this.dataManager = dataManager;
        this.unlockState = unlockState;
    }

    private static bool IsValid(BuddyEquip barding)
    {
        return !barding.Name.IsEmpty;
    }

    public int GetTotalCount()
    {
        var sheet = dataManager.GetExcelSheet<BuddyEquip>();
        return sheet?.Count(IsValid) ?? 0;
    }

    public List<uint> GetUnlockedIds()
    {
        var unlockedIds = new List<uint>();
        var bardingSheet = dataManager.GetExcelSheet<BuddyEquip>();

        if (bardingSheet == null)
            return unlockedIds;

        foreach (var row in bardingSheet)
        {
            if (!IsValid(row))
                continue;

            if (unlockState.IsBuddyEquipUnlocked(row))
            {
                unlockedIds.Add(row.RowId);
            }
        }

        return unlockedIds;
    }
}
