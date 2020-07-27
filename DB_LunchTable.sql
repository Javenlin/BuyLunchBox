
 
/*Customers	   Orders	    OrderDetails  Products	    Suppliers
CustomerID	    OrderID	    OrderID	      ProductID	    SupplierID
StudentName	    CustomerID	ProductID	 ProductName	ShopName
StudentAccount 	OrderDate	UnitPrice	 SupplierID  	Phone
StudentPassword		        Quantity	 UnitPrice	
				
				
Classes				
ClassID				
ClassName				
Room				

*/

--_______________________全部刪除____________________________

IF EXISTS (SELECT * FROM sys.tables WHERE name='Classes')
DROP TABLE Classes   --資料表全部刪除
IF EXISTS (SELECT * FROM sys.tables WHERE name='Customers')
DROP TABLE Customers   --資料表全部刪除

IF EXISTS (SELECT * FROM sys.tables WHERE name='Products')
DROP TABLE Products   --資料表全部刪除
IF EXISTS (SELECT * FROM sys.tables WHERE name='Suppliers')
DROP TABLE Suppliers   --資料表全部刪除


--____________________________________________________________

sp_help Classes
sp_help Customers
sp_help Suppliers
sp_help Products

select*from Classes
select*from Customers
select*from Suppliers
select*from Products

use lunch
go

-------------------------------------------------------------------

create table Classes--課別資料表
(  
  ClassID    varchar(8) primary key,
  ClassName	 varchar(50), 
  Room       varchar(8)   
)
go

insert into Classes
values('C01','行動應用開發程式設計師','R02')
go  
insert into Classes
values('C02','Big Data 資料分析師','R01')
go  
insert into Classes
values('C03','網路系統整合工程師','R03')
go  


select*from Classes

------------------------------------------------------------------

create table Customers--學生資料表
(
  CustomerID      varchar(8)  primary key,
  ClassID         varchar(8)  ,
  StudentName     varchar(12),
  StudentAccount   varchar(20),
  StudentPassword varchar(20),
  foreign key (ClassID) references Classes(ClassID) --關聯-班級ClassID
)
go

--行動應用開發程式設計師
insert into Customers
values('S0980101','C01','林鉦文','s0980101','student1')
go  
insert into Customers
values('S0980102','C01','林鉦貳','s0980102','student2')
go  
insert into Customers
values('S0980103','C01','林鉦參','s0980103','student3')
go  

--Big Data 資料分析師
insert into Customers
values('S0980201','C02','周杰壹','s0980201','student1')
go  
insert into Customers
values('S0980202','C02','周杰貳','s0980202','student2')
go  
insert into Customers
values('S0980203','C02','周杰參','s0980203','student3')
go  

--網路系統整合工程師
insert into Customers
values('S0980301','C03','王力壹','s0980301','student1')
go  
insert into Customers
values('S0980302','C03','王力貳','s0980302','student2')
go  
insert into Customers
values('S0980303','C03','王力參','s0980303','student3')
go  

select*from Customers

--delete from Customers where CustomerID = 'S0980104'

------------------------------------------------------------------

create table Suppliers--店家資料表
(  
  SupplierID    varchar(8) primary key,  
  ShopName	 varchar(50), 
  Phone       varchar(20), 
  
 
)
go

insert into Suppliers
values('H01','丹丹漢堡','072410928')
go  
insert into Suppliers
values('H02','金桔屋排骨飯','072165125')
go  
insert into Suppliers
values('H03','泡菜好忙','072817891')
go  



select*from Suppliers


------------------------------------------------------------------

create table Products--便當資料表
(  
  ProductID      varchar(8) primary key,
  SupplierID     varchar(8),
  PictureID      varchar(8),
  ProductName	 varchar(50), 
  UnitPrice      nvarchar(20),   

   foreign key (SupplierID) references Suppliers(SupplierID)
)
go

insert into Products
values('H01P01','H01','#H01P01','咖哩豬排堡',45)
go  
insert into Products
values('H01P02','H01','#H01P02','泰式椒麻雞堡',45)
go  
insert into Products
values('H01P03','H01','#H01P03','麵線羹',40)
go  

insert into Products
values('H02P01','H02','#H02P01','排骨飯',65)
go  
insert into Products
values('H02P02','H02','#H02P02','雞排飯',85)
go  
insert into Products
values('H02P03','H02','#H02P03','魚排飯',70)
go  

insert into Products
values('H03P01','H03','#H03P01','韓式燒肉飯',80)
go  
insert into Products
values('H03P02','H03','#H03P02','韓式泡菜鍋',80)
go  
insert into Products
values('H03P03','H03','#H03P03','辣炒年糕泡麵',90)
go  



select*from Products
--------------------------------------------------------------------------------------------
create table ChosenAdmin--今日人員表
(  
   ClassID   varchar(8) primary key,
   CustomerID varchar(8),
   StudentName varchar(12)
)
go


insert into ChosenAdmin
values('C01','S0980101','林鉦文')
go  
insert into ChosenAdmin
values('C02','S0980201','周杰壹')
go  
insert into ChosenAdmin
values('C03','S0980301','王力壹')
go  

select*from ChosenAdmin

IF EXISTS (SELECT * FROM sys.tables WHERE name='ChosenShop')
DROP TABLE ChosenShop   --資料表全部刪除




-----------------------------------------------------------------------------------
create table ChosenShop--今日餐資料表
(  
   ClassID     varchar(8) primary key,
   SupplierID  varchar(8),
   ShopName	   varchar(30),
)
go


insert into ChosenShop
values('C01','H01','丹丹漢堡')
go  
insert into ChosenShop
values('C02','H02','金桔屋排骨飯')
go  
insert into ChosenShop
values('C03','H03','泡菜好忙')
go  

select*from ChosenShop

IF EXISTS (SELECT * FROM sys.tables WHERE name='ChosenShop')
DROP TABLE ChosenShop   --資料表全部刪除


-----------------------------------------------------------------------------------------
 CREATE TABLE OrderDetails--訂購明細資料表
   (
     ItemNO      int  not null  primary key identity (1,1), 	 
     ClassID     varchar(8), 
	 OrderDate   datetime ,
	 CustomerID  varchar(8), 
     ProductID   varchar(8),    -- 料品代碼 
     Quantity	 int,       
     RowTotal	 int,  

   )
   go



   insert into OrderDetails
   values('C01','1989/06/18','S0980101','H01P01','5','225')
   go  

   insert into OrderDetails
   values('C01','1989/06/18','S0980101','H01P01','5','225')
   go  
   ---------------------
    insert into OrderDetails
   values('C01','2017/10/10','S0980101','H01P01','5','225')
   go  

   insert into OrderDetails
   values('C01','2017/10/10','S0980101','H01P01','5','225')
   go 

    insert into OrderDetails
   values('C01','2017/10/10','S0980101','H01P03','5','200')
   go  

   insert into OrderDetails
   values('C01','2017/10/10','S0980101','H01P03','5','200')
   go 


   ----------------------
    




   select*from OrderDetails

   IF EXISTS (SELECT * FROM sys.tables WHERE name='OrderDetails')
DROP TABLE OrderDetails   --資料表全部刪除

-----------------------------------------------------------------------------------------

create table OrderMasters--訂購時間資料表
( 
   ClassID     varchar(8) ,
   SupplierID  varchar(8),     
   OrderDate   datetime ,
   SumTotal    int,   
   
   Primary Key(ClassID,OrderDate)    
)
go



insert into OrderMasters
values('C01','H01','1989/06/18','450')
go  

insert into OrderMasters
values('C01','H01','2017/10/10','450')
go  

select*from OrderMasters

IF EXISTS (SELECT * FROM sys.tables WHERE name='OrderMasters')
DROP TABLE OrderMasters   --資料表全部刪除
 
 UPDATE OrderMasters SET              
 SumTotal='450' 
 where ClassID='C01' 
 and OrderDate='1989/06/18' 

  UPDATE OrderMasters SET              
 SumTotal='850' 
 where ClassID='C01' 
 and OrderDate= '2017/10/10'



SELECT DISTINCT RowTotal FROM OrderDetails D inner join OrderMasters M on D.ClassID = M.ClassID 
Where M.OrderDate ='2017/10/10' 
   

 SELECT DISTINCT RowTotal  
 FROM   Customers C, OrderDetails D, OrderMasters M 
 WHERE  C.CustomerID=D.CustomerID and M.ClassID=D.ClassID 
 and M.ClassID='C01'  
 and M.OrderDate ='2017/10/10' 

  SELECT RowTotal  
 FROM Customers C, OrderDetails D, OrderMasters M  
 WHERE C.CustomerID = D.CustomerID and M.ClassID = D.ClassID  
 and M.ClassID = 'C01'  and M.OrderDate = '2017/10/10'  

 SELECT  *
 FROM Customers C, OrderDetails D, OrderMasters M  
 WHERE C.CustomerID = D.CustomerID and M.ClassID = D.ClassID  
 and M.ClassID = 'C01'  and M.OrderDate = '2017/10/10'





 SELECT PruductID Customers C, OrderDetails D, OrderMasters M,Products P 
  FROM C.CustomerID = D.CustomerID and M.ClassID = D.ClassID and P.ProductID=D.ProductID 
 inner join Products II on I.SupplierID = II.SupplierID
 WHERE
 and M.ClassID =  'C01' and M.OrderDate =  '2017/10/11' and D.CustomerID='s0980101'  
 ------------------------------

 /*超級重要*/
 select II.ProductID,IV.ProductName,II.CustomerID,II.Quantity,II.RowTotal
 from (((Customers I  
 inner join OrderDetails II on I.CustomerID = II.CustomerID) 
 inner join OrderMasters III on I.ClassID = III.ClassID) 
 inner join Products IV on II.ProductID = IV.ProductID)           
 where II.OrderDate =  '2017/10/11' and II.CustomerID='s0980101'  
 group by II.CustomerID,II.ProductID,IV.ProductName,II.CustomerID,II.Quantity,II.RowTotal
  /*超級重要*/

select II.ProductID,IV.ProductName,IV.UnitPrice,II.Quantity,II.RowTotal
from(((Customers I 
inner join OrderDetails II on I.CustomerID = II.CustomerID)
inner join OrderMasters III on I.ClassID = III.ClassID)
inner join Products IV on II.ProductID = IV.ProductID) 
where II.OrderDate ='2017/10/17' and II.CustomerID = 'S0980101'
order by itemNO

 ----------------------
 select * from (((ChosenAdmin I  
 inner join ChosenShop II on I.ClassID = II.ClassID) 
 inner join Classes III on I.ClassID = III.ClassID)  
 inner join Customers IV on I.CustomerID = IV.CustomerID)     
 where ClassName like '{cbox_login_class.Text}' ";

   /*所有餐點數量清點*/
 select DISTINCT ProductName from OrderDetails I inner join  Products II on I.ProductID=II.ProductID
   Where I.ClassID ='C01' and I.OrderDate = '2017/10/17' and I.ProductID = 'H01P01'

 select count(*) from OrderDetails Where ClassID ='C01' and OrderDate = '2017/10/17' and ProductID = 'H01P01'
 select RowTotal from OrderDetails Where ClassID ='C01' and OrderDate = '2017/10/17' and ProductID = 'H01P01'


SELECT  DISTINCT D.ItemNO,C.StudentName,D.OrderDate,D.Quantity,D.RowTotal 
FROM Customers C, OrderDetails D, OrderMasters M
WHERE C.CustomerID = D.CustomerID and M.ClassID = D.ClassID 
and D.OrderDate = '2017/10/17' and D.CustomerID='S0980101' 

select Quantity from OrderDetails Where ClassID ='C01' and OrderDate = '2017-10-17' and ProductID = 'H01P01' 

select DISTINCT II.StudentName from OrderDetails I inner join  Customers II on I.CustomerID = II.CustomerID  Where I.ClassID = 'C01' and I.OrderDate = '2017/10/17' and I.CustomerID = 'S0980101'



 SELECT* FROM Suppliers I 
inner join Products II on I.SupplierID = II.SupplierID
WHERE I.ShopName = '{cBox_shop_supplier_name.Text}'

 select DISTINCT OrderDate from OrderMasters where OrderDate = '2017/10/10'and ClassID = 'C01' 

 delete from OrderDetails 
 where CustomerID = 'S0980101' and OrderDate ='2017/10/10'

 select * from OrderDetails 
 where OrderDate ='2017/10/10'and ClassID = 'C01'  

 select DISTINCT OrderDate from OrderDetails 
 where OrderDate ='2017/10/10'and ClassID = 'C01'  

 Select CustomerID,RowTotal 
from OrderDetails  
order by CustomerID

Select ClassID,SumTotal 
from OrderMasters  
order by ClassID

   UPDATE ChosenAdmin SET  
   CustomerID='S0980101'  
     where ClassID = 'C01'

  UPDATE ChosenAdmin SET 
 CustomerID='S0980102' , StudentName='林鉦貳'  
 where ClassID= 'C01'

 SELECT*FROM ChosenAdmin


/*@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@*/

-----------------------------------------------------------------------------------------
--Insert Into [Order](Order_ID,[Date]) Values('20071200710', getdate())

--insert into overtime(s_time) values ('" & TextBox1.Text & "')

--insert into overtime(s_time) values ('1980-2-15 08:30')

 create table Orders--訂單資料表
(  
  OrderID     int PRIMARY KEY IDENTITY,
  CustomerID  varchar(8),
  OrderDate   datetime,
   foreign key (CustomerID) references Customers(CustomerID) --關聯-班級ClassID
)
go

IF EXISTS (SELECT * FROM sys.tables WHERE name='Orders')
DROP TABLE Orders   --資料表全部刪除
---------------------------------------------------------------------------------------------------------

   create table OrderDetails--訂單資料表
(  
  OrderID       int PRIMARY KEY IDENTITY ,
  ProductID	   varchar(8),
  UnitPrice     nvarchar(20),
  Quantity	   nvarchar(20),

   foreign key (ProductID) references Products(ProductID) --關聯-商品ProductID
)

IF EXISTS (SELECT * FROM sys.tables WHERE name='OrderDetails')
DROP TABLE OrderDetails   --資料表全部刪除





IF EXISTS (SELECT * FROM sys.tables WHERE name='employee')
DROP TABLE employee   --如果有employee資料表就刪掉

create table employee--重建資料表
(
  emp_no	char(8) primary key,
  emp_name      char(12),
  dep                  char(6),
  address	char(30),
  sex                   char(1),
  salary               numeric(10,2),
  hired_date       char(10),
  is_foreign        char(1) default 'N'
)
go





sp_help employee
select*from employee-- is_foreign沒有值 就要指定預設值default=(N)

----------------------------------------------------------------------
/*  insert into method 1:完整指定資料行 */
use mis
go

  -- 檢視欄位:
select * from employee

  -- 新增資料(完整欄位)
insert into employee
values('E0010003','jack','D11000','kao','M',20000,'2000/10/10')
go  --錯誤 欄位不符

--@@@@@@
insert into employee
values('E0010003','jack','D11000','kao','M',20000,'2000/10/10',default)
go  --正確 欄位符


--------------------------------------------------------------------------------------
   -- 修改 (default 討論)
insert into employee
values('E0010003','jack','D11000','kao','M',20000,'2000/10/10',default)
 
   -- (null 討論)
insert into employee
values('E0010006','white','D11000','kao','F',null,'2000/10/11',default)

-- 練習: ('E0010008','black','D11001','tpi','M',40000,'2000/12/25','Y' )
insert into employee
values  ('E0010008','black','D11001','tpi','M',40000,'2000/12/25','Y' )

select*from employee

--------------------------------------------------------------------------

--select t. *--or select *
--from titles as t inner join publishers as p on t.pub_id = p.pub_id

select *from (((((Customers I 
inner join Classes II on I.ClassID = II.ClassID)
inner join ChosenShop III on  I.ClassID = III.ClassID)
inner join ChosenAdmin IV on  I.ClassID = IV.ClassID)
inner join Suppliers V on  III.SupplierID = V.SupplierID)
inner join Products VI on V.SupplierID = VI.SupplierID)
Where II.ClassName like '行動應用開發程式設計師' 
and I.CustomerID like 'S0980101'

 select* from ChosenAdmin where ClassID = ( 
   select ClassID from Classes where ClassName like '行動應用開發程式設計師' ) 

select*from ChosenAdmin I
inner join Classes II on I.ClassID = II.ClassID
where ClassName like '行動應用開發程式設計師' 



 select * from (( ChosenAdmin I  
             inner join Classes II on I.ClassID = II.ClassID)  
             inner join Customers III on I.ClassID = III.ClassID) 
             where ClassName like '{cbox_login_class.Text}'  
 

 select * from (((ChosenAdmin I 
             inner join ChosenShop II on I.ClassID = II.ClassID)  
            inner join Classes III on I.ClassID = III.ClassID)  
             inner join Customers IV on I.ClassID = IV.ClassID)              
            where ClassName like '行動應用開發程式設計師'  
            SqlCommand cmd4 = new SqlCommand(strSQL4, con);
            //行動應用開發程式設計師

 select* from Suppliers I 
 inner join Products II on I.SupplierID =II.SupplierID where I.ShopName ='丹丹漢堡'

  (  
 select SupplierID from Suppliers where ShopName = '丹丹漢堡' )
 
 select ProductID,UnitPrice from products where ProductName ='排骨飯';   