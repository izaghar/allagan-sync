using System.Collections.Generic;
using System.Linq;
using Dalamud.Plugin.Services;
using Lumina.Excel.Sheets;

namespace AllaganSync.Services;

public class MountService
{
    private readonly IDataManager dataManager;
    private readonly IUnlockState unlockState;

    public MountService(IDataManager dataManager, IUnlockState unlockState)
    {
        this.dataManager = dataManager;
        this.unlockState = unlockState;
    }

    private static bool IsValid(Mount mount)
    {
        return !mount.Singular.IsEmpty && mount.Order >= 0;
    }

    public int GetTotalCount()
    {
        var sheet = dataManager.GetExcelSheet<Mount>();
        return sheet?.Count(IsValid) ?? 0;
    }

    public List<uint> GetUnlockedIds()
    {
        var unlockedIds = new List<uint>();
        var mountSheet = dataManager.GetExcelSheet<Mount>();

        if (mountSheet == null)
            return unlockedIds;

        foreach (var row in mountSheet)
        {
            if (!IsValid(row))
                continue;

            if (unlockState.IsMountUnlocked(row))
            {
                unlockedIds.Add(row.RowId);
            }
        }

        return unlockedIds;
    }
}
