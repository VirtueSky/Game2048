# Game2048

Một game 2048 được phát triển bằng Unity và C# với OpenSpec workflow.

## 📋 Hướng dẫn phát triển

### File Prompt
File prompt gốc chứa yêu cầu và thiết kế game nằm tại:
```
docs/Prompt.md
```

File này bao gồm:
- Yêu cầu ban đầu cho game 2048
- Logic game cơ bản
- Cách chơi hiệu quả
- Hướng dẫn sử dụng Unity Coplay MCP
- Các yêu cầu về clean code và OOP

### OpenSpec Proposal Structure

Cấu trúc proposal OpenSpec cho dự án unity-2048-game nằm tại:
```
openspec/changes/unity-2048-game/
```

#### Cấu trúc chi tiết:

```
openspec/changes/unity-2048-game/
├── .openspec.yaml          # Cấu hình OpenSpec
├── proposal.md             # Đề xuất tổng quan dự án
├── design.md               # Thiết kế kiến trúc và UI
├── tasks.md                # Danh sách các task cần thực hiện
└── specs/                  # Thư mục chứa các specification chi tiết
    ├── board/
    │   └── spec.md         # Spec cho Board system
    ├── tile/
    │   └── spec.md         # Spec cho Tile system
    ├── game-manager/
    │   └── spec.md         # Spec cho Game Manager
    ├── ui/
    │   └── spec.md         # Spec cho UI system
    ├── score-system/
    │   └── spec.md         # Spec cho Score system
    ├── save-load/
    │   └── spec.md         # Spec cho Save/Load system
    └── undo-system/
        └── spec.md         # Spec cho Undo system
```

#### Các tài liệu quan trọng:

- **Proposal**: `openspec/changes/unity-2048-game/proposal.md`
- **Design**: `openspec/changes/unity-2048-game/design.md`
- **Tasks**: `openspec/changes/unity-2048-game/tasks.md`
- **Specifications**: `openspec/changes/unity-2048-game/specs/*/spec.md`

## 🎮 Game Features

- Bảng chơi 4x4 với cơ chế trượt và gộp ô
- Hệ thống điểm số
- Lưu/Load game
- Undo system
- UI trực quan và dễ sử dụng
- OOP design pattern với clean code

## 🛠️ Tech Stack

- Unity
- C#
- Unity Coplay MCP (cho automation)
- OpenSpec workflow

## 📦 Cấu hình OpenSpec

File cấu hình chính: `openspec/config.yaml`

Schema sử dụng: `spec-driven`