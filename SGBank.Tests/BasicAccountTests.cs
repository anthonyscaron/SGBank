using NUnit.Framework;
using SGBank.BLL.DepositRules;
using SGBank.BLL.WithdrawRules;
using SGBank.Models;
using SGBank.Models.Interfaces;
using SGBank.Models.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SGBank.Tests
{
    [TestFixture]
    public class BasicAccountTests
    {
        [TestCase("33333", "Basic Account", 100, AccountType.Free, 250, false)] // fail, wrong account type
        [TestCase("33333", "Basic Account", 100, AccountType.Basic, -100, false)] // fail, negative number deposited
        [TestCase("33333", "Basic Account", 100, AccountType.Basic, 250, true)] // success
        public void BasicAccountDepositRuleTest(string accountNumber, string name, decimal balance,
            AccountType accountType, decimal amount, bool expectedResult)
        {
            IDeposit deposit = new NoLimitDepositRule();

            Account account = new Account();

            account.AccountNumber = accountNumber;
            account.Balance = balance;
            account.Name = name;
            account.Type = accountType;

            AccountDepositResponse response = deposit.Deposit(account, amount);

            Assert.AreEqual(expectedResult, response.Success);
        }

        [TestCase("33333", "Basic Account", 1500, AccountType.Basic, -1000, 1500, false)] // fail, too much withdrawn
        [TestCase("33333", "Basic Account", 100, AccountType.Free, -100, 100, false)] // fail, not a basic account type
        [TestCase("33333", "Basic Account", 100, AccountType.Basic, 100, 100, false)] // fail, positive number withdrawn
        [TestCase("33333", "Basic Account", 150, AccountType.Basic, -50, 100, true)] // success
        [TestCase("33333", "Basic Account", 100, AccountType.Basic, -150, -60, true)] // success, overdraft fee (newBalance = -60, 100-150-10[fee])
        public void BasicAccountWithdrawRuleTest(string accountNumber, string name, decimal balance,
            AccountType accountType, decimal amount, decimal NewBalance, bool expectedResult)
        {
            IWithdraw withdraw = new BasicAccountWithdrawRule();

            Account account = new Account();

            account.AccountNumber = accountNumber;
            account.Balance = balance;
            account.Name = name;
            account.Type = accountType;

            AccountWithdrawResponse response = withdraw.Withdraw(account, amount);

            Assert.AreEqual(expectedResult, response.Success);

            if (response.Success == true)
            {
                Assert.AreEqual(NewBalance, response.Account.Balance);
            }
        } 
    }
}
