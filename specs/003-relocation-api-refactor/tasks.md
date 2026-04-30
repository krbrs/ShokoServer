# Tasks: Relocation API Refactor

**Input**: Design artifacts from `specs/003-relocation-api-refactor/spec.md` and `specs/003-relocation-api-refactor/plan.md`
**Prerequisites**: `spec.md`, `plan.md`
**Scope**: Refactor the relocation discovery, pipe metadata, pipe configuration, preview, and batch relocation surface while preserving compatibility for existing callers.

## Task 1: Inventory the current relocation surface

- [X] Review the existing `RelocationController` routes, response types, and compatibility-sensitive behaviors.
- [X] Identify every route that will remain public, be renamed, or be kept only as a compatibility alias.
- [X] Identify every controller behavior that currently depends on provider availability, pipe state, or configuration schema support.

**Depends on**: None  
**Outcome**: Clear migration map for the refactor.

## Task 2: Introduce explicit request models for relocation discovery and operations

- [ ] Create a request model for discovery filters where the list endpoint currently needs route or query branching.
- [ ] Create request models for preview and relocation operations where the body currently accepts generic JSON and extra query parameters.
- [ ] Add any shared response models needed to keep relocation outcomes explicit and self-documenting.

**Depends on**: Task 1  
**Outcome**: Stronger public request/response contracts.

## Task 3: Refine relocation metadata and pipe models

- [ ] Review `RelocationProvider`, `RelocationPipe`, and `RelocationSummary` to separate discovery metadata from editable pipe configuration.
- [ ] Update the metadata shape so WebUI callers can discover IDs and capabilities without mixing in pipe-edit semantics.
- [ ] Ensure the pipe configuration response shape stays focused on the editable configuration payload.

**Depends on**: Task 2  
**Outcome**: Clear distinction between discovery metadata and editable pipe content.

## Task 4: Add a service-level orchestration boundary for relocation operations

- [ ] Extract the controller-facing branching around provider discovery, pipe configuration, preview, and batch relocation into a focused service helper or dispatcher.
- [ ] Ensure the controller can request an operation without checking the full capability matrix itself.
- [ ] Preserve existing behavior for supported and unsupported relocation capabilities.

**Depends on**: Task 2, Task 3  
**Outcome**: Controller logic becomes thinner and easier to follow.

## Task 5: Refactor provider and pipe discovery endpoints

- [X] Simplify the provider and pipe discovery endpoints so the public contract clearly separates provider metadata from stored pipe metadata.
- [X] Keep discovery results stable and continue returning IDs and identifying metadata.
- [X] Preserve plugin scoping and availability behavior while simplifying the call site.

**Depends on**: Task 2, Task 3  
**Outcome**: Easier-to-read discovery routes.

## Task 6: Split pipe metadata access from pipe configuration access

- [X] Keep a dedicated metadata route for a single relocation pipe.
- [X] Make the main pipe read route return the editable configuration in a clear, predictable way.
- [X] Keep the schema-aligned behavior tied to the same pipe ID and configuration flow.

**Depends on**: Task 3, Task 4  
**Outcome**: Clear separation between metadata and editable configuration.

## Task 7: Refactor pipe save and partial-update behavior

- [ ] Update the full-update route to use the new request and response conventions.
- [ ] Update the partial-update route so it follows the same pipe-editing semantics as the full-update route.
- [ ] Replace success-with-validation-errors responses with consistent HTTP outcomes and structured error payloads.

**Depends on**: Task 4, Task 6  
**Outcome**: Editing flows become easier for callers to interpret.

## Task 8: Refactor preview and batch relocation routes

- [ ] Move preview to an explicit route with a request model that matches the public contract.
- [ ] Move batch relocation to an explicit route with a narrow, predictable shape.
- [ ] Keep compatibility aliases only where required by existing callers.

**Depends on**: Task 4, Task 6, Task 7  
**Outcome**: Action-oriented operations become explicit and easier to maintain.

## Task 9: Normalize response semantics and error handling

- [ ] Return standard success codes for successful discovery, reads, writes, previews, and relocations.
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

- [ ] Add tests that the provider and pipe list endpoints return IDs and identifying metadata with the new discovery filter shape.
- [ ] Add tests that the metadata route remains distinct from the editable pipe configuration route.
- [ ] Add tests for empty lists, plugin-scoped discovery, and availability behavior.

**Depends on**: Task 5, Task 6  
**Outcome**: Discovery and metadata behavior is covered.

## Task 12: Add controller and service tests for editing and relocation flows

- [ ] Add tests for reading a pipe configuration by ID.
- [ ] Add tests for full updates and partial updates.
- [ ] Add tests for preview failures, batch relocation validation failures, unsupported operations, and unavailable providers.
- [ ] Add tests for compatibility routes if they remain enabled.

**Depends on**: Task 7, Task 8, Task 9, Task 10  
**Outcome**: The full editing and relocation workflow is covered.

## Task 13: Update API-facing comments and route documentation

- [ ] Update controller comments so each route describes the user-facing responsibility rather than the implementation detail.
- [ ] Update any route summaries or response notes that still describe the old multiplexed behavior.
- [ ] Verify the public API text matches the new discovery, metadata, pipe configuration, preview, and relocation split.

**Depends on**: Task 5, Task 6, Task 7, Task 8  
**Outcome**: Maintainers can understand the surface from the code and generated docs.

## Task 14: Validate the refactor end to end

- [ ] Run the relevant test suite for relocation controller and relocation service coverage.
- [ ] Verify the new routes, aliases, and response semantics behave as intended.
- [ ] Confirm the WebUI can still discover, inspect, preview, and relocate configurations during the transition period.

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
