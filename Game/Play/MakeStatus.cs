
using System;
using System.Linq;

using Regulus.Project.ItIsNotAGame1.Data;
using Regulus.Remoting;
using Regulus.Utility;

namespace Regulus.Project.ItIsNotAGame1.Game.Play
{
    internal class MakeStatus : IStage , IMakeSkill
    {
        private readonly ISoulBinder _Binder;

        private readonly Entity _Player;

        public event Action DoneEvent;

        private ItemFormula[] _Formulas;
        
        

        public MakeStatus(ISoulBinder binder, Entity player)
        {            
            _Binder = binder;   
            _Player = player;            
        }

        void IStage.Enter()
        {
            _Player.Make();
            _Formulas = _Player.GetFormulas();
            
            _Binder.Bind<IMakeSkill>(this);
            
        }

        void IStage.Leave()
        {
            _Binder.Unbind<IMakeSkill>(this);
        }

        void IStage.Update()
        {
            
        }

        private ItemFormulaLite[] _BuildLite(ItemFormula[] formulas)
        {
            return (from f in formulas
                    select new ItemFormulaLite
                    {
                        Item = f.Item,
                        Name = f.Name,
                        NeedItems = (from need in f.NeedItems
                                     select new ItemFormulaNeedLite
                                     {
                                         Item = need.Item,
                                         Min = need.Min
                                     }).ToArray()
                    }).ToArray();
        }

        void IMakeSkill.Create(string name, int[] amounts)
        {
            var formula = (from f in _Formulas where f.Name == name select f).FirstOrDefault();
            if (formula == null)
                return;
            var needItems = formula.NeedItems;
            if(needItems.Length != amounts.Length)
                return;            

            for (int i = 0; i < amounts.Length  ; ++i)
            {
                if(needItems[i].Min > amounts[i])
                    return;
            }

            for(int i = 0; i < needItems.Length; i++)
            {
                var amount = _Player.Bag.GetItemAmount(needItems[i].Item);
                if(amount < amounts[i])
                    return;

                
            }
            for (int i = 0; i < needItems.Length; i++)
                _Player.Bag.Remove(needItems[i].Item, amounts[i]);


            _Create(formula , amounts);
        }

        private void _Create(ItemFormula key , int[] amounts)
        {
            var items = key.NeedItems;

            var total1 = items.Sum(i => i.Max);
            var itemScales1 = (from i in items
                               select new
                               {
                                   Item = i,
                                   Value = i.Max,
                                   Scale = i.Max / (float)total1
                               }).ToArray();

            var total2 = amounts.Sum();
            var itemScales2 = (from i in amounts
                               select new
                               {
                                   Value = i,
                                   Scale = i / (float)total2
                               }).ToArray();

            var maxScale = 0.0f;
            for (int i = 0; i < itemScales2.Length && i < itemScales1.Length; i++)
            {
                var scale1 = itemScales1[i].Scale;
                var scale2 = itemScales2[i].Scale;
                var ms = scale2 / scale1;
                if (ms > maxScale)
                    maxScale = ms;
            }

           
            var quality = 0.0f;
            for (int i = 0; i < itemScales2.Length && i < itemScales1.Length; i++)
            {
                quality += itemScales1[i].Scale * (itemScales2[i].Scale / itemScales1[i].Scale / maxScale);
            }

            var itemProvider = new ItemProvider();
            var item = itemProvider.BuildItem(quality , key.Item , key.Effects  );
            _Player.Bag.Add(item);

        }

        void IMakeSkill.Exit()
        {
            DoneEvent();
        }

        void IMakeSkill.QueryFormula()
        {
            _FormulasEvent(_BuildLite(_Formulas));
        }

        private event Action<ItemFormulaLite[]> _FormulasEvent;
        event Action<ItemFormulaLite[]> IMakeSkill.FormulasEvent
        {
            add { _FormulasEvent += value; }
            remove { _FormulasEvent -= value; }
        }
    }
}