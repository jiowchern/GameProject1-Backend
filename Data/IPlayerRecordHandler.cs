using System;


using Regulus.Remoting;




namespace Regulus.Project.ItIsNotAGame1.Data
{
	public interface IGameRecorder
	{
		Value<GamePlayerRecord> Load(Guid account_id);

		void Save(GamePlayerRecord game_player_record);
	}
}
