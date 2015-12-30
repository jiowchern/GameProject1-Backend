using System.Collections.Generic;
using System.Runtime.CompilerServices;

using Regulus.CustomType;

namespace Regulus.Project.ItIsNotAGame1.Data
{
    public class Determination
    {
        private readonly Vector2[] _Lefts;

        private readonly Vector2[] _Rights;

        private readonly float _Total;

        private readonly float _Begin;

        private readonly float _End;

        private readonly float _Lengeh;
        
        public Determination(Regulus.CustomType.Vector2[] lefts,
            Regulus.CustomType.Vector2[] rights,
            float total, float begin, float end)
        {
            _Lefts = lefts;
            _Rights = rights;
            _Total = total;
            _Begin = begin;
            _End = end;

            _Lengeh = _End - _Begin;
        }

        public Determination(SkillData skill) :this(skill.Lefts , skill.Rights , skill.Total , skill.Begin , skill.End)
        {            
        }

        public Regulus.CustomType.Polygon Find(float begin, float length)
        {
            if (begin > _End)
                return null;
            if (begin + length < _Begin)
                return null;
            var end = begin + length;
            end = end > _End ? _End : end;
            begin = begin < _Begin
                ? _Begin
                : begin;

            return _Capture(begin - _Begin, end - _Begin);
        }

        private Polygon _Capture(float begin, float end)
        {
            if (_Lefts.Length == 0 || _Rights.Length == 0)
                return null;
            var startScale = begin / _Lengeh;
            var endScale = end / _Lengeh;

            var startLeftIndex = startScale * (_Lefts.Length - 1);
            var endLeftIndex = endScale * (_Lefts.Length - 1);
            var startLeft = _Lefts[(int)startLeftIndex];
            var endLeft = _Lefts[(int)endLeftIndex];


            var startRightIndex = startScale * (_Rights.Length - 1);
            var endRightIndex = endScale * (_Rights.Length - 1);
            var startRight = _Rights[(int)startRightIndex];
            var endRight = _Rights[(int)endRightIndex];
            var hull = AForge.Math.Geometry.GrahamConvexHull.FindHull(
                new List<Vector2>(
                    new[]
                    {
                    startLeft,endLeft,
                    startRight,endRight
                    }));
            return new Polygon(hull.ToArray());
        }
    }
}