create database ShoppingCart 
go

use ShoppingCart
go

create table [dbo].[ShoppingCart] (
[ID] int Identity(1,1) Primary Key,
[UserId] [bigint] not null,
constraint ShoppingCartUnique Unique ([ID],[UserId])
)
go

create index ShoppingCart_UserID
on [dbo].[ShoppingCart] (UserId)
go


create table [dbo].[ShoppingCartItem] (
[ID] int Identity (1,1) Primary Key,
[ShoppingCartId] int not null,
[ProductCatalogId] [bigint] not null,
[ProductName] [nvarchar](100) not null,
[ProductDescription] [nvarchar](500)  null,
[Amount] [int] not null,
[Currency] [nvarchar](5) not null
)
go

alter table [dbo].[ShoppingCartItem] with check add constraint[FK_ShoppingCart] 
foreign key([ShoppingCartId])
references [dbo].[ShoppingCart] ([Id])
go

alter table [dbo].[ShoppingCartItem] check constraint [FK_ShoppingCart]
go

Create index ShoppingCartItem_ShoppingCartId
on [dbo].[ShoppingCartItem] (ShoppingCartId)
go




Create table [dbo].[EventStore](
[ID] int Identity(1,1) Primary Key,
[Name] [nvarchar](100) not null,
[OccurredAt] [DateTimeOffset] not null,
[Content] [nvarchar](max) not null
)
go
