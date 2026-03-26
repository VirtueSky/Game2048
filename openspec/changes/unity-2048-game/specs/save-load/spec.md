## ADDED Requirements

### Requirement: Lưu trạng thái game tự động
SaveManager SHALL tự động lưu trạng thái game (board values, score, best score) vào PlayerPrefs sau mỗi nước đi hợp lệ.

#### Scenario: Auto-save sau nước đi
- **WHEN** một nước đi hợp lệ được thực thi
- **THEN** SaveManager.Save() được gọi, serialize BoardState thành JSON và lưu vào PlayerPrefs

#### Scenario: Lưu game over state
- **WHEN** game kết thúc (thắng hoặc thua)
- **THEN** trạng thái cuối vẫn được lưu (không xóa save khi game over)

### Requirement: Tải trạng thái game khi khởi động
SaveManager SHALL tải và khôi phục trạng thái game đã lưu khi khởi động app.

#### Scenario: Load game khi có save
- **WHEN** app khởi động và có save data trong PlayerPrefs
- **THEN** board state được restore, score được restore, game tiếp tục từ điểm đã dừng

#### Scenario: Không có save data
- **WHEN** app khởi động và không có save data
- **THEN** game bắt đầu mới với 2 tile ban đầu

### Requirement: Xóa save khi New Game
SaveManager SHALL xóa save data khi người chơi chọn New Game.

#### Scenario: New Game xóa save
- **WHEN** SaveManager.DeleteSave() được gọi
- **THEN** tất cả save data bị xóa khỏi PlayerPrefs (trừ best score)

### Requirement: Cấu trúc dữ liệu save
SaveManager SHALL serialize/deserialize BoardState bao gồm: mảng giá trị board (int[,]), score hiện tại (int).

#### Scenario: Serialize thành công
- **WHEN** SaveManager.Save(boardState, currentScore) được gọi
- **THEN** data được convert sang JSON string hợp lệ và lưu vào PlayerPrefs key "GameSave"