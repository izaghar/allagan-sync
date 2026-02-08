using System.Collections.Generic;
using System.Linq;
using Dalamud.Plugin.Services;
using Lumina.Excel.Sheets;

namespace AllaganSync.Services;

public class EmoteService
{
    private const uint MaelstromCompanyId = 1;
    private const uint TwinAdderCompanyId = 2;
    private const uint ImmortalFlamesCompanyId = 3;

    private const uint FlameSaluteEmoteId = 57;
    private const uint SerpentSaluteEmoteId = 56;
    private const uint StormSaluteEmoteId = 55;

    // Salute GC company emotes: do not treat UnlockLink == 0 as auto-unlocked.
    private static readonly HashSet<uint> DefaultUnlockExceptions = new()
    {
        FlameSaluteEmoteId,
        SerpentSaluteEmoteId,
        StormSaluteEmoteId
    };

    private readonly IDataManager dataManager;
    private readonly IUnlockState unlockState;
    private readonly IPlayerState playerState;

    public EmoteService(IDataManager dataManager, IUnlockState unlockState, IPlayerState playerState)
    {
        this.dataManager = dataManager;
        this.unlockState = unlockState;
        this.playerState = playerState;
    }

    private static bool IsValid(Emote emote)
    {
        return !emote.Name.IsEmpty && emote.Order > 0;
    }

    private bool IsGrandCompanySaluteUnlocked(Emote emote)
    {
        var currentCompanyId = playerState.GrandCompany.RowId;
        if (currentCompanyId == 0)
            return unlockState.IsEmoteUnlocked(emote);

        return emote.RowId switch
        {
            FlameSaluteEmoteId => currentCompanyId == ImmortalFlamesCompanyId,
            SerpentSaluteEmoteId => currentCompanyId == TwinAdderCompanyId,
            StormSaluteEmoteId => currentCompanyId == MaelstromCompanyId,
            _ => unlockState.IsEmoteUnlocked(emote)
        };
    }

    private bool IsUnlocked(Emote emote)
    {
        // Default emotes often have no unlock link and should count as collected.
        if (emote.UnlockLink == 0)
        {
            if (DefaultUnlockExceptions.Contains(emote.RowId))
                return IsGrandCompanySaluteUnlocked(emote);

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
