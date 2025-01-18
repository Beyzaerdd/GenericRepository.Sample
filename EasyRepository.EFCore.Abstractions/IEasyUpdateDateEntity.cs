using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyRepository.EFCore.Abstractions
{
    public interface IEasyUpdateDateEntity
    {
        public DateTime? ModificationDate { get; set; }
    }
}
