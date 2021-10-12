using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace yapsi
{
    public interface IContract<T> : ICancelable<IContract<T>>, IDisposable
    {
        IPipeline<T> Pipeline { get; }

        void Publish(T packet);
    }
}
