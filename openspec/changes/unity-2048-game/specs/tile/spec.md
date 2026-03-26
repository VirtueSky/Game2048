## ADDED Requirements

### Requirement: Tile lưu trữ giá trị và vị trí
Tile SHALL lưu giá trị nguyên (power of 2: 2, 4, 8, ..., 2048) và vị trí (row, col) trong lưới board.

#### Scenario: Tạo tile mới
- **WHEN** Board tạo một Tile tại vị trí (row, col) với giá trị V
- **THEN** Tile.Value == V, Tile.Row == row, Tile.Col == col

### Requirement: TileView hiển thị màu sắc theo giá trị
TileView SHALL hiển thị màu nền và màu chữ khác nhau cho mỗi giá trị tile (2 đến 2048+). Màu sắc SHALL được định nghĩa trong ScriptableObject TileColorConfig.

#### Scenario: Màu tile theo giá trị
- **WHEN** TileView.SetValue(value) được gọi
- **THEN** background color và text color của tile thay đổi theo bảng màu tương ứng với value

#### Scenario: Giá trị vượt 2048
- **WHEN** tile có giá trị > 2048
- **THEN** hiển thị màu fallback (màu cuối trong config) thay vì crash

### Requirement: Animation tile xuất hiện
TileView SHALL hiển thị animation scale-in (0 → 1) khi tile mới được tạo.

#### Scenario: Tile mới spawn
- **WHEN** TileView.PlaySpawnAnimation() được gọi
- **THEN** tile scale từ 0 lên 1 trong 0.15 giây với ease out back

### Requirement: Animation tile gộp
TileView SHALL hiển thị animation pop (scale lên 1.2 rồi về 1) khi tile gộp thành công.

#### Scenario: Tile gộp
- **WHEN** TileView.PlayMergeAnimation() được gọi
- **THEN** tile scale lên 1.2 rồi về 1.0 trong 0.1 giây

### Requirement: Animation tile di chuyển
TileView SHALL di chuyển mượt từ vị trí cũ đến vị trí mới khi board thay đổi.

#### Scenario: Di chuyển tile
- **WHEN** TileView.MoveTo(targetPosition) được gọi
- **THEN** tile di chuyển từ vị trí hiện tại đến targetPosition trong 0.1 giây với ease out
