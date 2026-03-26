## ADDED Requirements

### Requirement: GameManager điều phối game loop
GameManager SHALL là Singleton MonoBehaviour điều phối toàn bộ luồng game: khởi tạo, xử lý input, cập nhật trạng thái, kết thúc game.

#### Scenario: Khởi động game
- **WHEN** scene được load
- **THEN** GameManager.Start() khởi tạo Board, BoardView, ScoreSystem, UndoSystem, cố gắng load save game

#### Scenario: Singleton enforcement
- **WHEN** có nhiều hơn một GameManager trong scene
- **THEN** instance thứ hai bị Destroy, chỉ instance đầu tiên tồn tại

### Requirement: Xử lý input di chuyển
GameManager SHALL nhận input từ InputHandler và thực thi nước đi tương ứng trên Board.

#### Scenario: Nhận input hợp lệ
- **WHEN** người chơi nhấn phím Arrow/WASD hoặc swipe
- **THEN** GameManager gọi Board.Move(direction), cập nhật score, lưu undo snapshot, cập nhật BoardView

#### Scenario: Block input khi animation đang chạy
- **WHEN** animation tile chưa hoàn thành và người chơi nhấn input
- **THEN** input bị bỏ qua, không thực thi nước đi mới

### Requirement: Xử lý thắng game
GameManager SHALL phát hiện và xử lý trạng thái thắng (có ô 2048).

#### Scenario: Người chơi đạt 2048
- **WHEN** Board.IsWon trả về true sau một nước đi
- **THEN** GameManager hiển thị win panel, dừng nhận input, lưu best score

#### Scenario: Tiếp tục chơi sau khi thắng
- **WHEN** người chơi bấm "Continue" trên win panel
- **THEN** win panel ẩn, game tiếp tục (không check win nữa cho đến khi reset)

### Requirement: Xử lý thua game
GameManager SHALL phát hiện và xử lý trạng thái thua (không còn nước đi).

#### Scenario: Người chơi thua
- **WHEN** Board.IsGameOver trả về true
- **THEN** GameManager hiển thị game over panel, dừng nhận input

### Requirement: New Game
GameManager SHALL reset toàn bộ trạng thái game khi người chơi chọn New Game.

#### Scenario: Reset game
- **WHEN** GameManager.NewGame() được gọi
- **THEN** Board reset, score reset về 0, BoardView xóa toàn bộ tile, undo cleared, save bị xóa, 2 tile mới xuất hiện
