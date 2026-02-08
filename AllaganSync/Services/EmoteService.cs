using System.Collections.Generic;
using System.Linq;
using Dalamud.Plugin.Services;
using Lumina.Excel.Sheets;

namespace AllaganSync.Services;

public class EmoteService
{
    // Salute GC company emotes: do not treat UnlockLink == 0 as auto-unlocked.
    private static readonly HashSet<uint> DefaultUnlockExceptions = new()
    {
        55,
        56,
        57
    };

    private readonly IDataManager dataManager;
    private readonly IUnlockState unlockState;

    public EmoteService(IDataManager dataManager, IUnlockState unlockState)
    {
        this.dataManager = dataManager;
        this.unlockState = unlockState;
    }

    private static bool IsValid(Emote emote)
    {
        return !emote.Name.IsEmpty && emote.Order > 0;
    }

    private bool IsUnlocked(Emote emote)
    {
        // Default emotes often have no unlock link and should count as collected.
        if (emote.UnlockLink == 0)
        {
            if (DefaultUnlockExceptions.Contains(emote.RowId))
                return unlockState.IsEmoteUnlocked(emote);

            return true;
        }

        return unlockState.IsEmoteUnlocked(emote);
    }

    public int GetTotalCount()
    {
        var sheet = dataManager.GetExcelSheet<Emote>();
        return sheet?.Count(IsValid) ?? 0;
    }

    public List<uint> GetUnlockedIds()
    {
        var unlockedIds = new List<uint>();
        var emoteSheet = dataManager.GetExcelSheet<Emote>();

        if (emoteSheet == null)
            return unlockedIds;

        foreach (var row in emoteSheet)
        {
            if (!IsValid(row))
                continue;

            if (IsUnlocked(row))
            {
                unlockedIds.Add(row.RowId);
            }
        }

        return unlockedIds;
    }
}
