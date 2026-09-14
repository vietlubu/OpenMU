# Repository Guidelines

## Project Overview

OpenMU is a .NET 10 MU Online server emulator: game rules, TCP protocol handling, persisted configuration, admin web UI, and deployment assets. The supported runtime topology is the all-in-one ASP.NET Core host; Dapr hosts are an alternative composition topology, while `deploy/distributed/` is unsupported.

## Architecture & Data Flow

- **Composition roots:** `src/Startup/Program.cs` builds the in-process host. `src/Dapr/*Host/Program.cs` builds per-service hosts behind the same interface contracts.
- **Initialization:** `src/Persistence/Initialization/DataInitializationBase.cs` creates/version-controls the `GameConfiguration` aggregate, maps, servers, plugin settings, and configuration-update records.
- **Runtime:** persisted definitions load into `src/GameLogic/GameContext.cs`, maps, and `Player` state. `src/GameServer/GameServer.cs` owns the network-facing game server.
- **Packet path:** TCP listener → `src/Network/Connection.cs` (pipeline/framing/encryption) → `src/GameServer/RemoteView/RemotePlayer.cs` → version-selected packet-handler plugins → GameLogic actions → view plugins serialize responses.
- **Persistence:** use `IPersistenceContextProvider` and the appropriate `IContext`; EF/PostgreSQL is in `src/Persistence/EntityFramework/`, in-memory persistence is in `src/Persistence/InMemory/`.
- **Service boundaries:** depend on `src/Interfaces/` contracts. Local and Dapr proxy implementations intentionally share those contracts.

Configuration is a persisted aggregate graph, not disposable runtime data. Preserve collection/back-reference relationships and apply changes through the relevant context. For configuration migrations, add an idempotent `UpdatePlugInBase` implementation, a stable `[Guid]`, and the next `UpdateVersion` value; do not call `SaveChangesAsync` inside the plugin.

## Key Directories

| Path | Purpose |
| --- | --- |
| `src/GameLogic/` | Gameplay state, actions, maps, attributes, and plugin points. |
| `src/GameServer/` | TCP game server, remote players, protocol-facing view adapters. |
| `src/Network/` | Packet transport, encryption, framing, and packet DTOs. |
| `src/DataModel/` | Persisted domain/configuration model and aggregate metadata. |
| `src/Persistence/` | EF/InMemory providers, initial data, and versioned updates. |
| `src/PlugIns/` | Plugin discovery, activation, and strategy infrastructure. |
| `src/Startup/` | Supported all-in-one host and Dockerfile. |
| `src/Dapr/` | Alternative distributed host/proxy implementation. |
| `src/Web/` | Admin panel and map/web UI. |
| `tests/` | NUnit subsystem tests; benchmark projects are separate. |
| `deploy/all-in-one/` | Supported Docker Compose deployment. |
| `docs-website/` | Canonical Docusaurus contributor/operator documentation. |
| `docs/` | Code-bound and generated technical documentation. |
| `wiki/README.md` | Instance-specific Vietnamese x9999 player guide; not canonical project behavior. |

## Development Commands

Run solution commands from the repository root by passing the solution path, or `cd src` first.

```sh
# Restore, build, and test as CI does
dotnet restore src/MUnique.OpenMU.sln
dotnet build src/MUnique.OpenMU.sln --configuration Release -p:ci=true
dotnet test tests/MUnique.OpenMU.Tests/MUnique.OpenMU.Tests.csproj --configuration Release --no-build

# Focus one NUnit test
dotnet test tests/MUnique.OpenMU.Persistence.Initialization.Tests/MUnique.OpenMU.Persistence.Initialization.Tests.csproj \
  --no-restore --filter FullyQualifiedName~TestName

# Publish the all-in-one host
dotnet publish src/Startup/MUnique.OpenMU.Startup.csproj --configuration Release -p:ci=true

# Local all-in-one stack (uses the Compose override)
cd deploy/all-in-one && docker compose up -d --no-build

# Documentation
cd docs-website && npm ci && npm run build
```

There is no separate repository lint or formatter command. Build analyzers are the style gate.

## Code Conventions & Common Patterns

- Match `src/.editorconfig`, `src/stylecop.json`, and neighboring code. Source files use the MUnique copyright header; exposed elements generally need XML documentation.
- Nullable, unobserved-task, and threading diagnostics are enforced by project settings. Use `async`/`ValueTask`, `ConfigureAwait(false)`, disposal, and existing locks/serialization patterns consistently.
- Runtime objects are mutable and event-driven. Preserve player state-machine gates, async locks, timer ownership, and event unsubscription/disposal.
- Plugin metadata is runtime behavior: `[Guid]`, plugin-point/container attributes, client-version suitability, and persisted plugin configuration must remain stable. Stateless packet handlers may be cached.
- Keep cross-service dependencies behind `src/Interfaces/`; update both Startup and Dapr registrations/proxies when a contract changes.
- Prefer existing initialization helpers and `GetItemDefinition`/GUID patterns for configuration changes. Configuration updates must be repeatable and repair partial state.
- Networking is allocation-sensitive: retain `System.IO.Pipelines`, pooled buffers, `ReadOnlySequence<byte>`, and cached-handler patterns rather than adding copies or blocking work.
- Do not hand-edit generated artifacts. Packet C# is generated from XML/XSLT under `src/Network/Packets/`; model partial code may be source-generated. Change the source definition/generator input instead.

## Important Files

- `src/Startup/Program.cs` — all-in-one host composition and startup parameters.
- `src/Persistence/Initialization/DataInitializationBase.cs` — initial data and update registration.
- `src/Persistence/Initialization/Updates/UpdatePlugInBase.cs` and `UpdateVersion.cs` — configuration migration contract/versioning.
- `src/DataModel/Configuration/GameConfiguration.cs` — main persisted game-configuration aggregate.
- `src/GameLogic/GameContext.cs`, `src/GameLogic/Player.cs` — central live gameplay state.
- `src/GameServer/DefaultTcpGameServerListener.cs` and `src/Network/Connection.cs` — listener and transport lifecycle.
- `src/Directory.Build.props`, `src/Directory.Packages.props`, `src/.editorconfig`, `src/stylecop.json` — build/style source of truth.
- `azure-pipelines.yml` and `.github/workflows/dotnetcore.yml` — canonical build/publish behavior.

## Runtime/Tooling Preferences

- Use **.NET SDK 10**. There is no `global.json`; CI installs `10.0.x`.
- NuGet versions are centrally managed in `src/Directory.Packages.props`; do not pin versions per project without a repository-wide reason.
- Use `-p:ci=true` for reproducible CI-style builds. Local builds intentionally run packet/model generation and may require Node tooling, XSLT, TypeScript tooling, and Dart Sass on non-Windows.
- `docs-website/` is Docusaurus on **Node 20+** with npm and `package-lock.json`.
- For local throwaway server state, `-demo` uses in-memory persistence. `-reinit` destroys persistent data; use only deliberately.
- Treat `deploy/all-in-one/deploy-lenovo.sh` as host-specific automation, not a general local command. It expects LAN secrets/configuration and applies mandatory updates. Do not use `deploy/distributed/` for new work.

## Completion & Commit Requirements

- When work adds or changes a Season 6 configuration update, set `expected_data_version` in `deploy/all-in-one/deploy-lenovo.sh` to the current highest `UpdateVersion` before finishing. This deployment gate must match the latest required configuration migration.
- Before reporting completion, create a Git commit containing every change made during the current agent run. Do not leave a completed change set uncommitted.

## Testing & QA

- Tests are NUnit (`tests/SharedTestUsings.cs`); use constraint assertions and async `Task`/`ValueTask` tests.
- Prefer behavioral coverage through real `GameContext`, `PlayerTestHelper`, and `InMemoryPersistenceContextProvider` rather than testing implementation plumbing.
- Reuse subsystem helpers: e.g. `tests/MUnique.OpenMU.Tests/PlayerTestHelper.cs`, `GameContextTestHelper.cs`, and initialization tests in `tests/MUnique.OpenMU.Persistence.Initialization.Tests/TestInitializationWithEfCore.cs`.
- Add a durable test only for a meaningful contract, state transition, boundary, or migration repair. Configuration-update tests should invoke the update twice and assert the resulting persisted graph.
- Isolate global environment/filesystem state. Existing tests use `[NonParallelizable]`, setup/teardown restoration, GUID temporary paths, and bounded polling where needed.
- Packet test sources may be generated during local pre-build; do not edit generated packet test files.
- Documentation changes under `docs-website/` require `npm run build`; it fails broken links, anchors, and images. `docs/` and `wiki/` are outside that Docusaurus check.
