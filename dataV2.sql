USE [MartyrGravesManagement]
GO

INSERT [dbo].[Areas] ([AreaName], [Description], [Status]) 
VALUES (N'A1', N'Khu A1', 1),
       (N'A2', N'Khu A2', 1),
       (N'A3', N'Khu A3', 1),
       (N'A4', N'Khu A4', 1),
       (N'B1', N'Khu B1', 1),
       (N'B2', N'Khu B2', 1),
       (N'B3', N'Khu B3', 1),
       (N'B4', N'Khu B4', 1),
       (N'C1', N'Khu C1', 1),
       (N'C2', N'Khu C2', 1);

GO

INSERT [dbo].[Locations] ([RowNumber], [MartyrNumber], [AreaNumber], [Status]) 
VALUES (1, 101, 1, 1),
       (2, 102, 1, 1),
       (3, 103, 1, 1),
       (1, 201, 2, 1),
       (2, 202, 2, 1),
       (3, 203, 2, 1),
       (1, 301, 3, 1),
       (2, 302, 3, 1),
       (1, 401, 4, 1),
       (1, 402, 4, 1),
       (1, 403, 4, 1);

GO

INSERT [dbo].[Roles] ([RoleName], [Description], [Status]) 
VALUES (N'Admin', N'Administrator role', 1),
       (N'Manager', N'Manager role', 1),
       (N'Staff', N'Staff role', 1),
       (N'Customer', N'Customer role', 1);

GO

INSERT [dbo].[Accounts] ([RoleId], [AreaId], [EmailAddress], [HashedPassword], [FullName], [DateOfBirth], [PhoneNumber], [Address], [AvatarPath], [Status], [CustomerCode], [CreateAt]) 
VALUES 
    (1, NULL, NULL, N'2757cb3cafc39af451abb2697be79b4ab61d63d74d85b0418629de8c26811b529f3f3780d0150063ff55a2beee74c4ec102a2a2731a1f1f7f10d473ad18a6a87', N'Admin', NULL, N'0101010101', NULL, N'https://firebasestorage.googleapis.com/v0/b/mtg-capstone-2024.appspot.com/o/accounts%2Favt-1.jpg?alt=media&token=925c1804-ca20-49f1-bf07-ad1619bfe3b0', 1, NULL, CAST(N'2024-01-01T00:00:00.0000000' AS DateTime2)),
    (2, 1, NULL, N'2757cb3cafc39af451abb2697be79b4ab61d63d74d85b0418629de8c26811b529f3f3780d0150063ff55a2beee74c4ec102a2a2731a1f1f7f10d473ad18a6a87', N'Manager1', NULL, N'0121010101', NULL, N'https://firebasestorage.googleapis.com/v0/b/mtg-capstone-2024.appspot.com/o/accounts%2Favt-10.jpg?alt=media&token=3f1553c5-b4a5-4d01-81a1-739d955629ad', 1, NULL, CAST(N'2024-01-01T00:00:00.0000000' AS DateTime2)),
    (2, 2, NULL, N'2757cb3cafc39af451abb2697be79b4ab61d63d74d85b0418629de8c26811b529f3f3780d0150063ff55a2beee74c4ec102a2a2731a1f1f7f10d473ad18a6a87', N'Manager2', NULL, N'0131010101', NULL, N'https://firebasestorage.googleapis.com/v0/b/mtg-capstone-2024.appspot.com/o/accounts%2Favt-11.jpg?alt=media&token=f7187fd0-1dba-4867-877c-fc3c546a2698', 1, NULL, CAST(N'2024-01-01T00:00:00.0000000' AS DateTime2)),
    (2, 3, NULL, N'2757cb3cafc39af451abb2697be79b4ab61d63d74d85b0418629de8c26811b529f3f3780d0150063ff55a2beee74c4ec102a2a2731a1f1f7f10d473ad18a6a87', N'Manager3', NULL, N'0141010101', NULL, N'https://firebasestorage.googleapis.com/v0/b/mtg-capstone-2024.appspot.com/o/accounts%2Favt-12.jpg?alt=media&token=fb425af9-ea37-4902-bd2d-7a149aaa663c', 1, NULL, CAST(N'2024-01-01T00:00:00.0000000' AS DateTime2)),
    (2, 4, NULL, N'2757cb3cafc39af451abb2697be79b4ab61d63d74d85b0418629de8c26811b529f3f3780d0150063ff55a2beee74c4ec102a2a2731a1f1f7f10d473ad18a6a87', N'Manager4', NULL, N'0151010101', NULL, N'https://firebasestorage.googleapis.com/v0/b/mtg-capstone-2024.appspot.com/o/accounts%2Favt-2.jpg?alt=media&token=28cdeb93-7184-4f17-99e0-7da9ce215bb0', 1, NULL, CAST(N'2024-01-01T00:00:00.0000000' AS DateTime2)),
    (3, 1, NULL, N'2757cb3cafc39af451abb2697be79b4ab61d63d74d85b0418629de8c26811b529f3f3780d0150063ff55a2beee74c4ec102a2a2731a1f1f7f10d473ad18a6a87', N'Staff1', NULL, N'8386836831', NULL, N'https://firebasestorage.googleapis.com/v0/b/mtg-capstone-2024.appspot.com/o/accounts%2Favt-3.jpg?alt=media&token=a18d3ad2-55c4-4a27-838c-1548761646c1', 0, NULL, CAST(N'2024-01-01T00:00:00.0000000' AS DateTime2)),
    (3, 2, NULL, N'2757cb3cafc39af451abb2697be79b4ab61d63d74d85b0418629de8c26811b529f3f3780d0150063ff55a2beee74c4ec102a2a2731a1f1f7f10d473ad18a6a87', N'Staff2', NULL, N'8386836832', NULL, N'https://firebasestorage.googleapis.com/v0/b/mtg-capstone-2024.appspot.com/o/accounts%2Favt-4.jpg?alt=media&token=222a0984-c9bb-492e-bada-25f42c8e713a', 1, NULL, CAST(N'2024-01-01T00:00:00.0000000' AS DateTime2)),
    (3, 3, NULL, N'2757cb3cafc39af451abb2697be79b4ab61d63d74d85b0418629de8c26811b529f3f3780d0150063ff55a2beee74c4ec102a2a2731a1f1f7f10d473ad18a6a87', N'Staff3', NULL, N'8386836833', NULL, N'https://firebasestorage.googleapis.com/v0/b/mtg-capstone-2024.appspot.com/o/accounts%2Favt-5.jpg?alt=media&token=f32e7537-f2c8-4f07-958a-8baa7e6d98be', 0, NULL, CAST(N'2024-01-01T00:00:00.0000000' AS DateTime2)),
    (3, 4, NULL, N'2757cb3cafc39af451abb2697be79b4ab61d63d74d85b0418629de8c26811b529f3f3780d0150063ff55a2beee74c4ec102a2a2731a1f1f7f10d473ad18a6a87', N'Staff4', NULL, N'8386836834', NULL, N'https://firebasestorage.googleapis.com/v0/b/mtg-capstone-2024.appspot.com/o/accounts%2Favt-6.jpg?alt=media&token=cb4ac467-ade1-457f-8c81-8c963f2c3b2f', 1, NULL, CAST(N'2024-01-01T00:00:00.0000000' AS DateTime2)),
    (4, NULL, N'nguyenkhanh3055@gmail.com', N'43437bc0ae06c833a794df33309c436de420736555f02264b9f7dfdabb9af06ac421d83d097879f51d1684522a59423f1373847c88fa6f4385d780bddd4de253', N'user1', CAST(N'1998-10-22T05:27:35.6010000' AS DateTime2), N'3461231546', N'string', N'https://firebasestorage.googleapis.com/v0/b/mtg-capstone-2024.appspot.com/o/accounts%2Favt-7.jpg?alt=media&token=43c0380f-9a7d-4b88-b72c-05fd146b764f', 1, N'Customer-user1-3461231546', CAST(N'2024-10-24T00:00:00.0000000' AS DateTime2)),
    (4, NULL, N'henry@example.com', N'6be53db75edb321c1a5476675cfdf2062420455683b5dc8344110873713e99953a73bae1ebf0a5c2d588ab16c98be26290dfb0c7d1b4d0ea8f16e0444f636818', N'Hoàng Anh', CAST(N'2024-10-24T15:02:28.7380000' AS DateTime2), N'0201030293', N'string', N'https://firebasestorage.googleapis.com/v0/b/mtg-capstone-2024.appspot.com/o/accounts%2Favt-8.jpg?alt=media&token=6736bfac-93d8-48ff-b721-12a98d7362c2', 1, N'Customer-Henry-0201030293', CAST(N'2024-10-24T22:04:54.7910071' AS DateTime2)),
    (4, NULL, N'mark@example.com', N'926236da4fd03f7872134760d70765f489d28cf159316a0bbb496577d5b094c535340883eaba1163daf92ee62d3078d4f6e13e578d83acc03da3fad8fb8deeb9', N'Văn Cường', CAST(N'2024-10-24T15:02:28.7380000' AS DateTime2), N'0201030273', N'string', N'https://firebasestorage.googleapis.com/v0/b/mtg-capstone-2024.appspot.com/o/accounts%2Favt-9.jpg?alt=media&token=3dd926af-0770-44eb-907a-2caeff1f6ad9', 1, N'Customer-Mark-0201030273', CAST(N'2024-10-24T22:06:47.8381199' AS DateTime2)),
    (4, NULL, NULL, N'73ce4fb9b74fe0f8db79c433d89dfe0b0b98cce7e76594a629caaa2ab05e18d138341fcfb146443396fc84e91936ea8a782856ca65a5ae21b19203291c58564d', NULL, NULL, N'0396941710', NULL, NULL, 1, NULL, CAST(N'2024-10-26T09:04:58.9453957' AS DateTime2)),
    (4, NULL, NULL, N'2757cb3cafc39af451abb2697be79b4ab61d63d74d85b0418629de8c26811b529f3f3780d0150063ff55a2beee74c4ec102a2a2731a1f1f7f10d473ad18a6a87', NULL, NULL, N'0239174620', NULL, NULL, 1, NULL, CAST(N'2024-10-26T11:59:26.2368524' AS DateTime2)),
    (4, NULL, NULL, N'2757cb3cafc39af451abb2697be79b4ab61d63d74d85b0418629de8c26811b529f3f3780d0150063ff55a2beee74c4ec102a2a2731a1f1f7f10d473ad18a6a87', NULL, NULL, N'0901835472', NULL, NULL, 1, NULL, CAST(N'2024-10-26T12:02:00.5820763' AS DateTime2)),
    (4, NULL, NULL, N'2757cb3cafc39af451abb2697be79b4ab61d63d74d85b0418629de8c26811b529f3f3780d0150063ff55a2beee74c4ec102a2a2731a1f1f7f10d473ad18a6a87', NULL, NULL, N'0908468260', NULL, NULL, 1, NULL, CAST(N'2024-10-26T12:02:49.5948083' AS DateTime2)),
    (4, NULL, NULL, N'2757cb3cafc39af451abb2697be79b4ab61d63d74d85b0418629de8c26811b529f3f3780d0150063ff55a2beee74c4ec102a2a2731a1f1f7f10d473ad18a6a87', NULL, NULL, N'0395951710', NULL, NULL, 1, NULL, CAST(N'2024-10-26T12:58:19.8585295' AS DateTime2));
GO


INSERT [dbo].[MartyrGraves] ([AreaId], [MartyrCode], [LocationId], [AccountId], [Status]) 
VALUES 
    (1, N'M001', 1, 10, 1),
    (1, N'M002', 2, 11, 1),
    (1, N'M003', 3, 12, 1),
    (2, N'M004', 4, 13, 1),
    (2, N'M005', 5, 14, 1),
    (2, N'M006', 6, 15, 1),
    (3, N'M007', 7, 16, 1),
    (3, N'M008', 8, 17, 1),
    (4, N'M009', 9, 17, 1),
    (4, N'M010', 10, 17, 1),
    (4, N'M011', 11, 17, 1);
GO




INSERT [dbo].[ServiceCategories] ([CategoryName], [Status], [Description], [UrlImageCategory]) 
VALUES 
    (N'Thay hoa tưởng niệm', 1, N'Dịch vụ thay hoa tưởng niệm cho các mộ liệt sĩ', N'https://firebasestorage.googleapis.com/v0/b/mtg-capstone-2024.appspot.com/o/service_category%2FThay-Hoa-Category.jpg?alt=media&token=2d1bcea8-15de-4187-a4f5-6b0fdb6b8369'),
    (N'Thay cây xanh', 1, N'Dịch vụ thay cây xanh cho các mộ liệt sĩ', N'https://firebasestorage.googleapis.com/v0/b/mtg-capstone-2024.appspot.com/o/service_category%2FThay-Cay-Category.jpg?alt=media&token=55428875-c35b-4b01-b442-8aa6a1a9916b'),
    (N'Bảo dưỡng mộ', 1, N'Dịch vụ bảo dưỡng và làm sạch mộ liệt sĩ', N'https://firebasestorage.googleapis.com/v0/b/mtg-capstone-2024.appspot.com/o/service_category%2FBao-Tri-Khu-Vuc-Mo-Category.jpg?alt=media&token=8e44e6b8-75a0-424d-815c-afa531ccd5d9'),
    (N'Cắm hoa lễ tưởng niệm', 1, N'Dịch vụ cắm hoa cho các lễ tưởng niệm anh hùng liệt sĩ', N'https://firebasestorage.googleapis.com/v0/b/mtg-capstone-2024.appspot.com/o/service_category%2FCam-Hoa-Le-Tuong-Niem-Category.jpg?alt=media&token=371434c7-a3c3-48fb-9a28-f6f84ff3c49a');
GO

INSERT [dbo].[Services] ([CategoryId], [ServiceName], [Description], [Price], [Status], [ImagePath]) 
VALUES 
    (1, N'Thay hoa cúc', N'Dịch vụ thay hoa cúc cho các mộ liệt sĩ', 315000, 1, N'https://firebasestorage.googleapis.com/v0/b/mtg-capstone-2024.appspot.com/o/service%2FThay-Hoa-Cuc.jpg?alt=media&token=dd4de2d2-6949-436f-98f8-50b123705be9'),
    (1, N'Thay hoa lan', N'Dịch vụ thay hoa lan cho các mộ liệt sĩ', 283500, 1, N'https://firebasestorage.googleapis.com/v0/b/mtg-capstone-2024.appspot.com/o/service%2FThay-Hoa-Lan.jpg?alt=media&token=79312b1b-85dc-4d55-acea-dda91f02e7b7'),
    (2, N'Thay cây xanh tươi', N'Dịch vụ thay cây xanh tươi cho các mộ liệt sĩ', 189000, 1, N'https://firebasestorage.googleapis.com/v0/b/mtg-capstone-2024.appspot.com/o/service%2FThay-Cay-Xanh-Tuoi.jpeg?alt=media&token=06e9f087-d3f6-4c44-86de-3869c3776c2f'),
    (2, N'Thay cây phong thủy', N'Dịch vụ thay cây phong thủy cho các mộ liệt sĩ', 210000, 1, N'https://firebasestorage.googleapis.com/v0/b/mtg-capstone-2024.appspot.com/o/service%2FThay-Cay-Phong-Thuy.jpg?alt=media&token=5e42388d-a18f-470b-899a-5ccf8e948cad'),
    (3, N'Lau dọn nghĩa trang', N'Dịch vụ bảo dưỡng mộ liệt sĩ', 73500, 1, N'https://firebasestorage.googleapis.com/v0/b/mtg-capstone-2024.appspot.com/o/service%2FBao-Tri-Dinh-Ky.jpg?alt=media&token=b3d42575-cffc-45da-ba42-d1dd0db7f707'),
    (3, N'Lau dọn nghĩa trang đặc biệt', N'Dịch vụ bảo dưỡng đặc biệt mộ liệt sĩ', 52500, 1, N'https://firebasestorage.googleapis.com/v0/b/mtg-capstone-2024.appspot.com/o/service%2FBao-Tri-Dac-Biet.jpg?alt=media&token=d08e6d24-a4cc-4fe7-b9da-a7a7b24f7d36'),
    (4, N'Cắm hoa ngày lễ', N'Dịch vụ cắm hoa cho các ngày lễ tưởng niệm liệt sĩ', 252000, 1, N'https://firebasestorage.googleapis.com/v0/b/mtg-capstone-2024.appspot.com/o/service%2FCam-Hoa-Ngay-Le.jpg?alt=media&token=1518cce4-ca8c-4f03-b9d1-d2867121979f'),
    (4, N'Cắm hoa thường ngày', N'Dịch vụ cắm hoa thường ngày cho các mộ liệt sĩ', 189000, 1, N'https://firebasestorage.googleapis.com/v0/b/mtg-capstone-2024.appspot.com/o/service%2FCam-Hoa-Thuong-Ngay.jpg?alt=media&token=15c083ad-67ab-44a4-978f-1b89608cb8a1');
GO

INSERT [dbo].[GraveImages] ([MartyrId], [UrlPath]) 
VALUES 
    (1, N'https://firebasestorage.googleapis.com/v0/b/mtg-capstone-2024.appspot.com/o/grave_images%2Fbevandan.jpeg?alt=media&token=c49ed54d-43df-449a-a57b-d3df25e14d63'),
    (1, N'https://firebasestorage.googleapis.com/v0/b/mtg-capstone-2024.appspot.com/o/grave_images%2Fbevandan-mo.jpg?alt=media&token=fc9340cd-e897-4f4a-8211-330e71da5291'),
    (2, N'https://firebasestorage.googleapis.com/v0/b/mtg-capstone-2024.appspot.com/o/grave_images%2FPhandinhgiot.jpg?alt=media&token=8e252c1f-69c2-464a-a974-37860507cfb3'),
    (2, N'https://firebasestorage.googleapis.com/v0/b/mtg-capstone-2024.appspot.com/o/grave_images%2FM%E1%BB%99_phan_%C4%91inh_gi%C3%B3t.JPG?alt=media&token=fa3daa32-37e1-478f-9327-48ed6d935380'),
    (3, N'https://firebasestorage.googleapis.com/v0/b/mtg-capstone-2024.appspot.com/o/grave_images%2Fnguyenvietxuan.jpg?alt=media&token=8e691f1a-77db-4384-a598-14dd094c7e04'),
    (4, N'https://firebasestorage.googleapis.com/v0/b/mtg-capstone-2024.appspot.com/o/grave_images%2Fdinhthimau.jpg?alt=media&token=7c41ff94-5286-4731-86b5-c9c94b217125'),
    (5, N'https://firebasestorage.googleapis.com/v0/b/mtg-capstone-2024.appspot.com/o/grave_images%2FPhamXuanAn.jpg?alt=media&token=0e28e54f-7f79-4716-b9e1-fdb8ac9853e0'),
    (5, N'https://firebasestorage.googleapis.com/v0/b/mtg-capstone-2024.appspot.com/o/grave_images%2FPhamXuanXuan-Mo.jpg?alt=media&token=b7cf9e3c-a022-4b60-a2d8-e744af4de7aa'),
    (6, N'https://firebasestorage.googleapis.com/v0/b/mtg-capstone-2024.appspot.com/o/grave_images%2Fphamngocthao.jpg?alt=media&token=3259431c-5a37-4655-9eb2-3f39d23684f6'),
    (6, N'https://firebasestorage.googleapis.com/v0/b/mtg-capstone-2024.appspot.com/o/grave_images%2Fphamngocthao-mo.jpg?alt=media&token=9c4d07c5-98ff-4080-b38e-30d2069a708d'),
    (7, N'https://firebasestorage.googleapis.com/v0/b/mtg-capstone-2024.appspot.com/o/grave_images%2FHoang_Minh_Dao.jpg?alt=media&token=fb9e38b4-1a9c-41f2-beed-76af614d44e0'),
    (7, N'https://firebasestorage.googleapis.com/v0/b/mtg-capstone-2024.appspot.com/o/grave_images%2FHoang_Minh_Dao_1957.jpg?alt=media&token=d88927b3-3124-4b30-bb42-4df93efdcf44'),
    (8, N'https://firebasestorage.googleapis.com/v0/b/mtg-capstone-2024.appspot.com/o/grave_images%2Fvungocnha.jpg?alt=media&token=1a452fcf-c1c5-44dc-89b5-e89d7e6dc020'),
    (8, N'https://firebasestorage.googleapis.com/v0/b/mtg-capstone-2024.appspot.com/o/grave_images%2Fvungocnha-mo.png?alt=media&token=4433427c-b765-4ec6-b2d6-3acec58efdbc'),
    (9, N'https://firebasestorage.googleapis.com/v0/b/mtg-capstone-2024.appspot.com/o/grave_images%2Fnguyenvantroi.jpg?alt=media&token=01a2e711-ef41-4325-86fd-3daa1097cf29'),
    (9, N'https://firebasestorage.googleapis.com/v0/b/mtg-capstone-2024.appspot.com/o/grave_images%2Fnguyenvantroi-mo.jpg?alt=media&token=c252743f-99b8-48c8-b138-e964a1602b14'),
    (10, N'https://firebasestorage.googleapis.com/v0/b/mtg-capstone-2024.appspot.com/o/grave_images%2Fvothisau.jpg?alt=media&token=3d61aabd-08b5-489c-b5fe-2bfa972dce3e'),
    (10, N'https://firebasestorage.googleapis.com/v0/b/mtg-capstone-2024.appspot.com/o/grave_images%2FMo-Vo-Thi-Sau.jpg?alt=media&token=24d630d5-f7fb-4019-8ca0-818cf1c28074'),
    (11, N'https://firebasestorage.googleapis.com/v0/b/mtg-capstone-2024.appspot.com/o/grave_images%2FMo-Vo-Thi-Sau.jpg?alt=media&token=24d630d5-f7fb-4019-8ca0-818cf1c28074'),
    (11, N'https://firebasestorage.googleapis.com/v0/b/mtg-capstone-2024.appspot.com/o/grave_images%2FMo-Vo-Thi-Sau.jpg?alt=media&token=24d630d5-f7fb-4019-8ca0-818cf1c28074');
GO

INSERT [dbo].[MartyrGraveInformations] ([MartyrId], [Name], [Medal], [DateOfSacrifice], [DateOfBirth], [HomeTown], [NickName], [Position], [ReasonOfSacrifice]) 
VALUES 
    (1, N'Bế Văn Đàn', N'Anh hùng lực lượng vũ trang nhân dân', '1953-12-12', '1931-01-01', N'Cao Bằng', NULL, N'Tiểu đội phó', N'Anh hùng Bế Văn Đàn lấy thân mình làm giá súng'),
    (2, N'Phan Đình Giót', N'Anh hùng lực lượng vũ trang nhân dân, Huân chương Quân công hạng Nhì', '1954-03-13', '1922-01-01', N'Hà Tĩnh', NULL, N'Tiểu đội phó', N'Anh hùng Phan Đình Giót dùng thân mình vào bịt kín lỗ châu mai của địch.'),
    (3, N'Nguyễn Viết Xuân', N'Anh hùng Lực lượng vũ trang nhân dân, Huy chương Kháng chiến hạng nhì', '1964-11-18', '1933-01-20', N'Vĩnh Yên', NULL, N'Thiếu úy', N'Ông bị máy bay bắn bị thương nát đùi phải, song ông yêu cầu phẫu thuật bỏ chân, tiếp tục được vào bờ công sự và chỉ huy chiến đấu.'),
    (4, N'Đinh Thị Mậu', N'Danh hiệu Anh hùng Lực lượng vũ trang Nhân dân, 2 Huân chương Độc lập hạng Nhì', '1995-01-01', '1916-01-01', N'Nam Định', N'Đinh Thị Vân', N'Đại tá', N'Hy sinh vì sự nghiệp cách mạng giải phóng dân tộc'),
    (5, N'Phạm Văn Thành', N'Anh hùng Lực lượng vũ trang nhân dân', '2006-09-20', '1927-09-12', N'Đồng Nai', N'Phạm Xuân Ẩn', N'Thiếu tướng', N'Hy sinh vì sự nghiệp cách mạng giải phóng dân tộc'),
    (6, N'Phạm Ngọc Thảo', N'Anh hùng Lực lượng vũ trang nhân dân', '1965-07-17', '1922-02-14', N'Cần Thơ', N'Albert Thảo', N'Đại tá', N'Ông bị bắt và bị tra tấn rất dã man. Tuy nhiên, ông không để lộ tung tích của mình'),
    (7, N'Đào Phúc Lộc', N'Anh hùng Lực lượng vũ trang nhân dân', '1969-12-25', '1923-08-04', N'Móng Cái', N'Hoàng Minh Đạo', N'Trung tướng', N'Ông cùng đồng đội của mình đã hy sinh trên sông Vàm Cỏ Đông do sa vào ổ phục kích của địch.'),
    (8, N'Vũ Ngọc Nhạ', N'Huân chương Độc lập hạng Ba', '2002-08-07', '1928-03-30', N'Thái Bình', N'Hai Long', N'Thiếu tướng', N'Hy sinh vì sự nghiệp cách mạng giải phóng dân tộc'),
    (9, N'Nguyễn Văn Trỗi', N'Anh hùng Lực lượng vũ trang nhân dân', '1964-10-15', '1940-02-01', N'Quảng Nam', N'Tư Trỗi', N'Chiến sĩ biệt động Sài Gòn', N'Ông bị xử bắn tại vườn rau nhà lao Chí Hòa - Sài Gòn (15/10/1964)'),
    (10, N'Võ Thị Sáu', N'Anh hùng lực lượng vũ trang nhân dân', '1952-01-23', '1933-01-01', N'Bà Rịa – Vũng Tàu', N'Chị Sáu', N'Du kích', N'Bà bị xử bắn vào ngày 23/1/1952'),
    (11, N'Nguyễn Văn A', N'Anh hùng lực lượng vũ trang nhân dân', '1952-01-23', '1933-01-01', N'Bà Rịa – Vũng Tàu', N'Anh A', N'Du kích', N'Hy sinh vì sự nghiệp cách mạng giải phóng dân tộc'),
    (11, N'Nguyễn Thị B', N'Anh hùng lực lượng vũ trang nhân dân', '1952-01-23', '1933-01-01', N'Bà Rịa – Vũng Tàu', N'Chị B', N'Du kích', N'Hy sinh vì sự nghiệp cách mạng giải phóng dân tộc');
GO

INSERT [dbo].[Materials] ([ServiceId], [MaterialName], [Description], [Price], [ImagePath]) 
VALUES 
    (1, N'Hoa cúc trắng', N'Hoa cúc trắng mang ý nghĩa trang trọng, thanh khiết.', 100000, NULL),
    (1, N'Hoa cúc vàng', N'Hoa cúc vàng tượng trưng cho sự tôn kính và lòng biết ơn.', 100000, NULL),
    (1, N'Phụ kiện cắm hoa', N'Phụ kiện giúp cắm hoa chắc chắn và đẹp mắt.', 100000, NULL),
    (2, N'Hoa lan trắng', N'Hoa lan trắng biểu trưng cho sự tinh khiết và thanh cao.', 90000, NULL),
    (2, N'Hoa lan tím', N'Hoa lan tím mang đến sự trang nghiêm và lòng thành kính.', 90000, NULL),
    (2, N'Phụ kiện cắm hoa', N'Phụ kiện dùng để cắm hoa tạo hình đẹp mắt.', 90000, NULL),
    (3, N'Cây xanh tươi nhỏ', N'Cây xanh nhỏ tạo không gian xanh mát và thân thiện.', 60000, NULL),
    (3, N'Cây xanh tươi vừa', N'Cây xanh vừa giúp tạo sự hài hòa và sinh động.', 60000, NULL),
    (3, N'Chậu cây', N'Chậu dùng để trồng cây xanh, đảm bảo tính thẩm mỹ.', 60000, NULL),
    (4, N'Cây phong thủy nhỏ', N'Cây nhỏ giúp tạo không gian yên tĩnh và thư giãn.', 70000, NULL),
    (4, N'Cây phong thủy vừa', N'Cây vừa tạo sự hài hòa và cân đối.', 70000, NULL),
    (4, N'Phụ kiện chậu cây', N'Chậu cây phong thủy giúp tăng tính thẩm mỹ.', 60000, NULL),
    (5, N'Dụng cụ bảo trì nhỏ', N'Dụng cụ nhỏ dùng cho các công việc bảo trì cơ bản.', 20000, NULL),
    (5, N'Dụng cụ bảo trì vừa', N'Dụng cụ vừa dùng cho các công việc bảo trì tổng quát.', 30000, NULL),
    (5, N'Phụ kiện bảo trì', N'Phụ kiện giúp hỗ trợ quá trình bảo trì diễn ra thuận lợi.', 20000, NULL),
    (6, N'Dụng cụ bảo trì đặc biệt 1', N'Dụng cụ chuyên dụng cho các công việc bảo trì đặc biệt.', 15000, NULL),
    (6, N'Dụng cụ bảo trì đặc biệt 2', N'Dụng cụ hỗ trợ thêm trong các công việc bảo trì.', 15000, NULL),
    (6, N'Phụ kiện bảo trì đặc biệt', N'Phụ kiện dùng cho bảo trì đặc biệt, nâng cao hiệu quả công việc.', 20000, NULL),
    (7, N'Hoa hồng đỏ', N'Hoa hồng đỏ tượng trưng cho lòng dũng cảm và sự hy sinh.', 80000, NULL),
    (7, N'Hoa hồng vàng', N'Hoa hồng vàng mang ý nghĩa tôn vinh sự hy sinh cao cả.', 80000, NULL),
    (7, N'Phụ kiện cắm hoa lễ', N'Phụ kiện tạo hình nghệ thuật cho hoa cắm lễ.', 80000, NULL),
    (8, N'Hoa cẩm chướng đỏ', N'Hoa cẩm chướng đỏ biểu hiện sự kiên cường.', 60000, NULL),
    (8, N'Hoa cẩm chướng vàng', N'Hoa cẩm chướng vàng tượng trưng cho lòng tri ân.', 60000, NULL),
    (8, N'Phụ kiện cắm hoa', N'Phụ kiện hỗ trợ cắm hoa tạo hình phù hợp.', 60000, NULL);
GO

INSERT [dbo].[HistoricalEvents] ([HistoryEventName], [Description], [StartTime], [EndTime], [Status]) 
VALUES 
    (N'Chiến dịch Điện Biên Phủ', N'Một trong những trận đánh quan trọng trong lịch sử Việt Nam, dẫn đến thắng lợi của quân đội nhân dân Việt Nam.', 
     CAST(N'1954-03-13T00:00:00.0000000' AS DateTime2), CAST(N'1954-05-15T00:00:00.0000000' AS DateTime2), 1),
    (N'Cuộc kháng chiến chống Mỹ', N'Cuộc kháng chiến kéo dài trong suốt 20 năm nhằm giành lại độc lập cho dân tộc.', 
     CAST(N'1954-01-01T00:00:00.0000000' AS DateTime2), CAST(N'1975-04-30T00:00:00.0000000' AS DateTime2), 1);
GO

INSERT [dbo].[HistoricalImages] ([HistoryId], [ImagePath]) 
VALUES 
    (1, N'https://firebasestorage.googleapis.com/v0/b/mtg-capstone-2024.appspot.com/o/HistoricalEventImage%2FVictory_in_Battle_of_Dien_Bien_Phu.jpg?alt=media&token=1a775612-8d09-4aee-9a95-61a805997cc6'),
    (2, N'https://firebasestorage.googleapis.com/v0/b/mtg-capstone-2024.appspot.com/o/HistoricalEventImage%2FchongMy.jpg?alt=media&token=b99d22d4-b2d1-4c6f-bc58-800f93637f55');
GO

INSERT [dbo].[HistoricalRelatedMartyrs] ([HistoryId], [InformationId], [MartyrGraveMartyrId], [Status]) 
VALUES 
    (1, 1, 1, 1),
    (1, 2, 2, 1),
    (1, 3, 3, 1),
    (2, 6, 6, 1),
    (2, 7, 7, 1),
    (2, 9, 9, 1),
    (2, 10, 10, 1),
    (2, 11, 11, 1),
    (2, 12, 11, 1)
GO
