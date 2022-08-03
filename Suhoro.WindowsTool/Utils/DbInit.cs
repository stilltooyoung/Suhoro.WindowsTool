using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Suhoro.WindowsTool.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Suhoro.WindowsTool.Utils
{
    public class DbInit
    {
        public DbInit(LocalDb localDb)
        {
            var databaseCreator= localDb.Database.GetService<IDatabaseCreator>() as RelationalDatabaseCreator;
            databaseCreator.CreateTables();
        }
    }
}
