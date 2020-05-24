using FleetManager.DbBase;
using FleetManager.Shared;
using System.Data.Common;
using System.Data.Entity.Infrastructure;

namespace MutticoFleet.Service.Interfaces
{
   public interface IDbContextHelper
    {
        BaseContext GetContext();
        DbParameter GetDbParameters(string parameterName, object value);
        bool? IsSaveDuplicateRecordError(DbUpdateException ex);
        bool? IsUpdateDuplicateRecordError(DbException ex);
        DbConnection GetDBConnection();
    }
}
