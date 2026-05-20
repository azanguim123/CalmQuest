# 🤝 Contributing to CalmQuest

Thank you for your interest in contributing to CalmQuest!

---

## 🌿 Branch Strategy

We follow a simplified **Git Flow**:

```
main          ← stable, production-ready
develop       ← integration branch (default for PRs)
feature/*     ← new features
fix/*         ← bug fixes
docs/*        ← documentation only
refactor/*    ← code cleanup, no new features
```

### Examples
```bash
git checkout -b feature/emotion-detector
git checkout -b fix/storm-calmer-crash
git checkout -b docs/update-readme
```

---

## ✍️ Commit Convention

We use **Conventional Commits**:

```
<type>(<scope>): <short description>
```

### Types
| Type | When to use |
|------|-------------|
| `feat` | New feature |
| `fix` | Bug fix |
| `docs` | Documentation only |
| `style` | Formatting, no logic change |
| `refactor` | Code restructure, no feature/fix |
| `test` | Adding or fixing tests |
| `chore` | Build process, dependencies |

### Scopes
`ai`, `unity`, `ui`, `minigame`, `bridge`, `docs`, `ci`

### Examples
```
feat(ai): add real-time emotion detection pipeline
fix(unity): resolve camera feed null reference
docs(readme): update project structure section
feat(minigame): implement StormCalmer breathing mechanic
refactor(ai): extract preprocessing into separate module
```

---

## 🔄 Workflow

1. Fork or branch from `develop`
2. Write code + tests
3. Commit with conventional format
4. Push and open a Pull Request → `develop`
5. At milestone: `develop` → `main` via PR

---

## 🧪 Tests

- Python: `pytest ai/tests/`
- Always test emotion detection accuracy before pushing AI changes

---

## ❓ Questions

Open an issue with the label `question`.
