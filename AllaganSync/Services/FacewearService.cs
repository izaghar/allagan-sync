using System.Collections.Generic;
using System.Linq;
using Dalamud.Plugin.Services;
using Lumina.Excel.Sheets;

namespace AllaganSync.Services;

public class FacewearService
{
    private readonly IDataManager dataManager;
    private readonly IUnlockState unlockState;

    public FacewearService(IDataManager dataManager, IUnlockState unlockState)
    {
        this.dataManager = dataManager;
        this.unlockState = unlockState;
    }

    private bool IsValid(GlassesStyle glassesStyle)
    {
        var firstGlassesId = glassesStyle.Glasses.FirstOrDefault().RowId;
        return IsValidGlasses(firstGlassesId);
    }

    private bool IsValidGlasses(uint glassesId)
    {
        if (glassesId == 0)
            return false;

        var glassesSheet = dataManager.GetExcelSheet<Glasses>();
        if (glassesSheet == null)
            return false;

        var glasses = glassesSheet.GetRowOrDefault(glassesId);
        return glasses != null && !glasses.Value.Name.IsEmpty;
    }

    public int GetTotalCount()
    {
        var sheet = dataManager.GetExcelSheet<GlassesStyle>();
        return sheet?.Count(IsValid) ?? 0;
    }

    public List<uint> GetUnlockedIds()
    {
        var unlockedIds = new List<uint>();
        var glassesStyleSheet = dataManager.GetExcelSheet<GlassesStyle>();
        var glassesSheet = dataManager.GetExcelSheet<Glasses>();

        if (glassesStyleSheet == null || glassesSheet == null)
            return unlockedIds;

        foreach (var row in glassesStyleSheet)
        {
            if (!IsValid(row))
                continue;

            var glassesId = row.Glasses.FirstOrDefault().RowId;
            if (!glassesSheet.TryGetRow(glassesId, out var glassesRow))
                continue;

            var isUnlocked = unlockState.IsGlassesUnlocked(glassesRow);

            if (isUnlocked)
            {
                unlockedIds.Add(row.RowId);
            }
        }

        return unlockedIds;
    }
}
