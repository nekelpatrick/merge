# Editor Script Archive

This folder contains editor scripts that have completed their purpose but are preserved for reference.

## Phase 5.5 Setup Scripts

Located in `Phase5_5_Setup/`:

- **Phase5_5_PrefabCreator.cs** - Created Phase 5.5 UI prefabs (ActionPreviewItem, RuneBadge, PhaseBannerUI, EnemyIntentIndicator)
- **Phase5_5_SceneIntegrator.cs** - Integrated Phase 5.5 UI components into Battle scene

**Status:** Completed. Prefabs and scene integration exist. Scripts kept for reference if scene recreation is needed.

## Migration Scripts

Located in `MigrationScripts/`:

- **FixHealthDisplayInPrefabs.cs** - One-time prefab migration for health display components
- **FixEnemyMaterials.cs** - Material assignment fix for enemy prefabs  
- **InstallGLTFPackage.cs** - Package installer utility for GLTF support

**Status:** Completed. Fixes applied and packages installed. Scripts kept for historical reference.

## Why Archive Instead of Delete?

These scripts are archived rather than deleted because:

1. **Documentation** - They show how systems were set up
2. **Reproducibility** - Useful if scenes/prefabs need recreation
3. **Reference** - Future similar migrations can learn from these patterns
4. **History** - Preserves project development history

## Can These Be Deleted?

Yes, if needed for cleanup. The functionality they provide has been completed and persisted in assets/scenes.

---

**Archived:** December 14, 2025  
**Reason:** Dead code cleanup - Phase 4
