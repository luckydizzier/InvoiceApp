# AGENTS.md – Wrecept Modular Agent System

## Purpose
This document defines a modular, agent-based execution framework for the InvoiceApp standalone desktop application. Agent definitions are **not predefined**. Instead, they are **instantiated dynamically** per task based on contextual inference by the orchestrator.

---

## Execution Philosophy
- The **Root Orchestrator** receives a user task and decomposes it into discrete, logically ordered sub-tasks.
- **Agents are not static**. For each sub-task, a new agent is instantiated with a unique identifier, clear scope, explicit input/output boundaries, and strict invariants.
- Agents exist only for the duration of their task, and may be composed or nested.

---

## Agent Declaration Format (for orchestration)
```yaml
- id: <agent_id>
  goal: <precise, single-purpose task goal>
  input: [<files, components, or prior outputs>]
  output: [<generated artifacts or modifications>]
  depends_on: [<agent_ids>]
  constraints: [<optional hard limits>]
  external: true|false  # true if human or out-of-container validation required
```

---

## Constraints
- No agent may operate outside its declared input/output boundaries.
- No agent may modify unrelated files or cross architectural boundaries.
- Agents that require rendering, interaction, or runtime execution must be marked `external: true`.

---

## Validation
- Every workflow must include at least one validation step: automated (e.g., diff check, syntax pass) or external (e.g., manual review).
- Fallback mechanisms should be included when runtime validation is blocked (e.g., Codex container limitations).

---

## Final Step
- A concluding agent (e.g., `aggregator`, `synthesizer`) must merge, summarize, or present the composite result of the workflow.
- It must not generate new artifacts—only consolidate.

---

## Notes
- There are no persistent agent roles.
- The Orchestrator dynamically assigns responsibilities based on each user task.
- This AGENTS.md serves as an execution policy, not an agent registry.

Maintained by: root_orchestrator  
Last updated: 2025-07-14
