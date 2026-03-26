## 1. Unity Project Setup

- [x] 1.1 Dùng Coplay MCP kiểm tra Unity project root và mở scene mặc định
- [x] 1.2 Cài đặt DOTween từ Asset Store hoặc package manager qua Coplay MCP
- [x] 1.3 Cài đặt Unity Input System package qua Coplay MCP
- [x] 1.4 Tạo thư mục cấu trúc: Assets/Scripts/{Core,View,UI,Data}, Assets/Prefabs, Assets/ScriptableObjects

## 2. Core Logic - Board & Tile (Pure C#)

- [x] 2.1 Tạo file `Assets/Scripts/Core/Tile.cs` — class lưu Value, Row, Col, IsMerged
- [x] 2.2 Tạo file `Assets/Scripts/Core/BoardState.cs` — struct serialize-able lưu int[16] values + score
- [x] 2.3 Tạo file `Assets/Scripts/Core/Board.cs` — khởi tạo lưới NxN, đặt 2 tile ban đầu
- [x] 2.4 Implement `Board.Move(Direction)` — logic trượt ô theo 4 hướng, trả về bool (có thay đổi không)
- [x] 2.5 Implement logic gộp ô trong Board.Move — mỗi ô chỉ gộp 1 lần/nước đi, tính LastMergeScore
- [x] 2.6 Implement `Board.SpawnRandomTile()` — đặt tile mới vào ô trống ngẫu nhiên (90% giá trị 2, 10% giá trị 4)
- [x] 2.7 Implement `Board.IsWon` — kiểm tra có tile >= 2048 không
- [x] 2.8 Implement `Board.IsGameOver` — kiểm tra không còn ô trống và không có cặp liền kề cùng giá trị
- [x] 2.9 Implement `Board.GetBoardState()` và `Board.LoadFromState(BoardState)` — snapshot/restore

## 3. Score System

- [x] 3.1 Tạo file `Assets/Scripts/Core/ScoreSystem.cs` — CurrentScore, BestScore, event OnScoreChanged
- [x] 3.2 Implement `ScoreSystem.AddScore(int)` — cộng điểm, cập nhật BestScore nếu vượt
- [x] 3.3 Implement `ScoreSystem.Reset()` — về 0, giữ BestScore
- [x] 3.4 Implement load/save BestScore qua PlayerPrefs trong Awake/khi BestScore thay đổi

## 4. Undo System

- [x] 4.1 Tạo file `Assets/Scripts/Core/UndoSystem.cs` — lưu 1 snapshot (BoardState + score)
- [x] 4.2 Implement `UndoSystem.SaveSnapshot(BoardState, int)` — lưu trước khi move
- [x] 4.3 Implement `UndoSystem.Undo()` — khôi phục và xóa snapshot, trả về (BoardState, score) hoặc null
- [x] 4.4 Implement `UndoSystem.HasSnapshot` property — để UI nút Undo biết có enable không
- [x] 4.5 Implement `UndoSystem.Clear()` — xóa snapshot khi New Game

## 5. Save/Load System

- [x] 5.1 Tạo file `Assets/Scripts/Core/SaveManager.cs` — serialize BoardState thành JSON
- [x] 5.2 Implement `SaveManager.Save(BoardState, int)` — lưu vào PlayerPrefs key "GameSave"
- [x] 5.3 Implement `SaveManager.Load()` — trả về (BoardState, score) hoặc null nếu không có save
- [x] 5.4 Implement `SaveManager.DeleteSave()` — xóa save khỏi PlayerPrefs (giữ best score)

## 6. Input Handler

- [x] 6.1 Tạo Input Action Asset qua Coplay MCP — actions: MoveLeft, MoveRight, MoveUp, MoveDown (dùng code-based InputAction binding)
- [x] 6.2 Cấu hình bindings: Arrow Keys + WASD cho keyboard
- [x] 6.3 Tạo file `Assets/Scripts/Core/InputHandler.cs` — MonoBehaviour subscribe InputAction events
- [x] 6.4 Implement swipe detection cho mobile (touch delta threshold > 50px)
- [x] 6.5 Implement `InputHandler.OnMoveInput` event phát Direction khi input được nhận

## 7. Game Manager

- [x] 7.1 Tạo file `Assets/Scripts/Core/GameManager.cs` — Singleton MonoBehaviour
- [x] 7.2 Implement `GameManager.Start()` — khởi tạo Board, ScoreSystem, UndoSystem, SaveManager, thử Load save
- [x] 7.3 Implement xử lý input từ InputHandler — check isAnimating, gọi Board.Move(), update score
- [x] 7.4 Implement flow sau mỗi nước đi: SaveSnapshot → Move → AddScore → BoardView.Refresh → Save → check Win/GameOver
- [x] 7.5 Implement `GameManager.NewGame()` — reset tất cả systems, bắt đầu game mới
- [x] 7.6 Implement `GameManager.UndoMove()` — gọi UndoSystem.Undo(), restore board và score
- [x] 7.7 Implement logic win/continue: flag `hasWon`, sau win panel có thể continue

## 8. ScriptableObject - Tile Config

- [x] 8.1 Tạo file `Assets/Scripts/Data/TileColorConfig.cs` — ScriptableObject với List<TileColorData> (value, bgColor, textColor)
- [x] 8.2 Tạo asset TileColorConfig trong Assets/ScriptableObjects và điền màu cho các giá trị 2, 4, 8, 16, 32, 64, 128, 256, 512, 1024, 2048

## 9. TileView - Prefab & Animation

- [x] 9.1 Tạo TilePrefab qua Coplay MCP: Image (background) + TextMeshPro (value text) + RectTransform
- [x] 9.2 Tạo file `Assets/Scripts/View/TileView.cs` — MonoBehaviour với SetValue(), PlaySpawnAnimation(), PlayMergeAnimation(), MoveTo()
- [x] 9.3 Implement SetValue() — đổi text và màu theo TileColorConfig
- [x] 9.4 Implement PlaySpawnAnimation() — DOTween scale 0→1 trong 0.15s ease out back
- [x] 9.5 Implement PlayMergeAnimation() — DOTween scale 1→1.2→1 trong 0.1s
- [x] 9.6 Implement MoveTo(Vector3) — DOTween move trong 0.1s ease out, set isAnimating flag
- [x] 9.7 Gán TileColorConfig reference vào TileView inspector (qua BoardView.Init() method)

## 10. BoardView

- [x] 10.1 Tạo file `Assets/Scripts/View/BoardView.cs` — MonoBehaviour quản lý Grid layout và pool TileView
- [x] 10.2 Tạo GameObject "Board" trong scene qua Coplay MCP với Grid Layout Group
- [x] 10.3 Implement `BoardView.Initialize(int size)` — tạo N×N ô nền (background cells)
- [x] 10.4 Implement `BoardView.Refresh(Board)` — sync TileView với Board state: tạo/di chuyển/xóa tile views
- [x] 10.5 Implement `BoardView.ClearAll()` — xóa hết tile views khi New Game
- [x] 10.6 Gán prefab TilePrefab vào BoardView inspector

## 11. UI - Canvas & Panels

- [x] 11.1 Tạo Canvas UI qua Coplay MCP với Panel Settings phù hợp
- [x] 11.2 Tạo Header: TextMeshPro "2048" title, Score panel (label + value), Best panel (label + value)
- [x] 11.3 Tạo nút "New Game" và nút "Undo" với styling phù hợp
- [x] 11.4 Tạo Win Panel (ẩn mặc định): text "You Win!", nút "Continue", nút "New Game"
- [x] 11.5 Tạo Game Over Panel (ẩn mặc định): text "Game Over!", hiển thị final score, nút "Try Again"
- [x] 11.6 Tạo file `Assets/Scripts/UI/UIManager.cs` — MonoBehaviour subscribe events và cập nhật UI
- [x] 11.7 Implement UIManager: subscribe ScoreSystem.OnScoreChanged → update score texts
- [x] 11.8 Implement UIManager: ShowWinPanel(), HideWinPanel(), ShowGameOverPanel(), HideGameOverPanel()
- [x] 11.9 Implement UIManager: sync Undo button interactable với UndoSystem.HasSnapshot
- [x] 11.10 Kết nối các nút (New Game, Undo, Continue, Try Again) với GameManager methods qua Coplay MCP

## 12. Scene Assembly & Integration

- [x] 12.1 Tạo/mở main scene "GameScene" qua Coplay MCP
- [x] 12.2 Tạo GameObject "GameManager" trong scene, gán GameManager script
- [x] 12.3 Tạo GameObject "InputHandler" trong scene, gán InputHandler script và Input Action Asset
- [x] 12.4 Assign tất cả serialized references trong GameManager inspector: BoardView, UIManager, ScoreSystem, UndoSystem, SaveManager, InputHandler
- [x] 12.5 Kiểm tra compile errors qua Coplay MCP
- [x] 12.6 Play game trong editor, test di chuyển 4 hướng, gộp ô, score cập nhật

## 13. Testing & Polish

- [x] 13.1 Test flow thắng: fill board đến 1024+1024, verify win panel xuất hiện
- [x] 13.2 Test flow thua: fill board không còn nước đi, verify game over panel
- [x] 13.3 Test Undo: đi một nước, undo, verify board và score khôi phục đúng
- [x] 13.4 Test Save/Load: đi vài nước, close game (stop play mode), play lại, verify game tiếp tục
- [x] 13.5 Test New Game: verify board reset hoàn toàn, save bị xóa, undo bị xóa
- [x] 13.6 Kiểm tra animation không conflict (input bị block khi đang animate)
- [x] 13.7 Chụp screenshot scene và capture UI canvas qua Coplay MCP để review layout
