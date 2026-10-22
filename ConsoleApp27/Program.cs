using System;
using System.Globalization;
using System.Text;

namespace CinemaApp
{
    internal class Program
    {
        // Місткість сховища. Більше за Capacity сеансів програма не прийме.
        const int Capacity = 100;

        //  Сім паралельних масивів: елементи з індексом i разом описують один сеанс
        static int[] ids = new int[Capacity];               // код сеансу
        static string[] titles = new string[Capacity];      // назва фільму
        static int[] halls = new int[Capacity];             // номер залу
        static DateTime[] showTimes = new DateTime[Capacity]; // дата й час початку
        static decimal[] prices = new decimal[Capacity];    // ціна квитка, грн
        static int[] soldSeats = new int[Capacity];         // продано місць
        static int[] totalSeats = new int[Capacity];        // усього місць у залі

        static int count = 0;  // скільки перших комірок реально заповнено
        static int nextId = 1; // лічильник для автонумерації сеансів

        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Seed(); // демонстраційні дані, щоб не вводити все руками при кожному запуску

            bool running = true;
            while (running)
            {
                Console.WriteLine();
                Console.WriteLine("=== Кінотеатр: облік сеансів ===");
                Console.WriteLine("1 — додати сеанс");
                Console.WriteLine("2 — показати всі сеанси");
                Console.WriteLine("3 — знайти сеанс за кодом");
                Console.WriteLine("4 — видалити сеанс за кодом");
                Console.WriteLine("0 — вихід");
                Console.Write("Ваш вибір: ");
                string choice = (Console.ReadLine() ?? "").Trim();

                switch (choice)
                {
                    case "1": AddSession(); break;
                    case "2": PrintAll(); break;
                    case "3": FindById(); break;
                    case "4": DeleteById(); break;
                    case "0": running = false; break;
                    default: Console.WriteLine("Немає такого пункту меню."); break;
                }
            }
            Console.WriteLine("Роботу завершено.");
        }

        //  Запис у першу вільну комірку всіх семи масивів 
        static void Store(string title, int hall, DateTime showTime,
                          decimal price, int sold, int total)
        {
            ids[count] = nextId;
            titles[count] = title;
            halls[count] = hall;
            showTimes[count] = showTime;
            prices[count] = price;
            soldSeats[count] = sold;
            totalSeats[count] = total;
            count++;  // зайнятих комірок стало на одну більше
            nextId++; // наступний сеанс отримає інший код
        }

        // Демонстраційні дані 
        static void Seed()
        {
            Store("Інтерстеллар", 1, new DateTime(2026, 10, 22, 12, 0, 0), 150m, 48, 120);
            Store("Зоотрополіс", 2, new DateTime(2026, 10, 22, 15, 30, 0), 130m, 80, 100);
            Store("Гладіатор", 1, new DateTime(2026, 10, 22, 19, 0, 0), 190m, 96, 120);
        }

        // Пошук позиції сеансу за кодом: індекс або -1
        static int IndexOfId(int id)
        {
            for (int i = 0; i < count; i++) // тільки до count, а не до Capacity
            {
                if (ids[i] == id) return i;
            }
            return -1;
        }

        //  додати сеанс
        static void AddSession()
        {
            if (count >= Capacity)
            {
                Console.WriteLine($"Сховище заповнене: більше за {Capacity} сеансів не вміщується.");
                return;
            }

            Console.WriteLine("--- Новий сеанс ---");

            Console.Write("Назва фільму: ");
            string title = (Console.ReadLine() ?? "").Trim();
            if (title.Length == 0)
            {
                Console.WriteLine("Назва не може бути порожньою. Сеанс не додано.");
                return;
            }

            Console.Write("Номер залу (1–10): ");
            if (!int.TryParse(Console.ReadLine(), out int hall) || hall < 1 || hall > 10)
            {
                Console.WriteLine("Зал має бути цілим числом від 1 до 10. Сеанс не додано.");
                return;
            }

            Console.Write("Дата й час (дд.мм.рррр гг:хх): ");
            bool dateOk = DateTime.TryParseExact(Console.ReadLine(), "dd.MM.yyyy HH:mm",
                                                 CultureInfo.InvariantCulture,
                                                 DateTimeStyles.None, out DateTime showTime);
            if (!dateOk)
            {
                Console.WriteLine("Дату не розпізнано. Потрібен формат 22.10.2026 21:00. Сеанс не додано.");
                return;
            }

            Console.Write("Ціна квитка, грн: ");
            // Кому замінюємо на крапку, щоб приймати і 160,50, і 160.50
            string priceText = (Console.ReadLine() ?? "").Trim().Replace(',', '.');
            bool priceOk = decimal.TryParse(priceText, NumberStyles.Number,
                                            CultureInfo.InvariantCulture, out decimal price);
            if (!priceOk || price <= 0)
            {
                Console.WriteLine("Ціна має бути додатним числом. Сеанс не додано.");
                return;
            }

            Console.Write("Усього місць у залі: ");
            if (!int.TryParse(Console.ReadLine(), out int total) || total <= 0)
            {
                Console.WriteLine("Кількість місць має бути додатним цілим числом. Сеанс не додано.");
                return;
            }

            Console.Write("Продано місць: ");
            // Проданих місць не може бути більше, ніж усього
            if (!int.TryParse(Console.ReadLine(), out int sold) || sold < 0 || sold > total)
            {
                Console.WriteLine($"Продано має бути цілим числом від 0 до {total}. Сеанс не додано.");
                return;
            }

            Store(title, hall, showTime, price, sold, total);
            Console.WriteLine($"Додано сеанс №{ids[count - 1]} у позицію {count - 1}. " +
                              $"Зайнято комірок: {count} зі {Capacity}.");
        }

        // показати всі сеанси
        static void PrintAll()
        {
            if (count == 0)
            {
                Console.WriteLine("Сеансів поки немає.");
                return;
            }

            Console.WriteLine();
            Console.WriteLine(" Код | Фільм              | Зал | Дата й час       |     Ціна | Продано | Усього | Заповн.");
            Console.WriteLine("-----+--------------------+-----+------------------+----------+---------+--------+--------");

            for (int i = 0; i < count; i++) // до count
            {
                // Заповненість — обчислюване поле, у масивах його немає
                double percent = (double)soldSeats[i] / totalSeats[i] * 100;
                Console.WriteLine($"{ids[i],4} | {titles[i],-18} | {halls[i],3} | " +
                                  $"{showTimes[i]:dd.MM.yyyy HH:mm} | {prices[i],8:F2} | " +
                                  $"{soldSeats[i],7} | {totalSeats[i],6} | {percent,6:F1}%");
            }
            Console.WriteLine($"Усього сеансів: {count}.");

            // заповненість кожного залу у відсотках 
            Console.WriteLine();
            Console.WriteLine("Заповненість залів:");
            for (int i = 0; i < count; i++)
            {
                // Зал уже оброблено, якщо він зустрічався раніше в списку
                bool seen = false;
                for (int j = 0; j < i; j++)
                {
                    if (halls[j] == halls[i]) { seen = true; break; }
                }
                if (seen) continue;

                int sold = 0, total = 0;
                for (int k = 0; k < count; k++)
                {
                    if (halls[k] == halls[i])
                    {
                        sold += soldSeats[k];
                        total += totalSeats[k];
                    }
                }
                double hallPercent = (double)sold / total * 100;
                Console.WriteLine($"  Зал {halls[i]}: продано {sold} із {total} місць — {hallPercent:F1}%");
            }

            // : касовий збір за кожен день 
            Console.WriteLine();
            for (int i = 0; i < count; i++)
            {
                bool seen = false;
                for (int j = 0; j < i; j++)
                {
                    if (showTimes[j].Date == showTimes[i].Date) { seen = true; break; }
                }
                if (seen) continue;

                decimal dayTotal = 0;
                for (int k = 0; k < count; k++)
                {
                    if (showTimes[k].Date == showTimes[i].Date)
                    {
                        dayTotal += prices[k] * soldSeats[k];
                    }
                }
                Console.WriteLine($"Касовий збір за {showTimes[i]:dd.MM.yyyy}: {dayTotal:F2} грн.");
            }
        }

        //   знайти сеанс за кодом 
        static void FindById()
        {
            Console.Write("Код сеансу для пошуку: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("Код має бути цілим числом.");
                return;
            }

            int index = IndexOfId(id);
            if (index == -1)
            {
                Console.WriteLine($"Сеансу з кодом {id} у списку немає.");
                return;
            }

            double percent = (double)soldSeats[index] / totalSeats[index] * 100;
            Console.WriteLine($"Знайдено в позиції {index}:");
            Console.WriteLine($"  фільм:      {titles[index]}");
            Console.WriteLine($"  зал:        {halls[index]}");
            Console.WriteLine($"  початок:    {showTimes[index]:dd.MM.yyyy HH:mm}");
            Console.WriteLine($"  ціна:       {prices[index]:F2} грн");
            Console.WriteLine($"  продано:    {soldSeats[index]} із {totalSeats[index]} ({percent:F1}%)");
            Console.WriteLine($"  виторг:     {prices[index] * soldSeats[index]:F2} грн");
        }

        //  видалити сеанс за кодом (зсувом уліво) 
        static void DeleteById()
        {
            Console.Write("Код сеансу для видалення: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("Код має бути цілим числом.");
                return;
            }

            int index = IndexOfId(id);
            if (index == -1)
            {
                Console.WriteLine($"Сеансу з кодом {id} у списку немає.");
                return;
            }

            Console.WriteLine($"Видалено сеанс №{ids[index]} " +
                              $"({titles[index]}, зал {halls[index]}, {showTimes[index]:dd.MM.yyyy HH:mm}).");

            // Зсув уліво: кожен наступний елемент переїжджає на місце попереднього.
            // Робимо це одночасно в усіх семи масивах, інакше поля переплутаються.
            for (int i = index; i < count - 1; i++)
            {
                ids[i] = ids[i + 1];
                titles[i] = titles[i + 1];
                halls[i] = halls[i + 1];
                showTimes[i] = showTimes[i + 1];
                prices[i] = prices[i + 1];
                soldSeats[i] = soldSeats[i + 1];
                totalSeats[i] = totalSeats[i + 1];
            }
            count--; // остання комірка більше не входить у робочу частину

            // Прибираємо «хвіст», щоб у вивільненій комірці не лишалося старих даних
            ids[count] = 0;
            titles[count] = "";
            halls[count] = 0;
            showTimes[count] = default;
            prices[count] = 0m;
            soldSeats[count] = 0;
            totalSeats[count] = 0;

            Console.WriteLine($"Зайнято комірок: {count} зі {Capacity}.");
        }
    }
}