using lootboxes_db.Context;
using lootboxes_db.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;

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
                            Console.WriteLine("6. Синхронизировать последовательность Id всех таблиц");
                            Console.WriteLine("0. Выход");
                            switch (Input("Выберите действие: ", 0, 6))
                            {
                                case 1:
                                    Console.Clear();
                                    LootBoxActions();
                                    break;
                                    void LootBoxActions()
                                    {
                                        while (true)
                                        {
                                            PrintLootboxes();
                                            Console.WriteLine("1. Вывести информацию о лутбоксах");
                                            Console.WriteLine("2. Создать лутбокс");
                                            Console.WriteLine("3. Добавить предмет в лутбокс");
                                            Console.WriteLine("4. Удалить предмет из лутбокса");
                                            Console.WriteLine("5. Редактировать лутбокс");
                                            Console.WriteLine("6. Удалить лутбокс");
                                            Console.WriteLine("7. Удалить все предметы из лутбокса");
                                            Console.WriteLine("0. В главное меню");

                                            switch (Input("Выберите действие: ", 0, 7))
                                            {
                                                case 1:
                                                    Console.Clear();
                                                    PrintLootboxes();
                                                    PressToContinue();
                                                    break;

                                                case 3:
                                                    Console.WriteLine();
                                                    PrintItems();
                                                    int itemId = Input<int>("Введите ID предмета для добавления в лутбокс: ");
                                                    if (context.Items.Find(itemId) == null)
                                                    {
                                                        Console.WriteLine("Предмет с таким ID не найден!");
                                                        PressToContinue();
                                                        break;
                                                    }
                                                    int lootBoxId = Input<int>("Введите ID лутбокса, в который вы хотите добавить предмет: ");
                                                    if (context.LootBoxes.Find(lootBoxId) == null)
                                                    {
                                                        Console.WriteLine("Лутбокс с таким ID не найден!");
                                                        PressToContinue();
                                                        break;
                                                    }
                                                    LootBox choosedLootBox = context.LootBoxes.Include(i => i.Items).FirstOrDefault(l => l.Id == lootBoxId)!;
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

                                                case 2:
                                                    Console.WriteLine();

                                                    context.LootBoxes.Add(new LootBox(Input("Введите название лутбокса: "), Input("Введите описание лутбокса: ")));
                                                    context.SaveChanges();
                                                    Console.WriteLine("Лутбокс успешно создан");

                                                    PressToContinue();
                                                    break;

                                                case 4:
                                                    Console.WriteLine();
                                                    lootBoxId = Input<int>("Введите ID лутбокса, из которого вы хотите удалить предмет: ");
                                                    LootBox? lootBox = context.LootBoxes.Include(x => x.Items).FirstOrDefault(x => x.Id == lootBoxId);
                                                    if (lootBox == null)
                                                    {
                                                        Console.WriteLine("Лутбокс с таким ID не найден!");
                                                        PressToContinue();
                                                        break;
                                                    }
                                                    if (lootBox.Items.Count == 0)
                                                    {
                                                        Console.WriteLine("Лутбокс пуст!");
                                                        PressToContinue();
                                                        break;
                                                    }
                                                    int idItemToRemove = Input<int>("Введите ID предмета для удаления из лутбокса: ");
                                                    Item? itemToRemove = null;
                                                    foreach (Item item in lootBox.Items)
                                                    {
                                                        if (item.Id == idItemToRemove)
                                                        {
                                                            itemToRemove = item;
                                                            break;
                                                        }
                                                    }
                                                    if (itemToRemove == null)
                                                    {
                                                        Console.WriteLine("Предмет с таким ID не найден в лутбоксе!");
                                                        PressToContinue();
                                                        break;
                                                    }

                                                    lootBox.Items.Remove(itemToRemove);
                                                    lootBox.ItemCount--;
                                                    context.SaveChanges();
                                                    Console.WriteLine($"Предмет {itemToRemove.Name} был удалён из лутбокса {lootBox.Name}");
                                                    PressToContinue();
                                                    break;

                                                case 5:
                                                    int boxToEditId = Input<int>("Введите ID лутбокса, который вы хотите редактировать: ");
                                                    LootBox boxToEdit = context.LootBoxes.Find(boxToEditId);
                                                    if (boxToEdit == null)
                                                    {
                                                        Console.WriteLine("Лутбокс с таким ID не найден!");
                                                        PressToContinue();
                                                        break;
                                                    }
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

                                                case 6:
                                                    Console.WriteLine();
                                                    int lootboxId = Input<int>("Введите id лутбокса для удаления: ");

                                                    if (context.LootBoxes.Find(lootboxId) == null)
                                                    {
                                                        Console.WriteLine("Лутбокс с таким ID не найден!");
                                                        PressToContinue();
                                                        break;
                                                    }

                                                    context.LootBoxes.Remove(context.LootBoxes.Find(lootboxId)!);
                                                    context.SaveChanges();
                                                    context.ChangeTracker.Clear();

                                                    Console.WriteLine("Лутбокс успешно удалён");
                                                    PressToContinue();
                                                    break;

                                                case 7:
                                                    Console.WriteLine();
                                                    lootboxId = Input<int>("Введите id лутбокса для удаления всех предметов: ");
                                                    LootBox? lootBoxToClear = context.LootBoxes.Include(i => i.Items).FirstOrDefault(id => id.Id == lootboxId);
                                                    if (lootBoxToClear == null)
                                                    {
                                                        Console.WriteLine("Лутбокс с таким ID не найден!");
                                                        PressToContinue();
                                                        break;
                                                    }
                                                    lootBoxToClear.Items.Clear();
                                                    lootBoxToClear.ItemCount = 0;
                                                    context.SaveChanges();
                                                    Console.WriteLine("Все предметы из лутбокса успешно удалены");
                                                    PressToContinue();
                                                    break;

                                                case 0:
                                                    Console.Clear();
                                                    return;
                                            }
                                        }
                                    }

                                case 2:
                                    Console.Clear();
                                    RarityActions();

                                    void RarityActions()
                                    {
                                        while (true)
                                        {
                                            PrintRarities();
                                            Console.WriteLine();
                                            Console.WriteLine("1. Вывести список редкостей");
                                            Console.WriteLine("2. Добавить новую редкость");
                                            Console.WriteLine("3. Редактировать редкость");
                                            Console.WriteLine("4. Удалить редкость");
                                            Console.WriteLine("5. Удалить все предметы с редкостью");
                                            Console.WriteLine("6. Вывод всех предметов с редкостью");
                                            Console.WriteLine("0. В главное меню");
                                            switch (Input("Выберите действие: ", 0, 6))
                                            {
                                                case 1:
                                                    Console.Clear();
                                                    PrintRarities();
                                                    PressToContinue();
                                                    break;
                                                case 2:
                                                    Console.WriteLine();

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
                                                    int rId = Input<int>("Введите ID редкости для редактирования: ");
                                                    Rarity? r = context.Rarities.Find(rId);
                                                    if (r == null)
                                                    {
                                                        Console.WriteLine("Редкость с таким ID не найдена!");
                                                        PressToContinue();
                                                        break;
                                                    }
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
                                                                    Console.Clear();
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
                                                    int rarityId = Input<int>("Введите id редкости для удаления: ");
                                                    if (context.Rarities.Find(rarityId) == null)
                                                    {
                                                        Console.WriteLine("Редкость с таким ID не найдена!");
                                                        PressToContinue();
                                                        break;
                                                    }
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

                                                        Console.WriteLine("Редкость успешно удалена");
                                                    }
                                                    else
                                                    {
                                                        Console.WriteLine("Невозможно удалить редкость, так как существует предмет с такой редкостью");
                                                    }
                                                    PressToContinue();
                                                    break;

                                                case 5:
                                                    Console.WriteLine();
                                                    rarityId = Input<int>("Введите id редкости для удаления: ");
                                                    Rarity? rarity = context.Rarities.Include(i => i.Items).ThenInclude(i => i.UserInventories).Include(i => i.Items).ThenInclude(i => i.LootBoxes).FirstOrDefault(r => r.Id == rarityId);
                                                    if (rarity == null)
                                                    {
                                                        Console.WriteLine("Редкость с таким ID не найдена!");
                                                        PressToContinue();
                                                        break;
                                                    }
                                                    int count = 0;
                                                    foreach (Item item in rarity.Items)
                                                    {
                                                        foreach (UserInventory userInventory in item.UserInventories)
                                                        {
                                                            context.UserInventories.Remove(userInventory);
                                                        }
                                                        foreach (LootBox lootBox in item.LootBoxes)
                                                        {
                                                            lootBox.Items.Remove(item);
                                                            lootBox.ItemCount--;
                                                        }
                                                        context.Items.Remove(item);
                                                        count++;
                                                    }
                                                    context.SaveChanges();
                                                    Console.WriteLine($"Было удалено {count} предметов");
                                                    PressToContinue();
                                                    break;

                                                case 6:
                                                    Console.WriteLine();
                                                    rarityId = Input<int>("Введите id редкости: ");
                                                    rarity = context.Rarities.Include(i => i.Items).ThenInclude(c => c.Category).FirstOrDefault(r => r.Id == rarityId);
                                                    if (rarity == null)
                                                    {
                                                        Console.WriteLine("Категория с таким ID не найдена!");
                                                        PressToContinue();
                                                        break;
                                                    }
                                                    if (rarity.Items.Count == 0)
                                                    {
                                                        Console.WriteLine("Предметов с этой редкостью не найдено");
                                                        PressToContinue();
                                                        break;
                                                    }
                                                    foreach (Item item in rarity.Items)
                                                    {
                                                        Console.ForegroundColor = (ConsoleColor)rarity.Color;
                                                        Console.WriteLine(item);
                                                        Console.ResetColor();
                                                    }
                                                    PressToContinue();
                                                    break;


                                                case 0:
                                                    Console.Clear();
                                                    return;
                                            }
                                        }


                                    }
                                    break;

                                case 3:
                                    Console.Clear();
                                    CategoryActions();
                                    break;

                                    void CategoryActions()
                                    {
                                        while (true)
                                        {
                                            PrintCategories();
                                            Console.WriteLine();
                                            Console.WriteLine("1. Добавить категорию");
                                            Console.WriteLine("2. Удалить категорию");
                                            Console.WriteLine("3. Редактировать категорию");
                                            Console.WriteLine("4. Удалить все предметы из категории");
                                            Console.WriteLine("5. Вывести предметы в категории");
                                            Console.WriteLine("0. В главное меню");
                                            switch (Input("Выберите действие: ", 0, 5))
                                            {
                                                case 0:
                                                    Console.Clear();
                                                    return;
                                                case 1:
                                                    Console.WriteLine();
                                                    string name = Input("Введите название категории: ", 3);
                                                    context.Categories.Add(new Category(name));
                                                    context.SaveChanges();
                                                    Console.WriteLine("Категория успешно добавлена");
                                                    PressToContinue();
                                                    break;
                                                case 2:
                                                    Console.WriteLine();
                                                    int categoryId = Input<int>("Введите ID категории для удаления: ");
                                                    Category? categoryToRemove = context.Categories.Find(categoryId);
                                                    if (categoryToRemove == null)
                                                    {
                                                        Console.WriteLine("Категория с таким ID не найдена!");
                                                        PressToContinue();
                                                        break;
                                                    }
                                                    bool isRemovable = true;
                                                    foreach (Item item in context.Items)
                                                    {
                                                        if (item.CategoryId == categoryId)
                                                        {
                                                            isRemovable = false;
                                                            break;
                                                        }
                                                    }
                                                    if (!isRemovable)
                                                    {
                                                        Console.WriteLine("Невозможно удалить категорию, так как существуют предметы с этой категорией");
                                                        PressToContinue();
                                                        break;
                                                    }
                                                    context.Categories.Remove(categoryToRemove);
                                                    context.SaveChanges();
                                                    Console.WriteLine("Категория успешно удалена");
                                                    PressToContinue();
                                                    break;
                                                case 3:
                                                    Console.WriteLine();
                                                    int editCategoryId = Input<int>("Введите ID категории для редактирования: ");
                                                    Category? categoryToEdit = context.Categories.Find(editCategoryId);
                                                    if (categoryToEdit == null)
                                                    {
                                                        Console.WriteLine("Категория с таким ID не найдена!");
                                                        PressToContinue();
                                                        break;
                                                    }
                                                    categoryToEdit.Name = Input("Введите новое название категории: ", 3);
                                                    context.SaveChanges();
                                                    Console.WriteLine("Категория успешно изменена");
                                                    PressToContinue();
                                                    break;
                                                case 4:
                                                    Console.WriteLine();
                                                    categoryId = Input<int>("Введите ID категории: ");
                                                    Category? category = context.Categories.Include(i => i.Items).ThenInclude(i => i.UserInventories).Include(i => i.Items).ThenInclude(i => i.LootBoxes).FirstOrDefault(c => c.Id == categoryId);
                                                    if (category == null)
                                                    {
                                                        Console.WriteLine("Категория с таким ID не найдена!");
                                                        PressToContinue();
                                                        break;
                                                    }
                                                    int count = 0;
                                                    foreach (Item item in category.Items)
                                                    {
                                                        foreach (UserInventory userInventory in item.UserInventories)
                                                        {
                                                            context.UserInventories.Remove(userInventory);
                                                        }
                                                        foreach (LootBox lootBox in item.LootBoxes)
                                                        {
                                                            lootBox.Items.Remove(item);
                                                            lootBox.ItemCount--;
                                                        }
                                                        context.Items.Remove(item);
                                                        count++;
                                                    }
                                                    context.SaveChanges();
                                                    Console.WriteLine($"Было удалено {count} предметов");
                                                    PressToContinue();
                                                    break;
                                                case 5:
                                                    Console.WriteLine();
                                                    categoryId = Input<int>("Введите ID категории: ");
                                                    category = context.Categories.Include(i => i.Items).ThenInclude(i => i.Rarity).FirstOrDefault(c => c.Id == categoryId);
                                                    if (category == null)
                                                    {
                                                        Console.WriteLine("Категория с таким ID не найдена!");
                                                        PressToContinue();
                                                        break;
                                                    }
                                                    if (category.Items.Count == 0)
                                                    {
                                                        Console.WriteLine("В этой категории нет предметов");
                                                    }
                                                    else
                                                    {
                                                        foreach (Item item in category.Items)
                                                        {
                                                            Console.ForegroundColor = (ConsoleColor)context.Rarities.Find(item.RarityId)!.Color;
                                                            Console.WriteLine(item);
                                                            Console.ResetColor();
                                                        }
                                                    }
                                                    PressToContinue();
                                                    break;
                                            }
                                        }
                                    }

                                case 4:
                                    Console.Clear();
                                    ItemActions();
                                    break;
                                    void ItemActions()
                                    {
                                        while (true)
                                        {
                                            PrintItems();
                                            Console.WriteLine();
                                            Console.WriteLine("1. Создать предмет");
                                            Console.WriteLine("2. Редактировать предмет");
                                            Console.WriteLine("3. Удалить предмет");
                                            Console.WriteLine("0. В главное меню");
                                            switch (Input("Выберите действие: ", 0, 3))
                                            {
                                                case 1:
                                                    Console.WriteLine();
                                                    if (!context.Categories.Any())
                                                    {
                                                        Console.WriteLine("Нет доступных категорий, добавьте категорию перед созданием предмета");
                                                        PressToContinue();
                                                        break;
                                                    }
                                                    if (!context.Rarities.Any())
                                                    {
                                                        Console.WriteLine("Нет доступных редкостей, добавьте редкость перед созданием предмета");
                                                        PressToContinue();
                                                        break;
                                                    }
                                                    string itemName = Input("Введите название предмета: ", 3);
                                                    string itemDescription = Input("Введите описание предмета: ", 5);
                                                    Console.WriteLine();
                                                    PrintRarities();
                                                    int rarityId;
                                                    do
                                                    {
                                                        rarityId = Input<int>("Введите ID редкости предмета: ");
                                                        if (!context.Rarities.Any(r => r.Id == rarityId))
                                                        {
                                                            Console.WriteLine("Редкость с таким ID не найдена!");
                                                        }
                                                        else
                                                        {
                                                            break;
                                                        }
                                                    } while (true);
                                                    Console.WriteLine();
                                                    PrintCategories();
                                                    int categoryId;
                                                    do
                                                    {
                                                        categoryId = Input<int>("Введите ID категории предмета: ");
                                                        if (!context.Categories.Any(c => c.Id == categoryId))
                                                        {
                                                            Console.WriteLine("Категория с таким ID не найдена!");
                                                        }
                                                        else
                                                        {
                                                            break;
                                                        }
                                                    } while (true);
                                                    context.Items.Add(new Item(itemName, itemDescription, rarityId, categoryId));
                                                    context.SaveChanges();
                                                    Console.WriteLine("Предмет успешно создан");
                                                    PressToContinue();
                                                    break;
                                                case 2:
                                                    Console.WriteLine();
                                                    int itemId = Input<int>("Введите ID предмета для редактирования: ");
                                                    if (!context.Items.Any(i => i.Id == itemId))
                                                    {
                                                        Console.WriteLine("Предмет с таким ID не найден!");
                                                        PressToContinue();
                                                        break;
                                                    }
                                                    Item itemToEdit = context.Items.Include(r => r.Rarity).Include(c => c.Category).Include(l => l.LootBoxes).Include(u => u.UserInventories).FirstOrDefault(i => i.Id == itemId)!;
                                                    Console.Clear();
                                                    ItemEdit();
                                                    break;
                                                    void ItemEdit()
                                                    {
                                                        while (true)
                                                        {
                                                            Console.WriteLine("Выбранный предмет:");
                                                            Console.ForegroundColor = (ConsoleColor)itemToEdit.Rarity.Color;
                                                            Console.WriteLine(itemToEdit);
                                                            Console.ResetColor();
                                                            Console.WriteLine();
                                                            Console.WriteLine("1. Изменить название");
                                                            Console.WriteLine("2. Изменить описание");
                                                            Console.WriteLine("3. Изменить редкость");
                                                            Console.WriteLine("4. Изменить категорию");
                                                            Console.WriteLine("0. Выход");
                                                            switch (Input("Выберите действие: ", 0, 4))
                                                            {
                                                                case 1:
                                                                    Console.WriteLine();
                                                                    itemToEdit.Name = Input("Введите новое название предмета: ", 3);
                                                                    context.SaveChanges();
                                                                    Console.WriteLine("Название успешно изменено");
                                                                    PressToContinue();
                                                                    break;
                                                                case 2:
                                                                    Console.WriteLine();
                                                                    itemToEdit.Description = Input("Введите новое описание предмета: ", 5);
                                                                    context.SaveChanges();
                                                                    Console.WriteLine("Описание успешно изменено");
                                                                    PressToContinue();
                                                                    break;
                                                                case 3:
                                                                    Console.WriteLine();
                                                                    PrintRarities();
                                                                    int newRarityId;
                                                                    do
                                                                    {
                                                                        newRarityId = Input<int>("Введите ID новой редкости: ");
                                                                        if (!context.Rarities.Any(r => r.Id == newRarityId))
                                                                        {
                                                                            Console.WriteLine("Редкость с таким ID не найдена!");
                                                                        }
                                                                        else
                                                                        {
                                                                            itemToEdit.RarityId = newRarityId;
                                                                            context.SaveChanges();
                                                                            Console.WriteLine("Редкость успешно изменена");
                                                                            PressToContinue();
                                                                            break;
                                                                        }
                                                                    } while (true);
                                                                    break;
                                                                case 4:
                                                                    Console.WriteLine();
                                                                    PrintCategories();
                                                                    int newCategoryId;
                                                                    do
                                                                    {
                                                                        newCategoryId = Input<int>("Введите ID новой категории: ");
                                                                        if (!context.Categories.Any(c => c.Id == newCategoryId))
                                                                        {
                                                                            Console.WriteLine("Категория с таким ID не найдена!");
                                                                        }
                                                                        else
                                                                        {
                                                                            itemToEdit.CategoryId = newCategoryId;
                                                                            context.SaveChanges();
                                                                            Console.WriteLine("Категория успешно изменена");
                                                                            PressToContinue();
                                                                            break;
                                                                        }
                                                                    } while (true);
                                                                    break;
                                                                case 0:
                                                                    Console.Clear();
                                                                    return;
                                                            }
                                                        }
                                                    }
                                                case 3:
                                                    Console.WriteLine();
                                                    int itemIdToRemove = Input<int>("Введите ID предмета для удаления: ");
                                                    Item? itemToRemove = context.Items.Include(r => r.Rarity).Include(c => c.Category).Include(l => l.LootBoxes).Include(u => u.UserInventories).FirstOrDefault(i => i.Id == itemIdToRemove);
                                                    if (itemToRemove == null)
                                                    {
                                                        Console.WriteLine("Предмет с таким ID не найден!");
                                                        PressToContinue();
                                                        break;
                                                    }
                                                    foreach (UserInventory userInventory in itemToRemove.UserInventories)
                                                    {
                                                        context.UserInventories.Remove(userInventory);
                                                    }
                                                    foreach (LootBox lootBox in itemToRemove.LootBoxes)
                                                    {
                                                        lootBox.Items.Remove(itemToRemove);
                                                        lootBox.ItemCount--;
                                                    }
                                                    context.Items.Remove(itemToRemove);
                                                    context.SaveChanges();
                                                    Console.WriteLine("Предмет успешно удалён");
                                                    PressToContinue();
                                                    break;

                                                case 0:
                                                    Console.Clear();
                                                    return;
                                            }
                                        }
                                    }

                                case 5:
                                    Console.Clear();
                                    UserdbActions();
                                    break;
                                    void UserdbActions()
                                    {
                                        while (true)
                                        {
                                            foreach (User user in context.Users.Include(u => u.UserInventories).ThenInclude(ui => ui.Item).ThenInclude(c => c.Category).Include(u => u.UserInventories).ThenInclude(ui => ui.Item).ThenInclude(r => r.Rarity))
                                            {
                                                Console.WriteLine(user);
                                            }
                                            Console.WriteLine();
                                            Console.WriteLine("1. Добавить пользователя");
                                            Console.WriteLine("2. Удалить пользователя");
                                            Console.WriteLine("3. Редактировать пользователя");
                                            Console.WriteLine("4. Узнать пароль пользователя");
                                            Console.WriteLine("5. Вывести инвентарь пользователя");
                                            Console.WriteLine("0. В главное меню");

                                            switch (Input("Выберите действие: ", 0, 7))
                                            {
                                                case 0:
                                                    Console.Clear();
                                                    return;

                                                case 1:
                                                    Console.WriteLine();
                                                    string name = Input("Введите полное имя: ", 3);
                                                    string loginReg;
                                                    do
                                                    {
                                                        loginReg = Input("Введите логин: ", 3);
                                                        if (context.Users.Any(u => u.Login == loginReg))
                                                            Console.WriteLine("Этот логин уже занят, попробуйте другой.");
                                                        else
                                                            break;
                                                    } while (true);
                                                    string password = Input("Введите пароль: ", 8);
                                                    context.Users.Add(new User(name, loginReg, password));
                                                    context.SaveChanges();
                                                    Console.WriteLine("Пользователь успешно добавлен");
                                                    PressToContinue();
                                                    break;

                                                case 2:
                                                    Console.WriteLine();
                                                    int userId = Input<int>("Введите ID пользователя для удаления: ");
                                                    User? userToRemove = context.Users.Include(u => u.UserInventories).ThenInclude(ui => ui.Item).ThenInclude(c => c.Category).Include(u => u.UserInventories).ThenInclude(ui => ui.Item).ThenInclude(r => r.Rarity).FirstOrDefault(u => u.Id == userId);
                                                    if (userToRemove == null)
                                                    {
                                                        Console.WriteLine("Пользователь с таким ID не найден!");
                                                        PressToContinue();
                                                        break;
                                                    }
                                                    foreach (UserInventory userInventory in userToRemove.UserInventories)
                                                    {
                                                        context.UserInventories.Remove(userInventory);
                                                    }
                                                    context.Users.Remove(userToRemove);
                                                    context.SaveChanges();
                                                    Console.WriteLine("Пользователь успешно удалён");
                                                    PressToContinue();
                                                    break;

                                                case 3:
                                                    Console.WriteLine();
                                                    int editUserId = Input<int>("Введите ID пользователя для редактирования: ");
                                                    User? userToEdit = context.Users.Include(u => u.UserInventories).ThenInclude(ui => ui.Item).ThenInclude(c => c.Category).Include(u => u.UserInventories).ThenInclude(ui => ui.Item).ThenInclude(r => r.Rarity).FirstOrDefault(u => u.Id == editUserId);
                                                    if (userToEdit == null)
                                                    {
                                                        Console.WriteLine("Пользователь с таким ID не найден!");
                                                        PressToContinue();
                                                        break;
                                                    }
                                                    Console.Clear();
                                                    UserEdit(userToEdit);
                                                    break;
                                                    void UserEdit(User user)
                                                    {
                                                        while (true)
                                                        {
                                                            Console.WriteLine("Выбранный пользователь:");
                                                            Console.WriteLine(user + $", Пароль: {user.Password}");
                                                            Console.WriteLine();
                                                            Console.WriteLine("1. Изменить полное имя");
                                                            Console.WriteLine("2. Изменить логин");
                                                            Console.WriteLine("3. Изменить пароль");
                                                            Console.WriteLine("4. Добавить предмет в инвентарь");
                                                            Console.WriteLine("5. Удалить предмет из инвентаря");
                                                            Console.WriteLine("6. Вывести инвентарь");
                                                            Console.WriteLine("0. Выход");
                                                            switch (Input("Выберите действие: ", 0, 6))
                                                            {
                                                                case 1:
                                                                    Console.WriteLine();
                                                                    user.FullName = Input("Введите новое полное имя: ", 3);
                                                                    context.SaveChanges();
                                                                    Console.WriteLine("Полное имя успешно изменено");
                                                                    PressToContinue();
                                                                    break;
                                                                case 2:
                                                                    Console.WriteLine();
                                                                    string newLogin;
                                                                    do
                                                                    {
                                                                        newLogin = Input("Введите новый логин: ", 3);
                                                                        if (context.Users.Any(u => u.Login == newLogin && u.Id != user.Id))
                                                                            Console.WriteLine("Этот логин уже занят, попробуйте другой.");
                                                                        else
                                                                            break;
                                                                    } while (true);
                                                                    user.Login = newLogin;
                                                                    context.SaveChanges();
                                                                    Console.WriteLine("Логин успешно изменён");
                                                                    PressToContinue();
                                                                    break;
                                                                case 3:
                                                                    Console.WriteLine();
                                                                    user.Password = Input("Введите новый пароль: ", 8);
                                                                    context.SaveChanges();
                                                                    Console.WriteLine("Пароль успешно изменён");
                                                                    PressToContinue();
                                                                    break;
                                                                case 4:
                                                                    Console.WriteLine();
                                                                    if (!context.Items.Any())
                                                                    {
                                                                        Console.WriteLine("Нет доступных предметов, добавьте предмет перед добавлением в инвентарь");
                                                                        PressToContinue();
                                                                        break;
                                                                    }
                                                                    PrintItems();
                                                                    int itemIdToAdd = Input<int>("Введите ID предмета для добавления в инвентарь: ");
                                                                    Item? itemToAdd = context.Items.Include(r => r.Rarity).Include(c => c.Category).FirstOrDefault(i => i.Id == itemIdToAdd);
                                                                    if (itemToAdd == null)
                                                                    {
                                                                        Console.WriteLine("Предмет с таким ID не найден!");
                                                                        PressToContinue();
                                                                        break;
                                                                    }
                                                                    context.UserInventories.Add(new UserInventory { UserId = user.Id, ItemId = itemToAdd.Id });
                                                                    context.SaveChanges();
                                                                    Console.WriteLine("Предмет успешно добавлен в инвентарь пользователя");
                                                                    PressToContinue();
                                                                    break;
                                                                case 5:
                                                                    Console.WriteLine();
                                                                    if (user.UserInventories.Count == 0)
                                                                    {
                                                                        Console.WriteLine("Инвентарь пользователя пуст");
                                                                        PressToContinue();
                                                                        break;
                                                                    }
                                                                    user.PrintInventory();
                                                                    int itemIdToRemove = Input<int>("Введите ID предмета для удаления из инвентаря: ");
                                                                    UserInventory? userInventoryToRemove = user.UserInventories.FirstOrDefault(ui => ui.ItemId == itemIdToRemove);
                                                                    if (userInventoryToRemove == null)
                                                                    {
                                                                        Console.WriteLine("Предмет с таким ID не найден в инвентаре пользователя!");
                                                                        PressToContinue();
                                                                        break;
                                                                    }
                                                                    context.UserInventories.Remove(userInventoryToRemove);
                                                                    context.SaveChanges();
                                                                    Console.WriteLine("Предмет успешно удалён из инвентаря пользователя");
                                                                    PressToContinue();
                                                                    break;
                                                                case 6:
                                                                    Console.Clear();
                                                                    Console.WriteLine($"Инвентарь пользователя {user.Login}:");
                                                                    user.PrintInventory();
                                                                    PressToContinue();
                                                                    break;
                                                                case 0:
                                                                    Console.Clear();
                                                                    return;
                                                            }
                                                        }
                                                    }
                                                case 4:
                                                    Console.WriteLine();
                                                    int userIdToFind = Input<int>("Введите ID пользователя для поиска пароля: ");
                                                    User? userToFind = context.Users.FirstOrDefault(u => u.Id == userIdToFind);
                                                    if (userToFind == null)
                                                    {
                                                        Console.WriteLine("Пользователь с таким ID не найден!");
                                                        PressToContinue();
                                                        break;
                                                    }
                                                    Console.WriteLine($"Пароль пользователя {userToFind.Login}: {userToFind.Password}");
                                                    PressToContinue();
                                                    break;
                                                case 5:
                                                    Console.WriteLine();
                                                    int userIdToShowInventory = Input<int>("Введите ID пользователя для просмотра инвентаря: ");
                                                    User? userToShowInventory = context.Users.Include(u => u.UserInventories).ThenInclude(ui => ui.Item).ThenInclude(c => c.Category).Include(u => u.UserInventories).ThenInclude(ui => ui.Item).ThenInclude(r => r.Rarity).FirstOrDefault(u => u.Id == userIdToShowInventory);
                                                    if (userToShowInventory == null)
                                                    {
                                                        Console.WriteLine("Пользователь с таким ID не найден!");
                                                        PressToContinue();
                                                        break;
                                                    }
                                                    Console.Clear();
                                                    Console.WriteLine($"Инвентарь пользователя {userToShowInventory.FullName}:");
                                                    userToShowInventory.PrintInventory();
                                                    PressToContinue();
                                                    break;


                                            }
                                        }
                                    }

                                case 6:
                                    context.Database.ExecuteSqlRaw(@"
                                        SELECT setval('public.""LootBoxes_Id_seq""', COALESCE((SELECT MAX(""Id"") FROM ""LootBoxes""), 0) + 1, false);
                                        SELECT setval('public.""Items_Id_seq""', COALESCE((SELECT MAX(""Id"") FROM ""Items""), 0) + 1, false);
                                        SELECT setval('public.""Categories_Id_seq""', COALESCE((SELECT MAX(""Id"") FROM ""Categories""), 0) + 1, false);
                                        SELECT setval('public.""Rarities_Id_seq""', COALESCE((SELECT MAX(""Id"") FROM ""Rarities""), 0) + 1, false);
                                        SELECT setval('public.""Users_Id_seq""', COALESCE((SELECT MAX(""Id"") FROM ""Users""), 0) + 1, false);
                                        SELECT setval('public.""UserInventory_UserInventoryId_seq""', COALESCE((SELECT MAX(""UserInventoryId"") FROM ""UserInventory""), 0) + 1, false);
                                    ");
                                    Console.WriteLine("Последовательность успешно синхронизирована");
                                    PressToContinue();
                                    break;

                                case 0:
                                    return;
                            }
                        }
                    }
                    break;
                case 2:
                    Console.Clear();
                    UserActions();
                    break;
                    void UserActions()
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
                                    string login = Input("Логин: ");
                                    string password = Input("Пароль: ");
                                    User? user = context.Users.Include(u => u.UserInventories).ThenInclude(ui => ui.Item).ThenInclude(c => c.Category).Include(u => u.UserInventories).ThenInclude(ui => ui.Item).ThenInclude(r => r.Rarity).Where(u => u.Login == login && u.Password == password).FirstOrDefault();

                                    if (user == null)
                                    {
                                        Console.WriteLine("Неверный логин или пароль!");
                                        PressToContinue();
                                        break;
                                    }
                                    UserMenu(user);
                                    break;

                                    void UserMenu(User user)
                                    {
                                        Console.Clear();
                                        while (true)
                                        {
                                            Console.WriteLine($"Здравствуйте, {user.FullName}");
                                            Console.WriteLine();
                                            PrintLootboxes();
                                            Console.WriteLine("1. Открыть лутбокс");
                                            Console.WriteLine("2. Посмотреть инвентарь");
                                            Console.WriteLine("0. Выход");

                                            switch (Input("Выберите действие: ", 0, 2))
                                            {
                                                case 0:
                                                    Console.Clear();
                                                    return;

                                                case 1:
                                                    Console.WriteLine();
                                                    int idLootBoxToOpen = Input<int>("Введите ID лутбокса, который вы хотите открыть: ");
                                                    LootBox lootBoxToOpen = context.LootBoxes.Include(i => i.Items).ThenInclude(r => r.Rarity).Include(i => i.Items).ThenInclude(c => c.Category).FirstOrDefault(i => i.Id == idLootBoxToOpen)!;
                                                    if (lootBoxToOpen == null)
                                                    {
                                                        Console.WriteLine("Лутбокс не найден");
                                                        PressToContinue();
                                                        break;
                                                    }
                                                    if (lootBoxToOpen.ItemCount == 0)
                                                    {
                                                        Console.WriteLine("Лутбокс пуст");
                                                        PressToContinue();
                                                        break;
                                                    }

                                                    int itemDropCount = random.Next(1, lootBoxToOpen.ItemCount + 1);

                                                    for (int i = 0; i < itemDropCount; i++)
                                                    {
                                                        decimal totalChance = 0.0m;
                                                        foreach (Item item in lootBoxToOpen.Items)
                                                        {
                                                            totalChance += item.Rarity.DropChance;
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
                                                        context.UserInventories.Add(new UserInventory { UserId = user.Id, ItemId = selectedItem.Id });
                                                        context.SaveChanges();
                                                    }
                                                    PressToContinue();
                                                    break;
                                                case 2:
                                                    Console.Clear();
                                                    Console.WriteLine($"Ваш инвентарь:");
                                                    user.PrintInventory();
                                                    PressToContinue();
                                                    break;

                                            }
                                        }
                                    }

                                case 2:
                                    Console.Clear();
                                    Console.WriteLine("Регистрация нового пользователя");

                                    string name = Input("Введите полное имя: ", 3);

                                    string loginReg;
                                    do
                                    {
                                        loginReg = Input("Введите логин: ", 3);
                                        if (context.Users.Any(u => u.Login == loginReg))
                                            Console.WriteLine("Этот логин уже занят, попробуйте другой.");
                                        else
                                            break;
                                    } while (true);

                                    context.Users.Add(new User(name, loginReg, Input("Введите пароль: ", 8)));
                                    context.SaveChanges();

                                    Console.WriteLine("Успешная регистрация");
                                    PressToContinue();
                                    break;


                                case 0:
                                    Console.Clear();
                                    return;
                            }
                        }
                    }
                case 0:
                    return;
            }
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

    static void PrintItems()
    {
        foreach (Item item in context.Items.Include(r => r.Rarity).Include(c => c.Category))
        {
            Console.ForegroundColor = (ConsoleColor)context.Rarities.Find(item.RarityId)!.Color;
            Console.WriteLine(item);
            Console.ResetColor();
        }
    }
    static void PrintRarities()
    {
        foreach (Rarity rarity in context.Rarities)
        {
            Console.ForegroundColor = (ConsoleColor)rarity.Color;
            Console.WriteLine(rarity);
            Console.ResetColor();
        }
    }
    static void PrintCategories()
    {
        foreach (Category category in context.Categories)
        {
            Console.WriteLine(category);
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
    static T Input<T>(string text) where T : struct, IComparable<T>
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

            return value;
        }
    }
    static string Input(string text)
    {
        Console.Write(text);
        return Console.ReadLine()!;
    }
    static string Input(string text, int minLength)
    {
        while (true)
        {
            Console.Write(text);
            string input = Console.ReadLine()!;
            if (input.Length < minLength)
            {
                Console.WriteLine($"Минимальная длина ввода: {minLength} символов!");
                continue;
            }
            return input;
        }
    }

    static void PressToContinue()
    {
        Console.WriteLine("\nДля продолжения нажмите любую клавишу...");
        Console.ReadKey();
        Console.Clear();
    }
}