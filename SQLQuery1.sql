create table Clients(
IdClient int primary key identity(1,1) not null,
Organization nvarchar(100) not null,
AgentName nvarchar(100) not null,
PhoneNumber nvarchar(20) not null
)

create table Projects(
IdProject int primary key identity(1,1) not null,
IdClient int foreign key references Clients(IdClient),
ContractNumber nvarchar(100) not null,
StartDate date not null,
EndDate date not null,
Territory nvarchar(200) not null
)

create table Sectors(
IdSector int primary key identity(1,1) not null,
IdProject int foreign key references Projects(IdProject),
SquareSector int not null
)

create table SectorCoordinates(
IdSector int foreign key references Sectors(IdSector),
IdAngle int not null,
CoordX int not null,
CoordY int not null,
primary key (IdSector, IdAngle)
)

create table Equipment(
IdEquipment int primary key identity(1,1) not null,
SerialNumber nvarchar(100) not null,
Manufacturer nvarchar(200) not null
)

create table Profiles(
IdProfile int primary key identity(1,1) not null,
IdSector int foreign key references Sectors(IdSector),
IdEquipment int foreign key references Equipment(IdEquipment)
)

create table ProfileCoordinates(
IdProfile int foreign key references Profiles(IdProfile),
IdCoord int not null,
CoordX int not null,
CoordY int not null,
primary key (IdProfile, IdCoord)
)

create table Pickets(
IdPicket int primary key identity(1,1) not null,
IdProfile int foreign key references Profiles(IdProfile),
CoordX int not null,
CoordY int not null
)

create table Measurements(
IdPicket1 int foreign key references Pickets(IdPicket),
IdPicket2 int foreign key references Pickets(IdPicket),
Depth int not null,
PotentialDifference float not null,
primary key (IdPicket1, IdPicket2, Depth)
)

insert into Clients values 
('organization', 'ZagorulkoKV', '89139131319');

insert into Projects values 
(1, 1, '2024-01-10', '2025-03-10', 'territory');

insert into Sectors values 
(1, 10900),
(1, 18037),
(1, 5750),
(1, 24625);

insert into SectorCoordinates values 
(1, 1, 60, 20),
(1, 2, 150, 30),
(1, 3, 70, 180),
(1, 4, 20, 130),
(2, 1, 190, 25),
(2, 2, 180, 220),
(2, 3, 70, 225),
(3, 1, 210, 30),
(3, 2, 240, 30),
(3, 3, 275, 270),
(3, 4, 200, 220),
(4, 1, 270, 65),
(4, 2, 350, 40),
(4, 3, 425, 70),
(4, 4, 445, 115),
(4, 5, 400, 190),
(4, 6, 300, 240);

insert into Equipment values 
('s1p1', 'aaa'),
('s1p2', 'aaa'),
('s1p3', 'bb'),
('s1p4', 'aaa'),
('s1p5', 'aaa'),
('s1p6', 'bb'),
('s1p7', 'bb');

insert into Profiles values 
(1, 1),
(1, 2),
(1, 3),
(1, 4),
(1, 5),
(1, 6),
(1, 7),
(2, 1),
(2, 2),
(2, 3),
(2, 4),
(2, 5),
(2, 6),
(2, 7);

insert into ProfileCoordinates values 
(1, 1, 60, 25),
(1, 2, 145, 35),
(2, 1, 55, 35),
(2, 2, 140, 50),
(3, 1, 50, 55),
(3, 2, 125, 80),
(4, 1, 40, 70),
(4, 2, 110, 110),
(5, 1, 30, 90),
(5, 2, 90, 140),
(6, 1, 25, 115),
(6, 2, 80, 160),
(7, 1, 10, 130),
(7, 2, 70, 180);

insert into Pickets values 
(1, 60, 20),
(1, 65, 21),
(1, 70, 22),
(1, 75, 23),
(1, 80, 23),
(1, 85, 24),
(1, 90, 25),
(1, 95, 26),
(1, 100, 27),
(1, 105, 28),
(1, 110, 28),
(1, 115, 29),
(1, 120, 30),
(1, 125, 31),
(1, 130, 32),
(1, 135, 33),
(1, 140, 34),
(1, 145, 35);

insert into Measurements values 
( 1, 2, 1, 1325),
( 2, 3, 1, 2380),
( 3, 4, 1, 2500),
( 4, 5, 1, 2520),
( 5, 6, 1, 2180),
( 6, 7, 1, 3400),
( 7, 8, 1, 3200),
( 8, 9, 1, 2910),
( 9, 10, 1, 2908),
( 10, 11, 1, 2846),
( 11, 12, 1, 2115),
( 12, 13, 1, 2510),
( 13, 14, 1, 2688),
( 14, 15, 1, 2560),
( 15, 16, 1, 1800),
( 16, 17, 1, 2610),
( 17, 18, 1, 2856),
( 2, 3, 2, 2510),
( 3, 4, 2, 2660),
( 4, 5, 2, 9700),
( 5, 6, 2, 9770),
( 6, 7, 2, 3400),
( 7, 8, 2, 3350),
( 8, 9, 2, 1835),
( 9, 10, 2, 550),
( 10, 11, 2, 200),
( 11, 12, 2, 1348),
( 12, 13, 2, 1564),
( 13, 14, 2, 1640),
( 14, 15, 2, 1890),
( 15, 16, 2, 2500),
( 16, 17, 2, 9650),
( 3, 4, 3, 9810),
( 4, 5, 3, 4570),
( 5, 6, 3, 3875),
( 6, 7, 3, 1112),
( 7, 8, 3, 1005),
( 8, 9, 3, 890),
( 9, 10, 3, 550),
( 10, 11, 3, 201),
( 11, 12, 3, 189),
( 12, 13, 3, 176),
( 13, 14, 3, 200),
( 14, 15, 3, 1205),
( 15, 16, 3, 2452),
( 4, 5, 4, 705),
( 5, 6, 4, 810),
( 6, 7, 4, 999),
( 7, 8, 4, 1006),
( 8, 9, 4, 595),
( 9, 10, 4, 980),
( 10, 11, 4, 1130),
( 11, 12, 4, 497),
( 12, 13, 4, 203),
( 13, 14, 4, 342),
( 14, 15, 4, 2410),
( 5, 6, 5, 220),
( 6, 7, 5, 354),
( 7, 8, 5, 356),
( 8, 9, 5, 452),
( 9, 10, 5, 543),
( 10, 11, 5, 780),
( 11, 12, 5, 1002),
( 12, 13, 5, 598),
( 13, 14, 5, 198),
( 6, 7, 6, 200),
( 7, 8, 6, 136),
( 8, 9, 6, 98),
( 9, 10, 6, 560),
( 10, 11, 6, 430),
( 11, 12, 6, 270),
( 12, 13, 6, 705),
( 7, 8, 7, 198),
( 8, 9, 7, 165),
( 9, 10, 7, 112),
( 10, 11, 7, 213),
( 11, 12, 7, 76),
( 8, 9, 8, 43),
( 9, 10, 8, 56),
( 10, 11, 8, 52),
( 9, 10, 9, 8);