using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InputLog.Core.IO.AnalysisXML.Input
{
    public class ReportingXMLReader: BasicXMLReader
    {
        public override ReadEventsMethod ReadEvents()
        {
            return NullEvents;
        }
        public override ReadExtraInfoMethod ReadExtraInfo()
        {
            return NullExtraInfo;
        }

        public override ReadHeaderMethod ReadHeader()
        {
            return NullHeader;
        }

        public override ReadModulesMethod ReadModules()
        {
            return Modules;
        }
    }
}
