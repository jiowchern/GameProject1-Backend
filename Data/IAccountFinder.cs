using System;


using Regulus.Remoting;

namespace Regulus.Project.ItIsNotAGame1.Data
{
	public interface IAccountFinder
	{
		Value<Account> FindAccountByName(string id);

		Value<Account> FindAccountById(Guid accountId);
	}
}
