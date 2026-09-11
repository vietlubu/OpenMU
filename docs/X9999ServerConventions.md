# Quy ước máy chủ MU Online x9999 và mặc định đề xuất cho OpenMU

Ngày truy cập nguồn: **2026-09-11**.

## Phạm vi và độ tin cậy

Ghi chú này đối chiếu các trang do chính đơn vị vận hành máy chủ công bố. Các trang đó là tài liệu cấu hình/quảng bá của máy chủ tư nhân, không phải tài liệu chính thức của Webzen; thông số có thể thay đổi mà không báo trước. Mẫu chính gồm GhostMU, NoLimit, PikaMU, LegendaryMU và Dragon.MU x9999; InfinityMU và DragonMu x99999 chỉ được dùng làm mốc tham khảo cho kiểu máy chủ “instant/supermax” cực cao.

## Quan sát có nguồn

### 1. EXP và tỷ lệ rơi đồ

- GhostMU đang công bố realm x9999 với EXP x9999, drop 50%, cấp tối đa 400; một đợt mở x9999 trước đó trên cùng trang dùng drop 100%. Điều này cho thấy EXP x9999 khá ổn định nhưng drop toàn cục thường được điều chỉnh theo từng đợt, trong khoảng 50–100% ở chính máy chủ này ([GhostMU](https://ghost-mu.com/)).
- NoLimit công bố drop 80% cho các realm PK x9999. Trang này ghi EXP không nhất quán giữa phần mô tả (`99999x`) và bảng thống kê (`9999x`), vì vậy chỉ nên dùng giá trị drop và nhãn realm x9999 làm dữ kiện ([NoLimit](https://nolimit.lv/)).
- Dragon.MU công bố đồng thời EXP Regular, Master và Majestic đều x9999; reset ở cấp 400, giữ chỉ số và thưởng 1.000 điểm mỗi reset ([Dragon.MU x9999](https://dragon.mu/news/mu-online-x9999-season-20-part-1-3-dragon-mu/2)).
- PikaMU công bố EXP thường x9999, Master x5000, Majestic x3000 và drop 100%; mỗi reset ở cấp 400 thưởng 500 điểm cùng tiền tệ máy chủ ([PikaMU x9999](https://pikamu.net/news/season-21-part-1-2-custom/2)).
- Ở mốc cực đoan hơn, InfinityMU công bố EXP 9.999.999x và drop 100%, nhưng vẫn dành Ancient/Uber cho boss, Land of Trials và chế tạo. Nghĩa là “drop 100%” không đồng nghĩa mọi nhóm vật phẩm hiếm đều rơi với xác suất 100% ([InfinityMU server info](https://www.infinitymu.net/about-infinitymu)).

**Mẫu rút ra:** x9999 là hệ số EXP danh nghĩa phổ biến; drop vật phẩm thường nằm khoảng 50–100%, còn Excellent/Ancient/Socket và tiền tệ sự kiện phải có bảng xác suất riêng để không phá vòng săn đồ.

### 2. Điểm chỉ số khi đạt cấp 400

- GhostMU dùng 5/7 điểm mỗi cấp, cấp tối đa 400 ([GhostMU](https://ghost-mu.com/)). Nếu nhân vật bắt đầu ở cấp 1 và nhận điểm từ cấp 2 đến 400, phần điểm do lên cấp là `399 × 5 = 1.995` hoặc `399 × 7 = 2.793`, chưa gồm chỉ số gốc và thưởng khác.
- NoLimit cũng dùng 5/7/7 điểm mỗi cấp, cấp tối đa 400, đồng thời tặng sẵn 2.500 điểm cho các lớp thường và 3.000 điểm cho các lớp 7 điểm/cấp. Do đó tổng điểm tự do thực dụng ở cấp 400 xấp xỉ 4.495 hoặc 5.793, chưa gồm chỉ số gốc ([NoLimit](https://nolimit.lv/)).
- DragonMu x99999 “supermax” dùng tới 100 điểm mỗi cấp thường, tức khoảng 39.900 điểm chỉ từ lên cấp; đây là mốc cực đoan phù hợp PvP full-stat, không phải mặc định bảo thủ ([DragonMu x99999](https://dragonmu.net/news/read/48-dragonmu-x99999-supermax-opening-27february)).

**Mẫu rút ra:** nhiều realm x9999 vẫn giữ nhịp 5/7 điểm/cấp và tăng tốc bằng điểm khởi đầu hoặc điểm thưởng reset, thay vì cấp hàng chục nghìn điểm ngay trong lần lên cấp đầu tiên.

### 3. Độ rộng cửa hàng

- InfinityMU bán tại NPC Box of Kundun +1 đến +4, cánh và jewels; trang này vẫn giữ các bộ Ancient/Uber mạnh cho hoạt động săn boss, Land of Trials và chế tạo ([InfinityMU server info](https://www.infinitymu.net/about-infinitymu)).
- DragonMu x99999 bán jewels, boxes, vũ khí Excellent và toàn bộ kỹ năng tại NPC, đồng thời cho mở shop ở mọi nơi bằng lệnh `/npc` ([DragonMu x99999](https://dragonmu.net/news/read/48-dragonmu-x99999-supermax-opening-27february)).
- LegendaryMU x9999 bán vật phẩm đơn giản bằng WCoin, vật phẩm Full bằng Goblin Point kiếm trong game; các Elite Monster xuất hiện mỗi giờ và có bảng rơi riêng cho Ancient Full Option/cánh cao cấp ([LegendaryMU x9999](https://legendarymu.com/news/read/13-legendarymu-season-21)).
- NoLimit đi xa hơn bằng bộ khởi đầu Full Option +15 và WebShop có tối đa 6 Excellent options/5 socket options; đây là mô hình “freebies/full-option”, không phải mức tối thiểu cần có của x9999 ([NoLimit](https://nolimit.lv/)).

**Mẫu rút ra:** shop instant thường loại bỏ ma sát ở vật phẩm tiêu hao, kỹ năng, jewels, boxes và trang bị khởi đầu; vật phẩm endgame nên tiếp tục gắn với boss/sự kiện để người chơi còn mục tiêu săn đồ.

### 4. Mật độ quái và điểm luyện cấp

- PikaMU x9999 đặt 8–11 quái mỗi spot, hồi sinh sau 5 giây ([PikaMU x9999](https://pikamu.net/news/season-21-part-1-2-custom/2)). Mốc này cho biết một cấu hình x9999 có thể dày tới đâu, nhưng cũng làm tăng tải và tranh chấp tài nguyên rơi.
- Ở mốc supermax, DragonMu x99999 dùng cấu hình thưa hơn: 5 quái cho spot thường trên mọi bản đồ và một hotspot 10 quái cho mỗi bản đồ ([DragonMu x99999](https://dragonmu.net/news/read/48-dragonmu-x99999-supermax-opening-27february)).
- InfinityMU mô tả mật độ cao ở kênh Non-PvP nhưng giảm/tối ưu mật độ ở kênh PvP để hạn chế lag trong giao tranh ([InfinityMU server info](https://www.infinitymu.net/about-infinitymu)).
- GhostMU bổ sung quy mô lớn cho invasion thay vì mọi spot: Golden Invasion có hơn 500 quái mỗi giờ và spot được hiển thị trên minimap ([GhostMU](https://ghost-mu.com/)).

**Mẫu rút ra:** mật độ nền vừa phải, hotspot rõ ràng và invasion đông theo chu kỳ an toàn hơn việc tăng đồng loạt số quái trên mọi bản đồ.

### 5. Tư thế PK/PvP

- NoLimit vận hành hai realm PK x9999 và một realm Non-PK dành cho off-attack ở rate thấp hơn ([NoLimit](https://nolimit.lv/)).
- InfinityMU tách thành kênh Non-PvP, Balanced PvP và Classic PvP ([InfinityMU server info](https://www.infinitymu.net/about-infinitymu)).
- DragonMu x99999 chọn toàn bộ máy chủ là PvP và quảng bá giao tranh full-stat kéo dài khoảng 3–6 giây ([DragonMu x99999](https://dragonmu.net/news/read/48-dragonmu-x99999-supermax-opening-27february)).

**Mẫu rút ra:** x9999 thường hướng tới PvP, nhưng các máy chủ lâu dài vẫn dành một không gian Non-PK/AFK hoặc tách kênh để người mới không bị khóa tiến độ bởi PK tự do.

### 6. Nhịp boss và sự kiện

- GhostMU chạy Golden Invasion mỗi giờ ([GhostMU](https://ghost-mu.com/)).
- NoLimit chạy Blood Castle và Chaos Castle mỗi 2 giờ; Devil Square, White Wizard và Skeleton King mỗi 4 giờ; Kundun mỗi 2 giờ tại năm vị trí; Erohim mỗi giờ; Crywolf mỗi 12 giờ; Nightmare mỗi ngày; Castle Siege mỗi tuần ([NoLimit](https://nolimit.lv/)).
- PikaMU x9999 đặt boss thường mỗi 4 giờ và boss sự kiện mỗi 2 giờ ([PikaMU x9999](https://pikamu.net/news/season-21-part-1-2-custom/2)).
- InfinityMU chạy Blood Castle, Devil Square và Chaos Castle mỗi 2 giờ; sự kiện PvP Battle mỗi giờ; Kundun 6 giờ sau lần chết trước; Nightmare mỗi 24 giờ và Castle Siege vào thứ Bảy hằng tuần ([InfinityMU event schedule](https://wiki.infinitymu.net/index.php?title=Events_Schedule), [InfinityMU Castle Siege](https://www.infinitymu.net/info/castle-siege)).

**Mẫu rút ra:** sự kiện vào cửa nên lặp 1–2 giờ để mọi múi giờ đều tham gia được; boss sinh lợi cao giãn 2–6 giờ; sự kiện thế giới lớn giãn 12–24 giờ; Castle Siege giữ nhịp tuần.

## Mặc định bảo thủ đề xuất cho OpenMU

Các giá trị dưới đây là **khuyến nghị của ghi chú này**, không phải thông số trích nguyên từ một máy chủ cụ thể.

| Khu vực | Mặc định đề xuất | Lý do |
| --- | --- | --- |
| EXP/drop | EXP Regular/Master **x9999**; drop vật phẩm thường **80%**. Excellent, Ancient, Socket, đồ boss và tiền tệ sự kiện dùng bảng rơi riêng, không kế thừa 80%. | 80% nằm giữa các mẫu 50%, 80% và 100%; đủ cảm giác instant nhưng không biến mọi quái thành nguồn đồ endgame. |
| Điểm ở cấp 400 | Giữ **5/7 điểm mỗi cấp** theo định nghĩa lớp hiện có; tặng một lần **2.500/3.000 điểm tự do** tương ứng. Ở cấp 400, mục tiêu là khoảng **4.495/5.793 điểm tự do**, chưa gồm chỉ số gốc. Không tự động cấp full 32k. | Bám sát mẫu GhostMU/NoLimit, cho phép build hoạt động ngay sau lần lên 400 đầu tiên mà vẫn còn giá trị cho reset và phần thưởng dài hạn. |
| Cửa hàng | NPC bán potion, town portal, toàn bộ skill scroll/orb, Jewel of Chaos/Bless/Soul/Life, Box of Kundun +1 đến +4, cánh cấp 1 và trang bị normal/Excellent cấp thấp–trung. Không bán Full Option, Ancient, Socket, cánh cao nhất hoặc đồ boss. | Loại bỏ việc farm đồ tiện ích nhưng giữ boss/sự kiện làm nguồn sức mạnh cuối game. |
| Mật độ quái | Mỗi spot thường **5 quái**, mỗi bản đồ luyện cấp có **1 hotspot 10 quái**, hồi sinh quái thường sau **7 giây**. Kênh PvP dùng ít hotspot hơn nếu tải chiến đấu tăng. Invasion dùng spawn riêng, không cộng vào spot nền. | Khởi đầu từ mẫu 5/10 đã công bố, thêm thời gian hồi sinh vừa phải để tránh bão đối tượng và tranh spot quá gay gắt. |
| PK/PvP | Nếu có nhiều kênh: một kênh **PK/PvP đầy đủ** và một kênh **Non-PK/AFK leveling**; boss cạnh tranh và phần thưởng PvP chỉ xuất hiện ở kênh PK. Nếu chỉ có một kênh: bản đồ khởi đầu Non-PK, Arena/endgame PvP. Không làm rơi trang bị hoặc inventory khi chết do người chơi. | Duy trì mục tiêu PvP của x9999 nhưng có đường tiến độ không bị grief cho người mới và người chơi AFK. |
| Boss/sự kiện | Golden Invasion **1 giờ**; Blood Castle/Devil Square/Chaos Castle **2 giờ** và chạy lệch nhau; Erohim **2 giờ**; Kundun **4 giờ**; White Wizard/Skeleton King **4 giờ**; Crywolf **12 giờ**; Nightmare **24 giờ**; Castle Siege **mỗi tuần**. | Đây là nhịp giữa các lịch NoLimit và InfinityMU: thường xuyên cho nội dung phổ thông, nhưng boss lớn vẫn đủ hiếm để tạo cạnh tranh. |

## Ranh giới cần giữ khi triển khai

1. “Drop 80%” chỉ nên điều khiển khả năng sinh một lượt rơi thông thường; xác suất option và nhóm vật phẩm hiếm phải độc lập.
2. Tổng điểm ở cấp 400 phải được tính từ 399 lần tăng cấp, không phải 400, nếu nhân vật bắt đầu ở cấp 1.
3. Không sao chép nguyên bộ Full Option/freebies của NoLimit hoặc mức 100 điểm/cấp của DragonMu x99999 vào mặc định; hai cấu hình đó bỏ gần hết vòng tiến triển.
4. Lịch sự kiện nên công bố theo một múi giờ máy chủ duy nhất và hiển thị bộ đếm trong game; các sự kiện 2 giờ nên lệch nhau ít nhất 30 phút để người chơi không phải chọn bỏ một sự kiện.
