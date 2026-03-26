## ADDED Requirements

### Requirement: Hiển thị bảng điểm
UI SHALL hiển thị điểm hiện tại (Score) và điểm cao nhất (Best) ở đầu màn hình, cập nhật realtime khi điểm thay đổi.

#### Scenario: Cập nhật điểm
- **WHEN** ScoreSystem.OnScoreChanged event được phát
- **THEN** UI text Score và Best được cập nhật ngay lập tức

### Requirement: Nút New Game
UI SHALL có nút "New Game" mà khi nhấn sẽ gọi GameManager.NewGame() và reset toàn bộ game.

#### Scenario: Nhấn New Game
- **WHEN** người chơi nhấn nút "New Game"
- **THEN** GameManager.NewGame() được gọi, board được reset, 2 tile mới xuất hiện

### Requirement: Nút Undo
UI SHALL có nút "Undo" mà khi nhấn sẽ gọi UndoSystem.Undo(). Nút SHALL bị disabled khi không có snapshot.

#### Scenario: Undo disabled khi không có snapshot
- **WHEN** không có snapshot undo
- **THEN** nút Undo có alpha giảm và không thể nhấn

#### Scenario: Undo enabled khi có snapshot
- **WHEN** có snapshot undo (sau nước đi đầu tiên)
- **THEN** nút Undo active và có thể nhấn

### Requirement: Win Panel
UI SHALL hiển thị panel chúc mừng khi người chơi đạt 2048, với 2 lựa chọn: "Continue" và "New Game".

#### Scenario: Hiển thị win panel
- **WHEN** GameManager phát hiện Board.IsWon == true
- **THEN** win panel xuất hiện với animation fade-in, overlay mờ che board

#### Scenario: Continue sau khi thắng
- **WHEN** người chơi nhấn "Continue" trên win panel
- **THEN** win panel ẩn, game tiếp tục, không check win nữa

### Requirement: Game Over Panel
UI SHALL hiển thị panel "Game Over" khi không còn nước đi nào, với nút "Try Again".

#### Scenario: Hiển thị game over panel
- **WHEN** GameManager phát hiện Board.IsGameOver == true
- **THEN** game over panel xuất hiện với animation, hiển thị điểm cuối

#### Scenario: Try Again
- **WHEN** người chơi nhấn "Try Again"
- **THEN** game over panel ẩn, GameManager.NewGame() được gọi

### Requirement: BoardView render tiles
BoardView SHALL render lưới board với các ô nền và các TileView tương ứng với state của Board.

#### Scenario: Render sau nước đi
- **WHEN** board state thay đổi sau một nước đi
- **THEN** BoardView cập nhật vị trí và giá trị của tất cả TileView đang hiển thị

#### Scenario: Responsive layout
- **WHEN** game chạy trên màn hình có tỷ lệ khác nhau
- **THEN** board được center và scale phù hợp, không bị crop hoặc stretch