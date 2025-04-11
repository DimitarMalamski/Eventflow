INSERT INTO Continent (Name) VALUES ('Africa');
INSERT INTO Continent (Name) VALUES ('Antarctica');
INSERT INTO Continent (Name) VALUES ('Asia');
INSERT INTO Continent (Name) VALUES ('Europe');
INSERT INTO Continent (Name) VALUES ('North America');
INSERT INTO Continent (Name) VALUES ('Oceania');
INSERT INTO Continent (Name) VALUES ('Australia')
INSERT INTO Continent (Name) VALUES ('South America');

select * from Continent

ALTER TABLE Country
DROP COLUMN Flag;

ALTER TABLE Country
ADD FlagPath NVARCHAR(255) NOT NULL;

Delete from Continent
Delete from Country

Select * from Country
Select * from Continent