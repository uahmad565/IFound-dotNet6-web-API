using IFoundBackend.Model.Enums;

namespace IFoundBackend.Areas.ToDTOs
{
    public class StatDto
    {
        public int? UserActiveLostCasesCount { get; set; }
        public int? UserActiveFoundCasesCount { get; set; }

        public int? UserUnresolvedCasesCount { get; set; }
        public int? UserResolvedCasesCount { get; set; }
        public int? AllActiveLostPostCount { get; set; }
        public int? AllActiveFoundPostCount { get; set; }
        public int? AllResolvedPostCount { get; set; }
        public int? AllUnResolvedPostCount { get; set; }

    }
}


//

//postManager.GetUserActiveCasesCount(targetType, id);
//postManager.GetUserUnresolvedCasesCount(id);
//postManager.GetUserResolvedCasesCount(id);
//postManager.GetActivePostCount(targetType);