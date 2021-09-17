namespace AES_TeamTool.Models
{
    public class ChangeManagement
    {
        public string ReleaseDate { get; set; }
        public ChangeType Type { get; set; }
        public string ItemName { get; set; }
        public string Profiles { get; set; }
        public string Description { get; set; }
        public string SpecAttached { get; set; }
        public string InvolvedModule { get; set; }
        public ImpactedPlant Plant { get; set; }
        public string Requester { get; set; }
        public string ImpactedTool { get; set; }
        public MES_Team Team { get; set; }
        public string Developer { get; set; }
        public string Comment { get; set; }
        public string Mark { get; set; }
        public string StableVersion { get; set; }
        public string ConfirmStatus { get; set; }
    }

    public enum ChangeType
    {
        DeeAction = 1,
        SystemPatch = 2,
        Tool = 3,
        Other = 9
    }

    public enum ImpactedPlant
    {
        CQ1 = 1,
        CQ2 = 2,
        CQ3 = 3,
        All = 4,
        Other = 9
    }

    public enum MES_Team
    {
        MES = 1,
        AES = 2,
        MCS = 3,
        DATA = 4,
        CMF = 5,
        Other = 9
    }
}