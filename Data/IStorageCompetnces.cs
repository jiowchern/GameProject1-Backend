using System;


using Regulus.Remoting;




namespace Regulus.Project.ItIsNotAGame1.Data
{
	public interface IStorageCompetences
	{
		Value<Account.COMPETENCE[]> Query();

		Value<Guid> QueryForId();
	}
}
