create table News
(
    NewsId        nvarchar(max),
    Title         nvarchar(max),
    Content       nvarchar(max),
    Url           nvarchar(max),
    DatePublished datetime not null
)
go
