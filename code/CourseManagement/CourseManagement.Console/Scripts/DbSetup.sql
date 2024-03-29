DECLARE @SubjectId as INT;
DECLARE @AccountId as INT;
DECLARE @CourseId as INT;

INSERT INTO [CourseManagement.Persistence.CourseManagementContext].dbo.Subjects
	VALUES ('Tecnicas de Dise�o');
	
SET @SubjectId = SCOPE_IDENTITY();

INSERT INTO [CourseManagement.Persistence.CourseManagementContext].dbo.Accounts
	VALUES ('coursemanagement2012@gmail.com', 'AABBCCDDEE');
	
SET @AccountId = SCOPE_IDENTITY();

INSERT INTO [CourseManagement.Persistence.CourseManagementContext].dbo.Configurations
	VALUES ('smtp', 'smtp.gmail.com', 587, 1, @AccountId);

INSERT INTO [CourseManagement.Persistence.CourseManagementContext].dbo.Configurations
	VALUES ('pop', 'pop.gmail.com', 995, 1, @AccountId);
	
INSERT INTO [CourseManagement.Persistence.CourseManagementContext].dbo.Courses
	VALUES (2012, 2, @SubjectId, @AccountId, 'cmpublicdistributionlist@gmail.com');

SET @CourseId = SCOPE_IDENTITY();
	
INSERT INTO [CourseManagement.Persistence.CourseManagementContext].dbo.Teachers
	VALUES (1,'Sample Teacher','cmteacher2012@gmail.com');

INSERT INTO [CourseManagement.Persistence.CourseManagementContext].dbo.TeacherCourses
	VALUES(1, @CourseId);