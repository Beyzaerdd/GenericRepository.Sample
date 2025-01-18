using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyRepository.EFCore.Abstractions
{
    public interface IEasySoftDeleteEntity
    {
        public DateTime? DeletionDate { get; set; }


        public bool IsDeleted { get; set; }
    }
}
