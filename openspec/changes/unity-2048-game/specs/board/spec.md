## ADDED Requirements

### Requirement: Board khởi tạo lưới 4x4
Board SHALL khởi tạo một lưới NxN (mặc định 4x4) với tất cả ô có giá trị 0, sau đó đặt ngẫu nhiên 2 ô với giá trị 2 hoặc 4.

#### Scenario: Khởi tạo board mới
- **WHEN** GameManager gọi Board.Initialize()
- **THEN** tất cả 16 ô có giá trị 0, sau đó 2 ô ngẫu nhiên được đặt giá trị 2 hoặc 4

#### Scenario: Xác suất tile xuất hiện
- **WHEN** một ô mới được tạo
- **THEN** 90% xác suất giá trị là 2, 10% xác suất giá trị là 4

### Requirement: Di chuyển ô theo 4 hướng
Board SHALL hỗ trợ di chuyển theo 4 hướng: Left, Right, Up, Down. Khi di chuyển, tất cả ô SHALL trượt về phía hướng di chuyển cho đến khi chạm vào tường hoặc ô khác.

#### Scenario: Di chuyển trái - ô trống
- **WHEN** Board.Move(Direction.Left) được gọi với board có ô trống bên trái các ô có giá trị
- **THEN** các ô trượt hết về phía trái, lấp đầy khoảng trống

#### Scenario: Di chuyển không hợp lệ
- **WHEN** Board.Move() được gọi nhưng không có ô nào thay đổi vị trí
- **THEN** phương thức trả về false (nước đi không hợp lệ), không thêm ô mới

#### Scenario: Di chuyển hợp lệ
- **WHEN** Board.Move() được gọi và ít nhất một ô thay đổi vị trí
- **THEN** phương thức trả về true, thêm một ô mới vào ô trống ngẫu nhiên

### Requirement: Gộp ô cùng giá trị
Board SHALL gộp 2 ô liền kề có cùng giá trị khi chúng trượt vào nhau. Giá trị ô sau gộp SHALL là tổng hai ô. Mỗi ô chỉ được gộp một lần trong một nước đi.

#### Scenario: Gộp hai ô bằng nhau
- **WHEN** hai ô có giá trị 2 trượt vào nhau theo hướng di chuyển
- **THEN** chúng gộp thành một ô có giá trị 4

#### Scenario: Không gộp liên tiếp
- **WHEN** ba ô có giá trị [2, 2, 2] di chuyển trái
- **THEN** kết quả là [4, 2, 0] — chỉ cặp đầu gộp, ô thứ 3 không gộp tiếp

#### Scenario: Gộp điểm được cộng
- **WHEN** hai ô có giá trị V gộp thành 2V
- **THEN** Board.LastMergeScore trả về tổng điểm từ các lần gộp trong nước đi đó

### Requirement: Kiểm tra trạng thái thắng
Board SHALL kiểm tra sau mỗi nước đi xem có ô nào đạt giá trị 2048 không.

#### Scenario: Phát hiện ô 2048
- **WHEN** hai ô 1024 gộp thành 2048
- **THEN** Board.IsWon trả về true

### Requirement: Kiểm tra trạng thái thua
Board SHALL kiểm tra trạng thái thua khi không còn nước đi hợp lệ nào.

#### Scenario: Board đầy không có gộp
- **WHEN** tất cả 16 ô đều có giá trị và không có 2 ô liền kề nào cùng giá trị
- **THEN** Board.IsGameOver trả về true

#### Scenario: Board đầy nhưng còn nước đi
- **WHEN** tất cả ô có giá trị nhưng có ít nhất một cặp ô liền kề cùng giá trị
- **THEN** Board.IsGameOver trả về false

### Requirement: Thêm ô mới sau mỗi nước đi
Board SHALL thêm một ô mới vào vị trí trống ngẫu nhiên sau mỗi nước đi hợp lệ.

#### Scenario: Thêm ô mới
- **WHEN** một nước đi hợp lệ được thực hiện
- **THEN** một ô trống ngẫu nhiên nhận giá trị 2 hoặc 4

#### Scenario: Không thêm ô khi board đầy
- **WHEN** không còn ô trống sau nước đi
- **THEN** không thêm ô mới, kiểm tra trạng thái thua ngay
