using System;
using System.Collections.Generic;
using System.Linq;

namespace Task_4
{
    public record Product(string Name, string Category, decimal Price, int Stock, bool IsAvailable, string[] Tags);

    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            // 1. Розширена колекція з 3 завдання 
            var products = new List<Product>
            {
                new("Монітор 27\"", "Дисплеї", 9500m, 4, true, new[] { "IPS", "165Hz", "Gaming" }),
                new("Клавіатура механічна", "Периферія", 2400m, 12, true, new[] { "RGB", "Gaming" }),
                new("Миша ігрова", "Периферія", 1800m, 0, false, new[] { "Wireless", "Gaming" }),
                new("SSD 1TB NVMe", "Комплектуючі", 3200m, 9, true, new[] { "PCIe4", "Storage" }),
                new("Навушники дротові", "Аудіо", 2100m, 6, true, new[] { "3.5mm", "Studio" }),
                new("Мікрофон USB", "Аудіо", 2900m, 0, false, new[] { "Streaming", "Studio" }),
                new("Килимок XL", "Периферія", 650m, 25, true, new[] { "Gaming", "Fabric" }),
                new("Блок живлення 750W", "Комплектуючі", 4100m, 3, true, new[] { "80+ Gold", "Modular" })
            };

            // 2. Where + Select: вбудовані лямбди vs змінні Func
            Console.WriteLine(" Where та Select ");
            var inlineQuery = products
                .Where(p => p.IsAvailable && p.Price >= 2000m)
                .Select(p => $"{p.Name} ({p.Price} грн)");

            Console.WriteLine("  Вбудовані лямбди: " + string.Join(", ", inlineQuery));

            Func<Product, bool> filterPredicate = p => p.IsAvailable && p.Price >= 2000m;
            Func<Product, string> labelSelector = p => $"{p.Name} ({p.Price} грн)";
            var delegateQuery = products.Where(filterPredicate).Select(labelSelector);

            Console.WriteLine("  Змінні Func:      " + string.Join(", ", delegateQuery));

            // 3. Сортування через лямбди та SelectMany для вкладених тегів
            Console.WriteLine("\n Сортування та SelectMany ");
            var sorted = products
                .OrderBy(p => p.Category)
                .ThenByDescending(p => p.Price);

            Console.WriteLine("  Сортування (Категорія ASC, Ціна DESC):");
            foreach (var p in sorted)
                Console.WriteLine($"    [{p.Category}] {p.Name} — {p.Price} грн");

            var uniqueTags = products
                .SelectMany(p => p.Tags)
                .Distinct()
                .OrderBy(tag => tag);
            Console.WriteLine("  Унікальні теги (SelectMany): " + string.Join(", ", uniqueTags));

            // 4. Агрегація та групування 
            Console.WriteLine("\n Агрегація та групування ");
            int inStockCount = products.Count(p => p.IsAvailable);
            decimal avgPrice = products.Average(p => p.Price);
            decimal totalStockSum = products.Sum(p => p.Price * p.Stock);

            Console.WriteLine($"  Доступних позицій: {inStockCount} із {products.Count}");
            Console.WriteLine($"  Середня ціна: {avgPrice:F2} грн | Загальна вартість складу: {totalStockSum} грн");

            var byCategory = products.GroupBy(p => p.Category);
            Console.WriteLine("  Групування за категорією:");
            foreach (var group in byCategory)
                Console.WriteLine($"    {group.Key}: {group.Count()} шт., макс. ціна = {group.Max(p => p.Price)} грн");

            // 5. Матеріалізація та відкладене виконання при повторному переборі
            Console.WriteLine("\nМатеріалізація (ToList) і відкладене виконання ");
            // Запит ще НЕ виконався — збережено лише план обчислень
            IEnumerable<Product> deferredQuery = products.Where(p => p.Price < 2500m);
            
            List<Product> materializedList = deferredQuery.ToList();

            Console.WriteLine($"  До зміни джерела -> deferred: {deferredQuery.Count()}, materialized: {materializedList.Count}");

            // Додаємо новий дешевий товар після оголошення запиту
            products.Add(new("Кабель HDMI 2m", "Периферія", 350m, 15, true, new[] { "4K" }));

            Console.WriteLine($"  Після додавання  -> deferred: {deferredQuery.Count()}, materialized: {materializedList.Count}");

            // Обробка матеріалізованого списку
            Func<Product, string> formatItem = p => $"    * {p.Name}: {p.Price} грн";
            Action<string> printLine = line => Console.WriteLine(line);

            Console.WriteLine("  Матеріалізований список (обробка поза LINQ):");
            foreach (var item in materializedList)
                printLine(formatItem(item));
        }
    }
}