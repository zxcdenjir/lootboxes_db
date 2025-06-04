using System;
using System.Collections.Generic;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace lootboxes_db.Models;

public partial class LootBox
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public int ItemCount { get; set; }

    public virtual ICollection<Item> Items { get; set; } = new List<Item>();

    public LootBox(string name, string description)
    {
        Name = name;
        Description = description;
        ItemCount = 0;
    }

    public void PrintLootBox()
    {
        Console.Write($"Лутбокс: ");
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.Write($"{Id}");
        Console.ResetColor();
        Console.WriteLine($", Название: {Name}, Колличество предметов: {ItemCount}");
        Console.WriteLine($"Описание: {Description}");
        Console.WriteLine("Предметы:");
        foreach (Item item in Items)
        {
            Console.ForegroundColor = (ConsoleColor)Program.context.Rarities.Find(item.RarityId)!.Color;
            Console.WriteLine(item);
            Console.ResetColor();
        }
    }
}
