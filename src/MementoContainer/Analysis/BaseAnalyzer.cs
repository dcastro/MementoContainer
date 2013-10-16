using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MementoContainer.Analysis
{
    internal class BaseAnalyzer
    {
        protected bool GetCascade(Attribute attr)
        {
            return ((MementoClassAttribute)attr).Cascade;
        }
    }
}
