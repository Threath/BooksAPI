IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'Books')
BEGIN
	Create DATABASE Books;
END;

GO
USE Books
GO
IF ((NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Book')) AND(NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Author')) AND (NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'BookAuthor'))) 
BEGIN
Create table Book
(
	Id int Not Null,
	Title NVARCHAR(100) Not Null,
	Description NVARCHAR(MAX) Not Null,
	Rating decimal Not null,
	ISBN NVARCHAR(13) Not Null,
	PublicationDate Datetime2 Not Null,	Primary Key(Id)
)
Create table Author
(
	Id int Not Null,
	FirstName NVARCHAR(50) Not Null,
	LastName NVARCHAR(50) Not Null,
	BirthDate Datetime2 Not Null,
	Gender bit Not Null,
	Primary Key(Id)
)
Create table BookAuthor
(
	BookId int Foreign Key References Book(Id) Not Null,
	AuthorId int Foreign Key References Author(Id) Not Null,
	primary key(BookId, AuthorId)
)
END