namespace InputLog.Core.IO.AnalysisXML.Input
{
    internal class GeneralAnalysisReader : BasicXMLReader
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
            return Events;
        }

        public override ReadModulesMethod ReadModules()
        {
            return NullModules;
        }
    }
}