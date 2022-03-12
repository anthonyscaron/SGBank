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
    public class PremiumAccountTests
    {
        [TestCase("44444", "Premium Account", 10000, AccountType.Free, 1000, false)] // fail, wrong account type
        [TestCase("44444", "Premium Account", 10000, AccountType.Premium, -1000, false)] // fail, negative number deposited
        [TestCase("44444", "Premium Account", 10000, AccountType.Premium, 1000, true)] // success
        public void PremiumAccountDepositRuleTest(string accountNumber, string name, decimal balance,
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

        [TestCase("44444", "Premium Account", 10000, AccountType.Free, -1000, 9000, false)] // fail, wrong account type
        [TestCase("44444", "Premium Account", 10000, AccountType.Premium, 1000, 9000, false)] // fail, positive number withdrawn
        [TestCase("44444", "Premium Account", 10000, AccountType.Premium, -1000, 9000, true)] // success
        [TestCase("44444", "Premium Account", 10000, AccountType.Premium, -11000, -1010, true)] // success, overdraft fee
        public void PremiumAccountWithdrawRuleTest(string accountNumber, string name, decimal balance,
            AccountType accountType, decimal amount, decimal NewBalance, bool expectedResult)
        {
            IWithdraw withdraw = new PremiumAccountWithdrawRule();

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
