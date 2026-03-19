DROP SCHEMA IF exists F1_Manager;
create schema if not exists F1_Manager;
use F1_Manager;
#tables
create table if not exists Seizoen(
IDSeizoen int not null auto_increment primary key,
NaamSeizoen varchar(64) not null,
BeginDatum date not null,
EindDatum date not null
);
create table if not exists Driver(
IDDriver int not null auto_increment primary key,
VoornaamDriver varchar(64) not null,
AchternaamDriver varchar(64) not null,
NationaliteitDriver varchar(64) not null,
Confidence int not null,
Rating int not null,
LeeftijdDriver int not null
);
create table if not exists Team(
IDTeam int not null auto_increment primary key,
NaamTeam varchar(128) not null,
NationaliteitTeam varchar(64) not null
);
create table if not exists Track(
IDtrack int not null auto_increment primary key,
NaamTrack varchar(64) not null,
LandTrack varchar(64) not null,
LapsTrack int not null
);
create table if not exists Auto(
IDAuto int not null auto_increment primary key,
PresatieAuto int not null,
NaamAuto varchar(64) not null,
FKTeam int not null,
foreign key (FKTeam) references Team (IDTeam)
);
create table if not exists f1_manager.User(
IDUser int auto_increment not null primary key,
nameUser varchar(60) not null unique,
passWordUser varchar(60) not null,
FKTeam int,
foreign key (FKTeam) references Team (IDTeam)
);
create table if not exists RaceWeekend(
IDRaceWeekend int not null auto_increment primary key,
BeginDatum date not null,
EindDatum date not null,
FKSeizoen int not null,
FKTrack int not null,
FKUser int,
foreign key (FKSeizoen) references Seizoen (IDSeizoen),
foreign key (FKTrack) references Track (IDTrack),
foreign key (FKUser) references User (IDUser)
);
#links voor veel op veel
create table if not exists TeamHasDriver(
IDTeamHasDriver int not null auto_increment primary key,
FKTeam int not null,
FKDriver int not null,
BeginDatum date,
EindDatum date not null,
foreign key (FKTeam) references Team (IDTeam),
foreign key (FKDriver) references Driver (IDDriver)
);
create table if not exists RaceWeekendHasDriver(
IDRaceWeekendHasDriver int not null auto_increment primary key,
FKDriver int not null,
FKRaceWeekend int not null,
positie int not null,
punten int not null,
foreign key (FKDriver) references Driver (IDDriver),
foreign key (FKRaceWeekend) references RaceWeekend (IDRaceWeekend)
);
create table if not exists TeamHasSeizoen(
IDTeamHasSeizoen int not null auto_increment primary key,
FKSeizoen int not null,
FKTeam int not null,
foreign key (FKSeizoen) references Seizoen (IDSeizoen),
foreign key (FKTeam) references Team (IDTeam)
);


#tabels
insert into Team (NaamTeam,NationaliteitTeam) values
('MClaren','UK'),('Red Bull','UK'),
('Mercedes','UK'),('Ferari','Italië'),
('Wiliams','UK'),('Aston Martin','UK'),
('Racing Buls','UK'),('Sauber','UK'),
('Haas','USA'),('Alpine','Frankrijk');
#date in YYYY-MM-DD
insert into Seizoen(NaamSeizoen,BeginDatum, EindDatum) values
("Seizoen 2025","2024:12:13","2025:12:12");
insert into Driver (VoornaamDriver,AchternaamDriver,NationaliteitDriver,Rating,LeeftijdDriver,Confidence) values
("Lando","Norris","UK",90,26,80),("Oscar","Piastri","Australie",89,24,80),
("Max","Verstappen","Nederland",95,28,80),("Yuki","Tsunoda","Japan",82,25,80),
("George","Russel","UK",86,27,80),("Kimi","Antonelli","Italie",75,19,80),
("Charles","Leclerc","Monaco",88,28,80),("Lewis","Hamilton","UK",92,40,80),
("Alex","Albon","Tailand",83,29,80),("Carlos","Sianz","Spanje",87,31,80),
("Lance","Stroll","Canada",80,27,80),("Fernando","Alonso","Spanje",89,44,80),
("Isak","Hadjar","Frankrijk",80,21,80),("Liam","Lawson","Amerika",77,22,80),
("Nico","Hulkenberg","Duitland",78,38,80),("Gabriel","Bortoleto","Brazilie",74,21,80),
("Esteban","Ocon","Frankrijk",79,29,80),("Oliver","Bearman","Amerika",77,20,80),
("Piere","Gasly","Frankrijk",82,29,80),("Franco","Colapinto","Argentinië",72,22,80);
insert into  Auto(PresatieAuto,NaamAuto,FKTeam) values
(95,"MC2025",1),(88,"RB2025",2),
(90,"MERC2025",3),(89,"Ferrari2025",4),
(85,"W2025",5),(82,"Aston2025",6),
(75,"RBuls2025",7),(73,"Sauber2025",8),
(81,"H2025",9),(70,"Apine2025",10);
insert into Track(NaamTrack,LandTrack,LapsTrack) values
("Albert Park","Australië",58),("Shanghai International Circuit","China",56),("Suzuka Circuit","Japan",53),("Bahrein International Circuit","Bahrein",57),("Jeddah Cornische Circuit","Saudi-Arabie",50),
("Maimi International Autodrome","Amerika",57),("Autodromo Internazionale Enzo e Dino Ferrari","Italie",63),("Circuit de Monaco","Monaco",78),("Circuit de Barcelona-Catalunya","Spanje",66),("Circuit Gilles-Villeneuve","Canada",70),
("Red Bull Ring","Oostenrijk",71),("Silverstone","UK",52),("Circuit de Spa-Francorchamps","België",44),("Hungaroring","Hongarije",70),("Circuit Zandvoort","Nederland",72),
("Autodromo Nazionale Monza","Italie",53),("Baku City Circuit","Azerbejang",51),("Marina Bay Street Circuit","Singapore",62),("Circuit of The Americas","Amerika",56),("Autódromo Hermanos Rodríguez","Mexico",71),
("Autódromo José Carlos Pace","Brazilië",71),("Las Vegas Strip Circuit","Amerika",50),("Lusail International Circuit","Qatar",57),("Yas Marina Circuit","Abu Dhabi",58),("MadRing","Spanje",56);
insert into RaceWeekend(BeginDatum,EindDatum,FKSeizoen,FKTrack) values 
("2025-03-14","2025-03-16",1,1),("2025-03-21","2025-03-23",1,2),("2025-04-04","2025-04-06",1,3),("2025-04-11","2025-04-13",1,4),("2025-04-18","2025-04-20",1,5),
("2025-05-02","2025-05-04",1,6),("2025-05-16","2025-05-18",1,7),("2025-05-23","2025-05-25",1,8),("2025-05-30","2025-06-01",1,9),("2025-06-13","2025-06-15",1,10),
("2025-06-27","2025-06-29",1,11),("2025-07-04","2025-07-06",1,12),("2025-07-25","2025-07-27",1,13),("2025-08-01","2025-08-03",1,14),("2025-08-29","2025-08-31",1,15),
("2025-09-05","2025-09-07",1,16),("2025-09-19","2025-09-21",1,17),("2025-10-03","2025-10-05",1,18),("2025-10-17","2025-10-19",1,19),("2025-10-24","2025-10-26",1,20),
("2025-11-07","2025-11-09",1,21),("2025-11-20","2025-11-22",1,22),("2025-11-28","2025-11-30",1,23),("2025-12-05","2025-12-07",1,24);

#links
insert into TeamHasDriver(FKTeam,FKDriver,EindDatum) values
(1,1,"2027:12:12"),(1,2,"2030:12:12"),
(2,3,"2028:12:12"),(2,4,"2026:12:12"),
(3,5,"2029:12:12"),(3,6,"2032:12:12"),
(4,7,"2027:12:12"),(4,8,"2026:12:12"),
(5,9,"2027:12:12"),(5,10,"2028:1:12"),
(6,11,"2028:12:12"),(6,12,"2029:12:12"),
(7,13,"2027:12:12"),(7,14,"2026:12:12"),
(8,15,"2029:12:12"),(8,16,"2030:12:12"),
(9,17,"2027:12:12"),(9,18,"2026:12:12"),
(10,19,"2028:12:12"),(10,20,"2029:12:12");
insert into TeamHasSeizoen(FKSeizoen,FKTeam) values
(1,1),(1,2),(1,3),(1,4),(1,5),(1,6),(1,7),(1,8),(1,9),(1,10);
insert into f1_manager.team (NaamTeam,NationaliteitTeam) values
("De Skadi's","Belgie");
insert into f1_manager.User(nameUser, passWordUser,FKTeam) values
('Lea', '1234','11');
insert into f1_manager.User(nameUser, passWordUser) values
('Pieter', '1234');

