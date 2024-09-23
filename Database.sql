USE [master]
GO

DROP DATABASE [MartyrGravesManagement]
GO

CREATE DATABASE [MartyrGravesManagement]
GO

USE [MartyrGravesManagement]
GO


-- Create Service_Category table
CREATE TABLE Service_Category (
    CategoryId INT PRIMARY KEY IDENTITY(1,1),
    CategoryName NVARCHAR(100) NOT NULL,
    Status BIT
);

-- Create Role table
CREATE TABLE Role (
    RoleId INT PRIMARY KEY IDENTITY(1,1),
    RoleName VARCHAR(100) NOT NULL,
    Description VARCHAR(255) NULL,
    Status BIT
);

-- Create Account table
CREATE TABLE Account (
    AccountId INT PRIMARY KEY IDENTITY(1,1),
    RoleId INT,
	AreaId INT NULL,
    EmailAddress VARCHAR(100) NOT NULL,
    HashedPassword VARCHAR(MAX) NOT NULL,
    FullName NVARCHAR(100) NULL,
    DateOfBirth DATETIME NULL,
    PhoneNumber VARCHAR(10) NULL,
    Address VARCHAR(255) NULL,
    AvatarPath VARCHAR(MAX) NULL,
    Status BIT,
    FOREIGN KEY (RoleId) REFERENCES [Role](RoleId)
);



-- Create Service table
CREATE TABLE Service (
    ServiceId INT PRIMARY KEY IDENTITY(1,1),
    CategoryId INT,
    ServiceName NVARCHAR(100) NOT NULL,
    Description NVARCHAR(250) NULL,
    Price DECIMAL(13, 2) NOT NULL CHECK (Price >= 0),
    Status BIT,
    FOREIGN KEY (CategoryId) REFERENCES Service_Category(CategoryId)
);


-- Create Area table
CREATE TABLE Area (
    AreaId INT PRIMARY KEY IDENTITY(1,1),
    AreaName NVARCHAR(100) NOT NULL,
    Description NVARCHAR(250) NULL,
    Status BIT
);

-- Create Martyr_Grave table
CREATE TABLE Martyr_Grave (
    MartyrId INT PRIMARY KEY IDENTITY(1,1),
    AreaId INT NOT NULL,
    MartyrCode VARCHAR(50) NOT NULL,
    RowNumber INT NOT NULL,
    MartyrNumber INT NOT NULL,
    AreaNumber INT NOT NULL,
    Status BIT,
    FOREIGN KEY (AreaId) REFERENCES Area(AreaId)
);

-- Create Order table
CREATE TABLE [Order] (
    OrderId INT PRIMARY KEY IDENTITY(1,1),
    ServiceId INT NOT NULL,
    MartyrId INT NOT NULL,
    AccountId INT NOT NULL,
    OrderDate DATETIME NOT NULL,
    StartDate DATETIME NOT NULL,
    EndDate DATETIME NOT NULL,
    TotalPrice DECIMAL(13, 2) NOT NULL CHECK (TotalPrice >=0),
    Status BIT,
    FOREIGN KEY (ServiceId) REFERENCES Service(ServiceId),
    FOREIGN KEY (MartyrId) REFERENCES Martyr_Grave(MartyrId),
    FOREIGN KEY (AccountId) REFERENCES Account(AccountId)
);

-- Create OrderDetail table
CREATE TABLE OrderDetail (
    DetailId INT PRIMARY KEY IDENTITY(1,1),
    ServiceId INT NOT NULL,
    MartyrId INT NOT NULL,
    OrderPrice DECIMAL(13, 2) NOT NULL CHECK (OrderPrice >= 0),
    Quantity INT NOT NULL CHECK (Quantity >= 0),
    Status BIT,
    FOREIGN KEY (ServiceId) REFERENCES Service(ServiceId),
    FOREIGN KEY (MartyrId) REFERENCES Martyr_Grave(MartyrId)
);

-- Create CartItem table
CREATE TABLE CartItem (
    CartId INT PRIMARY KEY IDENTITY(1,1),
    AccountId INT NOT NULL,
    ServiceId INT NOT NULL,
    MartyrId INT NOT NULL,
    CartQuantity INT NULL,
    Status BIT,
    FOREIGN KEY (AccountId) REFERENCES Account(AccountId),
    FOREIGN KEY (ServiceId) REFERENCES Service(ServiceId),
    FOREIGN KEY (MartyrId) REFERENCES Martyr_Grave(MartyrId)
);

-- Create Payment table
CREATE TABLE Payment (
    PaymentId INT PRIMARY KEY IDENTITY(1,1),
    OrderId INT,
    PaymentMethod NVARCHAR(100),
    BankCode VARCHAR(MAX),
    BankTranNo VARCHAR(MAX),
    CardType NVARCHAR(MAX),
    PayDate DATETIME,
    TransactionNo NVARCHAR(MAX),
    TransactionStatus INT,
    PaymentAmount DECIMAL(13, 2),
    FOREIGN KEY (OrderId) REFERENCES [Order](OrderId)
);

-- Create Feedback table
CREATE TABLE Feedback (
    FeedbackId INT PRIMARY KEY IDENTITY(1,1),
    AccountId INT NOT NULL,
    OrderId INT NOT NULL,
    Content NVARCHAR(500) NOT NULL,
    CreateAt DATETIME NOT NULL,
    UpdateAt DATETIME NOT NULL,
    Status BIT,
    FOREIGN KEY (AccountId) REFERENCES Account(AccountId),
    FOREIGN KEY (OrderId) REFERENCES [Order](OrderId)
);

-- Create Feedback_Response table
CREATE TABLE Feedback_Response (
    Id INT PRIMARY KEY IDENTITY(1,1),
    AccountId INT NOT NULL,
    Content NVARCHAR(500) NOT NULL,
    CreateAt DATETIME NOT NULL,
    UpdateAt DATETIME NULL,
    Status BIT,
    FOREIGN KEY (AccountId) REFERENCES Account(AccountId)
);

-- Create Martyr_Grave_Information table
CREATE TABLE Martyr_Grave_Information (
    InformationId INT PRIMARY KEY IDENTITY(1,1),
    MartyrId INT NOT NULL,
    Name NVARCHAR(100) NOT NULL,
    DateOfBirth DATETIME NOT NULL,
    Description NVARCHAR(500) NULL,
    DateOfSacrifice DATETIME NOT NULL,
    FOREIGN KEY (MartyrId) REFERENCES Martyr_Grave(MartyrId)
);

-- Create Grave_Image table
CREATE TABLE Grave_Image (
    Id INT PRIMARY KEY IDENTITY(1,1),
    MartyrId INT NOT NULL,
    UrlPath VARCHAR(MAX) NOT NULL,
    FOREIGN KEY (MartyrId) REFERENCES Martyr_Grave(MartyrId)
);

-- Create Weekly_Report_Grave table
CREATE TABLE Weekly_Report_Grave (
    WeeklyReportId INT PRIMARY KEY IDENTITY(1,1) ,
    MartyrId INT NOT NULL,
    AccountId INT NOT NULL,
    QualityOfTotalGrave_Point INT NOT NULL,
    QualityOfFlower_Point INT NOT NULL,
    UploadDate DATETIME NOT NULL,
    Discipline_Point INT NOT NULL,
    Description NVARCHAR(500) NULL,
    Status BIT,
    FOREIGN KEY (MartyrId) REFERENCES Martyr_Grave(MartyrId),
    FOREIGN KEY (AccountId) REFERENCES Account(AccountId)
);

-- Create Work_Performance table
CREATE TABLE Work_Performance (
    WorkId INT PRIMARY KEY IDENTITY(1,1),
    AccountId INT NOT NULL,
    QualityOfMaintenance_Point INT NOT NULL,
    TimeComplete_Point INT NOT NULL,
    Interaction_Point INT NOT NULL,
    Discipline_Point INT NOT NULL,
    Description NVARCHAR(500) NULL,
    UploadTime DATETIME NOT NULL,
    FOREIGN KEY (AccountId) REFERENCES Account(AccountId)
);

-- Create Task table
CREATE TABLE Task (
    TaskId INT PRIMARY KEY IDENTITY(1,1), 
    AccountId INT NOT NULL,
    OrderId INT NOT NULL,
    NameOfWork NVARCHAR(100) NULL,
    TypeOfWork BIT NULL,
    StartDate DATETIME NOT NULL,
    EndDate DATETIME NOT NULL,
    Description NVARCHAR(500) NULL,
    Status INT,
    FOREIGN KEY (AccountId) REFERENCES Account(AccountId),
    FOREIGN KEY (OrderId) REFERENCES [Order](OrderId)
);
