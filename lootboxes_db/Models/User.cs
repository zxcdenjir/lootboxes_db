using System;
using System.Collections.Generic;

namespace lootboxes_db.Models;

public partial class User
{
    public int Id { get; set; }

    public string FullName { get; set; } = null!;

    public string Login { get; set; } = null!;

    public string Password { get; set; } = null!;

    public virtual ICollection<Item> Items { get; set; } = new List<Item>();

    public void PrintInventory()
    {
        Console.WriteLine("Ваш инвентарь:");
        if (Items.Count == 0)
        {
            Console.WriteLine("Инвентарь пуст");
        }
        else
        {
            foreach (Item item in Items)
            {
                Console.ForegroundColor = (ConsoleColor)Program.context.Rarities.Find(item.RarityId)!.Color;
                Console.WriteLine(item);
                Console.ResetColor();
            }
        }
    }
}
