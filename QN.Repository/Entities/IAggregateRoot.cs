using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;

namespace QN.Repository.Entities
{
    [InheritedExport(typeof(IAggregateRoot))]
    public interface IAggregateRoot
    {
    }
}
