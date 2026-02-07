# Allagan Sync

Dalamud plugin for syncing FFXIV collections with allagan.app.

## Install as Custom Repo in Dalamud

Use this URL in `Settings -> Experimental -> Custom Plugin Repositories`:

`https://github.com/izaghar/allagan-sync/releases/latest/download/pluginmaster.json`

After adding the URL, open `/xlplugins`, search for `Allagan Sync`, and install.

## Release Workflow (Best Practice)

1. Increase `<Version>` in `AllaganSync/AllaganSync.csproj`.
2. Commit and push.
3. Create and push a tag (example: `v0.0.0.2`):
   - `git tag v0.0.0.2`
   - `git push origin v0.0.0.2`
4. GitHub Actions workflow `Build and Release` will:
   - build the plugin in `Release`
   - upload `latest.zip`
   - generate and upload `pluginmaster.json`

`latest.zip` and `pluginmaster.json` are always fetched via the stable `releases/latest/download/...` URLs.
