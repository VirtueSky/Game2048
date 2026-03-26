## ADDED Requirements

### Requirement: Theo dõi điểm hiện tại
ScoreSystem SHALL duy trì điểm số hiện tại của ván chơi, cộng thêm điểm sau mỗi lần gộp tile.

#### Scenario: Cộng điểm khi gộp
- **WHEN** ScoreSystem.AddScore(points) được gọi với giá trị tổng từ lần gộp
- **THEN** ScoreSystem.CurrentScore tăng thêm points, UIManager nhận event cập nhật UI

#### Scenario: Reset điểm khi New Game
- **WHEN** ScoreSystem.Reset() được gọi
- **THEN** CurrentScore về 0

### Requirement: Lưu trữ Best Score
ScoreSystem SHALL lưu điểm cao nhất qua các ván chơi bằng PlayerPrefs.

#### Scenario: Cập nhật best score
- **WHEN** CurrentScore vượt quá BestScore
- **THEN** BestScore được cập nhật và lưu ngay vào PlayerPrefs

#### Scenario: Load best score khi khởi động
- **WHEN** ScoreSystem được khởi tạo lần đầu
- **THEN** BestScore được load từ PlayerPrefs (mặc định 0 nếu chưa có)

### Requirement: Thông báo thay đổi điểm qua event
ScoreSystem SHALL phát event khi điểm thay đổi để UI có thể cập nhật.

#### Scenario: Event điểm thay đổi
- **WHEN** CurrentScore hoặc BestScore thay đổi
- **THEN** ScoreSystem.OnScoreChanged event được invoke với giá trị mới