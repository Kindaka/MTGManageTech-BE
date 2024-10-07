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