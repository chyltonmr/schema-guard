using System;
using System.Collections.Generic;
using System.Text;

namespace schema_guard
{
    public class AppSettingsSchemaGuard
    {
        public string Title { get; set; }
        public string Version { get; set; }
        public string ConnectionString { get; set; }
    }
}
