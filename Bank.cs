using System;
using System.Collections.Generic;

namespace BankingApp
{
    public enum Currency
    {
        UAH, // Гривня
        USD, // Долар
        GBP, // Фунт
        EUR, // Євро
        JPY  // Японська єна
    }

    public class AccountTransactionEventArgs : EventArgs
    {
        public string AccountId { get; }
        public Currency Currency { get; }
        public decimal Amount { get; }
        public decimal NewBalance { get; }

        public AccountTransactionEventArgs(string accountId, Currency currency, decimal amount, decimal newBalance)
        {
            AccountId = accountId;
            Currency = currency;
            Amount = amount;
            NewBalance = newBalance;
        }
    }

    public class BankAccount
    {
        public string AccountId { get; }
        public Currency AccountCurrency { get; }
        public decimal Balance { get; private set; }

        private readonly Predicate<decimal> _withdrawalNotificationRule;

        public event EventHandler<AccountTransactionEventArgs>? Deposited;
        public event EventHandler<AccountTransactionEventArgs>? Withdrawn;

        public BankAccount(string accountId, Currency currency, decimal initialBalance = 0, Predicate<decimal>? withdrawalNotificationRule = null)
        {
            AccountId = accountId;
            AccountCurrency = currency;
            Balance = initialBalance;
            _withdrawalNotificationRule = withdrawalNotificationRule ?? (amount => amount > 0);
        }

        public void Deposit(decimal amount)
        {
            if (amount <= 0)
                throw new ArgumentException("Сума внесення повинна бути більшою за 0");

            Balance += amount;
            Deposited?.Invoke(this, new AccountTransactionEventArgs(AccountId, AccountCurrency, amount, Balance));
        }

        public bool Withdraw(decimal amount)
        {
            if (amount <= 0)
                throw new ArgumentException("Сума зняття повинна бути більшою за 0");

            if (amount > Balance)
            {
                Console.WriteLine($"[Помилка] Недостатньо коштів на рахунку {AccountId} ({AccountCurrency})!");
                return false;
            }

            Balance -= amount;

            if (_withdrawalNotificationRule(amount))
            {
                Withdrawn?.Invoke(this, new AccountTransactionEventArgs(AccountId, AccountCurrency, amount, Balance));
            }

            return true;
        }
    }

    public class BankClient
    {
        public string Id { get; }
        public string Name { get; }
        
        public List<BankAccount> Accounts { get; } = new List<BankAccount>();

        public BankClient(string id, string name)
        {
            Id = id;
            Name = name;
        }

        public BankAccount OpenAccount(Currency currency, decimal initialBalance = 0, Predicate<decimal>? withdrawalRule = null)
        {
            string accountId = $"ACC-{currency}-{Accounts.Count + 1:D3}";
            var account = new BankAccount(accountId, currency, initialBalance, withdrawalRule);

            account.Deposited += OnAccountDeposited;
            account.Withdrawn += OnAccountWithdrawn;

            Accounts.Add(account);
            Console.WriteLine($"Клієнту {Name} відкрито рахунок {accountId} у валюті {currency}.");
            return account;
        }

        private void OnAccountDeposited(object? sender, AccountTransactionEventArgs e)
        {
            Console.WriteLine($"[Сповіщення для {Name}] Поповнення рахунку {e.AccountId}: +{e.Amount} {e.Currency}. Поточний баланс: {e.NewBalance} {e.Currency}");
        }

        private void OnAccountWithdrawn(object? sender, AccountTransactionEventArgs e)
        {
            Console.WriteLine($"[Сповіщення для {Name}] Зняття з рахунку {e.AccountId}: -{e.Amount} {e.Currency}. Поточний баланс: {e.NewBalance} {e.Currency}");
        }
    }
}