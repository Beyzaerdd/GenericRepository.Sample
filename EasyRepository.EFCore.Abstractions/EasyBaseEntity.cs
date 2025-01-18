using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyRepository.EFCore.Abstractions
{
    public abstract class EasyBaseEntity<TPrimaryKey> : IEasyEntity<TPrimaryKey>, IEasyCreateDateEntity, IEasyUpdateDateEntity, IEasySoftDeleteEntity
    {

        public virtual DateTime CreationDate { get; set; }

        public virtual TPrimaryKey Id { get; set; }


        public virtual DateTime? ModificationDate { get; set; }

        public virtual DateTime? DeletionDate { get; set; }


        public virtual bool IsDeleted { get; set; }
    }
}