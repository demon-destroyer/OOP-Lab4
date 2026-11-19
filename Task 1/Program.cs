using System;

namespace Task_1
{
    public delegate double PriceCalculator(double basePrice, double modifier);
    public delegate void OrderLogger(string message);

    public class PricingService
    {
        private readonly double _fixedBonus;

        public PricingService(double fixedBonus) => _fixedBonus = fixedBonus;

        public static double ApplyPercentageDiscount(double price, double percent)
        {
            double result = price * (1 - percent / 100.0);
            Console.WriteLine($"  -> [Static] Знижка {percent}%: {result}");
            return result;
        }

        public double ApplyLoyaltyDiscount(double price, double modifier)
        {
            double result = price - modifier - _fixedBonus;
            Console.WriteLine($"  -> [Instance] Бонус ({_fixedBonus} + {modifier}): {result}");
            return result;
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            var service = new PricingService(fixedBonus: 50);

            // 1–2. Створення екземплярів делегата трьома способами
            Console.WriteLine(" Три способи створення делегата");
            PriceCalculator staticDel = PricingService.ApplyPercentageDiscount;
            PriceCalculator instanceDel = service.ApplyLoyaltyDiscount;
            PriceCalculator lambdaDel = (price, mod) =>
            {
                double result = price - mod * 2;
                Console.WriteLine($"  -> [Lambda] Подвійна знижка ({mod * 2}): {result}");
                return result;
            };

            staticDel(1000, 10);
            instanceDel(1000, 10);
            lambdaDel(1000, 10);

            // 3. Безпечний виклик непризначеного делегата
            Console.WriteLine("\n Безпечний виклик");
            PriceCalculator? unassigned = null;
            double? safeResult = unassigned?.Invoke(1000, 10);
            Console.WriteLine($"Результат виклику null-делегата: {safeResult?.ToString() ?? "null (виклик пропущено)"}");

            // 4. Multicast-делегат типу void: додавання, порядок виклику, вилучення
            Console.WriteLine("\n  Multicast (void) ");
            OrderLogger? logger = LogToConsole;
            logger += LogToFileMock;
            logger += msg => Console.WriteLine($"  3. [Lambda-сповіщення] {msg}");

            Console.WriteLine("Виклик ланцюжка з 3 підписників:");
            logger?.Invoke("Замовлення #101 створено");

            logger -= LogToFileMock;
            Console.WriteLine("\nВиклик після вилучення другого підписника:");
            logger?.Invoke("Замовлення #101 оплачено");

            // 5. Multicast-делегат із значенням, що повертається
            Console.WriteLine("\n  Multicast  та GetInvocationList() ");
            PriceCalculator chain = staticDel;
            chain += instanceDel;
            chain += lambdaDel;

            Console.WriteLine("Прямий виклик усього ланцюжка:");
            // Виконуються всі методи, але повертається результат лише останнього підписника
            double directResult = chain(1000, 10);
            Console.WriteLine($"Прямий результат: {directResult}");

            Console.WriteLine("\nПоелементний виклик через GetInvocationList():");
            foreach (Delegate handler in chain.GetInvocationList())
            {
                var calc = (PriceCalculator)handler;
                double individualResult = calc(1000, 10);
                Console.WriteLine($"  Отримано значення: {individualResult}");
            }
        }

        private static void LogToConsole(string msg) => Console.WriteLine($"  1. [Console] {msg}");
        private static void LogToFileMock(string msg) => Console.WriteLine($"  2. [File] {msg}");
    }
}