<!--
Sync Impact Report
Version change: template/unversioned -> 1.0.0
Modified principles:
- template principle 1 -> Code Quality and Maintainability
- template principle 2 -> Test Coverage Is Mandatory
- template principle 3 -> User Experience Consistency
- template principle 4 -> Performance Budgets and Scalability
- template principle 5 -> Change Governance and Compatibility
Added sections:
- Engineering Standards
- Delivery and Review Gates
Removed sections:
- Template placeholder sections
Templates updated:
- ✅ .specify/templates/plan-template.md
- ✅ .specify/templates/spec-template.md
- ✅ .specify/templates/tasks-template.md
Follow-up TODOs:
- TODO(RATIFICATION_DATE): original adoption date is not recorded in the repository.
-->
# Shoko Server Constitution

## Core Principles

### Code Quality and Maintainability
Code changes MUST be small, explicit, and easy to review. Prefer the existing
service, repository, and controller boundaries over introducing new abstractions
or global state. New code MUST follow repository conventions, use `var` where
the type is obvious, and preserve the current model separation between
persistence entities, DTOs, and abstraction interfaces.

Every change MUST remove ambiguity rather than hide it: names must reflect the
domain, methods must do one thing, and dead code or duplicate logic MUST not be
left behind. If a simpler implementation exists, it MUST be preferred unless a
clear technical reason is documented.

Rationale: maintainable code is the only way this server can remain stable while
supporting multiple APIs, plugins, background jobs, and storage backends.

### Test Coverage Is Mandatory
Behavior changes MUST be covered by automated tests. Bug fixes SHOULD add a
regression test that fails before the fix and passes after it. Changes that
touch API contracts, background jobs, persistence mapping, cache invalidation,
or cross-service behavior MUST include the lowest-level test that can detect the
failure, plus integration coverage when the interaction crosses a boundary.

Tests MUST describe observable behavior, not implementation details. A change is
not complete until the relevant test suite passes and the new behavior can be
verified repeatably in CI or a local test run.

Rationale: this repository is a long-lived server with compatibility-sensitive
surfaces, so tests are the primary guard against regressions.

### User Experience Consistency
User-facing behavior across the Web UI, API, tray app, CLI output, logs, and
error messages MUST stay consistent in naming, status handling, route shape, and
terminology. New features MUST reuse established patterns for navigation,
defaults, permissions, response envelopes, and validation feedback rather than
inventing one-off interactions.

When a feature affects a visible flow, the implementation MUST preserve the
existing mental model for users and document any deliberate deviation. API and UI
changes MUST be validated from the user perspective, not only from internal
service behavior.

Rationale: the project serves both humans and integrations, so inconsistent
surfaces create support burden and make automation brittle.

### Performance Budgets and Scalability
Changes that affect startup, import, hashing, database access, search, or UI
responsiveness MUST state the expected performance impact. Hot-path code MUST
avoid unnecessary allocations, N+1 queries, repeated serialization, and
unbounded scans when a cached or indexed alternative exists.

Performance-sensitive work MUST use the existing caching and repository patterns
before introducing new infrastructure. If a change is expected to increase CPU,
memory, I/O, or latency, the change MUST include evidence that the cost is
acceptable and that the regression is intentional.

Rationale: the server manages large media libraries, so scalability and steady
latency matter as much as feature correctness.

### Change Governance and Compatibility
Technical decisions MUST prefer backward-compatible behavior, incremental
rollouts, and append-only evolution of public contracts and migrations.
Breaking changes require a documented migration path, explicit justification,
and a versioned deprecation plan.

Schema migrations MUST be additive or safely reversible when possible. Public API
changes MUST respect versioning boundaries, and any exception to existing
behavior MUST be called out in the plan before implementation starts.

Rationale: this repository exposes stable integrations and persistent data, so
compatibility must be managed deliberately.

## Engineering Standards

All implementation MUST target the repository's current stack and conventions:
.NET 10, C# with the existing style rules, dependency injection where available,
and the established host, repository, and controller patterns.

Persistence entities, API DTOs, and abstraction interfaces MUST remain separate
layers. Controllers MUST not bypass the service layer for business logic, and
new code MUST choose cached repositories over direct repositories when both are
available for the same data.

Serialization, routing, authorization, and event emission MUST use the existing
framework conventions already present in the server. New public endpoints or UI
features MUST integrate with the current Web UI and API versioning model.

## Delivery and Review Gates

Every feature plan MUST state how it satisfies the principles above. If a change
cannot satisfy one of them, the plan MUST document the exception in
`Complexity Tracking` before implementation begins.

Code review MUST verify test coverage, UX consistency, and performance impact
for every change that touches behavior. Reviewers MUST reject changes that add
unexplained complexity, silently alter contracts, or ship without a measurable
reason for the chosen implementation.

When a change affects user-visible behavior or runtime cost, the implementation
MUST include an acceptance path that a reviewer can reproduce locally.

## Governance

This constitution supersedes conflicting informal practice. Any amendment MUST
update this file, propagate required template changes, and record the reason for
the revision in the sync impact report.

Versioning policy:
- MAJOR: remove or redefine a principle, or change governance in a backward
  incompatible way.
- MINOR: add a principle, materially expand a principle, or add a new governed
  section.
- PATCH: clarify language, tighten examples, or fix wording without changing
  meaning.

Compliance expectations:
- Every plan, spec, and task set MUST be checked against this constitution.
- Any accepted exception MUST be explicit, time-bounded, and reviewed.
- Implementation work MUST not begin until constitution conflicts are resolved
  or recorded in `Complexity Tracking`.

Amendments require a documented rationale, updated dependent templates, and a
new version line with an updated `Last Amended` date.

**Version**: 1.0.0 | **Ratified**: TODO(RATIFICATION_DATE): original adoption date is not recorded in the repository. | **Last Amended**: 2026-04-27
