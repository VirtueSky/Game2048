---
name: Unity 2048 Game Project
description: Dự án game 2048 trong Unity với C# — trạng thái hoàn thành và các ghi chú kỹ thuật quan trọng
type: project
---

Dự án game 2048 hoàn chỉnh tại `/Users/duckhoa/Desktop/AIWork/Game2048`.

**Why:** Xây dựng game 2048 với kiến trúc OOP clean, dùng Coplay MCP để setup Unity.

**Trạng thái:** 76/76 tasks hoàn thành (2026-03-26). Tất cả 10 unit tests PASSED.

## Cấu trúc quan trọng

- `Assets/Scripts/Core/` — Pure C# logic: Board, Tile, BoardState, ScoreSystem, UndoSystem, SaveManager, GameManager, InputHandler
- `Assets/Scripts/View/` — TileView (DOTween animations), BoardView
- `Assets/Scripts/UI/` — UIManager
- `Assets/Scripts/Data/` — TileColorConfig (ScriptableObject)
- `Assets/Scripts/Editor/` — Editor scripts: SetupScene, GameTests, CreateTileColorConfig, FixLayout, FixBackground
- `Assets/Scenes/GameScene.unity` — Main scene
- `Assets/ScriptableObjects/TileColorConfig.asset` — Bảng màu tiles
- `Assets/Prefabs/TilePrefab.prefab`, `CellBackground.prefab`

## Ghi chú kỹ thuật

- `Direction` enum đặt trong Board.cs (đầu file trong namespace), KHÔNG phải file riêng — Unity không resolve cross-file enum trong cùng namespace lúc install Input System
- DOTween đã cài sẵn, Input System được cài qua MCP
- TMP fonts: dùng `LiberationSans SDF` — assign qua AssignTMPFonts.cs editor script
- Canvas: Scale With Screen Size, reference 1080x1920
- Board ở anchoredPosition (0, 250), Header (0, 820), ButtonBar (0, 650)

**How to apply:** Run `/opsx:archive unity-2048-game` khi muốn archive change.
