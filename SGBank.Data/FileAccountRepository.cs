using SGBank.Models;
using SGBank.Models.Interfaces;
using System.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SGBank.Data
{
    public class FileAccountRepository : IAccountRepository
    {
        private string _filePath;
        private List<Account> _accounts = new List<Account>();

        public FileAccountRepository(string filePath)
        {
            _filePath = filePath;
        }

        public List<Account> List()
        {
            using (StreamReader sr = new StreamReader(_filePath))
            {
                sr.ReadLine();
                string line;
                
                while((line = sr.ReadLine()) != null)
                {
                    Account newAccount = new Account();

                    string[] columns = line.Split(',');

                    newAccount.AccountNumber = columns[0];
                    newAccount.Name = columns[1];
                    newAccount.Balance = decimal.Parse(columns[2]);
                    
                    if (columns[3] == "F")
                    {
                        newAccount.Type = AccountType.Free;
                    }
                    else if (columns[3] == "B")
                    {
                        newAccount.Type = AccountType.Basic;
                    }
                    else if (columns[3] == "P")
                    {
                        newAccount.Type = AccountType.Premium;
                    }

                    _accounts.Add(newAccount);
                }
            }
            
            return _accounts;
        }

        public Account LoadAccount(string accountNumber)
        {
            _accounts = List();

            foreach (var a in _accounts)
            {
                if (a.AccountNumber == accountNumber)
                {
                    return a;
                }
            }

            return null;
        }

        public void SaveAccount(Account account)
        {
            foreach (var a in _accounts)
            {
                if (account.AccountNumber == a.AccountNumber)
                {
                    a.Name = account.Name;
                    a.Balance = account.Balance;
                    a.Type = account.Type;
                }
            }

            UpdateAccountsFile(_accounts);
        }

        private string CreateInfoForAccount(Account account)
        {
            string type = "";

            if (account.Type == AccountType.Basic)
            {
                type = "B";
            }
            else if (account.Type == AccountType.Free)
            {
                type = "F";
            }
            else if (account.Type == AccountType.Premium)
            {
                type = "P";
            }
            
            return string.Format("{0},{1},{2},{3}", account.AccountNumber, account.Name, 
                account.Balance, type);
        }
        
        private void UpdateAccountsFile(List<Account> _accounts)
        {
            if (File.Exists(_filePath))
            {
                File.Delete(_filePath);
            }

            using (StreamWriter sr = new StreamWriter(_filePath))
            {
                sr.WriteLine("AccountNumber,Name,Balance,Type");
                foreach(var a in _accounts)
                {
                    sr.WriteLine(CreateInfoForAccount(a));
                }
            }
        }
    }
}
