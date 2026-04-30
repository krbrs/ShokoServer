# Feature Specification: Relocation API Refactor

**Feature Branch**: `[003-relocation-api-refactor]`  
**Created**: 2026-04-30  
**Status**: Draft  
**Input**: User description: "Do the same cleanup for the Relocation provider after the configuration provider is cleaned up."

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Discover Relocation Providers and Pipes (Priority: P1)

A maintainer or WebUI client needs to discover which relocation providers exist, which stored pipes are available, and which pipe should be used for a relocation workflow.

**Why this priority**: Discovery is the entry point for managing relocation and understanding what the UI can offer.

**Independent Test**: A reviewer can request the provider and pipe lists and identify the correct provider ID, pipe ID, owning plugin, and capability metadata without reading source code.

**Acceptance Scenarios**:

1. **Given** the server has one or more registered relocation providers, **When** the provider list is requested, **Then** each returned item includes the provider ID, display name, plugin, and supported capabilities.
2. **Given** the server has one or more stored relocation pipes, **When** the pipe list is requested, **Then** each returned item includes the pipe ID, name, provider, default status, and usability status.
3. **Given** a maintainer filters by plugin or availability, **When** the discovery request is made, **Then** only matching providers or pipes are returned and the results remain stable enough to choose the target entry.

---

### User Story 2 - Read and Edit One Relocation Pipe (Priority: P1)

A maintainer or WebUI client needs to load a single relocation pipe, inspect its configuration, and save changes without having to understand internal provider flags.

**Why this priority**: Pipe configuration is the core editing workflow and is currently coupled to provider availability and configuration shape.

**Independent Test**: A reviewer can select a pipe by ID, read its current configuration, update it, and confirm the saved state through the public relocation endpoints.

**Acceptance Scenarios**:

1. **Given** a valid pipe ID, **When** its configuration is requested, **Then** the response contains the editable configuration data expected for that pipe.
2. **Given** a valid pipe ID, **When** its configuration is updated, **Then** the caller receives a clear success response or a clear validation failure response.
3. **Given** a provider does not support configuration, **When** the pipe configuration is requested, **Then** the caller receives a predictable failure response rather than a partial or ambiguous payload.

---

### User Story 3 - Preview and Run Relocation Clearly (Priority: P2)

A maintainer or WebUI client needs clear feedback when previewing relocation, running a batch relocation, or invoking a pipe with a configuration payload.

**Why this priority**: These are the most behavior-heavy operations and are currently mixed with route-level branching and compatibility behavior.

**Independent Test**: A reviewer can submit a preview or relocation request and receive a predictable response that clearly separates success, validation failure, and unsupported behavior.

**Acceptance Scenarios**:

1. **Given** a valid preview request, **When** the caller requests a relocation preview, **Then** the response is structured and identifies the affected files and outcomes.
2. **Given** a valid batch relocation request, **When** the caller submits the request, **Then** the response clearly identifies success, partial success, and validation failures.
3. **Given** a relocation provider or pipe does not support the requested capability, **When** the operation is attempted, **Then** the caller receives a clear failure response rather than an ambiguous mixed result.

### Edge Cases

- What happens when there are no stored relocation pipes?
- What happens when a requested pipe ID no longer exists?
- What happens when a pipe’s provider is unavailable but the stored configuration still exists?
- What happens when a client submits configuration data that does not match the provider schema?
- What happens when a preview or relocation request targets files that cannot be resolved?

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: The relocation surface MUST provide a discoverable list of providers and stored pipes that includes the IDs and identifying metadata needed to select a target entry.
- **FR-002**: The relocation surface MUST provide a way to retrieve a single relocation pipe by ID for editing.
- **FR-003**: The relocation surface MUST provide a way to retrieve and update the editable configuration for a relocation pipe.
- **FR-004**: The relocation surface MUST provide a way to preview relocation behavior using the same provider and pipe concepts that the runtime uses.
- **FR-005**: The relocation surface MUST provide a way to run batch relocation workflows with explicit request models and predictable outcomes.
- **FR-006**: The relocation surface MUST return clear and consistent outcomes for success, validation failure, unsupported operations, missing pipe IDs, and unavailable providers.
- **FR-007**: The relocation surface MUST expose enough metadata for a WebUI client to determine whether a provider supports configuration, relocation preview, and pipe editing.
- **FR-008**: The relocation surface MUST keep the public responsibility of each endpoint narrow enough that a maintainer can understand the endpoint purpose from its route and response contract alone.
- **FR-009**: Existing callers MUST be able to continue identifying providers and pipes by ID during the transition to the improved endpoint structure.
- **FR-010**: Any compatibility behavior kept during the transition MUST be clearly separated from the primary relocation workflow.

### Key Entities *(include if feature involves data)*

- **Relocation Provider**: A provider implementation with an ID, plugin, display metadata, and capability flags.
- **Relocation Pipe**: A stored provider instance that may include a persisted configuration blob and default status.
- **Relocation Configuration**: The editable configuration payload attached to a relocation pipe.
- **Relocation Operation Result**: The structured outcome of discovery, preview, edit, or batch relocation operations.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: A maintainer can identify the correct relocation provider or pipe from the discovery surface without reading source code in a single review session.
- **SC-002**: A maintainer can load, edit, and save a relocation pipe configuration using the public relocation surface without inspecting internal service behavior.
- **SC-003**: At least 90% of relocation-related maintenance tasks in a guided review can be mapped to a single obvious endpoint family on the first attempt.
- **SC-004**: Validation failures, missing IDs, unavailable providers, and unsupported operations are distinguishable from successful operations in a way that does not require guessing from the response body alone.
- **SC-005**: The number of distinct endpoint responsibilities in the relocation surface is reduced to a small, stable set that can be documented on one page.
- **SC-006**: WebUI maintainers can determine whether a relocation provider supports configuration and relocation workflows without reading implementation code.

## Assumptions

- The current provider and pipe identifiers remain stable during the refactor.
- The relocation surface keeps compatibility routes available during transition so the current WebUI does not break immediately.
- The feature does not change authorization rules or relocation storage format.
- The configuration cleanup for the configuration provider is the model for the relocation cleanup, so similar route and response simplifications are acceptable here.
