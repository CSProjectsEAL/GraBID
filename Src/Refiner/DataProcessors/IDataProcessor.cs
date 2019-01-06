using Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace Refiner
{
    public interface IDataProcessor
    {
        dynamic Process(dynamic envelopeData);
    }
}
