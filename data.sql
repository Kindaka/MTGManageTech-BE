USE [MartyrGravesManagement]
GO

-- Insert data for Roles (Must be inserted before Accounts due to foreign key constraint)
INSERT INTO [dbo].[Roles] (RoleName, Description, Status)
VALUES 
('Admin', 'Administrator role', 1),
('Manager', 'Manager role', 1),
('Staff', 'Staff role', 1),
('Customer', 'Customer role', 1);

GO

-- Insert data for Areas
INSERT INTO [dbo].[Areas] (AreaName, Description, Status)
VALUES 
('Area 1', 'Description for Area 1', 1),
('Area 2', 'Description for Area 2', 1),
('Area 3', 'Description for Area 3', 0),
('Area 4', 'Description for Area 4', 1),
('Area 5', 'Description for Area 5', 1);

GO

-- Insert data for Accounts (After Roles and Areas)
INSERT INTO [dbo].[Accounts] (RoleId, AreaId, EmailAddress, HashedPassword, FullName, DateOfBirth, PhoneNumber, Address, AvatarPath, Status, AccountName, CustomerCode)
VALUES 
(1, 1, 'john.doe@example.com', 'hashed_password_1', 'John Doe', '1985-05-10', '123456789', '123 Main St', '/avatars/john.png', 1, 'john_doe', 'CUST001'),
(2, 2, 'jane.smith@example.com', 'hashed_password_2', 'Jane Smith', '1990-07-15', '987654321', '456 Side St', '/avatars/jane.png', 1, 'jane_smith', 'CUST002'),
(3, 3, 'alice.johnson@example.com', 'hashed_password_3', 'Alice Johnson', '1992-11-20', '555555555', '789 Oak St', '/avatars/alice.png', 1, 'alice_j', 'CUST003'),
(3, NULL, 'bob.brown@example.com', 'hashed_password_4', 'Bob Brown', '1980-03-22', '444444444', '321 Pine St', '/avatars/bob.png', 0, 'bob_brown', 'CUST004'),
(4, 1, 'charlie.davis@example.com', 'hashed_password_5', 'Charlie Davis', '1975-08-30', '333333333', '654 Maple St', '/avatars/charlie.png', 1, 'charlie_d', 'CUST005');

GO

-- Insert data for MartyrGraves (Before MartyrGraveInformations)
INSERT INTO [dbo].[MartyrGraves] (AreaId, MartyrCode, RowNumber, MartyrNumber, AreaNumber, Status, CustomerCode)
VALUES 
(1, 'M001', 1, 101, 1, 1, 'CUST001'),
(2, 'M002', 2, 102, 1, 1, 'CUST002'),
(3, 'M003', 3, 103, 1, 0, 'CUST003'),
(4, 'M004', 4, 104, 1, 1, 'CUST004'),
(5, 'M005', 5, 105, 1, 1, 'CUST005');

GO

-- Insert data for MartyrGraveInformations (After MartyrGraves)
INSERT INTO [dbo].[MartyrGraveInformations] (MartyrId, Name, Medal, DateOfSacrifice)
VALUES 
(1, 'Martyr 1', 'Medal of Honor', '1975-04-30'),
(2, 'Martyr 2', 'Medal of Bravery', '1968-03-01'),
(3, 'Martyr 3', 'Medal of Courage', '1972-05-15'),
(4, 'Martyr 4', 'Medal of Valor', '1970-10-05'),
(5, 'Martyr 5', 'Medal of Sacrifice', '1974-12-25');

GO

-- Insert data for ServiceCategories (Before Services)
INSERT INTO [dbo].[ServiceCategories] (CategoryName, Status)
VALUES 
('Cleaning', 1),
('Maintenance', 1),
('Repair', 1),
('Events', 1),
('Inspection', 1);

GO

-- Insert data for Services (After ServiceCategories)
INSERT INTO [dbo].[Services] (CategoryId, ServiceName, Description, Price, Status, ImagePath)
VALUES 
(1, 'Grave Cleaning', 'Cleaning graves', 100.00, 1, '/images/service_cleaning.png'),
(2, 'Flower Maintenance', 'Maintaining flowers', 200.00, 1, '/images/service_maintenance.png'),
(3, 'Gravestone Repair', 'Repairing gravestones', 300.00, 0, '/images/service_repair.png'),
(4, 'Event Organization', 'Organizing events', 150.00, 1, '/images/service_events.png'),
(5, 'Grave Inspection', 'Inspecting graves', 250.00, 1, '/images/service_inspection.png');

GO




-- Insert data for Jobs (After Accounts)
INSERT INTO [dbo].[Jobs] (AccountId, NameOfWork, TypeOfWork, StartDate, EndDate, Description, Status)
VALUES 
(1, 'Cleaning graves', 1, '2024-09-15', '2024-09-16', 'Cleaning all graves in section A', 1),
(2, 'Maintaining flowers', 2, '2024-09-18', '2024-09-19', 'Watering and maintaining flowers around the graves', 1),
(3, 'Repairing grave stones', 3, '2024-09-20', '2024-09-21', 'Repairing broken gravestones in section B', 0),
(4, 'Organizing events', 1, '2024-09-25', '2024-09-26', 'Organizing a remembrance event for martyrs', 1);

GO


-- Insert data for WeeklyReportGraves (After MartyrGraves and Accounts)
INSERT INTO [dbo].[WeeklyReportGraves] (MartyrId, AccountId, QualityOfTotalGravePoint, QualityOfFlowerPoint, DisciplinePoint, Description, Status)
VALUES 
(1, 1, 8, 9, 7, 'Inspection of Area A', 1),
(2, 2, 7, 6, 8, 'Inspection of Area B', 1),
(3, 3, 9, 8, 7, 'Inspection of Area C', 0),
(4, 4, 8, 9, 9, 'Inspection of Area D', 1),
(5, 5, 7, 7, 8, 'Inspection of Area E', 1);

GO

-- Insert data for WorkPerformances (After Accounts)
INSERT INTO [dbo].[WorkPerformances] (AccountId, QualityMaintenancePoint, TimeCompletePoint, InteractionPoint, Description, UploadTime, Status)
VALUES 
(1, 9, 8, 7, 'Completed work on time', '2024-09-16 12:00:00', 1),
(2, 7, 9, 8, 'Good interaction with staff', '2024-09-19 14:00:00', 1),
(4, 9, 9, 8, 'Great performance', '2024-09-26 18:00:00', 1),
(5, 8, 8, 9, 'Satisfactory work', '2024-09-29 20:00:00', 1);

GO
