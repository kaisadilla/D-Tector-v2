namespace Kaisa.Digivice {
    public static class VisualDebug {
        private static DebugManager debugMgr;

        public static void Write(object output) => debugMgr.Write(output);
        public static void WriteLine(object output) => debugMgr.WriteLine(output);

        public static void SetDebugManager(DebugManager dbmgr) => debugMgr = dbmgr;
    }
}