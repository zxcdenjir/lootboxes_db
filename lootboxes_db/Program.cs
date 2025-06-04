using lootboxes_db.Context;
using lootboxes_db.Models;
using Microsoft.EntityFrameworkCore;

class Program
{
    public static PostgresContext context = new();
    static Random random = new();
    static List<int> AvaliableColors = [8, 9, 10, 11, 12, 13, 14, 15];

    static void Main()
    {
        foreach (Rarity rarity in context.Rarities)
        {
            AvaliableColors.Remove(rarity.Color);
        }

        while (true)
        {
            Console.Clear();
            Console.WriteLine("Добро пожаловать в систему управления лутбоксами!");
            Console.WriteLine("Выберите действие:");
            Console.WriteLine("1. Работа с базой данных");
            Console.WriteLine("2. Вход с систему");
            Console.WriteLine("0. Выход");
            switch (Input("Выберите действие: ", 0, 2))
            {
                case 1:
                    Console.Clear();
                    DBMenu();
                    void DBMenu()
                    {
                        while (true)
                        {
                            Console.WriteLine("1. Работа с лутбоксами");
                            Console.WriteLine("2. Работа с редкостью");
                            Console.WriteLine("3. Работа с категорией");
                            Console.WriteLine("4. Работа с предметами");
                            Console.WriteLine("5. Работа с пользователями");
                            Console.WriteLine("0. Выход");
                            switch (Input("Выберите действие: ", 0, 4))
                            {
                                case 1:
                                    Console.Clear();
                                    LootBoxActions();
                                    break;

                                case 2:
                                    Console.Clear();
                                    RarityActions();
                                    break;

                                case 3:
                                    Console.Clear();

                                    break;

                                case 4:
                                    Console.Clear();

                                    break;
                                case 0:
                                    return;
                            }
                        }
                    }
                    break;
                case 2: // тут сделать выбор для регистрации или входа по логину и паролю (когда выбран пользователь, можно открыть лутбокс или посмотреть инвентарь)
                    Console.Clear();
                    UserActions();
                    break;
                case 0:
                    return;
            }
        }
    }

    static void UserActions()
    {
        while (true)
        {
            Console.WriteLine("1. Войти");
            Console.WriteLine("2. Зарегистрироваться");
            Console.WriteLine("0. В главное меню");
            switch (Input("Выберите действие: ", 0, 2))
            {
                case 1:
                    Console.Clear();
                    //Login();
                    User user = context.Users.Find(Input("Введите ID пользователя: ", 1, context.Users.Count()))!; // переделать на логин и пароль
                    UserMenu();
                    break;

                    void UserMenu()
                    {
                        while (true)
                        {
                            Console.WriteLine("1. Открыть лутбокс");
                            Console.WriteLine("2. Посмотреть инвентарь");
                            Console.WriteLine("0. Выход");

                            switch (Input("Выберите действие: ", 0, 2))
                            {
                                case 1:
                                    Console.Clear();
                                    LootBox lootBoxToOpen = context.LootBoxes.Find(Input("Введите ID лутбокса, который вы хотите открыть: ", 1, context.LootBoxes.Count()))!;

                                    int itemDropCount = random.Next(1, context.LootBoxes.Count() + 1);

                                    for (int i = 0; i < itemDropCount; i++)
                                    {
                                        decimal totalChance = 0.0m;
                                        foreach (Item item in lootBoxToOpen.Items)
                                        {
                                            totalChance += context.Rarities.Find(item.RarityId)!.DropChance;
                                        }

                                        decimal roll = (decimal)random.NextDouble() * totalChance;
                                        Item selectedItem = lootBoxToOpen.Items.ElementAt(0);

                                        for (int j = 0; j < lootBoxToOpen.Items.Count; j++)
                                        {
                                            roll -= context.Rarities.Find(lootBoxToOpen.Items.ElementAt(j).RarityId)!.DropChance;
                                            if (roll <= 0)
                                            {
                                                selectedItem = lootBoxToOpen.Items.ElementAt(j);
                                                break;
                                            }
                                        }

                                        Console.ForegroundColor = (ConsoleColor)context.Rarities.Find(selectedItem.RarityId)!.Color;
                                        Console.WriteLine("Вы получили: " + selectedItem.Name + " - " + selectedItem.Description);
                                        Console.ResetColor();
                                        user.Items.Add(selectedItem);
                                        context.SaveChanges();
                                    }
                                    PressToContinue();
                                    break;
                                case 2:
                                    Console.Clear();
                                    Console.WriteLine($"Инвентарь пользователя {user.FullName} ({user.Login}):");
                                    user.PrintInventory();
                                    PressToContinue();
                                    break;
                                case 0:
                                    Console.Clear();
                                    return;
                            }
                            break;
                        }
                    }

                case 2:
                    Console.Clear();
                    //Register();
                    break;
                case 0:
                    Console.Clear();
                    return;
            }
        }
    }

    static void LootBoxActions()
    {
        while (true)
        {
            PrintLootboxes();
            Console.WriteLine("1. Вывести информацию о лутбоксах");
            Console.WriteLine("2. Добавить предмет в лутбокс");
            Console.WriteLine("3. Создать лутбокс");
            Console.WriteLine("4. Редактировать лутбокс");
            Console.WriteLine("5. Удалить лутбокс");
            Console.WriteLine("0. В главное меню");

            switch (Input("Выберите действие: ", 0, 5))
            {
                case 1:
                    Console.Clear();
                    PrintLootboxes();
                    PressToContinue();
                    break;

                case 2:
                    Console.WriteLine();
                    PrintItems();
                    int itemId = Input("Введите ID предмета для добавления в лутбокс: ", 1, context.Items.Count());
                    int lootBoxId = Input("Введите ID лутбокса, в который вы хотите добавить предмет: ", 1, context.LootBoxes.Count());

                    LootBox choosedLootBox = context.LootBoxes.Find(lootBoxId)!;
                    bool inLootbox = false;
                    foreach (Item i in choosedLootBox.Items)
                    {
                        if (i.Id == itemId)
                        {
                            Console.WriteLine("Этот предмет уже добавлен в лутбокс!");
                            inLootbox = true;
                            break;
                        }
                    }

                    if (!inLootbox)
                    {
                        choosedLootBox.Items.Add(context.Items.Find(itemId)!);
                        choosedLootBox.ItemCount++;
                        Console.WriteLine($"\nПредмет {context.Items.Find(itemId)!.Name} был добавлен в лутбокс {choosedLootBox.Name}");
                        context.SaveChanges();
                    }
                    PressToContinue();
                    break;

                case 3:
                    Console.WriteLine();

                    context.LootBoxes.Add(new LootBox(Input("Введите название лутбокса: "), Input("Введите описание лутбокса: ")));
                    context.SaveChanges();
                    Console.WriteLine("Лутбокс успешно создан");

                    PressToContinue();
                    break;

                case 4:
                    LootBox boxToEdit = context.LootBoxes.Find(Input("Введите ID лутбокса, в который вы хотите изменить: ", 1, context.LootBoxes.Count()))!;
                    Console.Clear();
                    EditMenu();

                    void EditMenu()
                    {
                        while (true)
                        {
                            Console.WriteLine("Выбранный лутбокс для редактирования:");
                            boxToEdit.PrintLootBox();
                            Console.WriteLine();
                            Console.WriteLine("1. Изменить название");
                            Console.WriteLine("2. Изменить описание");
                            Console.WriteLine("0. Выход");

                            switch (Input("Выберите действие: ", 0, 2))
                            {
                                case 1:
                                    boxToEdit.Name = Input("Введите новое название лутбокса: ");
                                    context.SaveChanges();
                                    Console.WriteLine("Название успешно изменено!");
                                    PressToContinue();
                                    break;
                                case 2:
                                    boxToEdit.Description = Input("Введите новое описание лутбокса:\n");
                                    context.SaveChanges();
                                    Console.WriteLine("Описание успешно изменено!");
                                    PressToContinue();
                                    break;
                                case 0:
                                    Console.Clear();
                                    return;

                            }
                        }
                    }
                    break;

                case 5: // тут сделать
                    Console.WriteLine();
                    int lootboxId = Input("Введите id лутбокса для удаления: ", 1, context.LootBoxes.Count());

                    context.LootBoxes.Remove(context.LootBoxes.Find(lootboxId)!);
                    context.SaveChanges();

                    context.Database.ExecuteSqlRaw(@"
                        ALTER SEQUENCE ""Rarities_Id_seq"" RESTART WITH 1;
                        UPDATE ""Rarities"" SET ""Id"" = nextval('""Rarities_Id_seq""');");

                    Console.WriteLine("Редкость успешно удалена");

                    PressToContinue();
                    break;

                case 0:
                    Console.Clear();
                    return;
            }
        }
        static void PrintLootboxes()
        {
            foreach (LootBox lootBox in context.LootBoxes.Include(lb => lb.Items).ThenInclude(i => i.Rarity).Include(lb => lb.Items).ThenInclude(i => i.Category))
            {
                lootBox.PrintLootBox();
                Console.WriteLine();
            }
        }
    }

    static void RarityActions()
    {
        while (true)
        {
            PrintRarities();
            Console.WriteLine();
            Console.WriteLine("1. Вывести список редкостей");
            Console.WriteLine("2. Добавить новую редкость");
            Console.WriteLine("3. Редактировать редкость");
            Console.WriteLine("4. Удалить редкость");
            Console.WriteLine("0. В главное меню");
            switch (Input("Выберите действие: ", 0, 4))
            {
                case 1:
                    Console.Clear();
                    PrintRarities();
                    PressToContinue();
                    break;
                case 2:
                    Console.Clear();

                    if (AvaliableColors.Count == 0)
                    {
                        Console.WriteLine("Максимальное количество редкостей");
                        PressToContinue();
                        continue;
                    }

                    string name = Input("Введите название редкости: ");

                    for (int i = 0; i < AvaliableColors.Count; i++)
                    {
                        ConsoleColor c = (ConsoleColor)AvaliableColors[i];
                        Console.ForegroundColor = c;
                        Console.WriteLine($"{i,2}. " + c.ToString());
                        Console.ResetColor();
                    }

                    int color = AvaliableColors[Input($"Введите цвет редкости (0 - {AvaliableColors.Count - 1}): ", 0, AvaliableColors.Count - 1)];

                    AvaliableColors.Remove(color);

                    decimal dropChance = Input("Введите шанс выпадения редкости (от 0 до 100): ", 0m, 100m) / 100m;

                    context.Rarities.Add(new Rarity(dropChance, color, name));

                    context.SaveChanges();

                    Console.WriteLine("Добавлена новая редкость:");
                    Console.ForegroundColor = (ConsoleColor)color;
                    Console.WriteLine(context.Rarities.OrderByDescending(r => r.Id).FirstOrDefault());
                    Console.ResetColor();
                    PressToContinue();
                    break;
                case 3:
                    Console.WriteLine();
                    Rarity r = context.Rarities.Find(Input("Введите Id редкости для редактирования: ", 2, context.Rarities.Count()))!;
                    Console.Clear();
                    Menu(r);

                    void Menu(Rarity r)
                    {
                        while (true)
                        {
                            Console.ForegroundColor = (ConsoleColor)r.Color;
                            Console.WriteLine(r);
                            Console.ResetColor();
                            Console.WriteLine();
                            Console.WriteLine("1. Изменить шанс выпадения");
                            Console.WriteLine("2. Изменить цвет");
                            Console.WriteLine("3. Изменить название");
                            Console.WriteLine("0. Выход");

                            switch (Input("Выберите действие: ", 0, 3))
                            {
                                case 0:
                                    return;
                                case 1:
                                    Console.WriteLine();
                                    r.DropChance = Input("Введите новый шанс выпадения (0 - 100): ", 0m, 100m) / 100;
                                    Console.WriteLine("Шанс выпадения успешно изменён");
                                    PressToContinue();
                                    break;
                                case 2:
                                    Console.WriteLine();
                                    if (AvaliableColors.Count == 0)
                                    {
                                        Console.WriteLine("Нет доступных цветов");
                                        PressToContinue();
                                        break;
                                    }

                                    for (int i = 0; i < AvaliableColors.Count; i++)
                                    {
                                        ConsoleColor c = (ConsoleColor)AvaliableColors[i];
                                        Console.ForegroundColor = c;
                                        Console.WriteLine($"{i,2}. " + c.ToString());
                                        Console.ResetColor();
                                    }

                                    int rColor = AvaliableColors[Input($"Введите новый цвет редкости (0 - {AvaliableColors.Count - 1}): ", 0, AvaliableColors.Count - 1)];
                                    AvaliableColors.Add(r.Color);
                                    r.Color = rColor;
                                    AvaliableColors.Remove(rColor);

                                    Console.WriteLine("Цвет успешно изменён");
                                    PressToContinue();
                                    break;
                                case 3:
                                    Console.WriteLine();
                                    r.Name = Input("Введите новое название редкости: ");
                                    Console.WriteLine("Шанс выпадения успешно изменён");
                                    PressToContinue();
                                    break;

                            }
                        }
                    }
                    break;
                case 4:
                    Console.WriteLine();
                    int rarityId = Input("Введите id редкости для удаления: ", 2, context.Rarities.Count());
                    bool isRemovable = true;
                    foreach (Item item in context.Items.Include(r => r.Rarity))
                    {
                        if (item.RarityId == rarityId)
                            isRemovable = false;
                    }
                    if (isRemovable)
                    {
                        AvaliableColors.Add(context.Rarities.Find(rarityId)!.Color);
                        context.Rarities.Remove(context.Rarities.Find(rarityId)!);
                        context.SaveChanges();

                        context.Database.ExecuteSqlRaw(@"
                            ALTER SEQUENCE ""Rarities_Id_seq"" RESTART WITH 1;
                            UPDATE ""Rarities"" SET ""Id"" = nextval('""Rarities_Id_seq""');");

                        Console.WriteLine("Редкость успешно удалена");
                    }
                    else
                    {
                        Console.WriteLine("Невозможно удалить редкость, так как существует предмет с такой редкостью");
                    }
                    PressToContinue();
                    break;
                case 0:
                    Console.Clear();
                    return;
            }
        }

        static void PrintRarities()
        {
            Console.WriteLine("Список редкостей:");
            foreach (Rarity rarity in context.Rarities)
            {
                Console.ForegroundColor = (ConsoleColor)rarity.Color;
                Console.WriteLine(rarity);
                Console.ResetColor();
            }
        }
    }

    static void PrintItems()
    {
        foreach (Item item in context.Items.Include(r => r.Rarity))
        {
            Console.ForegroundColor = (ConsoleColor)context.Rarities.Find(item.RarityId)!.Color;
            Console.WriteLine(item);
            Console.ResetColor();
        }
    }

    static T Input<T>(string text, T minValue, T maxValue) where T : struct, IComparable<T>
    {
        while (true)
        {
            Console.Write(text);
            string input = Console.ReadLine()!;

            T value;
            try
            {
                value = (T)Convert.ChangeType(input, typeof(T));
            }
            catch
            {
                Console.WriteLine("Неверный ввод данных!");
                continue;
            }

            if (value.CompareTo(minValue) < 0 || value.CompareTo(maxValue) > 0)
            {
                Console.WriteLine($"Некорректный ввод. Пожалуйста, введите число от {minValue} до {maxValue}");
                continue;
            }

            return value;
        }
    }
    static string Input(string text)
    {
        Console.Write(text);
        return Console.ReadLine()!;
    }

    static void PressToContinue()
    {
        Console.WriteLine("\nДля продолжения нажмите любую клавишу...");
        Console.ReadKey();
        Console.Clear();
    }

    static void ReCalculateItemsId()
    {
        int index = 1;
        foreach (Item item in context.Items)
        {
            item.Id = index;
        }
    }
    static void ReCalculateRaritiesId()
    {
        int index = 1;
        foreach (Rarity rarity in context.Rarities)
        {
            rarity.Id = index;
        }
    }
}