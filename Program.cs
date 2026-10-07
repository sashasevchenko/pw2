using System;

namespace WeatherAndBankingDemo
{
    using WeatherStationApp;
    using BankingApp;

    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.WriteLine("МЕТЕОСТАНЦІЯ");
            var station = new WeatherStation();
            var screen = new DisplayScreen();
            var phone = new PhoneAlertService();

            station.WeatherChanged += screen.OnWeatherChanged;
            station.WeatherChanged += phone.OnWeatherChanged;

            station.SetMeasurements(22.5, 65.0);
            station.SetMeasurements(24.0, 60.0);
            Console.WriteLine();

            Console.WriteLine("БАНКІВСЬКІ РАХУНКИ ТА ПРЕДИКАТИ");

            var client = new BankClient("C1001", "Користувач");

            Predicate<decimal> uahNotificationRule = amount => amount >= 500;

            Predicate<decimal> usdNotificationRule = amount => amount >= 100;

            var uahAccount = client.OpenAccount(Currency.UAH, 2000, uahNotificationRule);
            var usdAccount = client.OpenAccount(Currency.USD, 500, usdNotificationRule);
            var eurAccount = client.OpenAccount(Currency.EUR, 300);

            Console.WriteLine("\n--- Операції з поповнення ---");
            uahAccount.Deposit(1000);
            usdAccount.Deposit(200); 

            Console.WriteLine("\nОперації зі зняття коштів (Перевірка Predicate)");
            
            Console.WriteLine("-> Знімаємо 50 USD (ліміт сповіщення: 100 USD)...");
            usdAccount.Withdraw(50); 

            Console.WriteLine("-> Знімаємо 150 USD...");
            usdAccount.Withdraw(150);

            Console.WriteLine("-> Знімаємо 200 UAH (ліміт сповіщення: 500 UAH)...");
            uahAccount.Withdraw(200);

            Console.WriteLine("-> Знімаємо 600 UAH...");
            uahAccount.Withdraw(600);
        }
    }
}