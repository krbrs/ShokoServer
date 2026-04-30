# Implementation Plan: Configuration API Refactor

**Feature**: Configuration API Refactor  
**Spec**: [`spec.md`](./spec.md)  
**Status**: Draft plan

## Objective

Make the `Configuration` API easier to understand and maintain by turning the current mixed-purpose endpoint surface into a clearer discovery, document, and action workflow.

The current controller couples discovery, schema access, load/save behavior, validation, custom actions, and live-edit handling behind a single surface. That makes the API harder for WebUI maintainers to read and increases the chance of regressions when adding new configuration types.

## Target State

The configuration surface should read like three distinct workflows:

1. Discover configurations and their capabilities.
2. Read and update a single configuration document.
3. Run validation, custom actions, or live-edit operations through explicit routes and request models.

Legacy callers should continue to work during the transition through compatibility aliases or wrapper routes.

## Scope

### In Scope

- Simplify the configuration discovery endpoint by replacing the long list of tri-state query parameters with a single request model.
- Separate configuration metadata from the editable configuration document in the public API shape.
- Make route names describe intent instead of implementation details.
- Move capability branching out of the controller and into a focused service-level dispatcher or equivalent helper.
- Replace ambiguous success-plus-validation-failure responses with consistent HTTP outcomes.
- Introduce explicit request models for action-oriented operations where the current body shape is too generic.
- Preserve compatibility for existing WebUI callers during the transition.
- Add or update tests that cover discovery, metadata, document retrieval, validation, update, and action execution flows.

### Out of Scope

- Changing authorization rules.
- Changing configuration storage format or persistence behavior.
- Changing plugin configuration semantics.
- Reworking unrelated controller families such as plugin, TMDB, or series endpoints.

## Proposed API Shape

### Discovery

- Keep a list endpoint for configuration discovery.
- Replace the many individual filter parameters with one filter object.
- Continue returning identifiers and enough metadata for callers to choose the correct configuration.

### Metadata

- Keep a dedicated metadata endpoint for a single configuration.
- Make it clear that metadata is for discovery and capability checks, not for editing the configuration payload itself.

### Document

- Expose the editable configuration document through one obvious route for read and write operations.
- Keep `GET`, `PUT`, and `PATCH` aligned with the same resource.
- Make the response format consistent so callers do not have to infer meaning from mixed payload flags.

### Actions

- Use explicit action routes for validation, custom actions, and live-edit behavior.
- Keep the action request and response shape narrow and self-explanatory.
- Preserve special behavior needed by the WebUI, but keep it out of the primary editing flow.

## Implementation Phases

### Phase 1: Normalize the request contract

1. Add a dedicated configuration discovery filter model for the list endpoint.
2. Add explicit request DTOs for action-style operations where the current JSON body is too generic.
3. Define the success and failure response rules for validation and update operations.
4. Update the OpenAPI-visible shapes so the public contract is easier to read.

### Phase 2: Split responsibilities in the controller surface

1. Introduce clearer route groupings for discovery, metadata, document access, validation, and actions.
2. Keep the existing route set as compatibility aliases where required.
3. Move route-level branching out of the main controller methods so each endpoint has one obvious responsibility.

### Phase 3: Push capability branching behind a service boundary

1. Introduce a service-level helper or dispatcher that encapsulates load/save/new/validate/live-edit decisions.
2. Let the controller ask for the operation it wants instead of checking multiple capability flags itself.
3. Ensure the service boundary still returns enough information for callers to update their UI state.

### Phase 4: Align response semantics

1. Return standard success codes for successful document and action operations.
2. Return standard validation or conflict codes for unsupported or invalid cases.
3. Keep detailed validation errors in a structured error payload instead of mixing them into a success response.

### Phase 5: Test and verify

1. Add tests for the discovery filter behavior.
2. Add tests for config ID discovery and metadata retrieval.
3. Add tests for document retrieval, full update, partial update, and schema retrieval.
4. Add tests for validation errors, unsupported actions, and live-edit behavior.
5. Add compatibility tests for any legacy aliases kept during the transition.

## Files Likely To Change

- [`Shoko.Server/API/v3/Controllers/ConfigurationController.cs`](/Users/uwe/Documents/GitHub/ShokoServer_fork/Shoko.Server/API/v3/Controllers/ConfigurationController.cs)
- [`Shoko.Server/API/v3/Models/Configuration/ConfigurationInfo.cs`](/Users/uwe/Documents/GitHub/ShokoServer_fork/Shoko.Server/API/v3/Models/Configuration/ConfigurationInfo.cs)
- [`Shoko.Server/API/v3/Models/Configuration/ConfigurationActionResult.cs`](/Users/uwe/Documents/GitHub/ShokoServer_fork/Shoko.Server/API/v3/Models/Configuration/ConfigurationActionResult.cs)
- [`Shoko.Abstractions/Config/Services/IConfigurationService.cs`](/Users/uwe/Documents/GitHub/ShokoServer_fork/Shoko.Abstractions/Config/Services/IConfigurationService.cs)
- [`Shoko.Server/Services/Configuration/ConfigurationService.cs`](/Users/uwe/Documents/GitHub/ShokoServer_fork/Shoko.Server/Services/Configuration/ConfigurationService.cs)
- New request/response models under [`Shoko.Server/API/v3/Models/Configuration/`](/Users/uwe/Documents/GitHub/ShokoServer_fork/Shoko.Server/API/v3/Models/Configuration)
- Related controller tests in [`Shoko.Tests`](/Users/uwe/Documents/GitHub/ShokoServer_fork/Shoko.Tests) or [`Shoko.IntegrationTests`](/Users/uwe/Documents/GitHub/ShokoServer_fork/Shoko.IntegrationTests)

## Risks

- Existing WebUI callers may depend on legacy route names or the current success-with-validation-errors behavior.
- The configuration service currently contains feature-flag-driven behavior that may require careful extraction to avoid regressions.
- Response semantics must stay consistent across save, validate, and action flows so the WebUI does not need separate handling for each case.

## Rollout Strategy

1. Add the new request and response shapes first.
2. Introduce the clearer routes while keeping legacy aliases available.
3. Move the controller logic to the new service boundary.
4. Update the WebUI to use the clearer discovery and action flow.
5. Remove legacy aliases only after the WebUI no longer depends on them.

## Validation Plan

- Verify that the configuration list still returns IDs and metadata needed for selection.
- Verify that a single configuration can be discovered, loaded, schema-checked, updated, and validated with the new route shape.
- Verify that validation failures are distinguishable from success responses by HTTP status.
- Verify that custom action and live-edit flows remain available for supported configurations.
- Verify that legacy routes, if kept, continue to function during the transition.
