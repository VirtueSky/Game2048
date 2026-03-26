- /opsx:propose
- Tạo game 2048 bằng unity và c#.
- Sử dụng unity coplay mcp để control unity editor
- yêu cầu clean code, dễ hiểu, dễ bảo trì
- các chức năng cơ bản của game 2048 như: di chuyển, gộp ô, tạo ô mới, kiểm tra thắng thua
- sử dụng OOP để tổ chức code, có thể tạo các class như GameManager, Tile, Board, v.v.
- có thể thêm các tính năng nâng cao như lưu điểm, lưu trạng thái game, hỗ trợ undo, v.v.
- đảm bảo code có thể mở rộng trong tương lai nếu muốn thêm các tính năng mới hoặc thay đổi cách chơi.
- sử dụng sự trợ giúp của c# expert, unity expert để tối ưu code và giải quyết các vấn đề phát sinh trong quá trình phát triển game.

Đây là logic cơ bản và cách chơi hiệu quả của game 2048:
Logic game 2048:

Bảng 4x4, mỗi ô chứa số (2, 4, 8…).
Khi vuốt (lên/xuống/trái/phải), tất cả ô trượt theo hướng đó.
Hai ô cùng giá trị va vào nhau → gộp thành 1 ô có giá trị gấp đôi.
Sau mỗi lượt, spawn thêm 1 ô mới (2 hoặc 4).
Game kết thúc khi không còn nước đi hợp lệ.

Cách chơi hiệu quả:

Giữ số lớn nhất ở 1 góc (thường là góc dưới).
Ưu tiên vuốt theo 2 hướng chính để tránh phá cấu trúc.
Tạo chuỗi số giảm dần (snake) để dễ gộp.
Tránh vuốt lung tung → dễ “kẹt bàn”.

hãy check lỗi merge và fix