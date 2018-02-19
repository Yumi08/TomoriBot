using System.Collections.Generic;
using System.Linq;
using Discord.WebSocket;

namespace TomoriBot.Core.UserProfiles
{
	public static class UserAccounts
	{
		private static List<UserAccount> _accounts;

		private static string _accountsFile = "Resources/accounts.json";

		static UserAccounts()
		{
			if (DataStorage.SaveExists(_accountsFile))
			{
				_accounts = DataStorage.LoadUserAccounts(_accountsFile).ToList();
			}
			else
			{
				_accounts = new List<UserAccount>();
				SaveAccounts();
			}
		}

		public static void SaveAccounts()
		{
			DataStorage.SaveUserAccounts(_accounts, _accountsFile);
		}

		public static UserAccount GetAccount(SocketUser user)
		{
			return GetOrCreateAccount(user.Id);
		}

		private static UserAccount GetOrCreateAccount(ulong id)
		{
			var result = from a in _accounts
				where a.Id == id
				select a;

			var account = result.FirstOrDefault();
			if (account == null) account = CreateUserAccount(id);
			return account;
		}

		private static UserAccount CreateUserAccount(ulong id)
		{
			var newAccount = new UserAccount()
			{
				Id = id,
				Yen = 500,
				Experience = 0
			};

			_accounts.Add(newAccount);
			SaveAccounts();
			return newAccount;
		}
	}
}
