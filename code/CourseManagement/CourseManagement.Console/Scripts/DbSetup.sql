DECLARE @SubjectId as INT;
DECLARE @AccountId as INT;

INSERT INTO [CourseManagement.Persistence.CourseManagementContext].dbo.Subjects
	VALUES ('Tecnicas de Diseño');
	
SET @SubjectId = SCOPE_IDENTITY();

INSERT INTO [CourseManagement.Persistence.CourseManagementContext].dbo.Accounts
	VALUES ('coursemanagement2012@gmail.com', 'AABBCCDDEE');
	
SET @AccountId = SCOPE_IDENTITY();

INSERT INTO [CourseManagement.Persistence.CourseManagementContext].dbo.Configurations
	VALUES ('smtp', 'smtp.gmail.com', 587, 1, @AccountId);

INSERT INTO [CourseManagement.Persistence.CourseManagementContext].dbo.Configurations
	VALUES ('pop', 'pop.gmail.com', 995, 1, @AccountId);
	
INSERT INTO [CourseManagement.Persistence.CourseManagementContext].dbo.Courses
	VALUES (2012, 2, @SubjectId, @AccountId, 'coursemanagement2012@gmail.com');