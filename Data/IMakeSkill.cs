using System;

namespace Regulus.Project.ItIsNotAGame1.Data
{
    public interface IMakeSkill
    {

        void Create(string formula , int[] amounts);

        void Exit();

        void QueryFormula();

        event Action<ItemFormulaLite[]> FormulasEvent;
    }
}