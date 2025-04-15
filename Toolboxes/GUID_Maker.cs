using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Planetside.Toolboxes
{
    public static class GUID_Maker
    {
        public static string GenerateNewGUID(this string Tag)
        {
            var guid = $"[PSOG]{System.Guid.NewGuid()}";
            //ETGModConsole.Log(guid);
            return guid;
        }
        public static string GenerateNewGUID()
        {
            var guid = $"[PSOG]{System.Guid.NewGuid()}";
            //ETGModConsole.Log(guid);
            return guid;
        }
    }
}
