namespace InputLog.Core.IO.AnalysisXML.Input
{
    internal class PauseAnalysisReader : BasicXMLReader
    {
        public override ReadHeaderMethod ReadHeader()
        {
            return Header;
        }

        public override ReadExtraInfoMethod ReadExtraInfo()
        {
            return ExtraInfo;
        }

        public override ReadEventsMethod ReadEvents()
        {
            return NullEvents;
        }

        public override ReadModulesMethod ReadModules()
        {
            return Modules;
        }
    }
}