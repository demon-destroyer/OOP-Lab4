using System;
using System.Collections.Generic;

namespace Task_3
{
    public record Product(string Name, string Category, decimal Price, int Stock, bool IsAvailable);

    public record ProductSummary(string Title, decimal TotalStockValue);

    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            // 1. Колекція з 8 елементів сутності з полями різних типів
            var products = new List<Product>
            {
                new("Монітор 27\"", "Дисплеї", 9500m, 4, true),
                new("Клавіатура механічна", "Периферія", 2400m, 12, true),
                new("Миша ігрова", "Периферія", 1800m, 0, false),
                new("SSD 1TB NVMe", "Комплектуючі", 3200m, 9, true),
                new("Навушники дротові", "Аудіо", 2100m, 6, true),
                new("Мікрофон USB", "Аудіо", 2900m, 0, false),
                new("Килимок XL", "Периферія", 650m, 25, true),
                new("Блок живлення 750W", "Комплектуючі", 4100m, 3, true)
            };

            // 2. Фільтрація через Predicate<T> 
            Console.WriteLine(" Фільтрація через Predicate<T> ");
            Predicate<Product> expensive = p => p.Price >= 3000m;
            Predicate<Product> isPeripheral = p => p.Category.Equals("Периферія", StringComparison.OrdinalIgnoreCase);
            Predicate<Product> readyBudgetItem = p => p.IsAvailable && p.Price <= 2500m && p.Stock >= 5;

            PrintList("Ціна >= 3000 грн:", products.FindAll(expensive));
            PrintList("Категорія 'Периферія' (текстовий критерій):", products.FindAll(isPeripheral));
            PrintList("В наявності, <= 2500 грн, залишок >= 5 (комбінована умова):", products.FindAll(readyBudgetItem));

            // 3. Перетворення через Func<T, TResult>
            Console.WriteLine("\n Перетворення через Func<T, TResult> ");
            Func<Product, string> toLabel = p => $"{p.Name} [{p.Category}] — {p.Price} грн";
            Func<Product, ProductSummary> toSummary = p => new ProductSummary(p.Name, p.Price * p.Stock);

            List<string> labels = Map(products, toLabel);
            List<ProductSummary> summaries = Map(products, toSummary);

            Console.WriteLine("  Рядкові представлення (перші 3):");
            for (int i = 0; i < 3; i++) Console.WriteLine($"    - {labels[i]}");

            Console.WriteLine("  Обчислена вартість залишків (перші 3):");
            for (int i = 0; i < 3; i++) Console.WriteLine($"    - {summaries[i].Title}: {summaries[i].TotalStockValue} грн");

            // 4. Обхід через Action<T> із побічними ефектами (замикання + умовний вивід)
            Console.WriteLine("\n Обхід через Action<T> (статистика + умовний вивід) ");
            int availableCount = 0;
            decimal totalInventoryValue = 0m;

            Action<Product> collectStats = p =>
            {
                if (!p.IsAvailable)
                {
                    Console.WriteLine($"  [Немає в наявності]: {p.Name}");
                    return;
                }
                availableCount++;
                totalInventoryValue += p.Price * p.Stock;
            };

            products.ForEach(collectStats);
            Console.WriteLine($"  Доступних позицій: {availableCount}, загальна вартість складу: {totalInventoryValue} грн");

            // 5. Універсальний конвеєр  зі списку Func<T, T>
            Console.WriteLine("\n Ланцюжок перетворень Func<T, T> ");
            var pricePipeline = new List<Func<decimal, decimal>>
            {
                price => price * 0.85m,          // 1. Сезонна знижка 15%
                price => price * 1.20m,          // 2. Додавання ПДВ 20%
                price => price + 150m,           // 3. Фіксована доставка
                price => Math.Round(price, 2)    // 4. Округлення до копійок
            };

            decimal basePrice = 3200m;
            decimal finalPrice = ApplyPipeline(basePrice, pricePipeline);
            Console.WriteLine($"  Базова ціна: {basePrice} грн -> після 4 кроків конвеєра: {finalPrice} грн");
        }

        private static List<TResult> Map<T, TResult>(List<T> source, Func<T, TResult> selector)
        {
            var result = new List<TResult>(source.Count);
            foreach (var item in source)
                result.Add(selector(item));
            return result;
        }

        // Послідовно передає результат попереднього кроку на вхід наступному
        private static T ApplyPipeline<T>(T initialValue, List<Func<T, T>> steps)
        {
            T current = initialValue;
            foreach (var step in steps)
                current = step(current);
            return current;
        }

        private static void PrintList(string title, List<Product> items)
        {
            Console.WriteLine($"  {title}");
            foreach (var p in items)
                Console.WriteLine($"    - {p.Name} ({p.Price} грн, залишок: {p.Stock})");
        }
    }
}