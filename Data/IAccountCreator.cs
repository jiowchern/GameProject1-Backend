using Regulus.Remoting;




namespace Regulus.Project.ItIsNotAGame1.Data
{
	public interface IAccountCreator
	{
		Value<ACCOUNT_REQUEST_RESULT> Create(Account account);
	}
}
