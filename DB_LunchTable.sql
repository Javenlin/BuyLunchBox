
 
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

--_______________________�����R��____________________________

IF EXISTS (SELECT * FROM sys.tables WHERE name='Classes')
DROP TABLE Classes   --��ƪ�����R��
IF EXISTS (SELECT * FROM sys.tables WHERE name='Customers')
DROP TABLE Customers   --��ƪ�����R��

IF EXISTS (SELECT * FROM sys.tables WHERE name='Products')
DROP TABLE Products   --��ƪ�����R��
IF EXISTS (SELECT * FROM sys.tables WHERE name='Suppliers')
DROP TABLE Suppliers   --��ƪ�����R��


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

create table Classes--�ҧO��ƪ�
(  
  ClassID    varchar(8) primary key,
  ClassName	 varchar(50), 
  Room       varchar(8)   
)
go

insert into Classes
values('C01','������ζ}�o�{���]�p�v','R02')
go  
insert into Classes
values('C02','Big Data ��Ƥ��R�v','R01')
go  
insert into Classes
values('C03','�����t�ξ�X�u�{�v','R03')
go  


select*from Classes

------------------------------------------------------------------

create table Customers--�ǥ͸�ƪ�
(
  CustomerID      varchar(8)  primary key,
  ClassID         varchar(8)  ,
  StudentName     varchar(12),
  StudentAccount   varchar(20),
  StudentPassword varchar(20),
  foreign key (ClassID) references Classes(ClassID) --���p-�Z��ClassID
)
go

--������ζ}�o�{���]�p�v
insert into Customers
values('S0980101','C01','�Lತ�','s0980101','student1')
go  
insert into Customers
values('S0980102','C01','�LಶL','s0980102','student2')
go  
insert into Customers
values('S0980103','C01','�Lರ�','s0980103','student3')
go  

--Big Data ��Ƥ��R�v
insert into Customers
values('S0980201','C02','�P�N��','s0980201','student1')
go  
insert into Customers
values('S0980202','C02','�P�N�L','s0980202','student2')
go  
insert into Customers
values('S0980203','C02','�P�N��','s0980203','student3')
go  

--�����t�ξ�X�u�{�v
insert into Customers
values('S0980301','C03','���O��','s0980301','student1')
go  
insert into Customers
values('S0980302','C03','���O�L','s0980302','student2')
go  
insert into Customers
values('S0980303','C03','���O��','s0980303','student3')
go  

select*from Customers

--delete from Customers where CustomerID = 'S0980104'

------------------------------------------------------------------

create table Suppliers--���a��ƪ�
(  
  SupplierID    varchar(8) primary key,  
  ShopName	 varchar(50), 
  Phone       varchar(20), 
  
 
)
go

insert into Suppliers
values('H01','�����~��','072410928')
go  
insert into Suppliers
values('H02','���ܫαư���','072165125')
go  
insert into Suppliers
values('H03','�w��n��','072817891')
go  



select*from Suppliers


------------------------------------------------------------------

create table Products--�K���ƪ�
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
values('H01P01','H01','#H01P01','�@���ޱƳ�',45)
go  
insert into Products
values('H01P02','H01','#H01P02','�����Գ�����',45)
go  
insert into Products
values('H01P03','H01','#H01P03','�ѽuü',40)
go  

insert into Products
values('H02P01','H02','#H02P01','�ư���',65)
go  
insert into Products
values('H02P02','H02','#H02P02','���ƶ�',85)
go  
insert into Products
values('H02P03','H02','#H02P03','���ƶ�',70)
go  

insert into Products
values('H03P01','H03','#H03P01','�����N�׶�',80)
go  
insert into Products
values('H03P02','H03','#H03P02','�����w����',80)
go  
insert into Products
values('H03P03','H03','#H03P03','�����~�|�w��',90)
go  



select*from Products
--------------------------------------------------------------------------------------------
create table ChosenAdmin--����H����
(  
   ClassID   varchar(8) primary key,
   CustomerID varchar(8),
   StudentName varchar(12)
)
go


insert into ChosenAdmin
values('C01','S0980101','�Lತ�')
go  
insert into ChosenAdmin
values('C02','S0980201','�P�N��')
go  
insert into ChosenAdmin
values('C03','S0980301','���O��')
go  

select*from ChosenAdmin

IF EXISTS (SELECT * FROM sys.tables WHERE name='ChosenShop')
DROP TABLE ChosenShop   --��ƪ�����R��




-----------------------------------------------------------------------------------
create table ChosenShop--�����\��ƪ�
(  
   ClassID     varchar(8) primary key,
   SupplierID  varchar(8),
   ShopName	   varchar(30),
)
go


insert into ChosenShop
values('C01','H01','�����~��')
go  
insert into ChosenShop
values('C02','H02','���ܫαư���')
go  
insert into ChosenShop
values('C03','H03','�w��n��')
go  

select*from ChosenShop

IF EXISTS (SELECT * FROM sys.tables WHERE name='ChosenShop')
DROP TABLE ChosenShop   --��ƪ�����R��


-----------------------------------------------------------------------------------------
 CREATE TABLE OrderDetails--�q�ʩ��Ӹ�ƪ�
   (
     ItemNO      int  not null  primary key identity (1,1), 	 
     ClassID     varchar(8), 
	 OrderDate   datetime ,
	 CustomerID  varchar(8), 
     ProductID   varchar(8),    -- �ƫ~�N�X 
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
DROP TABLE OrderDetails   --��ƪ�����R��

-----------------------------------------------------------------------------------------

create table OrderMasters--�q�ʮɶ���ƪ�
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
DROP TABLE OrderMasters   --��ƪ�����R��
 
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

 /*�W�ŭ��n*/
 select II.ProductID,IV.ProductName,II.CustomerID,II.Quantity,II.RowTotal
 from (((Customers I  
 inner join OrderDetails II on I.CustomerID = II.CustomerID) 
 inner join OrderMasters III on I.ClassID = III.ClassID) 
 inner join Products IV on II.ProductID = IV.ProductID)           
 where II.OrderDate =  '2017/10/11' and II.CustomerID='s0980101'  
 group by II.CustomerID,II.ProductID,IV.ProductName,II.CustomerID,II.Quantity,II.RowTotal
  /*�W�ŭ��n*/

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

   /*�Ҧ��\�I�ƶq�M�I*/
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
 CustomerID='S0980102' , StudentName='�LಶL'  
 where ClassID= 'C01'

 SELECT*FROM ChosenAdmin


/*@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@*/

-----------------------------------------------------------------------------------------
--Insert Into [Order](Order_ID,[Date]) Values('20071200710', getdate())

--insert into overtime(s_time) values ('" & TextBox1.Text & "')

--insert into overtime(s_time) values ('1980-2-15 08:30')

 create table Orders--�q���ƪ�
(  
  OrderID     int PRIMARY KEY IDENTITY,
  CustomerID  varchar(8),
  OrderDate   datetime,
   foreign key (CustomerID) references Customers(CustomerID) --���p-�Z��ClassID
)
go

IF EXISTS (SELECT * FROM sys.tables WHERE name='Orders')
DROP TABLE Orders   --��ƪ�����R��
---------------------------------------------------------------------------------------------------------

   create table OrderDetails--�q���ƪ�
(  
  OrderID       int PRIMARY KEY IDENTITY ,
  ProductID	   varchar(8),
  UnitPrice     nvarchar(20),
  Quantity	   nvarchar(20),

   foreign key (ProductID) references Products(ProductID) --���p-�ӫ~ProductID
)

IF EXISTS (SELECT * FROM sys.tables WHERE name='OrderDetails')
DROP TABLE OrderDetails   --��ƪ�����R��





IF EXISTS (SELECT * FROM sys.tables WHERE name='employee')
DROP TABLE employee   --�p�G��employee��ƪ�N�R��

create table employee--���ظ�ƪ�
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
select*from employee-- is_foreign�S���� �N�n���w�w�]��default=(N)

----------------------------------------------------------------------
/*  insert into method 1:������w��Ʀ� */
use mis
go

  -- �˵����:
select * from employee

  -- �s�W���(�������)
insert into employee
values('E0010003','jack','D11000','kao','M',20000,'2000/10/10')
go  --���~ ��줣��

--@@@@@@
insert into employee
values('E0010003','jack','D11000','kao','M',20000,'2000/10/10',default)
go  --���T ����


--------------------------------------------------------------------------------------
   -- �ק� (default �Q��)
insert into employee
values('E0010003','jack','D11000','kao','M',20000,'2000/10/10',default)
 
   -- (null �Q��)
insert into employee
values('E0010006','white','D11000','kao','F',null,'2000/10/11',default)

-- �m��: ('E0010008','black','D11001','tpi','M',40000,'2000/12/25','Y' )
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
Where II.ClassName like '������ζ}�o�{���]�p�v' 
and I.CustomerID like 'S0980101'

 select* from ChosenAdmin where ClassID = ( 
   select ClassID from Classes where ClassName like '������ζ}�o�{���]�p�v' ) 

select*from ChosenAdmin I
inner join Classes II on I.ClassID = II.ClassID
where ClassName like '������ζ}�o�{���]�p�v' 



 select * from (( ChosenAdmin I  
             inner join Classes II on I.ClassID = II.ClassID)  
             inner join Customers III on I.ClassID = III.ClassID) 
             where ClassName like '{cbox_login_class.Text}'  
 

 select * from (((ChosenAdmin I 
             inner join ChosenShop II on I.ClassID = II.ClassID)  
            inner join Classes III on I.ClassID = III.ClassID)  
             inner join Customers IV on I.ClassID = IV.ClassID)              
            where ClassName like '������ζ}�o�{���]�p�v'  
            SqlCommand cmd4 = new SqlCommand(strSQL4, con);
            //������ζ}�o�{���]�p�v

 select* from Suppliers I 
 inner join Products II on I.SupplierID =II.SupplierID where I.ShopName ='�����~��'

  (  
 select SupplierID from Suppliers where ShopName = '�����~��' )
 
 select ProductID,UnitPrice from products where ProductName ='�ư���';   