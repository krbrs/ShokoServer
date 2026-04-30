# Feature Specification: Configuration API Refactor

**Feature Branch**: `[002-configuration-api-refactor]`  
**Created**: 2026-04-30  
**Status**: Draft  
**Input**: User description: "Make the /api/v3/Configuration endpoint better to understand and maintain."

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Discover Configuration Capabilities (Priority: P1)

A maintainer or WebUI client needs to discover which configuration entries exist, which one they need, and what each configuration supports before editing it.

**Why this priority**: The first step in using the configuration surface is finding the correct configuration and understanding its capabilities.

**Independent Test**: A reviewer can request the configuration list and identify the correct configuration ID, display name, owning plugin, and supported actions without reading source code.

**Acceptance Scenarios**:

1. **Given** the server has one or more registered configurations, **When** the list view is requested without extra filters, **Then** each returned item includes the configuration ID and the fields needed to identify the configuration.
2. **Given** a maintainer filters by name or plugin, **When** the list is requested, **Then** only matching configurations are returned and the results remain stable enough to choose the target configuration.

---

### User Story 2 - Read and Edit One Configuration (Priority: P1)

A maintainer or WebUI client needs to load one configuration, inspect its schema, and save updates without having to understand internal feature flags.

**Why this priority**: Editing a configuration is the main value of the endpoint and must be easy to follow.

**Independent Test**: A reviewer can select a configuration by ID, read its current state, inspect the schema, and apply a valid update using only the public configuration endpoints.

**Acceptance Scenarios**:

1. **Given** a valid configuration ID, **When** its current state is requested, **Then** the response contains the configuration data expected for editing.
2. **Given** a valid configuration ID, **When** its schema is requested, **Then** the response describes the editable shape of that configuration in a way that matches the current data.
3. **Given** a valid configuration update, **When** the update is submitted, **Then** the caller receives a clear success response that indicates whether the configuration changed.

---

### User Story 3 - Handle Validation and Special Actions Clearly (Priority: P2)

A maintainer or WebUI client needs clear feedback when a configuration cannot be saved, requires a custom action, or needs a live-edit style update.

**Why this priority**: These are the cases that most often confuse maintainers and create fragile client code.

**Independent Test**: A reviewer can submit an invalid configuration or a custom action request and receive a predictable response that clearly separates success, validation failure, and unsupported behavior.

**Acceptance Scenarios**:

1. **Given** an invalid configuration payload, **When** validation is requested, **Then** the response clearly identifies the validation problems.
2. **Given** a configuration that supports custom actions, **When** a custom action is invoked, **Then** the caller receives the result and any follow-up instructions in a single structured response.
3. **Given** a configuration that does not support a requested capability, **When** the unsupported operation is attempted, **Then** the caller receives a clear failure response rather than an ambiguous partial result.

### Edge Cases

- What happens when the configuration list is empty?
- What happens when a requested configuration ID no longer exists?
- What happens when a client submits data that does not match the configuration schema?
- What happens when a configuration supports a capability in metadata but the action cannot complete for the current user or state?
- What happens when a maintainer requests a partial update but the update does not change anything?

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: The configuration surface MUST provide a discoverable list of available configurations that includes the ID and display information needed to select a target configuration.
- **FR-002**: The configuration surface MUST provide a way to retrieve one configuration by ID for editing.
- **FR-003**: The configuration surface MUST provide a way to retrieve the editable shape or schema for one configuration by ID.
- **FR-004**: The configuration surface MUST provide a way to create a fresh configuration instance for a given configuration ID when that is a supported workflow.
- **FR-005**: The configuration surface MUST provide a way to validate a candidate configuration before saving it.
- **FR-006**: The configuration surface MUST provide a way to save a full configuration update and a partial configuration update.
- **FR-007**: The configuration surface MUST provide a way to execute supported configuration actions without requiring the caller to understand internal service flags.
- **FR-008**: The configuration surface MUST return clear and consistent outcomes for success, validation failure, unsupported operations, and missing configuration IDs.
- **FR-009**: The configuration surface MUST expose enough metadata for a WebUI client to determine whether a configuration supports loading, saving, validation, custom actions, or live editing.
- **FR-010**: The configuration surface MUST keep the public responsibility of each endpoint narrow enough that a maintainer can understand the endpoint purpose from its route and response contract alone.
- **FR-011**: Existing callers MUST be able to continue identifying configurations by ID during the transition to the improved endpoint structure.
- **FR-012**: Any compatibility behavior kept during the transition MUST be clearly separated from the primary configuration workflow.

### Key Entities *(include if feature involves data)*

- **Configuration Entry**: A registered configuration that has an ID, display name, owning plugin, and capability metadata.
- **Configuration Document**: The editable configuration data associated with a configuration entry.
- **Configuration Schema**: The description of the expected structure for a configuration document.
- **Configuration Action Result**: The structured outcome of validation, save, load, custom action, or live-edit operations.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: A maintainer can identify the correct configuration ID and its purpose from the configuration list without reading source code in a single review session.
- **SC-002**: A maintainer can complete the discover-read-validate-save workflow for one configuration using the public configuration surface without needing to inspect internal service behavior.
- **SC-003**: At least 90% of configuration-related maintenance tasks in a guided review can be mapped to a single obvious endpoint family on the first attempt.
- **SC-004**: Validation failures, missing IDs, and unsupported operations are distinguishable from successful updates in a way that does not require guessing from the response body alone.
- **SC-005**: The number of distinct endpoint responsibilities in the configuration surface is reduced to a small, stable set that can be documented on one page.
- **SC-006**: WebUI maintainers can determine whether a configuration supports custom actions or live editing without reading implementation code.

## Assumptions

- The current configuration list already remains the primary way to discover configuration IDs.
- The improved configuration surface keeps existing configuration IDs stable.
- Any temporary compatibility routes remain available during transition so existing WebUI behavior does not break immediately.
- The feature does not change authorization rules or configuration storage format.
