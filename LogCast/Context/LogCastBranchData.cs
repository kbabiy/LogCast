namespace LogCast.Context
{
    internal struct LogCastBranchData
    {
        public int Id { get; }
        public int? ParentId { get; }

        public LogCastBranchData(int id, int? parentId)
        {
            Id = id;
            ParentId = parentId;
        }
    }
}
