using System.Collections.Generic;
using System.Linq;
using Dalamud.Plugin.Services;
using Lumina.Excel.Sheets;

namespace AllaganSync.Services;

public class FashionAccessoryService
{
    private readonly IDataManager dataManager;
    private readonly IUnlockState unlockState;

    public FashionAccessoryService(IDataManager dataManager, IUnlockState unlockState)
    {
        this.dataManager = dataManager;
        this.unlockState = unlockState;
    }

    private static bool IsValid(Ornament ornament)
    {
        if (ornament.Singular.IsEmpty)
            return false;

        // Transient 200-299 = old glasses (converted to GlassesStyle)
        if (ornament.Transient >= 200 && ornament.Transient < 300)
            return false;

        return ornament.Order >= 0;
    }

    public int GetTotalCount()
    {
        var sheet = dataManager.GetExcelSheet<Ornament>();
        return sheet?.Count(IsValid) ?? 0;
    }

    public List<uint> GetUnlockedIds()
    {
        var unlockedIds = new List<uint>();
        var ornamentSheet = dataManager.GetExcelSheet<Ornament>();

        if (ornamentSheet == null)
            return unlockedIds;

        foreach (var row in ornamentSheet)
        {
            if (!IsValid(row))
                continue;

            if (unlockState.IsOrnamentUnlocked(row))
            {
                unlockedIds.Add(row.RowId);
            }
        }

        return unlockedIds;
    }
}
