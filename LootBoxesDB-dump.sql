CREATE TABLE "UserInventory" (
	"UserInventoryId" INTEGER NOT NULL UNIQUE GENERATED ALWAYS AS IDENTITY,
	"UserId" INTEGER NOT NULL,
	"ItemId" INTEGER NOT NULL,
	PRIMARY KEY("UserInventoryId")
);

CREATE TABLE "Items" (
	"Id" INTEGER NOT NULL UNIQUE GENERATED ALWAYS AS IDENTITY,
	"Name" TEXT NOT NULL,
	"Description" TEXT NOT NULL,
	"RarityId" INTEGER NOT NULL,
	"CategoryId" INTEGER NOT NULL,
	PRIMARY KEY("Id")
);

CREATE TABLE "LootBoxes" (
	"Id" INTEGER NOT NULL UNIQUE GENERATED ALWAYS AS IDENTITY,
	"Name" TEXT NOT NULL,
	"Description" TEXT NOT NULL,
	"ItemCount" INTEGER NOT NULL,
	PRIMARY KEY("Id")
);

CREATE TABLE "Categories" (
	"Id" INTEGER NOT NULL UNIQUE GENERATED ALWAYS AS IDENTITY,
	"Name" TEXT NOT NULL,
	PRIMARY KEY("Id")
);

CREATE TABLE "Rarities" (
	"Id" INTEGER NOT NULL UNIQUE GENERATED ALWAYS AS IDENTITY,
	"DropChance" DECIMAL NOT NULL,
	"Color" INTEGER NOT NULL,
	"Name" TEXT NOT NULL,
	PRIMARY KEY("Id")
);

CREATE TABLE "LootBoxItems" (
	"ItemId" INTEGER NOT NULL,
	"LootBoxId" INTEGER NOT NULL,
	PRIMARY KEY("ItemId", "LootBoxId")
);

CREATE TABLE "Users" (
	"Id" INTEGER NOT NULL UNIQUE GENERATED ALWAYS AS IDENTITY,
	"FullName" TEXT NOT NULL,
	"Login" TEXT NOT NULL UNIQUE,
	"Password" TEXT NOT NULL,
	PRIMARY KEY("Id")
);

ALTER TABLE "Items"
ADD FOREIGN KEY("CategoryId") REFERENCES "Categories"("Id")
ON UPDATE NO ACTION ON DELETE NO ACTION;

ALTER TABLE "Items"
ADD FOREIGN KEY("RarityId") REFERENCES "Rarities"("Id")
ON UPDATE NO ACTION ON DELETE NO ACTION;

ALTER TABLE "LootBoxItems"
ADD FOREIGN KEY("LootBoxId") REFERENCES "LootBoxes"("Id")
ON UPDATE NO ACTION ON DELETE NO ACTION;

ALTER TABLE "LootBoxItems"
ADD FOREIGN KEY("ItemId") REFERENCES "Items"("Id")
ON UPDATE NO ACTION ON DELETE NO ACTION;

ALTER TABLE "UserInventory"
ADD FOREIGN KEY("UserId") REFERENCES "Users"("Id")
ON UPDATE NO ACTION ON DELETE NO ACTION;

ALTER TABLE "UserInventory"
ADD FOREIGN KEY("ItemId") REFERENCES "Items"("Id")
ON UPDATE NO ACTION ON DELETE NO ACTION;

INSERT INTO public."Rarities" OVERRIDING SYSTEM VALUE VALUES (1, 0.50, 15, 'Обычный');
INSERT INTO public."Rarities" OVERRIDING SYSTEM VALUE VALUES (2, 0.25, 9, 'Редкий');
INSERT INTO public."Rarities" OVERRIDING SYSTEM VALUE VALUES (3, 0.10, 13, 'Эпический');
INSERT INTO public."Rarities" OVERRIDING SYSTEM VALUE VALUES (4, 0.05, 14, 'Легендарный');
INSERT INTO public."Rarities" OVERRIDING SYSTEM VALUE VALUES (5, 0.01, 12, 'Мифический');

INSERT INTO public."Categories" OVERRIDING SYSTEM VALUE VALUES (1, 'Оружие');
INSERT INTO public."Categories" OVERRIDING SYSTEM VALUE VALUES (2, 'Броня');
INSERT INTO public."Categories" OVERRIDING SYSTEM VALUE VALUES (3, 'Зелья');
INSERT INTO public."Categories" OVERRIDING SYSTEM VALUE VALUES (4, 'Аксессуары');
INSERT INTO public."Categories" OVERRIDING SYSTEM VALUE VALUES (5, 'Магические');
INSERT INTO public."Categories" OVERRIDING SYSTEM VALUE VALUES (6, 'Расходники');

INSERT INTO public."Items" OVERRIDING SYSTEM VALUE VALUES (1, 'Кинжал', 'Быстрый и лёгкий', 1, 1);
INSERT INTO public."Items" OVERRIDING SYSTEM VALUE VALUES (2, 'Меч', 'Острый клинок', 1, 1);
INSERT INTO public."Items" OVERRIDING SYSTEM VALUE VALUES (3, 'Щит', 'Защищает вас', 1, 2);
INSERT INTO public."Items" OVERRIDING SYSTEM VALUE VALUES (4, 'Зелье здоровья', 'Восстанавливает здоровье', 1, 3);
INSERT INTO public."Items" OVERRIDING SYSTEM VALUE VALUES (5, 'Зелье маны', 'Восполняет ману', 1, 3);
INSERT INTO public."Items" OVERRIDING SYSTEM VALUE VALUES (6, 'Лук', 'Стреляет стрелами', 2, 1);
INSERT INTO public."Items" OVERRIDING SYSTEM VALUE VALUES (7, 'Булава', 'Сокрушительная сила', 2, 1);
INSERT INTO public."Items" OVERRIDING SYSTEM VALUE VALUES (8, 'Тяжёлая броня', 'Защищает от ударов', 2, 2);
INSERT INTO public."Items" OVERRIDING SYSTEM VALUE VALUES (9, 'Щит драконьей чешуи', 'Почти не пробиваемый', 3, 2);
INSERT INTO public."Items" OVERRIDING SYSTEM VALUE VALUES (10, 'Кольцо силы', 'Увеличивает силу владельца', 3, 4);
INSERT INTO public."Items" OVERRIDING SYSTEM VALUE VALUES (11, 'Амулет мудрости', 'Дарует мудрость', 3, 4);
INSERT INTO public."Items" OVERRIDING SYSTEM VALUE VALUES (12, 'Посох стихий', 'Магический посох', 3, 5);
INSERT INTO public."Items" OVERRIDING SYSTEM VALUE VALUES (13, 'Эликсир бессмертия', 'Секрет древних', 4, 3);
INSERT INTO public."Items" OVERRIDING SYSTEM VALUE VALUES (14, 'Мантия теней', 'Невидимость в бою', 4, 5);
INSERT INTO public."Items" OVERRIDING SYSTEM VALUE VALUES (15, 'Сыр', 'Мгновенно восстанавливет жизненные силы того, кто его попробует', 5, 6);

INSERT INTO public."LootBoxes" OVERRIDING SYSTEM VALUE VALUES (2, 'Продвинутый ящик', 'Содержит улучшенные предметы', 5);
INSERT INTO public."LootBoxes" OVERRIDING SYSTEM VALUE VALUES (1, 'Базовый ящик', 'Содержит преимущественно обычные предметы', 4);
INSERT INTO public."LootBoxes" OVERRIDING SYSTEM VALUE VALUES (3, 'Легендарный ящик', 'Содержит элитные предметы', 7);

INSERT INTO public."LootBoxItems" VALUES (1, 1);
INSERT INTO public."LootBoxItems" VALUES (1, 2);
INSERT INTO public."LootBoxItems" VALUES (2, 1);
INSERT INTO public."LootBoxItems" VALUES (2, 2);
INSERT INTO public."LootBoxItems" VALUES (3, 1);
INSERT INTO public."LootBoxItems" VALUES (5, 3);
INSERT INTO public."LootBoxItems" VALUES (6, 3);
INSERT INTO public."LootBoxItems" VALUES (7, 3);
INSERT INTO public."LootBoxItems" VALUES (8, 2);
INSERT INTO public."LootBoxItems" VALUES (9, 3);
INSERT INTO public."LootBoxItems" VALUES (10, 2);
INSERT INTO public."LootBoxItems" VALUES (11, 2);
INSERT INTO public."LootBoxItems" VALUES (12, 3);
INSERT INTO public."LootBoxItems" VALUES (13, 3);
INSERT INTO public."LootBoxItems" VALUES (6, 1);
INSERT INTO public."LootBoxItems" VALUES (14, 3);

INSERT INTO public."Users" OVERRIDING SYSTEM VALUE VALUES (1, 'Администратор', 'Admin', 'admin123');

INSERT INTO public."UserInventory" OVERRIDING SYSTEM VALUE VALUES (1, 1, 1);
INSERT INTO public."UserInventory" OVERRIDING SYSTEM VALUE VALUES (2, 1, 2);
INSERT INTO public."UserInventory" OVERRIDING SYSTEM VALUE VALUES (3, 1, 3);
INSERT INTO public."UserInventory" OVERRIDING SYSTEM VALUE VALUES (4, 1, 4);
INSERT INTO public."UserInventory" OVERRIDING SYSTEM VALUE VALUES (5, 1, 5);
INSERT INTO public."UserInventory" OVERRIDING SYSTEM VALUE VALUES (6, 1, 6);
INSERT INTO public."UserInventory" OVERRIDING SYSTEM VALUE VALUES (7, 1, 7);
INSERT INTO public."UserInventory" OVERRIDING SYSTEM VALUE VALUES (8, 1, 8);
INSERT INTO public."UserInventory" OVERRIDING SYSTEM VALUE VALUES (9, 1, 9);
INSERT INTO public."UserInventory" OVERRIDING SYSTEM VALUE VALUES (10, 1, 10);
INSERT INTO public."UserInventory" OVERRIDING SYSTEM VALUE VALUES (11, 1, 11);
INSERT INTO public."UserInventory" OVERRIDING SYSTEM VALUE VALUES (12, 1, 12);
INSERT INTO public."UserInventory" OVERRIDING SYSTEM VALUE VALUES (13, 1, 13);
INSERT INTO public."UserInventory" OVERRIDING SYSTEM VALUE VALUES (14, 1, 14);
INSERT INTO public."UserInventory" OVERRIDING SYSTEM VALUE VALUES (15, 1, 15);

SELECT setval('public."LootBoxes_Id_seq"', COALESCE((SELECT MAX("Id") FROM "LootBoxes"), 0) + 1, false);
SELECT setval('public."Items_Id_seq"', COALESCE((SELECT MAX("Id") FROM "Items"), 0) + 1, false);
SELECT setval('public."Categories_Id_seq"', COALESCE((SELECT MAX("Id") FROM "Categories"), 0) + 1, false);
SELECT setval('public."Rarities_Id_seq"', COALESCE((SELECT MAX("Id") FROM "Rarities"), 0) + 1, false);
SELECT setval('public."Users_Id_seq"', COALESCE((SELECT MAX("Id") FROM "Users"), 0) + 1, false);
SELECT setval('public."UserInventory_UserInventoryId_seq"', COALESCE((SELECT MAX("UserInventoryId") FROM "UserInventory"), 0) + 1, false);