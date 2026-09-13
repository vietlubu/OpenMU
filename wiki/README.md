# Hướng dẫn chơi OpenMU x9999

Tài liệu này dành cho người mới tham gia server. Mục tiêu của server là **lên cấp nhanh, mua đủ đồ khởi đầu, săn box và boss cùng nhóm 5–6 người**, sau đó thử build nhân vật hoặc PvP mà không phải cày cuốc kéo dài.

## 1. Thông tin nhanh

| Nội dung | Cấu hình hiện tại |
| --- | --- |
| Phiên bản | OpenMU Season 6 |
| Cấp thường tối đa | 400 |
| EXP | x9999 |
| Điểm mỗi lần lên cấp | 500; nhân vật đã hoàn thành Hero Status nhận 501 |
| Giới hạn mỗi stat | 32.767 |
| Zen nhặt được | x1000 |
| Quái tại spot | Tối thiểu 10 con, hồi sinh khoảng 5 giây |
| PvP/PK | Đang bật; kỹ năng diện rộng có thể đánh trúng người chơi |
| Nhịp boss | Golden → Red Dragon → White Wizard, đổi event mỗi 10 phút |

Với 500 điểm mỗi cấp, một nhân vật đi từ level 1 đến 400 nhận khoảng **199.500 điểm**, đủ để đưa các stat cần thiết lên gần hoặc tới giới hạn. Vì vậy server không yêu cầu reset nhiều lần mới có một build hoàn chỉnh.

## 2. Kết nối server

### Trong mạng LAN

```text
Địa chỉ: 192.168.1.28
Connect Server: 44405
```

### Qua mạng riêng/Tailscale

```text
Địa chỉ: 100.108.169.118
Connect Server: 44405
```

Một số bộ client có thể được cấu hình dùng cổng `44406`. Nếu client được phát kèm file cấu hình thì nên giữ nguyên cổng ghi trong file đó.

## 3. Nên bắt đầu như thế nào?

1. Tạo nhân vật thuộc class muốn chơi.
2. Tới NPC trang bị và NPC skill tương ứng với class.
3. Mua bộ giáp và vũ khí Excellent +7 khởi đầu.
4. Mua đầy đủ sách, ngọc hoặc orb học skill có thể sử dụng.
5. Mua Large Healing Potion và Large Mana Potion; mỗi bình có thể chứa tối đa 255 lần dùng.
6. Lập party rồi luyện tại các spot đông quái cho tới level 400; quái luôn cho Zen và chỉ quay thêm ngọc hoặc box, không rơi trang bị trực tiếp.
7. Nhặt Box of Kundun +1 đến +3 từ quái thường để nâng dần trang bị.
8. Mua item change class tại Potion Girl Amy khi cần làm quest chuyển class.
9. Theo thông báo invasion, tập trung cả nhóm săn boss để lấy Box +4, +5 hoặc GM Gift Full Option.
10. Sau khi hoàn thiện build, thử đấu PvP hoặc đổi cách phân bổ stat.

## 4. Shop trang bị và skill

### Dark Wizard / Soul Master

- **Pasi the Mage (254), Lorencia**
  - Pad Set Excellent +7.
  - Skull Staff và Serpent Staff Excellent +7.
  - Sách/ngọc skill dành cho Dark Wizard và Magic Gladiator.
- **Izabel the Wizard (245), Devias**
  - Bản shop thuận tiện tại Devias, bán lại trang bị Dark Wizard và skill Dark Wizard/Magic Gladiator.

### Dark Knight / Blade Knight

- **Hanzo the Blacksmith (251), Lorencia**
  - Leather Set Excellent +7.
  - Blade và Gladius Excellent +7.
- **Alex (230), Lorencia**
  - Ngọc/orb skill của Dark Knight.

### Fairy Elf / Muse Elf

- **Eo the Craftsman (243), Noria**
  - Vine Set Excellent +7.
  - Short Bow và Battle Bow Excellent +7.
- **Elf Lala (242), Noria**
  - Toàn bộ skill tiêu hao dành cho Fairy Elf.
  - Summoning Orb level 0–6; mỗi level triệu hồi một loại quái khác nhau.

### Magic Gladiator

- **Hanzo the Blacksmith (251), Lorencia**
  - Storm Crow Set Excellent +7.
  - Blade và Skull Staff Excellent +7.
- Skill dùng chung với Dark Wizard được bán tại **Pasi** hoặc **Izabel**.

### Dark Lord

- **Hanzo the Blacksmith (251), Lorencia**
  - Light Plate Set Excellent +7.
  - Battle Scepter và Master Scepter Excellent +7.
- **Alex (230), Lorencia**
  - Skill tiêu hao dành cho Dark Lord.

### Summoner

- **Rhea (416), Elvenland**
  - Red Wing Set Excellent +7.
  - Violent Wind Stick, Book of Sahamutt và Book of Neil Excellent +7.
- **Marce (417), Elvenland**
  - Toàn bộ sách skill dành cho Summoner.

### Rage Fighter

- **Hanzo the Blacksmith (251), Lorencia**
  - Sacred Set Excellent +7.
  - Sacred Glove và Storm Hard Glove Excellent +7.
- **Alex (230), Lorencia**
  - Skill tiêu hao dành cho Rage Fighter.

### Shop vũ khí chung tại Devias

**Zienna the Weapons Merchant (246)** bán các vũ khí khởi đầu Excellent +7 cho Dark Knight, Fairy Elf, Magic Gladiator, Dark Lord và Rage Fighter. Skill đầy đủ vẫn nằm tại shop quê nhà của từng class.

## 5. Shop vật phẩm thiết yếu

Các NPC general-goods ở nhiều thành đều bán:

- Large Healing Potion +1, 255 lần dùng.
- Large Mana Potion +1, 255 lần dùng.
- Antidote.
- Bolt và Arrow.
- Town Portal Scroll.
- Armor of Guardsman.

Riêng **Potion Girl Amy (253)** còn bán:

- Toàn bộ item quest chuyển class: Scroll of Emperor, Ring of Honor, Broken Sword, Dark Stone, Tear of Elf, Soul Shard of Wizard, Flame of Death Beam Knight, Horn of Hell Maine, Feather of Dark Phoenix và Eye of Abyssal.
- Chaos Dragon Axe, Chaos Nature Bow và Chaos Lightning Staff **+4 +4 option** để đưa thẳng vào công thức tạo cánh cấp 1.
- Jewel of Bless, Soul, Chaos, Life và Creation dạng viên lẻ; đồng thời có Packed Jewel tương ứng loại 10, 20 và 30 viên.
- Loch's Feather thường, Loch's Feather +1 (Monarch's Crest), Flame of Condor và Feather of Condor cho các công thức cánh/cape cao hơn.

Các NPC áp dụng gồm Potion Girl Amy, Oracle Layla, Pamela, Angela, Silvia, Christine và Leina. Các shop barmaid, vé event và NPC crafting khác vẫn giữ nội dung Season 6 gốc.

### Công thức chế tạo Wings

Thực hiện tại **Chaos Goblin/Chaos Machine**. Toàn bộ công thức Wings, cape và Feather of Condor có tỷ lệ thành công cố định **90%**. Wings/cape tạo thành luôn có **Luck** và normal option tối đa; nếu loại đó hỗ trợ Recover HP thì hệ thống luôn ưu tiên Recover HP, nếu không sẽ dùng option gốc của loại Wings. Mỗi special wing line được quay độc lập với tỷ lệ **90%**, vì vậy khả năng ra full dòng rất cao.

#### Wings cấp 1

| Nguyên liệu | Yêu cầu |
| --- | --- |
| Chaos Weapon | 1 Chaos Dragon Axe, Chaos Nature Bow hoặc Chaos Lightning Staff, tối thiểu **+4 +4 option** |
| Jewel of Chaos | 1 viên |
| Trang bị thường bổ sung | Không bắt buộc; tối thiểu **+4 +4 option** |
| Jewel of Bless/Soul | Không bắt buộc |

Potion Girl bán sẵn ba Chaos Weapon **+4 +4 option có Luck**, nên có thể mua một món rồi đưa thẳng vào Chaos Machine. Kết quả ngẫu nhiên: Wings of Elf, Wings of Heaven, Wings of Satan hoặc Wings of Curse.

#### Wings cấp 2

| Nguyên liệu | Yêu cầu |
| --- | --- |
| Wings cấp 1 | 1 cánh, từ +0 trở lên |
| Loch's Feather | 1 cái |
| Jewel of Chaos | 1 viên |
| Trang bị Excellent bổ sung | Không bắt buộc; tối thiểu +4 |

Chi phí cơ bản **5.000.000 Zen**. Tỷ lệ thành công cố định **90%**. Kết quả ngẫu nhiên: Wings of Spirit, Wings of Soul, Wings of Dragon, Wings of Darkness hoặc Wings of Despair.

#### Cape of Lord hoặc Cape of Fighter

| Nguyên liệu | Yêu cầu |
| --- | --- |
| Wings cấp 1 | 1 cánh, từ +0 trở lên |
| Monarch's Crest | 1 **Loch's Feather +1** |
| Jewel of Chaos | 1 viên |
| Trang bị Excellent bổ sung | Không bắt buộc; tối thiểu +4 |

Chi phí cơ bản **5.000.000 Zen**. Tỷ lệ thành công cố định **90%**. Chaos Machine chọn ngẫu nhiên Cape of Lord hoặc Cape of Fighter.

#### Wings cấp 3 — bước 1: tạo Feather of Condor

| Nguyên liệu | Yêu cầu |
| --- | --- |
| Wings cấp 2 hoặc cape | 1 món **+9 trở lên, có option** |
| Trang bị Ancient | 1 món **+7 trở lên, có option** |
| Jewel of Chaos | 1 viên |
| Jewel of Creation | 1 viên |
| Packed Jewel of Soul | 1 pack 10 viên, tức Packed Soul +0 |

Tỷ lệ thành công cố định **90%**. Kết quả là **Feather of Condor**. Vì Potion Girl cũng bán sẵn Feather of Condor, có thể bỏ qua bước này nếu chỉ muốn chế tạo nhanh.

#### Wings cấp 3 — bước 2

| Nguyên liệu | Yêu cầu |
| --- | --- |
| Trang bị Excellent | 1 món **+9 trở lên, có option** |
| Feather of Condor | 1 cái |
| Flame of Condor | 1 cái |
| Jewel of Chaos | 1 viên |
| Jewel of Creation | 1 viên |
| Packed Jewel of Bless | 1 pack 10 viên, tức Packed Bless +0 |
| Packed Jewel of Soul | 1 pack 10 viên, tức Packed Soul +0 |

Tỷ lệ thành công cố định **90%**. Kết quả ngẫu nhiên: Wings of Storm, Wings of Eternal, Wings of Illusion, Wings of Ruin, Cape of Emperor, Wings of Dimension hoặc Cape of Overrule.

> **Lưu ý:** Packed Jewel +0/+1/+2 lần lượt đại diện cho pack **10/20/30 viên**. Công thức Wings cấp 3 chỉ yêu cầu pack 10 viên, vì vậy hãy mua bản **+0**.

Với Wings cấp 2 và Cape of Fighter có ba special lines, xác suất nhận đủ cả ba là khoảng **72,9%**. Cape of Lord và Wings cấp 3 có bốn special lines, nên xác suất full bốn dòng là khoảng **65,6%** (`90%` cho từng dòng, quay độc lập).

## 6. Trang bị Excellent trong shop

Đồ shop là bộ khởi đầu, không phải đồ cuối game:

- Tất cả giáp khởi đầu đều **+7**, có Luck và đúng một excellent option tăng khả năng phòng thủ/sinh tồn.
- Vũ khí khởi đầu đều **+7**, có Luck, có skill nếu loại vũ khí đó hỗ trợ và có một excellent option tấn công.
- Đồ shop khởi đầu không có normal option +16.
- Muốn đồ nhiều excellent options và +16, người chơi vẫn phải săn box — đặc biệt là GM Gift.

## 7. Box of Kundun rơi ở đâu?

### Quái thường

Quái thường không còn rơi giáp, vũ khí, đồ Excellent ngẫu nhiên, item quest hoặc nguyên liệu map. Mỗi lần chết luôn trả Zen, sau đó có đúng một lượt quay phần thưởng phụ:

| Phần thưởng | Tỷ lệ mỗi quái |
| --- | ---: |
| Ngọc ngẫu nhiên | 1% |
| Box of Kundun +1 | 2% |
| Box of Kundun +2 | 1,5% |
| Box of Kundun +3 | 1% |
| Không có phần thưởng phụ | 94,5% |

Ngọc và box dùng chung một lượt quay nên một quái không đồng thời rơi cả ngọc lẫn box. Trang bị mới đến từ shop hoặc từ lúc mở Box of Kundun/GM Gift.

### Boss

Mỗi boss hợp lệ luôn rơi đúng một phần thưởng gacha:

| Phần thưởng | Tỷ lệ |
| --- | ---: |
| Box of Kundun +4 | 47,5% |
| Box of Kundun +5 | 47,5% |
| GM Gift Full Option | 5% |

Các boss áp dụng gồm nhóm Golden Monster, Red Dragon, White Wizard, Illusion of Kundun, Erohim, Nightmare, Maya và hai tay Maya, Dark Elf và Selupan. Hộ vệ của White Wizard không được tính là boss.

## 8. Mỗi loại box mở ra gì?

Tất cả Box of Kundun +1 đến +5 hiện mở ra **một item Excellent có Luck với xác suất 100%**. Không còn trường hợp box Kundun mở ra Zen.

| Box | Nhóm phần thưởng điển hình |
| --- | --- |
| Kundun +1 | Vũ khí, khiên và các set cấp thấp như Leather, Pad, Vine, Bronze, Silk, Violent Wind, Red Wing. |
| Kundun +2 | Trang bị thấp–trung như Scale, Brass, Bone, Sphinx, Wind, Spirit, Light Plate, Ancient; có thể có nhẫn và dây chuyền. |
| Kundun +3 | Trang bị trung cấp như Plate, Dragon, Legendary, Guardian, Storm Crow, Adamantine, Bloody Amethyst, Sacred Fire. |
| Kundun +4 | Trang bị cao cấp, một số vũ khí Archangel và các set Black Dragon, Dark Phoenix, Grand Soul, Divine, Thunder Hawk, Dark Steel, Rhodon Quartz, Storm Jahad Fire. |
| Kundun +5 | Trang bị top-tier như Knight Blade, Dark Reign Blade, Rune Blade, Shining Scepter, Arrow Viper Bow, Staff of Kundun, Platina Staff và các set Great Dragon, Dark Soul, Hurricane, Red Spirit, Dark Master. |

Khi box chọn một set, phần thưởng là **một mảnh ngẫu nhiên của set**, không phải nguyên bộ.

Excellent options của Kundun +1 đến +5 vẫn được tạo ngẫu nhiên, nhưng Luck luôn có nếu loại trang bị hỗ trợ. Chúng không bảo đảm full option.

## 9. GM Gift Full Option

GM Gift là jackpot có tỷ lệ **5% từ boss**. Client gốc có thể vẫn hiển thị tên item là `GM Gift`; đây chính là box Full Option của server.

Khi mở, người chơi nhận một trang bị thuộc pool top-tier của Kundun +5 với:

- Level +13.
- Tối đa sáu excellent options khác nhau nếu item hỗ trợ.
- Luck nếu item hỗ trợ.
- Skill nếu vũ khí hỗ trợ skill.
- Normal option +16 nếu item hỗ trợ.
- Durability tối đa.

Chỉ GM Gift bảo đảm hợp đồng Full Option này. Box Kundun thông thường bảo đảm item Excellent và Luck, nhưng số excellent option vẫn ngẫu nhiên.

## 10. Lịch invasion boss

Ba invasion chạy thành một vòng liên tục 30 phút:

| Phút trong chu kỳ | Event |
| --- | --- |
| 00–10 | Golden Invasion |
| 10–20 | Red Dragon Invasion |
| 20–30 | White Wizard Invasion |

Sau phút 30, chu kỳ bắt đầu lại. Mỗi event kéo dài 10 phút và không chồng lên hai event còn lại. Hãy theo dõi thông báo trong game để biết event vừa bắt đầu và bản đồ liên quan.

## 11. Cách chơi đề xuất cho nhóm 5–6 người

### Chia vai trò

- Một hoặc hai nhân vật thiên về sát thương đơn mục tiêu để đánh boss.
- Một nhân vật có buff/hỗ trợ, thường là Fairy Elf.
- Một nhân vật có kỹ năng diện rộng để dọn spot hoặc quái invasion.
- Các thành viên còn lại có thể thử build PvP, tank hoặc class yêu thích.

Đây chỉ là gợi ý. Do tốc độ lên cấp và lượng point rất cao, nhóm có thể đổi chiến thuật mà không phải tạo lại toàn bộ tiến trình.

### Vòng chơi ngắn

1. Luyện level, nhặt Zen và ngọc.
2. Mua đồ Excellent +7 có Luck cùng toàn bộ skill cần thiết.
3. Mua item change class hoặc trang bị tạo cánh tại Potion Girl Amy khi cần.
4. Farm quái thường để lấy Kundun +1/+2/+3; quái không rơi trang bị trực tiếp.
5. Gom nhóm theo chu kỳ invasion 10 phút.
6. Săn Kundun +4/+5 và jackpot GM Gift.
7. So sánh build bằng duel hoặc PvP.

### Phân phối đồ trong party

Nên thống nhất trước một trong hai cách:

- Item phù hợp class nào thì ưu tiên class đó.
- Luân phiên quyền nhận box hoặc jackpot giữa các thành viên.

Server dành cho nhóm nhỏ nên chia đồ hợp lý sẽ giúp cả nhóm đạt ngưỡng săn boss nhanh hơn việc một người giữ toàn bộ vật phẩm.

## 12. Lưu ý về PK/PvP

- PvP đang bật trên game server.
- Kỹ năng diện rộng có thể đánh trúng người chơi khác.
- Khi đi invasion, nên lập party và thống nhất khu vực đánh để tránh vô tình PK nhau.
- Không nên đứng AFK tại khu vực boss hoặc spot đang tranh chấp.
- Các quy tắc Blood Castle, Chaos Castle, duel và chi phí `/pkclear` vẫn giữ theo cấu hình Season 6 hiện có.

## 13. Câu hỏi thường gặp

### Vì sao tôi chỉ nhận 5 point khi lên cấp?

Cấu hình hiện tại là 500 point mỗi level, hoặc 501 sau Hero Status. Hãy thoát hẳn nhân vật và đăng nhập lại để nạp cấu hình mới. Nếu vẫn nhận 5 point, báo tên nhân vật cho quản trị viên kiểm tra thuộc tính đã lưu.

### Vì sao quái không rơi trang bị hoặc item quest?

Đây là thiết kế mới của server. Quái chỉ trả Zen và quay thêm ngọc hoặc Box of Kundun. Trang bị lấy từ shop hoặc mở box; item change class mua tại Potion Girl Amy.

### Vì sao Kundun box không rơi Zen?

Kundun +1 đến +5 luôn trả về một item Excellent có Luck để giữ nhịp chơi nhanh.

### Vì sao đồ Excellent từ Kundun không có đủ sáu option?

Kundun box thường vẫn quay số excellent option ngẫu nhiên, nhưng Luck được bảo đảm. Muốn bảo đảm sáu Excellent options, Luck, skill và +16, cần săn GM Gift từ boss.

### Tôi có cần reset nhiều lần không?

Không bắt buộc. Với 500 point mỗi level, lần lên level 400 đầu tiên đã đủ điểm cho một build rất mạnh và có thể đạt giới hạn các stat cần thiết.

### Skill đã mua nhưng chưa học được?

Shop bán đủ skill phù hợp với class, kể cả skill cấp cao. Yêu cầu level, class và quest của từng skill vẫn được kiểm tra khi sử dụng.

## 14. Tóm tắt cho người mới

```text
Mua đồ Excellent +7 có Luck và skill đúng class
→ mua potion 255, item change class hoặc đồ tạo cánh tại Amy
→ lập party lên level 400
→ farm Zen/ngọc/Kundun +1/+2/+3; quái không rơi trang bị
→ theo invasion mỗi 10 phút
→ săn Kundun +4/+5 và GM Gift
→ hoàn thiện build rồi PvP
```
