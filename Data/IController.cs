using System;

namespace Regulus.Project.ItIsNotAGame1.Data
{
    public interface IController
    {
        Guid Id { get; }
        void Move(float angle);

        void Stop();
    }
}