using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Guilded.Data.ViewModels.Core;

namespace Guilded.Data.DAL.Core
{
    public interface IPermissionsRepository
    {
        IEnumerable<Permission> Get();
    }
}
