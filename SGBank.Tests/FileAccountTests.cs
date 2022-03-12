using NUnit.Framework;
using SGBank.Data;
using SGBank.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SGBank.Tests
{
    [TestFixture]
    public class FileAccountTests
    {
        private string _filePath = @"C:\GithubRepos\Portfolio\SGBank\SGBank.UI\Data\AccountsTest.txt";
        private string _originalData = @"C:\GithubRepos\Portfolio\SGBank\SGBank.UI\Data\AccountsSeed.txt";

        [SetUp]
        public void Setup()
        {
            if(File.Exists(_filePath))
            {
                File.Delete(_filePath);
            }

            File.Copy(_originalData, _filePath);
        }

        [TestCase("11111", "Free Customer", 100, AccountType.Free, 0)] // success
        [TestCase("22222", "Basic Customer", 500, AccountType.Basic, 1)] //success
        [TestCase("33333", "Premium Customer", 1000, AccountType.Premium, 2)] // success
        public void CanReadDataFromFile(string accountNumber, string name, decimal balance,
            AccountType accountType, int index)
        {
            FileAccountRepository repository = new FileAccountRepository(_filePath);

            var accounts = repository.List();

            Assert.AreEqual(3, accounts.Count());

            Account check = accounts[index];

            Assert.AreEqual(accountNumber, check.AccountNumber);
            Assert.AreEqual(name, check.Name);
            Assert.AreEqual(balance, check.Balance);
            Assert.AreEqual(accountType, check.Type);
        }
    }
}
