using System.Collections.Generic;
using System.Linq;
using Dalamud.Plugin.Services;
using Lumina.Excel.Sheets;

namespace AllaganSync.Services;

public class OrchestrionService
{
    private readonly IDataManager dataManager;
    private readonly IUnlockState unlockState;

    public OrchestrionService(IDataManager dataManager, IUnlockState unlockState)
    {
        this.dataManager = dataManager;
        this.unlockState = unlockState;
    }

    private static bool IsValid(Orchestrion orchestrion)
    {
        return !orchestrion.Name.IsEmpty;
    }

    public int GetTotalCount()
    {
        var sheet = dataManager.GetExcelSheet<Orchestrion>();
        return sheet?.Count(IsValid) ?? 0;
    }

    public List<uint> GetUnlockedIds()
    {
        var unlockedIds = new List<uint>();
        var orchestrionSheet = dataManager.GetExcelSheet<Orchestrion>();

        if (orchestrionSheet == null)
            return unlockedIds;

        foreach (var row in orchestrionSheet)
        {
            if (!IsValid(row))
                continue;

            if (unlockState.IsOrchestrionUnlocked(row))
            {
                unlockedIds.Add(row.RowId);
            }
        }

        return unlockedIds;
    }
}
