## Context

Dự án xây dựng game 2048 hoàn chỉnh trong Unity 2022.3+ LTS sử dụng C#. Unity Coplay MCP được dùng để điều khiển Unity Editor (tạo scene, GameObject, gán script, cấu hình component) thay vì thao tác thủ công. Game chạy trên PC (keyboard) và mobile (swipe). Không có backend — toàn bộ lưu trữ dùng PlayerPrefs.

## Goals / Non-Goals

**Goals:**
- Kiến trúc OOP rõ ràng: mỗi class có trách nhiệm đơn lẻ (SRP)
- Logic game tách biệt hoàn toàn khỏi Unity MonoBehaviour (testable pure C#)
- UI phản hồi nhanh với animation tile (xuất hiện, gộp, di chuyển)
- Hỗ trợ Undo 1 nước đi
- Lưu/tải game tự động
- Code dễ mở rộng: thay đổi kích thước board (4x4 → 5x5) không cần refactor lớn

**Non-Goals:**
- Multiplayer hoặc leaderboard online
- Lưu nhiều hơn 1 trạng thái undo
- Hỗ trợ nhiều chủ đề giao diện (theme)
- Tích hợp analytics hoặc ads

## Decisions

### D1: Tách Model khỏi View (MVC-lite)

**Quyết định**: `Board` và `Tile` là pure C# class (không kế thừa MonoBehaviour), chứa toàn bộ logic game. `BoardView` và `TileView` là MonoBehaviour chịu trách nhiệm render.

**Lý do**: Logic game có thể unit-test không cần Unity runtime. View có thể thay đổi mà không ảnh hưởng logic.

**Thay thế xem xét**: Gộp logic và view vào một MonoBehaviour — đơn giản hơn nhưng khó test và bảo trì.

---

### D2: GameManager là Singleton MonoBehaviour

**Quyết định**: `GameManager` dùng pattern Singleton để các component khác dễ truy cập. Kế thừa MonoBehaviour để tích hợp với Unity lifecycle.

**Lý do**: Unity không có DI container built-in; Singleton là pattern phổ biến và dễ hiểu cho game nhỏ. Tránh over-engineering với DI framework.

**Thay thế**: ServiceLocator pattern — linh hoạt hơn nhưng phức tạp không cần thiết ở quy mô này.

---

### D3: Undo chỉ lưu 1 snapshot

**Quyết định**: `UndoSystem` lưu một `BoardState` snapshot (mảng int 4x4 + score). Chỉ undo được 1 nước.

**Lý do**: Game 2048 gốc chỉ hỗ trợ 1 undo. Lưu stack nhiều state tốn bộ nhớ hơn và thay đổi gameplay.

**Thay thế**: Stack<BoardState> cho multi-undo — dễ mở rộng sau nếu cần, thiết kế hiện tại không block hướng này.

---

### D4: Serialization bằng JSON + PlayerPrefs

**Quyết định**: Serialize `BoardState` thành JSON string, lưu vào PlayerPrefs với key cố định.

**Lý do**: PlayerPrefs là giải pháp built-in của Unity, không cần dependency ngoài. JsonUtility của Unity đủ dùng cho struct đơn giản.

**Thay thế**: Binary serialization — nhanh hơn nhưng không human-readable và khó debug.

---

### D5: Animation dùng DOTween (hoặc LeanTween)

**Quyết định**: Dùng DOTween (free tier) cho animation tile (scale pop khi xuất hiện, di chuyển mượt, flash khi gộp).

**Lý do**: DOTween là thư viện phổ biến nhất trong Unity ecosystem, API đơn giản, hiệu năng tốt. Tránh viết coroutine animation thủ công.

**Thay thế**: Unity Animator — quá phức tạp cho animation procedural đơn giản. Coroutine thuần — verbose và khó đọc.

---

### D6: Input dùng Unity New Input System

**Quyết định**: Dùng Unity Input System package với InputAction asset cho keyboard (WASD + Arrow) và swipe gesture.

**Lý do**: Hỗ trợ cả keyboard và touch trong một hệ thống, dễ rebind, là hướng phát triển chính thức của Unity.

**Thay thế**: Input.GetKeyDown() cũ — đơn giản hơn nhưng không hỗ trợ touch tốt và là legacy API.

## Kiến trúc tổng thể

```
┌─────────────────────────────────────────────────────┐
│                    Unity Scene                       │
│  ┌─────────────┐    ┌──────────────┐                 │
│  │ GameManager │◄───│ InputHandler │                 │
│  │ (Singleton) │    └──────────────┘                 │
│  └──────┬──────┘                                     │
│         │ owns                                       │
│  ┌──────▼──────┐    ┌──────────────┐                 │
│  │    Board    │───►│  BoardView   │                 │
│  │ (pure C#)   │    │(MonoBehaviour│                 │
│  └──────┬──────┘    └──────┬───────┘                 │
│         │ contains         │ instantiates            │
│  ┌──────▼──────┐    ┌──────▼───────┐                 │
│  │    Tile     │    │   TileView   │                 │
│  │ (pure C#)   │    │(MonoBehaviour│                 │
│  └─────────────┘    └──────────────┘                 │
│                                                      │
│  ┌─────────────┐    ┌──────────────┐                 │
│  │ ScoreSystem │    │  UndoSystem  │                 │
│  └─────────────┘    └──────────────┘                 │
│                                                      │
│  ┌─────────────┐    ┌──────────────┐                 │
│  │ SaveManager │    │   UIManager  │                 │
│  └─────────────┘    └──────────────┘                 │
└─────────────────────────────────────────────────────┘
```

## Risks / Trade-offs

- **[Risk] DOTween dependency** → Mitigation: Dùng DOTween free từ Asset Store; nếu không available, fallback về coroutine animation đơn giản.
- **[Risk] PlayerPrefs bị xóa** → Mitigation: Chấp nhận — đây là behavior mong đợi (xóa app = xóa data). Không cần migration path.
- **[Risk] Animation race condition** (tile chưa xong animation đã nhận input mới) → Mitigation: Lock input trong khi animation đang chạy bằng flag `isAnimating`.
- **[Risk] Board size change breaks layout** → Mitigation: BoardView tính toán kích thước ô động dựa trên kích thước board, không hardcode.
