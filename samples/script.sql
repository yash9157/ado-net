USE [EmployeeManagementDB]
GO
/****** Object:  Table [dbo].[Departments]    Script Date: 25-09-2026 09:29:25 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Departments](
	[DepartmentId] [int] IDENTITY(1,1) NOT NULL,
	[DepartmentName] [nvarchar](100) NOT NULL,
PRIMARY KEY CLUSTERED
(
	[DepartmentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Employees]    Script Date: 25-09-2026 09:29:25 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Employees](
	[EmployeeId] [int] IDENTITY(1,1) NOT NULL,
	[FirstName] [nvarchar](100) NOT NULL,
	[LastName] [nvarchar](100) NOT NULL,
	[Email] [nvarchar](100) NOT NULL,
	[phone] [nvarchar](50) NULL,
	[salary] [decimal](10, 2) NULL,
	[DateOfBirth] [date] NULL,
	[JoiningDate] [datetime] NULL,
	[Gender] [nvarchar](20) NULL,
	[IsActive] [bit] NOT NULL,
	[DepartmentId] [int] NOT NULL,
PRIMARY KEY CLUSTERED
(
	[EmployeeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Users]    Script Date: 25-09-2026 09:29:25 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Users](
	[UserId] [int] IDENTITY(1,1) NOT NULL,
	[Email] [nvarchar](150) NOT NULL,
	[PasswordHash] [nvarchar](500) NOT NULL,
	[Role] [nvarchar](50) NOT NULL,
	[IsActive] [bit] NOT NULL,
PRIMARY KEY CLUSTERED
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED
(
	[Email] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Employees] ADD  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[Users] ADD  DEFAULT ('User') FOR [Role]
GO
ALTER TABLE [dbo].[Users] ADD  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[Employees]  WITH CHECK ADD  CONSTRAINT [FK_Employees_Departments] FOREIGN KEY([DepartmentId])
REFERENCES [dbo].[Departments] ([DepartmentId])
GO
ALTER TABLE [dbo].[Employees] CHECK CONSTRAINT [FK_Employees_Departments]
GO
/****** Object:  StoredProcedure [dbo].[sp_DeleteEmployee]    Script Date: 25-09-2026 09:29:25 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_DeleteEmployee]
    @EmployeeId INT
AS
BEGIN
    DELETE FROM Employees
    WHERE EmployeeId = @EmployeeId;
END;

GO
/****** Object:  StoredProcedure [dbo].[sp_GetDepartments]    Script Date: 25-09-2026 09:29:25 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_GetDepartments]
AS
BEGIN
    SELECT
        DepartmentId,
        DepartmentName
    FROM Departments
    ORDER BY DepartmentName;
END;

GO
/****** Object:  StoredProcedure [dbo].[sp_GetEmployee]    Script Date: 25-09-2026 09:29:25 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
Create procedure [dbo].[sp_GetEmployee]
As
Begin
 Select e.EmployeeId,
        e.FirstName,
        e.LastName,
        e.Email,
        e.Phone,
        e.Salary,
        e.DateOfBirth,
        e.JoiningDate,
        e.Gender,
        e.IsActive,
        e.DepartmentId,
        d.DepartmentName
 From Employees e
 inner join Departments d
 on e.DepartmentId = d.DepartmentId
End;
GO
/****** Object:  StoredProcedure [dbo].[sp_GetEmployeeById]    Script Date: 25-09-2026 09:29:25 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_GetEmployeeById]
    @EmployeeId INT
AS
BEGIN
    SELECT
        e.EmployeeId,
        e.FirstName,
        e.LastName,
        e.Email,
        e.Phone,
        e.Salary,
        e.DateOfBirth,
        e.JoiningDate,
        e.Gender,
        e.IsActive,
        e.DepartmentId,
        d.DepartmentName
    FROM Employees e
    INNER JOIN Departments d
        ON e.DepartmentId = d.DepartmentId
    WHERE e.EmployeeId = @EmployeeId;
END;
GO
/****** Object:  StoredProcedure [dbo].[sp_GetUserByEmail]    Script Date: 25-09-2026 09:29:25 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_GetUserByEmail]
    @Email NVARCHAR(150)
AS
BEGIN
    SELECT
        UserId,
        Email,
        PasswordHash,
        Role,
        IsActive
    FROM Users
    WHERE Email = @Email;
END;
GO
/****** Object:  StoredProcedure [dbo].[sp_InsertEmployee]    Script Date: 25-09-2026 09:29:25 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
Create Procedure [dbo].[sp_InsertEmployee]
  @FirstName nvarchar(100),
  @LastName nvarcHar(100),
  @Email nvarchar(50),
  @phone nvarchar(20),
  @salary decimal(10,2),
  @DateOFBirth date,
  @JoiningDate datetime,
  @Gender NvarChar(20),
  @IsActive bit,
  @DepartmentId int
As
Begin
   Insert into Employees (
       FirstName,
        LastName,
        Email,
        phone,
        salary,
        DateOfBirth,
        JoiningDate,
        Gender,
        IsActive,
        DepartmentId
   )
   values (
     @FirstName,
        @LastName,
        @Email,
        @phone,
        @salary,
        @DateOfBirth,
        @JoiningDate,
        @Gender,
        @IsActive,
        @DepartmentId
	 )
End;
GO
/****** Object:  StoredProcedure [dbo].[sp_RegisterUser]    Script Date: 25-09-2026 09:29:25 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_RegisterUser]
    @Email NVARCHAR(150),
    @PasswordHash NVARCHAR(500),
    @Role NVARCHAR(50)
AS
BEGIN
    INSERT INTO Users
    (
        Email,
        PasswordHash,
        Role
    )
    VALUES
    (
        @Email,
        @PasswordHash,
        @Role
    );
END;
GO
/****** Object:  StoredProcedure [dbo].[sp_UpdateEmployee]    Script Date: 25-09-2026 09:29:25 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_UpdateEmployee]
    @EmployeeId INT,
    @FirstName NVARCHAR(100) = NULL,
    @LastName NVARCHAR(100) = NULL,
    @Email NVARCHAR(150) = NULL,
    @Phone NVARCHAR(20) = NULL,
    @Salary DECIMAL(10,2) = NULL,
    @DateOfBirth DATE = NULL,
    @JoiningDate DATE = NULL,
    @Gender NVARCHAR(20) = NULL,
    @IsActive BIT = NULL,
    @DepartmentId INT = NULL
AS
BEGIN

    UPDATE Employees
    SET
        FirstName = COALESCE(@FirstName, FirstName),
        LastName = COALESCE(@LastName, LastName),
        Email = COALESCE(@Email, Email),
        Phone = COALESCE(@Phone, Phone),
        Salary = COALESCE(@Salary, Salary),
        DateOfBirth = COALESCE(@DateOfBirth, DateOfBirth),
        JoiningDate = COALESCE(@JoiningDate, JoiningDate),
        Gender = COALESCE(@Gender, Gender),
        IsActive = COALESCE(@IsActive, IsActive),
        DepartmentId = COALESCE(@DepartmentId, DepartmentId)

    WHERE EmployeeId = @EmployeeId;

END;
GO
