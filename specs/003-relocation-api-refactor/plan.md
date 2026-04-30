# Implementation Plan: Relocation API Refactor

**Feature**: Relocation API Refactor  
**Spec**: [`spec.md`](./spec.md)  
**Status**: Draft plan

## Objective

Make the relocation surface easier to understand and maintain by turning the current mixed-purpose controller into a clearer discovery, pipe editing, and relocation workflow.

The current controller couples provider discovery, pipe CRUD, pipe configuration read/write, preview, batch relocation, and settings into one large surface. That makes the API harder for WebUI maintainers to read and increases the chance of regressions when provider capabilities or stored pipe behavior change.

## Target State

The relocation surface should read like three distinct workflows:

1. Discover providers and stored pipes with clear capability metadata.
2. Read and update a single relocation pipe configuration.
3. Run preview and batch relocation operations through explicit routes and request models.

Legacy callers should continue to work during the transition through compatibility aliases or wrapper routes.

## Scope

### In Scope

- Simplify the relocation discovery endpoints by reducing route-level branching and clarifying the provider versus pipe relationship.
- Separate relocation metadata from the editable pipe configuration in the public API shape.
- Make route names describe intent instead of implementation details.
- Move capability branching out of the controller where it currently has to inspect provider and pipe state directly.
- Replace ambiguous success-plus-validation-failure responses with consistent HTTP outcomes.
- Introduce explicit request models for preview and relocation operations where the current body shape is too generic.
- Preserve compatibility for existing WebUI callers during the transition.
- Add or update tests that cover provider discovery, pipe discovery, pipe configuration, preview, and relocation flows.

### Out of Scope

- Changing authorization rules.
- Changing relocation storage format or persistence behavior.
- Changing provider plugin semantics.
- Reworking unrelated controller families such as configuration, series, or TMDB endpoints.

## Proposed API Shape

### Discovery

- Keep provider and pipe list endpoints for discovery.
- Make it obvious which response shape describes a provider and which describes a stored pipe.
- Continue returning identifiers and enough metadata for callers to choose the correct provider or pipe.

### Pipe Metadata and Configuration

- Keep a dedicated metadata endpoint for a single relocation pipe.
- Make it clear that metadata is for discovery and capability checks, not for editing the configuration payload itself.
- Expose the editable configuration through one obvious route for read and write operations.

### Preview and Relocation

- Use explicit request models for preview and relocation operations.
- Keep the request and response shape narrow and self-explanatory.
- Preserve special behavior needed by the WebUI, but keep it out of the primary editing flow.

## Implementation Phases

### Phase 1: Normalize the request contract

1. Add a dedicated discovery filter model for provider and pipe lists where route parameters are too broad.
2. Add explicit request DTOs for preview and relocation-style operations where the current JSON body is too generic.
3. Define the success and failure response rules for pipe configuration updates and preview operations.
4. Update the OpenAPI-visible shapes so the public contract is easier to read.

### Phase 2: Split responsibilities in the controller surface

1. Introduce clearer route groupings for discovery, pipe metadata, pipe configuration, preview, and batch relocation.
2. Keep the existing route set as compatibility aliases where required.
3. Move route-level branching out of the main controller methods so each endpoint has one obvious responsibility.

### Phase 3: Push capability branching behind a service boundary

1. Introduce a service-level helper or dispatcher that encapsulates provider and pipe capability decisions.
2. Let the controller ask for the operation it wants instead of checking multiple capability flags and stored configuration state itself.
3. Ensure the service boundary still returns enough information for callers to update their UI state.

### Phase 4: Align response semantics

1. Return standard success codes for successful pipe reads, writes, preview, and relocation operations.
2. Return standard validation or conflict codes for unsupported or invalid cases.
3. Keep detailed validation errors in a structured error payload instead of mixing them into a success response.

### Phase 5: Test and verify

1. Add tests for provider and pipe discovery behavior.
2. Add tests for pipe metadata retrieval and configuration read/write behavior.
3. Add tests for preview, relocation, and validation errors.
4. Add compatibility tests for any legacy aliases kept during the transition.

## Files Likely To Change

- [`Shoko.Server/API/v3/Controllers/RelocationController.cs`](/Users/uwe/Documents/GitHub/ShokoServer_fork/Shoko.Server/API/v3/Controllers/RelocationController.cs)
- [`Shoko.Server/API/v3/Models/Relocation/RelocationPipe.cs`](/Users/uwe/Documents/GitHub/ShokoServer_fork/Shoko.Server/API/v3/Models/Relocation/RelocationPipe.cs)
- [`Shoko.Server/API/v3/Models/Relocation/RelocationProvider.cs`](/Users/uwe/Documents/GitHub/ShokoServer_fork/Shoko.Server/API/v3/Models/Relocation/RelocationProvider.cs)
- [`Shoko.Server/API/v3/Models/Relocation/RelocationSummary.cs`](/Users/uwe/Documents/GitHub/ShokoServer_fork/Shoko.Server/API/v3/Models/Relocation/RelocationSummary.cs)
- [`Shoko.Abstractions/Video/Services/IVideoRelocationService.cs`](/Users/uwe/Documents/GitHub/ShokoServer_fork/Shoko.Abstractions/Video/Services/IVideoRelocationService.cs)
- [`Shoko.Server/Services/VideoRelocationService.cs`](/Users/uwe/Documents/GitHub/ShokoServer_fork/Shoko.Server/Services/VideoRelocationService.cs)
- New request/response models under [`Shoko.Server/API/v3/Models/Relocation/`](/Users/uwe/Documents/GitHub/ShokoServer_fork/Shoko.Server/API/v3/Models/Relocation)
- Related controller tests in [`Shoko.Tests`](/Users/uwe/Documents/GitHub/ShokoServer_fork/Shoko.Tests) or [`Shoko.IntegrationTests`](/Users/uwe/Documents/GitHub/ShokoServer_fork/Shoko.IntegrationTests)

## Risks

- Existing WebUI callers may depend on legacy route names or the current success-with-validation-errors behavior.
- The relocation service currently contains provider-aware behavior that may require careful extraction to avoid regressions.
- Response semantics must stay consistent across pipe configuration, preview, and batch relocation flows so the WebUI does not need separate handling for each case.

## Rollout Strategy

1. Add the new request and response shapes first.
2. Introduce the clearer routes while keeping legacy aliases available.
3. Move the controller logic to the new service boundary.
4. Update the WebUI to use the clearer discovery and relocation flow.
5. Remove legacy aliases only after the WebUI no longer depends on them.

## Validation Plan

- Verify that the provider and pipe lists still return IDs and metadata needed for selection.
- Verify that a single relocation pipe can be discovered, loaded, schema-checked, and updated with the new route shape.
- Verify that validation failures are distinguishable from success responses by HTTP status.
- Verify that preview and batch relocation flows remain available for supported providers and pipes.
- Verify that legacy routes, if kept, continue to function during the transition.
