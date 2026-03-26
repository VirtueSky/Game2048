## Why

Xây dựng game 2048 hoàn chỉnh trên nền tảng Unity với C# nhằm cung cấp một sản phẩm game mobile/desktop có khả năng mở rộng, dễ bảo trì, và là nền tảng học tập OOP trong phát triển game. Game 2048 là bài toán kinh điển kết hợp logic gộp ô, quản lý trạng thái, và UI động — phù hợp để thể hiện kiến trúc sạch trong Unity.

## What Changes

- Tạo mới toàn bộ dự án Unity game 2048 từ đầu
- Xây dựng hệ thống Board 4x4 với logic di chuyển và gộp ô theo 4 hướng (lên, xuống, trái, phải)
- Tạo hệ thống Tile hiển thị giá trị 2, 4, 8, ..., 2048 với màu sắc tương ứng
- Implement GameManager điều phối toàn bộ game loop (khởi tạo, thắng, thua, reset)
- Thêm hệ thống điểm số (Score) và điểm cao nhất (Best Score) với lưu trữ local (PlayerPrefs)
- Implement tính năng Undo (hoàn tác nước đi gần nhất)
- Xây dựng UI hoàn chỉnh: bảng game, điểm số, nút New Game, nút Undo, màn hình thắng/thua
- Hỗ trợ input từ bàn phím (WASD / Arrow Keys) và swipe cảm ứng
- Lưu/tải trạng thái game để tiếp tục khi mở lại app

## Capabilities

### New Capabilities

- `board`: Quản lý lưới 4x4, logic di chuyển và gộp ô theo 4 hướng, kiểm tra trạng thái thắng/thua
- `tile`: Thực thể ô vuông với giá trị, animation xuất hiện/gộp, màu sắc theo giá trị
- `game-manager`: Điều phối game loop, xử lý input, quản lý trạng thái game tổng thể
- `score-system`: Theo dõi điểm hiện tại và điểm cao nhất, lưu trữ bền vững qua PlayerPrefs
- `undo-system`: Lưu snapshot trạng thái board sau mỗi nước đi, hoàn tác về trạng thái trước
- `save-load`: Serialize/deserialize trạng thái game để tiếp tục chơi giữa các phiên
- `ui`: Canvas UI với bảng điểm, nút điều khiển, panel thắng/thua, animation tile

### Modified Capabilities

## Impact

- Tạo mới hoàn toàn: không ảnh hưởng code hiện có
- Phụ thuộc: Unity 2022.3+ LTS, TextMeshPro cho UI text
- Sử dụng Unity Coplay MCP để tạo và cấu hình toàn bộ scene, GameObject, script trong Unity Editor
- PlayerPrefs làm storage backend cho score và save state
