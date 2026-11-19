using System;
using System.Collections.Generic;

namespace Task_2
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            // 1–2. Лямбда-вирази різних форм із відповідними Action / Func
            Console.WriteLine("Форми лямбда-виразів ");

            Action greet = () => Console.WriteLine("  [Action, 0 параметрів, expression]: Старт обробки");
            Func<int, int> square = x => x * x;
            Func<double, double, double> powerSum = (a, b) => Math.Pow(a, 2) + Math.Pow(b, 2);
            Func<int, string> classify = n =>
            {
                if (n < 0) return "від'ємне";
                return n == 0 ? "нуль" : "додатне";
            };

            greet();
            Console.WriteLine($"  [Func<int, int>, 1 параметр, expression]: 7^2 = {square(7)}");
            Console.WriteLine($"  [Func<double, double, double>, 2 параметри, expression]: 3^2 + 4^2 = {powerSum(3, 4)}");
            Console.WriteLine($"  [Func<int, string>, 1 параметр, block body]: -5 -> {classify(-5)}");

            // 3. Фабрика делегатів із замиканням на параметри конфігурації
            Console.WriteLine("\n Фабрика делегатів (незалежні замикання) ");
            Func<double, double> standardVat = CreatePriceFormatter(taxRate: 0.20, deliveryFee: 50);
            Func<double, double> reducedVat = CreatePriceFormatter(taxRate: 0.07, deliveryFee: 0);

            Console.WriteLine($"  Товар 1000 грн (ПДВ 20% + доставка 50): {standardVat(1000)} грн");
            Console.WriteLine($"  Товар 1000 грн (ПДВ 7% без доставки):   {reducedVat(1000)} грн");

            // 4. Пастка замикання в циклі for
            Console.WriteLine("\n Пастка замикання в циклі ");
            var brokenActions = new List<Action>();
            for (int i = 1; i <= 3; i++)
            {
                
                brokenActions.Add(() => Console.Write($"{i} "));
            }

            Console.Write("  Некоректно (одна змінна циклу): ");
            foreach (var action in brokenActions) action(); 

            var fixedActions = new List<Action>();
            for (int i = 1; i <= 3; i++)
            {
                // Локальна копія створюється заново на кожній ітерації — кожна лямбда отримує свій об'єкт замикання
                int copy = i;
                fixedActions.Add(() => Console.Write($"{copy} "));
            }

            Console.Write("\n  Виправлено:    ");
            foreach (var action in fixedActions) action(); 
            Console.WriteLine();

            // 5. Спостереження зміни захопленого стану між викликами
            Console.WriteLine("\n Зміна захопленого стану між викликами ");
            int threshold = 10;
            // Делегат захоплює саму змінну threshold, а не її початкове значення 10
            Func<int, bool> isAboveThreshold = value => value > threshold;

            Console.WriteLine($"  threshold = {threshold} -> isAboveThreshold(15) = {isAboveThreshold(15)}");
            threshold = 20;
            Console.WriteLine($"  threshold = {threshold} -> isAboveThreshold(15) = {isAboveThreshold(15)}");

            Func<int> counter = CreateCounter(start: 100);
            Console.WriteLine($"  Лічильник (стан живе в купі навіть після виходу з CreateCounter): {counter()}, {counter()}, {counter()}");
        }

        private static Func<double, double> CreatePriceFormatter(double taxRate, double deliveryFee)
        {
            return basePrice => basePrice * (1 + taxRate) + deliveryFee;
        }

        private static Func<int> CreateCounter(int start)
        {
            int current = start;
            return () => ++current;
        }
    }
}