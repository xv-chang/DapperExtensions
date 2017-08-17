using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace DapperExtensions
{
    public class InsertBuilder<T>
    {
        private IDbConnection _conn;
       
        public InsertBuilder(IDbConnection conn,T entity)
        {
            _conn = conn;
        }

        public int Execute()
        {
            throw new NotImplementedException();
        }

    }
}
