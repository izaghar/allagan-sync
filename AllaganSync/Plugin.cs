using Dalamud.Game.Command;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using AllaganSync.Services;
using AllaganSync.UI;

namespace AllaganSync;

public sealed class Plugin : IDalamudPlugin
{
    private const string MainCommand = "/allagansync";

    private readonly IDalamudPluginInterface pluginInterface;
    private readonly ICommandManager commandManager;
    private readonly IPluginLog log;
    private readonly IClientState clientState;
    private readonly IPlayerState playerState;
    private readonly IFramework framework;
    private readonly TitleService titleService;
    private readonly AchievementService achievementService;
    private readonly AllaganSyncService syncService;
    private readonly WindowSystem windowSystem = new("AllaganSync");
    private readonly MainWindow mainWindow;
    private ulong lastContentId;

    public Plugin(
        IDalamudPluginInterface pluginInterface,
        ICommandManager commandManager,
        IPluginLog log,
        IPlayerState playerState,
        IDataManager dataManager,
        IClientState clientState,
        IFramework framework,
        IUnlockState unlockState)
    {
        this.pluginInterface = pluginInterface;
        this.commandManager = commandManager;
        this.log = log;
        this.playerState = playerState;
        this.clientState = clientState;
        this.framework = framework;
        lastContentId = playerState.ContentId;

        var configService = new ConfigurationService(pluginInterface, playerState);
        var orchestrionService = new OrchestrionService(dataManager);
        var emoteService = new EmoteService(dataManager);
        titleService = new TitleService(dataManager, log);
        var mountService = new MountService(dataManager);
        var minionService = new MinionService(dataManager);
        achievementService = new AchievementService(dataManager, log);
        var bardingService = new BardingService(dataManager);
        var tripleTriadCardService = new TripleTriadCardService(dataManager);
        var fashionAccessoryService = new FashionAccessoryService(dataManager);
        var facewearService = new FacewearService(dataManager);
        var vistaService = new VistaService(dataManager);
        var fishService = new FishService(dataManager);
        var blueMageSpellService = new BlueMageSpellService(dataManager, unlockState);
        var characterCustomizationService = new CharacterCustomizationService(dataManager, unlockState);
        syncService = new AllaganSyncService(log, configService, orchestrionService, emoteService, titleService, mountService, minionService, achievementService, bardingService, tripleTriadCardService, fashionAccessoryService, facewearService, vistaService, fishService, blueMageSpellService, characterCustomizationService);

        mainWindow = new MainWindow(configService, syncService);
        windowSystem.AddWindow(mainWindow);

        pluginInterface.UiBuilder.Draw += windowSystem.Draw;
        pluginInterface.UiBuilder.OpenConfigUi += ToggleMainWindow;
        pluginInterface.UiBuilder.OpenMainUi += ToggleMainWindow;
        framework.Update += OnFrameworkUpdate;
        commandManager.AddHandler(MainCommand, new CommandInfo(OnMainCommand)
        {
            HelpMessage = "Open the Allagan Sync window."
        });

        // Request data when character logs in
        clientState.Login += OnLogin;

        // If already logged in, request now
        if (clientState.IsLoggedIn)
            RequestData();

        log.Info("Allagan Sync loaded.");
    }

    private void OnLogin()
    {
        RequestData();
        syncService.RefreshCounts();
    }

    private void OnFrameworkUpdate(IFramework _)
    {
        var currentContentId = playerState.ContentId;
        if (currentContentId == lastContentId)
            return;

        lastContentId = currentContentId;
        if (currentContentId != 0)
        {
            RequestData();
            syncService.RefreshCounts();
        }
    }

    private void RequestData()
    {
        titleService.RequestTitleData();
        achievementService.RequestAchievementData();
    }

    public void ToggleMainWindow()
    {
        var wasOpen = mainWindow.IsOpen;
        mainWindow.Toggle();

        if (!wasOpen && mainWindow.IsOpen)
        {
            RequestData();
            syncService.RefreshCounts();
        }
    }

    private void OnMainCommand(string command, string args)
    {
        OpenMainWindow();
    }

    private void OpenMainWindow()
    {
        if (mainWindow.IsOpen)
            return;

        mainWindow.IsOpen = true;
        RequestData();
        syncService.RefreshCounts();
    }

    public void Dispose()
    {
        clientState.Login -= OnLogin;

        pluginInterface.UiBuilder.Draw -= windowSystem.Draw;
        pluginInterface.UiBuilder.OpenConfigUi -= ToggleMainWindow;
        pluginInterface.UiBuilder.OpenMainUi -= ToggleMainWindow;
        framework.Update -= OnFrameworkUpdate;
        commandManager.RemoveHandler(MainCommand);

        windowSystem.RemoveAllWindows();
        mainWindow.Dispose();

        log.Info("Allagan Sync unloaded.");
    }
}
