# Tasks: Configuration API Refactor

**Input**: Design artifacts from `specs/002-configuration-api-refactor/spec.md` and `specs/002-configuration-api-refactor/plan.md`
**Prerequisites**: `spec.md`, `plan.md`
**Scope**: Refactor the configuration discovery, metadata, document, validation, and action surface while preserving compatibility for existing callers.

## Task 1: Inventory the current configuration surface

- [ ] Review the existing `ConfigurationController` routes, response types, and compatibility-sensitive behaviors.
- [ ] Identify every route that will remain public, be renamed, or be kept only as a compatibility alias.
- [ ] Identify every controller behavior that currently depends on `HasCustomLoad`, `HasCustomSave`, `HasCustomNewFactory`, `HasCustomValidation`, `HasCustomActions`, or `HasLiveEdit`.

**Depends on**: None
**Outcome**: Clear migration map for the refactor.

## Task 2: Introduce explicit request models for configuration discovery and actions

- [ ] Create a request model for configuration discovery filters so the list endpoint no longer needs a long parameter list.
- [ ] Create request models for action-oriented operations where the body currently accepts generic JSON and extra query parameters.
- [ ] Add any shared response models needed to keep action outcomes explicit and self-documenting.

**Depends on**: Task 1
**Outcome**: Stronger public request/response contracts.

## Task 3: Refine configuration metadata and document models

- [ ] Review `ConfigurationInfo` and related response types to separate discovery metadata from editable document concerns.
- [ ] Update the metadata shape so WebUI callers can discover IDs and capabilities without mixing in document-edit semantics.
- [ ] Ensure the document response shape stays focused on the editable configuration payload.

**Depends on**: Task 2
**Outcome**: Clear distinction between discovery metadata and editable content.

## Task 4: Add a service-level orchestration boundary for configuration operations

- [ ] Extract the controller-facing branching around load, save, new, validation, custom actions, and live edit into a focused service helper or dispatcher.
- [ ] Ensure the controller can request an operation without checking the full capability matrix itself.
- [ ] Preserve existing behavior for supported and unsupported configuration capabilities.

**Depends on**: Task 2, Task 3
**Outcome**: Controller logic becomes thinner and easier to follow.

## Task 5: Refactor the discovery endpoint

- [ ] Replace the long list of tri-state query parameters on the configuration list endpoint with the new discovery filter model.
- [ ] Keep discovery results stable and continue returning IDs and identifying metadata.
- [ ] Preserve search and plugin scoping behavior while simplifying the call site.

**Depends on**: Task 2, Task 3
**Outcome**: Easier-to-read discovery route.

## Task 6: Split configuration document access from metadata access

- [ ] Keep a dedicated metadata route for a single configuration.
- [ ] Make the main configuration read route return the editable document in a clear, predictable way.
- [ ] Keep the schema route aligned with the same configuration ID and document flow.

**Depends on**: Task 3, Task 4
**Outcome**: Clear separation between discovery metadata and document editing.

## Task 7: Refactor save and partial-update behavior

- [ ] Update the full-update route to use the new request and response conventions.
- [ ] Update the partial-update route so it follows the same document-editing semantics as the full-update route.
- [ ] Replace success-with-validation-errors responses with consistent HTTP outcomes and structured error payloads.

**Depends on**: Task 4, Task 6
**Outcome**: Editing flows become easier for callers to interpret.

## Task 8: Refactor validation, custom action, and live-edit routes

- [ ] Move validation to an explicit route with a request model that matches the public contract.
- [ ] Move custom action execution to an explicit action route with a narrow, predictable shape.
- [ ] Move live-edit handling to an explicit route with clear semantics for the caller.
- [ ] Keep compatibility aliases only where required by existing callers.

**Depends on**: Task 4, Task 6, Task 7
**Outcome**: Action-oriented operations become explicit and easier to maintain.

## Task 9: Normalize response semantics and error handling

- [ ] Return standard success codes for successful configuration reads and writes.
- [ ] Return standard validation or conflict codes for invalid or unsupported cases.
- [ ] Ensure callers can distinguish successful operations from validation failures without inferring meaning from response body shape.

**Depends on**: Task 7, Task 8
**Outcome**: Predictable error handling for WebUI and other clients.

## Task 10: Preserve compatibility routes and transition behavior

- [ ] Keep the legacy routes that current clients depend on, if they are still needed during rollout.
- [ ] Make compatibility behavior obvious in code so it does not become the primary implementation path.
- [ ] Verify that any aliases map to the new service/controller behavior instead of duplicating logic.

**Depends on**: Task 5, Task 6, Task 7, Task 8
**Outcome**: Safe transition path for existing callers.

## Task 11: Add controller and service tests for discovery and metadata

- [ ] Add tests that the list endpoint returns IDs and identifying metadata with the new discovery filter shape.
- [ ] Add tests that the metadata route remains distinct from the editable document route.
- [ ] Add tests for empty lists, plugin-scoped discovery, and search behavior.

**Depends on**: Task 5, Task 6
**Outcome**: Discovery and metadata behavior is covered.

## Task 12: Add controller and service tests for editing and actions

- [ ] Add tests for reading a configuration document by ID.
- [ ] Add tests for full updates and partial updates.
- [ ] Add tests for validation failures, custom actions, live edit, and unsupported operations.
- [ ] Add tests for compatibility routes if they remain enabled.

**Depends on**: Task 7, Task 8, Task 9, Task 10
**Outcome**: The full editing and action workflow is covered.

## Task 13: Update API-facing comments and route documentation

- [ ] Update controller comments so each route describes the user-facing responsibility rather than the implementation detail.
- [ ] Update any route summaries or response notes that still describe the old multiplexed behavior.
- [ ] Verify the public API text matches the new discovery, metadata, document, and action split.

**Depends on**: Task 5, Task 6, Task 7, Task 8
**Outcome**: Maintainers can understand the surface from the code and generated docs.

## Task 14: Validate the refactor end to end

- [ ] Run the relevant test suite for configuration controller and configuration service coverage.
- [ ] Verify the new routes, aliases, and response semantics behave as intended.
- [ ] Confirm the WebUI can still discover, read, and update configurations during the transition period.

**Depends on**: Task 11, Task 12, Task 13
**Outcome**: Refactor is verified before rollout.

## Suggested Execution Order

1. Task 1
2. Task 2
3. Task 3
4. Task 4
5. Task 5
6. Task 6
7. Task 7
8. Task 8
9. Task 9
10. Task 10
11. Task 11
12. Task 12
13. Task 13
14. Task 14
