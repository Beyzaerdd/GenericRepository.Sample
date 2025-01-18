using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyRepository.EFCore.Abstractions
{
    internal interface IEasyEntity<TPrimaryKey>
    {
        TPrimaryKey Id { get; set; }
    }
}
