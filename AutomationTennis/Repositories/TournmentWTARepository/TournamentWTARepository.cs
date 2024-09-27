using AutomationTennis.Context;
using AutomationTennis.Domain;
using AutomationTennis.Repositories.RepositoryBase;

namespace AutomationTennis.Repositories.TournamentWTARepository
{
    public class TournamentWTARepository : RepositoryBase<TournamentWTA>, ITournamentWTARepository
    {
        public TournamentWTARepository(AutomationTennisContext context)
            : base(context)
        {
        }

    }
}
