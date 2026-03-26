## ADDED Requirements

### Requirement: Lưu snapshot trước mỗi nước đi
UndoSystem SHALL lưu một snapshot của trạng thái board (mảng giá trị 4x4) và điểm số trước khi thực thi nước đi.

#### Scenario: Lưu snapshot
- **WHEN** UndoSystem.SaveSnapshot(boardState, score) được gọi trước khi Board.Move()
- **THEN** snapshot được lưu trong bộ nhớ, nút Undo được kích hoạt

### Requirement: Hoàn tác nước đi gần nhất
UndoSystem SHALL khôi phục trạng thái board và điểm số về snapshot đã lưu khi người chơi nhấn Undo.

#### Scenario: Undo thành công
- **WHEN** UndoSystem.Undo() được gọi và có snapshot
- **THEN** board state được khôi phục, score được khôi phục, snapshot bị xóa, nút Undo bị disabled

#### Scenario: Không có gì để undo
- **WHEN** UndoSystem.Undo() được gọi nhưng không có snapshot (đầu game hoặc đã undo rồi)
- **THEN** không có gì xảy ra, nút Undo vẫn disabled

### Requirement: Chỉ hỗ trợ 1 nước undo
UndoSystem SHALL chỉ lưu snapshot của nước đi gần nhất. Không thể undo nhiều hơn 1 lần liên tiếp.

#### Scenario: Undo liên tiếp bị chặn
- **WHEN** người chơi nhấn Undo lần thứ 2 ngay sau lần đầu (chưa đi thêm nước nào)
- **THEN** nút Undo bị disabled, hành động bị chặn

### Requirement: Xóa snapshot khi New Game
UndoSystem SHALL xóa snapshot khi game reset.

#### Scenario: New Game xóa undo history
- **WHEN** UndoSystem.Clear() được gọi
- **THEN** snapshot bị xóa, nút Undo bị disabled